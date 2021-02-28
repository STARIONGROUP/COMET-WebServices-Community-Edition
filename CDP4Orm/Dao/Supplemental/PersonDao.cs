// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;


    /// <summary>
    /// The Person Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class PersonDao
    {
        /// <summary>
        /// The password change token is generated and only valid in the context of this class instance lifetime.
        /// </summary>
        private readonly string passwordChangeToken = string.Format("_{0}_", Guid.NewGuid());

        /// <summary>
        /// Gets the password change token that is only valid in the context of this class instance lifetime.
        /// </summary>
        public string PasswordChangeToken
        {
            get
            {
                return this.passwordChangeToken;
            }
        }

        /// <summary>
        /// The before update.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="thing">
        /// The thing.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="isHandled">
        /// The is handled.
        /// </param>
        /// <param name="valueTypeDictionaryAdditions">
        /// This dictionary instance can be used to add additional value (non reference) variables that are to be persisted as a HSTORE implementation together with the supplied <see cref="Thing"/> instance.
        /// Developers are required to take care to add property keys that are not already in the ValueTypeDictionary that is managed in the respective generated partial class.
        /// The supplied values will be persisted as is and thus must be in a valid format (escaped) that can be persisted to SQL.
        /// Even though the additional stored variables are read from the data-store, developers will have to add custom DTO mapping to get retrieve the value again.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool BeforeUpdate(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, out bool isHandled, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            var person = (Person)thing;
            var isPasswordChangeRequest = person.Password.StartsWith(this.passwordChangeToken) && person.Password.EndsWith(this.passwordChangeToken);

            if (isPasswordChangeRequest)
            {
                this.ExtractPasswordFromTokenizedString(person);
                this.ApplyPasswordChange(person, valueTypeDictionaryAdditions);
            }
            else
            {
                // if password is not being changed, salt should be re-persisted
                this.ExtractExistingSaltToValueDictionary(transaction, partition, person, valueTypeDictionaryAdditions);
            }

            isHandled = false;
            return true;
        }

        /// <summary>
        /// Execute additional logic before each write function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <param name="isHandled">
        /// Logic flag that can be set to true to skip the generated write logic
        /// </param>
        /// <param name="valueTypeDictionaryAdditions">
        /// This dictionary instance can be used to add additional value (non reference) variables that are to be persisted as a HSTORE implementation together with the supplied <see cref="Thing"/> instance.
        /// Developers are required to take care to add property keys that are not already in the ValueTypeDictionary that is managed in the respective generated partial class.
        /// The supplied values will be persisted as is and thus must be in a valid format (escaped) that can be persisted to SQL.
        /// Even though the additional stored variables are read from the data-store, developers will have to add custom DTO mapping to get retrieve the value again.
        /// </param>
        /// <returns>
        /// True if the concept was persisted.
        /// </returns>
        public new bool BeforeWrite(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, out bool isHandled, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            this.ApplyPasswordChange(thing, valueTypeDictionaryAdditions);

            isHandled = false;
            return true;
        }

        /// <summary>
        /// Gets the given name of a person.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="personIid">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GivenName(NpgsqlTransaction transaction, Guid personIid)
        {
            var partition = "SiteDirectory";
            var personObject = this.Read(transaction, partition).SingleOrDefault(person => person.Iid == personIid);

            return personObject != null ? personObject.GivenName : null;
        }

        /// <summary>
        /// Extracts the salt property from the hstore in the <see cref="Person"/> beiung updated and adds it to the the valueType dictionary
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="person">The relevant <see cref="Person"/></param>
        /// <param name="valueTypeDictionaryAdditions">
        /// This dictionary instance can be used to add additional value (non reference) variables that are to be persisted as a HSTORE implementation together with the supplied <see cref="Thing"/> instance.
        /// Developers are required to take care to add property keys that are not already in the ValueTypeDictionary that is managed in the respective generated partial class.
        /// The supplied values will be persisted as is and thus must be in a valid format (escaped) that can be persisted to SQL.
        /// Even though the additional stored variables are read from the data-store, developers will have to add custom DTO mapping to get retrieve the value again.
        /// </param>
        private void ExtractExistingSaltToValueDictionary(NpgsqlTransaction transaction, string partition, Person person, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            if(person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("SELECT \"ValueTypeSet\" FROM \"{0}\".\"Person_View\"", partition);

                if (person != null)
                {
                    sqlBuilder.Append(" WHERE \"Iid\" =:id");
                    command.Parameters.Add("id", NpgsqlDbType.Uuid).Value = person.Iid;
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
                        var valueDict = (Dictionary<string, string>) reader["ValueTypeSet"];

                        if (valueDict.TryGetValue("Salt", out var existingSalt))
                        {
                            valueTypeDictionaryAdditions.Add("Salt", existingSalt.UnEscape());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extract and re-assign the 'clear-text' password from the tokenized string.
        /// </summary>
        /// <param name="person">
        /// The person instance.
        /// </param>
        internal void ExtractPasswordFromTokenizedString(Person person)
        {
            person.Password = person.Password.Replace(this.passwordChangeToken, string.Empty);
        }

        /// <summary>
        /// The apply password change.
        /// </summary>
        /// <param name="thing">
        /// The thing.
        /// </param>
        /// <param name="valueTypeDictionaryAdditions">
        /// The value type dictionary additions.
        /// </param>
        internal void ApplyPasswordChange(Thing thing, Dictionary<string, string> valueTypeDictionaryAdditions)
        {
            var person = (Person)thing;

            if (person.Password == null)
            {
                person.Password = EncryptionUtils.GenerateRandomSaltString();
            }

            // encrypt the password as a salted hash
            var salt = EncryptionUtils.GenerateRandomSaltString();
            var saltedPassword = EncryptionUtils.GenerateSaltedString(person.Password, salt);

            // override the salted password of the person object, so it will be persisted
            person.Password = saltedPassword;

            // add the salt as a valuetype addition for persistence
            valueTypeDictionaryAdditions.Add("Salt", salt);
        }

        /// <summary>
        /// Update user credentials after migration
        /// </summary>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="partition">The database schema</param>
        /// <param name="person">The person <see cref="CDP4Common.DTO.Person" /></param>
        /// <param name="credentials">The new credentials from migration.json <see cref="MigrationPasswordCredentials" /></param>
        /// <returns>
        /// The true if operation finished with success
        /// </returns>
        public bool UpdateCredentials(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, MigrationPasswordCredentials credentials)
        {
            var operationSuccess = true;
            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "GivenName", !this.IsDerived(person, "GivenName") ? person.GivenName.Escape() : string.Empty },
                { "IsActive", !this.IsDerived(person, "IsActive") ? person.IsActive.ToString() : string.Empty },
                { "IsDeprecated", !this.IsDerived(person, "IsDeprecated") ? person.IsDeprecated.ToString() : string.Empty },
                { "OrganizationalUnit", !this.IsDerived(person, "OrganizationalUnit") ? person.OrganizationalUnit.Escape() : null },
                { "Password", !this.IsDerived(person, "Password") ? credentials.Password.Escape() : null },
                { "Salt", !this.IsDerived(person, "Salt") ? credentials.Salt.Escape() : null },
                { "ShortName", !this.IsDerived(person, "ShortName") ? person.ShortName.Escape() : string.Empty },
                { "Surname", !this.IsDerived(person, "Surname") ? person.Surname.Escape() : string.Empty },
            };

            try
            {
                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();

                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Person\"", partition);
                    sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\" = :valueTypeDictionary");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");
                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = credentials.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }
            catch
            {
                operationSuccess = false;
            }

            return operationSuccess;
        }
    }
}
