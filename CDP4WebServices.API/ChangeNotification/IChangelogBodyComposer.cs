// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangelogBodyComposer.cs" company="RHEA System S.A.">
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

namespace CometServer.ChangeNotification
{
    using System;
    using System.Collections.Generic;

    using Autofac;

    using CDP4Common.DTO;

    using CometServer.ChangeNotification.UserPreference;

    using Npgsql;

    /// <summary>
    /// Defines the implementation of a change log body composer that composes the body of an email that contais change log information
    /// </summary>
    public interface IChangelogBodyComposer
    {
        /// <summary>
        /// Creates the body of the email in text form
        /// </summary>
        /// <param name="changeLogSections">
        /// The <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/> that contains all data to show in the HTML body
        /// </param>
        /// <returns>
        /// The email body text as plain text. 
        /// </returns>
        string CreateTextBody(IEnumerable<ChangelogSection> changeLogSections);

        /// <summary>
        /// Creates the body of the email in html form
        /// </summary>
        /// <param name="changeLogSections">
        /// The <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/> that contains all data to show in the HTML body
        /// </param>
        /// <returns>
        /// The email body text as HTML. 
        /// </returns>
        string CreateHtmlBody(IEnumerable<ChangelogSection> changeLogSections);

        /// <summary>
        /// Create an <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/>s to be used to compose the email body
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="container">
        /// The <see cref="IContainer"/> used to resolve injectable objects
        /// </param>
        /// <param name="engineeringModelIid">
        /// The <see cref="EngineeringModel.Iid"/> property of the related <see cref="EngineeringModel"/>
        /// </param>
        /// <param name="person">
        /// The <see cref="Person"/> for whom to compose the email
        /// </param>
        /// <param name="changeNotificationSubscriptionUserPreference">
        /// The <see cref="ChangeNotificationSubscriptionUserPreference"/> that contains the change notification subscriptions
        /// </param>
        /// <param name="startDateTime">
        /// The start <see cref="DateTime"/> of the period we want to collect change log rows for.
        /// </param>
        /// <param name="endDateTime">
        /// The end <see cref="DateTime"/> of the period we want to collect change log rows for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/>s
        /// </returns>
        IEnumerable<ChangelogSection> CreateChangelogSections(
            NpgsqlTransaction transaction , 
            IContainer container, 
            Guid engineeringModelIid, 
            Person person, 
            ChangeNotificationSubscriptionUserPreference changeNotificationSubscriptionUserPreference,
            DateTime startDateTime,
            DateTime endDateTime);
    }
}
