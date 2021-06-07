// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightKindValidationService.cs" company="RHEA System S.A.">
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
    using System;
    using System.Linq;
    
    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The AccessRightKindValidation Service which validates permissions for things.
    /// </summary>
    public class AccessRightKindValidationService : IAccessRightKindValidationService
    {
        /// <summary>
        /// The owned thing.
        /// </summary>
        private const string OwnedThing = "OwnedThing";

        /// <summary>
        /// The thing.
        /// </summary>
        private const string Thing = "Thing";

        /// <summary>
        /// Gets or sets the request utils for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Check whether person permission valid.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check access right kind of.</param>
        /// <returns>
        /// The <see cref="bool"/> whether the set access right is valid.
        /// </returns>
        /// <exception cref="InvalidCastException"> If the supplied thing is not an instance of <see cref="PersonPermission"/>
        /// </exception>
        public bool IsPersonPermissionValid(Thing thing)
        {
            var personPermission = thing as PersonPermission;

            if (personPermission == null)
            {
                throw new InvalidCastException("The supplied thing is not an instance of PersonPermission.");
            }

            switch (personPermission.AccessRight)
            {
                case PersonAccessRightKind.MODIFY_IF_PARTICIPANT:
                    if (personPermission.ObjectClass == ClassKind.EngineeringModelSetup || 
                        personPermission.ObjectClass == ClassKind.IterationSetup ||
                        personPermission.ObjectClass == ClassKind.ModelReferenceDataLibrary ||
                        personPermission.ObjectClass == ClassKind.Participant)
                    {
                        return true;
                    }

                    return false;

                case PersonAccessRightKind.READ_IF_PARTICIPANT:
                    if (personPermission.ObjectClass == ClassKind.EngineeringModelSetup || 
                        personPermission.ObjectClass == ClassKind.IterationSetup ||
                        personPermission.ObjectClass == ClassKind.ModelReferenceDataLibrary ||
                        personPermission.ObjectClass == ClassKind.Participant)
                    {
                        return true;
                    }

                    return false;

                case PersonAccessRightKind.MODIFY_OWN_PERSON:
                    if (personPermission.ObjectClass == ClassKind.Person)
                    {
                        return true;
                    }

                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Check whether participant permission valid.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check access right kind of.</param>
        /// <returns>
        /// The <see cref="bool"/> whether the set access right is valid.
        /// </returns>
        /// <exception cref="InvalidCastException"> If the supplied thing is not an instance of <see cref="ParticipantPermission"/>
        /// </exception>
        public bool IsParticipantPermissionValid(Thing thing)
        {
            var participantPermission = thing as ParticipantPermission;

            if (participantPermission == null)
            {
                throw new InvalidCastException("The supplied thing is not an instance of ParticipantPermission.");
            }

            if (!this.IsOwnedClassKind(participantPermission.ObjectClass.ToString())
                && participantPermission.AccessRight == ParticipantAccessRightKind.MODIFY_IF_OWNER)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check whether classKind is owned.
        /// </summary>
        /// <param name="typeName">
        /// The string representation of the typeName to check whether it is owned.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether the classKind is owned.
        /// </returns>
        private bool IsOwnedClassKind(string typeName)
        {
            // for some reason single multiples type matching the conditions
            var type = typeof(Thing).Assembly.GetTypes().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.FullName) && x.FullName.Contains($"{typeof(Thing).Namespace}.{typeName}"));
            if (type == null)
            {
                throw new InvalidOperationException($"No type associated to classkind {typeName}");
            }

            return typeof(IOwnedThing).IsAssignableFrom(type);
        }
    }
}