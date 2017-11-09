// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersonResolver.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authentication
{
    using CDP4WebService.Authentication;

    using Nancy.Security;

    using Npgsql;

    /// <summary>
    /// The PersonResolver interface.
    /// </summary>
    public interface IPersonResolver
    {
        /// <summary>
        /// Resolves the username to <see cref="IUserIdentity"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="username">
        /// The supplied username
        /// </param>
        /// <returns>
        /// A <see cref="IUserIdentity"/> representing the resolved user, null if the user was not found.
        /// </returns>
        IUserIdentity ResolvePerson(NpgsqlTransaction transaction, string username);

        /// <summary>
        /// Resolve and set participant information for the passed in <see cref="ICredentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="credentials">
        /// The supplied credential class which can hold participant information
        /// </param>
        void ResolveParticipantCredentials(NpgsqlTransaction transaction, ICredentials credentials);
    }
}
