// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao.Revision
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NLog;

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
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the Command logger.
        /// </summary>
        public ICommandLogger CommandLogger { get; set; }

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
        public IEnumerable<RevisionRegistryInfo> ReadRevisionRegistry(NpgsqlTransaction transaction, string partition)
        {
            return this.InternalGetRevisionRegistryInfo(transaction, partition);
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
        public IEnumerable<RevisionInfo> Read(NpgsqlTransaction transaction, string partition, int revision)
        {
            return this.InternalGetRevisionInfo(transaction, partition, revision, ChangesSinceRevision);
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
        public IEnumerable<RevisionInfo> ReadCurrentRevisionChanges(NpgsqlTransaction transaction, string partition, int revision)
        {
            return this.InternalGetRevisionInfo(transaction, partition, revision, ChangesInCurrentRevision);
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
            var table = this.GetThingRevisionTableName(thing);

            var columns = string.Format("(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\")", IidKey, RevisionColumnName, InstantColumn, ActorColumn, JsonColumnName);
            var values = "(:iid, :revisionnumber, \"SiteDirectory\".get_transaction_time(), :actor, :jsonb)";
            var sqlQuery = string.Format("INSERT INTO \"{0}\".\"{1}\" {2} VALUES {3}", partition, table, columns, values);

            using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
            {
                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = thing.Iid;
                command.Parameters.Add("revisionnumber", NpgsqlDbType.Integer).Value = thing.RevisionNumber;
                command.Parameters.Add("actor", NpgsqlDbType.Uuid).Value = actor;
                command.Parameters.Add("jsonb", NpgsqlDbType.Jsonb).Value = thing.ToJsonObject().ToString(Formatting.None);

                // log the sql command 
                command.ExecuteNonQuery();
            }
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
        public IEnumerable<Thing> ReadRevision(NpgsqlTransaction transaction, string partition, Guid thingIid, int revisionFrom, int revisionTo)
        {
            var resolveInfos = this.ResolveDao.Read(transaction, partition, new[] { thingIid }).ToArray();

            if (resolveInfos.Length > 1)
            {
                throw new InvalidOperationException($"Multiple entries were found for {thingIid}");
            }

            var resolveInfo = resolveInfos.SingleOrDefault();

            if (resolveInfo != null)
            {
                var revisionTableName = this.GetThingRevisionTableName(resolveInfo);

                var sqlQuery = string.Format(
                    "SELECT \"{0}\" FROM \"{1}\".\"{2}\" WHERE \"{3}\" = :iid AND \"{4}\" >= :fromrevision AND \"{4}\" <= :torevision",
                    JsonColumnName,
                    resolveInfo.Partition,
                    revisionTableName,
                    IidKey,
                    RevisionColumnName);

                using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
                {
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = resolveInfo.InstanceInfo.Iid;
                    command.Parameters.Add("fromrevision", NpgsqlDbType.Integer).Value = revisionFrom;
                    command.Parameters.Add("torevision", NpgsqlDbType.Integer).Value = revisionTo;

                    // log the sql command 
                    this.CommandLogger.Log(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapToDto(reader);

                            if (thing != null)
                            {
                                yield return thing;
                            }
                        }
                    }
                }
            }
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
            var sqlQuery = string.Format(
                "SELECT * FROM \"{0}\".get_current_revision();",
                partition);

            int revision;

            using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
            {
                if (!int.TryParse(command.ExecuteScalar()?.ToString(), out revision))
                {
                    throw new ApplicationException("The revision number for this transaction could not be retrieved, cancel processing");
                }
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

            var sqlQuery = string.Format(
                "INSERT INTO \"{0}\".\"IterationRevisionLog\" ({1}) VALUES ({2})",
                partition,
                string.Join(", ", columns),
                string.Join(", ", values));

            using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
            {
                command.Parameters.Add("iterationId", NpgsqlDbType.Uuid).Value = iteration;
                if (fromRevision.HasValue)
                {
                    command.Parameters.Add("fromRevision", NpgsqlDbType.Integer).Value = fromRevision;
                }

                if (toRevision.HasValue)
                {
                    command.Parameters.Add("toRevision", NpgsqlDbType.Integer).Value = toRevision;
                }

                // log the sql command 
                this.CommandLogger.ExecuteAndLog(command);
            }
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
        private IEnumerable<RevisionInfo> InternalGetRevisionInfo(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
            if (partition == Utils.SiteDirectoryPartition)
            {
                return this.ReadSiteDirectoryRevisions(transaction, partition, revision, comparator).ToList();
            }

            // make sure to wrap the yield result as list; the internal iterator yield response otherwise (somehow) sets the transaction to an invalid state. 
            return this.ReadEngineeringModelRevisions(transaction, partition, revision, comparator).ToList();
        }

        /// <summary>
        /// Read the entries in the RevisionsRegistry table of specific partition
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <returns>The collection of revised <see cref="Thing"/></returns>
        private IEnumerable<RevisionRegistryInfo> InternalGetRevisionRegistryInfo(NpgsqlTransaction transaction, string partition)
        {
            var sqlQuery = $"SELECT \"Revision\", \"Instant\", \"Actor\" FROM \"{partition}\".\"RevisionRegistry\"";

            using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
            {
                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    return this.MapToRevisionRegistryInfoList(reader).ToList();
                }
            }
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
        private IEnumerable<RevisionRegistryInfo> MapToRevisionRegistryInfoList(NpgsqlDataReader reader)
        {
            while (reader.Read())
            {
                var revision = reader["Revision"];
                var instant = reader["Instant"];
                var actor = reader["Actor"];

                yield return new RevisionRegistryInfo
                {
                    Revision = (int?) revision ?? 0,
                    Instant = (DateTime?) instant ?? DateTime.MinValue,
                    Actor = actor == DBNull.Value ? Guid.Empty : (Guid) actor
                };
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
        /// <param name="revision">
        /// revision to retrieve from the database.
        /// </param>
        /// <param name="comparator">
        /// The comparator used to determine the delta response.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        private IEnumerable<RevisionInfo> ReadSiteDirectoryRevisions(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
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

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("revision", NpgsqlDbType.Integer).Value = revision;

                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToSiteDirectoryRevisionInfo(reader, partition);
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
        /// A deserialized instance of <see cref="RevisionInfo"/>.
        /// </returns>
        private RevisionInfo MapToSiteDirectoryRevisionInfo(NpgsqlDataReader reader, string connectedPartition)
        {
            var iid = (Guid)reader[IidKey];
            var typeInfo = reader[TypeInfoKey].ToString();

            var dto = new RevisionInfo { TypeInfo = typeInfo, Iid = iid, Partition = connectedPartition };

            return dto;
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
        private IEnumerable<RevisionInfo> ReadEngineeringModelRevisions(NpgsqlTransaction transaction, string partition, int revision, string comparator)
        {
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
            
            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("revision", NpgsqlDbType.Integer).Value = revision;
                
                // log the sql command 
                this.CommandLogger.Log(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToEngineeringModelRevisionInfo(reader, connectedPartition, subPartition);
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
        /// A deserialized instance of <see cref="RevisionInfo"/>.
        /// </returns>
        private RevisionInfo MapToEngineeringModelRevisionInfo(NpgsqlDataReader reader, string connectedPartition, string subPartition)
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
        private string GetThingRevisionTableName(ResolveInfo resolveInfo)
        {
            return resolveInfo.InstanceInfo.TypeName + RevisionTableSuffix;
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>The name of the revision table</returns>
        private string GetThingRevisionTableName(Thing thing)
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
                Logger.Error(e, "An error occured when converting the JSONB to DTO on the revision-history query.");
                thing = null;
            }

            return thing;
        }
    }
}
