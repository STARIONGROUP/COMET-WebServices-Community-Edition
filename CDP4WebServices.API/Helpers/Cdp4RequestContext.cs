// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4RequestContext.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using CDP4WebServices.API.Services.Authentication;

    using Nancy;

    /// <summary>
    /// A wrapper for the Nancy request context instance that can be injected.
    /// </summary>
    public class Cdp4RequestContext : ICdp4RequestContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4RequestContext"/> class.
        /// </summary>
        /// <param name="context">
        /// The request Context.
        /// </param>
        public Cdp4RequestContext(NancyContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public NancyContext Context { get; private set; }

        /// <summary>
        /// Gets the authenticated credentials, or null if request context is not present
        /// </summary>
        public Credentials AuthenticatedCredentials
        {
            get
            {
                return this.Context != null ? this.Context.CurrentUser as Credentials : null;
            }
        }
    }
}
