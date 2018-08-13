// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleVerificationDao.cs" company="RHEA System S.A.">
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
    /// The abstract RuleVerification Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class RuleVerificationDao : ThingDao
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
        /// <param name="ruleVerification">
        /// The ruleVerification DTO that is to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RuleVerification ruleVerification, long sequence, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, ruleVerification, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, ruleVerification, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "IsActive", !this.IsDerived(ruleVerification, "IsActive") ? ruleVerification.IsActive.ToString() : string.Empty },
                            { "ExecutedOn", !this.IsDerived(ruleVerification, "ExecutedOn") && ruleVerification.ExecutedOn.HasValue ? ruleVerification.ExecutedOn.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                            { "Status", !this.IsDerived(ruleVerification, "Status") ? ruleVerification.Status.ToString() : string.Empty },
                            { "Name", !this.IsDerived(ruleVerification, "Name") ? ruleVerification.Name.Escape() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerification\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Sequence\", \"Container\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :sequence, :container);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = ruleVerification.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = sequence;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, ruleVerification, container);
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
        /// <param name="ruleVerification">
        /// The ruleVerification DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.RuleVerification ruleVerification, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, ruleVerification, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, ruleVerification, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "IsActive", !this.IsDerived(ruleVerification, "IsActive") ? ruleVerification.IsActive.ToString() : string.Empty },
                            { "ExecutedOn", !this.IsDerived(ruleVerification, "ExecutedOn") && ruleVerification.ExecutedOn.HasValue ? ruleVerification.ExecutedOn.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                            { "Status", !this.IsDerived(ruleVerification, "Status") ? ruleVerification.Status.ToString() : string.Empty },
                            { "Name", !this.IsDerived(ruleVerification, "Name") ? ruleVerification.Name.Escape() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerification\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:container, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = ruleVerification.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, ruleVerification, container);
        }
 
        /// <summary>
        /// Update the containment order as indicated by the supplied orderedItem.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="orderedItem">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the contained item was successfully reordered.
        /// </returns>
        public override bool ReorderContainment(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            var isReordered = base.ReorderContainment(transaction, partition, orderedItem);
            
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
            
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerification\"", partition);
                sqlBuilder.AppendFormat(" SET (\"Sequence\")");
                sqlBuilder.AppendFormat(" = (:reorderSequence)");
                sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid");
                sqlBuilder.AppendFormat(" AND \"Sequence\" = :sequence;");
                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = (Guid)orderedItem.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = orderedItem.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = orderedItem.M;
            
                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                return isReordered && this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.RuleVerification"/> id that is to be deleted.
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
