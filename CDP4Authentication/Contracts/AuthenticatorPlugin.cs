// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using Newtonsoft.Json;

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
        private AuthenticatorConfig<TProperties> AuthenticatorConfig { get; set; }

        /// <summary>
        /// Read configuration from file.
        /// </summary>
        protected virtual void LoadConfig()
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
        private void InitializeConnectors()
        {
            this.Connectors = new List<IAuthenticatorConnector>();

            foreach (var authenticatorProperties in this.AuthenticatorConfig.AuthenticatorConnectorProperties)
            {
                var connector = (TConnector)Activator.CreateInstance(typeof(TConnector), authenticatorProperties);
                this.Connectors.Add(connector);
            }
        }
    }
}
