// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4WspDatabaseAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication
{
    using System.Linq;

    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    /// <summary>
    /// Defines an authentication plugin against the standard CDP4 database using wsp hash algorithm.
    /// </summary>
    public class Cdp4WspDatabaseAuthenticatorPlugin : AuthenticatorPlugin<AuthenticatorProperties, Cdp4WspDatabaseAuthenticatorConnector>, IUpdateConfiguration
    {
        /// <summary>
        /// Iterate through all authenticator connector properties and update wsp salt list
        /// </summary>
        /// <param name="salt">WSP server salt that will be added if not exists</param>
        public void UpdateSaltList(string salt)
        {
            var properties = this.AuthenticatorConfig.AuthenticatorConnectorProperties;
            var newSalt = false;

            foreach (var property in properties)
            {
                var saltsList = property.ServerSalts.ToList();

                if (!saltsList.Contains(salt))
                {
                    saltsList.Add(salt);
                    newSalt = true;
                }

                if (!newSalt)
                {
                    continue;
                }

                this.WriteConfig("ServerSalts", string.Join(",", saltsList.ToArray()));
            }

            if (!newSalt)
            {
                return;
            }

            this.LoadConfig();
            this.InitializeConnectors();
        }
    }
}
