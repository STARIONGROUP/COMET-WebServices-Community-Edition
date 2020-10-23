// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationPasswordCredentials.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
