// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
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
    public class ResolveDao : IResolveDao
    {
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
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        public IEnumerable<ResolveInfo> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
            
            if (partition == Utils.SiteDirectoryPartition)
            {
                return this.ReadSiteDirectoryThing(transaction, partition, ids).ToList();
            }

            // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
            return this.ReadEngineeringModelInternal(transaction, partition, ids).ToList();
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
        private IEnumerable<ResolveInfo> ReadSiteDirectoryThing(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();

            // get all Thing 'concepts' that are newer then passed in revision
            sqlBuilder.Append(
                "SELECT \"ValueTypeSet\"-> 'ClassKind' as \"TypeInfo\", \"Iid\"").Append(
                " FROM \"{0}\".\"Thing_View\"").Append(
                " WHERE \"Iid\" = ANY(:ids);");

            var sql = string.Format(sqlBuilder.ToString(), partition);

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

                // log the sql command 
                this.CommandLogger.Log(command);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToSiteDirectoryDto(reader);
                    }
                }
            }
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
        private IEnumerable<ResolveInfo> ReadEngineeringModelInternal(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids)
        {
            var sqlBuilder = new System.Text.StringBuilder();
            
            // union the EngineeringModel and subpartition (Iteration) to help resolve the requested Iids
            sqlBuilder.Append(
                "SELECT \"AllThings\".\"TypeInfo\", \"AllThings\".\"Iid\", \"AllThings\".\"SameAsConnectedPartition\"").Append(
                " FROM (SELECT \"Iid\", \"ValueTypeSet\"->'ClassKind' AS \"TypeInfo\", 'true'::boolean AS \"SameAsConnectedPartition\"").Append(
                "       FROM \"{0}\".\"Thing_View\"").Append(
                "       UNION ALL").Append(
                "       SELECT \"Iid\", \"ValueTypeSet\"->'ClassKind' AS \"TypeInfo\", 'false'::boolean AS \"SameAsConnectedPartition\"").Append(
                "       FROM \"{1}\".\"Thing_View\") AS \"AllThings\"").Append(
                " WHERE \"AllThings\".\"Iid\" = ANY(:ids);");

            var connectedPartition = partition;
            var subPartition = partition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            var sql = string.Format(
                sqlBuilder.ToString(),
                connectedPartition,
                subPartition);

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids.ToList();

                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToEngineeringModelDto(reader, connectedPartition, subPartition);
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
        /// <param name="subPartition">
        /// The sub Partition.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="ContainerInfo"/>.
        /// </returns>
        private ResolveInfo MapToEngineeringModelDto(NpgsqlDataReader reader, string connectedPartition, string subPartition)
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
        private ResolveInfo MapToSiteDirectoryDto(NpgsqlDataReader reader)
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
