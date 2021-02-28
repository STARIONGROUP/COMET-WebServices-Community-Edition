// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationService.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.MigrationEngine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Npgsql;

    /// <summary>
    /// The class responsble for applying all migration scripts
    /// </summary>
    public class MigrationService : IMigrationService
    {
        /// <summary>
        /// The namespace of the migration scripts
        /// </summary>
        public const string MIGRATION_SCRIPT_NAMESPACE = "CDP4Orm.MigrationScript.";

        /// <summary>
        /// Aplply the full set of migrations on a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The target partition</param>
        /// <param name="isStartup">Asserts whether the <see cref="IMigrationService"/> is called on startup</param>
        public void ApplyMigrations(NpgsqlTransaction transaction, string partition, bool isStartup)
        {
            var migrations = GetMigrations(isStartup).Where(x => partition.StartsWith(x.MigrationMetaData.MigrationScriptApplicationKind.ToString()) || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All);
            foreach (var migrationBase in migrations.OrderBy(x => x.MigrationMetaData.Version))
            {
                migrationBase.ApplyMigration(transaction, new [] { partition });
            }
        }

        /// <summary>
        /// Gets all migration scripts
        /// </summary>
        /// <returns>The List of <see cref="MigrationBase"/></returns>
        public static IReadOnlyList<MigrationBase> GetMigrations(bool isStartup)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceFullNames = assembly.GetManifestResourceNames().Where(x => x.StartsWith(MIGRATION_SCRIPT_NAMESPACE) && x.EndsWith(".sql"));

            var migrationScriptMetadatas = resourceFullNames.Select(x => new MigrationMetaData(x)).ToList();
            var migrationList = new List<MigrationBase>();
            foreach (var migrationMetaData in migrationScriptMetadatas)
            {
                if (migrationMetaData.MigrationScriptKind.HasValue && migrationMetaData.MigrationScriptKind.Value == MigrationScriptKind.OnStartUpOnly && !isStartup)
                {
                    // do not run script meant to be only executed on startup
                    continue;
                }

                MigrationBase migration;
                switch (migrationMetaData.MigrationScriptKind)
                {
                    case CDP4Orm.MigrationEngine.MigrationScriptKind.NonThingTableMigrationTemplate:
                        migration = new NonThingTableMigration(migrationMetaData);
                        break;
                    default:
                        migration = new GenericMigration(migrationMetaData);
                        break;
                }

                migrationList.Add(migration);
            }

            return migrationList;
        }
    }
}
