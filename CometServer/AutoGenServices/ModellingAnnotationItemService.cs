// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModellingAnnotationItemService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The <see cref="ModellingAnnotationItem"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ModellingAnnotationItemService : ServiceBase, IModellingAnnotationItemService
    {
        /// <summary>
        /// Gets or sets the <see cref="IActionItemService"/>.
        /// </summary>
        public IActionItemService ActionItemService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IChangeProposalService"/>.
        /// </summary>
        public IChangeProposalService ChangeProposalService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IContractChangeNoticeService"/>.
        /// </summary>
        public IContractChangeNoticeService ContractChangeNoticeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IContractDeviationService"/>.
        /// </summary>
        public IContractDeviationService ContractDeviationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReviewItemDiscrepancyService"/>.
        /// </summary>
        public IReviewItemDiscrepancyService ReviewItemDiscrepancyService { get; set; }

        /// <summary>
        /// Get the requested <see cref="ModellingAnnotationItem"/>s from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="ModellingAnnotationItem"/>, optionally with contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? await this.GetDeepAsync(transaction, partition, ids, containerSecurityContext)
                        : await this.GetShallowAsync(transaction, partition, ids, containerSecurityContext);
        }

        /// <summary>
        /// Add the supplied value to the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove the supplied value from the association property as indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property from where the supplied value will be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="ModellingAnnotationItem"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public async Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="ModellingAnnotationItem"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the containment order as indicated by the supplied orderedItem.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="orderedItem">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the contained item was successfully reordered as result.
        /// </returns>
        public async Task<bool> ReorderContainmentAsync(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="ModellingAnnotationItem"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModellingAnnotationItem"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModellingAnnotationItem"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="ModellingAnnotationItem"/> instance.
        /// A "Raw" Delete means that the delete is performed without calling before-, or after actions, or other side effects.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModellingAnnotationItem"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModellingAnnotationItem"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied <see cref="ModellingAnnotationItem"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModellingAnnotationItem"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModellingAnnotationItem"/> to be updated.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the update was successful as result.
        /// </returns>
        public async Task<bool> UpdateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            if (!await this.IsInstanceModifyAllowedAsync(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate update permission for " + thing.GetType().Name + ".");
            }

            var modellingAnnotationItem = thing as ModellingAnnotationItem;
            if (modellingAnnotationItem.IsSameOrDerivedClass<ActionItem>())
            {
                return await this.ActionItemService.UpdateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ChangeProposal>())
            {
                return await this.ChangeProposalService.UpdateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractChangeNotice>())
            {
                return await this.ContractChangeNoticeService.UpdateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractDeviation>())
            {
                return await this.ContractDeviationService.UpdateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ReviewItemDiscrepancy>())
            {
                return await this.ReviewItemDiscrepancyService.UpdateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", modellingAnnotationItem.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ModellingAnnotationItem"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModellingAnnotationItem"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModellingAnnotationItem"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> CreateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            if (!await this.IsInstanceModifyAllowedAsync(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var modellingAnnotationItem = thing as ModellingAnnotationItem;
            if (modellingAnnotationItem.IsSameOrDerivedClass<ActionItem>())
            {
                return await this.ActionItemService.CreateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ChangeProposal>())
            {
                return await this.ChangeProposalService.CreateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractChangeNotice>())
            {
                return await this.ContractChangeNoticeService.CreateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractDeviation>())
            {
                return await this.ContractDeviationService.CreateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ReviewItemDiscrepancy>())
            {
                return await this.ReviewItemDiscrepancyService.CreateConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", modellingAnnotationItem.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ModellingAnnotationItem"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModellingAnnotationItem"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModellingAnnotationItem"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var modellingAnnotationItem = thing as ModellingAnnotationItem;
            if (modellingAnnotationItem.IsSameOrDerivedClass<ActionItem>())
            {
                return await this.ActionItemService.UpsertConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ChangeProposal>())
            {
                return await this.ChangeProposalService.UpsertConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractChangeNotice>())
            {
                return await this.ContractChangeNoticeService.UpsertConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ContractDeviation>())
            {
                return await this.ContractDeviationService.UpsertConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }

            if (modellingAnnotationItem.IsSameOrDerivedClass<ReviewItemDiscrepancy>())
            {
                return await this.ReviewItemDiscrepancyService.UpsertConceptAsync(transaction, partition, modellingAnnotationItem, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", modellingAnnotationItem.GetType().Name));
        }

        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="ModellingAnnotationItem"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("ModellingAnnotationItem", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var modellingAnnotationItemColl = new List<Thing>();
            modellingAnnotationItemColl.AddRange(await this.ActionItemService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            modellingAnnotationItemColl.AddRange(await this.ChangeProposalService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            modellingAnnotationItemColl.AddRange(await this.ContractChangeNoticeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            modellingAnnotationItemColl.AddRange(await this.ContractDeviationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            modellingAnnotationItemColl.AddRange(await this.ReviewItemDiscrepancyService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));

            return await this.AfterGetAsync(modellingAnnotationItemColl, transaction, partition, idFilter);
        }

        /// <summary>
        /// Get the requested data from the ORM layer by chaining the containment properties.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="ModellingAnnotationItem"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(await this.ActionItemService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ChangeProposalService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ContractChangeNoticeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ContractDeviationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ReviewItemDiscrepancyService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            return results;
        }

        /// <summary>
        /// Execute additional logic after each GET function call.
        /// </summary>
        /// <param name="resultCollection">
        /// An instance collection that was retrieved from the persistence layer.
        /// </param>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from which the requested resource is to be retrieved.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="includeReferenceData">
        /// Control flag to indicate if reference library data should be retrieved extent=deep or extent=shallow.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having A post filtered instance of the passed in resultCollection as result.
        /// </returns>
        public override async Task<IEnumerable<Thing>> AfterGetAsync(IEnumerable<Thing> resultCollection, NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            var filteredCollection = new List<Thing>();
            foreach (var thing in resultCollection)
            {
                if (await this.IsInstanceReadAllowedAsync(transaction, thing, partition))
                {
                    filteredCollection.Add(thing);
                }
                else
                {
                    Logger.Trace("The person {0} does not have a read permission for {1}.", this.CredentialsService.Credentials.Person.UserName, thing.GetType().Name);
                }
            }

            return filteredCollection;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
