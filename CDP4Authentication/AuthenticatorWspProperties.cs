// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorWspProperties.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    /// <summary>
    /// Specific class for wsp authenticator properties that <see cref="IAuthenticatorPlugin"/>s might use.
    /// </summary>
    public class AuthenticatorWspProperties : IAuthenticatorWspProperties
    {
        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// true if this instance is enabled; otherwise, false.
        /// </value>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the server salts (WSP specific).
        /// </summary>
        public string[] ServerSalts { get; set; }
    }
}
