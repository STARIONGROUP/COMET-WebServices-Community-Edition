// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestSecurityContext.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;

    using CDP4WebServices.API.Services.Authentication;

    /// <summary>
    /// Request security context class that holds security data relevant for authorization checks
    /// </summary>
    public class RequestSecurityContext : ISecurityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSecurityContext"/> class.
        /// </summary>
        public RequestSecurityContext()
        {
            // set container authorization defaults
            this.ContainerReadAllowed = false;
            this.ContainerWriteAllowed = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for reading.
        /// </summary>
        public bool ContainerReadAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for writing.
        /// </summary>
        public bool ContainerWriteAllowed { get; set; }

        /// <summary>
        /// Gets or sets the top container for this request context.
        /// </summary>
        [Obsolete]
        public string TopContainer { get; set; }

        /// <summary>
        /// Gets or sets the Credentials
        /// </summary>
        public Credentials Credentials { get; set; }
    }
}
