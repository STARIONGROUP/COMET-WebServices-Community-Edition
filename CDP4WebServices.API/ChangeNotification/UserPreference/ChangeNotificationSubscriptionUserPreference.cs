// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationSubscriptionUserPreference.cs" company="RHEA System S.A.">
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

namespace CometServer.ChangeNotification.UserPreference
{
    using System.Collections.Generic;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CometServer.ChangeNotification.Subscription;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Class that holds information about the user's (=<see cref="Person"/>'s) ChangeLog <see cref="UserPreference"/>s.
    /// </summary>
    public class ChangeNotificationSubscriptionUserPreference : SavedUserPreference
    {
        /// <summary>
        /// Gets a <see cref="ICollection{T}"/> that holds all individual <see cref="ChangeNotificationSubscription"/>s
        /// </summary>
        public ICollection<ChangeNotificationSubscription> ChangeNotificationSubscriptions { get; }

        /// <summary>
        /// Gets the <see cref="ChangeNotificationReportType"/> for email/reporting purposes.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))] 
        public ChangeNotificationReportType ChangeNotificationReportType { get; set; }

        /// <summary>
        /// Retrieves the key for which the <see cref="UserPreference"/> settings will be saved (<see cref="UserPreference.ShortName"/>).
        /// </summary>
        /// <param name="engineeringModel">
        /// The <see cref="EngineeringModel"/> for which to calculate the key.
        /// </param>
        /// <returns>
        /// The key as a <see cref="string"/>.
        /// </returns>
        public static string GetUserPreferenceKey(EngineeringModel engineeringModel)
        {
            return $"ChangeLogSubscriptions_{engineeringModel.Iid}";
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ChangeNotificationSubscriptionUserPreference"/> class, specifically used during Json Deserialization
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </param>
        /// <param name="value">
        /// The value of the <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </param>
        /// <param name="description">
        /// The Description of the <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </param>
        /// <param name="changeNotificationSubscriptions">
        /// The <see cref="ICollection{T}"/> of type <see cref="ChangeNotificationSubscription"/>
        /// </param>
        /// <param name="changeNotificationReportType">
        /// The <see cref="ChangeNotificationReportType"/>
        /// </param>
        [JsonConstructor]
        public ChangeNotificationSubscriptionUserPreference(
            string name, 
            string value, 
            string description, 
            ICollection<ChangeNotificationSubscription> changeNotificationSubscriptions, 
            ChangeNotificationReportType changeNotificationReportType)
            : base(name, value, description)
        {
            this.ChangeNotificationSubscriptions = changeNotificationSubscriptions;
            this.ChangeNotificationReportType = changeNotificationReportType;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ChangeNotificationSubscriptionUserPreference"/> class.
        /// </summary>
        /// <param name="engineeringModel">
        /// The <see cref="EngineeringModel"/> for whic to create a <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </param>
        public ChangeNotificationSubscriptionUserPreference(EngineeringModel engineeringModel)
            : base(GetUserPreferenceKey(engineeringModel), "", "")
        {
            this.ChangeNotificationSubscriptions = new List<ChangeNotificationSubscription>();
            this.ChangeNotificationReportType = ChangeNotificationReportType.None;
        }
    }
}
