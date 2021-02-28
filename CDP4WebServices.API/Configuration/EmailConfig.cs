// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailConfig.cs" company="RHEA System S.A.">
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

namespace CometServer.Configuration
{
    /// <summary>
    /// The Email Service configuration.
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailConfig"/> class
        /// </summary>
        public EmailConfig()
        {
            this.Sender = "CDP4";
        }

        /// <summary>
        /// Gets or sets the name of the sender of any emails
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the host name of the SMTP server.
        /// </summary>
        public string SMTP { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the SMTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name of the account that will connect with the SMPT server
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password of the account that will connect with the SMPT server
        /// </summary>
        public string Password { get; set; }
    }
}
