// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryChangelogItemData.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CometServer.ChangeNotification.Subscription;

    public class LogEntryChangelogItemData
    {
        /// <summary>
        /// Gets the <see cref="Data.ModelLogEntryData"/>
        /// </summary>
        public ModelLogEntryData ModelLogEntryData { get; }

        /// <summary>
        /// Gets the <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>
        /// </summary>
        public LogEntryChangelogItem LogEntryChangelogItem { get; }

        /// <summary>
        /// Gets the <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>s <see cref="CDP4Common.DTO.LogEntryChangelogItem.ChangeDescription"/> property
        /// </summary>
        public string ChangeDescription => this.LogEntryChangelogItem.ChangeDescription;

        /// <summary>
        /// Gets the <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>s <see cref="CDP4Common.DTO.LogEntryChangelogItem.ChangelogKind"/> property
        /// </summary>
        public string ChangelogKind => this.LogEntryChangelogItem.ChangelogKind.ToString();

        /// <summary>
        /// Gets the related <see cref="ICollection{T}"/> of type <see cref="Data.ChangeNotificationSubscriptionData"/>
        /// </summary>
        public ICollection<ChangeNotificationSubscriptionData> ChangeNotificationSubscriptionData { get; } = new List<ChangeNotificationSubscriptionData>();

        /// <summary>
        /// Creates a new instance of <see cref="LogEntryChangelogItemData"/>
        /// </summary>
        /// <param name="logEntryChangelogItem">
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>
        /// </param>
        /// <param name="modelLogEntryData">
        /// The <see cref="Data.ModelLogEntryData"/>
        /// </param>
        public LogEntryChangelogItemData(LogEntryChangelogItem logEntryChangelogItem, ModelLogEntryData modelLogEntryData)
        {
            this.ModelLogEntryData = modelLogEntryData;
            this.LogEntryChangelogItem = logEntryChangelogItem;
        }

        /// <summary>
        /// Tries to add a <see cref="Data.ChangeNotificationSubscriptionData"/> based on a <see cref="ChangeNotificationSubscription"/> instance
        /// to the <see cref="ChangeNotificationSubscriptionData"/> property.
        /// If a <see cref="Data.ChangeNotificationSubscriptionData"/> for the <see cref="changeNotificationSubscription"/> was already create in the
        /// <see cref="ChangeNotificationSubscriptionData"/> property, then no new one will be created.
        /// </summary>
        /// <param name="changeNotificationSubscription">
        /// The <see cref="ChangeNotificationSubscription"/>
        /// </param>
        public void TryAddChangeNotificationSubscriptionData(ChangeNotificationSubscription changeNotificationSubscription)
        {
            if (this.ChangeNotificationSubscriptionData.All(x => x.ChangeNotificationSubscription != changeNotificationSubscription))
            {
                this.ChangeNotificationSubscriptionData.Add(new ChangeNotificationSubscriptionData(changeNotificationSubscription, this));
            }
        }
    }
}
