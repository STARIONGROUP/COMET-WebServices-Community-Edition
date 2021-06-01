// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterBaseDao.cs" company="RHEA System S.A.">
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
    /// The abstract ParameterBase Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ParameterBaseDao : ThingDao
    {

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="parameterBase">
        /// The parameterBase DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterBase parameterBase, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, parameterBase, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, parameterBase, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "IsOptionDependent", !this.IsDerived(parameterBase, "IsOptionDependent") ? parameterBase.IsOptionDependent.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterBase\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :group, :owner, :parameterType, :scale, :stateDependence);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("group", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Group") ? Utils.NullableValue(parameterBase.Group) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Owner") ? parameterBase.Owner : Utils.NullableValue(null);
                    command.Parameters.Add("parameterType", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "ParameterType") ? parameterBase.ParameterType : Utils.NullableValue(null);
                    command.Parameters.Add("scale", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Scale") ? Utils.NullableValue(parameterBase.Scale) : Utils.NullableValue(null);
                    command.Parameters.Add("stateDependence", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "StateDependence") ? Utils.NullableValue(parameterBase.StateDependence) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, parameterBase, container);
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
        /// <param name="parameterBase">
        /// The parameterBase DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterBase parameterBase, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, parameterBase, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "IsOptionDependent", !this.IsDerived(parameterBase, "IsOptionDependent") ? parameterBase.IsOptionDependent.ToString() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterBase\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :group, :owner, :parameterType, :scale, :stateDependence)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterBase.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("group", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Group") ? Utils.NullableValue(parameterBase.Group) : Utils.NullableValue(null);
                command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Owner") ? parameterBase.Owner : Utils.NullableValue(null);
                command.Parameters.Add("parameterType", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "ParameterType") ? parameterBase.ParameterType : Utils.NullableValue(null);
                command.Parameters.Add("scale", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Scale") ? Utils.NullableValue(parameterBase.Scale) : Utils.NullableValue(null);
                command.Parameters.Add("stateDependence", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "StateDependence") ? Utils.NullableValue(parameterBase.StateDependence) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :group, :owner, :parameterType, :scale, :stateDependence);");

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
        /// <param name="parameterBase">
        /// The ParameterBase DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterBase parameterBase, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, parameterBase, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, parameterBase, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "IsOptionDependent", !this.IsDerived(parameterBase, "IsOptionDependent") ? parameterBase.IsOptionDependent.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterBase\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :group, :owner, :parameterType, :scale, :stateDependence)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("group", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Group") ? Utils.NullableValue(parameterBase.Group) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Owner") ? parameterBase.Owner : Utils.NullableValue(null);
                    command.Parameters.Add("parameterType", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "ParameterType") ? parameterBase.ParameterType : Utils.NullableValue(null);
                    command.Parameters.Add("scale", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Scale") ? Utils.NullableValue(parameterBase.Scale) : Utils.NullableValue(null);
                    command.Parameters.Add("stateDependence", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "StateDependence") ? Utils.NullableValue(parameterBase.StateDependence) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, parameterBase, container);
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
        /// The <see cref="CDP4Common.DTO.ParameterBase"/> id that is to be deleted.
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
