// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
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
    /// The IterationSetup Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class IterationSetupDao : ThingDao, IIterationSetupDao
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
        /// List of instances of <see cref="CDP4Common.DTO.IterationSetup"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.IterationSetup> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"IterationSetup_Cache\"", partition);

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
                                yield return thing as IterationSetup;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"IterationSetup_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.IterationSetup"/>.
        /// </returns>
        public virtual CDP4Common.DTO.IterationSetup MapToDto(NpgsqlDataReader reader)
        {
            string tempCreatedOn;
            string tempDescription;
            string tempFrozenOn;
            string tempIsDeleted;
            string tempIterationIid;
            string tempIterationNumber;
            string tempModifiedOn;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.IterationSetup(iid, revisionNumber);
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.SourceIterationSetup = reader["SourceIterationSetup"] is DBNull ? (Guid?)null : Guid.Parse(reader["SourceIterationSetup"].ToString());

            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }

            if (valueDict.TryGetValue("Description", out tempDescription))
            {
                dto.Description = tempDescription.UnEscape();
            }

            if (valueDict.TryGetValue("FrozenOn", out tempFrozenOn) && tempFrozenOn != null)
            {
                dto.FrozenOn = Utils.ParseUtcDate(tempFrozenOn);
            }

            if (valueDict.TryGetValue("IsDeleted", out tempIsDeleted))
            {
                dto.IsDeleted = bool.Parse(tempIsDeleted);
            }

            if (valueDict.TryGetValue("IterationIid", out tempIterationIid))
            {
                dto.IterationIid = Guid.Parse(tempIterationIid);
            }

            if (valueDict.TryGetValue("IterationNumber", out tempIterationNumber))
            {
                dto.IterationNumber = int.Parse(tempIterationNumber);
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
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
        /// <param name="iterationSetup">
        /// The iterationSetup DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.IterationSetup iterationSetup, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, iterationSetup, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, iterationSetup, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(iterationSetup, "CreatedOn") ? iterationSetup.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                    { "Description", !this.IsDerived(iterationSetup, "Description") ? iterationSetup.Description.Escape() : string.Empty },
                    { "FrozenOn", !this.IsDerived(iterationSetup, "FrozenOn") && iterationSetup.FrozenOn.HasValue ? iterationSetup.FrozenOn.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                    { "IsDeleted", !this.IsDerived(iterationSetup, "IsDeleted") ? iterationSetup.IsDeleted.ToString() : string.Empty },
                    { "IterationIid", !this.IsDerived(iterationSetup, "IterationIid") ? iterationSetup.IterationIid.ToString() : string.Empty },
                    { "IterationNumber", iterationSetup.IterationNumber.ToString() },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"IterationSetup\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"SourceIterationSetup\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :sourceIterationSetup);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iterationSetup.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("sourceIterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iterationSetup, "SourceIterationSetup") ? Utils.NullableValue(iterationSetup.SourceIterationSetup) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, iterationSetup, container);
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
        /// <param name="iterationSetup">
        /// The IterationSetup DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.IterationSetup iterationSetup, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, iterationSetup, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, iterationSetup, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CreatedOn", !this.IsDerived(iterationSetup, "CreatedOn") ? iterationSetup.CreatedOn.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                    { "Description", !this.IsDerived(iterationSetup, "Description") ? iterationSetup.Description.Escape() : string.Empty },
                    { "FrozenOn", !this.IsDerived(iterationSetup, "FrozenOn") && iterationSetup.FrozenOn.HasValue ? iterationSetup.FrozenOn.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                    { "IsDeleted", !this.IsDerived(iterationSetup, "IsDeleted") ? iterationSetup.IsDeleted.ToString() : string.Empty },
                    { "IterationIid", !this.IsDerived(iterationSetup, "IterationIid") ? iterationSetup.IterationIid.ToString() : string.Empty },
                    { "IterationNumber", iterationSetup.IterationNumber.ToString() },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"IterationSetup\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"SourceIterationSetup\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :sourceIterationSetup)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iterationSetup.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("sourceIterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iterationSetup, "SourceIterationSetup") ? Utils.NullableValue(iterationSetup.SourceIterationSetup) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, iterationSetup, container);
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
        /// The <see cref="CDP4Common.DTO.IterationSetup"/> id that is to be deleted.
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
