// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersonDao.cs" company="Starion Group S.A.">
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

namespace CDP4Orm.Dao
{
    using System;

    using Npgsql;

    /// <summary>
    /// The Person dao Interface.
    /// </summary>
    public partial interface IPersonDao
    {
        /// <summary>
        /// Gets the password change token that is only valid in the context of this class instance lifetime.
        /// </summary>
        string PasswordChangeToken { get; }

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
        string GivenName(NpgsqlTransaction transaction, Guid personIid);

        /// <summary>
        /// Update user credentials after migration
        /// </summary>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="partition">The database schema</param>
        /// <param name="person">The person <see cref="CDP4Common.DTO.Person" /></param>
        /// <param name="credentials">The new credentials from migration.json <see cref="MigrationPasswordCredentials" /></param>
        /// <returns></returns>
        bool UpdateCredentials(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, MigrationPasswordCredentials credentials);
    }
}
