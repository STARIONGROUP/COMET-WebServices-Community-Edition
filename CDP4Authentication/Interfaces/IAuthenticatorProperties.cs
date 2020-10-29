// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticatorProperties.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    /// <summary>
    /// Interface for standard authenticator properties.
    /// </summary>
    public interface IAuthenticatorProperties
    {
        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        int Rank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// true if this instance is enabled; otherwise, false.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the server salts(WSP specific).
        /// </summary>
        string[] ServerSalts { get; set; }
    }
}