// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserValidator.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    using Nancy.Security;

    /// <summary>
    /// Provides a way to validate the username and password
    /// </summary>
    public interface IUserValidator
    {
        /// <summary>
        /// Validates the username and password
        /// </summary>
        /// <param name="username">The Username</param>
        /// <param name="password">The Password</param>
        /// <returns>A value representing the authenticated user, null if the user was not authenticated.</returns>
        IUserIdentity Validate(string username, string password);
    }
}
