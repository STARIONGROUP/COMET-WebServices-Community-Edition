// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelDao.cs" company="RHEA System S.A.">
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
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.EngineeringModel"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.EngineeringModel> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"EngineeringModel_Cache\"", partition);

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
                                yield return thing as EngineeringModel;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"EngineeringModel_View\"", partition);

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

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, engineeringModel, container);
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
                sqlBuilder.AppendFormat(" VALUES (:iid, :engineeringModelSetup);");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModel.Iid;
                command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModel, "EngineeringModelSetup") ? engineeringModel.EngineeringModelSetup : Utils.NullableValue(null);
                sqlBuilder.AppendFormat(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"EngineeringModel\"", partition);
                sqlBuilder.AppendFormat(" SET (\"EngineeringModelSetup\")");
                sqlBuilder.AppendFormat(" = (:engineeringModelSetup);");

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

                    this.ExecuteAndLogCommand(command);
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
                
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActionItem\" SELECT * FROM \"{1}\".\"ActionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Approval\" SELECT * FROM \"{1}\".\"Approval\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Approval\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryNote\" SELECT * FROM \"{1}\".\"BinaryNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book\" SELECT * FROM \"{1}\".\"Book\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeProposal\" SELECT * FROM \"{1}\".\"ChangeProposal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeProposal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ChangeRequest\" SELECT * FROM \"{1}\".\"ChangeRequest\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ChangeRequest\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"CommonFileStore\" SELECT * FROM \"{1}\".\"CommonFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"CommonFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractChangeNotice\" SELECT * FROM \"{1}\".\"ContractChangeNotice\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractChangeNotice\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ContractDeviation\" SELECT * FROM \"{1}\".\"ContractDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ContractDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiscussionItem\" SELECT * FROM \"{1}\".\"DiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModel\" SELECT * FROM \"{1}\".\"EngineeringModel\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModel\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataAnnotation\" SELECT * FROM \"{1}\".\"EngineeringModelDataAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataDiscussionItem\" SELECT * FROM \"{1}\".\"EngineeringModelDataDiscussionItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataDiscussionItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataNote\" SELECT * FROM \"{1}\".\"EngineeringModelDataNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" SELECT * FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" SELECT * FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" SELECT * FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" SELECT * FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"GenericAnnotation\" SELECT * FROM \"{1}\".\"GenericAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"GenericAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\" SELECT * FROM \"{1}\".\"Iteration\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Iteration\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"LogEntryChangelogItem\" SELECT * FROM \"{1}\".\"LogEntryChangelogItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"LogEntryChangelogItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem\" SELECT * FROM \"{1}\".\"ModellingAnnotationItem\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingThingReference\" SELECT * FROM \"{1}\".\"ModellingThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry\" SELECT * FROM \"{1}\".\"ModelLogEntry\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note\" SELECT * FROM \"{1}\".\"Note\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page\" SELECT * FROM \"{1}\".\"Page\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForDeviation\" SELECT * FROM \"{1}\".\"RequestForDeviation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForDeviation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForWaiver\" SELECT * FROM \"{1}\".\"RequestForWaiver\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequestForWaiver\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReviewItemDiscrepancy\" SELECT * FROM \"{1}\".\"ReviewItemDiscrepancy\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReviewItemDiscrepancy\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section\" SELECT * FROM \"{1}\".\"Section\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Solution\" SELECT * FROM \"{1}\".\"Solution\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Solution\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TextualNote\" SELECT * FROM \"{1}\".\"TextualNote\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TextualNote\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" SELECT * FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ThingReference\" SELECT * FROM \"{1}\".\"ThingReference\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ThingReference\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"TopContainer\" SELECT * FROM \"{1}\".\"TopContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"TopContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Book_Category\" SELECT * FROM \"{1}\".\"Book_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Book_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" SELECT * FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" SELECT * FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_Category\" SELECT * FROM \"{1}\".\"ModellingAnnotationItem_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" SELECT * FROM \"{1}\".\"ModellingAnnotationItem_SourceAnnotation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModelLogEntry_Category\" SELECT * FROM \"{1}\".\"ModelLogEntry_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModelLogEntry_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Note_Category\" SELECT * FROM \"{1}\".\"Note_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Note_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Page_Category\" SELECT * FROM \"{1}\".\"Page_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Page_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Section_Category\" SELECT * FROM \"{1}\".\"Section_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Section_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" SELECT * FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" SELECT * FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
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

                this.ExecuteAndLogCommand(command);
            }
        }
    }
}
