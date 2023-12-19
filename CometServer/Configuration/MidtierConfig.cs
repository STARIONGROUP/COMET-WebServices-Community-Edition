// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MidtierConfig.cs" company="RHEA System S.A.">
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
    /// The mid tier configuration.
    /// </summary>
    public class MidtierConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidtierConfig"/> class.
        /// </summary>
        public MidtierConfig()
        {
            // set defaults
            this.UploadDirectory = "upload";
            this.FileStorageDirectory = "storage";
            this.TemporaryFileStorageDirectory = "tempstorage";
            this.IsExportEnabled = true;
            this.ExportDirectory = "export";
            this.BacktierWaitTime = 300;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidtierConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public MidtierConfig(IConfiguration configuration)
        {
            this.UploadDirectory = configuration["Midtier:UploadDirectory"];
            this.FileStorageDirectory = configuration["Midtier:FileStorageDirectory"];
            this.TemporaryFileStorageDirectory = configuration["Midtier:TemporaryFileStorageDirectory"];
            this.IsExportEnabled = bool.Parse(configuration["Midtier:IsExportEnabled"]);
            this.ExportDirectory = configuration["Midtier:ExportDirectory"];
            this.BacktierWaitTime = int.Parse(configuration["Midtier:BacktierWaitTime"]);
        }

        /// <summary>
        /// Gets or sets the upload directory for the mid tier.
        /// </summary>
        /// <remarks>
        /// The default value is upload
        /// </remarks>
        public string UploadDirectory { get; set; }

        /// <summary>
        /// Gets or sets the upload directory for the mid tier.
        /// </summary>
        /// <remarks>
        /// The default value is storage
        /// </remarks>
        public string FileStorageDirectory { get; set; }

        /// <summary>
        /// Gets or sets the temporary file directory that the mid tier uses
        /// </summary>
        /// <remarks>
        /// The default value is tempstorage
        /// </remarks>
        public string TemporaryFileStorageDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the export route is enabled or not
        /// </summary>
        /// <remarks>
        /// The default value is true
        /// </remarks>
        public bool IsExportEnabled { get; set; }

        /// <summary>
        /// Gets or sets the path to the directory used to store export files that
        /// are to be downloaded once created and then cleaned up
        /// </summary>
        /// <remarks>
        /// The default value is export
        /// </remarks>
        public string ExportDirectory { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds to wait for the backtier to become available
        /// at appliation startus
        /// </summary>
        /// <remarks>
        /// The default value is 300
        /// </remarks>
        public int BacktierWaitTime { get; set; }
    }
}