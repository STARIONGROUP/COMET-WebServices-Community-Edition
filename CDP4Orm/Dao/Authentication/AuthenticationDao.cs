// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Authentication
{
    using System;
    using System.Collections.Generic;
    using CDP4Authentication;
    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The authentication dao.
    /// </summary>
    public class AuthenticationDao : IAuthenticationDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="userName">
        /// UserName to retrieve from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="AuthenticationPerson"/>.
        /// </returns>
        public IEnumerable<AuthenticationPerson> Read(NpgsqlTransaction transaction, string partition, string userName)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"Person_View\"", partition);

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    sqlBuilder.Append(" WHERE \"ValueTypeSet\" -> 'ShortName' = :shortname");
                    command.Parameters.Add("shortname", NpgsqlDbType.Varchar).Value = userName;
                }

                sqlBuilder.Append(";");

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sqlBuilder.ToString();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToDto(reader);
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
        /// A deserialized instance of <see cref="AuthenticationPerson"/>.
        /// </returns>
        private AuthenticationPerson MapToDto(NpgsqlDataReader reader)
        {
            string tempIsActive;
            string tempPassword;
            string tempSalt;
            string tempShortName;
            
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new AuthenticationPerson(iid, revisionNumber);

            dto.Role = reader["Role"] is DBNull ? (Guid?)null : Guid.Parse(reader["Role"].ToString());
            dto.DefaultDomain = reader["DefaultDomain"] is DBNull? (Guid?)null : Guid.Parse(reader["DefaultDomain"].ToString());
            dto.Organization = reader["Organization"] is DBNull ? (Guid?)null : Guid.Parse(reader["Organization"].ToString());

            if (valueDict.TryGetValue("IsActive", out tempIsActive))
            {
                dto.IsActive = bool.Parse(tempIsActive);
            }

            if (valueDict.TryGetValue("Password", out tempPassword) && !string.IsNullOrEmpty(tempPassword))
            {
                dto.Password = tempPassword.UnEscape();
            }

            if (valueDict.TryGetValue("Salt", out tempSalt))
            {
                dto.Salt = tempSalt.UnEscape();
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                // map shortname to UserName
                dto.UserName = tempShortName.UnEscape();
            }
            
            return dto;
        }
    }
}
