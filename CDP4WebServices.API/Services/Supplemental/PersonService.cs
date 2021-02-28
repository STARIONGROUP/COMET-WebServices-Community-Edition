// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using Npgsql;

    using System.Security;

    /// <summary>
    /// Extension for the code-generated <see cref="PersonService"/>
    /// </summary>
    public sealed partial class PersonService
    {
        /// <summary>
        /// Update user credentials after migration
        /// </summary>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="partition">The database schema</param>
        /// <param name="thing">The person <see cref="Thing" /></param>
        /// <param name="credentials">The new credentials from migration.json <see cref="MigrationPasswordCredentials" /></param>
        /// <returns>True if opperation succeeded</returns>
        public bool UpdateCredentials(NpgsqlTransaction transaction, string partition, Thing thing, MigrationPasswordCredentials credentials)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException($"The person {this.PermissionService.Credentials.Person.UserName} does not have an appropriate update permission for {thing.GetType().Name}.");
            }

            return this.PersonDao.UpdateCredentials(transaction, partition, thing as Person, credentials);
        }
    }
}
