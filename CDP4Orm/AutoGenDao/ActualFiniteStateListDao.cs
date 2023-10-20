// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActualFiniteStateListDao.cs" company="RHEA System S.A.">
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
    /// The ActualFiniteStateList Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ActualFiniteStateListDao : ThingDao, IActualFiniteStateListDao
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
        /// List of instances of <see cref="CDP4Common.DTO.ActualFiniteStateList"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.ActualFiniteStateList> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"ActualFiniteStateList_Cache\"", partition);
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
                                yield return thing as ActualFiniteStateList;
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
        /// A deserialized instance of <see cref="CDP4Common.DTO.ActualFiniteStateList"/>.
        /// </returns>
        public virtual CDP4Common.DTO.ActualFiniteStateList MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.ActualFiniteStateList(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.ActualState.AddRange(Array.ConvertAll((string[])reader["ActualState"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExcludeOption.AddRange(Array.ConvertAll((string[])reader["ExcludeOption"], Guid.Parse));
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            dto.PossibleFiniteStateList.AddRange(Utils.ParseOrderedList<Guid>(reader["PossibleFiniteStateList"] as string[,]));

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
        /// <param name="actualFiniteStateList">
        /// The actualFiniteStateList DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActualFiniteStateList actualFiniteStateList, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, actualFiniteStateList, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, actualFiniteStateList, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Container\", \"Owner\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :container, :owner);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actualFiniteStateList.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(actualFiniteStateList, "Owner") ? actualFiniteStateList.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
                actualFiniteStateList.ExcludeOption.ForEach(x => this.AddExcludeOption(transaction, partition, actualFiniteStateList.Iid, x));
                actualFiniteStateList.PossibleFiniteStateList.ForEach(x => this.AddPossibleFiniteStateList(transaction, partition, actualFiniteStateList.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, actualFiniteStateList, container);
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
        /// <param name="actualFiniteStateList">
        /// The actualFiniteStateList DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActualFiniteStateList actualFiniteStateList, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, actualFiniteStateList, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Container\", \"Owner\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :container, :owner)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actualFiniteStateList.Iid;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(actualFiniteStateList, "Owner") ? actualFiniteStateList.Owner : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"Container\", \"Owner\")");
                sqlBuilder.Append(" = (:container, :owner);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
            actualFiniteStateList.ExcludeOption.ForEach(x => this.UpsertExcludeOption(transaction, partition, actualFiniteStateList.Iid, x));
            actualFiniteStateList.PossibleFiniteStateList.ForEach(x => this.UpsertPossibleFiniteStateList(transaction, partition, actualFiniteStateList.Iid, x));

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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for each link table record.
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

                case "PossibleFiniteStateList":
                    {
                        isCreated = this.AddPossibleFiniteStateList(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for each link table record.
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
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_ExcludeOption\"", partition);
                sqlBuilder.AppendFormat(" (\"ActualFiniteStateList\", \"ExcludeOption\")");
                sqlBuilder.Append(" VALUES (:actualFiniteStateList, :excludeOption);");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("excludeOption", NpgsqlDbType.Uuid).Value = excludeOption;

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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="excludeOption">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertExcludeOption(NpgsqlTransaction transaction, string partition, Guid iid, Guid excludeOption)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_ExcludeOption\"", partition);
                sqlBuilder.AppendFormat(" (\"ActualFiniteStateList\", \"ExcludeOption\")");
                sqlBuilder.Append(" VALUES (:actualFiniteStateList, :excludeOption)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ActualFiniteStateList_ExcludeOption_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ActualFiniteStateList\", \"ExcludeOption\")");
                sqlBuilder.Append(" = (:actualFiniteStateList, :excludeOption);");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("excludeOption", NpgsqlDbType.Uuid).Value = excludeOption;

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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="possibleFiniteStateList">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddPossibleFiniteStateList(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem possibleFiniteStateList)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\"", partition);
                sqlBuilder.AppendFormat(" (\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:actualFiniteStateList, :possibleFiniteStateList, :sequence);");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("possibleFiniteStateList", NpgsqlDbType.Uuid).Value = Guid.Parse(possibleFiniteStateList.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = possibleFiniteStateList.K;

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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="possibleFiniteStateList">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertPossibleFiniteStateList(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem possibleFiniteStateList)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\"", partition);
                sqlBuilder.AppendFormat(" (\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:actualFiniteStateList, :possibleFiniteStateList, :sequence)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ActualFiniteStateList_PossibleFiniteStateList_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.AppendFormat(" SET (\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"Sequence\")");
                sqlBuilder.Append(" = (:actualFiniteStateList, :possibleFiniteStateList, :sequence);");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("possibleFiniteStateList", NpgsqlDbType.Uuid).Value = Guid.Parse(possibleFiniteStateList.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = possibleFiniteStateList.K;

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
        /// <param name="actualFiniteStateList">
        /// The ActualFiniteStateList DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActualFiniteStateList actualFiniteStateList, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, actualFiniteStateList, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, actualFiniteStateList, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"Owner\")");
                    sqlBuilder.AppendFormat(" = (:container, :owner)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actualFiniteStateList.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(actualFiniteStateList, "Owner") ? actualFiniteStateList.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, actualFiniteStateList, container);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is the source for the reordered link table record.
        /// </param> 
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public override bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            var isReordered = base.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);

            switch (propertyName)
            {
                case "PossibleFiniteStateList":
                    {
                        isReordered = this.ReorderPossibleFiniteStateList(transaction, partition, iid, orderUpdate);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isReordered;
        }

        /// <summary>
        /// Reorder an item in an association link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that will be the source for reordered link table record.
        /// </param> 
        /// <param name="possibleFiniteStateList">
        /// The value for which a link table record wil be reordered.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public bool ReorderPossibleFiniteStateList(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem possibleFiniteStateList)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.Append(" = :reorderSequence");
                sqlBuilder.Append(" WHERE \"ActualFiniteStateList\" = :actualFiniteStateList");
                sqlBuilder.Append(" AND \"PossibleFiniteStateList\" = :possibleFiniteStateList");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("possibleFiniteStateList", NpgsqlDbType.Uuid).Value = Guid.Parse(possibleFiniteStateList.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = possibleFiniteStateList.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = possibleFiniteStateList.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is the source of each link table record.
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

                case "PossibleFiniteStateList":
                    {
                        isDeleted = this.DeletePossibleFiniteStateList(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is the source for each link table record.
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
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ActualFiniteStateList_ExcludeOption\"", partition);
                sqlBuilder.Append(" WHERE \"ActualFiniteStateList\" = :actualFiniteStateList");
                sqlBuilder.Append(" AND \"ExcludeOption\" = :excludeOption;");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("excludeOption", NpgsqlDbType.Uuid).Value = excludeOption;

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
        /// The <see cref="CDP4Common.DTO.ActualFiniteStateList"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="possibleFiniteStateList">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeletePossibleFiniteStateList(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem possibleFiniteStateList)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\"", partition);
                sqlBuilder.Append(" WHERE \"ActualFiniteStateList\" = :actualFiniteStateList");
                sqlBuilder.Append(" AND \"PossibleFiniteStateList\" = :possibleFiniteStateList");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("actualFiniteStateList", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("possibleFiniteStateList", NpgsqlDbType.Uuid).Value = Guid.Parse(possibleFiniteStateList.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = possibleFiniteStateList.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="ActualFiniteStateListDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"ActualFiniteStateList\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"ActualFiniteStateList\".\"Owner\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"ActualFiniteStateList_ActualState\".\"ActualState\",'{}'::text[]) AS \"ActualState\",");
            sqlBuilder.Append(" COALESCE(\"ActualFiniteStateList_ExcludeOption\".\"ExcludeOption\",'{}'::text[]) AS \"ExcludeOption\",");
            sqlBuilder.Append(" COALESCE(\"ActualFiniteStateList_PossibleFiniteStateList\".\"PossibleFiniteStateList\",'{}'::text[]) AS \"PossibleFiniteStateList\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ActualFiniteStateList_Data\"() AS \"ActualFiniteStateList\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ActualFiniteState\".\"Container\" AS \"Iid\", array_agg(\"ActualFiniteState\".\"Iid\"::text) AS \"ActualState\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteState_Data\"() AS \"ActualFiniteState\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ActualFiniteStateList_Data\"() AS \"ActualFiniteStateList\" ON \"ActualFiniteState\".\"Container\" = \"ActualFiniteStateList\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ActualFiniteState\".\"Container\") AS \"ActualFiniteStateList_ActualState\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ActualFiniteStateList\" AS \"Iid\", array_agg(\"ExcludeOption\"::text) AS \"ExcludeOption\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteStateList_ExcludeOption_Data\"() AS \"ActualFiniteStateList_ExcludeOption\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ActualFiniteStateList_Data\"() AS \"ActualFiniteStateList\" ON \"ActualFiniteStateList\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ActualFiniteStateList\") AS \"ActualFiniteStateList_ExcludeOption\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ActualFiniteStateList\" AS \"Iid\", array_agg(\"PossibleFiniteStateList\"::text) AS \"PossibleFiniteStateList\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList_Data\"() AS \"ActualFiniteStateList_PossibleFiniteStateList\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ActualFiniteStateList_Data\"() AS \"ActualFiniteStateList\" ON \"ActualFiniteStateList\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ActualFiniteStateList\") AS \"ActualFiniteStateList_PossibleFiniteStateList\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"ActualFiniteStateList_Audit\".\"Actor\", \"ActualFiniteStateList_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ActualFiniteStateList_Audit\" AS \"ActualFiniteStateList_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"ActualFiniteStateList_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"ActualFiniteStateList_Audit\".\"Iid\", \"ActualFiniteStateList_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"ActualFiniteStateList\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
