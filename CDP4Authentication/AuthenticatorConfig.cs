// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorConfig.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CDP4Authentication
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds basic authenticator plugin configuration properties. 
    /// </summary>
    /// <typeparam name="TProperties">
    /// Generic typed <see cref="IAuthenticatorProperties"/>
    /// </typeparam>
    public class AuthenticatorConfig<TProperties> where TProperties : IAuthenticatorProperties
    {
        /// <summary>
        /// Gets or sets the list of properties of this authenticator. Each <see cref="IAuthenticatorProperties"/> instance constitutes a connector.
        /// </summary>
        public List<TProperties> AuthenticatorConnectorProperties { get; set; }
    }
}
