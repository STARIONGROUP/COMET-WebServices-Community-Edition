// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
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
    using System.Security;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CometServer.Services.Authorization;
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
        /// List of instances of <see cref="Thing"/>, optionally with contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? this.GetDeep(transaction, partition, ids, containerSecurityContext)
                        : this.GetShallow(transaction, partition, ids, containerSecurityContext);
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
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
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
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
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
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
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
        /// True if the contained item was successfully reordered.
        /// </returns>
        public bool ReorderContainment(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
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
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
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
        /// True if the update was successful.
        /// </returns>
        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
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
        /// True if the persistence was successful.
        /// </returns>
        public bool CreateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
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
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("Thing", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var thingColl = new List<Thing>();
            thingColl.AddRange(this.ActualFiniteStateService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ActualFiniteStateListService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.AliasService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.BookService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.BooleanExpressionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.CitationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DefinedThingService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DefinitionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DependentParameterTypeAssignmentService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DiagramThingBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.EmailAddressService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ExternalIdentifierMapService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileRevisionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileStoreService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FolderService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.GenericAnnotationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.HyperLinkService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IdCorrespondenceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IndependentParameterTypeAssignmentService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IterationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IterationSetupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.LogEntryChangelogItemService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.MappingToReferenceScaleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ModelLogEntryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NaturalLanguageService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NestedElementService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NestedParameterService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NoteService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.OrganizationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.OrganizationalParticipantService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PageService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterGroupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterSubscriptionValueSetService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterTypeComponentService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterValueSetBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParametricConstraintService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParticipantService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParticipantPermissionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PersonService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PersonPermissionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PublicationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.QuantityKindFactorService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RelationshipService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RuleVerificationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RuleViolationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ScaleReferenceQuantityValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SectionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SimpleParameterValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SiteLogEntryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.StakeHolderValueMapSettingsService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.TelephoneNumberService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ThingReferenceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.TopContainerService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.UnitFactorService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.UserPreferenceService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(thingColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="Thing"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(this.ActualFiniteStateService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ActualFiniteStateListService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.AliasService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.BookService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.BooleanExpressionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.CitationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DefinedThingService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DefinitionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DependentParameterTypeAssignmentService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DiagramThingBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.EmailAddressService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ExternalIdentifierMapService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileRevisionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileStoreService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FolderService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.GenericAnnotationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.HyperLinkService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IdCorrespondenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IndependentParameterTypeAssignmentService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IterationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IterationSetupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.LogEntryChangelogItemService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.MappingToReferenceScaleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ModelLogEntryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NaturalLanguageService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NestedElementService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NestedParameterService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NoteService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.OrganizationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.OrganizationalParticipantService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PageService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterGroupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterSubscriptionValueSetService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterTypeComponentService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterValueSetBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParametricConstraintService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParticipantService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParticipantPermissionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PersonService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PersonPermissionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PublicationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.QuantityKindFactorService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RelationshipService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleVerificationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleViolationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ScaleReferenceQuantityValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SectionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SimpleParameterValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SiteLogEntryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.StakeHolderValueMapSettingsService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TelephoneNumberService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ThingReferenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TopContainerService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.UnitFactorService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.UserPreferenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
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
        /// A post filtered instance of the passed in resultCollection.
        /// </returns>
        public override IEnumerable<Thing> AfterGet(IEnumerable<Thing> resultCollection, NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            var filteredCollection = new List<Thing>();
            foreach (var thing in resultCollection)
            {
                if (this.IsInstanceReadAllowed(transaction, thing, partition))
                {
                    filteredCollection.Add(thing);
                }
                else
                {
                    Logger.Info("The person " + this.PermissionService.Credentials.Person.UserName + " does not have a read permission for " + thing.GetType().Name + ".");
                }
            }

            return filteredCollection;
        }
    }
}
