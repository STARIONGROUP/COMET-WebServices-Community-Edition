// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModellingAnnotationItemDao.cs" company="RHEA System S.A.">
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
    /// The abstract ModellingAnnotationItem Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ModellingAnnotationItemDao : EngineeringModelDataAnnotationDao
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
        /// <param name="modellingAnnotationItem">
        /// The modellingAnnotationItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, modellingAnnotationItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, modellingAnnotationItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                    { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                    { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                    { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :owner);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modellingAnnotationItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(modellingAnnotationItem, "Owner") ? modellingAnnotationItem.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
                modellingAnnotationItem.Category.ForEach(x => this.AddCategory(transaction, partition, modellingAnnotationItem.Iid, x));
                modellingAnnotationItem.SourceAnnotation.ForEach(x => this.AddSourceAnnotation(transaction, partition, modellingAnnotationItem.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, modellingAnnotationItem, container);
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
        /// <param name="modellingAnnotationItem">
        /// The modellingAnnotationItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, modellingAnnotationItem, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :owner)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modellingAnnotationItem.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(modellingAnnotationItem, "Owner") ? modellingAnnotationItem.Owner : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"Owner\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :owner);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
            modellingAnnotationItem.Category.ForEach(x => this.UpsertCategory(transaction, partition, modellingAnnotationItem.Iid, x));
            modellingAnnotationItem.SourceAnnotation.ForEach(x => this.UpsertSourceAnnotation(transaction, partition, modellingAnnotationItem.Iid, x));

            return true;
        }

        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public override bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isCreated = base.AddToCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Category":
                    {
                        isCreated = this.AddCategory(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "SourceAnnotation":
                    {
                        isCreated = this.AddSourceAnnotation(transaction, partition, iid, (Guid)value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isCreated;
        }

        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="category">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddCategory(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_Category\"", partition);
                sqlBuilder.AppendFormat(" (\"ModellingAnnotationItem\", \"Category\")");
                sqlBuilder.Append(" VALUES (:modellingAnnotationItem, :category);");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("category", NpgsqlDbType.Uuid).Value = category;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table, or update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="category">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertCategory(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_Category\"", partition);
                sqlBuilder.AppendFormat(" (\"ModellingAnnotationItem\", \"Category\")");
                sqlBuilder.Append(" VALUES (:modellingAnnotationItem, :category)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ModellingAnnotationItem_Category_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ModellingAnnotationItem\", \"Category\")");
                sqlBuilder.Append(" = (:modellingAnnotationItem, :category);");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("category", NpgsqlDbType.Uuid).Value = category;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="sourceAnnotation">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddSourceAnnotation(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\"", partition);
                sqlBuilder.AppendFormat(" (\"ModellingAnnotationItem\", \"SourceAnnotation\")");
                sqlBuilder.Append(" VALUES (:modellingAnnotationItem, :sourceAnnotation);");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("sourceAnnotation", NpgsqlDbType.Uuid).Value = sourceAnnotation;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Insert a new association record in the link table, or update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="sourceAnnotation">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertSourceAnnotation(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\"", partition);
                sqlBuilder.AppendFormat(" (\"ModellingAnnotationItem\", \"SourceAnnotation\")");
                sqlBuilder.Append(" VALUES (:modellingAnnotationItem, :sourceAnnotation)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ModellingAnnotationItem_SourceAnnotation_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ModellingAnnotationItem\", \"SourceAnnotation\")");
                sqlBuilder.Append(" = (:modellingAnnotationItem, :sourceAnnotation);");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("sourceAnnotation", NpgsqlDbType.Uuid).Value = sourceAnnotation;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// <param name="modellingAnnotationItem">
        /// The ModellingAnnotationItem DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, modellingAnnotationItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, modellingAnnotationItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                    { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                    { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                    { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"Owner\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :owner)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modellingAnnotationItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(modellingAnnotationItem, "Owner") ? modellingAnnotationItem.Owner : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, modellingAnnotationItem, container);
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
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that is to be deleted.
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
        /// Delete the supplied value from the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property name from where the value is to be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that is the source of each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be removed.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public override bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isDeleted = base.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Category":
                    {
                        isDeleted = this.DeleteCategory(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "SourceAnnotation":
                    {
                        isDeleted = this.DeleteSourceAnnotation(transaction, partition, iid, (Guid)value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isDeleted;
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="category">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteCategory(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ModellingAnnotationItem_Category\"", partition);
                sqlBuilder.Append(" WHERE \"ModellingAnnotationItem\" = :modellingAnnotationItem");
                sqlBuilder.Append(" AND \"Category\" = :category;");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("category", NpgsqlDbType.Uuid).Value = category;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ModellingAnnotationItem"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="sourceAnnotation">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteSourceAnnotation(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\"", partition);
                sqlBuilder.Append(" WHERE \"ModellingAnnotationItem\" = :modellingAnnotationItem");
                sqlBuilder.Append(" AND \"SourceAnnotation\" = :sourceAnnotation;");

                command.Parameters.Add("modellingAnnotationItem", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("sourceAnnotation", NpgsqlDbType.Uuid).Value = sourceAnnotation;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="ModellingAnnotationItemDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"ModellingAnnotationItem\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"EngineeringModelDataAnnotation\".\"Author\",");

            sqlBuilder.Append(" \"EngineeringModelDataAnnotation\".\"PrimaryAnnotatedThing\",");

            sqlBuilder.Append(" \"ModellingAnnotationItem\".\"Owner\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelDataAnnotation_Discussion\".\"Discussion\",'{}'::text[]) AS \"Discussion\",");
            sqlBuilder.Append(" COALESCE(\"EngineeringModelDataAnnotation_RelatedThing\".\"RelatedThing\",'{}'::text[]) AS \"RelatedThing\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_ApprovedBy\".\"ApprovedBy\",'{}'::text[]) AS \"ApprovedBy\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_Category\".\"Category\",'{}'::text[]) AS \"Category\",");
            sqlBuilder.Append(" COALESCE(\"ModellingAnnotationItem_SourceAnnotation\".\"SourceAnnotation\",'{}'::text[]) AS \"SourceAnnotation\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"GenericAnnotation_Data\"() AS \"GenericAnnotation\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"EngineeringModelDataAnnotation_Data\"() AS \"EngineeringModelDataAnnotation\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ModellingAnnotationItem_Data\"() AS \"ModellingAnnotationItem\" USING (\"Iid\")", partition);

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedDomain_Data\"() AS \"Thing_ExcludedDomain\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_ExcludedPerson_Data\"() AS \"Thing_ExcludedPerson\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"Thing_Data\"() AS \"Thing\" ON \"Thing\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EngineeringModelDataDiscussionItem\".\"Container\" AS \"Iid\", array_agg(\"EngineeringModelDataDiscussionItem\".\"Iid\"::text) AS \"Discussion\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataDiscussionItem_Data\"() AS \"EngineeringModelDataDiscussionItem\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"EngineeringModelDataAnnotation_Data\"() AS \"EngineeringModelDataAnnotation\" ON \"EngineeringModelDataDiscussionItem\".\"Container\" = \"EngineeringModelDataAnnotation\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"EngineeringModelDataDiscussionItem\".\"Container\") AS \"EngineeringModelDataAnnotation_Discussion\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingThingReference\".\"Container\" AS \"Iid\", array_agg(\"ModellingThingReference\".\"Iid\"::text) AS \"RelatedThing\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingThingReference_Data\"() AS \"ModellingThingReference\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"EngineeringModelDataAnnotation_Data\"() AS \"EngineeringModelDataAnnotation\" ON \"ModellingThingReference\".\"Container\" = \"EngineeringModelDataAnnotation\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ModellingThingReference\".\"Container\") AS \"EngineeringModelDataAnnotation_RelatedThing\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Approval\".\"Container\" AS \"Iid\", array_agg(\"Approval\".\"Iid\"::text) AS \"ApprovedBy\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Approval_Data\"() AS \"Approval\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ModellingAnnotationItem_Data\"() AS \"ModellingAnnotationItem\" ON \"Approval\".\"Container\" = \"ModellingAnnotationItem\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Approval\".\"Container\") AS \"ModellingAnnotationItem_ApprovedBy\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem\" AS \"Iid\", array_agg(\"Category\"::text) AS \"Category\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Category_Data\"() AS \"ModellingAnnotationItem_Category\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ModellingAnnotationItem_Data\"() AS \"ModellingAnnotationItem\" ON \"ModellingAnnotationItem\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem\") AS \"ModellingAnnotationItem_Category\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem\" AS \"Iid\", array_agg(\"SourceAnnotation\"::text) AS \"SourceAnnotation\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation_Data\"() AS \"ModellingAnnotationItem_SourceAnnotation\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ModellingAnnotationItem_Data\"() AS \"ModellingAnnotationItem\" ON \"ModellingAnnotationItem\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem\") AS \"ModellingAnnotationItem_SourceAnnotation\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"ModellingAnnotationItem_Audit\".\"Actor\", \"ModellingAnnotationItem_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Audit\" AS \"ModellingAnnotationItem_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"ModellingAnnotationItem_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"ModellingAnnotationItem_Audit\".\"Iid\", \"ModellingAnnotationItem_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"GenericAnnotation\".\"ValueTypeDictionary\" || \"EngineeringModelDataAnnotation\".\"ValueTypeDictionary\" || \"ModellingAnnotationItem\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
