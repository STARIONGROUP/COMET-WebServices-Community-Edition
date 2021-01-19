// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObfuscationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Adrian Chivu, Cozmin Velciu, Alex Vorobiev
// 
//    This file is part of CDP4-Server-Administration-Tool.
//    The CDP4-Server-Administration-Tool is an ECSS-E-TM-10-25 Compliant tool
//    for advanced server administration.
// 
//    The CDP4-Server-Administration-Tool is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation; either version 3 of the
//    License, or (at your option) any later version.
// 
//    The CDP4-Server-Administration-Tool is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authentication;

    /// <summary>
    /// The obfuscation service obscures properties and children of Element Definitions based on OrganizationalParticipation of an EngineeringModelSetup 
    /// </summary>
    public interface IObfuscationService
    {
        /// <summary>
        /// Obfuscates the entire response
        /// </summary>
        /// <param name="resourceResponse">The list of all <see cref="Thing"/> contained in the response.</param>
        /// <param name="credentials">The <see cref="Credentials"/></param>
        void ObfuscateResponse(List<Thing> resourceResponse, Credentials credentials);
    }
}
