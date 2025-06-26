// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The Iteration Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class IterationDao : ThingDao, IIterationDao
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="CDP4Common.DTO.Iteration"/> as result.
        /// </returns>
        public virtual async Task<IEnumerable<CDP4Common.DTO.Iteration>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var result = new List<Iteration>();

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\", \"Actor\" FROM \"{0}\".\"Iteration_Cache\"", partition);
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

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                result.Add(thing as Iteration);
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

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(this.MapToDto(reader));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="CDP4Common.DTO.Iteration"/>.
        /// </returns>
        public virtual CDP4Common.DTO.Iteration MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempSourceIterationIid;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.Iteration(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.ActualFiniteStateList.AddRange(Array.ConvertAll((string[])reader["ActualFiniteStateList"], Guid.Parse));
            dto.DefaultOption = reader["DefaultOption"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultOption"].ToString());
            dto.DiagramCanvas.AddRange(Array.ConvertAll((string[])reader["DiagramCanvas"], Guid.Parse));
            dto.DomainFileStore.AddRange(Array.ConvertAll((string[])reader["DomainFileStore"], Guid.Parse));
            dto.Element.AddRange(Array.ConvertAll((string[])reader["Element"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExternalIdentifierMap.AddRange(Array.ConvertAll((string[])reader["ExternalIdentifierMap"], Guid.Parse));
            dto.Goal.AddRange(Array.ConvertAll((string[])reader["Goal"], Guid.Parse));
            dto.IterationSetup = Guid.Parse(reader["IterationSetup"].ToString());
            dto.Option.AddRange(Utils.ParseOrderedList<Guid>(reader["Option"] as string[,]));
            dto.PossibleFiniteStateList.AddRange(Array.ConvertAll((string[])reader["PossibleFiniteStateList"], Guid.Parse));
            dto.Publication.AddRange(Array.ConvertAll((string[])reader["Publication"], Guid.Parse));
            dto.Relationship.AddRange(Array.ConvertAll((string[])reader["Relationship"], Guid.Parse));
            dto.RequirementsSpecification.AddRange(Array.ConvertAll((string[])reader["RequirementsSpecification"], Guid.Parse));
            dto.RuleVerificationList.AddRange(Array.ConvertAll((string[])reader["RuleVerificationList"], Guid.Parse));
            dto.SharedDiagramStyle.AddRange(Array.ConvertAll((string[])reader["SharedDiagramStyle"], Guid.Parse));
            dto.Stakeholder.AddRange(Array.ConvertAll((string[])reader["Stakeholder"], Guid.Parse));
            dto.StakeholderValue.AddRange(Array.ConvertAll((string[])reader["StakeholderValue"], Guid.Parse));
            dto.StakeholderValueMap.AddRange(Array.ConvertAll((string[])reader["StakeholderValueMap"], Guid.Parse));
            dto.TopElement = reader["TopElement"] is DBNull ? (Guid?)null : Guid.Parse(reader["TopElement"].ToString());
            dto.ValueGroup.AddRange(Array.ConvertAll((string[])reader["ValueGroup"], Guid.Parse));

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("SourceIterationIid", out tempSourceIterationIid) && tempSourceIterationIid != null)
            {
                dto.SourceIterationIid = Guid.Parse(tempSourceIterationIid);
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
        /// <param name="iteration">
        /// The iteration DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWriteResult = await this.BeforeWriteAsync(transaction, partition, iteration, container, valueTypeDictionaryAdditions);

            var beforeWrite = beforeWriteResult.Value;
            var isHandled = beforeWriteResult.IsHandled;

            if (!isHandled)
            {
                beforeWrite = beforeWrite && await base.WriteAsync(transaction, partition, iteration, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                    command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                    command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterWriteAsync(beforeWrite, transaction, partition, iteration, container);
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
        /// <param name="iteration">
        /// The iteration DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            await base.UpsertAsync(transaction, partition, iteration, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                await command.ExecuteNonQueryAsync();
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
        /// <param name="iteration">
        /// The Iteration DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully updated as result.
        /// </returns>
        public virtual async Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdateResult = await this.BeforeUpdateAsync(transaction, partition, iteration, container, valueTypeDictionaryAdditions);

            var beforeUpdate = beforeUpdateResult.Value;
            var isHandled = beforeUpdateResult.IsHandled;

            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && await base.UpdateAsync(transaction, partition, iteration, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Iteration\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                    command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                    command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterUpdateAsync(beforeUpdate, transaction, partition, iteration, container);
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
        /// The <see cref="CDP4Common.DTO.Iteration"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully deleted as result.
        /// </returns>
        public override async Task<bool> DeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var beforeDeleteResult = await this.BeforeDeleteAsync(transaction, partition, iid);

            var beforeDelete = beforeDeleteResult.Value;
            var isHandled = beforeDeleteResult.IsHandled;

            if (!isHandled)
            {
                beforeDelete = beforeDelete && await base.DeleteAsync(transaction, partition, iid);
            }

            return await this.AfterDeleteAsync(beforeDelete, transaction, partition, iid);
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
        /// The <see cref="CDP4Common.DTO.Iteration"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully deleted as result.
        /// </returns>
        public override async Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = await base.DeleteAsync(transaction, partition, iid);
            return result;
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task CopyIterationAsync(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ActualFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"ActualFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Alias\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Alias\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Alias\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"AndExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryRelationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\" FROM \"{1}\".\"BinaryRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BooleanExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"BooleanExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BooleanExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Bounds\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Bounds\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Bounds\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BuiltInRuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"BuiltInRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BuiltInRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Citation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\" FROM \"{1}\".\"Citation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Citation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Color\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Color\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Color\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DefinedThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DefinedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DefinedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Definition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramCanvas\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"DiagramCanvas\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramCanvas\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramEdge\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\" FROM \"{1}\".\"DiagramEdge\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramEdge\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramElementContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DepictedThing\", \"SharedStyle\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DepictedThing\", \"SharedStyle\" FROM \"{1}\".\"DiagramElementThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagrammingStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FillColor\", \"FontColor\", \"StrokeColor\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FillColor\", \"FontColor\", \"StrokeColor\" FROM \"{1}\".\"DiagrammingStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagrammingStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramObject\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramObject\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramObject\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramShape\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramShape\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramShape\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramThingBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramThingBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramThingBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DomainFileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"DomainFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DomainFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"ElementBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ElementDefinition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ElementDefinition\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ElementDefinition\" FROM \"{1}\".\"ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ExclusiveOrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExternalIdentifierMap\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ExternalFormat\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ExternalFormat\", \"Owner\" FROM \"{1}\".\"ExternalIdentifierMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExternalIdentifierMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\" FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\" FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\" FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"HyperLink\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"HyperLink\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"HyperLink\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"IdCorrespondence\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"IdCorrespondence\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"IdCorrespondence\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"MultiRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RootElement\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RootElement\" FROM \"{1}\".\"NestedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedParameter\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ActualState\", \"AssociatedParameter\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ActualState\", \"AssociatedParameter\", \"Owner\" FROM \"{1}\".\"NestedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NotExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Term\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Term\" FROM \"{1}\".\"NotExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NotExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"Option\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"OrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OwnedStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"OwnedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OwnedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Parameter\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RequestedBy\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RequestedBy\" FROM \"{1}\".\"Parameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Parameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\" FROM \"{1}\".\"ParameterBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingGroup\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingGroup\" FROM \"{1}\".\"ParameterGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOrOverrideBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ParameterOrOverrideBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOrOverrideBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverride\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Parameter\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Parameter\" FROM \"{1}\".\"ParameterOverride\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverride\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverrideValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterValueSet\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterValueSet\" FROM \"{1}\".\"ParameterOverrideValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverrideValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscription\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ParameterSubscription\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscription\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscriptionValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SubscribedValueSet\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SubscribedValueSet\" FROM \"{1}\".\"ParameterSubscriptionValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscriptionValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"ParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ParameterValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ActualOption\", \"ActualState\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ActualOption\", \"ActualState\" FROM \"{1}\".\"ParameterValueSetBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSetBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParametricConstraint\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"TopExpression\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"TopExpression\", \"Sequence\" FROM \"{1}\".\"ParametricConstraint\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParametricConstraint\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Point\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"Point\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Point\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteState\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"PossibleFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultState\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultState\", \"Owner\" FROM \"{1}\".\"PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Publication\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationalExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"RelationalExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationalExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"Relationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationshipParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RelationshipParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationshipParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Group\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Group\" FROM \"{1}\".\"Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"RequirementsContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainerParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsContainerParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainerParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsSpecification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsSpecification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsSpecification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"RuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerificationList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"RuleVerificationList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerificationList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleViolation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RuleViolation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleViolation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SharedStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"SharedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SharedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterizableThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"SimpleParameterizableThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterizableThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"SimpleParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Stakeholder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"StakeHolderValueMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMapSettings\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"GoalToValueGroupRelationship\", \"StakeholderValueToRequirementRelationship\", \"ValueGroupToStakeholderValueRelationship\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"GoalToValueGroupRelationship\", \"StakeholderValueToRequirementRelationship\", \"ValueGroupToStakeholderValueRelationship\" FROM \"{1}\".\"StakeHolderValueMapSettings\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMapSettings\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"UserRuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Rule\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Rule\" FROM \"{1}\".\"UserRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"UserRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState_PossibleState\" (\"ActualFiniteState\", \"PossibleState\", \"ValidFrom\", \"ValidTo\") SELECT \"ActualFiniteState\", \"PossibleState\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ActualFiniteState_PossibleState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState_PossibleState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_ExcludeOption\" (\"ActualFiniteStateList\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\") SELECT \"ActualFiniteStateList\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ActualFiniteStateList_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" (\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"ActualFiniteStateList_PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression_Term\" (\"AndExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"AndExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"AndExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase_Category\" (\"ElementBase\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementBase\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementBase_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_OrganizationalParticipant\" (\"ElementDefinition\", \"OrganizationalParticipant\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementDefinition\", \"OrganizationalParticipant\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementDefinition_OrganizationalParticipant\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_OrganizationalParticipant\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_ReferencedElement\" (\"ElementDefinition\", \"ReferencedElement\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementDefinition\", \"ReferencedElement\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementDefinition_ReferencedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_ReferencedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage_ExcludeOption\" (\"ElementUsage\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementUsage\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementUsage_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression_Term\" (\"ExclusiveOrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"ExclusiveOrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ExclusiveOrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" (\"File\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"File\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" (\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal_Category\" (\"Goal\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Goal\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Goal_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship_RelatedThing\" (\"MultiRelationship\", \"RelatedThing\", \"ValidFrom\", \"ValidTo\") SELECT \"MultiRelationship\", \"RelatedThing\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"MultiRelationship_RelatedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship_RelatedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement_ElementUsage\" (\"NestedElement\", \"ElementUsage\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"NestedElement\", \"ElementUsage\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"NestedElement_ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement_ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option_Category\" (\"Option\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Option\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Option_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression_Term\" (\"OrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"OrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"OrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList_Category\" (\"PossibleFiniteStateList\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"PossibleFiniteStateList\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"PossibleFiniteStateList_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_Domain\" (\"Publication\", \"Domain\", \"ValidFrom\", \"ValidTo\") SELECT \"Publication\", \"Domain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Publication_Domain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_Domain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_PublishedParameter\" (\"Publication\", \"PublishedParameter\", \"ValidFrom\", \"ValidTo\") SELECT \"Publication\", \"PublishedParameter\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Publication_PublishedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_PublishedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship_Category\" (\"Relationship\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Relationship\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Relationship_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement_Category\" (\"Requirement\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Requirement\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Requirement_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer_Category\" (\"RequirementsContainer\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"RequirementsContainer\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequirementsContainer_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_Category\" (\"Stakeholder\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Stakeholder\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Stakeholder_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_StakeholderValue\" (\"Stakeholder\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\") SELECT \"Stakeholder\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Stakeholder_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue_Category\" (\"StakeholderValue\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeholderValue\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeholderValue_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Category\" (\"StakeHolderValueMap\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Goal\" (\"StakeHolderValueMap\", \"Goal\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Goal\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Requirement\" (\"StakeHolderValueMap\", \"Requirement\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Requirement\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_StakeholderValue\" (\"StakeHolderValueMap\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_ValueGroup\" (\"StakeHolderValueMap\", \"ValueGroup\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"ValueGroup\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" (\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" (\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup_Category\" (\"ValueGroup\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ValueGroup\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ValueGroup_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition and reset ITimeStampedThing.CreatedOn properties when needed
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task CopyIterationAndResetCreatedOnAsync(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var updateCreatedOnSql = ", \"ValueTypeDictionary\" = \"ValueTypeDictionary\" || concat('\"CreatedOn\"=>\"', \"SiteDirectory\".get_transaction_time(), '\"') :: hstore";
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ActualFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"ActualFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Alias\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Alias\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Alias\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"AndExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryRelationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\" FROM \"{1}\".\"BinaryRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BooleanExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"BooleanExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BooleanExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Bounds\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Bounds\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Bounds\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BuiltInRuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"BuiltInRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BuiltInRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Citation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\" FROM \"{1}\".\"Citation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Citation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Color\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Color\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Color\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DefinedThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DefinedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DefinedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Definition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramCanvas\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"DiagramCanvas\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramCanvas\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramEdge\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\" FROM \"{1}\".\"DiagramEdge\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramEdge\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramElementContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DepictedThing\", \"SharedStyle\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DepictedThing\", \"SharedStyle\" FROM \"{1}\".\"DiagramElementThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagrammingStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FillColor\", \"FontColor\", \"StrokeColor\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FillColor\", \"FontColor\", \"StrokeColor\" FROM \"{1}\".\"DiagrammingStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagrammingStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramObject\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramObject\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramObject\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramShape\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramShape\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramShape\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramThingBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"DiagramThingBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramThingBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DomainFileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"DomainFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DomainFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"ElementBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ElementDefinition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ElementDefinition\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ElementDefinition\" FROM \"{1}\".\"ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ExclusiveOrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExternalIdentifierMap\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ExternalFormat\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ExternalFormat\", \"Owner\" FROM \"{1}\".\"ExternalIdentifierMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExternalIdentifierMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\" FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\" FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\" FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"HyperLink\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"HyperLink\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"HyperLink\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"IdCorrespondence\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"IdCorrespondence\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"IdCorrespondence\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"MultiRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RootElement\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RootElement\" FROM \"{1}\".\"NestedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedParameter\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ActualState\", \"AssociatedParameter\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ActualState\", \"AssociatedParameter\", \"Owner\" FROM \"{1}\".\"NestedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NotExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Term\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Term\" FROM \"{1}\".\"NotExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NotExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"Option\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"OrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OwnedStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"OwnedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OwnedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Parameter\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RequestedBy\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RequestedBy\" FROM \"{1}\".\"Parameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Parameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\" FROM \"{1}\".\"ParameterBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingGroup\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingGroup\" FROM \"{1}\".\"ParameterGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOrOverrideBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ParameterOrOverrideBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOrOverrideBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverride\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Parameter\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Parameter\" FROM \"{1}\".\"ParameterOverride\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverride\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverrideValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterValueSet\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterValueSet\" FROM \"{1}\".\"ParameterOverrideValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverrideValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscription\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ParameterSubscription\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscription\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscriptionValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SubscribedValueSet\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SubscribedValueSet\" FROM \"{1}\".\"ParameterSubscriptionValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscriptionValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"ParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSet\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ParameterValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ActualOption\", \"ActualState\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ActualOption\", \"ActualState\" FROM \"{1}\".\"ParameterValueSetBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSetBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParametricConstraint\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"TopExpression\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"TopExpression\", \"Sequence\" FROM \"{1}\".\"ParametricConstraint\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParametricConstraint\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Point\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"Point\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Point\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteState\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"PossibleFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultState\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultState\", \"Owner\" FROM \"{1}\".\"PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Publication\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, updateCreatedOnSql);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationalExpression\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"RelationalExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationalExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"Relationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationshipParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RelationshipParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationshipParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Group\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Group\" FROM \"{1}\".\"Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"RequirementsContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainerParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsContainerParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainerParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsSpecification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RequirementsSpecification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsSpecification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\" FROM \"{1}\".\"RuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerificationList\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\" FROM \"{1}\".\"RuleVerificationList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerificationList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleViolation\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"RuleViolation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleViolation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SharedStyle\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"SharedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SharedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterizableThing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\" FROM \"{1}\".\"SimpleParameterizableThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterizableThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\" FROM \"{1}\".\"SimpleParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"Stakeholder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"StakeHolderValueMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMapSettings\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"GoalToValueGroupRelationship\", \"StakeholderValueToRequirementRelationship\", \"ValueGroupToStakeholderValueRelationship\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"GoalToValueGroupRelationship\", \"StakeholderValueToRequirementRelationship\", \"ValueGroupToStakeholderValueRelationship\" FROM \"{1}\".\"StakeHolderValueMapSettings\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMapSettings\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"UserRuleVerification\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Rule\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Rule\" FROM \"{1}\".\"UserRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"UserRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup\" (\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\") SELECT \"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\" FROM \"{1}\".\"ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity'{1};", targetPartition, "");
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState_PossibleState\" (\"ActualFiniteState\", \"PossibleState\", \"ValidFrom\", \"ValidTo\") SELECT \"ActualFiniteState\", \"PossibleState\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ActualFiniteState_PossibleState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState_PossibleState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_ExcludeOption\" (\"ActualFiniteStateList\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\") SELECT \"ActualFiniteStateList\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ActualFiniteStateList_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" (\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"ActualFiniteStateList_PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression_Term\" (\"AndExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"AndExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"AndExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase_Category\" (\"ElementBase\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementBase\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementBase_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_OrganizationalParticipant\" (\"ElementDefinition\", \"OrganizationalParticipant\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementDefinition\", \"OrganizationalParticipant\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementDefinition_OrganizationalParticipant\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_OrganizationalParticipant\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_ReferencedElement\" (\"ElementDefinition\", \"ReferencedElement\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementDefinition\", \"ReferencedElement\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementDefinition_ReferencedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_ReferencedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage_ExcludeOption\" (\"ElementUsage\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\") SELECT \"ElementUsage\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ElementUsage_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression_Term\" (\"ExclusiveOrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"ExclusiveOrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ExclusiveOrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" (\"File\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"File\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" (\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal_Category\" (\"Goal\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Goal\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Goal_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship_RelatedThing\" (\"MultiRelationship\", \"RelatedThing\", \"ValidFrom\", \"ValidTo\") SELECT \"MultiRelationship\", \"RelatedThing\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"MultiRelationship_RelatedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship_RelatedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement_ElementUsage\" (\"NestedElement\", \"ElementUsage\", \"ValidFrom\", \"ValidTo\", \"Sequence\") SELECT \"NestedElement\", \"ElementUsage\", \"ValidFrom\", \"ValidTo\", \"Sequence\" FROM \"{1}\".\"NestedElement_ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement_ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option_Category\" (\"Option\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Option\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Option_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression_Term\" (\"OrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\") SELECT \"OrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"OrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList_Category\" (\"PossibleFiniteStateList\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"PossibleFiniteStateList\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"PossibleFiniteStateList_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_Domain\" (\"Publication\", \"Domain\", \"ValidFrom\", \"ValidTo\") SELECT \"Publication\", \"Domain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Publication_Domain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_Domain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_PublishedParameter\" (\"Publication\", \"PublishedParameter\", \"ValidFrom\", \"ValidTo\") SELECT \"Publication\", \"PublishedParameter\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Publication_PublishedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_PublishedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship_Category\" (\"Relationship\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Relationship\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Relationship_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement_Category\" (\"Requirement\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Requirement\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Requirement_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer_Category\" (\"RequirementsContainer\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"RequirementsContainer\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"RequirementsContainer_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_Category\" (\"Stakeholder\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"Stakeholder\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Stakeholder_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_StakeholderValue\" (\"Stakeholder\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\") SELECT \"Stakeholder\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Stakeholder_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue_Category\" (\"StakeholderValue\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeholderValue\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeholderValue_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Category\" (\"StakeHolderValueMap\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Goal\" (\"StakeHolderValueMap\", \"Goal\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Goal\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Requirement\" (\"StakeHolderValueMap\", \"Requirement\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"Requirement\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_StakeholderValue\" (\"StakeHolderValueMap\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_ValueGroup\" (\"StakeHolderValueMap\", \"ValueGroup\", \"ValidFrom\", \"ValidTo\") SELECT \"StakeHolderValueMap\", \"ValueGroup\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"StakeHolderValueMap_ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" (\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" (\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\") SELECT \"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup_Category\" (\"ValueGroup\", \"Category\", \"ValidFrom\", \"ValidTo\") SELECT \"ValueGroup\", \"Category\", \"ValidFrom\", \"ValidTo\" FROM \"{1}\".\"ValueGroup_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task ModifyUserTriggerAsync(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            var triggerStatus = enable ? "ENABLE" : "DISABLE";
            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Alias\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"AndExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BinaryRelationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BooleanExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Bounds\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BuiltInRuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Citation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Color\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DefinedThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Definition\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramCanvas\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramEdge\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramElementContainer\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramElementThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagrammingStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramObject\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramShape\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramThingBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DomainFileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementUsage\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExclusiveOrExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExternalIdentifierMap\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Folder\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Goal\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"HyperLink\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"IdCorrespondence\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"MultiRelationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedElement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedParameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NotExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Option\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OrExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OwnedStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Parameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOrOverrideBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOverride\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOverrideValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterSubscription\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterSubscriptionValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValueSetBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParametricConstraint\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Point\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RelationalExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Relationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RelationshipParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Requirement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainer\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainerParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsSpecification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleVerificationList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleViolation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SharedStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SimpleParameterizableThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SimpleParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMapSettings\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"UserRuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ValueGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteState_PossibleState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList_ExcludeOption\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"AndExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementBase_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition_OrganizationalParticipant\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition_ReferencedElement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementUsage_ExcludeOption\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExclusiveOrExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision_FileType\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Goal_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"MultiRelationship_RelatedThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedElement_ElementUsage\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Option_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OrExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteStateList_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication_Domain\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication_PublishedParameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Relationship_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Requirement_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainer_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder_StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeholderValue_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Goal\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Requirement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_ValueGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedDomain\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedPerson\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ValueGroup_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="IterationDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition, DateTime? instant)
        {
            var partitionId = partition.Substring(partition.IndexOf('_') + 1);
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Iteration\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"Iteration\".\"DefaultOption\",");

            sqlBuilder.Append(" \"Iteration\".\"IterationSetup\",");

            sqlBuilder.Append(" \"Iteration\".\"TopElement\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_ActualFiniteStateList\".\"ActualFiniteStateList\",'{}'::text[]) AS \"ActualFiniteStateList\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_DiagramCanvas\".\"DiagramCanvas\",'{}'::text[]) AS \"DiagramCanvas\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_DomainFileStore\".\"DomainFileStore\",'{}'::text[]) AS \"DomainFileStore\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Element\".\"Element\",'{}'::text[]) AS \"Element\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_ExternalIdentifierMap\".\"ExternalIdentifierMap\",'{}'::text[]) AS \"ExternalIdentifierMap\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Goal\".\"Goal\",'{}'::text[]) AS \"Goal\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Option\".\"Option\",'{}'::text[]) AS \"Option\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_PossibleFiniteStateList\".\"PossibleFiniteStateList\",'{}'::text[]) AS \"PossibleFiniteStateList\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Publication\".\"Publication\",'{}'::text[]) AS \"Publication\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Relationship\".\"Relationship\",'{}'::text[]) AS \"Relationship\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_RequirementsSpecification\".\"RequirementsSpecification\",'{}'::text[]) AS \"RequirementsSpecification\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_RuleVerificationList\".\"RuleVerificationList\",'{}'::text[]) AS \"RuleVerificationList\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_SharedDiagramStyle\".\"SharedDiagramStyle\",'{}'::text[]) AS \"SharedDiagramStyle\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_Stakeholder\".\"Stakeholder\",'{}'::text[]) AS \"Stakeholder\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_StakeholderValue\".\"StakeholderValue\",'{}'::text[]) AS \"StakeholderValue\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_StakeholderValueMap\".\"StakeholderValueMap\",'{}'::text[]) AS \"StakeholderValueMap\",");
            sqlBuilder.Append(" COALESCE(\"Iteration_ValueGroup\".\"ValueGroup\",'{}'::text[]) AS \"ValueGroup\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" USING (\"Iid\")", this.GetIterationDataSql(partition, instant));

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedDomain\"", this.GetThing_ExcludedDomainDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedPerson\"", this.GetThing_ExcludedPersonDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ActualFiniteStateList\".\"Container\" AS \"Iid\", array_agg(\"ActualFiniteStateList\".\"Iid\"::text) AS \"ActualFiniteStateList\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ActualFiniteStateList\"", this.GetActualFiniteStateListDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"ActualFiniteStateList\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ActualFiniteStateList\".\"Container\") AS \"Iteration_ActualFiniteStateList\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DiagramCanvas\".\"Container\" AS \"Iid\", array_agg(\"DiagramCanvas\".\"Iid\"::text) AS \"DiagramCanvas\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"DiagramCanvas\"", this.GetDiagramCanvasDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"DiagramCanvas\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"DiagramCanvas\".\"Container\") AS \"Iteration_DiagramCanvas\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DomainFileStore\".\"Container\" AS \"Iid\", array_agg(\"DomainFileStore\".\"Iid\"::text) AS \"DomainFileStore\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"DomainFileStore\"", this.GetDomainFileStoreDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"DomainFileStore\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"DomainFileStore\".\"Container\") AS \"Iteration_DomainFileStore\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ElementDefinition\".\"Container\" AS \"Iid\", array_agg(\"ElementDefinition\".\"Iid\"::text) AS \"Element\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ElementDefinition\"", this.GetElementDefinitionDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"ElementDefinition\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ElementDefinition\".\"Container\") AS \"Iteration_Element\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ExternalIdentifierMap\".\"Container\" AS \"Iid\", array_agg(\"ExternalIdentifierMap\".\"Iid\"::text) AS \"ExternalIdentifierMap\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ExternalIdentifierMap\"", this.GetExternalIdentifierMapDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"ExternalIdentifierMap\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ExternalIdentifierMap\".\"Container\") AS \"Iteration_ExternalIdentifierMap\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Goal\".\"Container\" AS \"Iid\", array_agg(\"Goal\".\"Iid\"::text) AS \"Goal\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Goal\"", this.GetGoalDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"Goal\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Goal\".\"Container\") AS \"Iteration_Goal\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Option\".\"Container\" AS \"Iid\", ARRAY[array_agg(\"Option\".\"Sequence\"::text), array_agg(\"Option\".\"Iid\"::text)] AS \"Option\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Option\"", this.GetOptionDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"Option\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Option\".\"Container\") AS \"Iteration_Option\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"PossibleFiniteStateList\".\"Container\" AS \"Iid\", array_agg(\"PossibleFiniteStateList\".\"Iid\"::text) AS \"PossibleFiniteStateList\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"PossibleFiniteStateList\"", this.GetPossibleFiniteStateListDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"PossibleFiniteStateList\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"PossibleFiniteStateList\".\"Container\") AS \"Iteration_PossibleFiniteStateList\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Publication\".\"Container\" AS \"Iid\", array_agg(\"Publication\".\"Iid\"::text) AS \"Publication\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Publication\"", this.GetPublicationDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"Publication\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Publication\".\"Container\") AS \"Iteration_Publication\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Relationship\".\"Container\" AS \"Iid\", array_agg(\"Relationship\".\"Iid\"::text) AS \"Relationship\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Relationship\"", this.GetRelationshipDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"Relationship\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Relationship\".\"Container\") AS \"Iteration_Relationship\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"RequirementsSpecification\".\"Container\" AS \"Iid\", array_agg(\"RequirementsSpecification\".\"Iid\"::text) AS \"RequirementsSpecification\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"RequirementsSpecification\"", this.GetRequirementsSpecificationDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"RequirementsSpecification\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"RequirementsSpecification\".\"Container\") AS \"Iteration_RequirementsSpecification\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"RuleVerificationList\".\"Container\" AS \"Iid\", array_agg(\"RuleVerificationList\".\"Iid\"::text) AS \"RuleVerificationList\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"RuleVerificationList\"", this.GetRuleVerificationListDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"RuleVerificationList\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"RuleVerificationList\".\"Container\") AS \"Iteration_RuleVerificationList\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"SharedStyle\".\"Container\" AS \"Iid\", array_agg(\"SharedStyle\".\"Iid\"::text) AS \"SharedDiagramStyle\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"SharedStyle\"", this.GetSharedStyleDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"SharedStyle\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"SharedStyle\".\"Container\") AS \"Iteration_SharedDiagramStyle\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Stakeholder\".\"Container\" AS \"Iid\", array_agg(\"Stakeholder\".\"Iid\"::text) AS \"Stakeholder\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Stakeholder\"", this.GetStakeholderDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"Stakeholder\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Stakeholder\".\"Container\") AS \"Iteration_Stakeholder\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"StakeholderValue\".\"Container\" AS \"Iid\", array_agg(\"StakeholderValue\".\"Iid\"::text) AS \"StakeholderValue\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"StakeholderValue\"", this.GetStakeholderValueDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"StakeholderValue\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"StakeholderValue\".\"Container\") AS \"Iteration_StakeholderValue\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"StakeHolderValueMap\".\"Container\" AS \"Iid\", array_agg(\"StakeHolderValueMap\".\"Iid\"::text) AS \"StakeholderValueMap\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"StakeHolderValueMap\"", this.GetStakeHolderValueMapDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"StakeHolderValueMap\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"StakeHolderValueMap\".\"Container\") AS \"Iteration_StakeholderValueMap\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ValueGroup\".\"Container\" AS \"Iid\", array_agg(\"ValueGroup\".\"Iid\"::text) AS \"ValueGroup\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ValueGroup\"", this.GetValueGroupDataSql($"Iteration_{partitionId}", instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Iteration\" ON \"ValueGroup\".\"Container\" = \"Iteration\".\"Iid\"", this.GetIterationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ValueGroup\".\"Container\") AS \"Iteration_ValueGroup\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"Iteration_Audit\".\"Actor\", \"Iteration_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Iteration_Audit\" AS \"Iteration_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"Iteration_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"Iteration_Audit\".\"Iid\", \"Iteration_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"Iteration\".\"ValueTypeDictionary\"";

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
        private string GetActualFiniteStateListDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteStateList\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteStateList_Audit\"", partition);
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
        private string GetDiagramCanvasDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramCanvas\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramCanvas_Audit\"", partition);
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
        private string GetDomainFileStoreDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DomainFileStore\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"DomainFileStore_Audit\"", partition);
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
        private string GetElementDefinitionDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ElementDefinition\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ElementDefinition_Audit\"", partition);
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
        private string GetExternalIdentifierMapDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"ExternalFormat\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ExternalIdentifierMap\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ExternalIdentifierMap_Audit\"", partition);
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
        private string GetGoalDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Goal\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Goal_Audit\"", partition);
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
        private string GetOptionDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Sequence\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Option\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Option_Audit\"", partition);
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
        private string GetPossibleFiniteStateListDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultState\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"PossibleFiniteStateList\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"PossibleFiniteStateList_Audit\"", partition);
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
        private string GetPublicationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Publication\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Publication_Audit\"", partition);
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
        private string GetRelationshipDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Relationship\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Relationship_Audit\"", partition);
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
        private string GetRequirementsSpecificationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"RequirementsSpecification\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"RequirementsSpecification_Audit\"", partition);
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
        private string GetRuleVerificationListDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"RuleVerificationList\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"RuleVerificationList_Audit\"", partition);
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
        private string GetSharedStyleDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SharedStyle\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"SharedStyle_Audit\"", partition);
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
        private string GetStakeholderDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Stakeholder\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Stakeholder_Audit\"", partition);
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
        private string GetStakeholderValueDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"StakeholderValue\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"StakeholderValue_Audit\"", partition);
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
        private string GetStakeHolderValueMapDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"StakeHolderValueMap\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"StakeHolderValueMap_Audit\"", partition);
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
        private string GetValueGroupDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ValueGroup\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ValueGroup_Audit\"", partition);
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
