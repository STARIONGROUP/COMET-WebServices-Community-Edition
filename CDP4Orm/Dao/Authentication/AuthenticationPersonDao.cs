// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPersonDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The authentication dao.
    /// </summary>
    public class AuthenticationPersonDao : IAuthenticationPersonDao
    {
        /// <summary>
        /// Gets or sets the <see cref="IPersonDao"/> (injected)
        /// </summary>
        public IPersonDao PersonDao { get; set; }

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
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="AuthenticationPerson"/>.
        /// </returns>
        public async Task<IEnumerable<AuthenticationPerson>> Read(NpgsqlTransaction transaction, string partition, string userName, DateTime? instant = null)
        {
            var result = new List<AuthenticationPerson>();

            await using var command = new NpgsqlCommand();

            var sqlBuilder = new System.Text.StringBuilder();

            sqlBuilder.Append(this.PersonDao.BuildReadQuery(partition, instant));

            if (!string.IsNullOrWhiteSpace(userName))
            {
                sqlBuilder.Append($" WHERE {this.PersonDao.GetValueTypeSet()} -> 'ShortName' = :shortname");
                command.Parameters.Add("shortname", NpgsqlDbType.Varchar).Value = userName;
            }

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
            }

            sqlBuilder.Append(';');

            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.CommandText = sqlBuilder.ToString();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var authenticationPerson = MapToDto(reader);
                result.Add(authenticationPerson);
            }

            return result;
        }

        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="userId">
        /// User Iid to retrieve from the database.
        /// </param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="AuthenticationPerson"/>.
        /// </returns>
        public async Task<IEnumerable<AuthenticationPerson>> Read(NpgsqlTransaction transaction, string partition, Guid userId, DateTime? instant = null)
        {
            var result = new List<AuthenticationPerson>();

            await using var command = new NpgsqlCommand();

            var sqlBuilder = new System.Text.StringBuilder();

            sqlBuilder.Append(this.PersonDao.BuildReadQuery(partition, instant));

            sqlBuilder.Append(" WHERE \"Iid\" = :userId");
            command.Parameters.Add("userId", NpgsqlDbType.Uuid).Value = userId;

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
            }

            sqlBuilder.Append(';');

            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.CommandText = sqlBuilder.ToString();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var authenticationPerson = MapToDto(reader);
                result.Add(authenticationPerson);
            }

            return result;        }
        
        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="AuthenticationPerson"/>.
        /// </returns>
        private static AuthenticationPerson MapToDto(NpgsqlDataReader reader)
        {
            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);
            
            var dto = new AuthenticationPerson(iid, revisionNumber)
            {
                Role = reader["Role"] is DBNull ? null : Guid.Parse(reader["Role"].ToString()),
                DefaultDomain = reader["DefaultDomain"] is DBNull? null : Guid.Parse(reader["DefaultDomain"].ToString()),
                Organization = reader["Organization"] is DBNull ? null : Guid.Parse(reader["Organization"].ToString())
            };

            if (valueDict.TryGetValue("IsActive", out var tempIsActive))
            {
                dto.IsActive = bool.Parse(tempIsActive);
            }

            if (valueDict.TryGetValue("IsDeprecated", out var tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }

            if (valueDict.TryGetValue("Password", out var tempPassword) && !string.IsNullOrEmpty(tempPassword))
            {
                dto.Password = tempPassword.UnEscape();
            }

            if (valueDict.TryGetValue("Salt", out var tempSalt))
            {
                dto.Salt = tempSalt.UnEscape();
            }

            if (valueDict.TryGetValue("ShortName", out var tempShortName))
            {
                // map shortname to UserName
                dto.UserName = tempShortName.UnEscape();
            }
            
            return dto;
        }
    }
}
