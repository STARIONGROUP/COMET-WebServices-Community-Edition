// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryService.cs" company="RHEA System S.A.">
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
    
    using CDP4Orm.Dao;
 
    using CDP4WebServices.API.Services.Authorization;
 
    using Npgsql;
 
    /// <summary>
    /// The SiteDirectory Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class SiteDirectoryService : ServiceBase, ISiteDirectoryService
    {
        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }
 
        /// <summary>
        /// Gets or sets the person service.
        /// </summary>
        public IPersonService PersonService { get; set; }
 
        /// <summary>
        /// Gets or sets the participantRole service.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }
 
        /// <summary>
        /// Gets or sets the siteReferenceDataLibrary service.
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }
 
        /// <summary>
        /// Gets or sets the engineeringModelSetup service.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }
 
        /// <summary>
        /// Gets or sets the personRole service.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }
 
        /// <summary>
        /// Gets or sets the siteLogEntry service.
        /// </summary>
        public ISiteLogEntryService SiteLogEntryService { get; set; }
 
        /// <summary>
        /// Gets or sets the domainOfExpertiseGroup service.
        /// </summary>
        public IDomainOfExpertiseGroupService DomainOfExpertiseGroupService { get; set; }
 
        /// <summary>
        /// Gets or sets the domainOfExpertise service.
        /// </summary>
        public IDomainOfExpertiseService DomainOfExpertiseService { get; set; }
 
        /// <summary>
        /// Gets or sets the naturalLanguage service.
        /// </summary>
        public INaturalLanguageService NaturalLanguageService { get; set; }
 
        /// <summary>
        /// Gets or sets the siteDirectoryDataAnnotation service.
        /// </summary>
        public ISiteDirectoryDataAnnotationService SiteDirectoryDataAnnotationService { get; set; }
 
        /// <summary>
        /// Gets or sets the siteDirectory dao.
        /// </summary>
        public ISiteDirectoryDao SiteDirectoryDao { get; set; }

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
        /// List of instances of <see cref="SiteDirectory"/>.
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
        /// The transaction object.
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
        /// The current transaction to the database.
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
        /// The current transaction to the database.
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
        /// The current transaction to the database.
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
        /// The <see cref="SiteDirectory"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be removed.
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

            var siteDirectory = thing as SiteDirectory;
            return this.SiteDirectoryDao.Update(transaction, partition, siteDirectory, container);
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

            var siteDirectory = thing as SiteDirectory;
            var createSuccesful = this.SiteDirectoryDao.Write(transaction, partition, siteDirectory, container);
            return createSuccesful && this.CreateContainment(transaction, partition, siteDirectory);
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
        /// List of instances of <see cref="SiteDirectory"/>.
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

            results.AddRange(this.OrganizationService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Organization), containerSecurityContext));
            results.AddRange(this.PersonService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Person), containerSecurityContext));
            results.AddRange(this.ParticipantRoleService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.ParticipantRole), containerSecurityContext));
            results.AddRange(this.SiteReferenceDataLibraryService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.SiteReferenceDataLibrary), containerSecurityContext));
            results.AddRange(this.EngineeringModelSetupService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Model), containerSecurityContext));
            results.AddRange(this.PersonRoleService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.PersonRole), containerSecurityContext));
            results.AddRange(this.SiteLogEntryService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.LogEntry), containerSecurityContext));
            results.AddRange(this.DomainOfExpertiseGroupService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.DomainGroup), containerSecurityContext));
            results.AddRange(this.DomainOfExpertiseService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Domain), containerSecurityContext));
            results.AddRange(this.NaturalLanguageService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.NaturalLanguage), containerSecurityContext));
            results.AddRange(this.SiteDirectoryDataAnnotationService.GetDeep(transaction, partition, siteDirectoryColl.SelectMany(x => x.Annotation), containerSecurityContext));

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


        /// <summary>
        /// Persist the DTO composition to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="siteDirectory">
        /// The siteDirectory instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContainment(NpgsqlTransaction transaction, string partition, SiteDirectory siteDirectory)
        {
            var results = new List<bool>();

            foreach (var organization in this.ResolveFromRequestCache(siteDirectory.Organization))
            {
               results.Add(this.OrganizationService.CreateConcept(transaction, partition, organization, siteDirectory));
            }

            foreach (var person in this.ResolveFromRequestCache(siteDirectory.Person))
            {
               results.Add(this.PersonService.CreateConcept(transaction, partition, person, siteDirectory));
            }

            foreach (var participantRole in this.ResolveFromRequestCache(siteDirectory.ParticipantRole))
            {
               results.Add(this.ParticipantRoleService.CreateConcept(transaction, partition, participantRole, siteDirectory));
            }

            foreach (var siteReferenceDataLibrary in this.ResolveFromRequestCache(siteDirectory.SiteReferenceDataLibrary))
            {
               results.Add(this.SiteReferenceDataLibraryService.CreateConcept(transaction, partition, siteReferenceDataLibrary, siteDirectory));
            }

            foreach (var model in this.ResolveFromRequestCache(siteDirectory.Model))
            {
               results.Add(this.EngineeringModelSetupService.CreateConcept(transaction, partition, model, siteDirectory));
            }

            foreach (var personRole in this.ResolveFromRequestCache(siteDirectory.PersonRole))
            {
               results.Add(this.PersonRoleService.CreateConcept(transaction, partition, personRole, siteDirectory));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(siteDirectory.LogEntry))
            {
               results.Add(this.SiteLogEntryService.CreateConcept(transaction, partition, logEntry, siteDirectory));
            }

            foreach (var domainGroup in this.ResolveFromRequestCache(siteDirectory.DomainGroup))
            {
               results.Add(this.DomainOfExpertiseGroupService.CreateConcept(transaction, partition, domainGroup, siteDirectory));
            }

            foreach (var domain in this.ResolveFromRequestCache(siteDirectory.Domain))
            {
               results.Add(this.DomainOfExpertiseService.CreateConcept(transaction, partition, domain, siteDirectory));
            }

            foreach (var naturalLanguage in this.ResolveFromRequestCache(siteDirectory.NaturalLanguage))
            {
               results.Add(this.NaturalLanguageService.CreateConcept(transaction, partition, naturalLanguage, siteDirectory));
            }

            foreach (var annotation in this.ResolveFromRequestCache(siteDirectory.Annotation))
            {
               results.Add(this.SiteDirectoryDataAnnotationService.CreateConcept(transaction, partition, annotation, siteDirectory));
            }

            return results.All(x => x);
        }
    }
}
