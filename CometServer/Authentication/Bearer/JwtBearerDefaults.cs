﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtBearerDefaults.cs" company="Starion Group S.A.">
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

namespace CometServer.Authentication.Bearer
{
    /// <summary>
    /// Default values used by Basic Access JWT Bearer scheme.
    /// </summary>
    public static class JwtBearerDefaults
    {
        /// <summary>
        /// The name of the scheme used for local JWT Bearer authentication
        /// </summary>
        public const string LocalAuthenticationScheme = "LocalJwtBearer";

        /// <summary>
        /// The name of the scheme used for external JWT Bearer authentication
        /// </summary>
        public const string ExternalAuthenticationScheme = "ExternalJwtBearer";
    }
}
