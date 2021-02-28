﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOrOverrideChangeNotificationFilter.cs" company="RHEA System S.A.">
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

namespace CometServer.ChangeNotification.Notification
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.ChangeNotification.Subscription;

    /// <summary>
    /// Implements logic for Change notification filtering for an <see cref="ParameterOrOverrideBase"/>.
    /// </summary>
    public class ParameterOrOverrideChangeNotificationFilter : DefaultThingChangeNotificationFilter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ParameterTypeChangeNotificationFilter"/> class.
        /// </summary>
        /// <param name="changeNotificationSubscription">
        /// The <see cref="IChangeNotificationFilter"/> that resulted into this <see cref="IChangeNotificationFilter"/>.
        /// </param>
        /// <param name="domainOfExpertises">
        /// The <see cref="ChangeNotificationSubscription"/>s where to filter on.
        /// </param>
        public ParameterOrOverrideChangeNotificationFilter(ChangeNotificationSubscription changeNotificationSubscription, IEnumerable<DomainOfExpertise> domainOfExpertises)
            : base(changeNotificationSubscription, domainOfExpertises)
        {
        }
    }
}
