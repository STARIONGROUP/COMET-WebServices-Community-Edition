// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4DatabaseAuthenticatorConnector.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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

namespace CDP4DatabaseAuthentication
{
    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    /// <summary>
    /// A connector for basic authentication against a CDP4 database.
    /// </summary>
    public class Cdp4DatabaseAuthenticatorConnector : AuthenticatorConnector<AuthenticatorProperties>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4DatabaseAuthenticatorConnector"/> class. 
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public Cdp4DatabaseAuthenticatorConnector(AuthenticatorProperties properties)
            : base(properties)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the connector is up.
        /// </summary>
        public override bool IsUp 
        {
            get
            {
                this.IsDown = false;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connector is down.
        /// </summary>
        public override bool IsDown { get; set; }

        /// <summary>
        /// Gets or sets the status message of the <see cref="IAuthenticatorConnector"/>.
        /// </summary>
        public override string StatusMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="IAuthenticatorConnector"/> name for display purposes.
        /// </summary>
        public override string Name
        {
            get { return "COMET Database Authenticator"; }
        }

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
        public override bool Authenticate(AuthenticationPerson person, string password)
        {
            return this.ValidatePassword(password, person);
        }

        /// <summary>
        /// Verifies that the password that the login has provided is correct.
        /// </summary>
        /// <param name="password">
        /// The input password to test.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> that this password should authenticate against.
        /// </param>
        /// <returns>
        /// True if the passwords match.
        /// </returns>
        private bool ValidatePassword(string password, AuthenticationPerson person)
        {
            return EncryptionUtils.CompareSaltedStrings(password, person.Password, person.Salt);
        }
    }
}
