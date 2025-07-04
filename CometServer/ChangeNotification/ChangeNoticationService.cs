// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNoticationService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CometServer.ChangeNotification
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.ChangeNotification.UserPreference;
    using CometServer.Helpers;
    using CometServer.Services.Email;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ChangeNoticationService"/> is to send email notifications to users
    /// </summary>
    public class ChangeNoticationService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<ChangeNoticationService> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IChangelogBodyComposer"/>
        /// </summary>
        public IChangelogBodyComposer ChangelogBodyComposer { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IPersonDao"/>
        /// </summary>
        public IPersonDao PersonDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IEmailService"/>
        /// </summary>
        public IEmailService EmailService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IEmailAddressDao"/>
        /// </summary>
        public IEmailAddressDao EmailAddressDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IUserPreferenceDao"/>
        /// </summary>
        public IUserPreferenceDao UserPreferenceDao { get; set; }

        /// <summary>
        /// Gets or sets the DataSource manager.
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// Executes the <see cref="ChangeNoticationService"/>, processes all 
        /// </summary>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();

            await using var connection = await this.DataSource.OpenNewConnectionAsync();

            try
            {
                var transaction = await connection.BeginTransactionAsync();

                var persons = (await this.PersonDao.ReadAsync(transaction, "SiteDirectory", null, true)).ToList();

                foreach (var person in persons)
                {
                    if (!person.IsActive)
                    {
                        continue;
                    }

                    var emailAddresses = (await this.GetEmailAdressessAsync(transaction, person)).ToList();

                    if (emailAddresses.Count == 0)
                    {
                        continue;
                    }

                    var changeNotificationSubscriptionUserPreferences = await this.GetChangeLogSubscriptionUserPreferencesAsync(transaction, person);

                    var endDateTime = GetEndDateTime(DayOfWeek.Monday);
                    var startDateTime = endDateTime.AddDays(-7);
                    var htmlStringBuilder = new StringBuilder();
                    var textStringBuilder = new StringBuilder();
                    var subject = "Weekly Changelog from COMET server";
                    htmlStringBuilder.AppendLine($"<h3>{subject}<br />{startDateTime:R} - {endDateTime:R}</h3>");
                    textStringBuilder.AppendLine($"{subject}\n{startDateTime:R} - {endDateTime:R}");

                    foreach (var changeNotificationSubscriptionUserPreference in changeNotificationSubscriptionUserPreferences)
                    {
                        if (changeNotificationSubscriptionUserPreference.Value.ChangeNotificationSubscriptions.Count != 0
                            && changeNotificationSubscriptionUserPreference.Value.ChangeNotificationReportType != ChangeNotificationReportType.None)
                        {
                            var changelogSections = (await this.ChangelogBodyComposer.CreateChangelogSectionsAsync(
                                transaction,
                                Guid.Parse(changeNotificationSubscriptionUserPreference.Key),
                                person,
                                changeNotificationSubscriptionUserPreference.Value,
                                startDateTime,
                                endDateTime
                            )).ToList();

                            htmlStringBuilder.Append(this.ChangelogBodyComposer.CreateHtmlBody(changelogSections));
                            textStringBuilder.Append(this.ChangelogBodyComposer.CreateTextBody(changelogSections));
                        }
                    }

                    await this.EmailService.Send(emailAddresses, subject, textStringBuilder.ToString(), htmlStringBuilder.ToString());
                }
            }
            catch (PostgresException postgresException)
            {
                this.Logger.LogCritical(postgresException, "Could not connect to the database to process Change Notifications. Error message: {PostgresExceptionMessage}", postgresException.Message);
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex, "The ChangeNoticationService failed");
            }
            finally
            {
                this.Logger.LogInformation("ChangeNotifications processed in {ElapsedMilliseconds} [ms]", sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Retrieves a <see cref="Person"/>'s <see cref="EmailAddress"/>es used to send the change log email to
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="Person"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of type <see cref="EmailAddress"/>
        /// </returns>
        private async Task<IEnumerable<EmailAddress>> GetEmailAdressessAsync(NpgsqlTransaction transaction, Person person)
        {
            if (person.EmailAddress.Count == 0)
            {
                return [];
            }

            var emailAddresses = (await this.EmailAddressDao.ReadAsync(transaction, "SiteDirectory", person.EmailAddress)).ToList();

            if (emailAddresses.Count == 0)
            {
                return [];
            }

            if (person.DefaultEmailAddress != null && emailAddresses.Any(x => x.Iid == person.DefaultEmailAddress.Value))
            {
                return [emailAddresses.Single(x => x.Iid == person.DefaultEmailAddress.Value)];
            }
            else
            {
                return [emailAddresses.First()];
            }

            return [];
        }

        /// <summary>
        /// Gets the end <see cref="DateTime"/> of the period in which we want to find change log data
        /// </summary>
        /// <param name="dayOfWeek">
        /// The <see cref="DayOfWeek"/> for which to return the end <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// The end <see cref="DateTime"/>
        /// </returns>
        private static DateTime GetEndDateTime(DayOfWeek dayOfWeek)
        {
            var dt = DateTime.UtcNow;
            var diff = (7 + (dt.DayOfWeek - dayOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of type <see cref="string"/> and <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </summary>
        /// <param name="transaction">
        /// The <see cref="NpgsqlConnection"/> to the database
        /// </param>
        /// <param name="person">
        /// The <see cref="Person"/>
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> of type <see cref="string"/> and <see cref="ChangeNotificationSubscriptionUserPreference"/>
        /// </returns>
        private async Task<Dictionary<string, ChangeNotificationSubscriptionUserPreference>> GetChangeLogSubscriptionUserPreferencesAsync(NpgsqlTransaction transaction, Person person)
        {
            var changeLogSubscriptions = new Dictionary<string, ChangeNotificationSubscriptionUserPreference>();

            var userPreferences =
                (await this.UserPreferenceDao
                    .ReadAsync(transaction, "SiteDirectory", person.UserPreference, true))
                .Where(x => x.ShortName.StartsWith("ChangeLogSubscriptions_"))
                .ToList();

            foreach (var userPreference in userPreferences)
            {
                var engineeringModelSuffix = userPreference.ShortName.Replace("ChangeLogSubscriptions_", "");
                var changeNotificationSubscriptionUserPreference = JsonConvert.DeserializeObject<ChangeNotificationSubscriptionUserPreference>(userPreference.Value);

                changeLogSubscriptions.Add(engineeringModelSuffix, changeNotificationSubscriptionUserPreference);
            }

            return changeLogSubscriptions;
        }
    }
}
