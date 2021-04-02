// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICredentials.cs" company="RHEA System S.A.">
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

namespace CometServer.Authentication
{
    using System.Collections.Generic;

    using CDP4Authentication;

    using CDP4Common.DTO;

    /// <summary>
    /// The Credentials interface. Used purely to allow use of the Credentials object within this project without direct knowledge of it.
    /// </summary>
    public interface ICredentials
    {
        /// <summary>
        /// Gets or sets the username of the authenticated user.
        /// </summary>
        /// <value>
        /// A string containing the username.
        /// </value>
        public string UserName { get; }

        /// <summary>
        /// Gets or sets the <see cref="Person"/>.
        /// </summary>
        public AuthenticationPerson Person { get; }

        /// <summary>
        /// Gets or sets the list of <see cref="PersonPermission"/> that this <see cref="Person"/> has.
        /// </summary>
        public IEnumerable<PersonPermission> PersonPermissions { get; }

        /// <summary>
        /// Gets or sets the list of <see cref="ParticipantPermission"/> that this <see cref="Participant"/> has.
        /// </summary>
        /// <remarks>
        /// This property is only set if the request is based on an <see cref="EngineeringModel"/>
        /// </remarks>
        public IEnumerable<ParticipantPermission> ParticipantPermissions { get; }

        /// <summary>
        /// Gets or sets the <see cref="OrganizationalParticipant"/>
        /// </summary>
        public OrganizationalParticipant OrganizationalParticipant { get; }
    }
}
