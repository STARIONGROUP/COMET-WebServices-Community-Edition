// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightKindService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using CDP4Common.CommonData;
    using CDP4WebServices.API.Services.Authentication;

    /// <summary>
    /// The purpose of the <see cref="IAccessRightKindService"/> is to provide a service that returns the <see cref="PersonAccessRightKind"/>
    /// and <see cref="ParticipantAccessRightKind"/> for a specific type
    /// </summary>
    /// <remarks>
    /// the implementation should be a caching service and is therefore only valid per request
    /// </remarks>
    public interface IAccessRightKindService
    {
        /// <summary>
        /// Queries <see cref="PersonAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="IAccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns>
        /// <see cref="PersonAccessRightKind"/> for the supplied object type
        /// </returns>
        PersonAccessRightKind QueryPersonAccessRightKind(Credentials credentials, string typeName);

        /// <summary>
        /// Queries <see cref="ParticipantAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="IAccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="ParticipantAccessRightKind"/> for the supplied object type.</returns>
        ParticipantAccessRightKind QueryParticipantAccessRightKind(Credentials credentials, string typeName);
    }
}