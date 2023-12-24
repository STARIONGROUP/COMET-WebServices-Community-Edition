// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CDP4Orm.Dao.Revision
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using Microsoft.Extensions.Logging;
    
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Npgsql;

    using NpgsqlTypes;

    using Resolve;

    /// <summary>
    /// A data access class that allows revision based retrieval of concepts from the data store.
    /// </summary>
    public class RevisionDao : IRevisionDao
    {
        /// <summary>
        /// The Revision history table suffix
        /// </summary>
        private const string RevisionTableSuffix = "_Revision";

        /// <summary>
        /// The column name of the JSON representation of the thing in the revision-history table
        /// </summary>
        private const string JsonColumnName = "Jsonb";

        /// <summary>
        /// The column name of the Revision number of the thing in the revision-history table
        /// </summary>
        private const string RevisionColumnName = "RevisionNumber";

        /// <summary>
        /// The same as connected partition key.
        /// </summary>
        private const string SameAsConnectedPartitionKey = "SameAsConnectedPartition";

        /// <summary>
        /// The type info key.
        /// </summary>
        private const string TypeInfoKey = "TypeInfo";

        /// <summary>
        /// The IID key.
        /// </summary>
        private const string IidKey = "Iid";

        /// <summary>
        /// The Instant key.
        /// </summary>
        private const string InstantColumn = "Instant";

        /// <summary>
        /// The Actor key.
        /// </summary>
        private const string ActorColumn = "Actor";

        /// <summary>
        /// Use the comparator to determine the changes in the indicated revision only.
        /// </summary>
        private const string ChangesInCurrentRevision = "=";

        /// <summary>
        /// Use the comparator to determine the changes since the indicated revision.
        /// </summary>
        private const string ChangesSinceRevision = ">";

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        public ILogger<RevisionDao> Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IResolveDao"/> that retrieve metadata of an object
        /// </summary>
        public IResolveDao ResolveDao { get; set; }

        /// <summary>
        /// Retrieves data from the RevisionRegistry table in the specific partition.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionRegistryInfo"/>.
        /// </returns>
        public ReadOnlyCollection<RevisionRegistryInfo> ReadRevisionRegistry(NpgsqlTransaction transaction, string partition)
        {
            return InternalGetRevisionRegistryInfo(transaction, partition);
        }

        /// <summary>
        /// Retrieves the data that was changed after the indicated revision.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number from which to determine the delta response up to the current revision.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        public ReadOnlyCollection<RevisionInfo> Read(NpgsqlTransaction transaction, string partition, int revision)
        {
            return InternalGetRevisionInfo(transaction, partition, revision, ChangesSinceRevision);
        }

        /// <summary>
        /// Retrieves the data that was changed in the indicated revision.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number from which to return a delta response.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        public ReadOnlyCollection<RevisionInfo> ReadCurrentRevisionChanges(NpgsqlTransaction transaction, string partition, int revision)
        {
            return InternalGetRevisionInfo(transaction, partition, revision, ChangesInCurrentRevision);
        }

        /// <summary>
        /// Save The revision of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        /// <param name="actor">The identifier of the person who made this revision</param>
        public void WriteRevision(NpgsqlTransaction transaction, string partition, Thing thing, Guid actor)
        {
            this.Logger.LogDebug("WriteRevision of {thing} to {partition} by {actor}", thing, partition, actor);

            var table = GetThingRevisionTableName(thing);

            var columns = $"(\"{IidKey}\", \"{RevisionColumnName}\", \"{InstantColumn}\", \"{ActorColumn}\", \"{JsonColumnName}\")";
            var values = "(:iid, :revisionnumber, \"SiteDirectory\".get_transaction_time(), :actor, :jsonb)";
            var sqlQuery = $"INSERT INTO \"{partition}\".\"{table}\" {columns} VALUES {values}";

            using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

            command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = thing.Iid;
            command.Parameters.Add("revisionnumber", NpgsqlDbType.Integer).Value = thing.RevisionNumber;
            command.Parameters.Add("actor", NpgsqlDbType.Uuid).Value = actor;
            command.Parameters.Add("jsonb", NpgsqlDbType.Jsonb).Value = thing.ToJsonObject().ToString(Formatting.None);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Read the revisions of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thingIid">The identifier of the <see cref="Thing"/></param>
        /// <param name="revisionFrom">The oldest revision to query</param>
        /// <param name="revisionTo">The latest revision to query</param>
        /// <returns>The collection of revised <see cref="Thing"/></returns>
        public ReadOnlyCollection<Thing> ReadRevision(NpgsqlTransaction transaction, string partition, Guid thingIid, int revisionFrom, int revisionTo)
        {
            var result = new List<Thing>();

            var resolveInfos = this.ResolveDao.Read(transaction, partition, new[] { thingIid }).ToArray();

            if (resolveInfos.Length > 1)
            {
                throw new InvalidOperationException($"Multiple entries were found for {thingIid}");
            }

            var resolveInfo = resolveInfos.SingleOrDefault();

            if (resolveInfo != null)
            {
                var revisionTableName = GetThingRevisionTableName(resolveInfo);

                var sqlQuery = $"SELECT \"{JsonColumnName}\" FROM \"{resolveInfo.Partition}\".\"{revisionTableName}\" WHERE \"{IidKey}\" = :iid AND \"{RevisionColumnName}\" >= :fromrevision AND \"{RevisionColumnName}\" <= :torevision";

                using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = resolveInfo.InstanceInfo.Iid;
                command.Parameters.Add("fromrevision", NpgsqlDbType.Integer).Value = revisionFrom;
                command.Parameters.Add("torevision", NpgsqlDbType.Integer).Value = revisionTo;

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var thing = this.MapToDto(reader);

                    if (thing != null)
                    {
                        result.Add(thing);
                    }
                }
            }

            return result.AsReadOnly();
        }

        /// <summary>
        /// Gets a unique revision number for this transaction by reading it from the RevisionRegistry table, or adding it there if it does not exist yet
        /// This ensures that there is only 
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <returns>
        /// The current or next available revision number
        /// </returns>
        public int GetRevisionForTransaction(NpgsqlTransaction transaction, string partition)
        {
            var sqlQuery = $"SELECT * FROM \"{partition}\".get_current_revision();";

            using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

            if (!int.TryParse(command.ExecuteScalar()?.ToString(), out var revision))
            {
                throw new DataException("The revision number for this transaction could not be retrieved, cancel processing");
            }

            return revision;
        }

        /// <summary>
        /// Insert new values into the IterationRevisionLog table
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <param name="iteration">
        /// The iteration associated to the revision
        /// </param>
        /// <param name="fromRevision">
        /// The starting revision number for the iteration. If null the current revision is used.
        /// </param>
        /// <param name="toRevision">
        /// The ending revision number for the iteration. If null it means the iteration is the current one.
        /// </param>
        public void InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision)
        {
            var iterationColumn = "\"IterationIid\"";
            var fromRevisionColumn = "\"FromRevision\"";
            var toRevisionColumn = "\"ToRevision\"";

            var iterationValue = ":iterationId";
            var fromRevisionValue = ":fromRevision";
            var toRevisionValue = ":toRevision";

            var columns = new List<string> { iterationColumn };
            var values = new List<string> { iterationValue };

            if (fromRevision.HasValue)
            {
                columns.Add(fromRevisionColumn);
                values.Add(fromRevisionValue);
            }

            if (toRevision.HasValue)
            {
                columns.Add(toRevisionColumn);
                values.Add(toRevisionValue);
            }

            var sqlQuery = $"INSERT INTO \"{partition}\".\"IterationRevisionLog\" ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";

            using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

            command.Parameters.Add("iterationId", NpgsqlDbType.Uuid).Value = iteration;

            if (fromRevision.HasValue)
            {
                command.Parameters.Add("fromRevision", NpgsqlDbType.Integer).Value = fromRevision;
            }

            if (toRevision.HasValue)
            {
                command.Parameters.Add("toRevision", NpgsqlDbType.Integer).Value = toRevision;
            }

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// The internal get revision info.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="revision">
        /// The revision.
        /// </param>
        /// <param name="comparator">
        /// The comparator.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        private static ReadOnlyCollection<RevisionInfo> InternalGetRevisionInfo(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
            if (partition == Utils.SiteDirectoryPartition)
            {
                return ReadSiteDirectoryRevisions(transaction, partition, revision, comparator);
            }

            return ReadEngineeringModelRevisions(transaction, partition, revision, comparator);
        }

        /// <summary>
        /// Read the entries in the RevisionsRegistry table of specific partition
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <returns>The collection of revised <see cref="Thing"/></returns>
        private static ReadOnlyCollection<RevisionRegistryInfo> InternalGetRevisionRegistryInfo(NpgsqlTransaction transaction, string partition)
        {
            var sqlQuery = $"SELECT \"Revision\", \"Instant\", \"Actor\" FROM \"{partition}\".\"RevisionRegistry\"";

            using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

            using var reader = command.ExecuteReader();

            return MapToRevisionRegistryInfoList(reader);
        }

        /// <summary>
        /// The mapping from a database record to <see cref="IEnumerable{RevisionRegistryInfo}"/> object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// Enumerable of <see cref="RevisionRegistryInfo"/>.
        /// </returns>
        private static ReadOnlyCollection<RevisionRegistryInfo> MapToRevisionRegistryInfoList(NpgsqlDataReader reader)
        {
            var result = new List<RevisionRegistryInfo>();

            while (reader.Read())
            {
                var revision = reader["Revision"];
                var instant = reader["Instant"];
                var actor = reader["Actor"];

                result.Add(new RevisionRegistryInfo
                {
                    Revision = (int?)revision ?? 0,
                    Instant = (DateTime?)instant ?? DateTime.MinValue,
                    Actor = actor == DBNull.Value ? Guid.Empty : (Guid)actor
                });
            }

            return result.AsReadOnly();
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
        /// <param name="revision">
        /// revision to retrieve from the database.
        /// </param>
        /// <param name="comparator">
        /// The comparator used to determine the delta response.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        private static ReadOnlyCollection<RevisionInfo> ReadSiteDirectoryRevisions(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
            var result = new List<RevisionInfo>();

            var sqlBuilder = new System.Text.StringBuilder();

            // get all Thing 'concepts' whose revisions are as per the supplied revision comparator
            sqlBuilder.Append(
                "SELECT \"ValueTypeSet\"-> 'ClassKind' as \"{1}\", \"{2}\"").Append(
                " FROM \"{0}\".\"Thing_View\"").Append(
                " WHERE (\"ValueTypeSet\" -> 'RevisionNumber')::integer {3} :revision;");

            var sql = string.Format(
                sqlBuilder.ToString(),
                partition,
                TypeInfoKey,
                IidKey, 
                comparator);

            using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("revision", NpgsqlDbType.Integer).Value = revision;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(MapToSiteDirectoryRevisionInfo(reader, partition));
            }

            return result.AsReadOnly();
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
        /// A deserialized instance of <see cref="RevisionInfo"/>.
        /// </returns>
        private static RevisionInfo MapToSiteDirectoryRevisionInfo(NpgsqlDataReader reader, string connectedPartition)
        {
            var iid = (Guid)reader[IidKey];
            var typeInfo = reader[TypeInfoKey].ToString();

            var dto = new RevisionInfo { TypeInfo = typeInfo, Iid = iid, Partition = connectedPartition };

            return dto;
        }

        /// <summary>
        /// Internal read method that returns the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// revision to retrieve from the database.
        /// </param>
        /// <param name="comparator">
        /// The comparator used to determine the delta response.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        private static ReadOnlyCollection<RevisionInfo> ReadEngineeringModelRevisions(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
            var result = new List<RevisionInfo>();

            var sqlBuilder = new System.Text.StringBuilder();

            var connectedPartition = partition;
            var subPartition = partition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            // get all Thing 'concepts' whose revisions are as per the supplied revision comparator
            sqlBuilder.Append("SELECT \"AllThings\".\"{2}\", \"AllThings\".\"{3}\", \"AllThings\".\"{4}\"").Append(
                " FROM (SELECT \"{3}\", \"ValueTypeSet\"->'ClassKind' AS \"{2}\", 'true'::boolean AS \"{4}\"").Append(
                "       FROM \"{0}\".\"Thing_View\"").Append(
                "       WHERE (\"ValueTypeSet\" -> 'RevisionNumber')::integer {5} :revision").Append(
                "       UNION ALL").Append(
                "       SELECT \"{3}\", \"ValueTypeSet\"->'ClassKind' AS \"{2}\", 'false'::boolean AS \"{4}\"").Append(
                "       FROM \"{1}\".\"Thing_View\"").Append(
                "       WHERE (\"ValueTypeSet\" -> 'RevisionNumber')::integer {5} :revision) AS \"AllThings\";");

            var sql = string.Format(
                sqlBuilder.ToString(),
                connectedPartition,
                subPartition,
                TypeInfoKey,
                IidKey,
                SameAsConnectedPartitionKey,
                comparator);

            using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add("revision", NpgsqlDbType.Integer).Value = revision;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(MapToEngineeringModelRevisionInfo(reader, connectedPartition, subPartition));
            }

            return result.AsReadOnly();
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
        /// A deserialized instance of <see cref="RevisionInfo"/>.
        /// </returns>
        private static RevisionInfo MapToEngineeringModelRevisionInfo(NpgsqlDataReader reader, string connectedPartition, string subPartition)
        {
            var iid = (Guid)reader[IidKey];
            var typeInfo = reader[TypeInfoKey].ToString();
            var isConnectedPartition = (bool)reader[SameAsConnectedPartitionKey];

            var dto = new RevisionInfo
                          {
                              TypeInfo = typeInfo,
                              Iid = iid,
                              Partition = isConnectedPartition ? connectedPartition : subPartition
                          };

            return dto;
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="ResolveInfo"/>
        /// </summary>
        /// <param name="resolveInfo">The <see cref="ResolveInfo"/></param>
        /// <returns>The name of the revision table</returns>
        private static string GetThingRevisionTableName(ResolveInfo resolveInfo)
        {
            return resolveInfo.InstanceInfo.TypeName + RevisionTableSuffix;
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>The name of the revision table</returns>
        private static string GetThingRevisionTableName(Thing thing)
        {
            return thing.ClassKind + RevisionTableSuffix;
        }

        /// <summary>
        /// Instantiates a <see cref="Thing"/> from the content of a <see cref="NpgsqlDataReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="NpgsqlDataReader"/></param>
        /// <returns>A <see cref="Thing"/></returns>
        private Thing MapToDto(NpgsqlDataReader reader)
        {
            var jsonObject = JObject.Parse(reader.GetValue(0).ToString());

            Thing thing;

            try
            {
                thing = jsonObject.ToDto();
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, "An error occured when converting the JSONB to DTO on the revision-history query.");
                thing = null;
            }

            return thing;
        }
    }
}
