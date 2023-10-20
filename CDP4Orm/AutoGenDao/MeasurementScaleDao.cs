// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleDao.cs" company="RHEA System S.A.">
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
        /// Insert a new database record, or updates one if it already exists from the supplied data transfer object.
        /// This is typically used during the import of existing data to the Database.
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
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.MeasurementScale measurementScale, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, measurementScale, container);

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
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :unit)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = measurementScale.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("unit", NpgsqlDbType.Uuid).Value = !this.IsDerived(measurementScale, "Unit") ? measurementScale.Unit : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"Unit\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :unit);");

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
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :unit)");
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
                    sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = :valueTypeDictionary");
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
        /// The <see cref="CDP4Common.DTO.MeasurementScale"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool RawDelete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var valueTypeDictionaryContents = new Dictionary<string, string>
                        {
                            { "IsDeprecated", "true" }
                        };
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MeasurementScale\"", partition);
                sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = :valueTypeDictionary");
                sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                result = this.ExecuteAndLogCommand(command) > 0;
            }

            return result;
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="MeasurementScaleDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"MeasurementScale\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"MeasurementScale\".\"Unit\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"MeasurementScale_MappingToReferenceScale\".\"MappingToReferenceScale\",'{}'::text[]) AS \"MappingToReferenceScale\",");
            sqlBuilder.Append(" COALESCE(\"MeasurementScale_ValueDefinition\".\"ValueDefinition\",'{}'::text[]) AS \"ValueDefinition\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"MeasurementScale_Data\"() AS \"MeasurementScale\" USING (\"Iid\")", partition);

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

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MappingToReferenceScale\".\"Container\" AS \"Iid\", array_agg(\"MappingToReferenceScale\".\"Iid\"::text) AS \"MappingToReferenceScale\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MappingToReferenceScale_Data\"() AS \"MappingToReferenceScale\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"MeasurementScale_Data\"() AS \"MeasurementScale\" ON \"MappingToReferenceScale\".\"Container\" = \"MeasurementScale\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"MappingToReferenceScale\".\"Container\") AS \"MeasurementScale_MappingToReferenceScale\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ScaleValueDefinition\".\"Container\" AS \"Iid\", array_agg(\"ScaleValueDefinition\".\"Iid\"::text) AS \"ValueDefinition\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ScaleValueDefinition_Data\"() AS \"ScaleValueDefinition\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"MeasurementScale_Data\"() AS \"MeasurementScale\" ON \"ScaleValueDefinition\".\"Container\" = \"MeasurementScale\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ScaleValueDefinition\".\"Container\") AS \"MeasurementScale_ValueDefinition\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementScale_Audit\".\"Actor\", \"MeasurementScale_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementScale_Audit\" AS \"MeasurementScale_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"MeasurementScale_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"MeasurementScale_Audit\".\"Iid\", \"MeasurementScale_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"MeasurementScale\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
