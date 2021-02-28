// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services.Authorization
{
    using Authentication;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The PermissionService interface.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Gets or sets the <see cref="Credentials"/> assigned to this service.
        /// </summary>
        Credentials Credentials { get; set; }

        /// <summary>
        /// Determines whether the typeName can be read.
        /// </summary>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <param name="securityContext">
        /// The security context of the current request.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// True if the given typeName can be read.
        /// </returns>
        bool CanRead(string typeName, ISecurityContext securityContext, string partition);

        /// <summary>
        /// Determines whether the <see cref="Thing"/> can be read. This method is exclusively to be used in the after hook of the services to determine
        /// permission based on ownership. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>True if the given <see cref="Thing"/> can be read.</returns>
        bool CanRead(NpgsqlTransaction transaction, Thing thing, string partition);

        /// <summary>
        /// Determines whether the <see cref="Thing"/> can be written. This method is exclusively to be used in the after hook of the services to determine
        /// permission based on ownership. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <param name="securityContext">
        /// The security context of the current request.
        /// </param>
        /// <returns>True if the given <see cref="Thing"/> can be written.</returns>
        bool CanWrite(NpgsqlTransaction transaction, Thing thing, string typeName, string partition, string modifyOperation, ISecurityContext securityContext);

        /// <summary>
        /// Determines whether a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to check whether it is own <see cref="Person"/>.</param>
        /// <returns>True if a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.</returns>
        bool IsOwner(NpgsqlTransaction transaction, Thing thing);
    }
}