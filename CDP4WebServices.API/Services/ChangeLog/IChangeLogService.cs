// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeLogService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.ChangeLog
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services.Operations;

    using Npgsql;

    /// <summary>
    /// Defines the implementation of the <see cref="IChangeLogService"/> interface
    /// </summary>
    public interface IChangeLogService
    {
        /// <summary>
        /// Tries to append changelog data based on the changes made to certain <see cref="Thing"/>s.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="actor">
        /// The <see cref="Person.Iid"/> of the person that made the changes.
        /// </param>
        /// <param name="transactionRevision">
        /// The revisionNumber of the <see cref="transaction"/>
        /// </param>
        /// <param name="operation">
        /// <see cref="CdpPostOperation"/> that resulted to all the changes.
        /// </param>
        /// <param name="things">
        /// The <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> that contains changed <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// True if change log data was added, otherwise false
        /// </returns>
        bool TryAppendModelChangeLogData(NpgsqlTransaction transaction, string partition, Guid actor, int transactionRevision, CdpPostOperation operation, IReadOnlyList<Thing> things);
    }
}
