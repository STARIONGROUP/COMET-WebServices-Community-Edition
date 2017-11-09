// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContributorLocationCache.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.ContributorsLocation
{
    using System.Runtime.Caching;

    /// <summary>
    /// The cache of users/contributors location data.
    /// </summary>
    public static class ContributorLocationCache
    {
        /// <summary>
        /// The cache that contains data for each logged in user/contributor.
        /// </summary>
        private static MemoryCache contributorsCache;

        /// <summary>
        /// Gets the cache of users/contributors data.
        /// </summary>
        /// <returns>
        /// The <see cref="MemoryCache"/>.
        /// </returns>
        public static MemoryCache GetContributorsCache()
        {
            return contributorsCache ?? (contributorsCache = new MemoryCache("contributorsLocationCache"));
        }
    }
}
