// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao.Resolve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The container retrieval DAO.
    /// </summary>
    public class ContainerDao : IContainerDao
    {
        /// <summary>
        /// The contained IID key.
        /// </summary>
        private const string ContainedIidKey = "ContainedIid";

        /// <summary>
        /// The type info key.
        /// </summary>
        private const string TypeInfoKey = "TypeInfo";

        /// <summary>
        /// The IID key.
        /// </summary>
        private const string IidKey = "Iid";

        /// <summary>
        /// The sequence key.
        /// </summary>
        private const string SequenceKey = "Sequence";

        /// <summary>
        /// The same as connected partition key.
        /// </summary>
        private const string SameAsConnectedPartitionKey = "SameAsConnectedPartition";

        /// <summary>
        /// Gets or sets the Command logger.
        /// </summary>
        public ICommandLogger CommandLogger { get; set; }

        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="typeName">
        /// The type Name of the <see cref="CDP4Common.DTO.Thing"/> for which to return the container information.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ContainerInfo"/>.
        /// </returns>
        public IEnumerable<Tuple<Guid, ContainerInfo>> Read(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            if (partition == Utils.SiteDirectoryPartition)
            {
                // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
                return this.ReadInternalFromSiteDirectory(transaction, partition, typeName, ids).ToList();
            }

            return this.ReadInternalFromEngineeringModel(transaction, partition, typeName, ids).ToList();
        }

        /// <summary>
        /// Internal read method that uses yield to return the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="typeName">
        /// The type Name of the <see cref="CDP4Common.DTO.Thing"/> for which to return the container information.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ContainerInfo"/>.
        /// </returns>
        private IEnumerable<Tuple<Guid, ContainerInfo>> ReadInternalFromSiteDirectory(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();

            var connectedPartition = partition;

            sqlBuilder.Append(
                "SELECT \"{1}\".\"{2}\" AS \"{3}\",").Append(
                " \"AllThings\".\"ValueTypeSet\"->'ClassKind' AS \"{4}\",").Append(
                " \"AllThings\".\"{2}\",").Append(
                " \"{1}\".\"{5}\"").Append(
                " FROM \"{0}\".\"{1}_View\" \"{1}\"").Append(
                " JOIN \"{0}\".\"Thing_View\" AS \"AllThings\"").Append(
                " ON(\"{1}\".\"{2}\" = ANY(:ids) AND \"{1}\".\"Container\" = \"AllThings\".\"{2}\");");

            var sql = string.Format(
                sqlBuilder.ToString(),
                /*{0}*/ connectedPartition,
                /*{1}*/ typeName,
                /*{2}*/ IidKey,
                /*{3}*/ ContainedIidKey,
                /*{4}*/ TypeInfoKey,
                /*{5}*/ SequenceKey);

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();
                
                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToSiteDirectoryContainmentInfo(reader, connectedPartition);
                    }
                }
            }
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <param name="connectedPartition">
        /// The connected Partition.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="ContainerInfo"/>.
        /// </returns>
        private Tuple<Guid, ContainerInfo> MapToSiteDirectoryContainmentInfo(NpgsqlDataReader reader, string connectedPartition)
        {
            var containedIid = (Guid)reader[ContainedIidKey];

            // resolved container info
            var typeInfo = reader[TypeInfoKey].ToString();
            var iid = (Guid)reader[IidKey];
            var sequence = reader[SequenceKey] is DBNull ? -1 : long.Parse(reader[SequenceKey].ToString());

            return Tuple.Create(
                containedIid,
                new ContainerInfo(typeInfo, iid, sequence)
                {
                    Partition = connectedPartition
                });
        }

        /// <summary>
        /// Internal read method that uses yield to return the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="typeName">
        /// The type Name of the <see cref="CDP4Common.DTO.Thing"/> for which to return the container information.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ContainerInfo"/>.
        /// </returns>
        private IEnumerable<Tuple<Guid, ContainerInfo>> ReadInternalFromEngineeringModel(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();

            var connectedPartition = partition;
            var otherPartition = partition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            if (connectedPartition == otherPartition)
            {
                // make sure that other partition is referenced 
                otherPartition = partition.Replace(Utils.IterationSubPartition, Utils.EngineeringModelPartition);
            }

            sqlBuilder.Append(
                "SELECT \"{2}\".\"{3}\" AS \"{4}\",").Append(
                " \"AllThings\".\"{5}\",").Append(
                " \"AllThings\".\"{3}\",").Append(
                "  \"{2}\".\"{6}\",").Append(
                " \"AllThings\".\"{7}\"").Append(
                " FROM \"{0}\".\"{2}_View\" \"{2}\"").Append(
                " JOIN (SELECT \"{3}\", \"ValueTypeSet\"->'ClassKind' AS \"{5}\", 'true'::boolean AS \"{7}\"").Append(
                "       FROM \"{0}\".\"Thing_View\"").Append(
                "       UNION ALL").Append(
                "       SELECT \"{3}\", \"ValueTypeSet\"->'ClassKind' AS \"{5}\", 'false'::boolean AS \"{7}\"").Append(
                "       FROM \"{1}\".\"Thing_View\") AS \"AllThings\"").Append(
                " ON(\"{2}\".\"{3}\" = ANY(:ids) AND \"{2}\".\"Container\" = \"AllThings\".\"{3}\");");

            var sql = string.Format(
                sqlBuilder.ToString(),
                /*{0}*/ connectedPartition,
                /*{1}*/ otherPartition,
                /*{2}*/ typeName,
                /*{3}*/ IidKey,
                /*{4}*/ ContainedIidKey,
                /*{5}*/ TypeInfoKey,
                /*{6}*/ SequenceKey,
                /*{7}*/ SameAsConnectedPartitionKey);

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToEngineeringModelContainmentInfo(reader, connectedPartition, otherPartition);
                    }
                }
            }
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <param name="connectedPartition">
        /// The connected Partition.
        /// </param>
        /// <param name="otherPartition">
        /// The sub Partition.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="ContainerInfo"/>.
        /// </returns>
        private Tuple<Guid, ContainerInfo> MapToEngineeringModelContainmentInfo(NpgsqlDataReader reader, string connectedPartition, string otherPartition)
        {
            var containedIid = (Guid)reader[ContainedIidKey];

            // resolved container info
            var typeInfo = reader[TypeInfoKey].ToString();
            var iid = (Guid)reader[IidKey];
            var sequence = reader[SequenceKey] is DBNull ? -1 : long.Parse(reader[SequenceKey].ToString());
            var isConnectedPartition = (bool)reader[SameAsConnectedPartitionKey];

            return Tuple.Create(
                containedIid,
                new ContainerInfo(typeInfo, iid, sequence)
                    {
                        Partition =
                            isConnectedPartition
                                ? connectedPartition
                                : otherPartition
                    });
        }
    }
}
