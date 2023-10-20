// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleParameterizableThingDao.cs" company="RHEA System S.A.">
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
    /// The abstract SimpleParameterizableThing Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class SimpleParameterizableThingDao : DefinedThingDao
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
        /// <param name="simpleParameterizableThing">
        /// The simpleParameterizableThing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SimpleParameterizableThing simpleParameterizableThing, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, simpleParameterizableThing, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, simpleParameterizableThing, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterizableThing\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Owner\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :owner);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = simpleParameterizableThing.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(simpleParameterizableThing, "Owner") ? simpleParameterizableThing.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, simpleParameterizableThing, container);
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
        /// <param name="simpleParameterizableThing">
        /// The simpleParameterizableThing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SimpleParameterizableThing simpleParameterizableThing, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, simpleParameterizableThing, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterizableThing\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Owner\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :owner)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = simpleParameterizableThing.Iid;
                command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(simpleParameterizableThing, "Owner") ? simpleParameterizableThing.Owner : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET \"Owner\"");
                sqlBuilder.Append(" = :owner;");

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
        /// <param name="simpleParameterizableThing">
        /// The SimpleParameterizableThing DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.SimpleParameterizableThing simpleParameterizableThing, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, simpleParameterizableThing, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, simpleParameterizableThing, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterizableThing\"", partition);
                    sqlBuilder.AppendFormat(" SET \"Owner\"");
                    sqlBuilder.AppendFormat(" = :owner");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = simpleParameterizableThing.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(simpleParameterizableThing, "Owner") ? simpleParameterizableThing.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, simpleParameterizableThing, container);
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
        /// The <see cref="CDP4Common.DTO.SimpleParameterizableThing"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.SimpleParameterizableThing"/> id that is to be deleted.
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
        /// Build a SQL read query for the current <see cref="SimpleParameterizableThingDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"SimpleParameterizableThing\".\"Owner\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"SimpleParameterizableThing_ParameterValue\".\"ParameterValue\",'{}'::text[]) AS \"ParameterValue\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SimpleParameterizableThing_Data\"() AS \"SimpleParameterizableThing\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Alias\".\"Container\" AS \"Iid\", array_agg(\"Alias\".\"Iid\"::text) AS \"Alias\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Alias_Data\"() AS \"Alias\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"Alias\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Alias\".\"Container\") AS \"DefinedThing_Alias\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Definition\".\"Container\" AS \"Iid\", array_agg(\"Definition\".\"Iid\"::text) AS \"Definition\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Definition_Data\"() AS \"Definition\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"Definition\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Definition\".\"Container\") AS \"DefinedThing_Definition\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"HyperLink\".\"Container\" AS \"Iid\", array_agg(\"HyperLink\".\"Iid\"::text) AS \"HyperLink\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"HyperLink_Data\"() AS \"HyperLink\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" ON \"HyperLink\".\"Container\" = \"DefinedThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"HyperLink\".\"Container\") AS \"DefinedThing_HyperLink\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"SimpleParameterValue\".\"Container\" AS \"Iid\", array_agg(\"SimpleParameterValue\".\"Iid\"::text) AS \"ParameterValue\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SimpleParameterValue_Data\"() AS \"SimpleParameterValue\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"SimpleParameterizableThing_Data\"() AS \"SimpleParameterizableThing\" ON \"SimpleParameterValue\".\"Container\" = \"SimpleParameterizableThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"SimpleParameterValue\".\"Container\") AS \"SimpleParameterizableThing_ParameterValue\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"SimpleParameterizableThing_Audit\".\"Actor\", \"SimpleParameterizableThing_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"SimpleParameterizableThing_Audit\" AS \"SimpleParameterizableThing_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"SimpleParameterizableThing_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"SimpleParameterizableThing_Audit\".\"Iid\", \"SimpleParameterizableThing_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"SimpleParameterizableThing\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
