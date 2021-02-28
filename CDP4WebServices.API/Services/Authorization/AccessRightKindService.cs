// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightKindService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Authorization
{
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.Helpers;

    using CometServer.Services.Authentication;

    /// <summary>
    /// The purpose of the <see cref="AccessRightKindService"/> is to provide a service that returns the <see cref="PersonAccessRightKind"/>
    /// and <see cref="ParticipantAccessRightKind"/> for a specific type
    /// </summary>
    /// <remarks>
    /// this is a caching service and is therefore only valid per request
    /// </remarks>
    public class AccessRightKindService : IAccessRightKindService
    {
        /// <summary>
        /// The cached <see cref="PersonAccessRightKind"/>
        /// </summary>
        private readonly Dictionary<string, PersonAccessRightKind> personAccessRightKindCache;

        /// <summary>
        /// The cached <see cref="ParticipantAccessRightKind"/>
        /// </summary>
        private readonly Dictionary<string, ParticipantAccessRightKind> participantAccessRightKindCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRightKindService"/>
        /// </summary>
        public AccessRightKindService()
        {
            this.personAccessRightKindCache = new Dictionary<string, PersonAccessRightKind>();
            this.participantAccessRightKindCache = new Dictionary<string, ParticipantAccessRightKind>();
        }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IDefaultPermissionProvider"/> for this request
        /// </summary>
        public IDefaultPermissionProvider DefaultPermissionProvider { get; set; }

        /// <summary>
        /// Queries <see cref="PersonAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="AccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="PersonAccessRightKind"/> for the supplied object type.</returns>
        public PersonAccessRightKind QueryPersonAccessRightKind(Credentials credentials, string typeName)
        {
            // the PersonAccessRightKind already exists in the personAccessRightKindCache, return it
            if (this.personAccessRightKindCache.TryGetValue(typeName, out var personAccessRightKind))
            {
                return personAccessRightKind;
            }

            // Get the person's permission and if found use it. If not, use the default and add to personAccessRightKindCache for later use
            var personPermission = credentials.PersonPermissions.FirstOrDefault(pp => pp.ObjectClass.ToString().Equals(typeName));
            personAccessRightKind = personPermission == null
                ? this.DefaultPermissionProvider.GetDefaultPersonPermission(typeName)
                : personPermission.AccessRight;

            this.personAccessRightKindCache.Add(typeName, personAccessRightKind);

            return personAccessRightKind;
        }

        /// <summary>
        /// Queries <see cref="ParticipantAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="AccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="ParticipantAccessRightKind"/> for the supplied object type.</returns>
        public ParticipantAccessRightKind QueryParticipantAccessRightKind(Credentials credentials, string typeName)
        {
            // the ParticipantAccessRightKind already exists in the participantAccessRightKindCache, return it
            if (this.participantAccessRightKindCache.TryGetValue(typeName, out var participantAccessRightKind))
            {
                return participantAccessRightKind;
            }

            // Get the particpant's permission and if found use it. If not, use the default and add to participantAccessRightKindCache for later use
            var participantPermission = credentials.ParticipantPermissions.FirstOrDefault(
                pp => pp.ObjectClass.ToString().Equals(typeName));
            participantAccessRightKind = participantPermission == null
                ? this.DefaultPermissionProvider.GetDefaultParticipantPermission(typeName)
                : participantPermission.AccessRight;

            this.participantAccessRightKindCache.Add(typeName, participantAccessRightKind);

            return participantAccessRightKind;
        }
    }
}
