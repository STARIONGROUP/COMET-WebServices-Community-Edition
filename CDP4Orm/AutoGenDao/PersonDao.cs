// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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
    /// The Person Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class PersonDao : ThingDao, IPersonDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="isCachedDtoReadEnabledAndInstant">
        /// The value indicating whether to get cached last state of Dto from revision history.
        /// </param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.Person"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.Person> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"Person_Cache\"", partition);
                    sqlBuilder.Append(this.BuildJoinForActorProperty(partition));

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as Person;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.Append(this.BuildReadQuery(partition, instant));

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    if (instant.HasValue && instant.Value != DateTime.MaxValue)
                    {
                        command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return this.MapToDto(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="CDP4Common.DTO.Person"/>.
        /// </returns>
        public virtual CDP4Common.DTO.Person MapToDto(NpgsqlDataReader reader)
        {
            string tempGivenName;
            string tempIsActive;
            string tempIsDeprecated;
            string tempModifiedOn;
            string tempOrganizationalUnit;
            string tempPassword;
            string tempShortName;
            string tempSurname;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.Person(iid, revisionNumber);
            dto.Actor = reader["Actor"] is DBNull ? (Guid?)null : Guid.Parse(reader["Actor"].ToString());
            dto.DefaultDomain = reader["DefaultDomain"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultDomain"].ToString());
            dto.DefaultEmailAddress = reader["DefaultEmailAddress"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultEmailAddress"].ToString());
            dto.DefaultTelephoneNumber = reader["DefaultTelephoneNumber"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultTelephoneNumber"].ToString());
            dto.EmailAddress.AddRange(Array.ConvertAll((string[])reader["EmailAddress"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.Organization = reader["Organization"] is DBNull ? (Guid?)null : Guid.Parse(reader["Organization"].ToString());
            dto.Role = reader["Role"] is DBNull ? (Guid?)null : Guid.Parse(reader["Role"].ToString());
            dto.TelephoneNumber.AddRange(Array.ConvertAll((string[])reader["TelephoneNumber"], Guid.Parse));
            dto.UserPreference.AddRange(Array.ConvertAll((string[])reader["UserPreference"], Guid.Parse));

            if (valueDict.TryGetValue("GivenName", out tempGivenName))
            {
                dto.GivenName = tempGivenName.UnEscape();
            }

            if (valueDict.TryGetValue("IsActive", out tempIsActive))
            {
                dto.IsActive = bool.Parse(tempIsActive);
            }

            if (valueDict.TryGetValue("IsDeprecated", out tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("OrganizationalUnit", out tempOrganizationalUnit) && tempOrganizationalUnit != null)
            {
                dto.OrganizationalUnit = tempOrganizationalUnit.UnEscape();
            }

            if (valueDict.TryGetValue("Password", out tempPassword) && tempPassword != null)
            {
                dto.Password = tempPassword.UnEscape();
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
            }

            if (valueDict.TryGetValue("Surname", out tempSurname))
            {
                dto.Surname = tempSurname.UnEscape();
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
            }

            return dto;
        }

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="person">
        /// The person DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, person, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, person, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "GivenName", !this.IsDerived(person, "GivenName") ? person.GivenName.Escape() : string.Empty },
                    { "IsActive", !this.IsDerived(person, "IsActive") ? person.IsActive.ToString() : string.Empty },
                    { "IsDeprecated", !this.IsDerived(person, "IsDeprecated") ? person.IsDeprecated.ToString() : string.Empty },
                    { "OrganizationalUnit", !this.IsDerived(person, "OrganizationalUnit") ? person.OrganizationalUnit.Escape() : null },
                    { "Password", !this.IsDerived(person, "Password") ? person.Password.Escape() : null },
                    { "ShortName", !this.IsDerived(person, "ShortName") ? person.ShortName.Escape() : string.Empty },
                    { "Surname", !this.IsDerived(person, "Surname") ? person.Surname.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Person\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultDomain, :defaultEmailAddress, :defaultTelephoneNumber, :organization, :role);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = person.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultDomain", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultDomain") ? Utils.NullableValue(person.DefaultDomain) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultEmailAddress", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultEmailAddress") ? Utils.NullableValue(person.DefaultEmailAddress) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultTelephoneNumber", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultTelephoneNumber") ? Utils.NullableValue(person.DefaultTelephoneNumber) : Utils.NullableValue(null);
                    command.Parameters.Add("organization", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Organization") ? Utils.NullableValue(person.Organization) : Utils.NullableValue(null);
                    command.Parameters.Add("role", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Role") ? Utils.NullableValue(person.Role) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, person, container);
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
        /// <param name="person">
        /// The person DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, person, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "GivenName", !this.IsDerived(person, "GivenName") ? person.GivenName.Escape() : string.Empty },
                { "IsActive", !this.IsDerived(person, "IsActive") ? person.IsActive.ToString() : string.Empty },
                { "IsDeprecated", !this.IsDerived(person, "IsDeprecated") ? person.IsDeprecated.ToString() : string.Empty },
                { "OrganizationalUnit", !this.IsDerived(person, "OrganizationalUnit") ? person.OrganizationalUnit.Escape() : null },
                { "Password", !this.IsDerived(person, "Password") ? person.Password.Escape() : null },
                { "ShortName", !this.IsDerived(person, "ShortName") ? person.ShortName.Escape() : string.Empty },
                { "Surname", !this.IsDerived(person, "Surname") ? person.Surname.Escape() : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Person\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultDomain, :defaultEmailAddress, :defaultTelephoneNumber, :organization, :role)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = person.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("defaultDomain", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultDomain") ? Utils.NullableValue(person.DefaultDomain) : Utils.NullableValue(null);
                command.Parameters.Add("defaultEmailAddress", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultEmailAddress") ? Utils.NullableValue(person.DefaultEmailAddress) : Utils.NullableValue(null);
                command.Parameters.Add("defaultTelephoneNumber", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultTelephoneNumber") ? Utils.NullableValue(person.DefaultTelephoneNumber) : Utils.NullableValue(null);
                command.Parameters.Add("organization", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Organization") ? Utils.NullableValue(person.Organization) : Utils.NullableValue(null);
                command.Parameters.Add("role", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Role") ? Utils.NullableValue(person.Role) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :defaultDomain, :defaultEmailAddress, :defaultTelephoneNumber, :organization, :role);");

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
        /// <param name="person">
        /// The Person DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, person, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, person, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "GivenName", !this.IsDerived(person, "GivenName") ? person.GivenName.Escape() : string.Empty },
                    { "IsActive", !this.IsDerived(person, "IsActive") ? person.IsActive.ToString() : string.Empty },
                    { "IsDeprecated", !this.IsDerived(person, "IsDeprecated") ? person.IsDeprecated.ToString() : string.Empty },
                    { "OrganizationalUnit", !this.IsDerived(person, "OrganizationalUnit") ? person.OrganizationalUnit.Escape() : null },
                    { "Password", !this.IsDerived(person, "Password") ? person.Password.Escape() : null },
                    { "ShortName", !this.IsDerived(person, "ShortName") ? person.ShortName.Escape() : string.Empty },
                    { "Surname", !this.IsDerived(person, "Surname") ? person.Surname.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Person\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :defaultDomain, :defaultEmailAddress, :defaultTelephoneNumber, :organization, :role)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = person.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultDomain", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultDomain") ? Utils.NullableValue(person.DefaultDomain) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultEmailAddress", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultEmailAddress") ? Utils.NullableValue(person.DefaultEmailAddress) : Utils.NullableValue(null);
                    command.Parameters.Add("defaultTelephoneNumber", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "DefaultTelephoneNumber") ? Utils.NullableValue(person.DefaultTelephoneNumber) : Utils.NullableValue(null);
                    command.Parameters.Add("organization", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Organization") ? Utils.NullableValue(person.Organization) : Utils.NullableValue(null);
                    command.Parameters.Add("role", NpgsqlDbType.Uuid).Value = !this.IsDerived(person, "Role") ? Utils.NullableValue(person.Role) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, person, container);
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
        /// The <see cref="CDP4Common.DTO.Person"/> id that is to be deleted.
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
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Person\"", partition);
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
        /// The <see cref="CDP4Common.DTO.Person"/> id that is to be deleted.
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
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Person\"", partition);
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
        /// Build a SQL read query for the current <see cref="PersonDao" />
        /// </summary>
        /// <param name="partition">The database partition (schema) where the requested resource will be stored.</param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>The built SQL read query</returns>
        public override string BuildReadQuery(string partition, DateTime? instant)
        {

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"Thing\".\"Iid\",");
            sqlBuilder.AppendFormat(" {0} AS \"ValueTypeSet\",", this.GetValueTypeSet());

            sqlBuilder.Append(" \"Person\".\"Container\",");

            sqlBuilder.Append(" NULL::bigint AS \"Sequence\",");

            sqlBuilder.Append(" \"Actor\",");

            sqlBuilder.Append(" \"Person\".\"DefaultDomain\",");

            sqlBuilder.Append(" \"Person\".\"DefaultEmailAddress\",");

            sqlBuilder.Append(" \"Person\".\"DefaultTelephoneNumber\",");

            sqlBuilder.Append(" \"Person\".\"Organization\",");

            sqlBuilder.Append(" \"Person\".\"Role\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedDomain\".\"ExcludedDomain\",'{}'::text[]) AS \"ExcludedDomain\",");
            sqlBuilder.Append(" COALESCE(\"Thing_ExcludedPerson\".\"ExcludedPerson\",'{}'::text[]) AS \"ExcludedPerson\",");
            sqlBuilder.Append(" COALESCE(\"Person_EmailAddress\".\"EmailAddress\",'{}'::text[]) AS \"EmailAddress\",");
            sqlBuilder.Append(" COALESCE(\"Person_TelephoneNumber\".\"TelephoneNumber\",'{}'::text[]) AS \"TelephoneNumber\",");
            sqlBuilder.Append(" COALESCE(\"Person_UserPreference\".\"UserPreference\",'{}'::text[]) AS \"UserPreference\",");

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Person\" USING (\"Iid\")", this.GetPersonDataSql(partition, instant));

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedDomain\"::text) AS \"ExcludedDomain\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedDomain\"", this.GetThing_ExcludedDomainDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedDomain\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"Thing\" AS \"Iid\", array_agg(\"ExcludedPerson\"::text) AS \"ExcludedPerson\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"Thing_ExcludedPerson\"", this.GetThing_ExcludedPersonDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Thing\" ON \"Thing\" = \"Iid\"", this.GetThingDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"Thing\") AS \"Thing_ExcludedPerson\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"EmailAddress\".\"Container\" AS \"Iid\", array_agg(\"EmailAddress\".\"Iid\"::text) AS \"EmailAddress\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"EmailAddress\"", this.GetEmailAddressDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Person\" ON \"EmailAddress\".\"Container\" = \"Person\".\"Iid\"", this.GetPersonDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"EmailAddress\".\"Container\") AS \"Person_EmailAddress\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"TelephoneNumber\".\"Container\" AS \"Iid\", array_agg(\"TelephoneNumber\".\"Iid\"::text) AS \"TelephoneNumber\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"TelephoneNumber\"", this.GetTelephoneNumberDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Person\" ON \"TelephoneNumber\".\"Container\" = \"Person\".\"Iid\"", this.GetPersonDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"TelephoneNumber\".\"Container\") AS \"Person_TelephoneNumber\" USING (\"Iid\")");

            sqlBuilder.Append(" LEFT JOIN (SELECT \"UserPreference\".\"Container\" AS \"Iid\", array_agg(\"UserPreference\".\"Iid\"::text) AS \"UserPreference\"");
            sqlBuilder.AppendFormat(" FROM ({0}) AS \"UserPreference\"", this.GetUserPreferenceDataSql(partition, instant));
            sqlBuilder.AppendFormat(" JOIN ({0}) AS \"Person\" ON \"UserPreference\".\"Container\" = \"Person\".\"Iid\"", this.GetPersonDataSql(partition, instant));
            sqlBuilder.Append(" GROUP BY \"UserPreference\".\"Container\") AS \"Person_UserPreference\" USING (\"Iid\")");

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
            sqlBuilder.Append(" LEFT JOIN (SELECT \"Person_Audit\".\"Actor\", \"Person_Audit\".\"Iid\"");
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Person_Audit\" AS \"Person_Audit\"", partition);
            sqlBuilder.Append(" WHERE \"Person_Audit\".\"ValidTo\" = 'infinity'");
            sqlBuilder.Append(" GROUP BY \"Person_Audit\".\"Iid\", \"Person_Audit\".\"Actor\") AS \"Actor\" USING (\"Iid\")");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Gets the ValueTypeSet combination, based one ValueTypeDictionary
        /// </summary>        
        /// <returns>The ValueTypeSet combination</returns>
        public override string GetValueTypeSet() => "\"Thing\".\"ValueTypeDictionary\" || \"Person\".\"ValueTypeDictionary\"";

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
        private string GetPersonDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"Person\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"Person_Audit\"", partition);
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
        private string GetEmailAddressDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"EmailAddress\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"EmailAddress_Audit\"", partition);
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
        private string GetTelephoneNumberDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"TelephoneNumber\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"TelephoneNumber_Audit\"", partition);
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
        private string GetUserPreferenceDataSql(string partition, DateTime? instant)
        {
            var sqlBuilder = new StringBuilder();

            var fields = " \"Iid\", \"ValueTypeDictionary\", \"Container\",\"ValidFrom\",\"ValidTo\"";
            sqlBuilder.AppendFormat(" SELECT {0}", fields);
            sqlBuilder.AppendFormat(" FROM \"{0}\".\"UserPreference\"", partition);

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                sqlBuilder.Append(" WHERE \"ValidFrom\" < :instant");
                sqlBuilder.Append(" AND \"ValidTo\" >= :instant");
                sqlBuilder.Append(" UNION ALL");
                sqlBuilder.AppendFormat(" SELECT {0}", fields);
                sqlBuilder.AppendFormat(" FROM \"{0}\".\"UserPreference_Audit\"", partition);
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
