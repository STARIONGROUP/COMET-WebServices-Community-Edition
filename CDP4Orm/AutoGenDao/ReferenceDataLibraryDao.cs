// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceDataLibraryDao.cs" company="RHEA System S.A.">
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
    /// The abstract ReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ReferenceDataLibraryDao : DefinedThingDao
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
        /// <param name="referenceDataLibrary">
        /// The referenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ReferenceDataLibrary referenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, referenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, referenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"RequiredRdl\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :requiredRdl);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = referenceDataLibrary.Iid;
                    command.Parameters.Add("requiredRdl", NpgsqlDbType.Uuid).Value = !this.IsDerived(referenceDataLibrary, "RequiredRdl") ? Utils.NullableValue(referenceDataLibrary.RequiredRdl) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
                referenceDataLibrary.BaseQuantityKind.ForEach(x => this.AddBaseQuantityKind(transaction, partition, referenceDataLibrary.Iid, x));
                referenceDataLibrary.BaseUnit.ForEach(x => this.AddBaseUnit(transaction, partition, referenceDataLibrary.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, referenceDataLibrary, container);
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
        /// <param name="referenceDataLibrary">
        /// The referenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ReferenceDataLibrary referenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, referenceDataLibrary, container);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"RequiredRdl\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :requiredRdl)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = referenceDataLibrary.Iid;
                command.Parameters.Add("requiredRdl", NpgsqlDbType.Uuid).Value = !this.IsDerived(referenceDataLibrary, "RequiredRdl") ? Utils.NullableValue(referenceDataLibrary.RequiredRdl) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET \"RequiredRdl\"");
                sqlBuilder.Append(" = :requiredRdl;");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
            referenceDataLibrary.BaseQuantityKind.ForEach(x => this.UpsertBaseQuantityKind(transaction, partition, referenceDataLibrary.Iid, x));
            referenceDataLibrary.BaseUnit.ForEach(x => this.UpsertBaseUnit(transaction, partition, referenceDataLibrary.Iid, x));

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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
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
                case "BaseQuantityKind":
                    {
                        isCreated = this.AddBaseQuantityKind(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "BaseUnit":
                    {
                        isCreated = this.AddBaseUnit(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseQuantityKind\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseQuantityKind, :sequence);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = Guid.Parse(baseQuantityKind.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;

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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseQuantityKind\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseQuantityKind, :sequence)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ReferenceDataLibrary_BaseQuantityKind_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.AppendFormat(" SET (\"ReferenceDataLibrary\", \"BaseQuantityKind\", \"Sequence\")");
                sqlBuilder.Append(" = (:referenceDataLibrary, :baseQuantityKind, :sequence);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = Guid.Parse(baseQuantityKind.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;

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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseUnit">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddBaseUnit(NpgsqlTransaction transaction, string partition, Guid iid, Guid baseUnit)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseUnit\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseUnit);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseUnit", NpgsqlDbType.Uuid).Value = baseUnit;

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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseUnit">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool UpsertBaseUnit(NpgsqlTransaction transaction, string partition, Guid iid, Guid baseUnit)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseUnit\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseUnit)");
                sqlBuilder.Append(" ON CONFLICT ON CONSTRAINT \"ReferenceDataLibrary_BaseUnit_PK\"");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ReferenceDataLibrary\", \"BaseUnit\")");
                sqlBuilder.Append(" = (:referenceDataLibrary, :baseUnit);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseUnit", NpgsqlDbType.Uuid).Value = baseUnit;

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
        /// <param name="referenceDataLibrary">
        /// The ReferenceDataLibrary DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ReferenceDataLibrary referenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, referenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, referenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" SET \"RequiredRdl\"");
                    sqlBuilder.AppendFormat(" = :requiredRdl");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = referenceDataLibrary.Iid;
                    command.Parameters.Add("requiredRdl", NpgsqlDbType.Uuid).Value = !this.IsDerived(referenceDataLibrary, "RequiredRdl") ? Utils.NullableValue(referenceDataLibrary.RequiredRdl) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, referenceDataLibrary, container);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for the reordered link table record.
        /// </param> 
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public override bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            var isReordered = base.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);

            switch (propertyName)
            {
                case "BaseQuantityKind":
                    {
                        isReordered = this.ReorderBaseQuantityKind(transaction, partition, iid, orderUpdate);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isReordered;
        }

        /// <summary>
        /// Reorder an item in an association link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for reordered link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// The value for which a link table record wil be reordered.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public bool ReorderBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.Append(" = :reorderSequence");
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseQuantityKind\" = :baseQuantityKind");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = Guid.Parse(baseQuantityKind.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is to be deleted.
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source of each link table record.
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
                case "BaseQuantityKind":
                    {
                        isDeleted = this.DeleteBaseQuantityKind(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "BaseUnit":
                    {
                        isDeleted = this.DeleteBaseUnit(transaction, partition, iid, (Guid)value);
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseQuantityKind\" = :baseQuantityKind");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = Guid.Parse(baseQuantityKind.V.ToString());
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;

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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="baseUnit">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteBaseUnit(NpgsqlTransaction transaction, string partition, Guid iid, Guid baseUnit)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseUnit\" = :baseUnit;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseUnit", NpgsqlDbType.Uuid).Value = baseUnit;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Build a SQL read query for the current <see cref="ReferenceDataLibraryDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"ReferenceDataLibrary\".\"RequiredRdl\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Alias\".\"Alias\",'{}'::text[]) AS \"Alias\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_Definition\".\"Definition\",'{}'::text[]) AS \"Definition\",");
            sqlBuilder.Append(" COALESCE(\"DefinedThing_HyperLink\".\"HyperLink\",'{}'::text[]) AS \"HyperLink\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseQuantityKind\".\"BaseQuantityKind\",'{}'::text[]) AS \"BaseQuantityKind\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_BaseUnit\".\"BaseUnit\",'{}'::text[]) AS \"BaseUnit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Constant\".\"Constant\",'{}'::text[]) AS \"Constant\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_DefinedCategory\".\"DefinedCategory\",'{}'::text[]) AS \"DefinedCategory\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_FileType\".\"FileType\",'{}'::text[]) AS \"FileType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Glossary\".\"Glossary\",'{}'::text[]) AS \"Glossary\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ParameterType\".\"ParameterType\",'{}'::text[]) AS \"ParameterType\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_ReferenceSource\".\"ReferenceSource\",'{}'::text[]) AS \"ReferenceSource\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Rule\".\"Rule\",'{}'::text[]) AS \"Rule\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Scale\".\"Scale\",'{}'::text[]) AS \"Scale\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_Unit\".\"Unit\",'{}'::text[]) AS \"Unit\",");
            sqlBuilder.Append(" COALESCE(\"ReferenceDataLibrary_UnitPrefix\".\"UnitPrefix\",'{}'::text[]) AS \"UnitPrefix\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Thing_Data\"() AS \"Thing\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"DefinedThing_Data\"() AS \"DefinedThing\" USING (\"Iid\")", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" USING (\"Iid\")", partition);

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

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", array_agg(\"BaseQuantityKind\"::text) AS \"BaseQuantityKind\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind_Data\"() AS \"ReferenceDataLibrary_BaseQuantityKind\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseQuantityKind\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary\" AS \"Iid\", array_agg(\"BaseUnit\"::text) AS \"BaseUnit\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit_Data\"() AS \"ReferenceDataLibrary_BaseUnit\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceDataLibrary\" = \"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary\") AS \"ReferenceDataLibrary_BaseUnit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Constant\".\"Container\" AS \"Iid\", array_agg(\"Constant\".\"Iid\"::text) AS \"Constant\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Constant_Data\"() AS \"Constant\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Constant\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Constant\".\"Container\") AS \"ReferenceDataLibrary_Constant\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Category\".\"Container\" AS \"Iid\", array_agg(\"Category\".\"Iid\"::text) AS \"DefinedCategory\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Category_Data\"() AS \"Category\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Category\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Category\".\"Container\") AS \"ReferenceDataLibrary_DefinedCategory\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"FileType\".\"Container\" AS \"Iid\", array_agg(\"FileType\".\"Iid\"::text) AS \"FileType\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"FileType_Data\"() AS \"FileType\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"FileType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"FileType\".\"Container\") AS \"ReferenceDataLibrary_FileType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Glossary\".\"Container\" AS \"Iid\", array_agg(\"Glossary\".\"Iid\"::text) AS \"Glossary\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Glossary_Data\"() AS \"Glossary\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Glossary\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Glossary\".\"Container\") AS \"ReferenceDataLibrary_Glossary\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ParameterType\".\"Container\" AS \"Iid\", array_agg(\"ParameterType\".\"Iid\"::text) AS \"ParameterType\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ParameterType_Data\"() AS \"ParameterType\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ParameterType\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ParameterType\".\"Container\") AS \"ReferenceDataLibrary_ParameterType\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceSource\".\"Container\" AS \"Iid\", array_agg(\"ReferenceSource\".\"Iid\"::text) AS \"ReferenceSource\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceSource_Data\"() AS \"ReferenceSource\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"ReferenceSource\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"ReferenceSource\".\"Container\") AS \"ReferenceDataLibrary_ReferenceSource\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Rule\".\"Container\" AS \"Iid\", array_agg(\"Rule\".\"Iid\"::text) AS \"Rule\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Rule_Data\"() AS \"Rule\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"Rule\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"Rule\".\"Container\") AS \"ReferenceDataLibrary_Rule\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementScale\".\"Container\" AS \"Iid\", array_agg(\"MeasurementScale\".\"Iid\"::text) AS \"Scale\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementScale_Data\"() AS \"MeasurementScale\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"MeasurementScale\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"MeasurementScale\".\"Container\") AS \"ReferenceDataLibrary_Scale\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"MeasurementUnit\".\"Container\" AS \"Iid\", array_agg(\"MeasurementUnit\".\"Iid\"::text) AS \"Unit\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"MeasurementUnit_Data\"() AS \"MeasurementUnit\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"MeasurementUnit\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"MeasurementUnit\".\"Container\") AS \"ReferenceDataLibrary_Unit\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"UnitPrefix\".\"Container\" AS \"Iid\", array_agg(\"UnitPrefix\".\"Iid\"::text) AS \"UnitPrefix\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"UnitPrefix_Data\"() AS \"UnitPrefix\"", partition);
            sqlBuilder.AppendFormat(" JOIN \"{0}\".\"ReferenceDataLibrary_Data\"() AS \"ReferenceDataLibrary\" ON \"UnitPrefix\".\"Container\" = \"ReferenceDataLibrary\".\"Iid\"", partition);
            sqlBuilder.Append(" GROUP BY \"UnitPrefix\".\"Container\") AS \"ReferenceDataLibrary_UnitPrefix\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"ReferenceDataLibrary_Audit\".\"Actor\", \"ReferenceDataLibrary_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"ReferenceDataLibrary_Audit\" AS \"ReferenceDataLibrary_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"ReferenceDataLibrary_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"ReferenceDataLibrary_Audit\".\"Iid\", \"ReferenceDataLibrary_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"DefinedThing\".\"ValueTypeDictionary\" || \"ReferenceDataLibrary\".\"ValueTypeDictionary\"";
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
