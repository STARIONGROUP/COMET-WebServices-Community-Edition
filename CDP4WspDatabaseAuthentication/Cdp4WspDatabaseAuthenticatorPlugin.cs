// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4WspDatabaseAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication
{
    using System.IO;
    using System.Linq;

    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines an authentication plugin against the standard CDP4 database using wsp hash algorithm.
    /// </summary>
    public class Cdp4WspDatabaseAuthenticatorPlugin : AuthenticatorPlugin<AuthenticatorWspProperties, Cdp4WspDatabaseAuthenticatorConnector>, IUpdateConfiguration
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

        /// <summary>
        /// Write server salt value to config.json
        /// </summary>
        /// <param name="propertyName">property name that will be updated</param>
        /// <param name="propertyValue">property value</param>
        /// <returns>True if operation finished with success</returns>
        private void WriteConfig(string propertyName, string propertyValue)
        {
            var assemblyPath = Path.GetDirectoryName(this.GetType().Assembly.Location);

            if (assemblyPath == null)
            {
                throw new DirectoryNotFoundException("The assembly path could not be resolved.");
            }

            var configLocation = Path.Combine(assemblyPath, this.Configpath);

            if (!File.Exists(configLocation))
            {
                throw new FileNotFoundException("Configuration file not found.", configLocation);
            }

            var connectorProperties = this.AuthenticatorConfig.AuthenticatorConnectorProperties.Select(connectorProperty =>
            {
                if (!(JToken.FromObject(connectorProperty) is JObject o))
                {
                    return connectorProperty;
                }

                foreach (var property in o.Properties())
                {
                    if (property.Name != propertyName)
                    {
                        continue;
                    }

                    if (property.Value.GetType() == typeof(JArray))
                    {
                        property.Value = new JArray(propertyValue);
                    }
                    else
                    {
                        property.Value = propertyValue;
                    }

                    connectorProperty = (AuthenticatorWspProperties)o.ToObject(typeof(AuthenticatorWspProperties));

                    break;
                }

                return connectorProperty;
            }).ToList();

            this.AuthenticatorConfig.AuthenticatorConnectorProperties = connectorProperties;
            File.WriteAllText(configLocation, JsonConvert.SerializeObject(this.AuthenticatorConfig, Formatting.Indented));
        }
    }
}
