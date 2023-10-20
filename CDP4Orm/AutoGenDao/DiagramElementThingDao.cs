// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramElementThingDao.cs" company="RHEA System S.A.">
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
    /// The abstract DiagramElementThing Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class DiagramElementThingDao : DiagramElementContainerDao
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
        /// <param name="diagramElementThing">
        /// The diagramElementThing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramElementThing diagramElementThing, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, diagramElementThing, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, diagramElementThing, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementThing\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Container\", \"DepictedThing\", \"SharedStyle\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :container, :depictedThing, :sharedStyle);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramElementThing.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("depictedThing", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "DepictedThing") ? Utils.NullableValue(diagramElementThing.DepictedThing) : Utils.NullableValue(null);
                    command.Parameters.Add("sharedStyle", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "SharedStyle") ? Utils.NullableValue(diagramElementThing.SharedStyle) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, diagramElementThing, container);
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
        /// <param name="diagramElementThing">
        /// The diagramElementThing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramElementThing diagramElementThing, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, diagramElementThing, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementThing\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"Container\", \"DepictedThing\", \"SharedStyle\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :container, :depictedThing, :sharedStyle)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramElementThing.Iid;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("depictedThing", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "DepictedThing") ? Utils.NullableValue(diagramElementThing.DepictedThing) : Utils.NullableValue(null);
                command.Parameters.Add("sharedStyle", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "SharedStyle") ? Utils.NullableValue(diagramElementThing.SharedStyle) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"Container\", \"DepictedThing\", \"SharedStyle\")");
                sqlBuilder.Append(" = (:container, :depictedThing, :sharedStyle);");

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
        /// <param name="diagramElementThing">
        /// The DiagramElementThing DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagramElementThing diagramElementThing, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, diagramElementThing, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, diagramElementThing, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementThing\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"DepictedThing\", \"SharedStyle\")");
                    sqlBuilder.AppendFormat(" = (:container, :depictedThing, :sharedStyle)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagramElementThing.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("depictedThing", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "DepictedThing") ? Utils.NullableValue(diagramElementThing.DepictedThing) : Utils.NullableValue(null);
                    command.Parameters.Add("sharedStyle", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagramElementThing, "SharedStyle") ? Utils.NullableValue(diagramElementThing.SharedStyle) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, diagramElementThing, container);
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
        /// The <see cref="CDP4Common.DTO.DiagramElementThing"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.DiagramElementThing"/> id that is to be deleted.
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
        /// Build a SQL read query for the current <see cref="DiagramElementThingDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"DiagramElementThing\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"DiagramElementThing\".\"DepictedThing\",");

            sqlBuilder.Append(" \"DiagramElementThing\".\"SharedStyle\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementContainer_Bounds\".\"Bounds\",'{}'::text[]) AS \"Bounds\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementContainer_DiagramElement\".\"DiagramElement\",'{}'::text[]) AS \"DiagramElement\",");
            sqlBuilder.Append(" COALESCE(\"DiagramElementThing_LocalStyle\".\"LocalStyle\",'{}'::text[]) AS \"LocalStyle\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramThingBase_Data\"() AS \"DiagramThingBase\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Bounds\".\"Container\" AS \"Iid\", array_agg(\"Bounds\".\"Iid\"::text) AS \"Bounds\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Bounds_Data\"() AS \"Bounds\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" ON \"Bounds\".\"Container\" = \"DiagramElementContainer\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Bounds\".\"Container\") AS \"DiagramElementContainer_Bounds\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"DiagramElementThing\".\"Container\" AS \"Iid\", array_agg(\"DiagramElementThing\".\"Iid\"::text) AS \"DiagramElement\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementContainer_Data\"() AS \"DiagramElementContainer\" ON \"DiagramElementThing\".\"Container\" = \"DiagramElementContainer\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"DiagramElementThing\".\"Container\") AS \"DiagramElementContainer_DiagramElement\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"OwnedStyle\".\"Container\" AS \"Iid\", array_agg(\"OwnedStyle\".\"Iid\"::text) AS \"LocalStyle\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"OwnedStyle_Data\"() AS \"OwnedStyle\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DiagramElementThing_Data\"() AS \"DiagramElementThing\" ON \"OwnedStyle\".\"Container\" = \"DiagramElementThing\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"OwnedStyle\".\"Container\") AS \"DiagramElementThing_LocalStyle\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"DiagramElementThing_Audit\".\"Actor\", \"DiagramElementThing_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramElementThing_Audit\" AS \"DiagramElementThing_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"DiagramElementThing_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"DiagramElementThing_Audit\".\"Iid\", \"DiagramElementThing_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DiagramThingBase\".\"ValueTypeDictionary\" || \"DiagramElementContainer\".\"ValueTypeDictionary\" || \"DiagramElementThing\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
