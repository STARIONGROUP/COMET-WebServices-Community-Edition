// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalIdentifierMapDao.cs" company="RHEA System S.A.">
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
    /// The ExternalIdentifierMap Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ExternalIdentifierMapDao : ThingDao, IExternalIdentifierMapDao
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
        /// List of instances of <see cref="CDP4Common.DTO.ExternalIdentifierMap"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.ExternalIdentifierMap> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"ExternalIdentifierMap_Cache\"", partition);

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
                                yield return thing as ExternalIdentifierMap;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"ExternalIdentifierMap_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.ExternalIdentifierMap"/>.
        /// </returns>
        public virtual CDP4Common.DTO.ExternalIdentifierMap MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempExternalModelName;
            string tempExternalToolName;
            string tempExternalToolVersion;
            string tempName;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new CDP4Common.DTO.ExternalIdentifierMap(iid, revisionNumber);
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.Correspondence.AddRange(Array.ConvertAll((string[])reader["Correspondence"], Guid.Parse));
            dto.ExternalFormat = reader["ExternalFormat"] is DBNull ? (Guid?)null : Guid.Parse(reader["ExternalFormat"].ToString());
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            
            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }
            
            if (valueDict.TryGetValue("ExternalModelName", out tempExternalModelName))
            {
                dto.ExternalModelName = tempExternalModelName.UnEscape();
            }
            
            if (valueDict.TryGetValue("ExternalToolName", out tempExternalToolName))
            {
                dto.ExternalToolName = tempExternalToolName.UnEscape();
            }
            
            if (valueDict.TryGetValue("ExternalToolVersion", out tempExternalToolVersion) && tempExternalToolVersion != null)
            {
                dto.ExternalToolVersion = tempExternalToolVersion.UnEscape();
            }
            
            if (valueDict.TryGetValue("Name", out tempName))
            {
                dto.Name = tempName.UnEscape();
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
        /// <param name="externalIdentifierMap">
        /// The externalIdentifierMap DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ExternalIdentifierMap externalIdentifierMap, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, externalIdentifierMap, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, externalIdentifierMap, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "ExternalModelName", !this.IsDerived(externalIdentifierMap, "ExternalModelName") ? externalIdentifierMap.ExternalModelName.Escape() : string.Empty },
                            { "ExternalToolName", !this.IsDerived(externalIdentifierMap, "ExternalToolName") ? externalIdentifierMap.ExternalToolName.Escape() : string.Empty },
                            { "ExternalToolVersion", !this.IsDerived(externalIdentifierMap, "ExternalToolVersion") ? externalIdentifierMap.ExternalToolVersion.Escape() : null },
                            { "Name", !this.IsDerived(externalIdentifierMap, "Name") ? externalIdentifierMap.Name.Escape() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExternalIdentifierMap\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"ExternalFormat\", \"Owner\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :externalFormat, :owner);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = externalIdentifierMap.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("externalFormat", NpgsqlDbType.Uuid).Value = !this.IsDerived(externalIdentifierMap, "ExternalFormat") ? Utils.NullableValue(externalIdentifierMap.ExternalFormat) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(externalIdentifierMap, "Owner") ? externalIdentifierMap.Owner : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, externalIdentifierMap, container);
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
        /// <param name="externalIdentifierMap">
        /// The externalIdentifierMap DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ExternalIdentifierMap externalIdentifierMap, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, externalIdentifierMap, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, externalIdentifierMap, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "ExternalModelName", !this.IsDerived(externalIdentifierMap, "ExternalModelName") ? externalIdentifierMap.ExternalModelName.Escape() : string.Empty },
                            { "ExternalToolName", !this.IsDerived(externalIdentifierMap, "ExternalToolName") ? externalIdentifierMap.ExternalToolName.Escape() : string.Empty },
                            { "ExternalToolVersion", !this.IsDerived(externalIdentifierMap, "ExternalToolVersion") ? externalIdentifierMap.ExternalToolVersion.Escape() : null },
                            { "Name", !this.IsDerived(externalIdentifierMap, "Name") ? externalIdentifierMap.Name.Escape() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExternalIdentifierMap\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"ExternalFormat\", \"Owner\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:container, :externalFormat, :owner, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = externalIdentifierMap.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("externalFormat", NpgsqlDbType.Uuid).Value = !this.IsDerived(externalIdentifierMap, "ExternalFormat") ? Utils.NullableValue(externalIdentifierMap.ExternalFormat) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(externalIdentifierMap, "Owner") ? externalIdentifierMap.Owner : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, externalIdentifierMap, container);
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
        /// The <see cref="CDP4Common.DTO.ExternalIdentifierMap"/> id that is to be deleted.
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
