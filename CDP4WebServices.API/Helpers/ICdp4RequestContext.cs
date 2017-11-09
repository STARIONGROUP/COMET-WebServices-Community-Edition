// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICdp4RequestContext.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using CDP4WebServices.API.Services.Authentication;

    using Nancy;

    /// <summary>
    /// The RequestContext interface.
    /// </summary>
    public interface ICdp4RequestContext
    {
        /// <summary>
        /// Gets the request context.
        /// </summary>
        NancyContext Context { get; }

        /// <summary>
        /// Gets the authenticated person.
        /// </summary>
        Credentials AuthenticatedCredentials { get; }
    }
}
