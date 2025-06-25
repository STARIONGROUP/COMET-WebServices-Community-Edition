﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSideEffect.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services.Operations.SideEffects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services.Authorization;

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
        public virtual IEnumerable<string> DeferPropertyValidation => null;

        /// <summary>
        /// Gets or sets the <see cref="IIterationService" />
        /// </summary>
        public IOrganizationalParticipationResolverService OrganizationalParticipationResolverService { get; set; }

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
        /// <returns>
        /// Returns true if the create operation may continue, otherwise it shall be skipped.
        /// </returns>
        public virtual Task<bool> BeforeCreate(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return Task.FromResult(true);
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
        public virtual Task AfterCreate(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return Task.CompletedTask;
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
        public virtual Task BeforeUpdate(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            return Task.CompletedTask;
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
        public virtual Task AfterUpdate(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return Task.CompletedTask;
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
        public virtual Task BeforeDelete(T thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return Task.CompletedTask;
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
        public virtual Task AfterDelete(T thing, Thing container, T originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return Task.CompletedTask;
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
        public async Task<bool> BeforeCreateAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return await this.BeforeCreate((T)thing, container, transaction, partition, securityContext);
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
        public async Task AfterCreateAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.AfterCreate((T)thing, container, (T)originalThing, transaction, partition, securityContext);
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
        public async Task BeforeUpdateAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            await this.BeforeUpdate((T)thing, container, transaction, partition, securityContext, rawUpdateInfo);
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
        public async Task AfterUpdateAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.AfterUpdate((T)thing, container, (T)originalThing, transaction, partition, securityContext);
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
        public async Task BeforeDeleteAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.BeforeDelete((T)thing, container, transaction, partition, securityContext);
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
        public async Task AfterDeleteAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.AfterDelete((T)thing, container, (T)originalThing, transaction, partition, securityContext);
        }
    }
}
