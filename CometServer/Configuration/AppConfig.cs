// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The application Configuration.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfig"/> class.
        /// </summary>
        public AppConfig()
        {
            // make sure there defaults of the configuration sections are present
            this.Backtier = new BacktierConfig();
            this.Defaults = new DefaultsConfig();
            this.Midtier = new MidtierConfig();
            this.Changelog = new ChangelogConfig();
            this.EmailService = new EmailConfig();
            this.HealthConfig = new HealthConfig();
            this.ServiceMessagingConfig = new ServiceMessagingConfig();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfig"/> class.
        /// </summary>
        public AppConfig(IConfiguration configuration)
        {
            // make sure there defaults of the configuration sections are present
            this.Backtier = new BacktierConfig(configuration);
            this.Defaults = new DefaultsConfig(configuration);
            this.Midtier = new MidtierConfig(configuration);
            this.Changelog = new ChangelogConfig(configuration);
            this.EmailService = new EmailConfig(configuration);
            this.HealthConfig = new HealthConfig(configuration);
            this.ServiceMessagingConfig = new ServiceMessagingConfig(configuration);
        }

        /// <summary>
        /// Gets or sets the mid tier configuration.
        /// </summary>
        public MidtierConfig Midtier { get; set; }
        
        /// <summary>
        /// Gets or sets the back tier configuration.
        /// </summary>
        public BacktierConfig Backtier { get; set; }

        /// <summary>
        /// Gets or sets the health configuration.
        /// </summary>
        public HealthConfig HealthConfig { get; set; }

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
        /// Gets or sets the service messaging configuration.
        /// </summary>
        public ServiceMessagingConfig ServiceMessagingConfig { get; set; }
    }
}
