// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorConnector.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication.Contracts
{
    /// <summary>
    /// The abstract authenticator connector contract.
    /// </summary>
    /// <typeparam name="TProperties">
    /// Generic typed <see cref="IAuthenticatorProperties"/>.
    /// </typeparam>
    public abstract class AuthenticatorConnector<TProperties> : IAuthenticatorConnector
        where TProperties : IAuthenticatorProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorConnector{TProperties}"/> class.
        /// </summary>
        /// <param name="properties">
        /// The generic typed <see cref="IAuthenticatorProperties"/>.
        /// </param>
        /// <remarks>
        /// Force all concrete classes to have a constructor with properties of type T.
        /// </remarks>
        protected AuthenticatorConnector(TProperties properties)
        {
            this.ConnectorProperties = properties;
        }

        /// <summary>
        /// Gets the Connector Properties as <see cref="IAuthenticatorProperties"/>.
        /// </summary>
        public IAuthenticatorProperties Properties
        {
            get
            {
                return this.ConnectorProperties;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connection is up.
        /// </summary>
        public abstract bool IsUp { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is down.
        /// </summary>
        public abstract bool IsDown { get; set; }

        /// <summary>
        /// Gets or sets the status message of the <see cref="IAuthenticatorConnector"/>.
        /// </summary>
        public abstract string StatusMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="IAuthenticatorConnector"/> name for display purposes.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the generic typed connector properties to be used in any connector logic specific to type T.
        /// </summary>
        protected TProperties ConnectorProperties { get; private set; }

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
        public abstract bool Authenticate(AuthenticationPerson person, string password);
    }
}
