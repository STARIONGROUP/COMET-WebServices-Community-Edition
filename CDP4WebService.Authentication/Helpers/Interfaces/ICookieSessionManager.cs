// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICookieSessionManager.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    /// <summary>
    /// Public interface of the cookie session manager. At this point only needed for injection.
    /// </summary>
    public interface ICookieSessionManager
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        CookieSessionCache Cache { get; }

        /// <summary>
        /// Reset the current Cache, by forcing dispose on the current instance.
        /// </summary>
        void ResetCache();
    }
}
