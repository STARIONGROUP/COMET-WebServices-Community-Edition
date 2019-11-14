// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleDao.cs" company="RHEA System S.A.">
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
    /// The abstract MeasurementScale Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class MeasurementScaleDao : DefinedThingDao
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
                    { "IsDeprecated", !this.IsDerived(measurementScale, "IsDeprecated") ? measurementScale.IsDeprecated.ToString() : string.Empty },
                    { "IsMaximumInclusive", !this.IsDerived(measurementScale, "IsMaximumInclusive") ? measurementScale.IsMaximumInclusive.ToString() : string.Empty },
                    { "IsMinimumInclusive", !this.IsDerived(measurementScale, "IsMinimumInclusive") ? measurementScale.IsMinimumInclusive.ToString() : string.Empty },
                    { "MaximumPermissibleValue", !this.IsDerived(measurementScale, "MaximumPermissibleValue") ? measurementScale.MaximumPermissibleValue.Escape() : null },
                    { "MinimumPermissibleValue", !this.IsDerived(measurementScale, "MinimumPermissibleValue") ? measurementScale.MinimumPermissibleValue.Escape() : null },
                    { "NegativeValueConnotation", !this.IsDerived(measurementScale, "NegativeValueConnotation") ? measurementScale.NegativeValueConnotation.Escape() : null },
                    { "NumberSet", !this.IsDerived(measurementScale, "NumberSet") ? measurementScale.NumberSet.ToString() : string.Empty },
                    { "PositiveValueConnotation", !this.IsDerived(measurementScale, "PositiveValueConnotation") ? measurementScale.PositiveValueConnotation.Escape() : null },
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
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="measurementScale">
        /// The MeasurementScale DTO that is to be updated.
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
                    { "IsDeprecated", !this.IsDerived(measurementScale, "IsDeprecated") ? measurementScale.IsDeprecated.ToString() : string.Empty },
                    { "IsMaximumInclusive", !this.IsDerived(measurementScale, "IsMaximumInclusive") ? measurementScale.IsMaximumInclusive.ToString() : string.Empty },
                    { "IsMinimumInclusive", !this.IsDerived(measurementScale, "IsMinimumInclusive") ? measurementScale.IsMinimumInclusive.ToString() : string.Empty },
                    { "MaximumPermissibleValue", !this.IsDerived(measurementScale, "MaximumPermissibleValue") ? measurementScale.MaximumPermissibleValue.Escape() : null },
                    { "MinimumPermissibleValue", !this.IsDerived(measurementScale, "MinimumPermissibleValue") ? measurementScale.MinimumPermissibleValue.Escape() : null },
                    { "NegativeValueConnotation", !this.IsDerived(measurementScale, "NegativeValueConnotation") ? measurementScale.NegativeValueConnotation.Escape() : null },
                    { "NumberSet", !this.IsDerived(measurementScale, "NumberSet") ? measurementScale.NumberSet.ToString() : string.Empty },
                    { "PositiveValueConnotation", !this.IsDerived(measurementScale, "PositiveValueConnotation") ? measurementScale.PositiveValueConnotation.Escape() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MeasurementScale\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"Unit\")");
                    sqlBuilder.AppendFormat(" = (\"ValueTypeDictionary\" || :valueTypeDictionary, :container, :unit)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

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

            return this.AfterUpdate(beforeUpdate, transaction, partition, measurementScale, container);
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
                                { "IsDeprecated", "true" }
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
