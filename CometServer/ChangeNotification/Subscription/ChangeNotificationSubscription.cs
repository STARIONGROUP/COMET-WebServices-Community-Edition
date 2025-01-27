﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationSubscription.cs" company="Starion Group S.A.">
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

namespace CometServer.ChangeNotification.Subscription
{
    using System;
    using System.Text.Json.Serialization;

    using CDP4Common.CommonData;

    using CometServer.ChangeNotification.Notification;

    /// <summary>
    /// Holds data about an individual change notification subscription
    /// </summary>
    public class ChangeNotificationSubscription
    {
        /// <summary>
        /// Gets the <see cref="Subscription.ChangeNotificationSubscriptionType"/> of the subscription.
        /// This <see cref="Subscription.ChangeNotificationSubscriptionType"/> will be used to create the correct <see cref="IChangeNotificationFilter"/> accordingly.
        /// </summary>
        public ChangeNotificationSubscriptionType ChangeNotificationSubscriptionType { get; }
        
        /// <summary>
        /// Gets the <see cref="Guid"/> of the <see cref="Thing"/> the subscription references.
        /// </summary>
        public Guid Iid { get; }

        /// <summary>
        /// Gets the name of the <see cref="Thing"/> the subscription references.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="CDP4Common.CommonData.ClassKind"/> of the <see cref="Thing"/> the subscription references.
        /// </summary>
        public ClassKind ClassKind { get; }

        /// <summary>
        /// Create a new instance of the <see cref="ChangeNotificationSubscription"/> class.
        /// </summary>
        /// <param name="iid">
        /// The <see cref="Guid"/>of the <see cref="Thing"/> the subscription references.
        /// </param>
        /// <param name="name">
        /// The name of the <see cref="Thing"/> the subscription references
        /// </param>
        /// <param name="classKind">
        /// The <see cref="CDP4Common.CommonData.ClassKind"/>
        /// </param>
        /// <param name="changeNotificationSubscriptionType">
        /// The <see cref="Subscription.ChangeNotificationSubscriptionType"/>.
        /// </param>
        [JsonConstructor]
        public ChangeNotificationSubscription(Guid iid, string name, ClassKind classKind, ChangeNotificationSubscriptionType changeNotificationSubscriptionType)
        {
            this.Iid = iid;
            this.Name = name;
            this.ClassKind = classKind;
            this.ChangeNotificationSubscriptionType = changeNotificationSubscriptionType;
        }
    }
}
