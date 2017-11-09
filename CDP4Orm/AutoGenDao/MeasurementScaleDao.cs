// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleDao.cs" company="RHEA System S.A.">
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
    /// The abstract MeasurementScale Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class MeasurementScaleDao : DefinedThingDao
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
        /// <param name="measurementScale">
        /// The measurementScale DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.MeasurementScale measurementScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, measurementScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, measurementScale, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "NumberSet", !this.IsDerived(measurementScale, "NumberSet") ? measurementScale.NumberSet.ToString() : string.Empty },
                            { "MinimumPermissibleValue", !this.IsDerived(measurementScale, "MinimumPermissibleValue") ? measurementScale.MinimumPermissibleValue.Escape() : null },
                            { "IsMinimumInclusive", !this.IsDerived(measurementScale, "IsMinimumInclusive") ? measurementScale.IsMinimumInclusive.ToString() : string.Empty },
                            { "MaximumPermissibleValue", !this.IsDerived(measurementScale, "MaximumPermissibleValue") ? measurementScale.MaximumPermissibleValue.Escape() : null },
                            { "IsMaximumInclusive", !this.IsDerived(measurementScale, "IsMaximumInclusive") ? measurementScale.IsMaximumInclusive.ToString() : string.Empty },
                            { "PositiveValueConnotation", !this.IsDerived(measurementScale, "PositiveValueConnotation") ? measurementScale.PositiveValueConnotation.Escape() : null },
                            { "NegativeValueConnotation", !this.IsDerived(measurementScale, "NegativeValueConnotation") ? measurementScale.NegativeValueConnotation.Escape() : null },
                            { "IsDeprecated", !this.IsDerived(measurementScale, "IsDeprecated") ? measurementScale.IsDeprecated.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MeasurementScale\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"Unit\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :unit);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = measurementScale.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("unit", NpgsqlDbType.Uuid).Value = !this.IsDerived(measurementScale, "Unit") ? measurementScale.Unit : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, measurementScale, container);
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
        /// <param name="measurementScale">
        /// The measurementScale DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.MeasurementScale measurementScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, measurementScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, measurementScale, container);
                
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "NumberSet", !this.IsDerived(measurementScale, "NumberSet") ? measurementScale.NumberSet.ToString() : string.Empty },
                            { "MinimumPermissibleValue", !this.IsDerived(measurementScale, "MinimumPermissibleValue") ? measurementScale.MinimumPermissibleValue.Escape() : null },
                            { "IsMinimumInclusive", !this.IsDerived(measurementScale, "IsMinimumInclusive") ? measurementScale.IsMinimumInclusive.ToString() : string.Empty },
                            { "MaximumPermissibleValue", !this.IsDerived(measurementScale, "MaximumPermissibleValue") ? measurementScale.MaximumPermissibleValue.Escape() : null },
                            { "IsMaximumInclusive", !this.IsDerived(measurementScale, "IsMaximumInclusive") ? measurementScale.IsMaximumInclusive.ToString() : string.Empty },
                            { "PositiveValueConnotation", !this.IsDerived(measurementScale, "PositiveValueConnotation") ? measurementScale.PositiveValueConnotation.Escape() : null },
                            { "NegativeValueConnotation", !this.IsDerived(measurementScale, "NegativeValueConnotation") ? measurementScale.NegativeValueConnotation.Escape() : null },
                            { "IsDeprecated", !this.IsDerived(measurementScale, "IsDeprecated") ? measurementScale.IsDeprecated.ToString() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MeasurementScale\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"Unit\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:container, :unit, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = measurementScale.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("unit", NpgsqlDbType.Uuid).Value = !this.IsDerived(measurementScale, "Unit") ? measurementScale.Unit : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, measurementScale, container);
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
        /// The <see cref="CDP4Common.DTO.MeasurementScale"/> id that is to be deleted.
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
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    var valueTypeDictionaryContents = new Dictionary<string, string>
                            {
                                { "IsDeprecated", "true" },
                            };
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MeasurementScale\"", partition);
                    sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = \"ValueTypeDictionary\" || :valueTypeDictionary");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    isHandled = this.ExecuteAndLogCommand(command) > 0;
                }
            }

            return this.AfterDelete(beforeDelete, transaction, partition, iid);
        }
    }
}
