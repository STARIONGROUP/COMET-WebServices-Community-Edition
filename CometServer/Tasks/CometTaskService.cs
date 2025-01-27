// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometTaskService.cs" company="Starion Group S.A.">
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

namespace CometServer.Tasks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    
    using CDP4Common.DTO;

    using CDP4DalCommon.Protocol.Tasks;

    using CometServer.Configuration;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="CometTaskService"/> is to provide access to running <see cref="CometTask"/>s which are created for each
    /// POST request. 
    /// </summary>
    public class CometTaskService : ICometTaskService
    {
        /// <summary>
        /// The key prefix used in the <see cref="IMemoryCache"/>
        /// </summary>
        private const string KeyPrefix = "TASK-";

        /// <summary>
        /// The (injected) <see cref="IMemoryCache"/>
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/>
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The (injected) <see cref="ILogger"/>
        /// </summary>
        private readonly ILogger<CometTaskService> logger;

        /// <summary>
        /// A ConcurrentDictionary that keeps track of all the long running tasks for easy iteration
        /// </summary>
        private readonly ConcurrentDictionary<string, CometTask> tasks = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CometTaskService"/> class
        /// </summary>
        /// <param name="memoryCache">
        /// The (injected) <see cref="IMemoryCache"/>
        /// </param>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="logger">
        /// The (injected) <see cref="ILogger"/>
        /// </param>
        public CometTaskService(IMemoryCache memoryCache, IAppConfigService appConfigService, ILogger<CometTaskService> logger)
        {
            this.memoryCache = memoryCache;
            this.appConfigService = appConfigService;
            this.logger = logger;
        }

        /// <summary>
        /// Queries the <see cref="CometTask"/>s for the provided person
        /// </summary>
        /// <param name="person">
        /// The unique identifier of the <see cref="Person"/>
        /// </param>
        /// <returns>
        /// a readonly list of <see cref="CometTask"/>
        /// </returns>
        public IReadOnlyList<CometTask> QueryTasks(Guid person)
        {
            var result = new List<CometTask>();

            foreach (var kvp in this.tasks)
            {
                if (kvp.Value.Actor == person)
                {
                    result.Add(kvp.Value);
                }
            }

            return result.AsReadOnly();
        }

        /// <summary>
        /// Queries a specific <see cref="CometTask"/>
        /// </summary>
        /// <param name="identifier">
        /// The unique identifier of the <see cref="CometTask"/>
        /// </param>
        /// <returns>
        /// A <see cref="CometTask"/>, or null of not found
        /// </returns>
        public CometTask? QueryTask(Guid identifier)
        {
            var key = $"{KeyPrefix}{identifier}";

            return this.memoryCache.TryGetValue(key, out CometTask task) ? task : null;
        }

        /// <summary>
        /// Adds or updates the <see cref="CometTask"/> in the Cache
        /// </summary>
        /// <param name="cometTask">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        public void AddOrUpdateTask(CometTask cometTask)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(this.appConfigService.AppConfig.LongRunningTasksConfig.RetentionTime))
                .RegisterPostEvictionCallback(this.EvictionCallback, state: this);

            var key = $"{KeyPrefix}{cometTask.Id}";

            this.memoryCache.Set(key, cometTask, cacheEntryOptions);

            this.tasks.AddOrUpdate(key, cometTask, (_, _) => cometTask);

            this.logger.LogInformation("The long running task {id} for actor {actor} has been added or updated", key, cometTask.Actor);
        }

        /// <summary>
        /// Updates the <see cref="CometTask"/> with the provided data and add or updates it to the Cache
        /// </summary>
        /// <param name="task">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        /// <param name="finishedAt">
        /// the <see cref="DateTime"/> at which the <see cref="CometTask"/> was finished
        /// </param>
        /// <param name="statusKind">
        /// the status of the <see cref="CometTask"/>
        /// </param>
        /// <param name="error">
        /// the error in case the operation failed
        /// </param>
        public void AddOrUpdateTask(CometTask task, DateTime finishedAt, StatusKind statusKind, string error)
        {
            task.FinishedAt = finishedAt;
            task.StatusKind = statusKind;
            task.Error = error;

            this.AddOrUpdateTask(task);
        }

        /// <summary>
        /// Updates the <see cref="CometTask"/> with the provided data, sets it as successful and add or updates it to the Cache
        /// </summary>
        /// <param name="task">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        /// <param name="revision">
        /// the revision number corresponding to the TopContainer revision that was updated
        /// </param>
        public void AddOrUpdateSuccessfulTask(CometTask task, int revision)
        {
            task.FinishedAt = DateTime.Now;
            task.StatusKind = StatusKind.SUCCEEDED;
            task.Revision = revision;
        }

        /// <summary>
        /// Callback method triggered when an item is evicted from the <see cref="IMemoryCache"/>
        /// </summary>
        /// <param name="key">
        /// the key of the cached item that was evicted
        /// </param>
        /// <param name="value">
        /// the cached item
        /// </param>
        /// <param name="reason">
        /// the reason for the item being evicted
        /// </param>
        /// <param name="state">
        /// the evications state
        /// </param>
        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (state != this)
            {
                return;
            }

            if (key is string s)
            {
                var cometTask = (CometTask)value;

                if (this.tasks.TryRemove(s, out _))
                {
                    this.logger.LogInformation("The long running task {id} for actor {actor} has been evicted because of {reason}", key, cometTask.Actor, reason);
                }
                else
                {
                    this.logger.LogDebug("The long running task {id} for actor {actor} could not be evicted from the tasks since it does not exist", key, cometTask.Actor);
                }

                if (reason == EvictionReason.Replaced && this.memoryCache.TryGetValue(s, out CometTask updatedCometTask))
                {
                    this.tasks.AddOrUpdate(s, updatedCometTask, (_, _) => updatedCometTask);
                }
            }
        }
    }
}
