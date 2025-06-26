// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibraryDao.cs" company="Starion Group S.A.">
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
    /// The SiteReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class SiteReferenceDataLibraryDao : ReferenceDataLibraryDao, ISiteReferenceDataLibraryDao
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
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> as result.
        /// </returns>
        public virtual async Task<IEnumerable<CDP4Common.DTO.SiteReferenceDataLibrary>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var result = new List<SiteReferenceDataLibrary>();

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\", \"Actor\" FROM \"{0}\".\"SiteReferenceDataLibrary_Cache\"", partition);
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
                                result.Add(thing as SiteReferenceDataLibrary);
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/>.
        /// </returns>
        public virtual CDP4Common.DTO.SiteReferenceDataLibrary MapToDto(NpgsqlDataReader reader)
        {
            string tempIsDeprecated;
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.SiteReferenceDataLibrary(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.BaseQuantityKind.AddRange(Utils.ParseOrderedList<Guid>(reader["BaseQuantityKind"] as string[,]));
            dto.BaseUnit.AddRange(Array.ConvertAll((string[])reader["BaseUnit"], Guid.Parse));
            dto.Constant.AddRange(Array.ConvertAll((string[])reader["Constant"], Guid.Parse));
            dto.DefinedCategory.AddRange(Array.ConvertAll((string[])reader["DefinedCategory"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.FileType.AddRange(Array.ConvertAll((string[])reader["FileType"], Guid.Parse));
            dto.Glossary.AddRange(Array.ConvertAll((string[])reader["Glossary"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.ParameterType.AddRange(Array.ConvertAll((string[])reader["ParameterType"], Guid.Parse));
            dto.ReferenceSource.AddRange(Array.ConvertAll((string[])reader["ReferenceSource"], Guid.Parse));
            dto.RequiredRdl = reader["RequiredRdl"] is DBNull ? (Guid?)null : Guid.Parse(reader["RequiredRdl"].ToString());
            dto.Rule.AddRange(Array.ConvertAll((string[])reader["Rule"], Guid.Parse));
            dto.Scale.AddRange(Array.ConvertAll((string[])reader["Scale"], Guid.Parse));
            dto.Unit.AddRange(Array.ConvertAll((string[])reader["Unit"], Guid.Parse));
            dto.UnitPrefix.AddRange(Array.ConvertAll((string[])reader["UnitPrefix"], Guid.Parse));

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
        /// <param name="siteReferenceDataLibrary">
        /// The siteReferenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteReferenceDataLibrary siteReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWriteResult = await this.BeforeWriteAsync(transaction, partition, siteReferenceDataLibrary, container, valueTypeDictionaryAdditions);

            var beforeWrite = beforeWriteResult.Value;
            var isHandled = beforeWriteResult.IsHandled;

            if (!isHandled)
            {
                beforeWrite = beforeWrite && await base.WriteAsync(transaction, partition, siteReferenceDataLibrary, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "IsDeprecated", !this.IsDerived(siteReferenceDataLibrary, "IsDeprecated") ? siteReferenceDataLibrary.IsDeprecated.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SiteReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteReferenceDataLibrary.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterWriteAsync(beforeWrite, transaction, partition, siteReferenceDataLibrary, container);
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
        /// <param name="siteReferenceDataLibrary">
        /// The siteReferenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteReferenceDataLibrary siteReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            await base.UpsertAsync(transaction, partition, siteReferenceDataLibrary, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "IsDeprecated", !this.IsDerived(siteReferenceDataLibrary, "IsDeprecated") ? siteReferenceDataLibrary.IsDeprecated.ToString() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SiteReferenceDataLibrary\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteReferenceDataLibrary.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container);");

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
        /// <param name="siteReferenceDataLibrary">
        /// The SiteReferenceDataLibrary DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully updated as result.
        /// </returns>
        public virtual async Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteReferenceDataLibrary siteReferenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdateResult = await this.BeforeUpdateAsync(transaction, partition, siteReferenceDataLibrary, container, valueTypeDictionaryAdditions);

            var beforeUpdate = beforeUpdateResult.Value;
            var isHandled = beforeUpdateResult.IsHandled;

            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && await base.UpdateAsync(transaction, partition, siteReferenceDataLibrary, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "IsDeprecated", !this.IsDerived(siteReferenceDataLibrary, "IsDeprecated") ? siteReferenceDataLibrary.IsDeprecated.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SiteReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteReferenceDataLibrary.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterUpdateAsync(beforeUpdate, transaction, partition, siteReferenceDataLibrary, container);
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
        /// The <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> id that is to be deleted.
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
                await using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    var valueTypeDictionaryContents = new Dictionary<string, string>
                            {
                                { "IsDeprecated", "true" }
                            };
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SiteReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = :valueTypeDictionary");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    isHandled = (await command.ExecuteNonQueryAsync()) > 0;
                }
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
        /// The <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having True if the concept was successfully deleted as result.
        /// </returns>
        public override async Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            await using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "IsDeprecated", "true" }
                        };
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SiteReferenceDataLibrary\"", partition);
                sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = :valueTypeDictionary");
                sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                result = (await command.ExecuteNonQueryAsync()) > 0;
            }

            return result;
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="SiteReferenceDataLibraryDao" />
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

            sqlBuilder.Append(" \"SiteReferenceDataLibrary\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"ReferenceDataLibrary\".\"RequiredRdl\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseQuantityKind\".\"BaseQuantityKind\",'{}'::text[]) AS \"BaseQuantityKind\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseUnit\".\"BaseUnit\",'{}'::text[]) AS \"BaseUnit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Constant\".\"Constant\",'{}'::text[]) AS \"Constant\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_DefinedCategory\".\"DefinedCategory\",'{}'::text[]) AS \"DefinedCategory\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_FileType\".\"FileType\",'{}'::text[]) AS \"FileType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Glossary\".\"Glossary\",'{}'::text[]) AS \"Glossary\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ParameterType\".\"ParameterType\",'{}'::text[]) AS \"ParameterType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ReferenceSource\".\"ReferenceSource\",'{}'::text[]) AS \"ReferenceSource\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Rule\".\"Rule\",'{}'::text[]) AS \"Rule\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Scale\".\"Scale\",'{}'::text[]) AS \"Scale\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Unit\".\"Unit\",'{}'::text[]) AS \"Unit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_UnitPrefix\".\"UnitPrefix\",'{}'::text[]) AS \"UnitPrefix\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" USING (\"Iid\")", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" USING (\"Iid\")", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"SiteReferenceDataLibrary\" USING (\"Iid\")", this.GetSiteReferenceDataLibraryDataSql(partition, instant));

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedDomain\"", this.GetThing_ExcludedDomainDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedPerson\"", this.GetThing_ExcludedPersonDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Alias\".\"Container\" AS \"Iid\", array_agg(\"Alias\".\"Iid\"::text) AS \"Alias\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Alias\"", this.GetAliasDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"Alias\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Alias\".\"Container\") AS \"DefinedThing_Alias\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Definition\".\"Container\" AS \"Iid\", array_agg(\"Definition\".\"Iid\"::text) AS \"Definition\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Definition\"", this.GetDefinitionDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"Definition\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Definition\".\"Container\") AS \"DefinedThing_Definition\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"HyperLink\".\"Container\" AS \"Iid\", array_agg(\"HyperLink\".\"Iid\"::text) AS \"HyperLink\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"HyperLink\"", this.GetHyperLinkDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"HyperLink\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"HyperLink\".\"Container\") AS \"DefinedThing_HyperLink\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", ARRAY[array_agg(\"Sequence\"::text), array_agg(\"BaseQuantityKind\"::text)] AS \"BaseQuantityKind\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ReferenceDataLibrary_BaseQuantityKind\"", this.GetReferenceDataLibrary_BaseQuantityKindDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseQuantityKind\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", array_agg(\"BaseUnit\"::text) AS \"BaseUnit\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ReferenceDataLibrary_BaseUnit\"", this.GetReferenceDataLibrary_BaseUnitDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseUnit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Constant\".\"Container\" AS \"Iid\", array_agg(\"Constant\".\"Iid\"::text) AS \"Constant\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Constant\"", this.GetConstantDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"Constant\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Constant\".\"Container\") AS \"ReferenceDataLibrary_Constant\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Category\".\"Container\" AS \"Iid\", array_agg(\"Category\".\"Iid\"::text) AS \"DefinedCategory\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Category\"", this.GetCategoryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"Category\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Category\".\"Container\") AS \"ReferenceDataLibrary_DefinedCategory\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"FileType\".\"Container\" AS \"Iid\", array_agg(\"FileType\".\"Iid\"::text) AS \"FileType\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"FileType\"", this.GetFileTypeDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"FileType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"FileType\".\"Container\") AS \"ReferenceDataLibrary_FileType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Glossary\".\"Container\" AS \"Iid\", array_agg(\"Glossary\".\"Iid\"::text) AS \"Glossary\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Glossary\"", this.GetGlossaryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"Glossary\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Glossary\".\"Container\") AS \"ReferenceDataLibrary_Glossary\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ParameterType\".\"Container\" AS \"Iid\", array_agg(\"ParameterType\".\"Iid\"::text) AS \"ParameterType\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ParameterType\"", this.GetParameterTypeDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"ParameterType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ParameterType\".\"Container\") AS \"ReferenceDataLibrary_ParameterType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceSource\".\"Container\" AS \"Iid\", array_agg(\"ReferenceSource\".\"Iid\"::text) AS \"ReferenceSource\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ReferenceSource\"", this.GetReferenceSourceDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"ReferenceSource\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ReferenceSource\".\"Container\") AS \"ReferenceDataLibrary_ReferenceSource\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Rule\".\"Container\" AS \"Iid\", array_agg(\"Rule\".\"Iid\"::text) AS \"Rule\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Rule\"", this.GetRuleDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"Rule\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Rule\".\"Container\") AS \"ReferenceDataLibrary_Rule\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementScale\".\"Container\" AS \"Iid\", array_agg(\"MeasurementScale\".\"Iid\"::text) AS \"Scale\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"MeasurementScale\"", this.GetMeasurementScaleDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"MeasurementScale\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"MeasurementScale\".\"Container\") AS \"ReferenceDataLibrary_Scale\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementUnit\".\"Container\" AS \"Iid\", array_agg(\"MeasurementUnit\".\"Iid\"::text) AS \"Unit\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"MeasurementUnit\"", this.GetMeasurementUnitDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"MeasurementUnit\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"MeasurementUnit\".\"Container\") AS \"ReferenceDataLibrary_Unit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"UnitPrefix\".\"Container\" AS \"Iid\", array_agg(\"UnitPrefix\".\"Iid\"::text) AS \"UnitPrefix\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"UnitPrefix\"", this.GetUnitPrefixDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"ReferenceDataLibrary\" ON \"UnitPrefix\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", this.GetReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"UnitPrefix\".\"Container\") AS \"ReferenceDataLibrary_UnitPrefix\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"SiteReferenceDataLibrary_Audit\".\"Actor\", \"SiteReferenceDataLibrary_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteReferenceDataLibrary_Audit\" AS \"SiteReferenceDataLibrary_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"SiteReferenceDataLibrary_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"SiteReferenceDataLibrary_Audit\".\"Iid\", \"SiteReferenceDataLibrary_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"ReferenceDataLibrary\".\"ValueTypeDictionary\" || \"SiteReferenceDataLibrary\".\"ValueTypeDictionary\"";

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
        private string GetDefinedThingDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DefinedThing\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"DefinedThing_Audit\"", partition);
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
        private string GetReferenceDataLibraryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"RequiredRdl\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_Audit\"", partition);
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
        private string GetSiteReferenceDataLibraryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteReferenceDataLibrary\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteReferenceDataLibrary_Audit\"", partition);
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
        private string GetAliasDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Alias\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Alias_Audit\"", partition);
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
        private string GetDefinitionDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Definition\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Definition_Audit\"", partition);
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
        private string GetHyperLinkDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"HyperLink\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"HyperLink_Audit\"", partition);
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
        private string GetReferenceDataLibrary_BaseQuantityKindDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ReferenceDataLibrary\",\"BaseQuantityKind\",\"ValidFrom\",\"ValidTo\"";

            fields += ", \"Sequence\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind_Audit\"", partition);
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
        private string GetReferenceDataLibrary_BaseUnitDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ReferenceDataLibrary\",\"BaseUnit\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit_Audit\"", partition);
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
        private string GetConstantDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"ParameterType\", \"Scale\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Constant\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Constant_Audit\"", partition);
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
        private string GetCategoryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Category\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Category_Audit\"", partition);
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
        private string GetFileTypeDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"FileType\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"FileType_Audit\"", partition);
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
        private string GetGlossaryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Glossary\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Glossary_Audit\"", partition);
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
        private string GetParameterTypeDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ParameterType\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ParameterType_Audit\"", partition);
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
        private string GetReferenceSourceDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"PublishedIn\", \"Publisher\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceSource\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceSource_Audit\"", partition);
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
        private string GetRuleDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Rule\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Rule_Audit\"", partition);
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
        private string GetMeasurementScaleDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Unit\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementScale\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementScale_Audit\"", partition);
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
        private string GetMeasurementUnitDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementUnit\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementUnit_Audit\"", partition);
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
        private string GetUnitPrefixDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"UnitPrefix\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"UnitPrefix_Audit\"", partition);
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
