﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationOptions.cs" company="Starion Group S.A.">
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

namespace CometServer.Authentication.Basic
{
    using Microsoft.AspNetCore.Authentication;

    /// <summary>
    /// The purpose of the <see cref="BasicAuthenticationOptions"/> is to set the options for the <see cref="BasicAuthenticationHandler"/>
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the WWW-Authenticate header is suppressed on Unauthorized responses.
        /// </summary>
        /// <remarks>
        /// In case the WWWAuthenticateHeader is returned the browser will present the user with a UI to
        /// provide a username and password. In case this is suppressed the response will only be a
        /// 401 status code that the client will need to react to.
        /// </remarks>
        public bool IsWWWAuthenticateHeaderSuppressed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value for the realm
        /// </summary>
        public string Realm { get; set; } = "CDP4-COMET";
    }
}
