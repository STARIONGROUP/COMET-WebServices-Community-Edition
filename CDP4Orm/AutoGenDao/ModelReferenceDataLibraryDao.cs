// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryDao.cs" company="RHEA System S.A.">
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
    /// The ModelReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ModelReferenceDataLibraryDao : ReferenceDataLibraryDao, IModelReferenceDataLibraryDao
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
        /// List of instances of <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.ModelReferenceDataLibrary> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"ModelReferenceDataLibrary_Cache\"", partition);
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
                                yield return thing as ModelReferenceDataLibrary;
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/>.
        /// </returns>
        public virtual CDP4Common.DTO.ModelReferenceDataLibrary MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.ModelReferenceDataLibrary(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.BaseQuantityKind.AddRange(Utils.ParseOrderedList<Guid>(reader["BaseQuantityKind"] as string[,]));
            dto.BaseUnit.AddRange(Array.ConvertAll((string[])reader["BaseUnit"], Guid.Parse));
            dto.Constant.AddRange(Array.ConvertAll((string[])reader["Constant"], Guid.Parse));
            dto.DefinedCategory.AddRange(Array.ConvertAll((string[])reader["DefinedCategory"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.FileType.AddRange(Array.ConvertAll((string[])reader["FileType"], Guid.Parse));
            dto.Glossary.AddRange(Array.ConvertAll((string[])reader["Glossary"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.ParameterType.AddRange(Array.ConvertAll((string[])reader["ParameterType"], Guid.Parse));
            dto.ReferenceSource.AddRange(Array.ConvertAll((string[])reader["ReferenceSource"], Guid.Parse));
            dto.RequiredRdl = reader["RequiredRdl"] is DBNull ? (Guid?)null : Guid.Parse(reader["RequiredRdl"].ToString());
            dto.Rule.AddRange(Array.ConvertAll((string[])reader["Rule"], Guid.Parse));
            dto.Scale.AddRange(Array.ConvertAll((string[])reader["Scale"], Guid.Parse));
            dto.Unit.AddRange(Array.ConvertAll((string[])reader["Unit"], Guid.Parse));
            dto.UnitPrefix.AddRange(Array.ConvertAll((string[])reader["UnitPrefix"], Guid.Parse));

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("Name", out tempName))
            {
                dto.Name = tempName.UnEscape();
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
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
        /// <param name="modelReferenceDataLibrary">
        /// The modelReferenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModelReferenceDataLibrary modelReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, modelReferenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, modelReferenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modelReferenceDataLibrary.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, modelReferenceDataLibrary, container);
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
        /// <param name="modelReferenceDataLibrary">
        /// The modelReferenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModelReferenceDataLibrary modelReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, modelReferenceDataLibrary, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelReferenceDataLibrary\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modelReferenceDataLibrary.Iid;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET \"Container\"");
                sqlBuilder.Append(" = :container;");

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
        /// <param name="modelReferenceDataLibrary">
        /// The ModelReferenceDataLibrary DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModelReferenceDataLibrary modelReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, modelReferenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, modelReferenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" SET \"Container\"");
                    sqlBuilder.AppendFormat(" = :container");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modelReferenceDataLibrary.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, modelReferenceDataLibrary, container);
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
        /// The <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/> id that is to be deleted.
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
        /// Build a SQL read query for the current <see cref="ModelReferenceDataLibraryDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"ModelReferenceDataLibrary\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"ReferenceDataLibrary\".\"RequiredRdl\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseQuantityKind\".\"BaseQuantityKind\",'{}'::text[]) AS \"BaseQuantityKind\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseUnit\".\"BaseUnit\",'{}'::text[]) AS \"BaseUnit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Constant\".\"Constant\",'{}'::text[]) AS \"Constant\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_DefinedCategory\".\"DefinedCategory\",'{}'::text[]) AS \"DefinedCategory\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_FileType\".\"FileType\",'{}'::text[]) AS \"FileType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Glossary\".\"Glossary\",'{}'::text[]) AS \"Glossary\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ParameterType\".\"ParameterType\",'{}'::text[]) AS \"ParameterType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ReferenceSource\".\"ReferenceSource\",'{}'::text[]) AS \"ReferenceSource\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Rule\".\"Rule\",'{}'::text[]) AS \"Rule\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Scale\".\"Scale\",'{}'::text[]) AS \"Scale\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Unit\".\"Unit\",'{}'::text[]) AS \"Unit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_UnitPrefix\".\"UnitPrefix\",'{}'::text[]) AS \"UnitPrefix\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ModelReferenceDataLibrary_Data\"() AS \"ModelReferenceDataLibrary\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Alias\".\"Container\" AS \"Iid\", array_agg(\"Alias\".\"Iid\"::text) AS \"Alias\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Alias_Data\"() AS \"Alias\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"Alias\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Alias\".\"Container\") AS \"DefinedThing_Alias\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Definition\".\"Container\" AS \"Iid\", array_agg(\"Definition\".\"Iid\"::text) AS \"Definition\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Definition_Data\"() AS \"Definition\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"Definition\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Definition\".\"Container\") AS \"DefinedThing_Definition\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"HyperLink\".\"Container\" AS \"Iid\", array_agg(\"HyperLink\".\"Iid\"::text) AS \"HyperLink\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"HyperLink_Data\"() AS \"HyperLink\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"HyperLink\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"HyperLink\".\"Container\") AS \"DefinedThing_HyperLink\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", ARRAY[array_agg(\"Sequence\"::text), array_agg(\"BaseQuantityKind\"::text)] AS \"BaseQuantityKind\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind_Data\"() AS \"ReferenceDataLibrary_BaseQuantityKind\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseQuantityKind\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", array_agg(\"BaseUnit\"::text) AS \"BaseUnit\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit_Data\"() AS \"ReferenceDataLibrary_BaseUnit\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseUnit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Constant\".\"Container\" AS \"Iid\", array_agg(\"Constant\".\"Iid\"::text) AS \"Constant\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Constant_Data\"() AS \"Constant\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Constant\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Constant\".\"Container\") AS \"ReferenceDataLibrary_Constant\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Category\".\"Container\" AS \"Iid\", array_agg(\"Category\".\"Iid\"::text) AS \"DefinedCategory\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Category_Data\"() AS \"Category\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Category\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Category\".\"Container\") AS \"ReferenceDataLibrary_DefinedCategory\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"FileType\".\"Container\" AS \"Iid\", array_agg(\"FileType\".\"Iid\"::text) AS \"FileType\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"FileType_Data\"() AS \"FileType\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"FileType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"FileType\".\"Container\") AS \"ReferenceDataLibrary_FileType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Glossary\".\"Container\" AS \"Iid\", array_agg(\"Glossary\".\"Iid\"::text) AS \"Glossary\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Glossary_Data\"() AS \"Glossary\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Glossary\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Glossary\".\"Container\") AS \"ReferenceDataLibrary_Glossary\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ParameterType\".\"Container\" AS \"Iid\", array_agg(\"ParameterType\".\"Iid\"::text) AS \"ParameterType\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ParameterType_Data\"() AS \"ParameterType\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ParameterType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ParameterType\".\"Container\") AS \"ReferenceDataLibrary_ParameterType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceSource\".\"Container\" AS \"Iid\", array_agg(\"ReferenceSource\".\"Iid\"::text) AS \"ReferenceSource\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceSource_Data\"() AS \"ReferenceSource\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceSource\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceSource\".\"Container\") AS \"ReferenceDataLibrary_ReferenceSource\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Rule\".\"Container\" AS \"Iid\", array_agg(\"Rule\".\"Iid\"::text) AS \"Rule\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Rule_Data\"() AS \"Rule\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Rule\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Rule\".\"Container\") AS \"ReferenceDataLibrary_Rule\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementScale\".\"Container\" AS \"Iid\", array_agg(\"MeasurementScale\".\"Iid\"::text) AS \"Scale\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementScale_Data\"() AS \"MeasurementScale\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"MeasurementScale\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"MeasurementScale\".\"Container\") AS \"ReferenceDataLibrary_Scale\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementUnit\".\"Container\" AS \"Iid\", array_agg(\"MeasurementUnit\".\"Iid\"::text) AS \"Unit\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementUnit_Data\"() AS \"MeasurementUnit\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"MeasurementUnit\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"MeasurementUnit\".\"Container\") AS \"ReferenceDataLibrary_Unit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"UnitPrefix\".\"Container\" AS \"Iid\", array_agg(\"UnitPrefix\".\"Iid\"::text) AS \"UnitPrefix\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"UnitPrefix_Data\"() AS \"UnitPrefix\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"UnitPrefix\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"UnitPrefix\".\"Container\") AS \"ReferenceDataLibrary_UnitPrefix\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModelReferenceDataLibrary_Audit\".\"Actor\", \"ModelReferenceDataLibrary_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModelReferenceDataLibrary_Audit\" AS \"ModelReferenceDataLibrary_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"ModelReferenceDataLibrary_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"ModelReferenceDataLibrary_Audit\".\"Iid\", \"ModelReferenceDataLibrary_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"ReferenceDataLibrary\".\"ValueTypeDictionary\" || \"ModelReferenceDataLibrary\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
