// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChainOfRdlComputationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IChainOfRdlComputationService"/> is to compute and return the chain of rdls for a
    /// provided list of <see cref="EngineeringModelSetup"/>s
    /// </summary>
    /// <remarks>
    /// This service is a per request service and is not thread safe
    /// </remarks>
    public class ChainOfRdlComputationService : IChainOfRdlComputationService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<ChainOfRdlComputationService> Logger { get; set; }

        /// <summary>
        /// The site directory data.
        /// </summary>
        private const string SiteDirectoryPartition = "SiteDirectory";

        /// <summary>
        /// Gets or sets the (injected) <see cref="IModelReferenceDataLibraryDao"/> used to query the <see cref="ModelReferenceDataLibrary"/> objects from the datastore
        /// </summary>
        public IModelReferenceDataLibraryDao ModelReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ISiteReferenceDataLibraryDao"/> used to query the <see cref="SiteReferenceDataLibrary"/> objects from the datastore
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// The cached list of <see cref="ModelReferenceDataLibrary"/> used to make sure that in the context of one HTTP request the datastore
        /// needs to be queried only once
        /// </summary>
        private List<ModelReferenceDataLibrary> cachedModelReferenceDataLibraries;

        /// <summary>
        /// The cached list of <see cref="SiteReferenceDataLibrary"/> used to make sure that in the context of one HTTP request the datastore
        /// needs to be queried only once
        /// </summary>
        private List<SiteReferenceDataLibrary> cachedSiteReferenceDataLibraries;

        /// <summary>
        /// Gets the identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> dependencies for a list of <see cref="EngineeringModelSetup"/>s
        /// </summary>
        /// <param name="engineeringModelSetups">
        /// The <see cref="EngineeringModelSetup"/> for which the unique identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> needs to be determined
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the datastore
        /// </param>
        /// <returns>
        /// The unique identifiers of the <see cref="SiteReferenceDataLibrary"/> dependency of the provided <see cref="IEnumerable{EngineeringModelSetup}"/>
        /// </returns>
        public IEnumerable<Guid> QueryReferenceDataLibraryDependency(NpgsqlTransaction transaction, IEnumerable<EngineeringModelSetup> engineeringModelSetups)
        {
            if (this.cachedModelReferenceDataLibraries == null || this.cachedModelReferenceDataLibraries.Count == 0)
            {
                this.Logger.LogDebug("Retrieving the ModelReferenceDataLibrary objects from the cached tables in the datastore");

                var modelReferenceDataLibraries = this.ModelReferenceDataLibraryDao.Read(transaction, SiteDirectoryPartition, null, true);

                this.cachedModelReferenceDataLibraries = new List<ModelReferenceDataLibrary>();
                
                this.cachedModelReferenceDataLibraries.AddRange(modelReferenceDataLibraries);
            }

            if (this.cachedSiteReferenceDataLibraries == null || this.cachedSiteReferenceDataLibraries.Count == 0)
            {
                this.Logger.LogDebug("Retrieving the SiteReferenceDataLibrary objects from the cached tables in the datastore");

                var siteReferenceDataLibraries = this.SiteReferenceDataLibraryDao.Read(transaction, SiteDirectoryPartition, null, true);

                this.cachedSiteReferenceDataLibraries = new List<SiteReferenceDataLibrary>();

                this.cachedSiteReferenceDataLibraries.AddRange(siteReferenceDataLibraries);
            }

            var result = new HashSet<Guid>();

            foreach (var engineeringModelSetup in engineeringModelSetups)
            {
                if (engineeringModelSetup.RequiredRdl.Count > 1)
                {
                    this.Logger.LogWarning($"The EngineeringModelSetup { engineeringModelSetup.Iid } has more than 1 required rdl, this is not allowed, this EngineeringModelSetup is ignored");
                    continue;
                }

                var modelReferenceDataLibararyIid = engineeringModelSetup.RequiredRdl.SingleOrDefault();
                if (modelReferenceDataLibararyIid == Guid.Empty)
                {
                    this.Logger.LogWarning($"The EngineeringModelSetup { engineeringModelSetup.Iid } does not have a required rdl, this is not allowed, this EngineeringModelSetup is ignored");
                    continue;
                }

                var modelReferenceDataLibarary = this.cachedModelReferenceDataLibraries.SingleOrDefault(x => x.Iid == modelReferenceDataLibararyIid);
                if (modelReferenceDataLibarary == null)
                { 
                    this.Logger.LogWarning($"The ModelReferenceDataLibarary { modelReferenceDataLibararyIid } could not be found, there is a fault in the data, the EngineeringModelSetup {engineeringModelSetup.Iid} is ignored");
                    continue;
                }
                else
                {
                    this.QueryRequiredReferenceDataLibraryChainAndUpdateResult(modelReferenceDataLibarary, result);
                }
            }

            return result;
        }

        /// <summary>
        /// recursively queries the chain of rdls for a provided <see cref="ReferenceDataLibrary"/> and adds the found <see cref="SiteReferenceDataLibrary"/> to the
        /// resulting <paramref name="siteReferenceDataLibraryUniqueIdentifiers"/>
        /// </summary>
        /// <param name="referenceDataLibrary">
        /// The <see cref="ReferenceDataLibrary"/> for which the chain-of-rdl is to be computed
        /// </param>
        /// <param name="siteReferenceDataLibraryUniqueIdentifiers">
        /// A <see cref="HashSet{Guid}"/> to which the 
        /// </param>
        private void QueryRequiredReferenceDataLibraryChainAndUpdateResult(ReferenceDataLibrary referenceDataLibrary, HashSet<Guid> siteReferenceDataLibraryUniqueIdentifiers)
        {
            // the referenceDataLibrary does not have a requiredRdl, no need to continue
            if (!referenceDataLibrary.RequiredRdl.HasValue)
            {
                return;
            }

            // the required RDL of the referenceDataLibrary has already been found in a previous iteration, no need to redo it
            if (siteReferenceDataLibraryUniqueIdentifiers.Contains(referenceDataLibrary.RequiredRdl.Value))
            {
                return;
            }

            // iterate through the cached SiteReferenceDataLibrary objects untill we find a matching required RDL.
            // Add this matched object to the resultset and look for it's required rdl's with a recursive call
            var siteReferenceDataLibrary = this.cachedSiteReferenceDataLibraries.FirstOrDefault(x => x.Iid == referenceDataLibrary.RequiredRdl);
            if (siteReferenceDataLibrary != null)
            {
                siteReferenceDataLibraryUniqueIdentifiers.Add(siteReferenceDataLibrary.Iid);

                this.QueryRequiredReferenceDataLibraryChainAndUpdateResult(siteReferenceDataLibrary, siteReferenceDataLibraryUniqueIdentifiers);
            }
        }
    }
}
