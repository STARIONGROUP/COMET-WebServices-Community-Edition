// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorPlugin.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Generic abstract Authentication plugin definition.
    /// </summary>
    /// <typeparam name="TProperties">
    /// Generic typed <see cref="IAuthenticatorProperties"/>
    /// </typeparam>
    /// <typeparam name="TConnector">
    /// Generic typed <see cref="AuthenticatorConnector{TProperties}"/>
    /// </typeparam>
    public abstract class AuthenticatorPlugin<TProperties, TConnector> : IAuthenticatorPlugin
        where TProperties : IAuthenticatorProperties
        where TConnector : AuthenticatorConnector<TProperties>
    {
        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> to be used on serialization
        /// </summary>
        protected JsonSerializerOptions JsonSerializerOptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorPlugin{TProperties, TConnector}"/> class.
        /// </summary>
        protected AuthenticatorPlugin()
        {
            this.LoadConfig();
            this.InitializeConnectors();
            this.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
            this.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false));
            this.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        /// <summary>
        /// Gets or sets the connectors.
        /// </summary>
        public List<IAuthenticatorConnector> Connectors { get; protected set; }

        /// <summary>
        /// Gets the name of the configuration file.
        /// </summary>
        /// <remarks>
        /// Refactored this into a property to allow overrides in inherited classes.
        /// </remarks>
        protected virtual string Configpath
        {
            get { return "config.json"; }
        }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticatorConfig{T}"/> which holds all configuration data.
        /// </summary>
        protected AuthenticatorConfig<TProperties> AuthenticatorConfig { get; set; }

        /// <summary>
        /// Read configuration from file.
        /// </summary>
        protected void LoadConfig()
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

            var json = File.ReadAllText(configLocation);

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new JsonException($"Configuration file {configLocation} is empty.");
            }

            this.AuthenticatorConfig = this.DeserializeConfigFile(json, configLocation);
        }

        /// <summary>
        /// Deserialize the configuration file into <see cref="AuthenticatorConfig{T}"/>.
        /// </summary>
        /// <param name="json">
        /// The JSON to deserialize.
        /// </param>
        /// <param name="configLocation">
        /// The location of the file.
        /// </param>
        /// <returns>
        /// The <see cref="AuthenticatorConfig{T}"/>.
        /// The deserialized authenticator configuration
        /// </returns>
        protected virtual AuthenticatorConfig<TProperties> DeserializeConfigFile(string json, string configLocation)
        {
            var appConfig = JsonSerializer.Deserialize<AuthenticatorConfig<TProperties>>(json, this.JsonSerializerOptions);

            if (appConfig == null)
            {
                throw new JsonException($"Configuration file {configLocation} holds invalid settings.");
            }

            return appConfig;
        }

        /// <summary>
        /// Initializes all the connectors defined in this plugin configuration.
        /// </summary>
        protected void InitializeConnectors()
        {
            this.Connectors = new List<IAuthenticatorConnector>();

            foreach (var authenticatorProperties in this.AuthenticatorConfig.AuthenticatorConnectorProperties)
            {
                var connector = (TConnector) Activator.CreateInstance(typeof(TConnector), authenticatorProperties);
                this.Connectors.Add(connector);
            }
        }
    }
}
