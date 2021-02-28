// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao
{
    using System;

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
        void InsertDataFromAudit(NpgsqlTransaction transaction, string partition, DateTime instant);

        /// <summary>
        /// Set the end-validity of all iteration data
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        void SetIterationValidityEnd(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        void CopyIteration(NpgsqlTransaction transaction, string sourcePartition, string targetPartition);

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
        void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable);

        /// <summary>
        /// Deletes all things of the "current" tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        void DeleteAllIterationThings(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Deletes all OrganizationalParticipant links
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        void DeleteAllrganizationalParticipantThings(NpgsqlTransaction transaction, string partition);

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
        void MoveToNextIterationFromLast(NpgsqlTransaction transaction, string partition, Thing thing);
    }
}