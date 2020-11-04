// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorProperties.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    /// <summary>
    /// Base class for standard authenticator properties that all <see cref="IAuthenticatorPlugin"/>s use.
    /// </summary>
    public class AuthenticatorProperties : IAuthenticatorProperties
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
    }
}
