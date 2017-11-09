// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for different types of authentication realm validators.
    /// </summary>
    public interface IAuthenticatorPlugin
    {
        /// <summary>
        /// Gets the connectors.
        /// </summary>
        List<IAuthenticatorConnector> Connectors { get; }
    }
}