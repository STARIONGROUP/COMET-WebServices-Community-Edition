// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongRunningTasksConfig.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Configuration
{
    using System;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The long running Tasks configuration.
    /// </summary>
    public class LongRunningTasksConfig
    {
        /// <summary>
        /// The maximum time that long running Tasks are kept in memoery
        /// </summary>
        private const int MaxRetentionTime = 86400;

        /// <summary>
        /// The maximum time to wait for a long running Tasks
        /// </summary>
        private const int MaxWaitTime = 3600;

        /// <summary>
        /// backing field for the <see cref="RetentionTime"/> property
        /// </summary>
        private int retentionTime = 3600;

        /// <summary>
        /// backing field for the <see cref="WaitTime"/> property
        /// </summary>
        private int waitTime = 600;

        /// <summary>
        /// Initializes a new instance of the <see cref="LongRunningTasksConfig"/> class.
        /// </summary>
        public LongRunningTasksConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LongRunningTasksConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public LongRunningTasksConfig(IConfiguration configuration)
        {
            this.RetentionTime = configuration.GetValue("LongRunningTasks:RetentionTime", 3600);
            this.WaitTime = configuration.GetValue("LongRunningTasks:WaitTime", 600);
        }

        /// <summary>
        /// Gets or sets the time in seconds for which the long running <see cref="CometTask"/>s
        /// are kept in the cache. If the value is set to 0 the Tasks are not cached.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when the value is larger than 86400
        /// </exception>
        public int RetentionTime
        {
            get => this.retentionTime;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.RetentionTime), $"Value cannot be less than 0 seconds.");
                }

                if (value > MaxRetentionTime)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.RetentionTime), $"Value cannot exceed {MaxRetentionTime} seconds.");
                }

                this.retentionTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the time in seconds to wait for a long running <see cref="CometTask"/> to complete. If the task takes
        /// longer to complete the corresponding <see cref="CometTask"/> is returned to the caller which the user can poll to
        /// inspect its status.
        /// </summary>
        public int WaitTime
        {
            get => this.waitTime;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.WaitTime), $"Value cannot be less than 0 seconds.");
                }

                if (value > MaxWaitTime)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.WaitTime), $"Value cannot exceed {MaxWaitTime} seconds.");
                }

                this.waitTime = value;
            }
        }
    }
}
