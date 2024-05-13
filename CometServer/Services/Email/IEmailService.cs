// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.Email
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    /// <summary>
    /// Definition of the Email Service responsible for sending automated emails to <see cref="Person"/>s
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email with the subject body
        /// </summary>
        /// <param name="emailAddresses">
        /// An <see cref="IEnumerable{EmailAddress}"/> of the recipients of the email
        /// </param>
        /// <param name="subject">
        /// The subject of the email
        /// </param>
        /// <param name="textBody">
        /// The text part for the body of the email
        /// </param>
        /// <param name="htmlBody">
        /// The html part for the body of the email
        /// </param>
        /// /// <param name="filePaths">
        /// An <see cref="IEnumerable{String}"/> of file paths of files that can be attached to the email
        /// </param>
        /// <remarks>
        /// an awaitable <see cref="Task"/>
        /// </remarks>
        Task Send(IEnumerable<EmailAddress> emailAddresses, string subject, string textBody, string htmlBody, IEnumerable<string> filePaths = null);
    }
}
