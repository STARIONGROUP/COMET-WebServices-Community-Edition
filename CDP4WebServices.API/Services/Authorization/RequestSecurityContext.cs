// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestSecurityContext.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;

    using CDP4WebServices.API.Services.Authentication;

    /// <summary>
    /// Request security context class that holds security data relevant for authorization checks
    /// </summary>
    public class RequestSecurityContext : ISecurityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSecurityContext"/> class.
        /// </summary>
        public RequestSecurityContext()
        {
            // set container authorization defaults
            this.ContainerReadAllowed = false;
            this.ContainerWriteAllowed = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for reading.
        /// </summary>
        public bool ContainerReadAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container was authorized for writing.
        /// </summary>
        public bool ContainerWriteAllowed { get; set; }

        /// <summary>
        /// Gets or sets the top container for this request context.
        /// </summary>
        [Obsolete]
        public string TopContainer { get; set; }

        /// <summary>
        /// Gets or sets the Credentials
        /// </summary>
        public Credentials Credentials { get; set; }
    }
}
