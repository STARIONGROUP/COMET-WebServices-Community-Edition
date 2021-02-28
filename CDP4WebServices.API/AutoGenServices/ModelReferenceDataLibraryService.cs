// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryService.cs" company="RHEA System S.A.">
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
    /// The <see cref="ModelReferenceDataLibrary"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ModelReferenceDataLibraryService : ServiceBase, IModelReferenceDataLibraryService
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
        /// Gets or sets the <see cref="IModelReferenceDataLibraryDao"/>.
        /// </summary>
        public IModelReferenceDataLibraryDao ModelReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="ModelReferenceDataLibrary"/>s from the ORM layer.
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
        /// List of instances of <see cref="ModelReferenceDataLibrary"/>, optionally with contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep|| this.RequestUtils.QueryParameters.IncludeReferenceData
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
        /// The <see cref="ModelReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.ModelReferenceDataLibraryDao.AddToCollectionProperty(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="ModelReferenceDataLibrary"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.ModelReferenceDataLibraryDao.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);
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
        /// The <see cref="ModelReferenceDataLibrary"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.ModelReferenceDataLibraryDao.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
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
        /// Delete the supplied <see cref="ModelReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModelReferenceDataLibrary"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModelReferenceDataLibrary"/> to be removed.
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

            return this.ModelReferenceDataLibraryDao.Delete(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="ModelReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModelReferenceDataLibrary"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModelReferenceDataLibrary"/> to be updated.
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

            var modelReferenceDataLibrary = thing as ModelReferenceDataLibrary;
            return this.ModelReferenceDataLibraryDao.Update(transaction, partition, modelReferenceDataLibrary, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="ModelReferenceDataLibrary"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ModelReferenceDataLibrary"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ModelReferenceDataLibrary"/> to be persisted.
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

            var modelReferenceDataLibrary = thing as ModelReferenceDataLibrary;
            var createSuccesful = this.ModelReferenceDataLibraryDao.Write(transaction, partition, modelReferenceDataLibrary, container);
            return createSuccesful && this.CreateContainment(transaction, partition, modelReferenceDataLibrary);
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
        /// List of instances of <see cref="ModelReferenceDataLibrary"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("ModelReferenceDataLibrary", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var modelReferenceDataLibraryColl = new List<Thing>(this.ModelReferenceDataLibraryDao.Read(transaction, partition, idFilter, this.TransactionManager.IsCachedDtoReadEnabled(transaction)));

            return this.AfterGet(modelReferenceDataLibraryColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="ModelReferenceDataLibrary"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(this.GetShallow(transaction, partition, idFilter, containerSecurityContext));
            if (!this.RequestUtils.QueryParameters.IncludeReferenceData)
            {
                // if the includeReferenceData=true retrieval mode is requested 
                // eventhough extent=deep retrieve mode is requested this has to be e includeRference data needs to be set to true as well to get the deep result 
                return results;
            }
            var modelReferenceDataLibraryColl = results.Where(i => i.GetType() == typeof(ModelReferenceDataLibrary)).Cast<ModelReferenceDataLibrary>().ToList();

            results.AddRange(this.AliasService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Alias), containerSecurityContext));
            results.AddRange(this.ConstantService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Constant), containerSecurityContext));
            results.AddRange(this.DefinedCategoryService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.DefinedCategory), containerSecurityContext));
            results.AddRange(this.DefinitionService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Definition), containerSecurityContext));
            results.AddRange(this.FileTypeService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.FileType), containerSecurityContext));
            results.AddRange(this.GlossaryService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Glossary), containerSecurityContext));
            results.AddRange(this.HyperLinkService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.HyperLink), containerSecurityContext));
            results.AddRange(this.ParameterTypeService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.ParameterType), containerSecurityContext));
            results.AddRange(this.ReferenceSourceService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.ReferenceSource), containerSecurityContext));
            results.AddRange(this.RuleService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Rule), containerSecurityContext));
            results.AddRange(this.ScaleService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Scale), containerSecurityContext));
            results.AddRange(this.UnitService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.Unit), containerSecurityContext));
            results.AddRange(this.UnitPrefixService.GetDeep(transaction, partition, modelReferenceDataLibraryColl.SelectMany(x => x.UnitPrefix), containerSecurityContext));

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
        /// Persist the <see cref="ModelReferenceDataLibrary"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="modelReferenceDataLibrary">
        /// The <see cref="ModelReferenceDataLibrary"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContainment(NpgsqlTransaction transaction, string partition, ModelReferenceDataLibrary modelReferenceDataLibrary)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(modelReferenceDataLibrary.Alias))
            {
                results.Add(this.AliasService.CreateConcept(transaction, partition, alias, modelReferenceDataLibrary));
            }

            foreach (var constant in this.ResolveFromRequestCache(modelReferenceDataLibrary.Constant))
            {
                results.Add(this.ConstantService.CreateConcept(transaction, partition, constant, modelReferenceDataLibrary));
            }

            foreach (var definedCategory in this.ResolveFromRequestCache(modelReferenceDataLibrary.DefinedCategory))
            {
                results.Add(this.DefinedCategoryService.CreateConcept(transaction, partition, definedCategory, modelReferenceDataLibrary));
            }

            foreach (var definition in this.ResolveFromRequestCache(modelReferenceDataLibrary.Definition))
            {
                results.Add(this.DefinitionService.CreateConcept(transaction, partition, definition, modelReferenceDataLibrary));
            }

            foreach (var fileType in this.ResolveFromRequestCache(modelReferenceDataLibrary.FileType))
            {
                results.Add(this.FileTypeService.CreateConcept(transaction, partition, fileType, modelReferenceDataLibrary));
            }

            foreach (var glossary in this.ResolveFromRequestCache(modelReferenceDataLibrary.Glossary))
            {
                results.Add(this.GlossaryService.CreateConcept(transaction, partition, glossary, modelReferenceDataLibrary));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(modelReferenceDataLibrary.HyperLink))
            {
                results.Add(this.HyperLinkService.CreateConcept(transaction, partition, hyperLink, modelReferenceDataLibrary));
            }

            foreach (var parameterType in this.ResolveFromRequestCache(modelReferenceDataLibrary.ParameterType))
            {
                results.Add(this.ParameterTypeService.CreateConcept(transaction, partition, parameterType, modelReferenceDataLibrary));
            }

            foreach (var referenceSource in this.ResolveFromRequestCache(modelReferenceDataLibrary.ReferenceSource))
            {
                results.Add(this.ReferenceSourceService.CreateConcept(transaction, partition, referenceSource, modelReferenceDataLibrary));
            }

            foreach (var rule in this.ResolveFromRequestCache(modelReferenceDataLibrary.Rule))
            {
                results.Add(this.RuleService.CreateConcept(transaction, partition, rule, modelReferenceDataLibrary));
            }

            foreach (var scale in this.ResolveFromRequestCache(modelReferenceDataLibrary.Scale))
            {
                results.Add(this.ScaleService.CreateConcept(transaction, partition, scale, modelReferenceDataLibrary));
            }

            foreach (var unit in this.ResolveFromRequestCache(modelReferenceDataLibrary.Unit))
            {
                results.Add(this.UnitService.CreateConcept(transaction, partition, unit, modelReferenceDataLibrary));
            }

            foreach (var unitPrefix in this.ResolveFromRequestCache(modelReferenceDataLibrary.UnitPrefix))
            {
                results.Add(this.UnitPrefixService.CreateConcept(transaction, partition, unitPrefix, modelReferenceDataLibrary));
            }

            return results.All(x => x);
        }
    }
}
