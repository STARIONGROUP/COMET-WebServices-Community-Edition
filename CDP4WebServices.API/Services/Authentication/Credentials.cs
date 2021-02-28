// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Credentials.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Authentication
{
    using System;
    using System.Collections.Generic;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CometServer.Authentication;
    
    /// <summary>
    /// Contains the authenticated user credentials and basic user information.
    /// </summary>
    public class Credentials : IUserIdentity, ICredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        public Credentials()
        {
            this.PersonPermissions = new List<PersonPermission>();
            this.ParticipantPermissions = new List<ParticipantPermission>();
            this.EngineeringModelSetups = new List<EngineeringModelSetup>();
        }

        /// <summary>
        /// Gets or sets the username of the authenticated user.
        /// </summary>
        /// <value>
        /// A string containing the username.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the claims of the authenticated user.
        /// </summary>
        /// <remarks>
        /// This is redundant for our implementation but is enforced by the interface.
        /// </remarks>
        public IEnumerable<string> Claims { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Person"/>.
        /// </summary>
        public AuthenticationPerson Person { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="PersonPermission"/> that this <see cref="Person"/> has.
        /// </summary>
        public IEnumerable<PersonPermission> PersonPermissions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EngineeringModelSetup"/>
        /// </summary>
        public EngineeringModelSetup EngineeringModelSetup { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Iteration"/>
        /// </summary>
        public Iteration Iteration { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrganizationalParticipant"/>
        /// </summary>
        public OrganizationalParticipant OrganizationalParticipant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Person"/> has a <see cref="OrganizationalParticipant"/> connected to his/her <see cref="Organization"/> and that
        /// <see cref="OrganizationalParticipant"/> being the default for the <see cref="EngineeringModelSetup"/>.
        /// </summary>
        public bool IsDefaultOrganizationalParticipant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Person"/> has a <see cref="Participant"/> connected to himself for the <see cref="EngineeringModelSetup"/>.
        /// </summary>
        public bool IsParticipant { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="ParticipantPermission"/> that this <see cref="Participant"/> has.
        /// </summary>
        /// <remarks>
        /// This property is only set if the request is based on an <see cref="EngineeringModel"/>
        /// </remarks>
        public IEnumerable<ParticipantPermission> ParticipantPermissions { get; set; }

        /// <summary>
        /// Gets or sets the active <see cref="DomainOfExpertise"/> that this <see cref="Participant"/> has.
        /// </summary>
        public DomainOfExpertise DomainOfExpertise { get; set; }

        /// <summary>
        /// Gets or sets the active <see cref="Guid"/> of the <see cref="Organization"/> this <see cref="Person"/> belongs to.
        /// </summary>
        public Guid? OrganizationIid { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="List{T}"/> of <see cref="OrganizationalParticipant"/>s that this <see cref="Person"/> is applicable to.
        /// </summary>
        public List<OrganizationalParticipant> OrganizationalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="List{T}"/> of <see cref="EngineeringModelSetup"/>s that this <see cref="Person"/> is a <see cref="Participant"/> of.
        /// </summary>
        public List<EngineeringModelSetup> EngineeringModelSetups { get; set; }
    }
}
