// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorDao.cs" company="RHEA System S.A.">
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
    /// The Color Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ColorDao
    {
        /// <summary>
        /// Gets or sets the (Injected) <see cref="IDiagramCanvasDao"/>
        /// </summary>
        public IDiagramCanvasDao DiagramCanvasDao { get; set; }

        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="id">
        /// Id to retrieve from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.ValueGroup"/>.
        /// </returns>
        public DiagramCanvas GetTopDiagramCanvas(NpgsqlTransaction transaction, string partition, Guid id)
        {
            //get container DiagrammingStyle and use that for GetTopDiagramCanvas
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                sqlBuilder.Append($"""
                                   SELECT thing."Iid", thing."ValueTypeDictionary" -> 'ClassKind' AS "ClassKind"
                                   FROM "{partition}"."Color" color
                                   INNER JOIN "{partition}"."Thing" thing ON color."Container" = thing."Iid"
                                   WHERE color."Iid = :id"
                                     AND thing."ValidTo" = 'infinity';
                                   """
                );

                command.Parameters.Add("id", NpgsqlDbType.Uuid).Value = id;

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
                            case ClassKind.OwnedStyle:
                                return this.DiagramCanvasDao.Read(transaction, partition, new[] { topContainerIid }).FirstOrDefault();

                            default:
                                return null;
                        }
                    }
                }
            }

            return null;
        }
    }
}
