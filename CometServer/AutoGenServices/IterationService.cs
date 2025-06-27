// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationService.cs" company="Starion Group S.A.">
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
    /// The <see cref="Iteration"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class IterationService : ServiceBase, IIterationService
    {
        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>.
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDiagramCanvasService"/>.
        /// </summary>
        public IDiagramCanvasService DiagramCanvasService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainFileStoreService"/>.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService"/>.
        /// </summary>
        public IElementDefinitionService ElementService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IExternalIdentifierMapService"/>.
        /// </summary>
        public IExternalIdentifierMapService ExternalIdentifierMapService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGoalService"/>.
        /// </summary>
        public IGoalService GoalService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOptionService"/>.
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>.
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPublicationService"/>.
        /// </summary>
        public IPublicationService PublicationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRelationshipService"/>.
        /// </summary>
        public IRelationshipService RelationshipService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRequirementsSpecificationService"/>.
        /// </summary>
        public IRequirementsSpecificationService RequirementsSpecificationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleVerificationListService"/>.
        /// </summary>
        public IRuleVerificationListService RuleVerificationListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISharedStyleService"/>.
        /// </summary>
        public ISharedStyleService SharedDiagramStyleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStakeholderService"/>.
        /// </summary>
        public IStakeholderService StakeholderService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStakeholderValueService"/>.
        /// </summary>
        public IStakeholderValueService StakeholderValueService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStakeHolderValueMapService"/>.
        /// </summary>
        public IStakeHolderValueMapService StakeholderValueMapService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IValueGroupService"/>.
        /// </summary>
        public IValueGroupService ValueGroupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationDao"/>.
        /// </summary>
        public IIterationDao IterationDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="Iteration"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="Iteration"/>, optionally with contained <see cref="Thing"/>s as result.
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
        /// The <see cref="Iteration"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return await this.IterationDao.AddToCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="Iteration"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public async Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return await this.IterationDao.DeleteFromCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="Iteration"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return await this.IterationDao.ReorderCollectionPropertyAsync(transaction, partition, propertyName, iid, orderUpdate);
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
        public Task<bool> ReorderContainmentAsync(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="Iteration"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Iteration"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            if (!await this.IsInstanceModifyAllowedAsync(transaction, thing, partition, DeleteOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate delete permission for " + thing.GetType().Name + ".");
            }

            return await this.IterationDao.DeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="Iteration"/> instance.
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
        /// The <see cref="Iteration"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return await this.IterationDao.RawDeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="Iteration"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Iteration"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be updated.
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

            var iteration = thing as Iteration;
            return await this.IterationDao.UpdateAsync(transaction, partition, iteration, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="Iteration"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Iteration"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be persisted.
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

            var iteration = thing as Iteration;
            var createSuccesful = await this.IterationDao.WriteAsync(transaction, partition, iteration, container);
            return createSuccesful && await this.CreateContainmentAsync(transaction, partition, iteration);
        }

        /// <summary>
        /// Persist the supplied <see cref="Iteration"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Iteration"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var iteration = thing as Iteration;
            var createSuccesful = await this.IterationDao.UpsertAsync(transaction, partition, iteration, container);
            return createSuccesful && await this.UpsertContainmentAsync(transaction, partition, iteration);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="Iteration"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("Iteration", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var iterationColl = new List<Thing>(await this.IterationDao.ReadAsync(transaction, partition, idFilter, await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction), (DateTime)(await this.TransactionManager.GetRawSessionInstantAsync(transaction))));

            return await this.AfterGetAsync(iterationColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="Iteration"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(await this.GetShallowAsync(transaction, partition, idFilter, containerSecurityContext));
            var iterationColl = results.Where(i => i.GetType() == typeof(Iteration)).Cast<Iteration>().ToList();

            var iterationPartition = partition.Replace("EngineeringModel", "Iteration");
            results.AddRange(await this.ActualFiniteStateListService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.ActualFiniteStateList), containerSecurityContext));
            results.AddRange(await this.DiagramCanvasService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.DiagramCanvas), containerSecurityContext));
            results.AddRange(await this.DomainFileStoreService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.DomainFileStore), containerSecurityContext));
            results.AddRange(await this.ElementService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Element), containerSecurityContext));
            results.AddRange(await this.ExternalIdentifierMapService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.ExternalIdentifierMap), containerSecurityContext));
            results.AddRange(await this.GoalService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Goal), containerSecurityContext));
            results.AddRange(await this.OptionService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Option).ToIdList(), containerSecurityContext));
            results.AddRange(await this.PossibleFiniteStateListService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.PossibleFiniteStateList), containerSecurityContext));
            results.AddRange(await this.PublicationService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Publication), containerSecurityContext));
            results.AddRange(await this.RelationshipService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Relationship), containerSecurityContext));
            results.AddRange(await this.RequirementsSpecificationService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.RequirementsSpecification), containerSecurityContext));
            results.AddRange(await this.RuleVerificationListService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.RuleVerificationList), containerSecurityContext));
            results.AddRange(await this.SharedDiagramStyleService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.SharedDiagramStyle), containerSecurityContext));
            results.AddRange(await this.StakeholderService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.Stakeholder), containerSecurityContext));
            results.AddRange(await this.StakeholderValueService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.StakeholderValue), containerSecurityContext));
            results.AddRange(await this.StakeholderValueMapService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.StakeholderValueMap), containerSecurityContext));
            results.AddRange(await this.ValueGroupService.GetDeepAsync(transaction, iterationPartition, iterationColl.SelectMany(x => x.ValueGroup), containerSecurityContext));

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

        /// <summary>
        /// Persist the <see cref="Iteration"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> CreateContainmentAsync(NpgsqlTransaction transaction, string partition, Iteration iteration)
        {
            var results = new List<bool>();
            var iterationPartition = partition.Replace("EngineeringModel", "Iteration");

            foreach (var actualFiniteStateList in this.ResolveFromRequestCache(iteration.ActualFiniteStateList))
            {
                results.Add(await this.ActualFiniteStateListService.CreateConceptAsync(transaction, iterationPartition, actualFiniteStateList, iteration));
            }

            foreach (var diagramCanvas in this.ResolveFromRequestCache(iteration.DiagramCanvas))
            {
                results.Add(await this.DiagramCanvasService.CreateConceptAsync(transaction, iterationPartition, diagramCanvas, iteration));
            }

            foreach (var domainFileStore in this.ResolveFromRequestCache(iteration.DomainFileStore))
            {
                results.Add(await this.DomainFileStoreService.CreateConceptAsync(transaction, iterationPartition, domainFileStore, iteration));
            }

            foreach (var element in this.ResolveFromRequestCache(iteration.Element))
            {
                results.Add(await this.ElementService.CreateConceptAsync(transaction, iterationPartition, element, iteration));
            }

            foreach (var externalIdentifierMap in this.ResolveFromRequestCache(iteration.ExternalIdentifierMap))
            {
                results.Add(await this.ExternalIdentifierMapService.CreateConceptAsync(transaction, iterationPartition, externalIdentifierMap, iteration));
            }

            foreach (var goal in this.ResolveFromRequestCache(iteration.Goal))
            {
                results.Add(await this.GoalService.CreateConceptAsync(transaction, iterationPartition, goal, iteration));
            }

            foreach (var option in this.ResolveFromRequestCache(iteration.Option))
            {
                results.Add(await this.OptionService.CreateConceptAsync(transaction, iterationPartition, (Option)option.V, iteration, option.K));
            }

            foreach (var possibleFiniteStateList in this.ResolveFromRequestCache(iteration.PossibleFiniteStateList))
            {
                results.Add(await this.PossibleFiniteStateListService.CreateConceptAsync(transaction, iterationPartition, possibleFiniteStateList, iteration));
            }

            foreach (var publication in this.ResolveFromRequestCache(iteration.Publication))
            {
                results.Add(await this.PublicationService.CreateConceptAsync(transaction, iterationPartition, publication, iteration));
            }

            foreach (var relationship in this.ResolveFromRequestCache(iteration.Relationship))
            {
                results.Add(await this.RelationshipService.CreateConceptAsync(transaction, iterationPartition, relationship, iteration));
            }

            foreach (var requirementsSpecification in this.ResolveFromRequestCache(iteration.RequirementsSpecification))
            {
                results.Add(await this.RequirementsSpecificationService.CreateConceptAsync(transaction, iterationPartition, requirementsSpecification, iteration));
            }

            foreach (var ruleVerificationList in this.ResolveFromRequestCache(iteration.RuleVerificationList))
            {
                results.Add(await this.RuleVerificationListService.CreateConceptAsync(transaction, iterationPartition, ruleVerificationList, iteration));
            }

            foreach (var sharedDiagramStyle in this.ResolveFromRequestCache(iteration.SharedDiagramStyle))
            {
                results.Add(await this.SharedDiagramStyleService.CreateConceptAsync(transaction, iterationPartition, sharedDiagramStyle, iteration));
            }

            foreach (var stakeholder in this.ResolveFromRequestCache(iteration.Stakeholder))
            {
                results.Add(await this.StakeholderService.CreateConceptAsync(transaction, iterationPartition, stakeholder, iteration));
            }

            foreach (var stakeholderValue in this.ResolveFromRequestCache(iteration.StakeholderValue))
            {
                results.Add(await this.StakeholderValueService.CreateConceptAsync(transaction, iterationPartition, stakeholderValue, iteration));
            }

            foreach (var stakeholderValueMap in this.ResolveFromRequestCache(iteration.StakeholderValueMap))
            {
                results.Add(await this.StakeholderValueMapService.CreateConceptAsync(transaction, iterationPartition, stakeholderValueMap, iteration));
            }

            foreach (var valueGroup in this.ResolveFromRequestCache(iteration.ValueGroup))
            {
                results.Add(await this.ValueGroupService.CreateConceptAsync(transaction, iterationPartition, valueGroup, iteration));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="Iteration"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> UpsertContainmentAsync(NpgsqlTransaction transaction, string partition, Iteration iteration)
        {
            var results = new List<bool>();
            var iterationPartition = partition.Replace("EngineeringModel", "Iteration");

            foreach (var actualFiniteStateList in this.ResolveFromRequestCache(iteration.ActualFiniteStateList))
            {
                results.Add(await this.ActualFiniteStateListService.UpsertConceptAsync(transaction, iterationPartition, actualFiniteStateList, iteration));
            }

            foreach (var diagramCanvas in this.ResolveFromRequestCache(iteration.DiagramCanvas))
            {
                results.Add(await this.DiagramCanvasService.UpsertConceptAsync(transaction, iterationPartition, diagramCanvas, iteration));
            }

            foreach (var domainFileStore in this.ResolveFromRequestCache(iteration.DomainFileStore))
            {
                results.Add(await this.DomainFileStoreService.UpsertConceptAsync(transaction, iterationPartition, domainFileStore, iteration));
            }

            foreach (var element in this.ResolveFromRequestCache(iteration.Element))
            {
                results.Add(await this.ElementService.UpsertConceptAsync(transaction, iterationPartition, element, iteration));
            }

            foreach (var externalIdentifierMap in this.ResolveFromRequestCache(iteration.ExternalIdentifierMap))
            {
                results.Add(await this.ExternalIdentifierMapService.UpsertConceptAsync(transaction, iterationPartition, externalIdentifierMap, iteration));
            }

            foreach (var goal in this.ResolveFromRequestCache(iteration.Goal))
            {
                results.Add(await this.GoalService.UpsertConceptAsync(transaction, iterationPartition, goal, iteration));
            }

            foreach (var option in this.ResolveFromRequestCache(iteration.Option))
            {
                results.Add(await this.OptionService.UpsertConceptAsync(transaction, iterationPartition, (Option)option.V, iteration, option.K));
            }

            foreach (var possibleFiniteStateList in this.ResolveFromRequestCache(iteration.PossibleFiniteStateList))
            {
                results.Add(await this.PossibleFiniteStateListService.UpsertConceptAsync(transaction, iterationPartition, possibleFiniteStateList, iteration));
            }

            foreach (var publication in this.ResolveFromRequestCache(iteration.Publication))
            {
                results.Add(await this.PublicationService.UpsertConceptAsync(transaction, iterationPartition, publication, iteration));
            }

            foreach (var relationship in this.ResolveFromRequestCache(iteration.Relationship))
            {
                results.Add(await this.RelationshipService.UpsertConceptAsync(transaction, iterationPartition, relationship, iteration));
            }

            foreach (var requirementsSpecification in this.ResolveFromRequestCache(iteration.RequirementsSpecification))
            {
                results.Add(await this.RequirementsSpecificationService.UpsertConceptAsync(transaction, iterationPartition, requirementsSpecification, iteration));
            }

            foreach (var ruleVerificationList in this.ResolveFromRequestCache(iteration.RuleVerificationList))
            {
                results.Add(await this.RuleVerificationListService.UpsertConceptAsync(transaction, iterationPartition, ruleVerificationList, iteration));
            }

            foreach (var sharedDiagramStyle in this.ResolveFromRequestCache(iteration.SharedDiagramStyle))
            {
                results.Add(await this.SharedDiagramStyleService.UpsertConceptAsync(transaction, iterationPartition, sharedDiagramStyle, iteration));
            }

            foreach (var stakeholder in this.ResolveFromRequestCache(iteration.Stakeholder))
            {
                results.Add(await this.StakeholderService.UpsertConceptAsync(transaction, iterationPartition, stakeholder, iteration));
            }

            foreach (var stakeholderValue in this.ResolveFromRequestCache(iteration.StakeholderValue))
            {
                results.Add(await this.StakeholderValueService.UpsertConceptAsync(transaction, iterationPartition, stakeholderValue, iteration));
            }

            foreach (var stakeholderValueMap in this.ResolveFromRequestCache(iteration.StakeholderValueMap))
            {
                results.Add(await this.StakeholderValueMapService.UpsertConceptAsync(transaction, iterationPartition, stakeholderValueMap, iteration));
            }

            foreach (var valueGroup in this.ResolveFromRequestCache(iteration.ValueGroup))
            {
                results.Add(await this.ValueGroupService.UpsertConceptAsync(transaction, iterationPartition, valueGroup, iteration));
            }

            return results.All(x => x);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
