// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalarParameterTypeService.cs" company="Starion Group S.A.">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The <see cref="ScalarParameterType"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class ScalarParameterTypeService : ServiceBase, IScalarParameterTypeService
    {
        /// <summary>
        /// Gets or sets the <see cref="IBooleanParameterTypeService"/>.
        /// </summary>
        public IBooleanParameterTypeService BooleanParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDateParameterTypeService"/>.
        /// </summary>
        public IDateParameterTypeService DateParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDateTimeParameterTypeService"/>.
        /// </summary>
        public IDateTimeParameterTypeService DateTimeParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEnumerationParameterTypeService"/>.
        /// </summary>
        public IEnumerationParameterTypeService EnumerationParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQuantityKindService"/>.
        /// </summary>
        public IQuantityKindService QuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITextParameterTypeService"/>.
        /// </summary>
        public ITextParameterTypeService TextParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITimeOfDayParameterTypeService"/>.
        /// </summary>
        public ITimeOfDayParameterTypeService TimeOfDayParameterTypeService { get; set; }

        /// <summary>
        /// Get the requested <see cref="ScalarParameterType"/>s from the ORM layer.
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="ScalarParameterType"/>, optionally with contained <see cref="Thing"/>s as result.
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
        /// The <see cref="ScalarParameterType"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
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
        /// The <see cref="ScalarParameterType"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was removed as result.
        /// </returns>
        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
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
        /// The <see cref="ScalarParameterType"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the link was created as result.
        /// </returns>
        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
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
        public Task<bool> ReorderContainmentAsync(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="ScalarParameterType"/> instance.
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
        /// The <see cref="ScalarParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the removal was successful as result.
        /// </returns>
        public Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be updated.
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return await this.BooleanParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return await this.DateParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return await this.DateTimeParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return await this.EnumerationParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return await this.QuantityKindService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return await this.TextParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return await this.TimeOfDayParameterTypeService.UpdateConceptAsync(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be persisted.
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return await this.BooleanParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return await this.DateParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return await this.DateTimeParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return await this.EnumerationParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return await this.QuantityKindService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return await this.TextParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return await this.TimeOfDayParameterTypeService.CreateConceptAsync(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ScalarParameterType"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the persistence was successful as result.
        /// </returns>
        public async Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return await this.BooleanParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return await this.DateParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return await this.DateTimeParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return await this.EnumerationParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return await this.QuantityKindService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return await this.TextParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return await this.TimeOfDayParameterTypeService.UpsertConceptAsync(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="ScalarParameterType"/> as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("ScalarParameterType", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && await this.BeforeGetAsync(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var scalarParameterTypeColl = new List<Thing>();
            scalarParameterTypeColl.AddRange(await this.BooleanParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.DateParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.DateTimeParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.EnumerationParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.QuantityKindService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.TextParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(await this.TimeOfDayParameterTypeService.GetShallowAsync(transaction, partition, idFilter, authorizedContext));

            return await this.AfterGetAsync(scalarParameterTypeColl, transaction, partition, idFilter);
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
        /// An awaitable <see cref="Task"/> having List of instances of <see cref="ScalarParameterType"/> and contained <see cref="Thing"/>s as result.
        /// </returns>
        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(await this.BooleanParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DateParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.DateTimeParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.EnumerationParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.QuantityKindService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.TextParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(await this.TimeOfDayParameterTypeService.GetDeepAsync(transaction, partition, idFilter, containerSecurityContext));
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
