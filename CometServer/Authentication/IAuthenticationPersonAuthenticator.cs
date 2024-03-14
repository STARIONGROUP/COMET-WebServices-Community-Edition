// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationPersonAuthenticator.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace CometServer.Authentication
{
    using System.Threading.Tasks;

    using CDP4Authentication;

    /// <summary>
    /// The purpose of the <see cref="IAuthenticationPersonAuthenticator"/> is to authenticate a <see cref="AuthenticationPerson"/>
    /// against the E-TM-10-25 datasource and associated authentication plugin
    /// </summary>
    public interface IAuthenticationPersonAuthenticator
    {
        /// <summary>
        /// Authenticates the <see cref="AuthenticationPerson"/> from the E-TM-10-25 datasource
        /// </summary>
        /// <param name="username">
        /// the username of the <see cref="AuthenticationPerson"/> that is to be authenticated
        /// </param>
        /// <param name="password"></param>
        /// the password of the <see cref="AuthenticationPerson"/> that is to be authenticated
        /// <returns>
        /// an instance of <see cref="AuthenticationPerson"/> or null if not found
        /// </returns>
        public Task<AuthenticationPerson> Authenticate(string username, string password);
    }
}
