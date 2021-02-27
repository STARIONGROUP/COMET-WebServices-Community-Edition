﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserProvider.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft.
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

namespace CometServer.Authentication
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="UserProvider"/> is used to authenticate a User based
    /// on the provided username and password.
    /// </summary>
    public class UserProvider : IUserProvider
    {
        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="username">
        /// The username used to identify the user
        /// </param>
        /// <param name="password">
        /// the password (secret)
        /// </param>
        /// <returns>
        /// returns true when authenticated, false if not
        /// </returns>
        public Task<Guid> Authenticate(string username, string password)
        {
            var id = Guid.NewGuid();

            return Task.FromResult(id);
        }
    }
}
