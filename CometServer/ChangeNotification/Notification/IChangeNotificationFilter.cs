// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeNotificationFilter.cs" company="RHEA System S.A.">
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
    using System;

    using CDP4Common.DTO;

    /// <summary>
    /// The interface that defines the implementation of a changelog notification filter
    /// </summary>
    public interface IChangeNotificationFilter
    {
        /// <summary>
        /// The <see cref="Guid"/> that represents a <see cref="Thing"/>'s <see cref="Thing.Iid"/>.
        /// </summary>
        Guid Iid { get; }

        /// <summary>
        /// Checks if a <see cref="ModelLogEntry"/> has certain specifics related to the <see cref="Iid"/>.
        /// </summary>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <returns>
        /// True is the specifics of the <see cref="ModelLogEntry"/> match certain criteria, otherwise false.
        /// </returns>
        bool CheckFilter(ModelLogEntry modelLogEntry);

        /// <summary>
        /// Checks if a <see cref="LogEntryChangelogItem"/> has certain specifics related to the <see cref="Iid"/>.
        /// </summary>
        /// <param name="logEntryChangelogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <returns>
        /// True is the specifics of the <see cref="LogEntryChangelogItem"/> match certain criteria, otherwise false.
        /// </returns>
        bool CheckFilter(LogEntryChangelogItem logEntryChangelogItem);
    }
}
