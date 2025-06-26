// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CometServer.Services
{
    using System;
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    
    using Npgsql;

    /// <summary>
    /// The EngineeringModel Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class EngineeringModelService
    {
        /// <summary>
        /// Copy the tables from a source to a target Engineering-Model partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model
        /// </param>
        /// <param name="targetPartition">
        /// The target Engineering-Model
        /// </param>
        public async Task CopyEngineeringModel(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            await this.EngineeringModelDao.CopyEngineeringModelAndResetCreatedOnAsync(transaction, sourcePartition, targetPartition);
        }

        /// <summary>
        /// Modify the identifier of all records in a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The egineering-model partition to modify</param>
        public async Task ModifyIdentifierAsync(NpgsqlTransaction transaction, string partition)
        {
            await this.EngineeringModelDao.ModifyIdentifierAsync(transaction, partition);
        }

        /// <summary>
        /// Copy the tables from a source to an EngineeringModel partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source EngineeringModel partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        public async Task ModifyUserTriggerAsync(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            await this.EngineeringModelDao.ModifyUserTriggerAsync(transaction, sourcePartition, enable);
        }

        /// <summary>
        /// Modify the identifier of the <paramref name="thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="thing">The updated <see cref="Thing"/></param>
        /// <param name="oldIid">The old identifier</param>
        public async Task ModifyIdentifierAsync(NpgsqlTransaction transaction, string partition, Thing thing, Guid oldIid)
        {
            await this.EngineeringModelDao.ModifyIdentifierAsync(transaction, partition, thing, oldIid);
        }

        /// <summary>
        /// Query the next iteration number
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <returns>The next iteration number</returns>
        public async Task<int> QueryNextIterationNumber(NpgsqlTransaction transaction, string partition)
        {
            return await this.EngineeringModelDao.GetNextIterationNumberAsync(transaction, partition);
        }
    }
}
