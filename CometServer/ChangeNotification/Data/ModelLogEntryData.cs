// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelLogEntryData.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CometServer.ChangeNotification.Subscription;

    /// <summary>
    /// Parent class of an object structure used to create a selection of data from <see cref="CDP4Common.DTO.ModelLogEntry"/>s,
    /// <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>s and <see cref="ChangeNotificationSubscription"/>s.
    /// </summary>
    public class ModelLogEntryData
    {
        /// <summary>
        /// Gets the <see cref="CDP4Common.DTO.ModelLogEntry"/>
        /// </summary>
        public ModelLogEntry ModelLogEntry { get; }

        /// <summary>
        /// Get sthe <see cref="CDP4Common.DTO.ModelLogEntry"/>'s <see cref="CDP4Common.DTO.ModelLogEntry.Content"/>
        /// </summary>
        public string JustificationText => this.ModelLogEntry.Content;

        /// <summary>
        /// Get sthe <see cref="CDP4Common.DTO.ModelLogEntry"/>'s <see cref="CDP4Common.DTO.ModelLogEntry.ModifiedOn"/>
        /// </summary>
        public DateTime ModifiedOn => this.ModelLogEntry.ModifiedOn;

        /// <summary>
        /// Gets the related <see cref="ICollection{T}"/> of type <see cref="Data.LogEntryChangelogItemData"/>
        /// </summary>
        public ICollection<LogEntryChangelogItemData> LogEntryChangelogItemData { get; } = new List<LogEntryChangelogItemData>();

        /// <summary>
        /// Creates a new instance of <see cref="ModelLogEntryData"/>
        /// </summary>
        /// <param name="modelLogEntry">
        /// The <see cref="CDP4Common.DTO.ModelLogEntry"/>
        /// </param>
        public ModelLogEntryData(ModelLogEntry modelLogEntry)
        {
            this.ModelLogEntry = modelLogEntry;
        }

        /// <summary>
        /// Tries to add objects base on a combination of <see cref="LogEntryChangelogItem"/> and <see cref="ChangeNotificationSubscription"/> instances to the object structure
        /// where this <see cref="ModelLogEntryData"/> is the root object of.
        /// If a <see cref="Data.LogEntryChangelogItemData"/> for the <see cref="logEntryChangelogItem"/> was already create in the
        /// <see cref="LogEntryChangelogItemData"/> property, then no new one will be created.
        /// </summary>
        /// <param name="logEntryChangelogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <param name="changeNotificationSubscription">
        /// The <see cref="ChangeNotificationSubscription"/>
        /// </param>
        public void TryAddLogEntryChangelogData(LogEntryChangelogItem logEntryChangelogItem, ChangeNotificationSubscription changeNotificationSubscription)
        {
            if (this.LogEntryChangelogItemData.All(x => x.LogEntryChangelogItem != logEntryChangelogItem))
            {
                this.LogEntryChangelogItemData.Add(new LogEntryChangelogItemData(logEntryChangelogItem, this));
            }

            var logEntryChangelogItemData = this.LogEntryChangelogItemData.Single(x => x.LogEntryChangelogItem == logEntryChangelogItem);
            logEntryChangelogItemData.TryAddChangeNotificationSubscriptionData(changeNotificationSubscription);
        }
    }
}
