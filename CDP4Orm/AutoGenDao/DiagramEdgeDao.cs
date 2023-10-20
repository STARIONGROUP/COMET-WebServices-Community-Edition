// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramEdgeDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The DiagramEdge Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class DiagramEdgeDao : DiagramElementThingDao, IDiagramEdgeDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="isCachedDtoReadEnabledAndInstant">
        /// The value indicating whether to get cached last state of Dto from revision history.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.DiagramEdge"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.DiagramEdge> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"DiagramEdge_Cache\"", partition);
                    sqlBuilder.Append(this.BuildJoinForActorProperty(partition));

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as DiagramEdge;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.Append(this.BuildReadQuery(partition));

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return this.MapToDto(reader);
                        }
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
        /// <returns>
        /// A deserialized instance of <see cref="CDP4Common.DTO.DiagramEdge"/>.
        /// </returns>
        public virtual CDP4Common.DTO.DiagramEdge MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.DiagramEdge(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Bounds.AddRange(Array.ConvertAll((string[])reader["Bounds"], Guid.Parse));
            dto.DepictedThing = reader["DepictedThing"] is DBNull ? (Guid?)null : Guid.Parse(reader["DepictedThing"].ToString());
            dto.DiagramElement.AddRange(Array.ConvertAll((string[])reader["DiagramElement"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.LocalStyle.AddRange(Array.ConvertAll((string[])reader["LocalStyle"], Guid.Parse));
            dto.Point.AddRange(Utils.ParseOrderedList<Guid>(reader["Point"] as string[,]));
            dto.SharedStyle = reader["SharedStyle"] is DBNull ? (Guid?)null : Guid.Parse(reader["SharedStyle"].ToString());
            dto.Source = Guid.Parse(reader["Source"].ToString());
            dto.Target = Guid.Parse(reader["Target"].ToString());

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("Name", out tempName))
            {
                dto.Name = tempName.UnEscape();
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
            }

            return dto;
        }

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="diagramEdge">
        /// The diagramEdge DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramEdge diagramEdge, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, diagramEdge, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, diagramEdge, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramEdge\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Source\", \"Target\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :source, :target);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramEdge.Iid;
                    command.Parameters.Add("source", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Source") ? diagramEdge.Source : Utils.NullableValue(null);
                    command.Parameters.Add("target", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Target") ? diagramEdge.Target : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, diagramEdge, container);
        }

        /// <summary>
        /// Insert a new database record, or updates one if it already exists from the supplied data transfer object.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="diagramEdge">
        /// The diagramEdge DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramEdge diagramEdge, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, diagramEdge, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramEdge\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Source\", \"Target\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :source, :target)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramEdge.Iid;
                command.Parameters.Add("source", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Source") ? diagramEdge.Source : Utils.NullableValue(null);
                command.Parameters.Add("target", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Target") ? diagramEdge.Target : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"Source\", \"Target\")");
                sqlBuilder.Append(" = (:source, :target);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            return true;
        }

        /// <summary>
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="diagramEdge">
        /// The DiagramEdge DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramEdge diagramEdge, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, diagramEdge, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, diagramEdge, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramEdge\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Source\", \"Target\")");
                    sqlBuilder.AppendFormat(" = (:source, :target)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramEdge.Iid;
                    command.Parameters.Add("source", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Source") ? diagramEdge.Source : Utils.NullableValue(null);
                    command.Parameters.Add("target", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramEdge, "Target") ? diagramEdge.Target : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, diagramEdge, container);
        }

        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.DiagramEdge"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            bool isHandled;
            var beforeDelete = this.BeforeDelete(transaction, partition, iid, out isHandled);
            if (!isHandled)
            {
                beforeDelete = beforeDelete && base.Delete(transaction, partition, iid);
            }

            return this.AfterDelete(beforeDelete, transaction, partition, iid);
        }

        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// A "Raw" Delete means that the delete is performed without calling BeforeDelete or AfterDelete.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.DiagramEdge"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool RawDelete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = base.Delete(transaction, partition, iid);
            return result;
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="DiagramEdgeDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"DiagramElementThing\".\"DepictedThing\",");

            sqlBuilder.Append(" \"DiagramElementThing\".\"SharedStyle\",");

            sqlBuilder.Append(" \"DiagramEdge\".\"Source\",");

            sqlBuilder.Append(" \"DiagramEdge\".\"Target\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementContainer_Bounds\".\"Bounds\",'{}'::text[]) AS \"Bounds\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementContainer_DiagramElement\".\"DiagramElement\",'{}'::text[]) AS \"DiagramElement\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementThing_LocalStyle\".\"LocalStyle\",'{}'::text[]) AS \"LocalStyle\",");
            sqlBuilder.Append(" COALESCE(\"DiagramEdge_Point\".\"Point\",'{}'::text[]) AS \"Point\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramThingBase_Data\"() AS \"DiagramThingBase\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramEdge_Data\"() AS \"DiagramEdge\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Bounds\".\"Container\" AS \"Iid\", array_agg(\"Bounds\".\"Iid\"::text) AS \"Bounds\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Bounds_Data\"() AS \"Bounds\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" ON \"Bounds\".\"Container\" = \"DiagramElementContainer\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Bounds\".\"Container\") AS \"DiagramElementContainer_Bounds\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DiagramElementThing\".\"Container\" AS \"Iid\", array_agg(\"DiagramElementThing\".\"Iid\"::text) AS \"DiagramElement\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" ON \"DiagramElementThing\".\"Container\" = \"DiagramElementContainer\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"DiagramElementThing\".\"Container\") AS \"DiagramElementContainer_DiagramElement\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"OwnedStyle\".\"Container\" AS \"Iid\", array_agg(\"OwnedStyle\".\"Iid\"::text) AS \"LocalStyle\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"OwnedStyle_Data\"() AS \"OwnedStyle\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\" ON \"OwnedStyle\".\"Container\" = \"DiagramElementThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"OwnedStyle\".\"Container\") AS \"DiagramElementThing_LocalStyle\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Point\".\"Container\" AS \"Iid\", ARRAY[array_agg(\"Point\".\"Sequence\"::text), array_agg(\"Point\".\"Iid\"::text)] AS \"Point\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Point_Data\"() AS \"Point\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramEdge_Data\"() AS \"DiagramEdge\" ON \"Point\".\"Container\" = \"DiagramEdge\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Point\".\"Container\") AS \"DiagramEdge_Point\" USING (\"Iid\")");

            sqlBuilder.Append(this.BuildJoinForActorProperty(partition));
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Build a SQL LEFT JOIN to retrieve the Actor column
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL LEFT JOIN</returns>
        public override string BuildJoinForActorProperty(string partition)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" LEFT JOIN (SELECT \"DiagramEdge_Audit\".\"Actor\", \"DiagramEdge_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramEdge_Audit\" AS \"DiagramEdge_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"DiagramEdge_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"DiagramEdge_Audit\".\"Iid\", \"DiagramEdge_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DiagramThingBase\".\"ValueTypeDictionary\" || \"DiagramElementContainer\".\"ValueTypeDictionary\" || \"DiagramElementThing\".\"ValueTypeDictionary\" || \"DiagramEdge\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
