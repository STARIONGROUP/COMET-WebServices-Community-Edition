// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetBaseDao.cs" company="RHEA System S.A.">
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
    using CDP4JsonSerializer;
    using Npgsql;
    using NpgsqlTypes;
 
    /// <summary>
    /// The abstract ParameterValueSetBase Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ParameterValueSetBaseDao : ThingDao
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
        /// <param name="parameterValueSetBase">
        /// The parameterValueSetBase DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterValueSetBase parameterValueSetBase, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, parameterValueSetBase, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, parameterValueSetBase, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "Published", !this.IsDerived(parameterValueSetBase, "Published") ? parameterValueSetBase.Published.ToHstoreString() : string.Empty },
                            { "Formula", !this.IsDerived(parameterValueSetBase, "Formula") ? parameterValueSetBase.Formula.ToHstoreString() : string.Empty },
                            { "Computed", !this.IsDerived(parameterValueSetBase, "Computed") ? parameterValueSetBase.Computed.ToHstoreString() : string.Empty },
                            { "Manual", !this.IsDerived(parameterValueSetBase, "Manual") ? parameterValueSetBase.Manual.ToHstoreString() : string.Empty },
                            { "Reference", !this.IsDerived(parameterValueSetBase, "Reference") ? parameterValueSetBase.Reference.ToHstoreString() : string.Empty },
                            { "ValueSwitch", !this.IsDerived(parameterValueSetBase, "ValueSwitch") ? parameterValueSetBase.ValueSwitch.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"ActualState\", \"ActualOption\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :actualState, :actualOption);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterValueSetBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("actualState", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualState") ? Utils.NullableValue(parameterValueSetBase.ActualState) : Utils.NullableValue(null);
                    command.Parameters.Add("actualOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualOption") ? Utils.NullableValue(parameterValueSetBase.ActualOption) : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, parameterValueSetBase, container);
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
        /// <param name="parameterValueSetBase">
        /// The parameterValueSetBase DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterValueSetBase parameterValueSetBase, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, parameterValueSetBase, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, parameterValueSetBase, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "Published", !this.IsDerived(parameterValueSetBase, "Published") ? parameterValueSetBase.Published.ToHstoreString() : string.Empty },
                            { "Formula", !this.IsDerived(parameterValueSetBase, "Formula") ? parameterValueSetBase.Formula.ToHstoreString() : string.Empty },
                            { "Computed", !this.IsDerived(parameterValueSetBase, "Computed") ? parameterValueSetBase.Computed.ToHstoreString() : string.Empty },
                            { "Manual", !this.IsDerived(parameterValueSetBase, "Manual") ? parameterValueSetBase.Manual.ToHstoreString() : string.Empty },
                            { "Reference", !this.IsDerived(parameterValueSetBase, "Reference") ? parameterValueSetBase.Reference.ToHstoreString() : string.Empty },
                            { "ValueSwitch", !this.IsDerived(parameterValueSetBase, "ValueSwitch") ? parameterValueSetBase.ValueSwitch.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSetBase\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ActualState\", \"ActualOption\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:actualState, :actualOption, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterValueSetBase.Iid;
                    command.Parameters.Add("actualState", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualState") ? Utils.NullableValue(parameterValueSetBase.ActualState) : Utils.NullableValue(null);
                    command.Parameters.Add("actualOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualOption") ? Utils.NullableValue(parameterValueSetBase.ActualOption) : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, parameterValueSetBase, container);
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
        /// The <see cref="CDP4Common.DTO.ParameterValueSetBase"/> id that is to be deleted.
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
