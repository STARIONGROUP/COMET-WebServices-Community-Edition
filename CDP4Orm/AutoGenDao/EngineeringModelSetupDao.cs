// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The EngineeringModelSetup Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class EngineeringModelSetupDao : DefinedThingDao, IEngineeringModelSetupDao
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
        /// List of instances of <see cref="CDP4Common.DTO.EngineeringModelSetup"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.EngineeringModelSetup> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\", \"Actor\" FROM \"{0}\".\"EngineeringModelSetup_Cache\"", partition);
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

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as EngineeringModelSetup;
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.EngineeringModelSetup"/>.
        /// </returns>
        public virtual CDP4Common.DTO.EngineeringModelSetup MapToDto(NpgsqlDataReader reader)
        {
            string tempAutoPublish;
            string tempEngineeringModelIid;
            string tempKind;
            string tempModifiedOn;
            string tempName;
            string tempShortName;
            string tempSourceEngineeringModelSetupIid;
            string tempStudyPhase;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.EngineeringModelSetup(iid, revisionNumber);
            dto.ActiveDomain.AddRange(Array.ConvertAll((string[])reader["ActiveDomain"], Guid.Parse));
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Attachment.AddRange(Array.ConvertAll((string[])reader["Attachment"], Guid.Parse));
            dto.DefaultOrganizationalParticipant = reader["DefaultOrganizationalParticipant"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultOrganizationalParticipant"].ToString());
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.IterationSetup.AddRange(Array.ConvertAll((string[])reader["IterationSetup"], Guid.Parse));
            dto.OrganizationalParticipant.AddRange(Array.ConvertAll((string[])reader["OrganizationalParticipant"], Guid.Parse));
            dto.Participant.AddRange(Array.ConvertAll((string[])reader["Participant"], Guid.Parse));
            dto.RequiredRdl.AddRange(Array.ConvertAll((string[])reader["RequiredRdl"], Guid.Parse));

            if (valueDict.TryGetValue("AutoPublish", out tempAutoPublish))
            {
                dto.AutoPublish = bool.Parse(tempAutoPublish);
            }

            if (valueDict.TryGetValue("EngineeringModelIid", out tempEngineeringModelIid))
            {
                dto.EngineeringModelIid = Guid.Parse(tempEngineeringModelIid);
            }

            if (valueDict.TryGetValue("Kind", out tempKind))
            {
                dto.Kind = Utils.ParseEnum<CDP4Common.SiteDirectoryData.EngineeringModelKind>(tempKind);
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

            if (valueDict.TryGetValue("SourceEngineeringModelSetupIid", out tempSourceEngineeringModelSetupIid) && tempSourceEngineeringModelSetupIid != null)
            {
                dto.SourceEngineeringModelSetupIid = Guid.Parse(tempSourceEngineeringModelSetupIid);
            }

            if (valueDict.TryGetValue("StudyPhase", out tempStudyPhase))
            {
                dto.StudyPhase = Utils.ParseEnum<CDP4Common.SiteDirectoryData.StudyPhaseKind>(tempStudyPhase);
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
        /// <param name="engineeringModelSetup">
        /// The engineeringModelSetup DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModelSetup engineeringModelSetup, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, engineeringModelSetup, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, engineeringModelSetup, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "AutoPublish", !this.IsDerived(engineeringModelSetup, "AutoPublish") ? engineeringModelSetup.AutoPublish.ToString() : string.Empty },
                    { "EngineeringModelIid", !this.IsDerived(engineeringModelSetup, "EngineeringModelIid") ? engineeringModelSetup.EngineeringModelIid.ToString() : string.Empty },
                    { "Kind", !this.IsDerived(engineeringModelSetup, "Kind") ? engineeringModelSetup.Kind.ToString() : string.Empty },
                    { "SourceEngineeringModelSetupIid", !this.IsDerived(engineeringModelSetup, "SourceEngineeringModelSetupIid") && engineeringModelSetup.SourceEngineeringModelSetupIid.HasValue ? engineeringModelSetup.SourceEngineeringModelSetupIid.Value.ToString() : null },
                    { "StudyPhase", !this.IsDerived(engineeringModelSetup, "StudyPhase") ? engineeringModelSetup.StudyPhase.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelSetup\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOrganizationalParticipant\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOrganizationalParticipant);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModelSetup.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOrganizationalParticipant", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelSetup, "DefaultOrganizationalParticipant") ? Utils.NullableValue(engineeringModelSetup.DefaultOrganizationalParticipant) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
                engineeringModelSetup.ActiveDomain.ForEach(x => this.AddActiveDomain(transaction, partition, engineeringModelSetup.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, engineeringModelSetup, container);
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
        /// <param name="engineeringModelSetup">
        /// The engineeringModelSetup DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModelSetup engineeringModelSetup, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, engineeringModelSetup, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "AutoPublish", !this.IsDerived(engineeringModelSetup, "AutoPublish") ? engineeringModelSetup.AutoPublish.ToString() : string.Empty },
                { "EngineeringModelIid", !this.IsDerived(engineeringModelSetup, "EngineeringModelIid") ? engineeringModelSetup.EngineeringModelIid.ToString() : string.Empty },
                { "Kind", !this.IsDerived(engineeringModelSetup, "Kind") ? engineeringModelSetup.Kind.ToString() : string.Empty },
                { "SourceEngineeringModelSetupIid", !this.IsDerived(engineeringModelSetup, "SourceEngineeringModelSetupIid") && engineeringModelSetup.SourceEngineeringModelSetupIid.HasValue ? engineeringModelSetup.SourceEngineeringModelSetupIid.Value.ToString() : null },
                { "StudyPhase", !this.IsDerived(engineeringModelSetup, "StudyPhase") ? engineeringModelSetup.StudyPhase.ToString() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelSetup\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOrganizationalParticipant\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOrganizationalParticipant)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModelSetup.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("defaultOrganizationalParticipant", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelSetup, "DefaultOrganizationalParticipant") ? Utils.NullableValue(engineeringModelSetup.DefaultOrganizationalParticipant) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOrganizationalParticipant\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :defaultOrganizationalParticipant);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
            }
            engineeringModelSetup.ActiveDomain.ForEach(x => this.UpsertActiveDomain(transaction, partition, engineeringModelSetup.Iid, x));

            return true;
        }

        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that will be the source for each link table record.
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
                case "ActiveDomain":
                    {
                        isCreated = this.AddActiveDomain(transaction, partition, iid, (Guid)value);
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
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="activeDomain">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddActiveDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid activeDomain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelSetup_ActiveDomain\"", partition);
                sqlBuilder.AppendFormat(" (\"EngineeringModelSetup\", \"ActiveDomain\")");
                sqlBuilder.Append(" VALUES (:engineeringModelSetup, :activeDomain);");

                command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("activeDomain", NpgsqlDbType.Uuid).Value = activeDomain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table, or update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="activeDomain">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertActiveDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid activeDomain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelSetup_ActiveDomain\"", partition);
                sqlBuilder.AppendFormat(" (\"EngineeringModelSetup\", \"ActiveDomain\")");
                sqlBuilder.Append(" VALUES (:engineeringModelSetup, :activeDomain)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"EngineeringModelSetup_ActiveDomain_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"EngineeringModelSetup\", \"ActiveDomain\")");
                sqlBuilder.Append(" = (:engineeringModelSetup, :activeDomain);");

                command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("activeDomain", NpgsqlDbType.Uuid).Value = activeDomain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return command.ExecuteNonQuery() > 0;
            }
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
        /// <param name="engineeringModelSetup">
        /// The EngineeringModelSetup DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModelSetup engineeringModelSetup, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, engineeringModelSetup, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, engineeringModelSetup, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "AutoPublish", !this.IsDerived(engineeringModelSetup, "AutoPublish") ? engineeringModelSetup.AutoPublish.ToString() : string.Empty },
                    { "EngineeringModelIid", !this.IsDerived(engineeringModelSetup, "EngineeringModelIid") ? engineeringModelSetup.EngineeringModelIid.ToString() : string.Empty },
                    { "Kind", !this.IsDerived(engineeringModelSetup, "Kind") ? engineeringModelSetup.Kind.ToString() : string.Empty },
                    { "SourceEngineeringModelSetupIid", !this.IsDerived(engineeringModelSetup, "SourceEngineeringModelSetupIid") && engineeringModelSetup.SourceEngineeringModelSetupIid.HasValue ? engineeringModelSetup.SourceEngineeringModelSetupIid.Value.ToString() : null },
                    { "StudyPhase", !this.IsDerived(engineeringModelSetup, "StudyPhase") ? engineeringModelSetup.StudyPhase.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelSetup\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOrganizationalParticipant\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :defaultOrganizationalParticipant)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModelSetup.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOrganizationalParticipant", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelSetup, "DefaultOrganizationalParticipant") ? Utils.NullableValue(engineeringModelSetup.DefaultOrganizationalParticipant) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, engineeringModelSetup, container);
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
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that is to be deleted.
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
        /// Delete the supplied value from the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property name from where the value is to be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that is the source of each link table record.
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
                case "ActiveDomain":
                    {
                        isDeleted = this.DeleteActiveDomain(transaction, partition, iid, (Guid)value);
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
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="activeDomain">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteActiveDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid activeDomain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"EngineeringModelSetup_ActiveDomain\"", partition);
                sqlBuilder.Append(" WHERE \"EngineeringModelSetup\" = :engineeringModelSetup");
                sqlBuilder.Append(" AND \"ActiveDomain\" = :activeDomain;");

                command.Parameters.Add("engineeringModelSetup", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("activeDomain", NpgsqlDbType.Uuid).Value = activeDomain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="EngineeringModelSetupDao" />
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

            sqlBuilder.Append(" \"EngineeringModelSetup\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"EngineeringModelSetup\".\"DefaultOrganizationalParticipant\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Attachment\".\"Attachment\",'{}'::text[]) AS \"Attachment\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelSetup_ActiveDomain\".\"ActiveDomain\",'{}'::text[]) AS \"ActiveDomain\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelSetup_IterationSetup\".\"IterationSetup\",'{}'::text[]) AS \"IterationSetup\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelSetup_OrganizationalParticipant\".\"OrganizationalParticipant\",'{}'::text[]) AS \"OrganizationalParticipant\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelSetup_Participant\".\"Participant\",'{}'::text[]) AS \"Participant\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelSetup_RequiredRdl\".\"RequiredRdl\",'{}'::text[]) AS \"RequiredRdl\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" USING (\"Iid\")", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" USING (\"Iid\")", this.GetEngineeringModelSetupDataSql(partition, instant));

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

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Attachment\".\"Container\" AS \"Iid\", array_agg(\"Attachment\".\"Iid\"::text) AS \"Attachment\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Attachment\"", this.GetAttachmentDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"Attachment\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Attachment\".\"Container\") AS \"DefinedThing_Attachment\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Definition\".\"Container\" AS \"Iid\", array_agg(\"Definition\".\"Iid\"::text) AS \"Definition\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Definition\"", this.GetDefinitionDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"Definition\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Definition\".\"Container\") AS \"DefinedThing_Definition\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"HyperLink\".\"Container\" AS \"Iid\", array_agg(\"HyperLink\".\"Iid\"::text) AS \"HyperLink\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"HyperLink\"", this.GetHyperLinkDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"DefinedThing\" ON \"HyperLink\".\"Container\" = \"DefinedThing\".\"Iid\"", this.GetDefinedThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"HyperLink\".\"Container\") AS \"DefinedThing_HyperLink\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelSetup\" AS \"Iid\", array_agg(\"ActiveDomain\"::text) AS \"ActiveDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"EngineeringModelSetup_ActiveDomain\"", this.GetEngineeringModelSetup_ActiveDomainDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" ON \"EngineeringModelSetup\" = \"Iid\"", this.GetEngineeringModelSetupDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"EngineeringModelSetup\") AS \"EngineeringModelSetup_ActiveDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"IterationSetup\".\"Container\" AS \"Iid\", array_agg(\"IterationSetup\".\"Iid\"::text) AS \"IterationSetup\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"IterationSetup\"", this.GetIterationSetupDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" ON \"IterationSetup\".\"Container\" = \"EngineeringModelSetup\".\"Iid\"", this.GetEngineeringModelSetupDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"IterationSetup\".\"Container\") AS \"EngineeringModelSetup_IterationSetup\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"OrganizationalParticipant\".\"Container\" AS \"Iid\", array_agg(\"OrganizationalParticipant\".\"Iid\"::text) AS \"OrganizationalParticipant\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"OrganizationalParticipant\"", this.GetOrganizationalParticipantDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" ON \"OrganizationalParticipant\".\"Container\" = \"EngineeringModelSetup\".\"Iid\"", this.GetEngineeringModelSetupDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"OrganizationalParticipant\".\"Container\") AS \"EngineeringModelSetup_OrganizationalParticipant\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Participant\".\"Container\" AS \"Iid\", array_agg(\"Participant\".\"Iid\"::text) AS \"Participant\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Participant\"", this.GetParticipantDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" ON \"Participant\".\"Container\" = \"EngineeringModelSetup\".\"Iid\"", this.GetEngineeringModelSetupDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Participant\".\"Container\") AS \"EngineeringModelSetup_Participant\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModelReferenceDataLibrary\".\"Container\" AS \"Iid\", array_agg(\"ModelReferenceDataLibrary\".\"Iid\"::text) AS \"RequiredRdl\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"ModelReferenceDataLibrary\"", this.GetModelReferenceDataLibraryDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"EngineeringModelSetup\" ON \"ModelReferenceDataLibrary\".\"Container\" = \"EngineeringModelSetup\".\"Iid\"", this.GetEngineeringModelSetupDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"ModelReferenceDataLibrary\".\"Container\") AS \"EngineeringModelSetup_RequiredRdl\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelSetup_Audit\".\"Actor\", \"EngineeringModelSetup_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup_Audit\" AS \"EngineeringModelSetup_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"EngineeringModelSetup_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"EngineeringModelSetup_Audit\".\"Iid\", \"EngineeringModelSetup_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"EngineeringModelSetup\".\"ValueTypeDictionary\"";

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
        private string GetEngineeringModelSetupDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOrganizationalParticipant\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup_Audit\"", partition);
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
        private string GetAttachmentDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Attachment\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Attachment_Audit\"", partition);
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
        private string GetEngineeringModelSetup_ActiveDomainDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"EngineeringModelSetup\",\"ActiveDomain\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup_ActiveDomain\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelSetup_ActiveDomain_Audit\"", partition);
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
        private string GetIterationSetupDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"SourceIterationSetup\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"IterationSetup\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"IterationSetup_Audit\"", partition);
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
        private string GetOrganizationalParticipantDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Organization\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"OrganizationalParticipant\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"OrganizationalParticipant_Audit\"", partition);
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
        private string GetParticipantDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Person\", \"Role\", \"SelectedDomain\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Participant\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Participant_Audit\"", partition);
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
        private string GetModelReferenceDataLibraryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModelReferenceDataLibrary\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModelReferenceDataLibrary_Audit\"", partition);
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
