// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestForWaiverDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated class. Any manual changes on this file will be overwritten!
// </summary>
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
    /// The RequestForWaiver Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class RequestForWaiverDao : ContractDeviationDao, IRequestForWaiverDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
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
        /// List of instances of <see cref="CDP4Common.DTO.RequestForWaiver"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.RequestForWaiver> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"RequestForWaiver_Cache\"", partition);

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
                                yield return thing as RequestForWaiver;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"RequestForWaiver_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.RequestForWaiver"/>.
        /// </returns>
        public virtual CDP4Common.DTO.RequestForWaiver MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempLanguageCode;
            string tempContent;
            string tempCreatedOn;
            string tempStatus;
            string tempTitle;
            string tempClassification;
            string tempShortName;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new CDP4Common.DTO.RequestForWaiver(iid, revisionNumber);
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.RelatedThing.AddRange(Array.ConvertAll((string[])reader["RelatedThing"], Guid.Parse));
            dto.Author = Guid.Parse(reader["Author"].ToString());
            dto.PrimaryAnnotatedThing = reader["PrimaryAnnotatedThing"] is DBNull ? (Guid?)null : Guid.Parse(reader["PrimaryAnnotatedThing"].ToString());
            dto.Discussion.AddRange(Array.ConvertAll((string[])reader["Discussion"], Guid.Parse));
            dto.ApprovedBy.AddRange(Array.ConvertAll((string[])reader["ApprovedBy"], Guid.Parse));
            dto.SourceAnnotation.AddRange(Array.ConvertAll((string[])reader["SourceAnnotation"], Guid.Parse));
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            dto.Category.AddRange(Array.ConvertAll((string[])reader["Category"], Guid.Parse));
            
            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }
            
            if (valueDict.TryGetValue("LanguageCode", out tempLanguageCode))
            {
                dto.LanguageCode = tempLanguageCode.UnEscape();
            }
            
            if (valueDict.TryGetValue("Content", out tempContent))
            {
                dto.Content = tempContent.UnEscape();
            }
            
            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }
            
            if (valueDict.TryGetValue("Status", out tempStatus))
            {
                dto.Status = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationStatusKind>(tempStatus);
            }
            
            if (valueDict.TryGetValue("Title", out tempTitle))
            {
                dto.Title = tempTitle.UnEscape();
            }
            
            if (valueDict.TryGetValue("Classification", out tempClassification))
            {
                dto.Classification = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationClassificationKind>(tempClassification);
            }
            
            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
            }
            
            return dto;
        }
 
        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="requestForWaiver">
        /// The requestForWaiver DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RequestForWaiver requestForWaiver, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, requestForWaiver, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, requestForWaiver, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForWaiver\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = requestForWaiver.Iid;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, requestForWaiver, container);
        }
 
        /// <summary>
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="requestForWaiver">
        /// The requestForWaiver DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RequestForWaiver requestForWaiver, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, requestForWaiver, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, requestForWaiver, container);
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, requestForWaiver, container);
        }
 
        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.RequestForWaiver"/> id that is to be deleted.
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
    }
}
