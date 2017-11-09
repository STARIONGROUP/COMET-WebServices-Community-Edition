// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorConfig.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds basic authenticator plugin configuration properties. 
    /// </summary>
    /// <typeparam name="TProperties">
    /// Generic typed <see cref="IAuthenticatorProperties"/>
    /// </typeparam>
    public class AuthenticatorConfig<TProperties>
        where TProperties : IAuthenticatorProperties
    {
        /// <summary>
        /// Gets or sets the list of properties of this authenticator. Each <see cref="IAuthenticatorProperties"/> instance constitutes a connector.
        /// </summary>
        public List<TProperties> AuthenticatorConnectorProperties { get; set; }
    }
}
