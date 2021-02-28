// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorPlugin.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

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
        /// Initializes a new instance of the <see cref="AuthenticatorPlugin{TProperties, TConnector}"/> class.
        /// </summary>
        protected AuthenticatorPlugin()
        {
            this.LoadConfig();
            this.InitializeConnectors();
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
            var appConfig = JsonConvert.DeserializeObject<AuthenticatorConfig<TProperties>>(json);

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
