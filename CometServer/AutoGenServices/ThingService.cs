// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingService.cs" company="Starion Group S.A.">
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
    /// The <see cref="Thing"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ThingService : ServiceBase, IThingService
    {
        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateService"/>.
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>.
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAliasService"/>.
        /// </summary>
        public IAliasService AliasService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IBookService"/>.
        /// </summary>
        public IBookService BookService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IBooleanExpressionService"/>.
        /// </summary>
        public IBooleanExpressionService BooleanExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICitationService"/>.
        /// </summary>
        public ICitationService CitationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefinedThingService"/>.
        /// </summary>
        public IDefinedThingService DefinedThingService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefinitionService"/>.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDependentParameterTypeAssignmentService"/>.
        /// </summary>
        public IDependentParameterTypeAssignmentService DependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDiagramThingBaseService"/>.
        /// </summary>
        public IDiagramThingBaseService DiagramThingBaseService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEmailAddressService"/>.
        /// </summary>
        public IEmailAddressService EmailAddressService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IExternalIdentifierMapService"/>.
        /// </summary>
        public IExternalIdentifierMapService ExternalIdentifierMapService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileService"/>.
        /// </summary>
        public IFileService FileService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileRevisionService"/>.
        /// </summary>
        public IFileRevisionService FileRevisionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileStoreService"/>.
        /// </summary>
        public IFileStoreService FileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFolderService"/>.
        /// </summary>
        public IFolderService FolderService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGenericAnnotationService"/>.
        /// </summary>
        public IGenericAnnotationService GenericAnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHyperLinkService"/>.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIdCorrespondenceService"/>.
        /// </summary>
        public IIdCorrespondenceService IdCorrespondenceService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIndependentParameterTypeAssignmentService"/>.
        /// </summary>
        public IIndependentParameterTypeAssignmentService IndependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>.
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationSetupService"/>.
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ILogEntryChangelogItemService"/>.
        /// </summary>
        public ILogEntryChangelogItemService LogEntryChangelogItemService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMappingToReferenceScaleService"/>.
        /// </summary>
        public IMappingToReferenceScaleService MappingToReferenceScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModelLogEntryService"/>.
        /// </summary>
        public IModelLogEntryService ModelLogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INaturalLanguageService"/>.
        /// </summary>
        public INaturalLanguageService NaturalLanguageService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INestedElementService"/>.
        /// </summary>
        public INestedElementService NestedElementService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INestedParameterService"/>.
        /// </summary>
        public INestedParameterService NestedParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INoteService"/>.
        /// </summary>
        public INoteService NoteService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOrganizationService"/>.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOrganizationalParticipantService"/>.
        /// </summary>
        public IOrganizationalParticipantService OrganizationalParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPageService"/>.
        /// </summary>
        public IPageService PageService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterBaseService"/>.
        /// </summary>
        public IParameterBaseService ParameterBaseService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterGroupService"/>.
        /// </summary>
        public IParameterGroupService ParameterGroupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>.
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeComponentService"/>.
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueService"/>.
        /// </summary>
        public IParameterValueService ParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetBaseService"/>.
        /// </summary>
        public IParameterValueSetBaseService ParameterValueSetBaseService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParametricConstraintService"/>.
        /// </summary>
        public IParametricConstraintService ParametricConstraintService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParticipantService"/>.
        /// </summary>
        public IParticipantService ParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParticipantPermissionService"/>.
        /// </summary>
        public IParticipantPermissionService ParticipantPermissionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonService"/>.
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonPermissionService"/>.
        /// </summary>
        public IPersonPermissionService PersonPermissionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPublicationService"/>.
        /// </summary>
        public IPublicationService PublicationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQuantityKindFactorService"/>.
        /// </summary>
        public IQuantityKindFactorService QuantityKindFactorService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRelationshipService"/>.
        /// </summary>
        public IRelationshipService RelationshipService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleVerificationService"/>.
        /// </summary>
        public IRuleVerificationService RuleVerificationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleViolationService"/>.
        /// </summary>
        public IRuleViolationService RuleViolationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IScaleReferenceQuantityValueService"/>.
        /// </summary>
        public IScaleReferenceQuantityValueService ScaleReferenceQuantityValueService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISectionService"/>.
        /// </summary>
        public ISectionService SectionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISimpleParameterValueService"/>.
        /// </summary>
        public ISimpleParameterValueService SimpleParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteLogEntryService"/>.
        /// </summary>
        public ISiteLogEntryService SiteLogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStakeHolderValueMapSettingsService"/>.
        /// </summary>
        public IStakeHolderValueMapSettingsService StakeHolderValueMapSettingsService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITelephoneNumberService"/>.
        /// </summary>
        public ITelephoneNumberService TelephoneNumberService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IThingReferenceService"/>.
        /// </summary>
        public IThingReferenceService ThingReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITopContainerService"/>.
        /// </summary>
        public ITopContainerService TopContainerService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUnitFactorService"/>.
        /// </summary>
        public IUnitFactorService UnitFactorService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUserPreferenceService"/>.
        /// </summary>
        public IUserPreferenceService UserPreferenceService { get; set; }

        /// <summary>
        /// Get the requested <see cref="Thing"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="Thing"/>, optionally with contained <see cref="Thing"/>s as result.
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
        /// The <see cref="Thing"/> id that will be the source for each link table record.
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
        /// The <see cref="Thing"/> id that is the source of the link table records.
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
        /// The <see cref="Thing"/> id that is the source for the reordered link table record.
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
        /// Delete the supplied <see cref="Thing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="Thing"/> instance.
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
        /// The <see cref="Thing"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied <see cref="Thing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> to be updated.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the update was successful as result.
        /// </returns>
        public async Task<bool> UpdateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be updated.", thing.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="Thing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> CreateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be created.", thing.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="Thing"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be created.", thing.GetType().Name));
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="Thing"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("Thing", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var thingColl = new List<Thing>();
            thingColl.AddRange(await this.ActualFiniteStateService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ActualFiniteStateListService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.AliasService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.BookService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.BooleanExpressionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.CitationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.DefinedThingService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.DefinitionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.DependentParameterTypeAssignmentService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.DiagramThingBaseService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.EmailAddressService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ExternalIdentifierMapService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.FileService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.FileRevisionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.FileStoreService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.FolderService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.GenericAnnotationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.HyperLinkService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.IdCorrespondenceService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.IndependentParameterTypeAssignmentService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.IterationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.IterationSetupService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.LogEntryChangelogItemService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.MappingToReferenceScaleService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ModelLogEntryService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.NaturalLanguageService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.NestedElementService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.NestedParameterService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.NoteService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.OrganizationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.OrganizationalParticipantService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.PageService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterBaseService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterGroupService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterSubscriptionValueSetService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterTypeComponentService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterValueService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParameterValueSetBaseService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParametricConstraintService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParticipantService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ParticipantPermissionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.PersonService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.PersonPermissionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.PublicationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.QuantityKindFactorService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.RelationshipService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.RuleVerificationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.RuleViolationService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ScaleReferenceQuantityValueService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.SectionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.SimpleParameterValueService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.SiteLogEntryService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.StakeHolderValueMapSettingsService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.TelephoneNumberService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.ThingReferenceService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.TopContainerService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.UnitFactorService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(await this.UserPreferenceService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));

            return await this.AfterGetAsync(thingColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="Thing"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(await this.ActualFiniteStateService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ActualFiniteStateListService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.AliasService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.BookService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.BooleanExpressionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.CitationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DefinedThingService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DefinitionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DependentParameterTypeAssignmentService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DiagramThingBaseService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.EmailAddressService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ExternalIdentifierMapService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.FileService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.FileRevisionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.FileStoreService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.FolderService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.GenericAnnotationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.HyperLinkService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.IdCorrespondenceService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.IndependentParameterTypeAssignmentService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.IterationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.IterationSetupService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.LogEntryChangelogItemService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.MappingToReferenceScaleService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ModelLogEntryService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.NaturalLanguageService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.NestedElementService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.NestedParameterService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.NoteService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.OrganizationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.OrganizationalParticipantService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PageService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterBaseService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterGroupService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterSubscriptionValueSetService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterTypeComponentService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterValueService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterValueSetBaseService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParametricConstraintService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParticipantService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParticipantPermissionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PersonService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PersonPermissionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PublicationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.QuantityKindFactorService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RelationshipService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RuleVerificationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RuleViolationService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ScaleReferenceQuantityValueService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.SectionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.SimpleParameterValueService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.SiteLogEntryService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.StakeHolderValueMapSettingsService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.TelephoneNumberService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ThingReferenceService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.TopContainerService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.UnitFactorService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.UserPreferenceService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
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
