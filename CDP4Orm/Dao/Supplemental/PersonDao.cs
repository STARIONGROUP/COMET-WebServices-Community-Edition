// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
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
    }
}