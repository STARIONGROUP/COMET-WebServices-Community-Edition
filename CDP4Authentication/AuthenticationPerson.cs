// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPerson.cs" company="RHEA System S.A.">
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

namespace CDP4Authentication
{
    using System;

    /// <summary>
    /// The authentication person.
    /// </summary>
    public class AuthenticationPerson
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPerson"/> class.
        /// </summary>
        /// <param name="iid">
        /// The identifier of the instance.
        /// </param>
        /// <param name="revisionNumber">
        /// The revision number of the instance.
        /// </param>
        public AuthenticationPerson(Guid iid, int revisionNumber)
        {
            this.Iid = iid;
            this.RevisionNumber = revisionNumber;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Iid { get; set; }

        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Gets or sets the Person Role id.
        /// </summary>
        public Guid? Role { get; set; }

        /// <summary>
        /// Gets or sets the default domain.
        /// </summary>
        public Guid? DefaultDomain { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public Guid? Organization { get; set; }
    }
}
