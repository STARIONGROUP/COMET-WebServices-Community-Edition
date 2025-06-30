// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionResolver.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
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
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Orm.Dao.Revision;

    using CometServer.Services.Protocol;

    using FluentResults;

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
        /// <returns>An awaitable <see cref="Task "/> having result of True if revision numbers have been resolved, otherwise false</returns>
        public async Task<Result<RevisionResolveResult>> TryResolve(NpgsqlTransaction transaction, string partition, object revisionFrom, object revisionTo)
        {
            var resolvedRevisionFrom = 0;
            var resolvedRevisionTo = int.MaxValue;

            if (revisionFrom == null && revisionTo == null)
            {
                return Result.Fail("No revisions to search for");
            }

            var revisions = await this.RevisionDao.ReadRevisionRegistryAsync(transaction, partition);

            if (revisions.Count != 0)
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

            return Result.Ok(new RevisionResolveResult(resolvedRevisionFrom, resolvedRevisionTo, revisions.Where(x => x.Revision >= resolvedRevisionFrom && x.Revision <= resolvedRevisionTo)));
        }
    }
}