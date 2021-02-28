// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionResolver.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Orm.Dao.Revision;

    using Npgsql;

    /// <summary>
    /// From the webservices' perspective, the <see cref="IQueryParameters"/> class can contain <see cref="int"/>, or <see cref="DateTime"/> type values for the revisionFrom, and revisionTo query parameters.
    /// For the <see cref="DateTime"/> type a call has to be made to the datastore, to get the closest revision (before, or after) the specified DateTime value.
    /// </summary>
    public class RevisionResolver : IRevisionResolver
    {
        /// <summary>
        /// The injected instance of <see cref="IRevisionDao"/> that is used to collect RevisionRegistry data
        /// </summary>
        public IRevisionDao RevisionDao { get; set; }

        /// <summary>
        /// Try to resolve the revision numbers that belong to the revisionFrom and revisionTo parameters.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revisionFrom"><see cref="int"/> or <see cref="DateTime"/> type parameter that indicates the From revision number, or timestamp.</param>
        /// <param name="revisionTo"><see cref="int"/> or <see cref="DateTime"/> type parameter that indicates the To revision number, or timestamp.</param>
        /// <param name="resolvedValues">A <see cref="ValueTuple"/> containing the resolved From revision number and To revision number.</param>
        /// <returns>True is revision numbers have been resolved, otherwise false</returns>
        public bool TryResolve(NpgsqlTransaction transaction, string partition, object revisionFrom, object revisionTo, out (int FromRevision, int ToRevision, IEnumerable<RevisionRegistryInfo> RevisionRegistryInfoList) resolvedValues)
        {
            var resolvedRevisionFrom = 0;
            var resolvedRevisionTo = int.MaxValue;

            if (revisionFrom == null && revisionTo == null)
            {
                resolvedValues = default((int, int, IEnumerable<RevisionRegistryInfo> RevisionRegistryInfoList));
                return false;
            }

            var revisions = this.RevisionDao.ReadRevisionRegistry(transaction, partition).ToList();

            if (revisions.Any())
            {
                resolvedRevisionFrom = revisions.OrderBy(x => x.Revision).First().Revision;
                resolvedRevisionTo = revisions.OrderBy(x => x.Revision).Last().Revision;
            }

            if (revisionFrom is int fromAsInt)
            {
                resolvedRevisionFrom = revisions.OrderBy(x => x.Revision).FirstOrDefault(x => x.Revision >= fromAsInt)?.Revision ?? resolvedRevisionFrom;
            }
            else if (revisionFrom is DateTime fromAsDateTime)
            {
                resolvedRevisionFrom = revisions.OrderBy(x => x.Instant).FirstOrDefault(x => x.Instant >= fromAsDateTime)?.Revision ?? resolvedRevisionFrom;
            }

            if (revisionTo is int toAsInt)
            {
                resolvedRevisionTo = revisions.OrderBy(x => x.Revision).LastOrDefault(x => x.Revision <= toAsInt)?.Revision ?? resolvedRevisionTo;
            }
            else if (revisionTo is DateTime toAsDateTime)
            {
                resolvedRevisionTo = revisions.OrderBy(x => x.Instant).LastOrDefault(x => x.Instant <= toAsDateTime)?.Revision ?? resolvedRevisionTo;
            }

            resolvedValues = (resolvedRevisionFrom, resolvedRevisionTo, revisions.Where(x => x.Revision >= resolvedRevisionFrom && x.Revision <= resolvedRevisionTo));

            return true;
        }
    }
}