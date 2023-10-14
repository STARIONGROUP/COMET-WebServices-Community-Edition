// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObfuscationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Authorization
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// The obfuscation service obscures properties and children of Element Definitions based on OrganizationalParticipation of
    /// an EngineeringModelSetup
    /// </summary>
    public interface IObfuscationService
    {
        /// <summary>
        /// Obfuscates the entire response
        /// </summary>
        /// <param name="resourceResponse">The list of all <see cref="Thing" /> contained in the response.</param>
        /// <param name="credentials">The <see cref="Credentials" /></param>
        void ObfuscateResponse(List<Thing> resourceResponse, Credentials credentials);
    }
}
