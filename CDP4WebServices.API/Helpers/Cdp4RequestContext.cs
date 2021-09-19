// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4RequestContext.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Helpers
{
    using CDP4WebServices.API.Services.Authentication;

    using Nancy;

    /// <summary>
    /// A wrapper for the Nancy request context instance that can be injected.
    /// </summary>
    public class Cdp4RequestContext : ICdp4RequestContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4RequestContext"/> class.
        /// </summary>
        /// <param name="context">
        /// The request Context.
        /// </param>
        public Cdp4RequestContext(NancyContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public NancyContext Context { get; private set; }

        /// <summary>
        /// Gets the authenticated credentials, or null if request context is not present
        /// </summary>
        public Credentials AuthenticatedCredentials
        {
            get
            {
                return this.Context != null ? this.Context.CurrentUser as Credentials : null;
            }
        }
    }
}
