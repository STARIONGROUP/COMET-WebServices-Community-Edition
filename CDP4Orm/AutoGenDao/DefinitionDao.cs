// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinitionDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The Definition Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class DefinitionDao : ThingDao, IDefinitionDao
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
        /// List of instances of <see cref="CDP4Common.DTO.Definition"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.Definition> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"Definition_Cache\"", partition);

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
                                yield return thing as Definition;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"Definition_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.Definition"/>.
        /// </returns>
        public virtual CDP4Common.DTO.Definition MapToDto(NpgsqlDataReader reader)
        {
            string tempContent;
            string tempLanguageCode;
            string tempModifiedOn;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.Definition(iid, revisionNumber);
            dto.Citation.AddRange(Array.ConvertAll((string[])reader["Citation"], Guid.Parse));
            dto.Example.AddRange(Utils.ParseOrderedList<string>(reader["Example"] as string[,]));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.Note.AddRange(Utils.ParseOrderedList<string>(reader["Note"] as string[,]));

            if (valueDict.TryGetValue("Content", out tempContent))
            {
                dto.Content = tempContent.UnEscape();
            }

            if (valueDict.TryGetValue("LanguageCode", out tempLanguageCode))
            {
                dto.LanguageCode = tempLanguageCode.UnEscape();
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
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
        /// <param name="definition">
        /// The definition DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Definition definition, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, definition, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, definition, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Content", !this.IsDerived(definition, "Content") ? definition.Content.Escape() : string.Empty },
                    { "LanguageCode", !this.IsDerived(definition, "LanguageCode") ? definition.LanguageCode.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = definition.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }

                definition.Example.ForEach(x => this.AddExample(transaction, partition, definition.Iid, x));

                definition.Note.ForEach(x => this.AddNote(transaction, partition, definition.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, definition, container);
        }

        /// <summary>
        /// Insert a new database record, or updates one if it already exists from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="definition">
        /// The definition DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Definition definition, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, definition, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "Content", !this.IsDerived(definition, "Content") ? definition.Content.Escape() : string.Empty },
                { "LanguageCode", !this.IsDerived(definition, "LanguageCode") ? definition.LanguageCode.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = definition.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                sqlBuilder.AppendFormat(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"Definition\"", partition);
                sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            definition.Example.ForEach(x => this.UpsertExample(transaction, partition, definition.Iid, x));

            definition.Note.ForEach(x => this.UpsertNote(transaction, partition, definition.Iid, x));

            return true;
        }

        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public override bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isCreated = base.AddToCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Example":
                    {
                        isCreated = this.AddExample(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "Note":
                    {
                        isCreated = this.AddNote(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isCreated;
        }

        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="example">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddExample(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem example)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition_Example\"", partition);
                sqlBuilder.AppendFormat(" (\"Definition\", \"Example\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:definition, :example, :sequence);");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("example", NpgsqlDbType.Text).Value = (string)example.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = example.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table, or update an existing one if it already exists.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="example">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertExample(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem example)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition_Example\"", partition);
                sqlBuilder.AppendFormat(" (\"Definition\", \"Example\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:definition, :example, :sequence);");
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"Definition_Example\"", partition);
                sqlBuilder.AppendFormat(" SET (\"Definition\", \"Example\", \"Sequence\")");
                sqlBuilder.Append(" = (:definition, :example, :sequence));");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("example", NpgsqlDbType.Text).Value = (string)example.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = example.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="note">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddNote(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem note)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition_Note\"", partition);
                sqlBuilder.AppendFormat(" (\"Definition\", \"Note\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:definition, :note, :sequence);");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("note", NpgsqlDbType.Text).Value = (string)note.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = note.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table, or update an existing one if it already exists.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="note">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertNote(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem note)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition_Note\"", partition);
                sqlBuilder.AppendFormat(" (\"Definition\", \"Note\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:definition, :note, :sequence);");
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"Definition_Note\"", partition);
                sqlBuilder.AppendFormat(" SET (\"Definition\", \"Note\", \"Sequence\")");
                sqlBuilder.Append(" = (:definition, :note, :sequence));");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("note", NpgsqlDbType.Text).Value = (string)note.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = note.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// <param name="definition">
        /// The Definition DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Definition definition, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, definition, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, definition, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Content", !this.IsDerived(definition, "Content") ? definition.Content.Escape() : string.Empty },
                    { "LanguageCode", !this.IsDerived(definition, "LanguageCode") ? definition.LanguageCode.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = definition.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, definition, container);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that is the source for the reordered link table record.
        /// </param> 
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public override bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            var isReordered = base.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
 
            switch (propertyName)
            {
                case "Example":
                    {
                        isReordered = this.ReorderExample(transaction, partition, iid, orderUpdate);
                        break;
                    }

                case "Note":
                    {
                        isReordered = this.ReorderNote(transaction, partition, iid, orderUpdate);
                        break;
                    }

                default:
                {
                    break;
                }
            }

            return isReordered;
      }

        /// <summary>
        /// Reorder an item in an association link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for reordered link table record.
        /// </param> 
        /// <param name="example">
        /// The value for which a link table record wil be reordered.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public bool ReorderExample(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem example)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition_Example\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.Append(" = :reorderSequence");
                sqlBuilder.Append(" WHERE \"Definition\" = :definition");
                sqlBuilder.Append(" AND \"Example\" = :example");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("example", NpgsqlDbType.Text).Value = (string)example.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = example.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = example.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Reorder an item in an association link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that will be the source for reordered link table record.
        /// </param> 
        /// <param name="note">
        /// The value for which a link table record wil be reordered.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public bool ReorderNote(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem note)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition_Note\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.Append(" = :reorderSequence");
                sqlBuilder.Append(" WHERE \"Definition\" = :definition");
                sqlBuilder.Append(" AND \"Note\" = :note");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("note", NpgsqlDbType.Text).Value = (string)note.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = note.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = note.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
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
        /// The <see cref="CDP4Common.DTO.Definition"/> id that is to be deleted.
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
        /// Delete the supplied value from the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property name from where the value is to be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that is the source of each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be removed.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public override bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isDeleted = base.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Example":
                    {
                        isDeleted = this.DeleteExample(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "Note":
                    {
                        isDeleted = this.DeleteNote(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                default:
                {
                    break;
                }
            }

            return isDeleted;
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="example">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteExample(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem example)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Definition_Example\"", partition);
                sqlBuilder.Append(" WHERE \"Definition\" = :definition");
                sqlBuilder.Append(" AND \"Example\" = :example");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("example", NpgsqlDbType.Text).Value = (string)example.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = example.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Definition"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="note">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteNote(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem note)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Definition_Note\"", partition);
                sqlBuilder.Append(" WHERE \"Definition\" = :definition");
                sqlBuilder.Append(" AND \"Note\" = :note");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("definition", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("note", NpgsqlDbType.Text).Value = (string)note.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = note.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
    }
}
