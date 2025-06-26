// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryService.cs" company="Starion Group S.A.">
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
    /// The <see cref="SiteDirectory"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class SiteDirectoryService : ServiceBase, ISiteDirectoryService
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteDirectoryDataAnnotationService"/>.
        /// </summary>
        public ISiteDirectoryDataAnnotationService AnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainOfExpertiseService"/>.
        /// </summary>
        public IDomainOfExpertiseService DomainService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainOfExpertiseGroupService"/>.
        /// </summary>
        public IDomainOfExpertiseGroupService DomainGroupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteLogEntryService"/>.
        /// </summary>
        public ISiteLogEntryService LogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupService"/>.
        /// </summary>
        public IEngineeringModelSetupService ModelService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INaturalLanguageService"/>.
        /// </summary>
        public INaturalLanguageService NaturalLanguageService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOrganizationService"/>.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParticipantRoleService"/>.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonService"/>.
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonRoleService"/>.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>.
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteDirectoryDao"/>.
        /// </summary>
        public ISiteDirectoryDao SiteDirectoryDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="SiteDirectory"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="SiteDirectory"/>, optionally with contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? await this.GetDeepAsync(transaction, partition, ids, containerSecurityContext)
                        : await this.GetShallowAsync(transaction, partition, ids, containerSecurityContext);
        }

        /// <summary>
        /// Persist the supplied DTO instance to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="dto">
        /// The DTO instance to persist.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> Insert(NpgsqlTransaction transaction, string partition, Thing dto, Thing container = null)
        {
            var siteDirectory = dto as SiteDirectory;
            return await this.CreateConceptAsync(transaction, partition, siteDirectory, container);
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
        /// The <see cref="SiteDirectory"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return await this.SiteDirectoryDao.AddToCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="SiteDirectory"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public async Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return await this.SiteDirectoryDao.DeleteFromCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="SiteDirectory"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public async Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return await this.SiteDirectoryDao.ReorderCollectionPropertyAsync(transaction, partition, propertyName, iid, orderUpdate);
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
        /// Delete the supplied <see cref="SiteDirectory"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteDirectory"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteDirectory"/> to be removed.
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

            return await this.SiteDirectoryDao.DeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="SiteDirectory"/> instance.
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
        /// The <see cref="SiteDirectory"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteDirectory"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public async Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return await this.SiteDirectoryDao.RawDeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="SiteDirectory"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteDirectory"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteDirectory"/> to be updated.
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

            var siteDirectory = thing as SiteDirectory;
            return await this.SiteDirectoryDao.UpdateAsync(transaction, partition, siteDirectory, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="SiteDirectory"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteDirectory"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteDirectory"/> to be persisted.
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

            var siteDirectory = thing as SiteDirectory;
            var createSuccesful = await this.SiteDirectoryDao.WriteAsync(transaction, partition, siteDirectory, container);
            return createSuccesful && await this.CreateContainmentAsync(transaction, partition, siteDirectory);
        }

        /// <summary>
        /// Persist the supplied <see cref="SiteDirectory"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteDirectory"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteDirectory"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var siteDirectory = thing as SiteDirectory;
            var createSuccesful = await this.SiteDirectoryDao.UpsertAsync(transaction, partition, siteDirectory, container);
            return createSuccesful && await this.UpsertContainmentAsync(transaction, partition, siteDirectory);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="SiteDirectory"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("SiteDirectory", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var siteDirectoryColl = new List<Thing>(await this.SiteDirectoryDao.ReadAsync(transaction, partition, idFilter, await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction), (DateTime)(await this.TransactionManager.GetRawSessionInstantAsync(transaction))));

            return await this.AfterGetAsync(siteDirectoryColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="SiteDirectory"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(await this.GetShallowAsync(transaction, partition, idFilter, containerSecurityContext));
            var siteDirectoryColl = results.Where(i => i.GetType() == typeof(SiteDirectory)).Cast<SiteDirectory>().ToList();

            results.AddRange(await this.AnnotationService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.Annotation), containerSecurityContext));
            results.AddRange(await this.DomainService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.Domain), containerSecurityContext));
            results.AddRange(await this.DomainGroupService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.DomainGroup), containerSecurityContext));
            results.AddRange(await this.LogEntryService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.LogEntry), containerSecurityContext));
            results.AddRange(await this.ModelService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.Model), containerSecurityContext));
            results.AddRange(await this.NaturalLanguageService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.NaturalLanguage), containerSecurityContext));
            results.AddRange(await this.OrganizationService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.Organization), containerSecurityContext));
            results.AddRange(await this.ParticipantRoleService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.ParticipantRole), containerSecurityContext));
            results.AddRange(await this.PersonService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.Person), containerSecurityContext));
            results.AddRange(await this.PersonRoleService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.PersonRole), containerSecurityContext));
            results.AddRange(await this.SiteReferenceDataLibraryService.GetDeepAsync(transaction, partition, siteDirectoryColl.SelectMany(x => x.SiteReferenceDataLibrary), containerSecurityContext));

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
        /// Persist the <see cref="SiteDirectory"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="siteDirectory">
        /// The <see cref="SiteDirectory"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> CreateContainmentAsync(NpgsqlTransaction transaction, string partition, SiteDirectory siteDirectory)
        {
            var results = new List<bool>();

            foreach (var annotation in this.ResolveFromRequestCache(siteDirectory.Annotation))
            {
                results.Add(await this.AnnotationService.CreateConceptAsync(transaction, partition, annotation, siteDirectory));
            }

            foreach (var domain in this.ResolveFromRequestCache(siteDirectory.Domain))
            {
                results.Add(await this.DomainService.CreateConceptAsync(transaction, partition, domain, siteDirectory));
            }

            foreach (var domainGroup in this.ResolveFromRequestCache(siteDirectory.DomainGroup))
            {
                results.Add(await this.DomainGroupService.CreateConceptAsync(transaction, partition, domainGroup, siteDirectory));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(siteDirectory.LogEntry))
            {
                results.Add(await this.LogEntryService.CreateConceptAsync(transaction, partition, logEntry, siteDirectory));
            }

            foreach (var model in this.ResolveFromRequestCache(siteDirectory.Model))
            {
                results.Add(await this.ModelService.CreateConceptAsync(transaction, partition, model, siteDirectory));
            }

            foreach (var naturalLanguage in this.ResolveFromRequestCache(siteDirectory.NaturalLanguage))
            {
                results.Add(await this.NaturalLanguageService.CreateConceptAsync(transaction, partition, naturalLanguage, siteDirectory));
            }

            foreach (var organization in this.ResolveFromRequestCache(siteDirectory.Organization))
            {
                results.Add(await this.OrganizationService.CreateConceptAsync(transaction, partition, organization, siteDirectory));
            }

            foreach (var participantRole in this.ResolveFromRequestCache(siteDirectory.ParticipantRole))
            {
                results.Add(await this.ParticipantRoleService.CreateConceptAsync(transaction, partition, participantRole, siteDirectory));
            }

            foreach (var person in this.ResolveFromRequestCache(siteDirectory.Person))
            {
                results.Add(await this.PersonService.CreateConceptAsync(transaction, partition, person, siteDirectory));
            }

            foreach (var personRole in this.ResolveFromRequestCache(siteDirectory.PersonRole))
            {
                results.Add(await this.PersonRoleService.CreateConceptAsync(transaction, partition, personRole, siteDirectory));
            }

            foreach (var siteReferenceDataLibrary in this.ResolveFromRequestCache(siteDirectory.SiteReferenceDataLibrary))
            {
                results.Add(await this.SiteReferenceDataLibraryService.CreateConceptAsync(transaction, partition, siteReferenceDataLibrary, siteDirectory));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="SiteDirectory"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="siteDirectory">
        /// The <see cref="SiteDirectory"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> UpsertContainmentAsync(NpgsqlTransaction transaction, string partition, SiteDirectory siteDirectory)
        {
            var results = new List<bool>();

            foreach (var annotation in this.ResolveFromRequestCache(siteDirectory.Annotation))
            {
                results.Add(await this.AnnotationService.UpsertConceptAsync(transaction, partition, annotation, siteDirectory));
            }

            foreach (var domain in this.ResolveFromRequestCache(siteDirectory.Domain))
            {
                results.Add(await this.DomainService.UpsertConceptAsync(transaction, partition, domain, siteDirectory));
            }

            foreach (var domainGroup in this.ResolveFromRequestCache(siteDirectory.DomainGroup))
            {
                results.Add(await this.DomainGroupService.UpsertConceptAsync(transaction, partition, domainGroup, siteDirectory));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(siteDirectory.LogEntry))
            {
                results.Add(await this.LogEntryService.UpsertConceptAsync(transaction, partition, logEntry, siteDirectory));
            }

            foreach (var model in this.ResolveFromRequestCache(siteDirectory.Model))
            {
                results.Add(await this.ModelService.UpsertConceptAsync(transaction, partition, model, siteDirectory));
            }

            foreach (var naturalLanguage in this.ResolveFromRequestCache(siteDirectory.NaturalLanguage))
            {
                results.Add(await this.NaturalLanguageService.UpsertConceptAsync(transaction, partition, naturalLanguage, siteDirectory));
            }

            foreach (var organization in this.ResolveFromRequestCache(siteDirectory.Organization))
            {
                results.Add(await this.OrganizationService.UpsertConceptAsync(transaction, partition, organization, siteDirectory));
            }

            foreach (var participantRole in this.ResolveFromRequestCache(siteDirectory.ParticipantRole))
            {
                results.Add(await this.ParticipantRoleService.UpsertConceptAsync(transaction, partition, participantRole, siteDirectory));
            }

            foreach (var person in this.ResolveFromRequestCache(siteDirectory.Person))
            {
                results.Add(await this.PersonService.UpsertConceptAsync(transaction, partition, person, siteDirectory));
            }

            foreach (var personRole in this.ResolveFromRequestCache(siteDirectory.PersonRole))
            {
                results.Add(await this.PersonRoleService.UpsertConceptAsync(transaction, partition, personRole, siteDirectory));
            }

            foreach (var siteReferenceDataLibrary in this.ResolveFromRequestCache(siteDirectory.SiteReferenceDataLibrary))
            {
                results.Add(await this.SiteReferenceDataLibraryService.UpsertConceptAsync(transaction, partition, siteReferenceDataLibrary, siteDirectory));
            }

            return results.All(x => x);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
