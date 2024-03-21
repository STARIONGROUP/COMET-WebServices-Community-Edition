// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramCanvasDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Linq;
    using System.Text;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The ArchitectureDiagram Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class DiagramCanvasDao
    {
        /// <summary>
        /// The (injected) <see cref="IArchitectureDiagramDao"/>
        /// </summary>
        public IArchitectureDiagramDao ArchitectureDiagramDao { get; set; }

        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="diagramElementThingId">
        /// Id to retrieve from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.ValueGroup"/>.
        /// </returns>
        public virtual DiagramCanvas GetTopDiagramCanvas(NpgsqlTransaction transaction, string partition, Guid diagramElementThingId)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                sqlBuilder.Append($"""
                                     WITH RECURSIVE diagram_hierarchy AS
                                     (
                                         SELECT "Iid", "Container"
                                         FROM "{partition}"."DiagramElementThing"
                                         WHERE "ValidTo" = 'infinity' AND "Iid" = :id
                                   
                                         UNION ALL
                                   
                                         SELECT "diagramElementContainer"."Iid", "diagramElementThing"."Container"
                                         FROM "{partition}"."DiagramElementContainer" "diagramElementContainer"
                                         LEFT JOIN "{partition}"."DiagramElementThing" "diagramElementThing" ON "diagramElementContainer"."Iid" = "diagramElementThing"."Iid"
                                         JOIN diagram_hierarchy "diagramElementThingHierarchy" ON "diagramElementContainer"."Iid" = "diagramElementThingHierarchy"."Container"
                                         WHERE "diagramElementContainer"."ValidTo" = 'infinity')
                                     )
                                     
                                     SELECT thing."Iid", thing."ValueTypeDictionary" -> 'ClassKind' AS "ClassKind"
                                     FROM diagram_hierarchy
                                     INNER JOIN "{partition}"."Thing" thing ON diagram_hierarchy."Iid" = thing."Iid" 
                                     WHERE "Container" IS NULL
                                       AND thing."ValidTo" = 'infinity';
                                   """);

                command.Parameters.Add("id", NpgsqlDbType.Uuid).Value = diagramElementThingId;

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sqlBuilder.ToString();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var topContainerIid = Guid.Parse(reader["Iid"].ToString());
                        var classKind = Enum.Parse<ClassKind>(reader["ClassKind"].ToString());

                        switch (classKind)
                        {
                            case ClassKind.DiagramCanvas:
                                return this.Read(transaction, partition, new[] { topContainerIid }).FirstOrDefault();

                            case ClassKind.ArchitectureDiagram:
                                return this.ArchitectureDiagramDao.Read(transaction, partition, new[] { topContainerIid }).FirstOrDefault();

                            default :
                                return null;
                        }
                    }
                }
            }

            return null;
        }
    }
}
