// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    /// The EngineeringModel Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class EngineeringModelDao : TopContainerDao, IEngineeringModelDao
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
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.EngineeringModel"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.EngineeringModel> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\", \"Actor\" FROM \"{0}\".\"EngineeringModel_Cache\"", partition);
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

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as EngineeringModel;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.Append(this.BuildReadQuery(partition, instant));

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    if (instant.HasValue && instant.Value != DateTime.MaxValue)
                    {
                        command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.EngineeringModel"/>.
        /// </returns>
        public virtual CDP4Common.DTO.EngineeringModel MapToDto(NpgsqlDataReader reader)
        {
            string tempLastModifiedOn;
            string tempModifiedOn;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.EngineeringModel(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Book.AddRange(Utils.ParseOrderedList<Guid>(reader["Book"] as string[,]));
            dto.CommonFileStore.AddRange(Array.ConvertAll((string[])reader["CommonFileStore"], Guid.Parse));
            dto.EngineeringModelSetup = Guid.Parse(reader["EngineeringModelSetup"].ToString());
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.GenericNote.AddRange(Array.ConvertAll((string[])reader["GenericNote"], Guid.Parse));
            dto.Iteration.AddRange(Array.ConvertAll((string[])reader["Iteration"], Guid.Parse));
            dto.LogEntry.AddRange(Array.ConvertAll((string[])reader["LogEntry"], Guid.Parse));
            dto.ModellingAnnotation.AddRange(Array.ConvertAll((string[])reader["ModellingAnnotation"], Guid.Parse));

            if (valueDict.TryGetValue("LastModifiedOn", out tempLastModifiedOn))
            {
                dto.LastModifiedOn = Utils.ParseUtcDate(tempLastModifiedOn);
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
        /// <param name="engineeringModel">
        /// The engineeringModel DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModel engineeringModel, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, engineeringModel, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, engineeringModel, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModel\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"EngineeringModelSetup\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :engineeringModelSetup);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModel.Iid;
                    command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModel, "EngineeringModelSetup") ? engineeringModel.EngineeringModelSetup : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, engineeringModel, container);
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
        /// <param name="engineeringModel">
        /// The engineeringModel DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModel engineeringModel, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, engineeringModel, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModel\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"EngineeringModelSetup\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :engineeringModelSetup)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModel.Iid;
                command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModel, "EngineeringModelSetup") ? engineeringModel.EngineeringModelSetup : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET \"EngineeringModelSetup\"");
                sqlBuilder.Append(" = :engineeringModelSetup;");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
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
        /// <param name="engineeringModel">
        /// The EngineeringModel DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModel engineeringModel, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, engineeringModel, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, engineeringModel, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModel\"", partition);
                    sqlBuilder.AppendFormat(" SET \"EngineeringModelSetup\"");
                    sqlBuilder.AppendFormat(" = :engineeringModelSetup");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModel.Iid;
                    command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModel, "EngineeringModelSetup") ? engineeringModel.EngineeringModelSetup : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, engineeringModel, container);
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
        /// The <see cref="CDP4Common.DTO.EngineeringModel"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.EngineeringModel"/> id that is to be deleted.
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
        /// Copy the tables from a source to a target Engineering-Model partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model
        /// </param>
        /// <param name="targetPartition">
        /// The target Engineering-Model
        /// </param>
        public void CopyEngineeringModel(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Actionee\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Actionee\" FROM \"{1}\".\"ActionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Approval\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\" FROM \"{1}\".\"Approval\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Approval\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FileType\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FileType\" FROM \"{1}\".\"BinaryNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Book\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeProposal\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeRequest\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeRequest\" FROM \"{1}\".\"ChangeProposal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeProposal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeRequest\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ChangeRequest\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeRequest\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"CommonFileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"CommonFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"CommonFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractChangeNotice\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeProposal\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeProposal\" FROM \"{1}\".\"ContractChangeNotice\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractChangeNotice\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractDeviation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ContractDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiscussionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\" FROM \"{1}\".\"DiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModel\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"EngineeringModelSetup\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"EngineeringModelSetup\" FROM \"{1}\".\"EngineeringModel\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModel\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataAnnotation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Author\", \"PrimaryAnnotatedThing\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Author\", \"PrimaryAnnotatedThing\" FROM \"{1}\".\"EngineeringModelDataAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataDiscussionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\" FROM \"{1}\".\"EngineeringModelDataDiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataDiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"EngineeringModelDataNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\" FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\" FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\" FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"GenericAnnotation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"GenericAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"GenericAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\" FROM \"{1}\".\"Iteration\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Iteration\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"LogEntryChangelogItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"LogEntryChangelogItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"ModellingAnnotationItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingThingReference\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ModellingThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\" FROM \"{1}\".\"ModelLogEntry\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Note\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Page\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForDeviation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequestForDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForWaiver\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequestForWaiver\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForWaiver\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReviewItemDiscrepancy\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ReviewItemDiscrepancy\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReviewItemDiscrepancy\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Section\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Solution\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\" FROM \"{1}\".\"Solution\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Solution\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TextualNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"TextualNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TextualNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ThingReference\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\" FROM \"{1}\".\"ThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TopContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"TopContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TopContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book_Category\" (\"Book\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Book\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Book_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" (\"File\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"File\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" (\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_Category\" (\"ModellingAnnotationItem\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ModellingAnnotationItem\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModellingAnnotationItem_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" (\"ModellingAnnotationItem\", \"SourceAnnotation\", \"ValidFrom\", \"ValidTo\") SELECT \"ModellingAnnotationItem\", \"SourceAnnotation\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModellingAnnotationItem_SourceAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry_Category\" (\"ModelLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ModelLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModelLogEntry_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note_Category\" (\"Note\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Note\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Note_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page_Category\" (\"Page\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Page\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Page_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section_Category\" (\"Section\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Section\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Section_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" (\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" (\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Copy the tables from a source to a target Engineering-Model partition and reset ITimeStampedThing.CreatedOn properties when needed
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model
        /// </param>
        /// <param name="targetPartition">
        /// The target Engineering-Model
        /// </param>
        public void CopyEngineeringModelAndResetCreatedOn(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var updateCreatedOnSql = ", \"ValueTypeDictionary\" = \"ValueTypeDictionary\" || concat('\"CreatedOn\"=>\"', \"SiteDirectory\".get_transaction_time(), '\"') :: hstore";
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Actionee\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Actionee\" FROM \"{1}\".\"ActionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Approval\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\" FROM \"{1}\".\"Approval\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Approval\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FileType\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FileType\" FROM \"{1}\".\"BinaryNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Book\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeProposal\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeRequest\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeRequest\" FROM \"{1}\".\"ChangeProposal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeProposal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeRequest\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ChangeRequest\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeRequest\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"CommonFileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"CommonFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"CommonFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractChangeNotice\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeProposal\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeProposal\" FROM \"{1}\".\"ContractChangeNotice\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractChangeNotice\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractDeviation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ContractDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiscussionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\" FROM \"{1}\".\"DiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModel\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"EngineeringModelSetup\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"EngineeringModelSetup\" FROM \"{1}\".\"EngineeringModel\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModel\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataAnnotation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Author\", \"PrimaryAnnotatedThing\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Author\", \"PrimaryAnnotatedThing\" FROM \"{1}\".\"EngineeringModelDataAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataDiscussionItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\" FROM \"{1}\".\"EngineeringModelDataDiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataDiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"EngineeringModelDataNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\" FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\" FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\" FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"GenericAnnotation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"GenericAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"GenericAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\" FROM \"{1}\".\"Iteration\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Iteration\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"LogEntryChangelogItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"LogEntryChangelogItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"ModellingAnnotationItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingThingReference\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ModellingThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\" FROM \"{1}\".\"ModelLogEntry\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Note\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Page\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForDeviation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequestForDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForWaiver\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequestForWaiver\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForWaiver\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReviewItemDiscrepancy\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ReviewItemDiscrepancy\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReviewItemDiscrepancy\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\" FROM \"{1}\".\"Section\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Solution\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\" FROM \"{1}\".\"Solution\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Solution\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TextualNote\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"TextualNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TextualNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ThingReference\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\" FROM \"{1}\".\"ThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TopContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"TopContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TopContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book_Category\" (\"Book\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Book\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Book_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" (\"File\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"File\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" (\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_Category\" (\"ModellingAnnotationItem\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ModellingAnnotationItem\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModellingAnnotationItem_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" (\"ModellingAnnotationItem\", \"SourceAnnotation\", \"ValidFrom\", \"ValidTo\") SELECT \"ModellingAnnotationItem\", \"SourceAnnotation\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModellingAnnotationItem_SourceAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry_Category\" (\"ModelLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ModelLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ModelLogEntry_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note_Category\" (\"Note\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Note\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Note_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page_Category\" (\"Page\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Page\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Page_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section_Category\" (\"Section\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Section\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Section_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" (\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" (\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Copy the tables from a source to an EngineeringModel partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        public void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            var triggerStatus = enable ? "ENABLE" : "DISABLE";
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActionItem\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Approval\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BinaryNote\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Book\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ChangeProposal\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ChangeRequest\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"CommonFileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ContractChangeNotice\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ContractDeviation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiscussionItem\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"EngineeringModel\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"EngineeringModelDataAnnotation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"EngineeringModelDataDiscussionItem\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"EngineeringModelDataNote\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Folder\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"GenericAnnotation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Iteration\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"LogEntryChangelogItem\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModellingAnnotationItem\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModellingThingReference\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModelLogEntry\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Note\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Page\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequestForDeviation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequestForWaiver\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ReviewItemDiscrepancy\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Section\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Solution\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"TextualNote\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ThingReference\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"TopContainer\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Book_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision_FileType\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModellingAnnotationItem_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ModelLogEntry_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Note_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Page_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Section_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedDomain\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedPerson\" {1} TRIGGER USER;", sourcePartition, triggerStatus);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="EngineeringModelDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition, DateTime? instant)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"EngineeringModel\".\"EngineeringModelSetup\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_Book\".\"Book\",'{}'::text[]) AS \"Book\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_CommonFileStore\".\"CommonFileStore\",'{}'::text[]) AS \"CommonFileStore\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_GenericNote\".\"GenericNote\",'{}'::text[]) AS \"GenericNote\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_Iteration\".\"Iteration\",'{}'::text[]) AS \"Iteration\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_LogEntry\".\"LogEntry\",'{}'::text[]) AS \"LogEntry\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModel_ModellingAnnotation\".\"ModellingAnnotation\",'{}'::text[]) AS \"ModellingAnnotation\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"TopContainer\" USING (\"Iid\")", this.GetTopContainerDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" USING (\"Iid\")", this.GetEngineeringModelDataSql(partition, instant));

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedDomain\"", this.GetThing_ExcludedDomainDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedPerson\"", this.GetThing_ExcludedPersonDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Book\".\"Container\" AS \"Iid\", ARRAY[array_agg(\"Book\".\"Sequence\"::text), array_agg(\"Book\".\"Iid\"::text)] AS \"Book\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Book\"", this.GetBookDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"Book\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Book\".\"Container\") AS \"EngineeringModel_Book\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"CommonFileStore\".\"Container\" AS \"Iid\", array_agg(\"CommonFileStore\".\"Iid\"::text) AS \"CommonFileStore\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"CommonFileStore\"", this.GetCommonFileStoreDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"CommonFileStore\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"CommonFileStore\".\"Container\") AS \"EngineeringModel_CommonFileStore\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelDataNote\".\"Container\" AS \"Iid\", array_agg(\"EngineeringModelDataNote\".\"Iid\"::text) AS \"GenericNote\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"EngineeringModelDataNote\"", this.GetEngineeringModelDataNoteDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"EngineeringModelDataNote\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"EngineeringModelDataNote\".\"Container\") AS \"EngineeringModel_GenericNote\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Iteration\".\"Container\" AS \"Iid\", array_agg(\"Iteration\".\"Iid\"::text) AS \"Iteration\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Iteration\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"Iteration\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Iteration\".\"Container\") AS \"EngineeringModel_Iteration\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModelLogEntry\".\"Container\" AS \"Iid\", array_agg(\"ModelLogEntry\".\"Iid\"::text) AS \"LogEntry\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModelLogEntry\"", this.GetModelLogEntryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"ModelLogEntry\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModelLogEntry\".\"Container\") AS \"EngineeringModel_LogEntry\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem\".\"Container\" AS \"Iid\", array_agg(\"ModellingAnnotationItem\".\"Iid\"::text) AS \"ModellingAnnotation\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModellingAnnotationItem\"", this.GetModellingAnnotationItemDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModel\" ON \"ModellingAnnotationItem\".\"Container\" = \"EngineeringModel\".\"Iid\"", this.GetEngineeringModelDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem\".\"Container\") AS \"EngineeringModel_ModellingAnnotation\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModel_Audit\".\"Actor\", \"EngineeringModel_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModel_Audit\" AS \"EngineeringModel_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"EngineeringModel_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"EngineeringModel_Audit\".\"Iid\", \"EngineeringModel_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"TopContainer\".\"ValueTypeDictionary\" || \"EngineeringModel\".\"ValueTypeDictionary\"";

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThingDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetTopContainerDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"TopContainer\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"TopContainer_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetEngineeringModelDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"EngineeringModelSetup\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModel\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModel_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThing_ExcludedDomainDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"Thing\",\"ExcludedDomain\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThing_ExcludedPersonDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"Thing\",\"ExcludedPerson\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetBookDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Sequence\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Book\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Book_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetCommonFileStoreDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"CommonFileStore\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"CommonFileStore_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetEngineeringModelDataNoteDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataNote\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataNote_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetIterationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Iteration\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Iteration_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetModelLogEntryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Author\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModelLogEntry\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModelLogEntry_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetModellingAnnotationItemDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
