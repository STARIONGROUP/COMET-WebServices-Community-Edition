﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Configuration;

    using MimeKit;

    using Microsoft.Extensions.Logging;

    using SmtpClient = MailKit.Net.Smtp.SmtpClient;

    /// <summary>
    /// The purpose of the <see cref="EmailService"/> used to send automated notification emails to <see cref="Person"/>s
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<EmailService> Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

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
        /// <param name="filePaths">
        /// An <see cref="IEnumerable{String}"/> of file paths of files that can be attached to the email
        /// </param>
        /// <remarks>
        /// an awaitable <see cref="Task"/>
        /// </remarks>
        public async Task Send(IEnumerable<EmailAddress> emailAddresses, string subject, string textBody, string htmlBody, IEnumerable<string> filePaths = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.AppConfigService.AppConfig.EmailService.Sender, this.AppConfigService.AppConfig.EmailService.SMTP));
            
            foreach (var emailAddress in emailAddresses)
            {
                message.To.Add(new MailboxAddress(emailAddress.Value, emailAddress.Value));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody, 
                TextBody = textBody
            };

            if ((filePaths ?? Array.Empty<string>()).Any())
            {
                var attachments = this.CreateAttachments(filePaths);

                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(this.AppConfigService.AppConfig.EmailService.SMTP, this.AppConfigService.AppConfig.EmailService.Port);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtpClient.AuthenticateAsync(this.AppConfigService.AppConfig.EmailService.UserName, this.AppConfigService.AppConfig.EmailService.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }

            this.Logger.LogDebug("{subject} - Emails sent", subject);
        }

        /// <summary>
        /// Create list of attachments based on the provided file paths
        /// </summary>
        /// <param name="filepaths">
        /// A list of path to the file that is to be attached to an email
        /// </param>
        /// <returns>
        /// An instance of <see cref="MimePart"/> that contains the attachments
        /// </returns>
        private IEnumerable<MimePart> CreateAttachments(IEnumerable<string> filepaths)
        {
            foreach (var filepath in filepaths)
            {
                if (System.IO.File.Exists(filepath))
                {
                    var attachment = new MimePart(MimeTypes.GetMimeType(filepath))
                    {
                        Content = new MimeContent(System.IO.File.OpenRead(filepath), ContentEncoding.Binary),
                        ContentDisposition = new ContentDisposition("multipart/form-data"),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(filepath)
                    };

                    yield return attachment;
                }
                else
                {
                    this.Logger.LogDebug("The file-path {filepath} does not exist, the associated attachment could not be created.", filepath);
                }
            }
        }
    }
}
