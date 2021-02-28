// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelDao.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The engineering model dao supplemental code.
    /// </summary>
    public partial class EngineeringModelDao
    {
        /// <summary>
        /// The get next iteration number.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws if an iterationNumber could not be retrieved
        /// </exception>
        public int GetNextIterationNumber(NpgsqlTransaction transaction, string partition)
        {
            using (var command = new NpgsqlCommand())
            {
                var sql = string.Format("SELECT nextval('\"{0}\".\"IterationNumberSequence\"');", partition);

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sql;

                // log the sql command 
                this.LogCommand(command);
                object response = command.ExecuteScalar();
                int iterationNumber;

                if (response != null && int.TryParse(response.ToString(), out iterationNumber))
                {
                    return iterationNumber;
                }

                throw new InvalidOperationException("The expected iterationNumber could not be retrieved.");
            }
        }

        /// <summary>
        /// The reset iteration number sequence start number.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="iterationStartNumber">
        /// The start number.
        /// </param>
        public void ResetIterationNumberSequenceStartNumber(NpgsqlTransaction transaction, string partition, int iterationStartNumber)
        {
            using (var command = new NpgsqlCommand())
            {
                var sql = string.Format("ALTER SEQUENCE \"{0}\".\"IterationNumberSequence\" RESTART WITH {1};", partition, iterationStartNumber);

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sql;

                // log the sql command 
                this.LogCommand(command);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Modify the identifier of all records in a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The egineering-model partition to modify</param>
        public void ModifyIdentifier(NpgsqlTransaction transaction, string partition)
        {
            using (var command = new NpgsqlCommand())
            {
                var valueTypeDictionaryContents = new Dictionary<string, string>
                                                      {
                                                          { "RevisionNumber", "1" }
                                                      };

                var sql = string.Format("UPDATE \"{0}\".\"Thing\" SET \"Iid\" = uuid_generate_v4() WHERE \"ValueTypeDictionary\" -> 'ClassKind' != 'EngineeringModel' AND \"ValueTypeDictionary\" -> 'ClassKind' != 'Iteration'; UPDATE \"{0}\".\"Thing\" SET \"ValueTypeDictionary\" = \"ValueTypeDictionary\" || :valueTypeDictionary;", partition);

                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sql;

                // log the sql command 
                this.LogCommand(command);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Modify the identifier of the <paramref name="thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="thing">The updated <see cref="Thing"/></param>
        /// <param name="oldIid">The old identifier</param>
        public void ModifyIdentifier(NpgsqlTransaction transaction, string partition, Thing thing, Guid oldIid)
        {
            using (var command = new NpgsqlCommand())
            {
                var sql = string.Format("UPDATE \"{0}\".\"Thing\" SET \"Iid\" = :newIid WHERE \"Iid\" = :oldIid;", partition);

                command.Parameters.Add("newIid", NpgsqlDbType.Uuid).Value = thing.Iid;
                command.Parameters.Add("oldIid", NpgsqlDbType.Uuid).Value = oldIid;

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sql;

                // log the sql command 
                this.LogCommand(command);
                command.ExecuteNonQuery();
            }
        }
    }
}