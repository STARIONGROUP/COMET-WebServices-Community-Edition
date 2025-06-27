﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryService.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

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
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="iteration"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <returns>The <see cref="ReferenceDataLibrary"/></returns>
        public async Task<IEnumerable<ReferenceDataLibrary>> QueryReferenceDataLibraryAsync(NpgsqlTransaction transaction, Iteration iteration)
        {
            var engineeringModelSetup = (await this.EngineeringModelSetupDao.ReadAsync(transaction, nameof(SiteDirectory), null, true)).FirstOrDefault(ems => ems.IterationSetup.Contains(iteration.IterationSetup));

            if (engineeringModelSetup == null)
            {
                throw new InvalidOperationException($"Could not find the associated EngineeringModelSetup for Iteration {iteration.Iid}");
            }

            var mrdl = (await this.ModelReferenceDataLibraryDao.ReadAsync(transaction, nameof(SiteDirectory), engineeringModelSetup.RequiredRdl, true)).FirstOrDefault();

            if (mrdl == null)
            {
                throw new InvalidOperationException($"Could not find the associated ModelReferenceDataLibrary for Iteration {iteration.Iid}");
            }

            var requiredRdls = new List<ReferenceDataLibrary> { mrdl };
            TryCopyToRequiredRdls(await this.GetRequiredRdlAsync(transaction, mrdl), requiredRdls);

            return requiredRdls;
        }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="rdl"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="rdl">The <see cref="ReferenceDataLibrary"/></param>
        /// <returns>The required <see cref="ReferenceDataLibrary"/></returns>
        private async Task<ReadOnlyCollection<ReferenceDataLibrary>> GetRequiredRdlAsync(NpgsqlTransaction transaction, ReferenceDataLibrary rdl)
        {
            var requiredRdls = new List<ReferenceDataLibrary>();

            if (!rdl.RequiredRdl.HasValue)
            {
                return requiredRdls.AsReadOnly();
            }

            var requiredRdl =
                (await this.ModelReferenceDataLibraryDao.ReadAsync(transaction, nameof(SiteDirectory), [rdl.RequiredRdl.Value], true)).FirstOrDefault() as ReferenceDataLibrary
                ?? (await this.SiteReferenceDataLibraryDao.ReadAsync(transaction, nameof(SiteDirectory), [rdl.RequiredRdl.Value], true)).FirstOrDefault();

            if (requiredRdl != null)
            {
                TryCopyToRequiredRdls([requiredRdl], requiredRdls);
                TryCopyToRequiredRdls(await this.GetRequiredRdlAsync(transaction, requiredRdl), requiredRdls);
            }

            return requiredRdls.AsReadOnly();
        }

        /// <summary>
        /// Tries to copy inidividual <see cref="ReferenceDataLibrary"/>s from an <see cref="IEnumerable{ReferenceDataLibrary}"/> to an <see cref="ICollection{ReferenceDataLibrary}"/>
        /// </summary>
        /// <param name="source">The source <see cref="IEnumerable{ReferenceDataLibrary}"/></param>
        /// <param name="target">The target <see cref="List{ReferenceDataLibrary}"/></param>
        private static void TryCopyToRequiredRdls(IEnumerable<ReferenceDataLibrary> source, List<ReferenceDataLibrary> target)
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
