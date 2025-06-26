// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
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

namespace CDP4Orm.Dao.Resolve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Orm.Helper;

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
        /// Gets or sets the injected <see cref="IDaoResolver" />
        /// </summary>
        public IDaoResolver DaoResolver { get; set; }

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
        public async Task<IEnumerable<Tuple<Guid, ContainerInfo>>> ReadAsync(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            if (partition == Utils.SiteDirectoryPartition)
            {
                // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
                return  (await this.ReadInternalFromSiteDirectoryAsync(transaction, partition, typeName, ids)).ToList();
            }

            return (await this.ReadInternalFromEngineeringModelAsync(transaction, partition, typeName, ids)).ToList();
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
        private async Task<IEnumerable<Tuple<Guid, ContainerInfo>>> ReadInternalFromSiteDirectoryAsync(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new StringBuilder();

            var connectedPartition = partition;
            var dao = this.DaoResolver.QueryDaoByTypeName(typeName);

            sqlBuilder.Append($"SELECT \"{typeName}\".\"{IidKey}\" AS \"{ContainedIidKey}\",");
            sqlBuilder.Append($" \"AllThings\".\"ValueTypeDictionary\"->'ClassKind' AS \"{TypeInfoKey}\",");
            sqlBuilder.Append($" \"AllThings\".\"{IidKey}\",");
            sqlBuilder.Append($" \"{typeName}\".\"{SequenceKey}\"");
            sqlBuilder.Append($" FROM ({dao.BuildReadQuery(partition, null)}) \"{typeName}\"");
            sqlBuilder.Append($"    JOIN \"{connectedPartition}\".\"Thing\" AS \"AllThings\"");
            sqlBuilder.Append($"    ON(\"{typeName}\".\"{IidKey}\" = ANY(:ids) AND \"{typeName}\".\"Container\" = \"AllThings\".\"{IidKey}\");");

            var sql = sqlBuilder.ToString();

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<Tuple<Guid, ContainerInfo>>();

            while (await reader.ReadAsync())
            {
                var tuple = MapToSiteDirectoryContainmentInfo(reader, connectedPartition);

                result.Add(tuple);
            }

            return result;
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
        private static Tuple<Guid, ContainerInfo> MapToSiteDirectoryContainmentInfo(NpgsqlDataReader reader, string connectedPartition)
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
        private async Task<IEnumerable<Tuple<Guid, ContainerInfo>>> ReadInternalFromEngineeringModelAsync(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new StringBuilder();

            var connectedPartition = partition;
            var otherPartition = partition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            if (connectedPartition == otherPartition)
            {
                // make sure that other partition is referenced 
                otherPartition = partition.Replace(Utils.IterationSubPartition, Utils.EngineeringModelPartition);
            }

            var dao = this.DaoResolver.QueryDaoByTypeName(typeName);
            
            sqlBuilder.Append($"SELECT \"{typeName}\".\"{IidKey}\" AS \"{ContainedIidKey}\",");
            sqlBuilder.Append($" \"AllThings\".\"{TypeInfoKey}\",");
            sqlBuilder.Append($" \"AllThings\".\"{IidKey}\",");
            sqlBuilder.Append($" \"{typeName}\".\"{SequenceKey}\",");
            sqlBuilder.Append($" \"AllThings\".\"{SameAsConnectedPartitionKey}\"");
            sqlBuilder.Append($" FROM ({dao.BuildReadQuery(partition, null)}) \"{typeName}\"");
            sqlBuilder.Append($" JOIN (SELECT \"{IidKey}\", \"ValueTypeDictionary\"->'ClassKind' AS \"{TypeInfoKey}\", 'true'::boolean AS \"{SameAsConnectedPartitionKey}\"");
            sqlBuilder.Append($"    FROM \"{connectedPartition}\".\"Thing\"");
            sqlBuilder.Append("     UNION ALL");
            sqlBuilder.Append($"    SELECT \"{IidKey}\", \"ValueTypeDictionary\"->'ClassKind' AS \"{TypeInfoKey}\", 'false'::boolean AS \"{SameAsConnectedPartitionKey}\"");
            sqlBuilder.Append($"    FROM \"{otherPartition}\".\"Thing\") AS \"AllThings\"");
            sqlBuilder.Append($" ON(\"{typeName}\".\"{IidKey}\" = ANY(:ids) AND \"{typeName}\".\"Container\" = \"AllThings\".\"{IidKey}\");");

            var sql = sqlBuilder.ToString();

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<Tuple<Guid, ContainerInfo>>();

            while (await reader.ReadAsync())
            {
                var tuple = MapToEngineeringModelContainmentInfo(reader, connectedPartition, otherPartition);

                result.Add(tuple);
            }

            return result;
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
        private static Tuple<Guid, ContainerInfo> MapToEngineeringModelContainmentInfo(NpgsqlDataReader reader, string connectedPartition, string otherPartition)
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
