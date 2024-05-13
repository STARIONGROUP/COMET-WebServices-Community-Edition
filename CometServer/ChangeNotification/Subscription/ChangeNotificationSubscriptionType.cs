﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationSubscriptionType.cs" company="Starion Group S.A.">
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
    /// <summary>
    /// Assertions that determine the kind of <see cref="ChangeNotificationSubscription"/>.
    /// </summary>
    public enum ChangeNotificationSubscriptionType
    {
        /// <summary>
        /// For <see cref="CDP4Common.DTO.INamedThing"/>s
        /// </summary>
        NamedThing,

        /// <summary>
        /// For applied <see cref="CDP4Common.DTO.Category"/>s
        /// </summary>
        AppliedCategory,

        /// <summary>
        /// For <see cref="CDP4Common.DTO.ParameterType"/>s
        /// </summary>
        ParameterType,

        /// <summary>
        /// For <see cref="CDP4Common.DTO.ParameterSubscription"/>s
        /// </summary>
        ParameterSubscription,

        /// <summary>
        /// For <see cref="CDP4Common.DTO.Parameter"/>s and <see cref="CDP4Common.DTO.ParameterOverride"/>s
        /// </summary>
        ParameterOrOverride
    }
}
