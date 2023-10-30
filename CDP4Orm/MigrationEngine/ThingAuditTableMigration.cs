// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingAuditTableMigration.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.MigrationEngine
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A migration script handler class that is used to handle migration script template that are applied on every non-thing table
    /// </summary>
    public class ThingAuditTableMigration : ThingTableMigrationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingAuditTableMigration"/> class
        /// </summary>
        /// <param name="migrationFileMetadata">The migration metadata</param>
        /// <param name="logger">
        /// The <see cref="ILogger{T}"></see> of type <see cref="ThingAuditTableMigration"/>/>
        /// </param>
        public ThingAuditTableMigration(MigrationMetaData migrationFileMetadata, ILogger<ThingAuditTableMigration> logger) : base(migrationFileMetadata, logger)
        {
        }

        /// <summary>
        /// Gets the template string that defines the type of ThingTableMigration to be used in for example; logging statements
        /// </summary>
        /// <returns></returns>
        protected override string GetTableNameTemplate()
        {
            return "Thing Audit";
        }

        /// <summary>
        /// Aplies a specific filter on the TableName
        /// </summary>
        /// <param name="thingTypes">The type of things that are allowed to be used for this migration</param>
        /// <param name="tableName">The name of the table that was found while searching the database for tables</param>
        /// <returns>True is the entry is allowed to be used in the Migration</returns>
        protected override bool ApplyFilter(List<string> thingTypes, string tableName)
        {
            return tableName.EndsWith("_Audit") &&
                   thingTypes.Contains(
                       tableName.Substring(0, tableName.LastIndexOf("_Audit", StringComparison.Ordinal)
                       ));
        }
    }
}
