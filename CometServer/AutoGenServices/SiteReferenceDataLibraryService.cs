// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibraryService.cs" company="Starion Group S.A.">
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
    /// The <see cref="SiteReferenceDataLibrary"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class SiteReferenceDataLibraryService : ServiceBase, ISiteReferenceDataLibraryService
    {
        /// <summary>
        /// Gets or sets the <see cref="IAliasService"/>.
        /// </summary>
        public IAliasService AliasService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConstantService"/>.
        /// </summary>
        public IConstantService ConstantService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICategoryService"/>.
        /// </summary>
        public ICategoryService DefinedCategoryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefinitionService"/>.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileTypeService"/>.
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGlossaryService"/>.
        /// </summary>
        public IGlossaryService GlossaryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHyperLinkService"/>.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeService"/>.
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReferenceSourceService"/>.
        /// </summary>
        public IReferenceSourceService ReferenceSourceService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRuleService"/>.
        /// </summary>
        public IRuleService RuleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMeasurementScaleService"/>.
        /// </summary>
        public IMeasurementScaleService ScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMeasurementUnitService"/>.
        /// </summary>
        public IMeasurementUnitService UnitService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUnitPrefixService"/>.
        /// </summary>
        public IUnitPrefixService UnitPrefixService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryDao"/>.
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="SiteReferenceDataLibrary"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="SiteReferenceDataLibrary"/>, optionally with contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep|| this.RequestUtils.QueryParameters.IncludeReferenceData
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
        /// The <see cref="SiteReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SiteReferenceDataLibraryDao.AddToCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="SiteReferenceDataLibrary"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SiteReferenceDataLibraryDao.DeleteFromCollectionPropertyAsync(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="SiteReferenceDataLibrary"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.SiteReferenceDataLibraryDao.ReorderCollectionPropertyAsync(transaction, partition, propertyName, iid, orderUpdate);
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
        /// Delete the supplied <see cref="SiteReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteReferenceDataLibrary"/> to be removed.
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

            return await this.SiteReferenceDataLibraryDao.DeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="SiteReferenceDataLibrary"/> instance.
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
        /// The <see cref="SiteReferenceDataLibrary"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteReferenceDataLibrary"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return this.SiteReferenceDataLibraryDao.RawDeleteAsync(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="SiteReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteReferenceDataLibrary"/> to be updated.
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

            var siteReferenceDataLibrary = thing as SiteReferenceDataLibrary;
            return await this.SiteReferenceDataLibraryDao.UpdateAsync(transaction, partition, siteReferenceDataLibrary, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="SiteReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteReferenceDataLibrary"/> to be persisted.
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

            var siteReferenceDataLibrary = thing as SiteReferenceDataLibrary;
            var createSuccesful = await this.SiteReferenceDataLibraryDao.WriteAsync(transaction, partition, siteReferenceDataLibrary, container);
            return createSuccesful && await this.CreateContainmentAsync(transaction, partition, siteReferenceDataLibrary);
        }

        /// <summary>
        /// Persist the supplied <see cref="SiteReferenceDataLibrary"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SiteReferenceDataLibrary"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var siteReferenceDataLibrary = thing as SiteReferenceDataLibrary;
            var createSuccesful = await this.SiteReferenceDataLibraryDao.UpsertAsync(transaction, partition, siteReferenceDataLibrary, container);
            return createSuccesful && await this.UpsertContainmentAsync(transaction, partition, siteReferenceDataLibrary);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="SiteReferenceDataLibrary"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("SiteReferenceDataLibrary", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var siteReferenceDataLibraryColl = new List<Thing>(await this.SiteReferenceDataLibraryDao.ReadAsync(transaction, partition, idFilter, await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction), (DateTime)(await this.TransactionManager.GetRawSessionInstantAsync(transaction))));

            return await this.AfterGetAsync(siteReferenceDataLibraryColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="SiteReferenceDataLibrary"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(await this.GetShallowAsync(transaction, partition, idFilter, containerSecurityContext));
            if (!this.RequestUtils.QueryParameters.IncludeReferenceData)
            {
                // if the includeReferenceData=true retrieval mode is requested 
                // eventhough extent=deep retrieve mode is requested this has to be e includeRference data needs to be set to true as well to get the deep result 
                return results;
            }
            var siteReferenceDataLibraryColl = results.Where(i => i.GetType() == typeof(SiteReferenceDataLibrary)).Cast<SiteReferenceDataLibrary>().ToList();

            results.AddRange(await this.AliasService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Alias), containerSecurityContext));
            results.AddRange(await this.ConstantService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Constant), containerSecurityContext));
            results.AddRange(await this.DefinedCategoryService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.DefinedCategory), containerSecurityContext));
            results.AddRange(await this.DefinitionService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Definition), containerSecurityContext));
            results.AddRange(await this.FileTypeService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.FileType), containerSecurityContext));
            results.AddRange(await this.GlossaryService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Glossary), containerSecurityContext));
            results.AddRange(await this.HyperLinkService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.HyperLink), containerSecurityContext));
            results.AddRange(await this.ParameterTypeService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.ParameterType), containerSecurityContext));
            results.AddRange(await this.ReferenceSourceService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.ReferenceSource), containerSecurityContext));
            results.AddRange(await this.RuleService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Rule), containerSecurityContext));
            results.AddRange(await this.ScaleService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Scale), containerSecurityContext));
            results.AddRange(await this.UnitService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.Unit), containerSecurityContext));
            results.AddRange(await this.UnitPrefixService.GetDeepAsync(transaction, partition, siteReferenceDataLibraryColl.SelectMany(x => x.UnitPrefix), containerSecurityContext));

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
        /// Persist the <see cref="SiteReferenceDataLibrary"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="siteReferenceDataLibrary">
        /// The <see cref="SiteReferenceDataLibrary"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> CreateContainmentAsync(NpgsqlTransaction transaction, string partition, SiteReferenceDataLibrary siteReferenceDataLibrary)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(siteReferenceDataLibrary.Alias))
            {
                results.Add(await this.AliasService.CreateConceptAsync(transaction, partition, alias, siteReferenceDataLibrary));
            }

            foreach (var constant in this.ResolveFromRequestCache(siteReferenceDataLibrary.Constant))
            {
                results.Add(await this.ConstantService.CreateConceptAsync(transaction, partition, constant, siteReferenceDataLibrary));
            }

            foreach (var definedCategory in this.ResolveFromRequestCache(siteReferenceDataLibrary.DefinedCategory))
            {
                results.Add(await this.DefinedCategoryService.CreateConceptAsync(transaction, partition, definedCategory, siteReferenceDataLibrary));
            }

            foreach (var definition in this.ResolveFromRequestCache(siteReferenceDataLibrary.Definition))
            {
                results.Add(await this.DefinitionService.CreateConceptAsync(transaction, partition, definition, siteReferenceDataLibrary));
            }

            foreach (var fileType in this.ResolveFromRequestCache(siteReferenceDataLibrary.FileType))
            {
                results.Add(await this.FileTypeService.CreateConceptAsync(transaction, partition, fileType, siteReferenceDataLibrary));
            }

            foreach (var glossary in this.ResolveFromRequestCache(siteReferenceDataLibrary.Glossary))
            {
                results.Add(await this.GlossaryService.CreateConceptAsync(transaction, partition, glossary, siteReferenceDataLibrary));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(siteReferenceDataLibrary.HyperLink))
            {
                results.Add(await this.HyperLinkService.CreateConceptAsync(transaction, partition, hyperLink, siteReferenceDataLibrary));
            }

            foreach (var parameterType in this.ResolveFromRequestCache(siteReferenceDataLibrary.ParameterType))
            {
                results.Add(await this.ParameterTypeService.CreateConceptAsync(transaction, partition, parameterType, siteReferenceDataLibrary));
            }

            foreach (var referenceSource in this.ResolveFromRequestCache(siteReferenceDataLibrary.ReferenceSource))
            {
                results.Add(await this.ReferenceSourceService.CreateConceptAsync(transaction, partition, referenceSource, siteReferenceDataLibrary));
            }

            foreach (var rule in this.ResolveFromRequestCache(siteReferenceDataLibrary.Rule))
            {
                results.Add(await this.RuleService.CreateConceptAsync(transaction, partition, rule, siteReferenceDataLibrary));
            }

            foreach (var scale in this.ResolveFromRequestCache(siteReferenceDataLibrary.Scale))
            {
                results.Add(await this.ScaleService.CreateConceptAsync(transaction, partition, scale, siteReferenceDataLibrary));
            }

            foreach (var unit in this.ResolveFromRequestCache(siteReferenceDataLibrary.Unit))
            {
                results.Add(await this.UnitService.CreateConceptAsync(transaction, partition, unit, siteReferenceDataLibrary));
            }

            foreach (var unitPrefix in this.ResolveFromRequestCache(siteReferenceDataLibrary.UnitPrefix))
            {
                results.Add(await this.UnitPrefixService.CreateConceptAsync(transaction, partition, unitPrefix, siteReferenceDataLibrary));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="SiteReferenceDataLibrary"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="siteReferenceDataLibrary">
        /// The <see cref="SiteReferenceDataLibrary"/> instance to persist.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        private async Task<bool> UpsertContainmentAsync(NpgsqlTransaction transaction, string partition, SiteReferenceDataLibrary siteReferenceDataLibrary)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(siteReferenceDataLibrary.Alias))
            {
                results.Add(await this.AliasService.UpsertConceptAsync(transaction, partition, alias, siteReferenceDataLibrary));
            }

            foreach (var constant in this.ResolveFromRequestCache(siteReferenceDataLibrary.Constant))
            {
                results.Add(await this.ConstantService.UpsertConceptAsync(transaction, partition, constant, siteReferenceDataLibrary));
            }

            foreach (var definedCategory in this.ResolveFromRequestCache(siteReferenceDataLibrary.DefinedCategory))
            {
                results.Add(await this.DefinedCategoryService.UpsertConceptAsync(transaction, partition, definedCategory, siteReferenceDataLibrary));
            }

            foreach (var definition in this.ResolveFromRequestCache(siteReferenceDataLibrary.Definition))
            {
                results.Add(await this.DefinitionService.UpsertConceptAsync(transaction, partition, definition, siteReferenceDataLibrary));
            }

            foreach (var fileType in this.ResolveFromRequestCache(siteReferenceDataLibrary.FileType))
            {
                results.Add(await this.FileTypeService.UpsertConceptAsync(transaction, partition, fileType, siteReferenceDataLibrary));
            }

            foreach (var glossary in this.ResolveFromRequestCache(siteReferenceDataLibrary.Glossary))
            {
                results.Add(await this.GlossaryService.UpsertConceptAsync(transaction, partition, glossary, siteReferenceDataLibrary));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(siteReferenceDataLibrary.HyperLink))
            {
                results.Add(await this.HyperLinkService.UpsertConceptAsync(transaction, partition, hyperLink, siteReferenceDataLibrary));
            }

            foreach (var parameterType in this.ResolveFromRequestCache(siteReferenceDataLibrary.ParameterType))
            {
                results.Add(await this.ParameterTypeService.UpsertConceptAsync(transaction, partition, parameterType, siteReferenceDataLibrary));
            }

            foreach (var referenceSource in this.ResolveFromRequestCache(siteReferenceDataLibrary.ReferenceSource))
            {
                results.Add(await this.ReferenceSourceService.UpsertConceptAsync(transaction, partition, referenceSource, siteReferenceDataLibrary));
            }

            foreach (var rule in this.ResolveFromRequestCache(siteReferenceDataLibrary.Rule))
            {
                results.Add(await this.RuleService.UpsertConceptAsync(transaction, partition, rule, siteReferenceDataLibrary));
            }

            foreach (var scale in this.ResolveFromRequestCache(siteReferenceDataLibrary.Scale))
            {
                results.Add(await this.ScaleService.UpsertConceptAsync(transaction, partition, scale, siteReferenceDataLibrary));
            }

            foreach (var unit in this.ResolveFromRequestCache(siteReferenceDataLibrary.Unit))
            {
                results.Add(await this.UnitService.UpsertConceptAsync(transaction, partition, unit, siteReferenceDataLibrary));
            }

            foreach (var unitPrefix in this.ResolveFromRequestCache(siteReferenceDataLibrary.UnitPrefix))
            {
                results.Add(await this.UnitPrefixService.UpsertConceptAsync(transaction, partition, unitPrefix, siteReferenceDataLibrary));
            }

            return results.All(x => x);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
