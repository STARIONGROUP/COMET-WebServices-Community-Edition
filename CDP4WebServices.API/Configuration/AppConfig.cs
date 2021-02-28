// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="RHEA System S.A.">
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

namespace CometServer.Configuration
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    /// <summary>
    /// The application Configuration.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// The configuration file path.
        /// </summary>
        private const string Configpath = "config.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfig"/> class.
        /// </summary>
        protected AppConfig()
        {
            // make sure there defaults of the configuration sections are present
            this.Backtier = new BacktierConfig();
            this.Defaults = new DefaultsConfig();
            this.Midtier = new MidtierConfig();
            this.Changelog = new ChangelogConfig();
            this.EmailService = new EmailConfig();
        }

        /// <summary>
        /// Gets the current application configuration.
        /// </summary>
        public static AppConfig Current { get; private set; }

        /// <summary>
        /// Gets or sets the mid tier configuration.
        /// </summary>
        public MidtierConfig Midtier { get; set; }
        
        /// <summary>
        /// Gets or sets the back tier configuration.
        /// </summary>
        public BacktierConfig Backtier { get; set; }

        /// <summary>
        /// Gets or sets the email service configuration.
        /// </summary>
        public EmailConfig EmailService { get; set; }

        /// <summary>
        /// Gets or sets the default settings configuration.
        /// </summary>
        public DefaultsConfig Defaults { get; set; }

        /// <summary>
        /// Gets the current changelog configuration.
        /// </summary>
        public ChangelogConfig Changelog { get; set; }

        /// <summary>
        /// Read configuration from file.
        /// </summary>
        /// <param name="configPath">
        /// The config Path.
        /// </param>
        /// <remarks>
        /// This it the application configuration of the web server
        /// The configuration is read from a JSON config file located in the server root folder
        /// </remarks>
        public static void Load(string configPath = null)
        {
            var json = ReadConfigFile(string.IsNullOrEmpty(configPath) ? Configpath : configPath);

            var appConfig = JsonConvert.DeserializeObject<AppConfig>(json);

            ValidateConfiguration(appConfig);

            Current = appConfig;
        }

        /// <summary>
        /// Validate the loaded application configuration settings.
        /// </summary>
        /// <param name="appConfig">
        /// The app config.
        /// </param>
        /// <exception cref="ConfigurationErrorsException">
        /// If the configuration is invalid
        /// </exception>
        private static void ValidateConfiguration(AppConfig appConfig)
        {
            if (appConfig == null)
            {
                throw new ArgumentNullException("appConfig", "Configuration file holds invalid settings.");
            }

            if (appConfig.Backtier == null)
            {
                throw new ConfigurationErrorsException("The Backtier configuration is missing.", Configpath, 0);
            }

            if (appConfig.Midtier == null)
            {
                throw new ConfigurationErrorsException("The Midtier configuration is missing.", Configpath, 0);
            }

            if (appConfig.EmailService == null)
            {
                throw new ConfigurationErrorsException("The Email service configuration is missing.", Configpath, 0);
            }

            if (appConfig.Defaults == null)
            {
                throw new ConfigurationErrorsException("The Defaults configuration is missing.", Configpath, 0);
            }
        }

        /// <summary>
        /// Reads the configuration file from the provided path
        /// </summary>
        /// <param name="configPath">
        /// The path to the configuration file
        /// </param>
        /// <returns>
        /// A JSON string that contains the contents of the configuration file
        /// </returns>
        private static string ReadConfigFile(string configPath)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configPath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Configuration file not found.", Configpath);
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ConfigurationErrorsException("Configuration file is empty.", Configpath, 0);
            }

            return json;
        }
    }
}
