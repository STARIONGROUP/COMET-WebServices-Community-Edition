// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICDP4WebServiceAuthentication.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    using System.Collections.Generic;
    
    public interface ICDP4WebServiceAuthentication
    {
        /// <summary>
        /// Gets or sets the CDP4 authentication cookie name
        /// </summary>
        string CDP4AuthenticationCookieName { get; set; }

        /// <summary>
        /// Enables basic authentication for the application
        /// </summary>
        /// <param name="pipelines">
        /// Pipelines to add handlers to (usually "this")
        /// </param>
        /// <param name="configuration">
        /// Forms authentication configuration
        /// </param>
        /// <param name="basicAuthenticationPaths">
        /// The basic Authentication Paths. Each supplied path shall be preceded by a '/' character.
        /// </param>
        void Enable(IPipelines pipelines, CDP4WebServiceAuthenticationConfiguration configuration, IEnumerable<string> basicAuthenticationPaths);

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from the authentication header.
        /// </summary>
        /// <param name="context">The current <see cref="NancyContext"/></param>
        /// <returns>null response</returns>
        Response GetCredentialRetrievalHook(NancyContext context);

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context"> Current context </param>
        /// <returns>Nancy response</returns>
        Response LogOutResponse(NancyContext context);

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context"> Current context </param>
        /// <param name="redirectUrl"> URL to redirect to </param>
        /// <returns>Nancy response</returns>
        Response LogOutAndRedirectResponse(NancyContext context, string redirectUrl);

        /// <summary>
        /// Invalidate all entries in the credential cache.
        /// </summary>
        /// <returns>
        /// True if successfully cleared.
        /// </returns>
        void ResetCredentialCache();

        /// <summary>
        /// Evict a cached credential by supplying the userName.
        /// </summary>
        /// <param name="userName">
        /// The user name of the credentials that are to be evicted.
        /// </param>
        /// <returns>
        /// True if the user credentials where evicted.
        /// </returns>
        bool EvictCredentialFromCache(string userName);
    }
}