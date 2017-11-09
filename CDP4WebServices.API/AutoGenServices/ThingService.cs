// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated class. Any manual changes on this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using System.Security;

    using CDP4Common.DTO;
 
    using CDP4WebServices.API.Services.Authorization;
 
    using Npgsql;
 
    /// <summary>
    /// The Thing Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ThingService : ServiceBase, IThingService
    {
        /// <summary>
        /// Gets or sets the participantPermission service.
        /// </summary>
        public IParticipantPermissionService ParticipantPermissionService { get; set; }
 
        /// <summary>
        /// Gets or sets the person service.
        /// </summary>
        public IPersonService PersonService { get; set; }
 
        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }
 
        /// <summary>
        /// Gets or sets the participant service.
        /// </summary>
        public IParticipantService ParticipantService { get; set; }
 
        /// <summary>
        /// Gets or sets the scaleReferenceQuantityValue service.
        /// </summary>
        public IScaleReferenceQuantityValueService ScaleReferenceQuantityValueService { get; set; }
 
        /// <summary>
        /// Gets or sets the mappingToReferenceScale service.
        /// </summary>
        public IMappingToReferenceScaleService MappingToReferenceScaleService { get; set; }
 
        /// <summary>
        /// Gets or sets the quantityKindFactor service.
        /// </summary>
        public IQuantityKindFactorService QuantityKindFactorService { get; set; }
 
        /// <summary>
        /// Gets or sets the unitFactor service.
        /// </summary>
        public IUnitFactorService UnitFactorService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterTypeComponent service.
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }
 
        /// <summary>
        /// Gets or sets the personPermission service.
        /// </summary>
        public IPersonPermissionService PersonPermissionService { get; set; }
 
        /// <summary>
        /// Gets or sets the siteLogEntry service.
        /// </summary>
        public ISiteLogEntryService SiteLogEntryService { get; set; }
 
        /// <summary>
        /// Gets or sets the iterationSetup service.
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }
 
        /// <summary>
        /// Gets or sets the telephoneNumber service.
        /// </summary>
        public ITelephoneNumberService TelephoneNumberService { get; set; }
 
        /// <summary>
        /// Gets or sets the emailAddress service.
        /// </summary>
        public IEmailAddressService EmailAddressService { get; set; }
 
        /// <summary>
        /// Gets or sets the userPreference service.
        /// </summary>
        public IUserPreferenceService UserPreferenceService { get; set; }
 
        /// <summary>
        /// Gets or sets the naturalLanguage service.
        /// </summary>
        public INaturalLanguageService NaturalLanguageService { get; set; }
 
        /// <summary>
        /// Gets or sets the definedThing service.
        /// </summary>
        public IDefinedThingService DefinedThingService { get; set; }
 
        /// <summary>
        /// Gets or sets the hyperLink service.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }
 
        /// <summary>
        /// Gets or sets the definition service.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }
 
        /// <summary>
        /// Gets or sets the alias service.
        /// </summary>
        public IAliasService AliasService { get; set; }
 
        /// <summary>
        /// Gets or sets the citation service.
        /// </summary>
        public ICitationService CitationService { get; set; }
 
        /// <summary>
        /// Gets or sets the topContainer service.
        /// </summary>
        public ITopContainerService TopContainerService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterBase service.
        /// </summary>
        public IParameterBaseService ParameterBaseService { get; set; }
 
        /// <summary>
        /// Gets or sets the fileStore service.
        /// </summary>
        public IFileStoreService FileStoreService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterGroup service.
        /// </summary>
        public IParameterGroupService ParameterGroupService { get; set; }
 
        /// <summary>
        /// Gets or sets the publication service.
        /// </summary>
        public IPublicationService PublicationService { get; set; }
 
        /// <summary>
        /// Gets or sets the file service.
        /// </summary>
        public IFileService FileService { get; set; }
 
        /// <summary>
        /// Gets or sets the parametricConstraint service.
        /// </summary>
        public IParametricConstraintService ParametricConstraintService { get; set; }
 
        /// <summary>
        /// Gets or sets the externalIdentifierMap service.
        /// </summary>
        public IExternalIdentifierMapService ExternalIdentifierMapService { get; set; }
 
        /// <summary>
        /// Gets or sets the nestedElement service.
        /// </summary>
        public INestedElementService NestedElementService { get; set; }
 
        /// <summary>
        /// Gets or sets the folder service.
        /// </summary>
        public IFolderService FolderService { get; set; }
 
        /// <summary>
        /// Gets or sets the idCorrespondence service.
        /// </summary>
        public IIdCorrespondenceService IdCorrespondenceService { get; set; }
 
        /// <summary>
        /// Gets or sets the relationship service.
        /// </summary>
        public IRelationshipService RelationshipService { get; set; }
 
        /// <summary>
        /// Gets or sets the simpleParameterValue service.
        /// </summary>
        public ISimpleParameterValueService SimpleParameterValueService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterSubscriptionValueSet service.
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }
 
        /// <summary>
        /// Gets or sets the actualFiniteState service.
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }
 
        /// <summary>
        /// Gets or sets the modelLogEntry service.
        /// </summary>
        public IModelLogEntryService ModelLogEntryService { get; set; }
 
        /// <summary>
        /// Gets or sets the iteration service.
        /// </summary>
        public IIterationService IterationService { get; set; }
 
        /// <summary>
        /// Gets or sets the actualFiniteStateList service.
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }
 
        /// <summary>
        /// Gets or sets the booleanExpression service.
        /// </summary>
        public IBooleanExpressionService BooleanExpressionService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterValueSetBase service.
        /// </summary>
        public IParameterValueSetBaseService ParameterValueSetBaseService { get; set; }
 
        /// <summary>
        /// Gets or sets the nestedParameter service.
        /// </summary>
        public INestedParameterService NestedParameterService { get; set; }
 
        /// <summary>
        /// Gets or sets the fileRevision service.
        /// </summary>
        public IFileRevisionService FileRevisionService { get; set; }
 
        /// <summary>
        /// Gets or sets the ruleVerification service.
        /// </summary>
        public IRuleVerificationService RuleVerificationService { get; set; }
 
        /// <summary>
        /// Gets or sets the ruleViolation service.
        /// </summary>
        public IRuleViolationService RuleViolationService { get; set; }
 
        /// <summary>
        /// Gets or sets the stakeHolderValueMapSettings service.
        /// </summary>
        public IStakeHolderValueMapSettingsService StakeHolderValueMapSettingsService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterValue service.
        /// </summary>
        public IParameterValueService ParameterValueService { get; set; }
 
        /// <summary>
        /// Gets or sets the book service.
        /// </summary>
        public IBookService BookService { get; set; }
 
        /// <summary>
        /// Gets or sets the section service.
        /// </summary>
        public ISectionService SectionService { get; set; }
 
        /// <summary>
        /// Gets or sets the page service.
        /// </summary>
        public IPageService PageService { get; set; }
 
        /// <summary>
        /// Gets or sets the note service.
        /// </summary>
        public INoteService NoteService { get; set; }
 
        /// <summary>
        /// Gets or sets the thingReference service.
        /// </summary>
        public IThingReferenceService ThingReferenceService { get; set; }
 
        /// <summary>
        /// Gets or sets the genericAnnotation service.
        /// </summary>
        public IGenericAnnotationService GenericAnnotationService { get; set; }
 
        /// <summary>
        /// Gets or sets the diagramThingBase service.
        /// </summary>
        public IDiagramThingBaseService DiagramThingBaseService { get; set; }

        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
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
        /// The current transaction to the database.
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
        /// The current transaction to the database.
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
        /// The current transaction to the database.
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
        /// The current transaction to the database.
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
        /// Delete the supplied DTO instance.
        /// </summary>
        /// <param name="transaction">
        /// The transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied DTO instance.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The Thing to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the update was successful.
        /// </returns>
        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be updated.", thing.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied DTO instance.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The Thing to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be persisted.
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
        /// The transaction object.
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
            thingColl.AddRange(this.ParticipantPermissionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PersonService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.OrganizationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParticipantService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ScaleReferenceQuantityValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.MappingToReferenceScaleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.QuantityKindFactorService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.UnitFactorService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterTypeComponentService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PersonPermissionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SiteLogEntryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IterationSetupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.TelephoneNumberService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.EmailAddressService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.UserPreferenceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NaturalLanguageService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DefinedThingService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.HyperLinkService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DefinitionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.AliasService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.CitationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.TopContainerService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileStoreService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterGroupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PublicationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParametricConstraintService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ExternalIdentifierMapService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NestedElementService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FolderService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IdCorrespondenceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RelationshipService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SimpleParameterValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterSubscriptionValueSetService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ActualFiniteStateService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ModelLogEntryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.IterationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ActualFiniteStateListService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.BooleanExpressionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterValueSetBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NestedParameterService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.FileRevisionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RuleVerificationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.RuleViolationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.StakeHolderValueMapSettingsService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ParameterValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.BookService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.SectionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.PageService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.NoteService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.ThingReferenceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.GenericAnnotationService.GetShallow(transaction, partition, idFilter, authorizedContext));
            thingColl.AddRange(this.DiagramThingBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(thingColl, transaction, partition, idFilter);
        }

        /// <summary>
        /// Get the requested data from the ORM layer by chaining the containment properties.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
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
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(this.ParticipantPermissionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PersonService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.OrganizationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParticipantService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ScaleReferenceQuantityValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.MappingToReferenceScaleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.QuantityKindFactorService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.UnitFactorService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterTypeComponentService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PersonPermissionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SiteLogEntryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IterationSetupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TelephoneNumberService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.EmailAddressService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.UserPreferenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NaturalLanguageService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DefinedThingService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.HyperLinkService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DefinitionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.AliasService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.CitationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TopContainerService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileStoreService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterGroupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PublicationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParametricConstraintService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ExternalIdentifierMapService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NestedElementService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FolderService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IdCorrespondenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RelationshipService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SimpleParameterValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterSubscriptionValueSetService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ActualFiniteStateService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ModelLogEntryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.IterationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ActualFiniteStateListService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.BooleanExpressionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterValueSetBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NestedParameterService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileRevisionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleVerificationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleViolationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.StakeHolderValueMapSettingsService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.BookService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SectionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PageService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.NoteService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ThingReferenceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.GenericAnnotationService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DiagramThingBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));

            return results;
         }

        /// <summary>
        /// Execute additional logic after each get function call.
        /// </summary>
        /// <param name="resultCollection">
        /// An instance collection that was retrieved from the persistence layer.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
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
