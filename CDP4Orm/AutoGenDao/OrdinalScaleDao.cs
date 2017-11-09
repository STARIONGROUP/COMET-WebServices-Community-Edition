// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrdinalScaleDao.cs" company="RHEA System S.A.">
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
    /// The OrdinalScale Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class OrdinalScaleDao : MeasurementScaleDao, IOrdinalScaleDao
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
        /// List of instances of <see cref="CDP4Common.DTO.OrdinalScale"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.OrdinalScale> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"OrdinalScale_Cache\"", partition);

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
                                yield return thing as OrdinalScale;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"OrdinalScale_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.OrdinalScale"/>.
        /// </returns>
        public virtual CDP4Common.DTO.OrdinalScale MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempNumberSet;
            string tempMinimumPermissibleValue;
            string tempIsMinimumInclusive;
            string tempMaximumPermissibleValue;
            string tempIsMaximumInclusive;
            string tempPositiveValueConnotation;
            string tempNegativeValueConnotation;
            string tempIsDeprecated;
            string tempUseShortNameValues;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new CDP4Common.DTO.OrdinalScale(iid, revisionNumber);
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.Unit = Guid.Parse(reader["Unit"].ToString());
            dto.ValueDefinition.AddRange(Array.ConvertAll((string[])reader["ValueDefinition"], Guid.Parse));
            dto.MappingToReferenceScale.AddRange(Array.ConvertAll((string[])reader["MappingToReferenceScale"], Guid.Parse));
            
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
            
            if (valueDict.TryGetValue("NumberSet", out tempNumberSet))
            {
                dto.NumberSet = Utils.ParseEnum<CDP4Common.SiteDirectoryData.NumberSetKind>(tempNumberSet);
            }
            
            if (valueDict.TryGetValue("MinimumPermissibleValue", out tempMinimumPermissibleValue) && tempMinimumPermissibleValue != null)
            {
                dto.MinimumPermissibleValue = tempMinimumPermissibleValue.UnEscape();
            }
            
            if (valueDict.TryGetValue("IsMinimumInclusive", out tempIsMinimumInclusive))
            {
                dto.IsMinimumInclusive = bool.Parse(tempIsMinimumInclusive);
            }
            
            if (valueDict.TryGetValue("MaximumPermissibleValue", out tempMaximumPermissibleValue) && tempMaximumPermissibleValue != null)
            {
                dto.MaximumPermissibleValue = tempMaximumPermissibleValue.UnEscape();
            }
            
            if (valueDict.TryGetValue("IsMaximumInclusive", out tempIsMaximumInclusive))
            {
                dto.IsMaximumInclusive = bool.Parse(tempIsMaximumInclusive);
            }
            
            if (valueDict.TryGetValue("PositiveValueConnotation", out tempPositiveValueConnotation) && tempPositiveValueConnotation != null)
            {
                dto.PositiveValueConnotation = tempPositiveValueConnotation.UnEscape();
            }
            
            if (valueDict.TryGetValue("NegativeValueConnotation", out tempNegativeValueConnotation) && tempNegativeValueConnotation != null)
            {
                dto.NegativeValueConnotation = tempNegativeValueConnotation.UnEscape();
            }
            
            if (valueDict.TryGetValue("IsDeprecated", out tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }
            
            if (valueDict.TryGetValue("UseShortNameValues", out tempUseShortNameValues))
            {
                dto.UseShortNameValues = bool.Parse(tempUseShortNameValues);
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
        /// <param name="ordinalScale">
        /// The ordinalScale DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.OrdinalScale ordinalScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, ordinalScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, ordinalScale, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "UseShortNameValues", !this.IsDerived(ordinalScale, "UseShortNameValues") ? ordinalScale.UseShortNameValues.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrdinalScale\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = ordinalScale.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, ordinalScale, container);
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
        /// <param name="ordinalScale">
        /// The ordinalScale DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.OrdinalScale ordinalScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, ordinalScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, ordinalScale, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "UseShortNameValues", !this.IsDerived(ordinalScale, "UseShortNameValues") ? ordinalScale.UseShortNameValues.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrdinalScale\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (\"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = ordinalScale.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, ordinalScale, container);
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
        /// The <see cref="CDP4Common.DTO.OrdinalScale"/> id that is to be deleted.
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
