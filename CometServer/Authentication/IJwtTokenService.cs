// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJwtTokenService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Authentication
{
    using CDP4Authentication;

    /// <summary>
    /// The purpose of the <see cref="IJwtTokenService"/> is to generate a JWT token based on the provided authenticated
    /// user
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Creates a JWT token based on the <see cref="AuthenticationPerson"/> and the settings provided
        /// by the <see cref="IAppConfigService"/>
        /// </summary>
        /// <param name="authenticationPerson">
        /// The subject <see cref="AuthenticationPerson"/>
        /// </param>
        /// <returns>The created JWT token</returns>
        string CreateToken(AuthenticationPerson authenticationPerson);
    }
}
