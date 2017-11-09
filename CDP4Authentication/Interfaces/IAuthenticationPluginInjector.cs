// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationPluginInjector.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface for the authenticator plugin injector.
    /// </summary>
    public interface IAuthenticationPluginInjector
    {
        /// <summary>
        /// Gets the list of plugins that this plugin injector holds.
        /// </summary>
        List<IAuthenticatorPlugin> Plugins { get; }

        /// <summary>
        /// Gets the ranked list of injectors that all the <see cref="IAuthenticatorPlugin"/> contain.
        /// </summary>
        List<IAuthenticatorConnector> Connectors { get; }
    }
}
