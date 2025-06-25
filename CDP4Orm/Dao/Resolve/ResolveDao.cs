// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveDao.cs" company="Starion Group S.A.">
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

namespace CDP4Orm.Dao.Resolve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The container retrieval DAO.
    /// </summary>
    public class ResolveDao : IResolveDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        public async Task<IEnumerable<ResolveInfo>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
            
            if (partition == Utils.SiteDirectoryPartition)
            {
                return (await ReadSiteDirectoryThing(transaction, partition, ids)).ToList();
            }

            // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
            return (await ReadEngineeringModelInternal(transaction, partition, ids)).ToList();
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
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        private static async Task<IEnumerable<ResolveInfo>> ReadSiteDirectoryThing(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();

            // get all Thing 'concepts' that are newer then passed in revision
            sqlBuilder.Append("SELECT \"ValueTypeDictionary\"-> 'ClassKind' as \"TypeInfo\", \"Iid\"");
            sqlBuilder.Append($" FROM \"{partition}\".\"Thing\"");
            sqlBuilder.Append("  WHERE \"Iid\" = ANY(:ids);");

            var sql = sqlBuilder.ToString();

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<ResolveInfo>();

            while (await reader.ReadAsync())
            {
                result.Add(MapToSiteDirectoryDto(reader));
            }

            return result;
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
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        /// <remarks>
        /// Do not use for SiteDirectory items
        /// </remarks>
        private static async Task<IEnumerable<ResolveInfo>> ReadEngineeringModelInternal(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();
            
            // union the EngineeringModel and subpartition (Iteration) to help resolve the requested Iids
            sqlBuilder.Append(
                "SELECT \"AllThings\".\"TypeInfo\", \"AllThings\".\"Iid\", \"AllThings\".\"SameAsConnectedPartition\"").Append(
                " FROM (SELECT \"Iid\", \"ValueTypeDictionary\"->'ClassKind' AS \"TypeInfo\", 'true'::boolean AS \"SameAsConnectedPartition\"").Append(
                "       FROM \"{0}\".\"Thing\"").Append(
                "       UNION ALL").Append(
                "       SELECT \"Iid\", \"ValueTypeDictionary\"->'ClassKind' AS \"TypeInfo\", 'false'::boolean AS \"SameAsConnectedPartition\"").Append(
                "       FROM \"{1}\".\"Thing\") AS \"AllThings\"").Append(
                " WHERE \"AllThings\".\"Iid\" = ANY(:ids);");

            var connectedPartition = partition;
            var subPartition = partition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            var sql = string.Format(
                sqlBuilder.ToString(),
                connectedPartition,
                subPartition);

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<ResolveInfo>();

            while (await reader.ReadAsync())
            {
                var resolveInfo = MapToEngineeringModelDto(reader, connectedPartition, subPartition);
                result.Add(resolveInfo);
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
        /// <param name="subPartition">
        /// The sub Partition.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="ContainerInfo"/>.
        /// </returns>
        private static ResolveInfo MapToEngineeringModelDto(NpgsqlDataReader reader, string connectedPartition, string subPartition)
        {
            var typeInfo = reader["TypeInfo"].ToString();
            var iid = (Guid)reader["Iid"];
            var isConnectedPartition = (bool)reader["SameAsConnectedPartition"];

            return new ResolveInfo
                       {
                           InstanceInfo = new DtoInfo(typeInfo, iid),
                           Partition = isConnectedPartition ? connectedPartition : subPartition
                       };
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="ResolveInfo"/>.
        /// </returns>
        private static ResolveInfo MapToSiteDirectoryDto(NpgsqlDataReader reader)
        {
            var typeInfo = reader["TypeInfo"].ToString();
            var iid = (Guid)reader["Iid"];

            return new ResolveInfo
            {
                InstanceInfo = new DtoInfo(typeInfo, iid)
            };
        }
    }
}
