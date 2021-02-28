// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MidtierConfig.cs" company="RHEA System S.A.">
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
            this.Protocol = "http";
            this.HostName = "127.0.0.1";
            this.Port = 5000;
            this.UploadDirectory = "upload";
            this.FileStorageDirectory = "storage";
        }

        /// <summary>
        /// Gets or sets the protocol to use (http, https).
        /// </summary>
        /// <remarks>
        /// The default value is http
        /// </remarks>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the host name of the mid tier.
        /// </summary>
        /// <remarks>
        /// The default value is 127.0.0.1
        /// </remarks>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the mid tier.
        /// </summary>
        /// <remarks>
        /// The default value is 5000
        /// </remarks>
        public int Port { get; set; }

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
    }
}