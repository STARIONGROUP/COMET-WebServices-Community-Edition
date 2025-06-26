// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedThingService.cs" company="Starion Group S.A.">
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
    /// The <see cref="DefinedThing"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class DefinedThingService : ServiceBase, IDefinedThingService
    {
        /// <summary>
        /// Gets or sets the <see cref="ICategoryService"/>.
        /// </summary>
        public ICategoryService CategoryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConstantService"/>.
        /// </summary>
        public IConstantService ConstantService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainOfExpertiseService"/>.
        /// </summary>
        public IDomainOfExpertiseService DomainOfExpertiseService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainOfExpertiseGroupService"/>.
        /// </summary>
        public IDomainOfExpertiseGroupService DomainOfExpertiseGroupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementBaseService"/>.
        /// </summary>
        public IElementBaseService ElementBaseService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupService"/>.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEnumerationValueDefinitionService"/>.
        /// </summary>
        public IEnumerationValueDefinitionService EnumerationValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileTypeService"/>.
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGlossaryService"/>.
        /// </summary>
        public IGlossaryService GlossaryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGoalService"/>.
        /// </summary>
        public IGoalService GoalService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMeasurementScaleService"/>.
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMeasurementUnitService"/>.
        /// </summary>
        public IMeasurementUnitService MeasurementUnitService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOptionService"/>.
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeService"/>.
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParticipantRoleService"/>.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonRoleService"/>.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateService"/>.
        /// </summary>
        public IPossibleFiniteStateService PossibleFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>.
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReferenceDataLibraryService"/>.
        /// </summary>
        public IReferenceDataLibraryService ReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReferenceSourceService"/>.
        /// </summary>
        public IReferenceSourceService ReferenceSourceService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRequirementsContainerService"/>.
        /// </summary>
        public IRequirementsContainerService RequirementsContainerService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleService"/>.
        /// </summary>
        public IRuleService RuleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleVerificationListService"/>.
        /// </summary>
        public IRuleVerificationListService RuleVerificationListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IScaleValueDefinitionService"/>.
        /// </summary>
        public IScaleValueDefinitionService ScaleValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISimpleParameterizableThingService"/>.
        /// </summary>
        public ISimpleParameterizableThingService SimpleParameterizableThingService { get; set; }

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
        public IStakeHolderValueMapService StakeHolderValueMapService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITermService"/>.
        /// </summary>
        public ITermService TermService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUnitPrefixService"/>.
        /// </summary>
        public IUnitPrefixService UnitPrefixService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IValueGroupService"/>.
        /// </summary>
        public IValueGroupService ValueGroupService { get; set; }

        /// <summary>
        /// Get the requested <see cref="DefinedThing"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="DefinedThing"/>, optionally with contained <see cref="Thing"/>s as result.
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
        /// The <see cref="DefinedThing"/> id that will be the source for each link table record.
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
        /// The <see cref="DefinedThing"/> id that is the source of the link table records.
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
        /// The <see cref="DefinedThing"/> id that is the source for the reordered link table record.
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
        /// Delete the supplied <see cref="DefinedThing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DefinedThing"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="DefinedThing"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="DefinedThing"/> instance.
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
        /// The <see cref="DefinedThing"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="DefinedThing"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied <see cref="DefinedThing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DefinedThing"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="DefinedThing"/> to be updated.
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

            var definedThing = thing as DefinedThing;
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be updated.", definedThing.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="DefinedThing"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DefinedThing"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="DefinedThing"/> to be persisted.
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

            var definedThing = thing as DefinedThing;
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be created.", definedThing.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="DefinedThing"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DefinedThing"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="DefinedThing"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var definedThing = thing as DefinedThing;
            throw new NotSupportedException(string.Format("The abstract DTO type: {0} cannot be created.", definedThing.GetType().Name));
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="DefinedThing"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("DefinedThing", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var definedThingColl = new List<Thing>();
            definedThingColl.AddRange(await this.CategoryService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ConstantService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.DomainOfExpertiseService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.DomainOfExpertiseGroupService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ElementBaseService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.EngineeringModelSetupService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.EnumerationValueDefinitionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.FileTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.GlossaryService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.GoalService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.MeasurementScaleService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.MeasurementUnitService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.OptionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ParticipantRoleService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.PersonRoleService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.PossibleFiniteStateService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.PossibleFiniteStateListService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ReferenceDataLibraryService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ReferenceSourceService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.RequirementsContainerService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.RuleService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.RuleVerificationListService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ScaleValueDefinitionService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.SimpleParameterizableThingService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.StakeholderService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.StakeholderValueService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.StakeHolderValueMapService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.TermService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.UnitPrefixService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            definedThingColl.AddRange(await this.ValueGroupService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));

            return await this.AfterGetAsync(definedThingColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="DefinedThing"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(await this.CategoryService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ConstantService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DomainOfExpertiseService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DomainOfExpertiseGroupService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ElementBaseService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.EngineeringModelSetupService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.EnumerationValueDefinitionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.FileTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.GlossaryService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.GoalService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.MeasurementScaleService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.MeasurementUnitService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.OptionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ParticipantRoleService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PersonRoleService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PossibleFiniteStateService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.PossibleFiniteStateListService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ReferenceDataLibraryService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ReferenceSourceService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RequirementsContainerService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RuleService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.RuleVerificationListService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ScaleValueDefinitionService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.SimpleParameterizableThingService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.StakeholderService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.StakeholderValueService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.StakeHolderValueMapService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.TermService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.UnitPrefixService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.ValueGroupService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
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
