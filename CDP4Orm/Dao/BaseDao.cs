// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using Newtonsoft.Json.Linq;

    using Npgsql;

    /// <summary>
    /// The base class for all Dao classes
    /// </summary>
    public abstract class BaseDao
    {
        /// <summary>
        /// Gets or sets the DataModel Utils helper.
        /// </summary>
        public IDataModelUtils DataModelUtils { get; set; }

        /// <summary>
        /// Gets or sets the Command logger.
        /// </summary>
        public ICommandLogger CommandLogger { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> of the current <see cref="NpgsqlTransaction"/>
        /// </summary>
        private DateTime currentTransactionDatetime;

        /// <summary>
        /// The <see cref="NpgsqlTransaction"/> for which <see cref="currentTransactionDatetime"/> was retrieved
        /// </summary>
        private NpgsqlTransaction currentTransactionDataTimeTransaction;

        /// <summary>
        /// Execute additional logic before each update function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <param name="isHandled">
        /// Logic flag that can be set to true to skip the generated update logic
        /// </param>
        /// <param name="valueTypeDictionaryAdditions">
        /// This dictionary instance can be used to add additional value (non reference) variables that are to be persisted as a HSTORE implementation together with the supplied <see cref="Thing"/> instance.
        /// Developers are required to take care to add property keys that are not already in the ValueTypeDictionary that is managed in the respective generated partial class.
        /// The supplied values will be persisted as is and thus must be in a valid format (escaped) that can be persisted to SQL.
        /// Even though the additional stored variables are read from the data-store, developers will have to add custom DTO mapping to get retrieve the value again. 
        /// </param>
        /// <returns>
        /// True if the concept was persisted.
        /// </returns>
        public virtual bool BeforeUpdate(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, out bool isHandled, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            var transactionDateTime = this.GetTransactionDateTime(transaction);
            thing.ModifiedOn = transactionDateTime;

            isHandled = false;
            return true;
        }

        /// <summary>
        /// Execute additional logic after each update function call.
        /// </summary>
        /// <param name="updateResult">
        /// The result of the update function.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that has been updated.
        /// </param>
        /// <param name="container">
        /// The container of the updated DTO.
        /// </param>
        /// <returns>
        /// True if the after update logic and the update result were both successful.
        /// </returns>
        public virtual bool AfterUpdate(bool updateResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            return updateResult;
        }

        /// <summary>
        /// Execute additional logic before each delete function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The thing DTO id that is to be deleted.
        /// </param>
        /// <param name="isHandled">
        /// Logic flag that can be set to true to skip the generated deleted logic
        /// </param>
        /// <returns>
        /// True if the concept was deleted.
        /// </returns>
        public virtual bool BeforeDelete(NpgsqlTransaction transaction, string partition, Guid iid, out bool isHandled)
        {
            isHandled = false;
            return true;
        }

        /// <summary>
        /// Execute additional logic after each delete function call.
        /// </summary>
        /// <param name="deleteResult">
        /// The result of the delete function.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The thing DTO id that has been deleted.
        /// </param>
        /// <returns>
        /// True if the after delete logic and the delete result were both successful.
        /// </returns>
        public virtual bool AfterDelete(bool deleteResult, NpgsqlTransaction transaction, string partition, Guid iid)
        {
            return deleteResult;
        }

        /// <summary>
        /// Execute additional logic before each write function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <param name="isHandled">
        /// Logic flag that can be set to true to skip the generated write logic
        /// </param>
        /// <param name="valueTypeDictionaryAdditions">
        /// This dictionary instance can be used to add additional value (non reference) variables that are to be persisted as a HSTORE implementation together with the supplied <see cref="Thing"/> instance.
        /// Developers are required to take care to add property keys that are not already in the ValueTypeDictionary that is managed in the respective generated partial class.
        /// The supplied values will be persisted as is and thus must be in a valid format (escaped) that can be persisted to SQL.
        /// Even though the additional stored variables are read from the data-store, developers will have to add custom DTO mapping to get retrieve the value again. 
        /// </param>
        /// <returns>
        /// True if the concept was persisted.
        /// </returns>
        public virtual bool BeforeWrite(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, out bool isHandled, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            thing.ModifiedOn = this.GetTransactionDateTime(transaction);

            isHandled = false;
            return true;
        }

        /// <summary>
        /// Execute additional logic after each write function call.
        /// </summary>
        /// <param name="writeResult">
        /// The result of the write function.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is has been persisted.
        /// </param>
        /// <param name="container">
        /// The container of the persisted DTO.
        /// </param>
        /// <returns>
        /// True if the after write logic and the write result were both successful.
        /// </returns>
        public virtual bool AfterWrite(bool writeResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            return writeResult;
        }

        /// <summary>
        /// Method to check if the property of the passed in object is derived.
        /// </summary>
        /// <param name="inspectObject">
        /// The object that holds the property.
        /// </param>
        /// <param name="propertyName">
        /// The property to check.
        /// </param>
        /// <returns>
        /// True if derived.
        /// </returns>
        public bool IsDerived(Thing inspectObject, string propertyName)
        {
            return this.DataModelUtils.IsDerived(inspectObject.GetType().Name, propertyName);
        }

        /// <summary>
        /// Log the command message if a logger is injected
        /// </summary>
        /// <param name="command">the command to log</param>
        protected void LogCommand(NpgsqlCommand command)
        {
            if (this.CommandLogger == null)
            {
                return;
            }

            this.CommandLogger.Log(command);
        }

        /// <summary>
        /// Log the command message if a logger is injected
        /// </summary>
        /// <param name="command">
        /// the command to log
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int ExecuteAndLogCommand(NpgsqlCommand command)
        {
            if (this.CommandLogger == null)
            {
                return 0;
            }

            return command.ExecuteAndLogNonQuery(this.CommandLogger);
        }

        /// <summary>
        /// Deletes all data from the <paramref name="table"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition of the table to delete</param>
        /// <param name="table">The table to clear</param>
        protected void DeleteAll(NpgsqlTransaction transaction, string partition, string table)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"{1}\";", partition, table);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                this.ExecuteAndLogCommand(command);
            }
        }

        /// <summary>
        /// Returns the current transaction time from the server
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private DateTime GetTransactionDateTime(NpgsqlTransaction transaction)
        {
            if (transaction != null && this.currentTransactionDataTimeTransaction != transaction)
            {
                this.currentTransactionDataTimeTransaction = transaction;

                using (var command = new NpgsqlCommand(
                    $"SELECT * FROM \"SiteDirectory\".\"get_transaction_time\"();",
                    transaction.Connection,
                    transaction))
                {
                    this.currentTransactionDatetime = (DateTime)command.ExecuteScalar();
                }
            }

            return this.currentTransactionDatetime;
        }

        /// <summary>
        /// Instantiates a <see cref="Thing"/> from the content of a <see cref="NpgsqlDataReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="NpgsqlDataReader"/></param>
        /// <returns>A <see cref="Thing"/></returns>
        protected Thing MapJsonbToDto(NpgsqlDataReader reader)
        {
            var jsonObject = JObject.Parse(reader.GetValue(0).ToString());

            Thing thing = null;

            try
            {
                thing = jsonObject.ToDto();
            }
            catch (Exception e)
            {
                thing = null;
            }

            return thing;
        }
    }
}