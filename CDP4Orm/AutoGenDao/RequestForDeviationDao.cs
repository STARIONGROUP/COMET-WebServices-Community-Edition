// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestForDeviationDao.cs" company="Starion Group S.A.">
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
    /// The RequestForDeviation Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class RequestForDeviationDao : ContractDeviationDao, IRequestForDeviationDao
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="CDP4Common.DTO.RequestForDeviation"/> as result.
        /// </returns>
        public virtual async Task<IEnumerable<CDP4Common.DTO.RequestForDeviation>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var result = new List<RequestForDeviation>();

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\", \"Actor\" FROM \"{0}\".\"RequestForDeviation_Cache\"", partition);
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
                                result.Add(thing as RequestForDeviation);
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.RequestForDeviation"/>.
        /// </returns>
        public virtual CDP4Common.DTO.RequestForDeviation MapToDto(NpgsqlDataReader reader)
        {
            string tempClassification;
            string tempContent;
            string tempCreatedOn;
            string tempLanguageCode;
            string tempModifiedOn;
            string tempShortName;
            string tempStatus;
            string tempThingPreference;
            string tempTitle;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.RequestForDeviation(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.ApprovedBy.AddRange(Array.ConvertAll((string[])reader["ApprovedBy"], Guid.Parse));
            dto.Author = Guid.Parse(reader["Author"].ToString());
            dto.Category.AddRange(Array.ConvertAll((string[])reader["Category"], Guid.Parse));
            dto.Discussion.AddRange(Array.ConvertAll((string[])reader["Discussion"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            dto.PrimaryAnnotatedThing = reader["PrimaryAnnotatedThing"] is DBNull ? (Guid?)null : Guid.Parse(reader["PrimaryAnnotatedThing"].ToString());
            dto.RelatedThing.AddRange(Array.ConvertAll((string[])reader["RelatedThing"], Guid.Parse));
            dto.SourceAnnotation.AddRange(Array.ConvertAll((string[])reader["SourceAnnotation"], Guid.Parse));

            if (valueDict.TryGetValue("Classification", out tempClassification))
            {
                dto.Classification = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationClassificationKind>(tempClassification);
            }

            if (valueDict.TryGetValue("Content", out tempContent))
            {
                dto.Content = tempContent.UnEscape();
            }

            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }

            if (valueDict.TryGetValue("LanguageCode", out tempLanguageCode))
            {
                dto.LanguageCode = tempLanguageCode.UnEscape();
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
            }

            if (valueDict.TryGetValue("Status", out tempStatus))
            {
                dto.Status = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationStatusKind>(tempStatus);
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
            }

            if (valueDict.TryGetValue("Title", out tempTitle))
            {
                dto.Title = tempTitle.UnEscape();
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
        /// <param name="requestForDeviation">
        /// The requestForDeviation DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RequestForDeviation requestForDeviation, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWriteResult = await this.BeforeWriteAsync(transaction, partition, requestForDeviation, container, valueTypeDictionaryAdditions);

            var beforeWrite = beforeWriteResult.Value;
            var isHandled = beforeWriteResult.IsHandled;

            if (!isHandled)
            {
                beforeWrite = beforeWrite && await base.WriteAsync(transaction, partition, requestForDeviation, container);

                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForDeviation\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = requestForDeviation.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterWriteAsync(beforeWrite, transaction, partition, requestForDeviation, container);
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
        /// <param name="requestForDeviation">
        /// The requestForDeviation DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RequestForDeviation requestForDeviation, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            await base.UpsertAsync(transaction, partition, requestForDeviation, container);

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequestForDeviation\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\")");
                sqlBuilder.AppendFormat(" VALUES (:iid)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = requestForDeviation.Iid;
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO NOTHING; ");

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
        /// <param name="requestForDeviation">
        /// The RequestForDeviation DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully updated as result.
        /// </returns>
        public virtual async Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RequestForDeviation requestForDeviation, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdateResult = await this.BeforeUpdateAsync(transaction, partition, requestForDeviation, container, valueTypeDictionaryAdditions);

            var beforeUpdate = beforeUpdateResult.Value;
            var isHandled = beforeUpdateResult.IsHandled;

            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && await base.UpdateAsync(transaction, partition, requestForDeviation, container);
            }

            return await this.AfterUpdateAsync(beforeUpdate, transaction, partition, requestForDeviation, container);
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
        /// The <see cref="CDP4Common.DTO.RequestForDeviation"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.RequestForDeviation"/> id that is to be deleted.
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
        /// Build a SQL read query for the current <see cref="RequestForDeviationDao" />
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

            sqlBuilder.Append(" \"ModellingAnnotationItem\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"EngineeringModelDataAnnotation\".\"Author\",");

            sqlBuilder.Append(" \"EngineeringModelDataAnnotation\".\"PrimaryAnnotatedThing\",");

            sqlBuilder.Append(" \"ModellingAnnotationItem\".\"Owner\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelDataAnnotation_Discussion\".\"Discussion\",'{}'::text[]) AS \"Discussion\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelDataAnnotation_RelatedThing\".\"RelatedThing\",'{}'::text[]) AS \"RelatedThing\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_ApprovedBy\".\"ApprovedBy\",'{}'::text[]) AS \"ApprovedBy\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_Category\".\"Category\",'{}'::text[]) AS \"Category\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_SourceAnnotation\".\"SourceAnnotation\",'{}'::text[]) AS \"SourceAnnotation\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"GenericAnnotation\" USING (\"Iid\")", this.GetGenericAnnotationDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelDataAnnotation\" USING (\"Iid\")", this.GetEngineeringModelDataAnnotationDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ModellingAnnotationItem\" USING (\"Iid\")", this.GetModellingAnnotationItemDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ContractDeviation\" USING (\"Iid\")", this.GetContractDeviationDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"RequestForDeviation\" USING (\"Iid\")", this.GetRequestForDeviationDataSql(partition, instant));

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedDomain\"", this.GetThing_ExcludedDomainDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedPerson\"", this.GetThing_ExcludedPersonDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelDataDiscussionItem\".\"Container\" AS \"Iid\", array_agg(\"EngineeringModelDataDiscussionItem\".\"Iid\"::text) AS \"Discussion\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"EngineeringModelDataDiscussionItem\"", this.GetEngineeringModelDataDiscussionItemDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelDataAnnotation\" ON \"EngineeringModelDataDiscussionItem\".\"Container\" = \"EngineeringModelDataAnnotation\".\"Iid\"", this.GetEngineeringModelDataAnnotationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"EngineeringModelDataDiscussionItem\".\"Container\") AS \"EngineeringModelDataAnnotation_Discussion\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingThingReference\".\"Container\" AS \"Iid\", array_agg(\"ModellingThingReference\".\"Iid\"::text) AS \"RelatedThing\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModellingThingReference\"", this.GetModellingThingReferenceDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelDataAnnotation\" ON \"ModellingThingReference\".\"Container\" = \"EngineeringModelDataAnnotation\".\"Iid\"", this.GetEngineeringModelDataAnnotationDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModellingThingReference\".\"Container\") AS \"EngineeringModelDataAnnotation_RelatedThing\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Approval\".\"Container\" AS \"Iid\", array_agg(\"Approval\".\"Iid\"::text) AS \"ApprovedBy\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Approval\"", this.GetApprovalDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ModellingAnnotationItem\" ON \"Approval\".\"Container\" = \"ModellingAnnotationItem\".\"Iid\"", this.GetModellingAnnotationItemDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Approval\".\"Container\") AS \"ModellingAnnotationItem_ApprovedBy\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem\" AS \"Iid\", array_agg(\"Category\"::text) AS \"Category\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModellingAnnotationItem_Category\"", this.GetModellingAnnotationItem_CategoryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ModellingAnnotationItem\" ON \"ModellingAnnotationItem\" = \"Iid\"", this.GetModellingAnnotationItemDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem\") AS \"ModellingAnnotationItem_Category\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem\" AS \"Iid\", array_agg(\"SourceAnnotation\"::text) AS \"SourceAnnotation\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModellingAnnotationItem_SourceAnnotation\"", this.GetModellingAnnotationItem_SourceAnnotationDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ModellingAnnotationItem\" ON \"ModellingAnnotationItem\" = \"Iid\"", this.GetModellingAnnotationItemDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem\") AS \"ModellingAnnotationItem_SourceAnnotation\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"RequestForDeviation_Audit\".\"Actor\", \"RequestForDeviation_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"RequestForDeviation_Audit\" AS \"RequestForDeviation_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"RequestForDeviation_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"RequestForDeviation_Audit\".\"Iid\", \"RequestForDeviation_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"GenericAnnotation\".\"ValueTypeDictionary\" || \"EngineeringModelDataAnnotation\".\"ValueTypeDictionary\" || \"ModellingAnnotationItem\".\"ValueTypeDictionary\" || \"ContractDeviation\".\"ValueTypeDictionary\" || \"RequestForDeviation\".\"ValueTypeDictionary\"";

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
        private string GetGenericAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"GenericAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"GenericAnnotation_Audit\"", partition);
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
        private string GetEngineeringModelDataAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Author\", \"PrimaryAnnotatedThing\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataAnnotation_Audit\"", partition);
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

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetContractDeviationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ContractDeviation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ContractDeviation_Audit\"", partition);
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
        private string GetRequestForDeviationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"RequestForDeviation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"RequestForDeviation_Audit\"", partition);
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
        private string GetEngineeringModelDataDiscussionItemDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Author\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataDiscussionItem\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataDiscussionItem_Audit\"", partition);
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
        private string GetModellingThingReferenceDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingThingReference\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingThingReference_Audit\"", partition);
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
        private string GetApprovalDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Author\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Approval\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Approval_Audit\"", partition);
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
        private string GetModellingAnnotationItem_CategoryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ModellingAnnotationItem\",\"Category\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Category\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Category_Audit\"", partition);
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
        private string GetModellingAnnotationItem_SourceAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ModellingAnnotationItem\",\"SourceAnnotation\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation_Audit\"", partition);
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
