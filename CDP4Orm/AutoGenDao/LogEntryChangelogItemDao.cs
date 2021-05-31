// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryChangelogItemDao.cs" company="RHEA System S.A.">
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
    /// The LogEntryChangelogItem Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class LogEntryChangelogItemDao : ThingDao, ILogEntryChangelogItemDao
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
        /// List of instances of <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.LogEntryChangelogItem> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"LogEntryChangelogItem_Cache\"", partition);

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
                                yield return thing as LogEntryChangelogItem;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"LogEntryChangelogItem_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.LogEntryChangelogItem"/>.
        /// </returns>
        public virtual CDP4Common.DTO.LogEntryChangelogItem MapToDto(NpgsqlDataReader reader)
        {
            string tempAffectedItemIid;
            string tempChangeDescription;
            string tempChangelogKind;
            string tempModifiedOn;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.LogEntryChangelogItem(iid, revisionNumber);
            dto.AffectedReferenceIid.AddRange(Array.ConvertAll((string[])reader["AffectedReferenceIid"], Guid.Parse));
            
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));

            if (valueDict.TryGetValue("AffectedItemIid", out tempAffectedItemIid))
            {
                dto.AffectedItemIid = Guid.Parse(tempAffectedItemIid);
            }

            if (valueDict.TryGetValue("ChangeDescription", out tempChangeDescription) && tempChangeDescription != null)
            {
                dto.ChangeDescription = tempChangeDescription.UnEscape();
            }

            if (valueDict.TryGetValue("ChangelogKind", out tempChangelogKind))
            {
                dto.ChangelogKind = Utils.ParseEnum<CDP4Common.CommonData.LogEntryChangelogItemKind>(tempChangelogKind);
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
        /// <param name="logEntryChangelogItem">
        /// The logEntryChangelogItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.LogEntryChangelogItem logEntryChangelogItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, logEntryChangelogItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, logEntryChangelogItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "AffectedItemIid", !this.IsDerived(logEntryChangelogItem, "AffectedItemIid") ? logEntryChangelogItem.AffectedItemIid.ToString() : string.Empty },
                    { "ChangeDescription", !this.IsDerived(logEntryChangelogItem, "ChangeDescription") ? logEntryChangelogItem.ChangeDescription.Escape() : null },
                    { "ChangelogKind", !this.IsDerived(logEntryChangelogItem, "ChangelogKind") ? logEntryChangelogItem.ChangelogKind.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = logEntryChangelogItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }

                logEntryChangelogItem.AffectedReferenceIid.ForEach(x => this.AddAffectedReferenceIid(transaction, partition, logEntryChangelogItem.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, logEntryChangelogItem, container);
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
        /// <param name="logEntryChangelogItem">
        /// The logEntryChangelogItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.LogEntryChangelogItem logEntryChangelogItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, logEntryChangelogItem, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "AffectedItemIid", !this.IsDerived(logEntryChangelogItem, "AffectedItemIid") ? logEntryChangelogItem.AffectedItemIid.ToString() : string.Empty },
                { "ChangeDescription", !this.IsDerived(logEntryChangelogItem, "ChangeDescription") ? logEntryChangelogItem.ChangeDescription.Escape() : null },
                { "ChangelogKind", !this.IsDerived(logEntryChangelogItem, "ChangelogKind") ? logEntryChangelogItem.ChangelogKind.ToString() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = logEntryChangelogItem.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                sqlBuilder.AppendFormat(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"LogEntryChangelogItem\"", partition);
                sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            logEntryChangelogItem.AffectedReferenceIid.ForEach(x => this.UpsertAffectedReferenceIid(transaction, partition, logEntryChangelogItem.Iid, x));

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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that will be the source for each link table record.
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
                case "AffectedReferenceIid":
                    {
                        isCreated = this.AddAffectedReferenceIid(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="affectedReferenceIid">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddAffectedReferenceIid(NpgsqlTransaction transaction, string partition, Guid iid, Guid affectedReferenceIid)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem_AffectedReferenceIid\"", partition);
                sqlBuilder.AppendFormat(" (\"LogEntryChangelogItem\", \"AffectedReferenceIid\")");
                sqlBuilder.Append(" VALUES (:logEntryChangelogItem, :affectedReferenceIid);");

                command.Parameters.Add("logEntryChangelogItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("affectedReferenceIid", NpgsqlDbType.Uuid).Value = affectedReferenceIid;

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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="affectedReferenceIid">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertAffectedReferenceIid(NpgsqlTransaction transaction, string partition, Guid iid, Guid affectedReferenceIid)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem_AffectedReferenceIid\"", partition);
                sqlBuilder.AppendFormat(" (\"LogEntryChangelogItem\", \"AffectedReferenceIid\")");
                sqlBuilder.Append(" VALUES (:logEntryChangelogItem, :affectedReferenceIid)");

                command.Parameters.Add("logEntryChangelogItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("affectedReferenceIid", NpgsqlDbType.Uuid).Value = affectedReferenceIid;

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
        /// <param name="logEntryChangelogItem">
        /// The LogEntryChangelogItem DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.LogEntryChangelogItem logEntryChangelogItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, logEntryChangelogItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, logEntryChangelogItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "AffectedItemIid", !this.IsDerived(logEntryChangelogItem, "AffectedItemIid") ? logEntryChangelogItem.AffectedItemIid.ToString() : string.Empty },
                    { "ChangeDescription", !this.IsDerived(logEntryChangelogItem, "ChangeDescription") ? logEntryChangelogItem.ChangeDescription.Escape() : null },
                    { "ChangelogKind", !this.IsDerived(logEntryChangelogItem, "ChangelogKind") ? logEntryChangelogItem.ChangelogKind.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"LogEntryChangelogItem\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = logEntryChangelogItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, logEntryChangelogItem, container);
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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that is the source of each link table record.
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
                case "AffectedReferenceIid":
                    {
                        isDeleted = this.DeleteAffectedReferenceIid(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="affectedReferenceIid">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteAffectedReferenceIid(NpgsqlTransaction transaction, string partition, Guid iid, Guid affectedReferenceIid)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"LogEntryChangelogItem_AffectedReferenceIid\"", partition);
                sqlBuilder.Append(" WHERE \"LogEntryChangelogItem\" = :logEntryChangelogItem");
                sqlBuilder.Append(" AND \"AffectedReferenceIid\" = :affectedReferenceIid;");

                command.Parameters.Add("logEntryChangelogItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("affectedReferenceIid", NpgsqlDbType.Uuid).Value = affectedReferenceIid;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
    }
}
