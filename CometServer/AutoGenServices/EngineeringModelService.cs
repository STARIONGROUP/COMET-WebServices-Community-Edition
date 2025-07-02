// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelService.cs" company="Starion Group S.A.">
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
    /// The <see cref="EngineeringModel"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class EngineeringModelService : ServiceBase, IEngineeringModelService
    {
        /// <summary>
        /// Gets or sets the <see cref="IBookService"/>.
        /// </summary>
        public IBookService BookService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICommonFileStoreService"/>.
        /// </summary>
        public ICommonFileStoreService CommonFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelDataNoteService"/>.
        /// </summary>
        public IEngineeringModelDataNoteService GenericNoteService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModelLogEntryService"/>.
        /// </summary>
        public IModelLogEntryService LogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModellingAnnotationItemService"/>.
        /// </summary>
        public IModellingAnnotationItemService ModellingAnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelDao"/>.
        /// </summary>
        public IEngineeringModelDao EngineeringModelDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="EngineeringModel"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="EngineeringModel"/>, optionally with contained <see cref="Thing"/>s as result.
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
        public Task<bool> InsertAsync(NpgsqlTransaction transaction, string partition, Thing dto, Thing container = null)
        {
            var engineeringModel = dto as EngineeringModel;
            return this.CreateConceptAsync(transaction, partition, engineeringModel, container);
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
        /// The <see cref="EngineeringModel"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.EngineeringModelDao.AddToCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="EngineeringModel"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.EngineeringModelDao.DeleteFromCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="EngineeringModel"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.EngineeringModelDao.ReorderCollectionPropertyAsync(transaction, partition, propertyName, iid, orderUpdate);
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
        /// Delete the supplied <see cref="EngineeringModel"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="EngineeringModel"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="EngineeringModel"/> to be removed.
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

            return await this.EngineeringModelDao.DeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="EngineeringModel"/> instance.
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
        /// The <see cref="EngineeringModel"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="EngineeringModel"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return this.EngineeringModelDao.RawDeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="EngineeringModel"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="EngineeringModel"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="EngineeringModel"/> to be updated.
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

            var engineeringModel = thing as EngineeringModel;
            return await this.EngineeringModelDao.UpdateAsync(transaction, partition, engineeringModel, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="EngineeringModel"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="EngineeringModel"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="EngineeringModel"/> to be persisted.
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

            var engineeringModel = thing as EngineeringModel;
            var createSuccesful = await this.EngineeringModelDao.WriteAsync(transaction, partition, engineeringModel, container);
            return createSuccesful && await this.CreateContainmentAsync(transaction, partition, engineeringModel);
        }

        /// <summary>
        /// Persist the supplied <see cref="EngineeringModel"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="EngineeringModel"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="EngineeringModel"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var engineeringModel = thing as EngineeringModel;
            var createSuccesful = await this.EngineeringModelDao.UpsertAsync(transaction, partition, engineeringModel, container);
            return createSuccesful && await this.UpsertContainmentAsync(transaction, partition, engineeringModel);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="EngineeringModel"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("EngineeringModel", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var isCachedDtoReadEnabled = await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction);
            var sessionInstant = (DateTime)await this.TransactionManager.GetRawSessionInstantAsync(transaction);
            var engineeringModelColl = new List<Thing>(await this.EngineeringModelDao.ReadAsync(transaction, partition, idFilter, isCachedDtoReadEnabled, sessionInstant));

            return await this.AfterGetAsync(engineeringModelColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="EngineeringModel"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(await this.GetShallowAsync(transaction, partition, idFilter, containerSecurityContext));
            var engineeringModelColl = results.Where(i => i.GetType() == typeof(EngineeringModel)).Cast<EngineeringModel>().ToList();

            results.AddRange(await this.BookService.GetDeepAsync(transaction, partition, engineeringModelColl.SelectMany(x => x.Book).ToIdList(), containerSecurityContext));
            results.AddRange(await this.CommonFileStoreService.GetDeepAsync(transaction, partition, engineeringModelColl.SelectMany(x => x.CommonFileStore), containerSecurityContext));
            results.AddRange(await this.GenericNoteService.GetDeepAsync(transaction, partition, engineeringModelColl.SelectMany(x => x.GenericNote), containerSecurityContext));
            results.AddRange(await this.LogEntryService.GetDeepAsync(transaction, partition, engineeringModelColl.SelectMany(x => x.LogEntry), containerSecurityContext));
            results.AddRange(await this.ModellingAnnotationService.GetDeepAsync(transaction, partition, engineeringModelColl.SelectMany(x => x.ModellingAnnotation), containerSecurityContext));

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
        /// Persist the <see cref="EngineeringModel"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="engineeringModel">
        /// The <see cref="EngineeringModel"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> CreateContainmentAsync(NpgsqlTransaction transaction, string partition, EngineeringModel engineeringModel)
        {
            var results = new List<bool>();

            foreach (var book in this.ResolveFromRequestCache(engineeringModel.Book))
            {
                results.Add(await this.BookService.CreateConceptAsync(transaction, partition, (Book)book.V, engineeringModel, book.K));
            }

            foreach (var commonFileStore in this.ResolveFromRequestCache(engineeringModel.CommonFileStore))
            {
                results.Add(await this.CommonFileStoreService.CreateConceptAsync(transaction, partition, commonFileStore, engineeringModel));
            }

            foreach (var genericNote in this.ResolveFromRequestCache(engineeringModel.GenericNote))
            {
                results.Add(await this.GenericNoteService.CreateConceptAsync(transaction, partition, genericNote, engineeringModel));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(engineeringModel.LogEntry))
            {
                results.Add(await this.LogEntryService.CreateConceptAsync(transaction, partition, logEntry, engineeringModel));
            }

            foreach (var modellingAnnotation in this.ResolveFromRequestCache(engineeringModel.ModellingAnnotation))
            {
                results.Add(await this.ModellingAnnotationService.CreateConceptAsync(transaction, partition, modellingAnnotation, engineeringModel));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="EngineeringModel"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="engineeringModel">
        /// The <see cref="EngineeringModel"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> UpsertContainmentAsync(NpgsqlTransaction transaction, string partition, EngineeringModel engineeringModel)
        {
            var results = new List<bool>();

            foreach (var book in this.ResolveFromRequestCache(engineeringModel.Book))
            {
                results.Add(await this.BookService.UpsertConceptAsync(transaction, partition, (Book)book.V, engineeringModel, book.K));
            }

            foreach (var commonFileStore in this.ResolveFromRequestCache(engineeringModel.CommonFileStore))
            {
                results.Add(await this.CommonFileStoreService.UpsertConceptAsync(transaction, partition, commonFileStore, engineeringModel));
            }

            foreach (var genericNote in this.ResolveFromRequestCache(engineeringModel.GenericNote))
            {
                results.Add(await this.GenericNoteService.UpsertConceptAsync(transaction, partition, genericNote, engineeringModel));
            }

            foreach (var logEntry in this.ResolveFromRequestCache(engineeringModel.LogEntry))
            {
                results.Add(await this.LogEntryService.UpsertConceptAsync(transaction, partition, logEntry, engineeringModel));
            }

            foreach (var modellingAnnotation in this.ResolveFromRequestCache(engineeringModel.ModellingAnnotation))
            {
                results.Add(await this.ModellingAnnotationService.UpsertConceptAsync(transaction, partition, modellingAnnotation, engineeringModel));
            }

            return results.All(x => x);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
