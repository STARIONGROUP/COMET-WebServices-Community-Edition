// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingStyleDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
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
    /// The abstract DiagrammingStyle Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class DiagrammingStyleDao : DiagramThingBaseDao
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
        /// <param name="diagrammingStyle">
        /// The diagrammingStyle DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagrammingStyle diagrammingStyle, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, diagrammingStyle, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, diagrammingStyle, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "FillOpacity", !this.IsDerived(diagrammingStyle, "FillOpacity") && diagrammingStyle.FillOpacity.HasValue ? diagrammingStyle.FillOpacity.Value.ToString() : null },
                    { "FontBold", !this.IsDerived(diagrammingStyle, "FontBold") && diagrammingStyle.FontBold.HasValue ? diagrammingStyle.FontBold.Value.ToString() : null },
                    { "FontItalic", !this.IsDerived(diagrammingStyle, "FontItalic") && diagrammingStyle.FontItalic.HasValue ? diagrammingStyle.FontItalic.Value.ToString() : null },
                    { "FontName", !this.IsDerived(diagrammingStyle, "FontName") ? diagrammingStyle.FontName.Escape() : null },
                    { "FontSize", !this.IsDerived(diagrammingStyle, "FontSize") && diagrammingStyle.FontSize.HasValue ? diagrammingStyle.FontSize.Value.ToString() : null },
                    { "FontStrokeThrough", !this.IsDerived(diagrammingStyle, "FontStrokeThrough") && diagrammingStyle.FontStrokeThrough.HasValue ? diagrammingStyle.FontStrokeThrough.Value.ToString() : null },
                    { "FontUnderline", !this.IsDerived(diagrammingStyle, "FontUnderline") && diagrammingStyle.FontUnderline.HasValue ? diagrammingStyle.FontUnderline.Value.ToString() : null },
                    { "StrokeOpacity", !this.IsDerived(diagrammingStyle, "StrokeOpacity") && diagrammingStyle.StrokeOpacity.HasValue ? diagrammingStyle.StrokeOpacity.Value.ToString() : null },
                    { "StrokeWidth", !this.IsDerived(diagrammingStyle, "StrokeWidth") && diagrammingStyle.StrokeWidth.HasValue ? diagrammingStyle.StrokeWidth.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagrammingStyle\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"FillColor\", \"FontColor\", \"StrokeColor\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :fillColor, :fontColor, :strokeColor);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagrammingStyle.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("fillColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FillColor") ? Utils.NullableValue(diagrammingStyle.FillColor) : Utils.NullableValue(null);
                    command.Parameters.Add("fontColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FontColor") ? Utils.NullableValue(diagrammingStyle.FontColor) : Utils.NullableValue(null);
                    command.Parameters.Add("strokeColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "StrokeColor") ? Utils.NullableValue(diagrammingStyle.StrokeColor) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, diagrammingStyle, container);
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
        /// <param name="diagrammingStyle">
        /// The diagrammingStyle DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagrammingStyle diagrammingStyle, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, diagrammingStyle, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "FillOpacity", !this.IsDerived(diagrammingStyle, "FillOpacity") && diagrammingStyle.FillOpacity.HasValue ? diagrammingStyle.FillOpacity.Value.ToString() : null },
                { "FontBold", !this.IsDerived(diagrammingStyle, "FontBold") && diagrammingStyle.FontBold.HasValue ? diagrammingStyle.FontBold.Value.ToString() : null },
                { "FontItalic", !this.IsDerived(diagrammingStyle, "FontItalic") && diagrammingStyle.FontItalic.HasValue ? diagrammingStyle.FontItalic.Value.ToString() : null },
                { "FontName", !this.IsDerived(diagrammingStyle, "FontName") ? diagrammingStyle.FontName.Escape() : null },
                { "FontSize", !this.IsDerived(diagrammingStyle, "FontSize") && diagrammingStyle.FontSize.HasValue ? diagrammingStyle.FontSize.Value.ToString() : null },
                { "FontStrokeThrough", !this.IsDerived(diagrammingStyle, "FontStrokeThrough") && diagrammingStyle.FontStrokeThrough.HasValue ? diagrammingStyle.FontStrokeThrough.Value.ToString() : null },
                { "FontUnderline", !this.IsDerived(diagrammingStyle, "FontUnderline") && diagrammingStyle.FontUnderline.HasValue ? diagrammingStyle.FontUnderline.Value.ToString() : null },
                { "StrokeOpacity", !this.IsDerived(diagrammingStyle, "StrokeOpacity") && diagrammingStyle.StrokeOpacity.HasValue ? diagrammingStyle.StrokeOpacity.Value.ToString() : null },
                { "StrokeWidth", !this.IsDerived(diagrammingStyle, "StrokeWidth") && diagrammingStyle.StrokeWidth.HasValue ? diagrammingStyle.StrokeWidth.Value.ToString() : null },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagrammingStyle\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"FillColor\", \"FontColor\", \"StrokeColor\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :fillColor, :fontColor, :strokeColor)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagrammingStyle.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("fillColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FillColor") ? Utils.NullableValue(diagrammingStyle.FillColor) : Utils.NullableValue(null);
                command.Parameters.Add("fontColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FontColor") ? Utils.NullableValue(diagrammingStyle.FontColor) : Utils.NullableValue(null);
                command.Parameters.Add("strokeColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "StrokeColor") ? Utils.NullableValue(diagrammingStyle.StrokeColor) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"FillColor\", \"FontColor\", \"StrokeColor\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :fillColor, :fontColor, :strokeColor);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                command.ExecuteNonQuery();
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
        /// <param name="diagrammingStyle">
        /// The DiagrammingStyle DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.DiagrammingStyle diagrammingStyle, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, diagrammingStyle, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, diagrammingStyle, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "FillOpacity", !this.IsDerived(diagrammingStyle, "FillOpacity") && diagrammingStyle.FillOpacity.HasValue ? diagrammingStyle.FillOpacity.Value.ToString() : null },
                    { "FontBold", !this.IsDerived(diagrammingStyle, "FontBold") && diagrammingStyle.FontBold.HasValue ? diagrammingStyle.FontBold.Value.ToString() : null },
                    { "FontItalic", !this.IsDerived(diagrammingStyle, "FontItalic") && diagrammingStyle.FontItalic.HasValue ? diagrammingStyle.FontItalic.Value.ToString() : null },
                    { "FontName", !this.IsDerived(diagrammingStyle, "FontName") ? diagrammingStyle.FontName.Escape() : null },
                    { "FontSize", !this.IsDerived(diagrammingStyle, "FontSize") && diagrammingStyle.FontSize.HasValue ? diagrammingStyle.FontSize.Value.ToString() : null },
                    { "FontStrokeThrough", !this.IsDerived(diagrammingStyle, "FontStrokeThrough") && diagrammingStyle.FontStrokeThrough.HasValue ? diagrammingStyle.FontStrokeThrough.Value.ToString() : null },
                    { "FontUnderline", !this.IsDerived(diagrammingStyle, "FontUnderline") && diagrammingStyle.FontUnderline.HasValue ? diagrammingStyle.FontUnderline.Value.ToString() : null },
                    { "StrokeOpacity", !this.IsDerived(diagrammingStyle, "StrokeOpacity") && diagrammingStyle.StrokeOpacity.HasValue ? diagrammingStyle.StrokeOpacity.Value.ToString() : null },
                    { "StrokeWidth", !this.IsDerived(diagrammingStyle, "StrokeWidth") && diagrammingStyle.StrokeWidth.HasValue ? diagrammingStyle.StrokeWidth.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagrammingStyle\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"FillColor\", \"FontColor\", \"StrokeColor\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :fillColor, :fontColor, :strokeColor)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = diagrammingStyle.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("fillColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FillColor") ? Utils.NullableValue(diagrammingStyle.FillColor) : Utils.NullableValue(null);
                    command.Parameters.Add("fontColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "FontColor") ? Utils.NullableValue(diagrammingStyle.FontColor) : Utils.NullableValue(null);
                    command.Parameters.Add("strokeColor", NpgsqlDbType.Uuid).Value = !this.IsDerived(diagrammingStyle, "StrokeColor") ? Utils.NullableValue(diagrammingStyle.StrokeColor) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, diagrammingStyle, container);
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
        /// The <see cref="CDP4Common.DTO.DiagrammingStyle"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.DiagrammingStyle"/> id that is to be deleted.
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
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThingDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetDiagramThingBaseDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramThingBase\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagramThingBase_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetDiagrammingStyleDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"FillColor\", \"FontColor\", \"StrokeColor\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagrammingStyle\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"DiagrammingStyle_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThing_ExcludedDomainDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"Thing\",\"ExcludedDomain\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetThing_ExcludedPersonDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"Thing\",\"ExcludedPerson\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets a DataSql string for a specific table
        /// </summary>        
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The DataSql string</returns>
        private string GetColorDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Color\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Color_Audit\"", partition);
                sqlBuilder.Append(" WHERE \"Action\" <> 'I'");
                sqlBuilder.Append(" AND \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
            }

            return sqlBuilder.ToString();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
