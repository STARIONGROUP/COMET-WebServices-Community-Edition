// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedThingService.cs" company="RHEA System S.A.">
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
    /// The DefinedThing Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class DefinedThingService : ServiceBase, IDefinedThingService
    {
        /// <summary>
        /// Gets or sets the participantRole service.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }
 
        /// <summary>
        /// Gets or sets the engineeringModelSetup service.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }
 
        /// <summary>
        /// Gets or sets the glossary service.
        /// </summary>
        public IGlossaryService GlossaryService { get; set; }
 
        /// <summary>
        /// Gets or sets the referenceDataLibrary service.
        /// </summary>
        public IReferenceDataLibraryService ReferenceDataLibraryService { get; set; }
 
        /// <summary>
        /// Gets or sets the term service.
        /// </summary>
        public ITermService TermService { get; set; }
 
        /// <summary>
        /// Gets or sets the fileType service.
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the measurementScale service.
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }
 
        /// <summary>
        /// Gets or sets the domainOfExpertise service.
        /// </summary>
        public IDomainOfExpertiseService DomainOfExpertiseService { get; set; }
 
        /// <summary>
        /// Gets or sets the parameterType service.
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the scaleValueDefinition service.
        /// </summary>
        public IScaleValueDefinitionService ScaleValueDefinitionService { get; set; }
 
        /// <summary>
        /// Gets or sets the measurementUnit service.
        /// </summary>
        public IMeasurementUnitService MeasurementUnitService { get; set; }
 
        /// <summary>
        /// Gets or sets the category service.
        /// </summary>
        public ICategoryService CategoryService { get; set; }
 
        /// <summary>
        /// Gets or sets the unitPrefix service.
        /// </summary>
        public IUnitPrefixService UnitPrefixService { get; set; }
 
        /// <summary>
        /// Gets or sets the rule service.
        /// </summary>
        public IRuleService RuleService { get; set; }
 
        /// <summary>
        /// Gets or sets the enumerationValueDefinition service.
        /// </summary>
        public IEnumerationValueDefinitionService EnumerationValueDefinitionService { get; set; }
 
        /// <summary>
        /// Gets or sets the personRole service.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }
 
        /// <summary>
        /// Gets or sets the domainOfExpertiseGroup service.
        /// </summary>
        public IDomainOfExpertiseGroupService DomainOfExpertiseGroupService { get; set; }
 
        /// <summary>
        /// Gets or sets the referenceSource service.
        /// </summary>
        public IReferenceSourceService ReferenceSourceService { get; set; }
 
        /// <summary>
        /// Gets or sets the constant service.
        /// </summary>
        public IConstantService ConstantService { get; set; }
 
        /// <summary>
        /// Gets or sets the possibleFiniteState service.
        /// </summary>
        public IPossibleFiniteStateService PossibleFiniteStateService { get; set; }
 
        /// <summary>
        /// Gets or sets the option service.
        /// </summary>
        public IOptionService OptionService { get; set; }
 
        /// <summary>
        /// Gets or sets the possibleFiniteStateList service.
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }
 
        /// <summary>
        /// Gets or sets the elementBase service.
        /// </summary>
        public IElementBaseService ElementBaseService { get; set; }
 
        /// <summary>
        /// Gets or sets the requirementsContainer service.
        /// </summary>
        public IRequirementsContainerService RequirementsContainerService { get; set; }
 
        /// <summary>
        /// Gets or sets the simpleParameterizableThing service.
        /// </summary>
        public ISimpleParameterizableThingService SimpleParameterizableThingService { get; set; }
 
        /// <summary>
        /// Gets or sets the ruleVerificationList service.
        /// </summary>
        public IRuleVerificationListService RuleVerificationListService { get; set; }
 
        /// <summary>
        /// Gets or sets the goal service.
        /// </summary>
        public IGoalService GoalService { get; set; }
 
        /// <summary>
        /// Gets or sets the stakeholder service.
        /// </summary>
        public IStakeholderService StakeholderService { get; set; }
 
        /// <summary>
        /// Gets or sets the valueGroup service.
        /// </summary>
        public IValueGroupService ValueGroupService { get; set; }
 
        /// <summary>
        /// Gets or sets the stakeholderValue service.
        /// </summary>
        public IStakeholderValueService StakeholderValueService { get; set; }
 
        /// <summary>
        /// Gets or sets the stakeHolderValueMap service.
        /// </summary>
        public IStakeHolderValueMapService StakeHolderValueMapService { get; set; }

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
        /// List of instances of <see cref="DefinedThing"/>.
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
        /// The <see cref="DefinedThing"/> id that will be the source for each link table record.
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
        /// The <see cref="DefinedThing"/> id that is the source of the link table records.
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
        /// The <see cref="DefinedThing"/> id that is the source for the reordered link table record.
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
        /// The <see cref="DefinedThing"/> to delete.
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
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate update permission for " + thing.GetType().Name + ".");
            }

            var definedThing = thing as DefinedThing;
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be updated.", definedThing.GetType().Name));
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
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var definedThing = thing as DefinedThing;
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be created.", definedThing.GetType().Name));
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
        /// List of instances of <see cref="DefinedThing"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("DefinedThing", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var definedThingColl = new List<Thing>();
            definedThingColl.AddRange(this.ParticipantRoleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.EngineeringModelSetupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.GlossaryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ReferenceDataLibraryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.TermService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.FileTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.MeasurementScaleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.DomainOfExpertiseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ScaleValueDefinitionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.MeasurementUnitService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.CategoryService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.UnitPrefixService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.RuleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.EnumerationValueDefinitionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.PersonRoleService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.DomainOfExpertiseGroupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ReferenceSourceService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ConstantService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.PossibleFiniteStateService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.OptionService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.PossibleFiniteStateListService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ElementBaseService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.RequirementsContainerService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.SimpleParameterizableThingService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.RuleVerificationListService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.GoalService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.StakeholderService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.ValueGroupService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.StakeholderValueService.GetShallow(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(this.StakeHolderValueMapService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(definedThingColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="DefinedThing"/>.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(this.ParticipantRoleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.EngineeringModelSetupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.GlossaryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ReferenceDataLibraryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TermService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.FileTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.MeasurementScaleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DomainOfExpertiseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ScaleValueDefinitionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.MeasurementUnitService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.CategoryService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.UnitPrefixService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.EnumerationValueDefinitionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PersonRoleService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DomainOfExpertiseGroupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ReferenceSourceService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ConstantService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PossibleFiniteStateService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.OptionService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.PossibleFiniteStateListService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ElementBaseService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RequirementsContainerService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.SimpleParameterizableThingService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.RuleVerificationListService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.GoalService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.StakeholderService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.ValueGroupService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.StakeholderValueService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.StakeHolderValueMapService.GetDeep(transaction, partition, idFilter, containerSecurityContext));

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
