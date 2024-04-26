// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelLogEntryDataCreator.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.ChangeNotification.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.ChangeNotification.Notification;
    using CometServer.ChangeNotification.Subscription;
    using CometServer.ChangeNotification.UserPreference;

    using Npgsql;

    /// <summary>
    /// Contains logic used to create an <see cref="IEnumerable{T}"/> of type <see cref="ModelLogEntryData"/> for a specific <see cref="EngineeringModel"/>
    /// base on a list of filtered <see cref="ModelLogEntry"/>s.
    /// </summary>
    public class ModelLogEntryDataCreator : IModelLogEntryDataCreator
    {
        /// <summary>
        /// The (injected) <see cref="IDomainOfExpertiseDao"/>
        /// </summary>
        public IDomainOfExpertiseDao DomainOfExpertiseDao { get; set; }

        /// <summary>
        /// The (injected) <see cref="ILogEntryChangelogItemDao"/>
        /// </summary>
        public ILogEntryChangelogItemDao LogEntryChangelogItemDao { get; set; }

        /// <summary>
        /// Create an <see cref="IEnumerable{T}"/> of type <see cref="ModelLogEntryData"/> for a specific <see cref="EngineeringModel"/>
        /// base on a list of filtered <see cref="ModelLogEntry"/>s
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="engineeringModelPartition">
        /// The partition in the database
        /// </param>
        /// <param name="modelLogEntries">
        /// The <see cref="ModelLogEntry"/>s
        /// </param>
        /// <param name="domains">
        /// The <see cref="DomainOfExpertise.Iid"/>s used to filter <see cref="LogEntryChangelogItem"/>s to be used.
        /// </param>
        /// <param name="changeNotificationSubscriptionUserPreference">
        /// The <see cref="ChangeNotificationSubscriptionUserPreference"/> that contains the change notification subscriptions
        /// </param>
        /// <returns>
        /// The created <see cref="IEnumerable{T}"/> of type <see cref="ModelLogEntryData"/>
        /// </returns>
        public IEnumerable<ModelLogEntryData> Create(
            NpgsqlTransaction transaction, 
            string engineeringModelPartition, 
            IEnumerable<ModelLogEntry> modelLogEntries, 
            IEnumerable<Guid> domains, 
            ChangeNotificationSubscriptionUserPreference changeNotificationSubscriptionUserPreference)
        {
            var modelLogEntryDataList = new List<ModelLogEntryData>();

            var domainOfExpertises = this.DomainOfExpertiseDao.Read(transaction, "SiteDirectory", domains).ToList();

            foreach (var changeNotificationSubscription in changeNotificationSubscriptionUserPreference.ChangeNotificationSubscriptions)
            {
                var changeNotificationFilter = CreateChangeLogNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                foreach (var modelLogEntry in modelLogEntries.OrderBy(x => x.CreatedOn))
                {
                    if (changeNotificationFilter.CheckFilter(modelLogEntry))
                    {
                        var modelLogEntryData = new ModelLogEntryData(modelLogEntry);
                        var addModelLogEntryData = false;

                        var logEntryChangeLogItems = 
                            this.LogEntryChangelogItemDao.Read(transaction, engineeringModelPartition, modelLogEntry.LogEntryChangelogItem).ToList();

                        foreach (var logEntryChangelogItem in logEntryChangeLogItems)
                        {
                            if (changeNotificationFilter.CheckFilter(logEntryChangelogItem))
                            {
                                addModelLogEntryData = true;
                                modelLogEntryData.TryAddLogEntryChangelogData(logEntryChangelogItem, changeNotificationSubscription);
                            }
                        }

                        if (addModelLogEntryData)
                        {
                            modelLogEntryDataList.Add(modelLogEntryData);
                        }
                    }
                }
            }

            return modelLogEntryDataList;
        }

        /// <summary>
        /// Create a <see cref="IChangeNotificationFilter"/> for a <see cref="ChangeNotificationSubscription"/>.
        /// </summary>
        /// <param name="changeNotificationSubscription">
        /// The <see cref="ChangeNotificationSubscription"/>
        /// </param>
        /// <param name="domainOfExpertises">
        /// The current user's <see cref="DomainOfExpertise"/>s
        /// </param>
        /// <returns>
        /// The <see cref="IChangeNotificationFilter"/>
        /// </returns>
        private static IChangeNotificationFilter CreateChangeLogNotificationFilter(ChangeNotificationSubscription changeNotificationSubscription, IEnumerable<DomainOfExpertise> domainOfExpertises)
        {
            switch (changeNotificationSubscription.ChangeNotificationSubscriptionType)
            {
                case ChangeNotificationSubscriptionType.AppliedCategory:
                    return new CategoryChangeNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                case ChangeNotificationSubscriptionType.ParameterSubscription:
                    return new ParameterSubscriptionChangeNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                case ChangeNotificationSubscriptionType.ParameterType:
                    return new ParameterTypeChangeNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                case ChangeNotificationSubscriptionType.NamedThing:
                    return new DefaultThingChangeNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                case ChangeNotificationSubscriptionType.ParameterOrOverride:
                    return new ParameterOrOverrideChangeNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                default:
                    throw new NotImplementedException($"{changeNotificationSubscription.ChangeNotificationSubscriptionType} is not implemented.");
            }
        }
    }
}
