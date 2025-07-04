// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationDao.cs" company="Starion Group S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The Iteration Dao Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationDao
    {
        /// <summary>
        /// Insert data in the "current" tables using the audit table data at a specific <paramref name="instant"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="instant">The instant that matches an iteration</param>
        Task InsertDataFromAuditAsync(NpgsqlTransaction transaction, string partition, DateTime instant);

        /// <summary>
        /// Set the end-validity of all iteration data
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        Task SetIterationValidityEndAsync(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        Task ModifyUserTriggerAsync(NpgsqlTransaction transaction, string sourcePartition, bool enable);

        /// <summary>
        /// Deletes all things of the "current" tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        Task DeleteAllIterationThingsAsync(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Deletes all OrganizationalParticipant links
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        Task DeleteAllrganizationalParticipantThingsAsync(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Move the database to the next iteration.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        Task MoveToNextIterationFromLastAsync(NpgsqlTransaction transaction, string partition, Thing thing);
    }
}