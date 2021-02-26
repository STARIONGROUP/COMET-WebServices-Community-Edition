// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CDP4WebServiceAuthenticationConfiguration.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    using System;

    /// <summary>
    /// The CDP4 web service authentication configuration.
    /// </summary>
    public class CDP4WebServiceAuthenticationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CDP4WebServiceAuthenticationConfiguration"/> class.
        /// </summary>
        /// <param name="userValidator">
        /// A valid instance of <see cref="IUserValidator"/> class
        /// </param>
        /// <param name="realm">
        /// Basic authentication realm
        /// </param>
        /// <param name="userPromptBehaviour">
        /// Control when the browser should be instructed to prompt for credentials
        /// </param>
        /// <param name="cryptographyConfiguration">
        /// The cryptography configuration.
        /// </param>
        public CDP4WebServiceAuthenticationConfiguration(IUserValidator userValidator, string realm, UserPromptBehaviour userPromptBehaviour = UserPromptBehaviour.NonAjax, CryptographyConfiguration cryptographyConfiguration = null)
        {
            this.CryptographyConfiguration = cryptographyConfiguration ?? CryptographyConfiguration.Default;

            if (userValidator == null)
            {
                throw new ArgumentNullException("userValidator", "The userValidator must not be null");
            }

            if (string.IsNullOrEmpty(realm))
            {
                throw new ArgumentException("realm");
            }

            this.UserValidator = userValidator;
            this.Realm = realm;
            this.UserPromptBehaviour = userPromptBehaviour;
        }

        /// <summary>
        /// Gets the user validator
        /// </summary>
        public IUserValidator UserValidator { get; private set; }

        /// <summary>
        /// Gets the basic authentication realm
        /// </summary>
        public string Realm { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is required.
        /// </summary>
        /// <value>
        /// The flag that indicates whether SSL is required
        /// </value>
        public bool RequiresSsl { get; set; }

        /// <summary>
        /// Gets the domain of the cookie
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the path of the AUTH cookie
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Gets whether the browser should prompt the user for credentials
        /// </summary>
        public UserPromptBehaviour UserPromptBehaviour { get; private set; }
    }
}
