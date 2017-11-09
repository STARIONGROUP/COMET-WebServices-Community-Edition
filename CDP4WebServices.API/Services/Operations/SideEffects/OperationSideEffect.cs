// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// Abstract super class from which all SideEffect classes derive.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type T that must be of type <see cref="Thing"/>
    /// </typeparam>
    /// <remarks>
    /// Only one SideEffect sub-class per DTO type is allowed.
    /// </remarks>
    public abstract class OperationSideEffect<T> : IOperationSideEffect
        where T : Thing
    {
        /// <summary>
        /// Gets or sets the request utils information
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the permission service.
        /// </summary>
        public IPermissionService PermissionService { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets the <see cref="Thing"/> type name of this generically typed <see cref="OperationSideEffect{T}"/>.
        /// </summary>
        /// <returns>
        /// The type name.
        /// </returns>
        /// <remarks>
        /// The result is used to register this instance in the <see cref="OperationSideEffectProcessor"/> map.
        /// </remarks>
        public string RegistryKey
        {
            get
            {
                return typeof(T).Name;
            }
        }

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public virtual IEnumerable<string> DeferPropertyValidation
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Predicate to determine if a property will be validated.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> for which to check if the property validation is to be included.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// true if the passed in property name is to be validated,
        /// false to skip validation
        /// </returns>
        public bool ValidateProperty(Thing thing, string propertyName)
        {
            if (this.DeferPropertyValidation == null)
            {
                return true;
            }

            return !this.DeferPropertyValidation.Contains(propertyName);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public virtual void BeforeCreate(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public virtual void AfterCreate(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public virtual void BeforeUpdate(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public virtual void AfterUpdate(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public virtual void BeforeDelete(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public virtual void AfterDelete(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Pattern boiler plate method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded before create function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void BeforeCreate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.BeforeCreate((T)thing, container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Pattern boiler plate method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded after create function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterCreate(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.AfterCreate((T)thing, container, (T)originalThing, transaction, partition, securityContext);
        }

        /// <summary>
        /// Pattern boiler plate method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded before update function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw Update Info.
        /// </param>
        public void BeforeUpdate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            this.BeforeUpdate((T)thing, container, transaction, partition, securityContext, rawUpdateInfo);
        }

        /// <summary>
        /// Pattern boiler plate method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded after update function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterUpdate(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.AfterUpdate((T)thing, container, (T)originalThing, transaction, partition, securityContext);
        }

        /// <summary>
        /// Pattern boiler plate method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded before delete function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void BeforeDelete(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.BeforeDelete((T)thing, container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Pattern boiler plate code method to wire-up the <see cref="IOperationSideEffect"/> to the generically typed overloaded after delete function.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterDelete(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.AfterDelete((T)thing, container, (T)originalThing, transaction, partition, securityContext);
        }
    }
}
