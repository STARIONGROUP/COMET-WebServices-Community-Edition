// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterBaseDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated class. Any manual changes on this file will be overwritten!
// </summary>
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
        /// The current transaction to the database.
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
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"ParameterType\", \"Scale\", \"StateDependence\", \"Group\", \"Owner\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :parameterType, :scale, :stateDependence, :group, :owner);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("parameterType", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "ParameterType") ? parameterBase.ParameterType : Utils.NullableValue(null);
                    command.Parameters.Add("scale", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Scale") ? Utils.NullableValue(parameterBase.Scale) : Utils.NullableValue(null);
                    command.Parameters.Add("stateDependence", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "StateDependence") ? Utils.NullableValue(parameterBase.StateDependence) : Utils.NullableValue(null);
                    command.Parameters.Add("group", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Group") ? Utils.NullableValue(parameterBase.Group) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Owner") ? parameterBase.Owner : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, parameterBase, container);
        }
 
        /// <summary>
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="parameterBase">
        /// The parameterBase DTO that is to be updated.
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
                    sqlBuilder.AppendFormat(" SET (\"ParameterType\", \"Scale\", \"StateDependence\", \"Group\", \"Owner\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:parameterType, :scale, :stateDependence, :group, :owner, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterBase.Iid;
                    command.Parameters.Add("parameterType", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "ParameterType") ? parameterBase.ParameterType : Utils.NullableValue(null);
                    command.Parameters.Add("scale", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Scale") ? Utils.NullableValue(parameterBase.Scale) : Utils.NullableValue(null);
                    command.Parameters.Add("stateDependence", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "StateDependence") ? Utils.NullableValue(parameterBase.StateDependence) : Utils.NullableValue(null);
                    command.Parameters.Add("group", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Group") ? Utils.NullableValue(parameterBase.Group) : Utils.NullableValue(null);
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterBase, "Owner") ? parameterBase.Owner : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
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
        /// The current transaction to the database.
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
