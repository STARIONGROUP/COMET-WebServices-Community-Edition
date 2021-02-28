// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelLogEntryDataCreator.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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

    using Autofac;

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
        /// Create an <see cref="IEnumerable{T}"/> of type <see cref="ModelLogEntryData"/> for a specific <see cref="EngineeringModel"/>
        /// base on a list of filtered <see cref="ModelLogEntry"/>s
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="engineeringModelPartition">
        /// The partition in the database
        /// </param>
        /// <param name="container">
        /// The <see cref="IContainer"/> used to resolve injectable objects
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
            IContainer container, 
            IEnumerable<ModelLogEntry> modelLogEntries, 
            IEnumerable<Guid> domains, 
            ChangeNotificationSubscriptionUserPreference changeNotificationSubscriptionUserPreference)
        {
            var modelLogEntryDataList = new List<ModelLogEntryData>();

            var domainOfExpertiseDao = container.Resolve<IDomainOfExpertiseDao>();
            var logEntryChangeLogItemDao = container.Resolve<ILogEntryChangelogItemDao>();

            var domainOfExpertises = domainOfExpertiseDao.Read(transaction, "SiteDirectory", domains).ToList();

            foreach (var changeNotificationSubscription in changeNotificationSubscriptionUserPreference.ChangeNotificationSubscriptions)
            {
                var changeNotificationFilter = this.CreateChangeLogNotificationFilter(changeNotificationSubscription, domainOfExpertises);

                foreach (var modelLogEntry in modelLogEntries.OrderBy(x => x.CreatedOn))
                {
                    if (changeNotificationFilter.CheckFilter(modelLogEntry))
                    {
                        var modelLogEntryData = new ModelLogEntryData(modelLogEntry);
                        var addModelLogEntryData = false;

                        var logEntryChangeLogItems = 
                            logEntryChangeLogItemDao.Read(transaction, engineeringModelPartition, modelLogEntry.LogEntryChangelogItem).ToList();

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
        private IChangeNotificationFilter CreateChangeLogNotificationFilter(ChangeNotificationSubscription changeNotificationSubscription, IEnumerable<DomainOfExpertise> domainOfExpertises)
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
