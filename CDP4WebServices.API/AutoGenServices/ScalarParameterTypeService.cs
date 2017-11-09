// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalarParameterTypeService.cs" company="RHEA System S.A.">
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
    /// The ScalarParameterType Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ScalarParameterTypeService : ServiceBase, IScalarParameterTypeService
    {
        /// <summary>
        /// Gets or sets the enumerationParameterType service.
        /// </summary>
        public IEnumerationParameterTypeService EnumerationParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the booleanParameterType service.
        /// </summary>
        public IBooleanParameterTypeService BooleanParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the dateParameterType service.
        /// </summary>
        public IDateParameterTypeService DateParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the textParameterType service.
        /// </summary>
        public ITextParameterTypeService TextParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the dateTimeParameterType service.
        /// </summary>
        public IDateTimeParameterTypeService DateTimeParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the timeOfDayParameterType service.
        /// </summary>
        public ITimeOfDayParameterTypeService TimeOfDayParameterTypeService { get; set; }
 
        /// <summary>
        /// Gets or sets the quantityKind service.
        /// </summary>
        public IQuantityKindService QuantityKindService { get; set; }

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
        /// List of instances of <see cref="ScalarParameterType"/>.
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
        /// The <see cref="ScalarParameterType"/> id that will be the source for each link table record.
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
        /// The <see cref="ScalarParameterType"/> id that is the source of the link table records.
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
        /// The <see cref="ScalarParameterType"/> id that is the source for the reordered link table record.
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
        /// The <see cref="ScalarParameterType"/> to delete.
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
               return this.EnumerationParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
               return this.BooleanParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
               return this.DateParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
               return this.TextParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
               return this.DateTimeParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
               return this.TimeOfDayParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
               return this.QuantityKindService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
               return this.EnumerationParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
               return this.BooleanParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
               return this.DateParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
               return this.TextParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
               return this.DateTimeParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
               return this.TimeOfDayParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
               return this.QuantityKindService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
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
        /// List of instances of <see cref="ScalarParameterType"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("ScalarParameterType", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var scalarParameterTypeColl = new List<Thing>();
            scalarParameterTypeColl.AddRange(this.EnumerationParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.BooleanParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.DateParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.TextParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.DateTimeParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.TimeOfDayParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.QuantityKindService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(scalarParameterTypeColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="ScalarParameterType"/>.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(this.EnumerationParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.BooleanParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DateParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TextParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DateTimeParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TimeOfDayParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.QuantityKindService.GetDeep(transaction, partition, idFilter, containerSecurityContext));

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
