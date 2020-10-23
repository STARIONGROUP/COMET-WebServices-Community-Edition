// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationPasswordCredentials.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;

    /// <summary>
    /// This class is used only in the migration process updating target user password with source user password
    /// </summary>
    public class MigrationPasswordCredentials
    {

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public Guid Iid { get; private set; }

        /// <summary>
        /// Gets or sets the user password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets or sets the salt
        /// </summary>
        public string Salt { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iid"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        public MigrationPasswordCredentials(Guid iid, string password, string salt)
        {
            this.Iid = iid;
            this.Password = password;
            this.Salt = salt;
        }
    }
}
