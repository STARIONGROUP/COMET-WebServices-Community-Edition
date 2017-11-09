// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieSessionManager.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    /// <summary>
    /// Manages session cookies.
    /// </summary>
    public class CookieSessionManager : ICookieSessionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieSessionManager"/> class.
        /// </summary>
        public CookieSessionManager()
        {
            this.CreateCookieCache();
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public CookieSessionCache Cache { get; private set; }

        /// <summary>
        /// Reset the current Cache, by forcing dispose on the current instance.
        /// </summary>
        public void ResetCache()
        {
            this.Cache.Dispose();

            this.CreateCookieCache();
        }

        /// <summary>
        /// The create cookie cache.
        /// </summary>
        private void CreateCookieCache()
        {
            this.Cache = new CookieSessionCache("CDP4");
        }
    }
}
