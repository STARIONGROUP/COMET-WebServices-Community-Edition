// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationSideEffectFunctions.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Geren�, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Th�ate
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
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the OperationSideEffectFunctions interface is to provide a contract for additional logic to be executed during an operation.
    /// </summary>
    public interface IOperationSideEffectFunctions
    {
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
        bool ValidateProperty(Thing thing, string propertyName);

        /// <summary>
        /// Execute additional logic before a create operation.
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
        /// An awaitable <see cref="Task"/> having true if the create operation may continue, otherwise it shall be skipped as a result.
        /// </returns>
        Task<bool> BeforeCreateAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Execute additional logic after a successful create operation.
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
        /// <returns>
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        Task AfterCreateAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Execute additional logic before an update operation.
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
        /// <returns>
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        Task BeforeUpdateAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo);

        /// <summary>
        /// Execute additional logic after a successful update operation.
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
        /// <returns>
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        Task AfterUpdateAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Execute additional logic before a delete operation.
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
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        Task BeforeDeleteAsync(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Execute additional logic after a successful delete operation.
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
        /// <returns>
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        Task AfterDeleteAsync(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);
    }
}