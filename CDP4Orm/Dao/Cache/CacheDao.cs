// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao.Revision;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using NpgsqlTypes;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The cache dao.
    /// </summary>
    public class CacheDao : ICacheDao
    {
        /// <summary>
        /// The Revision history table suffix
        /// </summary>
        private const string CacheTableSuffix = "_Cache";

        /// <summary>
        /// The column name of the JSON representation of the thing in the revision-history table
        /// </summary>
        private const string JsonColumnName = "Jsonb";

        /// <summary>
        /// The column name of the Revision number of the thing in the revision-history table
        /// </summary>
        private const string RevisionColumnName = "RevisionNumber";

        /// <summary>
        /// The IID key.
        /// </summary>
        private const string IidKey = "Iid";

        /// <summary>
        /// The number of bind parameters used per <see cref="Thing" /> by <see cref="BulkWriteAsync" />
        /// </summary>
        private const int BulkWriteParametersPerThing = 3;

        /// <summary>
        /// Gets or sets the file dao.
        /// </summary>
        public IFileDao FileDao { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="ILogger{T}" />
        /// </summary>
        public ILogger<CacheDao> Logger { get; set; }

        /// <summary>
        /// Save a <see cref="Thing" /> to a cache table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing" /></param>
        public async Task WriteAsync(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            var table = GetThingCacheTableName(thing);

            var columns = $"(\"{IidKey}\", \"{RevisionColumnName}\", \"{JsonColumnName}\")";
            var values = "(:iid, :revisionnumber, :jsonb)";
            var sqlQuery = $"INSERT INTO \"{partition}\".\"{table}\" {columns} VALUES {values} ON CONFLICT (\"{IidKey}\") DO UPDATE SET \"{RevisionColumnName}\"=:revisionnumber, \"{JsonColumnName}\"=:jsonb;";

            await using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

            command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = thing.Iid;
            command.Parameters.Add("revisionnumber", NpgsqlDbType.Integer).Value = thing.RevisionNumber;
            command.Parameters.Add("jsonb", NpgsqlDbType.Jsonb).Value = thing.ToJsonString();

            // log the sql command 
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Save a collection of <see cref="Thing" /> to cache tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="things">The collection of revised <see cref="Thing" />s</param>
        /// <exception cref="ArgumentException">When <paramref name="things" /> contains more than one <see cref="Thing" /> with the same iid</exception>
        public async Task BulkWriteAsync(NpgsqlTransaction transaction, string partition, IReadOnlyCollection<Thing> things)
        {
            // an INSERT ... ON CONFLICT DO UPDATE cannot affect the same row twice; reject duplicates up front so that
            // the outcome does not depend on whether they end up in the same batch or in different batches
            var duplicateIids = things.GroupBy(x => x.Iid).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            if (duplicateIids.Count > 0)
            {
                throw new ArgumentException($"The things to write to the cache contain duplicate iids: {string.Join(", ", duplicateIids)}", nameof(things));
            }

            var thingsGroupedByClasskind = things.GroupBy(x => x.ClassKind).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var groupedThings in thingsGroupedByClasskind)
            {
                var table = GetThingCacheTableName(groupedThings.Key);

                // a single statement cannot hold more than Utils.MaximumParametersPerStatement bind parameters
                foreach (var batch in Utils.ChunkByParameterLimit(groupedThings.Value, BulkWriteParametersPerThing))
                {
                    var sqlQueryBuilder = new StringBuilder();

                    sqlQueryBuilder.Append($"INSERT INTO \"{partition}\".\"{table}\" (\"{IidKey}\", \"{RevisionColumnName}\", \"{JsonColumnName}\") VALUES ");

                    var parameters = new List<NpgsqlParameter>(batch.Length * BulkWriteParametersPerThing);

                    for (var index = 0; index < batch.Length; index++)
                    {
                        var thing = batch[index];

                        var serializedJson = thing.ToJsonString();

                        if (string.IsNullOrWhiteSpace(serializedJson))
                        {
                            serializedJson = "{}";
                        }

                        parameters.Add(new NpgsqlParameter($"iid_{index}", NpgsqlDbType.Uuid) { Value = thing.Iid });
                        parameters.Add(new NpgsqlParameter($"revision_{index}", NpgsqlDbType.Integer) { Value = thing.RevisionNumber });
                        parameters.Add(new NpgsqlParameter($"jsonb_{index}", NpgsqlDbType.Jsonb) { Value = serializedJson });

                        sqlQueryBuilder.Append($"(@iid_{index}, @revision_{index}, @jsonb_{index})");

                        if (index < batch.Length - 1)
                        {
                            sqlQueryBuilder.Append(',');
                        }
                    }

                    sqlQueryBuilder.Append($" ON CONFLICT (\"{IidKey}\") DO UPDATE SET \"{RevisionColumnName}\"=EXCLUDED.\"{RevisionColumnName}\", \"{JsonColumnName}\"=EXCLUDED.\"{JsonColumnName}\";");

                    var sqlQuery = sqlQueryBuilder.ToString();

                    if (this.Logger.IsEnabled(LogLevel.Debug))
                    {
                        this.Logger.LogDebug("Running insert command for Cache : {SqlQuery}", sqlQuery);
                    }

                    await using var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction);

                    command.Parameters.AddRange(parameters.ToArray());
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The name of the revision table</returns>
        private static string GetThingCacheTableName(Thing thing)
        {
            return GetThingCacheTableName(thing.ClassKind);
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="ClassKind" />
        /// </summary>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>The name of the revision table</returns>
        private static string GetThingCacheTableName(ClassKind classKind)
        {
            return $"{classKind}{CacheTableSuffix}";
        }
    }
}
