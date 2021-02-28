// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatorConnector.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
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

namespace CDP4Authentication.Contracts
{
    /// <summary>
    /// The abstract authenticator connector contract.
    /// </summary>
    /// <typeparam name="TProperties">
    /// Generic typed <see cref="IAuthenticatorProperties"/>.
    /// </typeparam>
    public abstract class AuthenticatorConnector<TProperties> : IAuthenticatorConnector
        where TProperties : IAuthenticatorProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorConnector{TProperties}"/> class.
        /// </summary>
        /// <param name="properties">
        /// The generic typed <see cref="IAuthenticatorProperties"/>.
        /// </param>
        /// <remarks>
        /// Force all concrete classes to have a constructor with properties of type T.
        /// </remarks>
        protected AuthenticatorConnector(TProperties properties)
        {
            this.ConnectorProperties = properties;
        }

        /// <summary>
        /// Gets the Connector Properties as <see cref="IAuthenticatorProperties"/>.
        /// </summary>
        public IAuthenticatorProperties Properties
        {
            get
            {
                return this.ConnectorProperties;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connection is up.
        /// </summary>
        public abstract bool IsUp { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is down.
        /// </summary>
        public abstract bool IsDown { get; set; }

        /// <summary>
        /// Gets or sets the status message of the <see cref="IAuthenticatorConnector"/>.
        /// </summary>
        public abstract string StatusMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="IAuthenticatorConnector"/> name for display purposes.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the generic typed connector properties to be used in any connector logic specific to type T.
        /// </summary>
        protected TProperties ConnectorProperties { get; private set; }

        /// <summary>
        /// Authenticate the <see cref="AuthenticationPerson"/> information against the supplied password.
        /// </summary>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> information to authenticate.
        /// </param>
        /// <param name="password">
        /// The password to authenticate against.
        /// </param>
        /// <returns>
        /// True if the <see cref="AuthenticationPerson"/> could be authenticated.
        /// </returns>
        public abstract bool Authenticate(AuthenticationPerson person, string password);
    }
}
