// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryDao.cs" company="RHEA System S.A.">
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

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The SiteDirectory Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class SiteDirectoryDao : TopContainerDao, ISiteDirectoryDao
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
        /// List of instances of <see cref="CDP4Common.DTO.SiteDirectory"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.SiteDirectory> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"SiteDirectory_Cache\"", partition);
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

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as SiteDirectory;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.Append(this.BuildReadQuery(partition));

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.SiteDirectory"/>.
        /// </returns>
        public virtual CDP4Common.DTO.SiteDirectory MapToDto(NpgsqlDataReader reader)
        {
            string tempCreatedOn;
            string tempLastModifiedOn;
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.SiteDirectory(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Annotation.AddRange(Array.ConvertAll((string[])reader["Annotation"], Guid.Parse));
            dto.DefaultParticipantRole = reader["DefaultParticipantRole"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultParticipantRole"].ToString());
            dto.DefaultPersonRole = reader["DefaultPersonRole"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultPersonRole"].ToString());
            dto.Domain.AddRange(Array.ConvertAll((string[])reader["Domain"], Guid.Parse));
            dto.DomainGroup.AddRange(Array.ConvertAll((string[])reader["DomainGroup"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.LogEntry.AddRange(Array.ConvertAll((string[])reader["LogEntry"], Guid.Parse));
            dto.Model.AddRange(Array.ConvertAll((string[])reader["Model"], Guid.Parse));
            dto.NaturalLanguage.AddRange(Array.ConvertAll((string[])reader["NaturalLanguage"], Guid.Parse));
            dto.Organization.AddRange(Array.ConvertAll((string[])reader["Organization"], Guid.Parse));
            dto.ParticipantRole.AddRange(Array.ConvertAll((string[])reader["ParticipantRole"], Guid.Parse));
            dto.Person.AddRange(Array.ConvertAll((string[])reader["Person"], Guid.Parse));
            dto.PersonRole.AddRange(Array.ConvertAll((string[])reader["PersonRole"], Guid.Parse));
            dto.SiteReferenceDataLibrary.AddRange(Array.ConvertAll((string[])reader["SiteReferenceDataLibrary"], Guid.Parse));

            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }

            if (valueDict.TryGetValue("LastModifiedOn", out tempLastModifiedOn))
            {
                dto.LastModifiedOn = Utils.ParseUtcDate(tempLastModifiedOn);
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
        /// <param name="siteDirectory">
        /// The siteDirectory DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteDirectory siteDirectory, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, siteDirectory, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, siteDirectory, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(siteDirectory, "CreatedOn") ? siteDirectory.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                    { "Name", !this.IsDerived(siteDirectory, "Name") ? siteDirectory.Name.Escape() : string.Empty },
                    { "ShortName", !this.IsDerived(siteDirectory, "ShortName") ? siteDirectory.ShortName.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SiteDirectory\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"DefaultParticipantRole\", \"DefaultPersonRole\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :defaultParticipantRole, :defaultPersonRole);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteDirectory.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("defaultParticipantRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultParticipantRole") ? Utils.NullableValue(siteDirectory.DefaultParticipantRole) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultPersonRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultPersonRole") ? Utils.NullableValue(siteDirectory.DefaultPersonRole) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, siteDirectory, container);
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
        /// <param name="siteDirectory">
        /// The siteDirectory DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteDirectory siteDirectory, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, siteDirectory, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "CreatedOn", !this.IsDerived(siteDirectory, "CreatedOn") ? siteDirectory.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                { "Name", !this.IsDerived(siteDirectory, "Name") ? siteDirectory.Name.Escape() : string.Empty },
                { "ShortName", !this.IsDerived(siteDirectory, "ShortName") ? siteDirectory.ShortName.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SiteDirectory\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"DefaultParticipantRole\", \"DefaultPersonRole\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :defaultParticipantRole, :defaultPersonRole)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteDirectory.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("defaultParticipantRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultParticipantRole") ? Utils.NullableValue(siteDirectory.DefaultParticipantRole) : Utils.NullableValue(null);
                command.Parameters.Add("defaultPersonRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultPersonRole") ? Utils.NullableValue(siteDirectory.DefaultPersonRole) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"DefaultParticipantRole\", \"DefaultPersonRole\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :defaultParticipantRole, :defaultPersonRole);");

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
        /// <param name="siteDirectory">
        /// The SiteDirectory DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SiteDirectory siteDirectory, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, siteDirectory, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, siteDirectory, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(siteDirectory, "CreatedOn") ? siteDirectory.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                    { "Name", !this.IsDerived(siteDirectory, "Name") ? siteDirectory.Name.Escape() : string.Empty },
                    { "ShortName", !this.IsDerived(siteDirectory, "ShortName") ? siteDirectory.ShortName.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SiteDirectory\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"DefaultParticipantRole\", \"DefaultPersonRole\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :defaultParticipantRole, :defaultPersonRole)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = siteDirectory.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("defaultParticipantRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultParticipantRole") ? Utils.NullableValue(siteDirectory.DefaultParticipantRole) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultPersonRole", NpgsqlDbType.Uuid).Value = !this.IsDerived(siteDirectory, "DefaultPersonRole") ? Utils.NullableValue(siteDirectory.DefaultPersonRole) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, siteDirectory, container);
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
        /// The <see cref="CDP4Common.DTO.SiteDirectory"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.SiteDirectory"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool RawDelete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = base.Delete(transaction, partition, iid);
            return result;
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="SiteDirectoryDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"SiteDirectory\".\"DefaultParticipantRole\",");

            sqlBuilder.Append(" \"SiteDirectory\".\"DefaultPersonRole\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_Annotation\".\"Annotation\",'{}'::text[]) AS \"Annotation\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_Domain\".\"Domain\",'{}'::text[]) AS \"Domain\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_DomainGroup\".\"DomainGroup\",'{}'::text[]) AS \"DomainGroup\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_LogEntry\".\"LogEntry\",'{}'::text[]) AS \"LogEntry\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_Model\".\"Model\",'{}'::text[]) AS \"Model\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_NaturalLanguage\".\"NaturalLanguage\",'{}'::text[]) AS \"NaturalLanguage\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_Organization\".\"Organization\",'{}'::text[]) AS \"Organization\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_ParticipantRole\".\"ParticipantRole\",'{}'::text[]) AS \"ParticipantRole\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_Person\".\"Person\",'{}'::text[]) AS \"Person\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_PersonRole\".\"PersonRole\",'{}'::text[]) AS \"PersonRole\",");
            sqlBuilder.Append(" COALESCE(\"SiteDirectory_SiteReferenceDataLibrary\".\"SiteReferenceDataLibrary\",'{}'::text[]) AS \"SiteReferenceDataLibrary\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"TopContainer_Data\"() AS \"TopContainer\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"SiteDirectoryDataAnnotation\".\"Container\" AS \"Iid\", array_agg(\"SiteDirectoryDataAnnotation\".\"Iid\"::text) AS \"Annotation\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteDirectoryDataAnnotation_Data\"() AS \"SiteDirectoryDataAnnotation\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"SiteDirectoryDataAnnotation\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"SiteDirectoryDataAnnotation\".\"Container\") AS \"SiteDirectory_Annotation\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DomainOfExpertise\".\"Container\" AS \"Iid\", array_agg(\"DomainOfExpertise\".\"Iid\"::text) AS \"Domain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DomainOfExpertise_Data\"() AS \"DomainOfExpertise\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"DomainOfExpertise\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"DomainOfExpertise\".\"Container\") AS \"SiteDirectory_Domain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DomainOfExpertiseGroup\".\"Container\" AS \"Iid\", array_agg(\"DomainOfExpertiseGroup\".\"Iid\"::text) AS \"DomainGroup\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DomainOfExpertiseGroup_Data\"() AS \"DomainOfExpertiseGroup\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"DomainOfExpertiseGroup\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"DomainOfExpertiseGroup\".\"Container\") AS \"SiteDirectory_DomainGroup\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"SiteLogEntry\".\"Container\" AS \"Iid\", array_agg(\"SiteLogEntry\".\"Iid\"::text) AS \"LogEntry\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteLogEntry_Data\"() AS \"SiteLogEntry\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"SiteLogEntry\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"SiteLogEntry\".\"Container\") AS \"SiteDirectory_LogEntry\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelSetup\".\"Container\" AS \"Iid\", array_agg(\"EngineeringModelSetup\".\"Iid\"::text) AS \"Model\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup_Data\"() AS \"EngineeringModelSetup\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"EngineeringModelSetup\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"EngineeringModelSetup\".\"Container\") AS \"SiteDirectory_Model\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"NaturalLanguage\".\"Container\" AS \"Iid\", array_agg(\"NaturalLanguage\".\"Iid\"::text) AS \"NaturalLanguage\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"NaturalLanguage_Data\"() AS \"NaturalLanguage\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"NaturalLanguage\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"NaturalLanguage\".\"Container\") AS \"SiteDirectory_NaturalLanguage\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Organization\".\"Container\" AS \"Iid\", array_agg(\"Organization\".\"Iid\"::text) AS \"Organization\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Organization_Data\"() AS \"Organization\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"Organization\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Organization\".\"Container\") AS \"SiteDirectory_Organization\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ParticipantRole\".\"Container\" AS \"Iid\", array_agg(\"ParticipantRole\".\"Iid\"::text) AS \"ParticipantRole\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ParticipantRole_Data\"() AS \"ParticipantRole\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"ParticipantRole\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ParticipantRole\".\"Container\") AS \"SiteDirectory_ParticipantRole\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Person\".\"Container\" AS \"Iid\", array_agg(\"Person\".\"Iid\"::text) AS \"Person\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Person_Data\"() AS \"Person\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"Person\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Person\".\"Container\") AS \"SiteDirectory_Person\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"PersonRole\".\"Container\" AS \"Iid\", array_agg(\"PersonRole\".\"Iid\"::text) AS \"PersonRole\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"PersonRole_Data\"() AS \"PersonRole\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"PersonRole\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"PersonRole\".\"Container\") AS \"SiteDirectory_PersonRole\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"SiteReferenceDataLibrary\".\"Container\" AS \"Iid\", array_agg(\"SiteReferenceDataLibrary\".\"Iid\"::text) AS \"SiteReferenceDataLibrary\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteReferenceDataLibrary_Data\"() AS \"SiteReferenceDataLibrary\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SiteDirectory_Data\"() AS \"SiteDirectory\" ON \"SiteReferenceDataLibrary\".\"Container\" = \"SiteDirectory\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"SiteReferenceDataLibrary\".\"Container\") AS \"SiteDirectory_SiteReferenceDataLibrary\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"SiteDirectory_Audit\".\"Actor\", \"SiteDirectory_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SiteDirectory_Audit\" AS \"SiteDirectory_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"SiteDirectory_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"SiteDirectory_Audit\".\"Iid\", \"SiteDirectory_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"TopContainer\".\"ValueTypeDictionary\" || \"SiteDirectory\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
