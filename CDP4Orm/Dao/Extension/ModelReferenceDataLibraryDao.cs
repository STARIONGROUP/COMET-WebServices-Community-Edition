// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The ModelReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ModelReferenceDataLibraryDao
    {
        /// <summary>
        /// GEts the identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> dependencies for <see cref="EngineeringModelSetup"/>s
        /// </summary>
        /// <param name="modelSetups">The <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <returns>The identifiers of the <see cref="SiteReferenceDataLibrary"/> dependency</returns>
        public IEnumerable<Guid> GetSiteReferenceDataLibraryDependency(IEnumerable<EngineeringModelSetup> modelSetups, NpgsqlTransaction transaction)
        {
            using (var command = new NpgsqlCommand())
            {
                var sql = $@"
WITH RECURSIVE get_chain AS (
    SELECT * FROM ""SiteDirectory"".""SiteReferenceDataLibrary_View"" WHERE ""Iid"" IN (SELECT ""RequiredRdl"" FROM ""SiteDirectory"".""ModelReferenceDataLibrary_View"" WHERE ""Iid"" = ANY(:modelRdls))
    UNION
    SELECT ""SiteDirectory"".""SiteReferenceDataLibrary_View"".*
        FROM ""SiteDirectory"".""SiteReferenceDataLibrary_View""
        JOIN get_chain on get_chain.""RequiredRdl"" = ""SiteDirectory"".""SiteReferenceDataLibrary_View"".""Iid""
)
SELECT * FROM get_chain";

                command.Parameters.Add("modelRdls", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = modelSetups.Select(x => x.RequiredRdl.First()).ToArray();

                command.CommandText = sql;
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                // log the sql command 
                this.LogCommand(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Guid.Parse(reader.GetValue(0).ToString());
                    }
                }
            }
        }
    }
}
