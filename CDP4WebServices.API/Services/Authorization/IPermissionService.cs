// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
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
    }
}