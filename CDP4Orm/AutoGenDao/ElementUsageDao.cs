// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementUsageDao.cs" company="RHEA System S.A.">
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
    /// The ElementUsage Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ElementUsageDao : ElementBaseDao, IElementUsageDao
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
        /// List of instances of <see cref="CDP4Common.DTO.ElementUsage"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.ElementUsage> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"ElementUsage_Cache\"", partition);

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
                                yield return thing as ElementUsage;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"ElementUsage_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.ElementUsage"/>.
        /// </returns>
        public virtual CDP4Common.DTO.ElementUsage MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempInterfaceEnd;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new CDP4Common.DTO.ElementUsage(iid, revisionNumber);
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.Category.AddRange(Array.ConvertAll((string[])reader["Category"], Guid.Parse));
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            dto.ElementDefinition = Guid.Parse(reader["ElementDefinition"].ToString());
            dto.ParameterOverride.AddRange(Array.ConvertAll((string[])reader["ParameterOverride"], Guid.Parse));
            dto.ExcludeOption.AddRange(Array.ConvertAll((string[])reader["ExcludeOption"], Guid.Parse));
            
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
            
            if (valueDict.TryGetValue("InterfaceEnd", out tempInterfaceEnd))
            {
                dto.InterfaceEnd = Utils.ParseEnum<CDP4Common.EngineeringModelData.InterfaceEndKind>(tempInterfaceEnd);
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
        /// <param name="elementUsage">
        /// The elementUsage DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ElementUsage elementUsage, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, elementUsage, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, elementUsage, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "InterfaceEnd", !this.IsDerived(elementUsage, "InterfaceEnd") ? elementUsage.InterfaceEnd.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"ElementDefinition\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :elementDefinition);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = elementUsage.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("elementDefinition", NpgsqlDbType.Uuid).Value = !this.IsDerived(elementUsage, "ElementDefinition") ? elementUsage.ElementDefinition : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
                
                elementUsage.ExcludeOption.ForEach(x => this.AddExcludeOption(transaction, partition, elementUsage.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, elementUsage, container);
        }
 
        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ElementUsage"/> id that will be the source for each link table record.
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
                case "ExcludeOption":
                    {
                        isCreated = this.AddExcludeOption(transaction, partition, iid, (Guid)value);
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
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ElementUsage"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="excludeOption">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddExcludeOption(NpgsqlTransaction transaction, string partition, Guid iid, Guid excludeOption)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
            
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage_ExcludeOption\"", partition);
                sqlBuilder.AppendFormat(" (\"ElementUsage\", \"ExcludeOption\")");
                sqlBuilder.Append(" VALUES (:elementUsage, :excludeOption);");
                command.Parameters.Add("elementUsage", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("excludeOption", NpgsqlDbType.Uuid).Value = excludeOption;
            
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
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="elementUsage">
        /// The elementUsage DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ElementUsage elementUsage, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, elementUsage, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, elementUsage, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "InterfaceEnd", !this.IsDerived(elementUsage, "InterfaceEnd") ? elementUsage.InterfaceEnd.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"ElementDefinition\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:container, :elementDefinition, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = elementUsage.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("elementDefinition", NpgsqlDbType.Uuid).Value = !this.IsDerived(elementUsage, "ElementDefinition") ? elementUsage.ElementDefinition : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, elementUsage, container);
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
        /// The <see cref="CDP4Common.DTO.ElementUsage"/> id that is to be deleted.
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
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property name from where the value is to be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ElementUsage"/> id that is the source of each link table record.
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
                case "ExcludeOption":
                    {
                        isDeleted = this.DeleteExcludeOption(transaction, partition, iid, (Guid)value);
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
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ElementUsage"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="excludeOption">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteExcludeOption(NpgsqlTransaction transaction, string partition, Guid iid, Guid excludeOption)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
            
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ElementUsage_ExcludeOption\"", partition);
                sqlBuilder.Append(" WHERE \"ElementUsage\" = :elementUsage");
                sqlBuilder.Append(" AND \"ExcludeOption\" = :excludeOption;");
                command.Parameters.Add("elementUsage", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("excludeOption", NpgsqlDbType.Uuid).Value = excludeOption;
            
                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
    }
}
