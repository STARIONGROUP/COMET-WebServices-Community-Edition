// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 System RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Cache
{
    using CDP4JsonSerializer;

    using CDP4Orm.Dao.Revision;

    using Newtonsoft.Json;

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
        /// Gets or sets the file dao.
        /// </summary>
        public IFileDao FileDao { get; set; }

        /// <summary>
        /// Gets or sets the folder dao.
        /// </summary>
        public IFolderDao FolderDao { get; set; }

        /// <summary>
        /// Gets or sets the fileRevision dao.
        /// </summary>
        public IFileRevisionDao FileRevisionDao { get; set; }

        /// <summary>
        /// Save a <see cref="CDP4Common.DTO.Thing"/> to a cache table 
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="CDP4Common.DTO.Thing"/></param>
        public void Write(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            var table = this.GetThingCacheTableName(thing);

            var columns = string.Format("(\"{0}\", \"{1}\", \"{2}\")", IidKey, RevisionColumnName, JsonColumnName);
            var values = "(:iid, :revisionnumber, :jsonb)";
            var sqlQuery = string.Format("INSERT INTO \"{0}\".\"{1}\" {2} VALUES {3} ON CONFLICT (\"{4}\") DO UPDATE SET \"{5}\"=:revisionnumber, \"{6}\"=:jsonb;", partition, table, columns, values, IidKey, RevisionColumnName, JsonColumnName);

            using (var command = new NpgsqlCommand(sqlQuery, transaction.Connection, transaction))
            {
                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = thing.Iid;
                command.Parameters.Add("revisionnumber", NpgsqlDbType.Integer).Value = thing.RevisionNumber;
                command.Parameters.Add("jsonb", NpgsqlDbType.Jsonb).Value = thing.ToJsonObject().ToString(Formatting.None);

                // log the sql command 
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets the revision table name for a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>The name of the revision table</returns>
        private string GetThingCacheTableName(Thing thing)
        {
            return thing.ClassKind + CacheTableSuffix;
        }
    }
}
