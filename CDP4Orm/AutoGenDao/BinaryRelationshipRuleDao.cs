// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryRelationshipRuleDao.cs" company="RHEA System S.A.">
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
    /// The BinaryRelationshipRule Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class BinaryRelationshipRuleDao : RuleDao, IBinaryRelationshipRuleDao
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
        /// List of instances of <see cref="CDP4Common.DTO.BinaryRelationshipRule"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.BinaryRelationshipRule> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"BinaryRelationshipRule_Cache\"", partition);

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
                                yield return thing as BinaryRelationshipRule;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"BinaryRelationshipRule_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.BinaryRelationshipRule"/>.
        /// </returns>
        public virtual CDP4Common.DTO.BinaryRelationshipRule MapToDto(NpgsqlDataReader reader)
        {
            string tempForwardRelationshipName;
            string tempInverseRelationshipName;
            string tempIsDeprecated;
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.BinaryRelationshipRule(iid, revisionNumber);
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.RelationshipCategory = Guid.Parse(reader["RelationshipCategory"].ToString());
            dto.SourceCategory = Guid.Parse(reader["SourceCategory"].ToString());
            dto.TargetCategory = Guid.Parse(reader["TargetCategory"].ToString());

            if (valueDict.TryGetValue("ForwardRelationshipName", out tempForwardRelationshipName))
            {
                dto.ForwardRelationshipName = tempForwardRelationshipName.UnEscape();
            }

            if (valueDict.TryGetValue("InverseRelationshipName", out tempInverseRelationshipName))
            {
                dto.InverseRelationshipName = tempInverseRelationshipName.UnEscape();
            }

            if (valueDict.TryGetValue("IsDeprecated", out tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }

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
        /// <param name="binaryRelationshipRule">
        /// The binaryRelationshipRule DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.BinaryRelationshipRule binaryRelationshipRule, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, binaryRelationshipRule, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, binaryRelationshipRule, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "ForwardRelationshipName", !this.IsDerived(binaryRelationshipRule, "ForwardRelationshipName") ? binaryRelationshipRule.ForwardRelationshipName.Escape() : string.Empty },
                    { "InverseRelationshipName", !this.IsDerived(binaryRelationshipRule, "InverseRelationshipName") ? binaryRelationshipRule.InverseRelationshipName.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryRelationshipRule\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"RelationshipCategory\", \"SourceCategory\", \"TargetCategory\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :relationshipCategory, :sourceCategory, :targetCategory);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = binaryRelationshipRule.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("relationshipCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "RelationshipCategory") ? binaryRelationshipRule.RelationshipCategory : Utils.NullableValue(null);
                    command.Parameters.Add("sourceCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "SourceCategory") ? binaryRelationshipRule.SourceCategory : Utils.NullableValue(null);
                    command.Parameters.Add("targetCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "TargetCategory") ? binaryRelationshipRule.TargetCategory : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, binaryRelationshipRule, container);
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
        /// <param name="binaryRelationshipRule">
        /// The binaryRelationshipRule DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.BinaryRelationshipRule binaryRelationshipRule, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, binaryRelationshipRule, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "ForwardRelationshipName", !this.IsDerived(binaryRelationshipRule, "ForwardRelationshipName") ? binaryRelationshipRule.ForwardRelationshipName.Escape() : string.Empty },
                { "InverseRelationshipName", !this.IsDerived(binaryRelationshipRule, "InverseRelationshipName") ? binaryRelationshipRule.InverseRelationshipName.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryRelationshipRule\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"RelationshipCategory\", \"SourceCategory\", \"TargetCategory\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :relationshipCategory, :sourceCategory, :targetCategory)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = binaryRelationshipRule.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("relationshipCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "RelationshipCategory") ? binaryRelationshipRule.RelationshipCategory : Utils.NullableValue(null);
                command.Parameters.Add("sourceCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "SourceCategory") ? binaryRelationshipRule.SourceCategory : Utils.NullableValue(null);
                command.Parameters.Add("targetCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "TargetCategory") ? binaryRelationshipRule.TargetCategory : Utils.NullableValue(null);
                sqlBuilder.AppendFormat(" ON CONFLICT (\"Iid\")");
                sqlBuilder.AppendFormat(" DO UPDATE \"{0}\".\"BinaryRelationshipRule\"", partition);
                sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"RelationshipCategory\", \"SourceCategory\", \"TargetCategory\")");
                sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :relationshipCategory, :sourceCategory, :targetCategory);");

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
        /// <param name="binaryRelationshipRule">
        /// The BinaryRelationshipRule DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.BinaryRelationshipRule binaryRelationshipRule, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, binaryRelationshipRule, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, binaryRelationshipRule, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "ForwardRelationshipName", !this.IsDerived(binaryRelationshipRule, "ForwardRelationshipName") ? binaryRelationshipRule.ForwardRelationshipName.Escape() : string.Empty },
                    { "InverseRelationshipName", !this.IsDerived(binaryRelationshipRule, "InverseRelationshipName") ? binaryRelationshipRule.InverseRelationshipName.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryRelationshipRule\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"RelationshipCategory\", \"SourceCategory\", \"TargetCategory\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :relationshipCategory, :sourceCategory, :targetCategory)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = binaryRelationshipRule.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("relationshipCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "RelationshipCategory") ? binaryRelationshipRule.RelationshipCategory : Utils.NullableValue(null);
                    command.Parameters.Add("sourceCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "SourceCategory") ? binaryRelationshipRule.SourceCategory : Utils.NullableValue(null);
                    command.Parameters.Add("targetCategory", NpgsqlDbType.Uuid).Value = !this.IsDerived(binaryRelationshipRule, "TargetCategory") ? binaryRelationshipRule.TargetCategory : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, binaryRelationshipRule, container);
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
        /// The <see cref="CDP4Common.DTO.BinaryRelationshipRule"/> id that is to be deleted.
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
