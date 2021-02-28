// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryService.cs" company="RHEA System S.A.">
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
        /// List of instances of <see cref="SiteDirectory"/>, optionally with contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? this.GetDeep(transaction, partition, ids, containerSecurityContext)
                        : this.GetShallow(transaction, partition, ids, containerSecurityContext);
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
        /// True if the persistence was successful.
        /// </returns>
        public bool Insert(NpgsqlTransaction transaction, string partition, Thing dto, Thing container = null)
        {
            var siteDirectory = dto as SiteDirectory;
            return this.CreateConcept(transaction, partition, siteDirectory, container);
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
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SiteDirectoryDao.AddToCollectionProperty(transaction, partition, propertyName, iid, value);
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
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SiteDirectoryDao.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);
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
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.SiteDirectoryDao.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
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
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, DeleteOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate delete permission for " + thing.GetType().Name + ".");
            }

            return this.SiteDirectoryDao.Delete(transaction, partition, thing.Iid);
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
        /// True if the update was successful.
        /// </returns>
        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate update permission for " + thing.GetType().Name + ".");
            }

            var siteDirectory = thing as SiteDirectory;
            return this.SiteDirectoryDao.Update(transaction, partition, siteDirectory, container);
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
        /// True if the persistence was successful.
        /// </returns>
        public bool CreateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var siteDirectory = thing as SiteDirectory;
            var createSuccesful = this.SiteDirectoryDao.Write(transaction, partition, siteDirectory, container);
            return createSuccesful && this.CreateContainment(transaction, partition, siteDirectory);
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
        /// List of instances of <see cref="SiteDirectory"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("SiteDirectory", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var siteDirectoryColl = new List<Thing>(this.SiteDirectoryDao.Read(transaction, partition, idFilter, this.TransactionManager.IsCachedDtoReadEnabled(transaction)));

            return this.AfterGet(siteDirectoryColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="SiteDirectory"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(this.GetShallow(transaction, partition, idFilter, containerSecurityContext));
            var siteDirectoryColl = results.Where(i => i.GetType() == typeof(SiteDirectory)).Cast<SiteDirectory>().ToList();

            results.AddRange(this.AnnotationService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Annotation), containerSecurityContext));
            results.AddRange(this.DomainService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Domain), containerSecurityContext));
            results.AddRange(this.DomainGroupService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.DomainGroup), containerSecurityContext));
            results.AddRange(this.LogEntryService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.LogEntry), containerSecurityContext));
            results.AddRange(this.ModelService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Model), containerSecurityContext));
            results.AddRange(this.NaturalLanguageService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.NaturalLanguage), containerSecurityContext));
            results.AddRange(this.OrganizationService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Organization), containerSecurityContext));
            results.AddRange(this.ParticipantRoleService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.ParticipantRole), containerSecurityContext));
            results.AddRange(this.PersonService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Person), containerSecurityContext));
            results.AddRange(this.PersonRoleService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.PersonRole), containerSecurityContext));
            results.AddRange(this.SiteReferenceDataLibraryService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.SiteReferenceDataLibrary), containerSecurityContext));

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
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContainment(NpgsqlTransaction transaction, string partition, SiteDirectory siteDirectory)
        {
            var results = new List<bool>();

            foreach (var annotation in this.ResolveFromRequestCache(siteDirectory.Annotation))
            {
                results.Add(this.AnnotationService.CreateConcept(transaction, partition, annotation, siteDirectory));
            }

            foreach (var domain in this.ResolveFromRequestCache(siteDirectory.Domain))
            {
                results.Add(this.DomainService.CreateConcept(transaction, partition, domain, siteDirectory));
            }

            foreach (var domainGroup in this.ResolveFromRequestCache(siteDirectory.DomainGroup))
            {
                results.Add(this.DomainGroupService.CreateConcept(transaction, partition, domainGroup, siteDirectory));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(siteDirectory.LogEntry))
            {
                results.Add(this.LogEntryService.CreateConcept(transaction, partition, logEntry, siteDirectory));
            }

            foreach (var model in this.ResolveFromRequestCache(siteDirectory.Model))
            {
                results.Add(this.ModelService.CreateConcept(transaction, partition, model, siteDirectory));
            }

            foreach (var naturalLanguage in this.ResolveFromRequestCache(siteDirectory.NaturalLanguage))
            {
                results.Add(this.NaturalLanguageService.CreateConcept(transaction, partition, naturalLanguage, siteDirectory));
            }

            foreach (var organization in this.ResolveFromRequestCache(siteDirectory.Organization))
            {
                results.Add(this.OrganizationService.CreateConcept(transaction, partition, organization, siteDirectory));
            }

            foreach (var participantRole in this.ResolveFromRequestCache(siteDirectory.ParticipantRole))
            {
                results.Add(this.ParticipantRoleService.CreateConcept(transaction, partition, participantRole, siteDirectory));
            }

            foreach (var person in this.ResolveFromRequestCache(siteDirectory.Person))
            {
                results.Add(this.PersonService.CreateConcept(transaction, partition, person, siteDirectory));
            }

            foreach (var personRole in this.ResolveFromRequestCache(siteDirectory.PersonRole))
            {
                results.Add(this.PersonRoleService.CreateConcept(transaction, partition, personRole, siteDirectory));
            }

            foreach (var siteReferenceDataLibrary in this.ResolveFromRequestCache(siteDirectory.SiteReferenceDataLibrary))
            {
                results.Add(this.SiteReferenceDataLibraryService.CreateConcept(transaction, partition, siteReferenceDataLibrary, siteDirectory));
            }

            return results.All(x => x);
        }
    }
}
