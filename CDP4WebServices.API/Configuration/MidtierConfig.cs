// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MidtierConfig.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Configuration
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
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the host name of the mid tier.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the mid tier.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the upload directory for the mid tier.
        /// </summary>
        public string UploadDirectory { get; set; }

        /// <summary>
        /// Gets or sets the upload directory for the mid tier.
        /// </summary>
        public string FileStorageDirectory { get; set; }
    }
}
