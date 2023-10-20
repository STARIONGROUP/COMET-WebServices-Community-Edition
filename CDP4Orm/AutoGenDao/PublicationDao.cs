// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationDao.cs" company="RHEA System S.A.">
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
    /// The Publication Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class PublicationDao : ThingDao, IPublicationDao
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
        /// List of instances of <see cref="CDP4Common.DTO.Publication"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.Publication> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"Publication_Cache\"", partition);
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
                                yield return thing as Publication;
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.Publication"/>.
        /// </returns>
        public virtual CDP4Common.DTO.Publication MapToDto(NpgsqlDataReader reader)
        {
            string tempCreatedOn;
            string tempModifiedOn;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.Publication(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.Domain.AddRange(Array.ConvertAll((string[])reader["Domain"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.PublishedParameter.AddRange(Array.ConvertAll((string[])reader["PublishedParameter"], Guid.Parse));

            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
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
        /// <param name="publication">
        /// The publication DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Publication publication, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, publication, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, publication, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(publication, "CreatedOn") ? publication.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = publication.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
                publication.Domain.ForEach(x => this.AddDomain(transaction, partition, publication.Iid, x));
                publication.PublishedParameter.ForEach(x => this.AddPublishedParameter(transaction, partition, publication.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, publication, container);
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
        /// <param name="publication">
        /// The publication DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Publication publication, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, publication, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "CreatedOn", !this.IsDerived(publication, "CreatedOn") ? publication.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = publication.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
            publication.Domain.ForEach(x => this.UpsertDomain(transaction, partition, publication.Iid, x));
            publication.PublishedParameter.ForEach(x => this.UpsertPublishedParameter(transaction, partition, publication.Iid, x));

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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that will be the source for each link table record.
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
                case "Domain":
                    {
                        isCreated = this.AddDomain(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "PublishedParameter":
                    {
                        isCreated = this.AddPublishedParameter(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="domain">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid domain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_Domain\"", partition);
                sqlBuilder.AppendFormat(" (\"Publication\", \"Domain\")");
                sqlBuilder.Append(" VALUES (:publication, :domain);");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("domain", NpgsqlDbType.Uuid).Value = domain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="domain">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid domain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_Domain\"", partition);
                sqlBuilder.AppendFormat(" (\"Publication\", \"Domain\")");
                sqlBuilder.Append(" VALUES (:publication, :domain)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"Publication_Domain_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"Publication\", \"Domain\")");
                sqlBuilder.Append(" = (:publication, :domain);");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("domain", NpgsqlDbType.Uuid).Value = domain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="publishedParameter">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddPublishedParameter(NpgsqlTransaction transaction, string partition, Guid iid, Guid publishedParameter)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_PublishedParameter\"", partition);
                sqlBuilder.AppendFormat(" (\"Publication\", \"PublishedParameter\")");
                sqlBuilder.Append(" VALUES (:publication, :publishedParameter);");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("publishedParameter", NpgsqlDbType.Uuid).Value = publishedParameter;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="publishedParameter">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertPublishedParameter(NpgsqlTransaction transaction, string partition, Guid iid, Guid publishedParameter)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_PublishedParameter\"", partition);
                sqlBuilder.AppendFormat(" (\"Publication\", \"PublishedParameter\")");
                sqlBuilder.Append(" VALUES (:publication, :publishedParameter)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"Publication_PublishedParameter_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"Publication\", \"PublishedParameter\")");
                sqlBuilder.Append(" = (:publication, :publishedParameter);");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("publishedParameter", NpgsqlDbType.Uuid).Value = publishedParameter;

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
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="publication">
        /// The Publication DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Publication publication, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, publication, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, publication, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(publication, "CreatedOn") ? publication.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = publication.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, publication, container);
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that is the source of each link table record.
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
                case "Domain":
                    {
                        isDeleted = this.DeleteDomain(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "PublishedParameter":
                    {
                        isDeleted = this.DeletePublishedParameter(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="domain">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteDomain(NpgsqlTransaction transaction, string partition, Guid iid, Guid domain)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Publication_Domain\"", partition);
                sqlBuilder.Append(" WHERE \"Publication\" = :publication");
                sqlBuilder.Append(" AND \"Domain\" = :domain;");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("domain", NpgsqlDbType.Uuid).Value = domain;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.Publication"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="publishedParameter">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeletePublishedParameter(NpgsqlTransaction transaction, string partition, Guid iid, Guid publishedParameter)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Publication_PublishedParameter\"", partition);
                sqlBuilder.Append(" WHERE \"Publication\" = :publication");
                sqlBuilder.Append(" AND \"PublishedParameter\" = :publishedParameter;");

                command.Parameters.Add("publication", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("publishedParameter", NpgsqlDbType.Uuid).Value = publishedParameter;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="PublicationDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Publication\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"Publication_Domain\".\"Domain\",'{}'::text[]) AS \"Domain\",");
            sqlBuilder.Append(" COALESCE(\"Publication_PublishedParameter\".\"PublishedParameter\",'{}'::text[]) AS \"PublishedParameter\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Publication_Data\"() AS \"Publication\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Publication\" AS \"Iid\", array_agg(\"Domain\"::text) AS \"Domain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Publication_Domain_Data\"() AS \"Publication_Domain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Publication_Data\"() AS \"Publication\" ON \"Publication\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Publication\") AS \"Publication_Domain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Publication\" AS \"Iid\", array_agg(\"PublishedParameter\"::text) AS \"PublishedParameter\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Publication_PublishedParameter_Data\"() AS \"Publication_PublishedParameter\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Publication_Data\"() AS \"Publication\" ON \"Publication\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Publication\") AS \"Publication_PublishedParameter\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"Publication_Audit\".\"Actor\", \"Publication_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Publication_Audit\" AS \"Publication_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"Publication_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"Publication_Audit\".\"Iid\", \"Publication_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"Publication\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
