// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Credentials.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authentication
{
    using System;
    using System.Collections.Generic;
    using CDP4Authentication;
    using CDP4Common.DTO;
    using CDP4WebService.Authentication;
    using Nancy.Security;

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