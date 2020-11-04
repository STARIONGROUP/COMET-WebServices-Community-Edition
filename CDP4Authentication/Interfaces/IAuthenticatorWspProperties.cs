// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticatorWspProperties.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    /// <summary>
    /// Specific interface for wsp authenticator properties.
    /// </summary>
    public interface IAuthenticatorWspProperties : IAuthenticatorProperties
    {
        /// <summary>
        /// Gets or sets the server salts (WSP specific).
        /// </summary>
        string[] ServerSalts { get; set; }
    }
}