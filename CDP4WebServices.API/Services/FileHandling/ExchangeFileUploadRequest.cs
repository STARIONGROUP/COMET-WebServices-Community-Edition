// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileUploadRequest.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.FileHandling
{
    using Nancy;

    /// <summary>
    /// The exchange file upload request.
    /// </summary>
    public class ExchangeFileUploadRequest
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the content size.
        /// </summary>
        public long ContentSize { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public HttpFile File { get; set; }
    }
}
