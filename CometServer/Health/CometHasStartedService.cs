// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometHasStartedService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Health
{
    using System;

    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// The purpose of the <see cref="CometHasStartedService"/> is to provide a shared resource
    /// that can be used to check whether the CDP4-COMET has succcesfuly started and is ready
    /// to accept traffic
    /// </summary>
    public class CometHasStartedService : ICometHasStartedService
    {
        /// <summary>
        /// The key used in the <see cref="IMemoryCache"/>
        /// </summary>
        private const string IsReadyKey = "COMET_HAS_STARTED_AND_IS_READY_TO_ACCEPT_CONNECTIONS";

        /// <summary>
        /// The (inkected) <see cref="IMemoryCache"/>
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// The default <see cref="ServerStatus"/> that is returned when the <see cref="IsReadyKey"/> jey
        /// is not found in the <see cref="IMemoryCache"/>
        /// </summary>
        private readonly ServerStatus defaServerStatus = new(false, DateTime.MinValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="CometHasStartedService"/> class
        /// </summary>
        /// <param name="memoryCache">
        /// The (inkected) <see cref="IMemoryCache"/>
        /// </param>
        public CometHasStartedService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;

            this.SetDedaultValue();
        }

        /// <summary>
        /// Gets the <see cref="ServerStatus"/> that is used to indicuate whether the 
        /// server is ready to start accepting requests
        /// </summary>
        /// <returns>
        /// A <see cref="ServerStatus"/>
        /// </returns>
        public ServerStatus GetHasStartedAndIsReady()
        {
            if (this.memoryCache.TryGetValue(IsReadyKey, out ServerStatus data))
            {
                return data;
            }

            return this.defaServerStatus;
        }

        /// <summary>
        /// Sets the <see cref="ICometHasStartedService"/> to indicate whether the
        /// CDP4-COMET server has started and is ready to accept requests
        /// </summary>
        public void SetHasStartedAndIsReady(bool isHealthy)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };

            this.memoryCache.Set(IsReadyKey, new ServerStatus(isHealthy, DateTime.UtcNow), cacheEntryOptions);
        }

        /// <summary>
        /// Sets the default <see cref="ServerStatus"/> in the <see cref="IMemoryCache"/>
        /// </summary>
        private void SetDedaultValue()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };

            this.memoryCache.Set(IsReadyKey, this.defaServerStatus, cacheEntryOptions);
        }
    }
}
