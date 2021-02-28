// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryService.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="ModelReferenceDataLibraryService"/>
    /// </summary>
    public partial class ModelReferenceDataLibraryService
    {
        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryDao"/>
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDato { get; set; }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="iteration"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <returns>The <see cref="ReferenceDataLibrary"/></returns>
        public IEnumerable<ReferenceDataLibrary> QueryReferenceDataLibrary(NpgsqlTransaction transaction, Iteration iteration)
        {
            var engineeringModelSetup = this.EngineeringModelSetupDao.Read(transaction, nameof(SiteDirectory)).FirstOrDefault(ems => ems.IterationSetup.Contains(iteration.IterationSetup));

            if (engineeringModelSetup == null)
            {
                throw new InvalidOperationException($"Could not find the associated engineering-modem-setup for iteration {iteration.Iid}");
            }

            var mrdl = this.ModelReferenceDataLibraryDao.Read(transaction, nameof(SiteDirectory), engineeringModelSetup.RequiredRdl).FirstOrDefault();

            if (mrdl == null)
            {
                throw new InvalidOperationException($"Could not find the associated reference-data-library for iteration {iteration.Iid}");
            }

            var requiredRdls = new List<ReferenceDataLibrary> { mrdl };
            this.TryCopyToRequiredRdls(this.GetRequiredRdl(transaction, mrdl), requiredRdls);

            return requiredRdls;
        }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="rdl"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="rdl">The <see cref="ReferenceDataLibrary"/></param>
        /// <returns>The required <see cref="ReferenceDataLibrary"/></returns>
        private IReadOnlyList<ReferenceDataLibrary> GetRequiredRdl(NpgsqlTransaction transaction, ReferenceDataLibrary rdl)
        {
            var requiredRdls = new List<ReferenceDataLibrary>();

            if (!rdl.RequiredRdl.HasValue)
            {
                return requiredRdls;
            }

            var requiredRdl =
                this.ModelReferenceDataLibraryDao.Read(transaction, nameof(SiteDirectory), new[] { rdl.RequiredRdl.Value }).FirstOrDefault() as ReferenceDataLibrary
                ?? this.SiteReferenceDataLibraryDato.Read(transaction, nameof(SiteDirectory), new[] { rdl.RequiredRdl.Value }).FirstOrDefault();

            if (requiredRdl != null)
            {
                this.TryCopyToRequiredRdls(new[] { requiredRdl }, requiredRdls);
                this.TryCopyToRequiredRdls(this.GetRequiredRdl(transaction, requiredRdl), requiredRdls);
            }

            return requiredRdls;
        }

        /// <summary>
        /// Tries to copy inidividual <see cref="ReferenceDataLibrary"/>s from an <see cref="IEnumerable{ReferenceDataLibrary}"/> to an <see cref="ICollection{ReferenceDataLibrary}"/>
        /// </summary>
        /// <param name="source">The source <see cref="IEnumerable{ReferenceDataLibrary}"/></param>
        /// <param name="target">The target <see cref="ICollection{ReferenceDataLibrary}"/></param>
        private void TryCopyToRequiredRdls(IEnumerable<ReferenceDataLibrary> source, ICollection<ReferenceDataLibrary> target)
        {
            foreach (var possibleAdditionalRdl in source)
            {
                if (target.All(x => x.Iid != possibleAdditionalRdl.Iid))
                {
                    target.Add(possibleAdditionalRdl);
                }
            }
        }
    }
}
