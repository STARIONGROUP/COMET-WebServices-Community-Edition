// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMessagingConfig.cs" company="RHEA System S.A.">
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
    /// The Inter service messaging configuration.
    /// </summary>
    public class ServiceMessagingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessagingConfig"/> class.
        /// </summary>
        public ServiceMessagingConfig()
        {
            // set defaults
            this.IsEnabled = true;
            this.HostName = "127.0.0.1";
            this.Port = 5672;
            this.AdminPort = 15672;
            this.UserName = "guest";
            this.Password = "guest";
            this.MaxConnectionRetryAttempts = 4;
            this.TimeSpanBetweenAttempts = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessagingConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public ServiceMessagingConfig(IConfiguration configuration)
        {
            this.IsEnabled = !bool.TryParse(configuration["MessageBroker:IsEnabled"], out var isEnabled) || isEnabled;
            this.Port = int.TryParse(configuration["MessageBroker:Port"], out var port) ? port : 5672;
            this.AdminPort = int.TryParse(configuration["MessageBroker:AdminPort"], out var adminPort) ? adminPort : 15672;

            this.MaxConnectionRetryAttempts = int.TryParse(configuration["MessageBroker:MaxConnectionRetryAttempts"], out var maxConnectionRetryAttempts) 
                ? maxConnectionRetryAttempts 
                : 4;

            this.TimeSpanBetweenAttempts = int.TryParse(configuration["MessageBroker:TimeSpanBetweenAttempts"], out var timeSpanBetweenAttempts) 
                ? timeSpanBetweenAttempts
                : 1;

            this.UserName = configuration["MessageBroker:UserName"];
            this.Password = configuration["MessageBroker:Password"];
            this.HostName = configuration["MessageBroker:HostName"];
        }

        /// <summary>
        /// Gets or sets the host name of the back tier.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the service messaging.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the admin port of the back tier.
        /// </summary>
        public int AdminPort { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of attempts to connect to the message broker
        /// </summary>
        public int MaxConnectionRetryAttempts { get; set; }
        
        /// <summary>
        /// Gets or sets the timespan between attemps to connect to the message broker in second
        /// </summary>
        public int TimeSpanBetweenAttempts { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password of the back tier.
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the service messaging should be enabled
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}