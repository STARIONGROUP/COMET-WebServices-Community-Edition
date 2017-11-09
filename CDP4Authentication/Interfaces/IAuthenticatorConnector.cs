// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticatorConnector.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    /// <summary>
    /// Defines a single authentication connector.
    /// </summary>
    public interface IAuthenticatorConnector
    {
        /// <summary>
        /// Gets the <see cref="IAuthenticatorProperties"/> of this connector.
        /// </summary>
        IAuthenticatorProperties Properties { get; }

        /// <summary>
        /// Gets a value indicating whether the connector is up.
        /// </summary>
        bool IsUp { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the connector is down.
        /// </summary>
        bool IsDown { get; set; }

        /// <summary>
        /// Gets or sets the status message of the <see cref="IAuthenticatorConnector"/>.
        /// </summary>
        string StatusMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="IAuthenticatorConnector"/> name for display purposes.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Authenticate the <see cref="AuthenticationPerson"/> information against the supplied password.
        /// </summary>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> information to authenticate.
        /// </param>
        /// <param name="password">
        /// The password to authenticate against.
        /// </param>
        /// <returns>
        /// True if the <see cref="AuthenticationPerson"/> could be authenticated.
        /// </returns>
        bool Authenticate(AuthenticationPerson person, string password);
    }
}
