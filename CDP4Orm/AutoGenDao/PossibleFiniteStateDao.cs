// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateDao.cs" company="RHEA System S.A.">
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
    /// The PossibleFiniteState Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class PossibleFiniteStateDao : DefinedThingDao, IPossibleFiniteStateDao
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
        /// List of instances of <see cref="CDP4Common.DTO.PossibleFiniteState"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.PossibleFiniteState> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"PossibleFiniteState_Cache\"", partition);
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
                                yield return thing as PossibleFiniteState;
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.PossibleFiniteState"/>.
        /// </returns>
        public virtual CDP4Common.DTO.PossibleFiniteState MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.PossibleFiniteState(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));

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
        /// <param name="possibleFiniteState">
        /// The possibleFiniteState DTO that is to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.PossibleFiniteState possibleFiniteState, long sequence, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, possibleFiniteState, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, possibleFiniteState, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteState\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Sequence\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :sequence, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = possibleFiniteState.Iid;
                    command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = sequence;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, possibleFiniteState, container);
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
        /// <param name="possibleFiniteState">
        /// The possibleFiniteState DTO that is to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.PossibleFiniteState possibleFiniteState, long sequence, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, possibleFiniteState, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteState\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Sequence\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :sequence, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = possibleFiniteState.Iid;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = sequence;
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
        /// <param name="possibleFiniteState">
        /// The PossibleFiniteState DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.PossibleFiniteState possibleFiniteState, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, possibleFiniteState, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, possibleFiniteState, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteState\"", partition);
                    sqlBuilder.AppendFormat(" SET \"Container\"");
                    sqlBuilder.AppendFormat(" = :container");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = possibleFiniteState.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, possibleFiniteState, container);
        }

        /// <summary>
        /// Update the containment order as indicated by the supplied orderedItem.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="orderedItem">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the contained item was successfully reordered.
        /// </returns>
        public override bool ReorderContainment(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            var isReordered = base.ReorderContainment(transaction, partition, orderedItem);
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteState\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.AppendFormat(" = :reorderSequence");
                sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid");
                sqlBuilder.AppendFormat(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = Guid.Parse(orderedItem.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = orderedItem.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = orderedItem.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return isReordered && this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.PossibleFiniteState"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.PossibleFiniteState"/> id that is to be deleted.
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
        /// Build a SQL read query for the current <see cref="PossibleFiniteStateDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"PossibleFiniteState\".\"Container\",");

            sqlBuilder.Append(" \"PossibleFiniteState\".\"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"PossibleFiniteState_Data\"() AS \"PossibleFiniteState\" USING (\"Iid\")", partition);

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"PossibleFiniteState_Audit\".\"Actor\", \"PossibleFiniteState_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"PossibleFiniteState_Audit\" AS \"PossibleFiniteState_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"PossibleFiniteState_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"PossibleFiniteState_Audit\".\"Iid\", \"PossibleFiniteState_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"PossibleFiniteState\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
