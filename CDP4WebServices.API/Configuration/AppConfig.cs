// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Configuration
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
