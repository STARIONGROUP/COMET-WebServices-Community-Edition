// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetBaseDao.cs" company="RHEA System S.A.">
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
    /// The abstract ParameterValueSetBase Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ParameterValueSetBaseDao : ThingDao
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
                    { "Computed", !this.IsDerived(parameterValueSetBase, "Computed") ? parameterValueSetBase.Computed.ToHstoreString() : string.Empty },
                    { "Formula", !this.IsDerived(parameterValueSetBase, "Formula") ? parameterValueSetBase.Formula.ToHstoreString() : string.Empty },
                    { "Manual", !this.IsDerived(parameterValueSetBase, "Manual") ? parameterValueSetBase.Manual.ToHstoreString() : string.Empty },
                    { "Published", !this.IsDerived(parameterValueSetBase, "Published") ? parameterValueSetBase.Published.ToHstoreString() : string.Empty },
                    { "Reference", !this.IsDerived(parameterValueSetBase, "Reference") ? parameterValueSetBase.Reference.ToHstoreString() : string.Empty },
                    { "ValueSwitch", !this.IsDerived(parameterValueSetBase, "ValueSwitch") ? parameterValueSetBase.ValueSwitch.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"ActualOption\", \"ActualState\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :actualOption, :actualState);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterValueSetBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("actualOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualOption") ? Utils.NullableValue(parameterValueSetBase.ActualOption) : Utils.NullableValue(null);
                    command.Parameters.Add("actualState", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualState") ? Utils.NullableValue(parameterValueSetBase.ActualState) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, parameterValueSetBase, container);
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
        /// <param name="parameterValueSetBase">
        /// The parameterValueSetBase DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ParameterValueSetBase parameterValueSetBase, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, parameterValueSetBase, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "Computed", !this.IsDerived(parameterValueSetBase, "Computed") ? parameterValueSetBase.Computed.ToHstoreString() : string.Empty },
                { "Formula", !this.IsDerived(parameterValueSetBase, "Formula") ? parameterValueSetBase.Formula.ToHstoreString() : string.Empty },
                { "Manual", !this.IsDerived(parameterValueSetBase, "Manual") ? parameterValueSetBase.Manual.ToHstoreString() : string.Empty },
                { "Published", !this.IsDerived(parameterValueSetBase, "Published") ? parameterValueSetBase.Published.ToHstoreString() : string.Empty },
                { "Reference", !this.IsDerived(parameterValueSetBase, "Reference") ? parameterValueSetBase.Reference.ToHstoreString() : string.Empty },
                { "ValueSwitch", !this.IsDerived(parameterValueSetBase, "ValueSwitch") ? parameterValueSetBase.ValueSwitch.ToString() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"ActualOption\", \"ActualState\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :actualOption, :actualState)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterValueSetBase.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("actualOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualOption") ? Utils.NullableValue(parameterValueSetBase.ActualOption) : Utils.NullableValue(null);
                command.Parameters.Add("actualState", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualState") ? Utils.NullableValue(parameterValueSetBase.ActualState) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"ActualOption\", \"ActualState\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :actualOption, :actualState);");

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
        /// <param name="parameterValueSetBase">
        /// The ParameterValueSetBase DTO that is to be updated.
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
                    { "Computed", !this.IsDerived(parameterValueSetBase, "Computed") ? parameterValueSetBase.Computed.ToHstoreString() : string.Empty },
                    { "Formula", !this.IsDerived(parameterValueSetBase, "Formula") ? parameterValueSetBase.Formula.ToHstoreString() : string.Empty },
                    { "Manual", !this.IsDerived(parameterValueSetBase, "Manual") ? parameterValueSetBase.Manual.ToHstoreString() : string.Empty },
                    { "Published", !this.IsDerived(parameterValueSetBase, "Published") ? parameterValueSetBase.Published.ToHstoreString() : string.Empty },
                    { "Reference", !this.IsDerived(parameterValueSetBase, "Reference") ? parameterValueSetBase.Reference.ToHstoreString() : string.Empty },
                    { "ValueSwitch", !this.IsDerived(parameterValueSetBase, "ValueSwitch") ? parameterValueSetBase.ValueSwitch.ToString() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSetBase\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"ActualOption\", \"ActualState\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :actualOption, :actualState)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = parameterValueSetBase.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("actualOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualOption") ? Utils.NullableValue(parameterValueSetBase.ActualOption) : Utils.NullableValue(null);
                    command.Parameters.Add("actualState", NpgsqlDbType.Uuid).Value = !this.IsDerived(parameterValueSetBase, "ActualState") ? Utils.NullableValue(parameterValueSetBase.ActualState) : Utils.NullableValue(null);

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
        /// The current <see cref="NpgsqlTransaction"/> to the database.
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
        /// The <see cref="CDP4Common.DTO.ParameterValueSetBase"/> id that is to be deleted.
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
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
