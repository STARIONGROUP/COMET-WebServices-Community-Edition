// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModellingAnnotationItemDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Threading.Tasks;

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
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWriteResult = await this.BeforeWriteAsync(transaction, partition, modellingAnnotationItem, container, valueTypeDictionaryAdditions);

            var beforeWrite = beforeWriteResult.Value;
            var isHandled = beforeWriteResult.IsHandled;

            if (!isHandled)
            {
                beforeWrite = beforeWrite && await base.WriteAsync(transaction, partition, modellingAnnotationItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                    { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                    { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                    { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
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

                    await command.ExecuteNonQueryAsync();
                }

                foreach (var item in modellingAnnotationItem.Category)
                {
                    await this.AddCategoryAsync(transaction, partition, modellingAnnotationItem.Iid, item);
                }

                foreach (var item in modellingAnnotationItem.SourceAnnotation)
                {
                    await this.AddSourceAnnotationAsync(transaction, partition, modellingAnnotationItem.Iid, item);
                }
            }

            return await this.AfterWriteAsync(beforeWrite, transaction, partition, modellingAnnotationItem, container);
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
        /// An awaitable <see cref="Task"/> having True if the concept was successfully persisted as result.
        /// </returns>
        public virtual async Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            await base.UpsertAsync(transaction, partition, modellingAnnotationItem, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            await using (var command = new NpgsqlCommand())
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

                await command.ExecuteNonQueryAsync();
            }

            foreach (var item in modellingAnnotationItem.Category)
            {
                await this.UpsertCategoryAsync(transaction, partition, modellingAnnotationItem.Iid, item);
            }

            foreach (var item in modellingAnnotationItem.SourceAnnotation)
            {
                await this.UpsertSourceAnnotationAsync(transaction, partition, modellingAnnotationItem.Iid, item);
            }

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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully created as result.
        /// </returns>
        public override async Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isCreated = await base.AddToCollectionPropertyAsync(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Category":
                    {
                        isCreated = await this.AddCategoryAsync(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "SourceAnnotation":
                    {
                        isCreated = await this.AddSourceAnnotationAsync(transaction, partition, iid, (Guid)value);
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully created as result.
        /// </returns>
        public async Task<bool> AddCategoryAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully created as result.
        /// </returns>
        public async Task<bool> UpsertCategoryAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully created as result.
        /// </returns>
        public async Task<bool> AddSourceAnnotationAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully created as result.
        /// </returns>
        public async Task<bool> UpsertSourceAnnotationAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
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
        /// An awaitable <see cref="Task"/> having True if the concept was successfully updated as result.
        /// </returns>
        public virtual async Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ModellingAnnotationItem modellingAnnotationItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdateResult = await this.BeforeUpdateAsync(transaction, partition, modellingAnnotationItem, container, valueTypeDictionaryAdditions);

            var beforeUpdate = beforeUpdateResult.Value;
            var isHandled = beforeUpdateResult.IsHandled;

            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && await base.UpdateAsync(transaction, partition, modellingAnnotationItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                    { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                    { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                    { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await using (var command = new NpgsqlCommand())
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

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await this.AfterUpdateAsync(beforeUpdate, transaction, partition, modellingAnnotationItem, container);
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
        /// An awaitable <see cref="Task"/> having True if the concept was successfully deleted as result.
        /// </returns>
        public override async Task<bool> DeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var beforeDeleteResult = await this.BeforeDeleteAsync(transaction, partition, iid);

            var beforeDelete = beforeDeleteResult.Value;
            var isHandled = beforeDeleteResult.IsHandled;

            if (!isHandled)
            {
                beforeDelete = beforeDelete && await base.DeleteAsync(transaction, partition, iid);
            }

            return await this.AfterDeleteAsync(beforeDelete, transaction, partition, iid);
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
        /// An awaitable <see cref="Task"/> having True if the concept was successfully deleted as result.
        /// </returns>
        public override async Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = await base.DeleteAsync(transaction, partition, iid);
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully deleted as result.
        /// </returns>
        public override async Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isDeleted = await base.DeleteFromCollectionPropertyAsync(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "Category":
                    {
                        isDeleted = await this.DeleteCategoryAsync(transaction, partition, iid, (Guid)value);
                        break;
                    }

                case "SourceAnnotation":
                    {
                        isDeleted = await this.DeleteSourceAnnotationAsync(transaction, partition, iid, (Guid)value);
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully removed as result.
        /// </returns>
        public async Task<bool> DeleteCategoryAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid category)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
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
        /// An awaitable <see cref="Task"/> having True if the value link was successfully removed as result.
        /// </returns>
        public async Task<bool> DeleteSourceAnnotationAsync(NpgsqlTransaction transaction, string partition, Guid iid, Guid sourceAnnotation)
        {
            await using (var command = new NpgsqlCommand())
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

                return (await command.ExecuteNonQueryAsync()) > 0;
            }
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
        private string GetGenericAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"GenericAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"GenericAnnotation_Audit\"", partition);
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
        private string GetEngineeringModelDataAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Author\", \"PrimaryAnnotatedThing\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataAnnotation_Audit\"", partition);
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
        private string GetModellingAnnotationItemDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Audit\"", partition);
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
        private string GetEngineeringModelDataDiscussionItemDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Author\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataDiscussionItem\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EngineeringModelDataDiscussionItem_Audit\"", partition);
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
        private string GetModellingThingReferenceDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingThingReference\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingThingReference_Audit\"", partition);
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
        private string GetApprovalDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"Author\", \"Owner\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Approval\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Approval_Audit\"", partition);
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
        private string GetModellingAnnotationItem_CategoryDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ModellingAnnotationItem\",\"Category\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Category\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_Category_Audit\"", partition);
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
        private string GetModellingAnnotationItem_SourceAnnotationDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = "\"ModellingAnnotationItem\",\"SourceAnnotation\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"ModellingAnnotationItem_SourceAnnotation_Audit\"", partition);
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
