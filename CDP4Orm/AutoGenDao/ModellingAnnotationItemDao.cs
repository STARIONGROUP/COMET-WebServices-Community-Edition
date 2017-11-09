// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModellingAnnotationItemDao.cs" company="RHEA System S.A.">
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
    /// The abstract ModellingAnnotationItem Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ModellingAnnotationItemDao : EngineeringModelDataAnnotationDao
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
                            { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                            { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                            { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                            { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
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
                
                modellingAnnotationItem.SourceAnnotation.ForEach(x => this.AddSourceAnnotation(transaction, partition, modellingAnnotationItem.Iid, x));
                modellingAnnotationItem.Category.ForEach(x => this.AddCategory(transaction, partition, modellingAnnotationItem.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, modellingAnnotationItem, container);
        }
 
        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
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
                case "SourceAnnotation":
                    {
                        isCreated = this.AddSourceAnnotation(transaction, partition, iid, (Guid)value);
                        break;
                    }
 
                case "Category":
                    {
                        isCreated = this.AddCategory(transaction, partition, iid, (Guid)value);
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
        /// The current transaction to the database.
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
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
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
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="modellingAnnotationItem">
        /// The modellingAnnotationItem DTO that is to be updated.
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
                            { "Status", !this.IsDerived(modellingAnnotationItem, "Status") ? modellingAnnotationItem.Status.ToString() : string.Empty },
                            { "Title", !this.IsDerived(modellingAnnotationItem, "Title") ? modellingAnnotationItem.Title.Escape() : string.Empty },
                            { "Classification", !this.IsDerived(modellingAnnotationItem, "Classification") ? modellingAnnotationItem.Classification.ToString() : string.Empty },
                            { "ShortName", !this.IsDerived(modellingAnnotationItem, "ShortName") ? modellingAnnotationItem.ShortName.Escape() : string.Empty },
                        }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ModellingAnnotationItem\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Container\", \"Owner\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" = (:container, :owner, \"ValueTypeDictionary\" || :valueTypeDictionary)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = modellingAnnotationItem.Iid;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("owner", NpgsqlDbType.Uuid).Value = !this.IsDerived(modellingAnnotationItem, "Owner") ? modellingAnnotationItem.Owner : Utils.NullableValue(null);
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                
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
        /// The current transaction to the database.
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
        /// Delete the supplied value from the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
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
                case "SourceAnnotation":
                    {
                        isDeleted = this.DeleteSourceAnnotation(transaction, partition, iid, (Guid)value);
                        break;
                    }
 
                case "Category":
                    {
                        isDeleted = this.DeleteCategory(transaction, partition, iid, (Guid)value);
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
        /// The current transaction to the database.
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
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
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
    }
}
