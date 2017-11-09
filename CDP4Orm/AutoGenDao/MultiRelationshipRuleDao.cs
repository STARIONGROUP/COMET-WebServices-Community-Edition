// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiRelationshipRuleDao.cs" company="RHEA System S.A.">
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
    /// The MultiRelationshipRule Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class MultiRelationshipRuleDao : RuleDao, IMultiRelationshipRuleDao
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
        /// List of instances of <see cref="CDP4Common.DTO.MultiRelationshipRule"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.MultiRelationshipRule> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"MultiRelationshipRule_Cache\"", partition);

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
                                yield return thing as MultiRelationshipRule;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"MultiRelationshipRule_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.MultiRelationshipRule"/>.
        /// </returns>
        public virtual CDP4Common.DTO.MultiRelationshipRule MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempIsDeprecated;
            string tempMinRelated;
            string tempMaxRelated;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new CDP4Common.DTO.MultiRelationshipRule(iid, revisionNumber);
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.RelationshipCategory = Guid.Parse(reader["RelationshipCategory"].ToString());
            dto.RelatedCategory.AddRange(Array.ConvertAll((string[])reader["RelatedCategory"], Guid.Parse));
            
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
            
            if (valueDict.TryGetValue("IsDeprecated", out tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }
            
            if (valueDict.TryGetValue("MinRelated", out tempMinRelated))
            {
                dto.MinRelated = int.Parse(tempMinRelated);
            }
            
            if (valueDict.TryGetValue("MaxRelated", out tempMaxRelated))
            {
                dto.MaxRelated = int.Parse(tempMaxRelated);
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
        /// <param name="multiRelationshipRule">
        /// The multiRelationshipRule DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.MultiRelationshipRule multiRelationshipRule, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, multiRelationshipRule, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, multiRelationshipRule, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "MinRelated", !this.IsDerived(multiRelationshipRule, "MinRelated") ? multiRelationshipRule.MinRelated.ToString() : string.Empty },
                            { "MaxRelated", !this.IsDerived(multiRelationshipRule, "MaxRelated") ? multiRelationshipRule.MaxRelated.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationshipRule\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"RelationshipCategory\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :relationshipCategory);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = multiRelationshipRule.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("relationshipCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(multiRelationshipRule, "RelationshipCategory") ? multiRelationshipRule.RelationshipCategory : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
                
                multiRelationshipRule.RelatedCategory.ForEach(x => this.AddRelatedCategory(transaction, partition, multiRelationshipRule.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, multiRelationshipRule, container);
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
        /// The <see cref="CDP4Common.DTO.MultiRelationshipRule"/> id that will be the source for each link table record.
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
                case "RelatedCategory":
                    {
                        isCreated = this.AddRelatedCategory(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.MultiRelationshipRule"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="relatedCategory">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddRelatedCategory(NpgsqlTransaction transaction, string partition, Guid iid, Guid relatedCategory)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
            
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationshipRule_RelatedCategory\"", partition);
                sqlBuilder.AppendFormat(" (\"MultiRelationshipRule\", \"RelatedCategory\")");
                sqlBuilder.Append(" VALUES (:multiRelationshipRule, :relatedCategory);");
                command.Parameters.Add("multiRelationshipRule", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("relatedCategory", NpgsqlDbType.Uuid).Value = relatedCategory;
            
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
        /// <param name="multiRelationshipRule">
        /// The multiRelationshipRule DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.MultiRelationshipRule multiRelationshipRule, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, multiRelationshipRule, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, multiRelationshipRule, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "MinRelated", !this.IsDerived(multiRelationshipRule, "MinRelated") ? multiRelationshipRule.MinRelated.ToString() : string.Empty },
                            { "MaxRelated", !this.IsDerived(multiRelationshipRule, "MaxRelated") ? multiRelationshipRule.MaxRelated.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationshipRule\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"RelationshipCategory\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:relationshipCategory, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = multiRelationshipRule.Iid;
                    command.Parameters.Add("relationshipCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(multiRelationshipRule, "RelationshipCategory") ? multiRelationshipRule.RelationshipCategory : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, multiRelationshipRule, container);
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
        /// The <see cref="CDP4Common.DTO.MultiRelationshipRule"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.MultiRelationshipRule"/> id that is the source of each link table record.
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
                case "RelatedCategory":
                    {
                        isDeleted = this.DeleteRelatedCategory(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.MultiRelationshipRule"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="relatedCategory">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteRelatedCategory(NpgsqlTransaction transaction, string partition, Guid iid, Guid relatedCategory)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
            
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"MultiRelationshipRule_RelatedCategory\"", partition);
                sqlBuilder.Append(" WHERE \"MultiRelationshipRule\" = :multiRelationshipRule");
                sqlBuilder.Append(" AND \"RelatedCategory\" = :relatedCategory;");
                command.Parameters.Add("multiRelationshipRule", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("relatedCategory", NpgsqlDbType.Uuid).Value = relatedCategory;
            
                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
    }
}
