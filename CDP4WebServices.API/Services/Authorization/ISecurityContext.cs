// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityContext.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;

    using CDP4WebServices.API.Services.Authentication;

    /// <summary>
    /// The Request Security Context interface.
    /// </summary>
    public interface ISecurityContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for reading.
        /// </summary>
        bool ContainerReadAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for writing.
        /// </summary>
        bool ContainerWriteAllowed { get; set; }

        /// <summary>
        /// Gets or sets the top container for this request context.
        /// </summary>
        [Obsolete]
        string TopContainer { get; set; }

        /// <summary>
        /// Gets or sets the Credentials
        /// </summary>
        Credentials Credentials { get; set; }
    }
}
