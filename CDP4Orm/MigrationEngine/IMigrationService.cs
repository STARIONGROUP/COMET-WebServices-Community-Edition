// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.MigrationEngine
{
    using Npgsql;

    /// <summary>
    /// The interface for the migration service which is responsible for applying all migration scripts
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Aplply the full set of migrations on a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The target partition</param>
        void ApplyMigrations(NpgsqlTransaction transaction, string partition);
    }
}
