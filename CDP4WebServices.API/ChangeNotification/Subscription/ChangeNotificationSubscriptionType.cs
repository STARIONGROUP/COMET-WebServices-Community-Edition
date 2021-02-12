//  --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationSubscriptionType.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.ChangeNotification.Subscription
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
