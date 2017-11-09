// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelDataAnnotationDao.cs" company="RHEA System S.A.">
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
    /// The abstract EngineeringModelDataAnnotation Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class EngineeringModelDataAnnotationDao : GenericAnnotationDao
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
        /// <param name="engineeringModelDataAnnotation">
        /// The engineeringModelDataAnnotation DTO that is to be persisted.
        /// </param> 
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModelDataAnnotation engineeringModelDataAnnotation, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, engineeringModelDataAnnotation, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, engineeringModelDataAnnotation, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"EngineeringModelDataAnnotation\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"Author\", \"PrimaryAnnotatedThing\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :author, :primaryAnnotatedThing);");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModelDataAnnotation.Iid;
                    command.Parameters.Add("author", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelDataAnnotation, "Author") ? engineeringModelDataAnnotation.Author : Utils.NullableValue(null);
                    command.Parameters.Add("primaryAnnotatedThing", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelDataAnnotation, "PrimaryAnnotatedThing") ? Utils.NullableValue(engineeringModelDataAnnotation.PrimaryAnnotatedThing) : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, engineeringModelDataAnnotation, container);
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
        /// <param name="engineeringModelDataAnnotation">
        /// The engineeringModelDataAnnotation DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.EngineeringModelDataAnnotation engineeringModelDataAnnotation, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, engineeringModelDataAnnotation, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, engineeringModelDataAnnotation, container);
                
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"EngineeringModelDataAnnotation\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"Author\", \"PrimaryAnnotatedThing\")");
                    sqlBuilder.AppendFormat(" = (:author, :primaryAnnotatedThing)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = engineeringModelDataAnnotation.Iid;
                    command.Parameters.Add("author", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelDataAnnotation, "Author") ? engineeringModelDataAnnotation.Author : Utils.NullableValue(null);
                    command.Parameters.Add("primaryAnnotatedThing", NpgsqlDbType.Uuid).Value = !this.IsDerived(engineeringModelDataAnnotation, "PrimaryAnnotatedThing") ? Utils.NullableValue(engineeringModelDataAnnotation.PrimaryAnnotatedThing) : Utils.NullableValue(null);
                
                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, engineeringModelDataAnnotation, container);
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
        /// The <see cref="CDP4Common.DTO.EngineeringModelDataAnnotation"/> id that is to be deleted.
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
    }
}
