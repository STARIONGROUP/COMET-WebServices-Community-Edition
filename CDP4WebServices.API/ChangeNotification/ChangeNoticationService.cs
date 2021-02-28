// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNoticationService.cs" company="RHEA System S.A.">
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

namespace CometServer.ChangeNotification
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Autofac;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.ChangeNotification.Data;
    using CometServer.ChangeNotification.UserPreference;
    using CometServer.Configuration;
    using CometServer.Services;
    using CometServer.Services.Email;

    using Newtonsoft.Json;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ChangeNoticationService"/> is to send email notifications to users
    /// </summary>
    public class ChangeNoticationService
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The DI container used to resolve the services required to interact with the database
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNoticationService"/>
        /// </summary>
        public ChangeNoticationService()
        {
            this.container = this.RegisterServices();
        }

        /// <summary>
        /// Register the required services to interact with the database
        /// </summary>
        public IContainer RegisterServices()
        {
            var builder = new ContainerBuilder();

            //wireup data model utils
            builder.RegisterType<DataModelUtils>().As<IDataModelUtils>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // wireup command logger for this request
            builder.RegisterType<CommandLogger>().As<ICommandLogger>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // wireup class meta info provider
            builder.RegisterType<MetaInfoProvider>().As<IMetaInfoProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // wireup class cdp4JsonSerializer
            builder.RegisterType<Cdp4JsonSerializer>().As<ICdp4JsonSerializer>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // the ResolveDao is used to get type info on any Thing instance based on it's unique identifier
            builder.RegisterType<ResolveDao>().As<IResolveDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // The ChanglogRetriever retrieves changelog data from the database
            builder.RegisterType<ModelLogEntryDataCreator>().As<IModelLogEntryDataCreator>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // The ChanglogRetriever retrieves changelog data from the database
            builder.RegisterType<ChangelogBodyComposer>().As<IChangelogBodyComposer>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // wireup DAO classes
            builder.RegisterAssemblyTypes(typeof(BaseDao).Assembly).Where(x => typeof(BaseDao).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            
            // Used to send emails
            builder.RegisterType<EmailService>().As<IEmailService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            return builder.Build();
        }

        /// <summary>
        /// Executes the <see cref="ChangeNoticationService"/>, processes all 
        /// </summary>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        public async Task Execute()
        {
            var sw = Stopwatch.StartNew();

            var connection = new NpgsqlConnection(Services.Utils.GetConnectionString(AppConfig.Current.Backtier.Database));

            // ensure an open connection
            if (connection.State != ConnectionState.Open)
            {
                try
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var personDao = this.container.Resolve<IPersonDao>();

                    var persons = personDao.Read(transaction, "SiteDirectory", null, true).ToList();

                    foreach (var person in persons)
                    {
                        var changelogBodyComposer = this.container.Resolve<IChangelogBodyComposer>();

                        if (!person.IsActive)
                        {
                            continue;
                        }

                        var emailAddresses = this.GetEmailAdressess(transaction, person).ToList();

                        if (!emailAddresses.Any())
                        {
                            continue;
                        }

                        var changeNotificationSubscriptionUserPreferences = this.GetChangeLogSubscriptionUserPreferences(transaction, person);

                        var endDateTime = this.GetEndDateTime(DayOfWeek.Monday);
                        var startDateTime = endDateTime.AddDays(-7);
                        var htmlStringBuilder = new StringBuilder();
                        var textStringBuilder = new StringBuilder();
                        var subject = $"Weekly Changelog from server '{AppConfig.Current.Midtier.HostName}'";
                        htmlStringBuilder.AppendLine($"<h3>{subject}<br />{startDateTime:R} - {endDateTime:R}</h3>");
                        textStringBuilder.AppendLine($"{subject}\n{startDateTime:R} - {endDateTime:R}");

                        foreach (var changeNotificationSubscriptionUserPreference in changeNotificationSubscriptionUserPreferences)
                        {
                            if (changeNotificationSubscriptionUserPreference.Value.ChangeNotificationSubscriptions.Any()
                                && changeNotificationSubscriptionUserPreference.Value.ChangeNotificationReportType != ChangeNotificationReportType.None)
                            {
                                var changelogSections = changelogBodyComposer.CreateChangelogSections(
                                    transaction,
                                    this.container,
                                    Guid.Parse(changeNotificationSubscriptionUserPreference.Key),
                                    person,
                                    changeNotificationSubscriptionUserPreference.Value,
                                    startDateTime,
                                    endDateTime
                                ).ToList();

                                htmlStringBuilder.Append(changelogBodyComposer.CreateHtmlBody(changelogSections));
                                textStringBuilder.Append(changelogBodyComposer.CreateTextBody(changelogSections));
                            }
                        }

                        var emailService = this.container.Resolve<IEmailService>();
                        await emailService.Send(emailAddresses, subject, textStringBuilder.ToString(), htmlStringBuilder.ToString());
                    }
                }
                catch (PostgresException postgresException)
                {
                    Logger.Error("Could not connect to the database to process Change Notifications. Error message: {0}", postgresException.Message);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    if (connection?.State == ConnectionState.Open)
                    {
                        await connection.CloseAsync();
                    }

                    Logger.Info($"ChangeNotifications processed in {sw.ElapsedMilliseconds} [ms]");
                }
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
        private IEnumerable<EmailAddress> GetEmailAdressess(NpgsqlTransaction transaction, Person person)
        {
            if (!person.EmailAddress.Any())
            {
                yield break;
            }

            var emailAddressDao = this.container.Resolve<IEmailAddressDao>();
            var emailAddresses = emailAddressDao.Read(transaction, "SiteDirectory", person.EmailAddress).ToList();

            if (!emailAddresses.Any())
            {
                yield break;
            }

            if (person.DefaultEmailAddress != null && emailAddresses.Any(x => x.Iid == person.DefaultEmailAddress.Value))
            {
                yield return emailAddresses.Single(x => x.Iid == person.DefaultEmailAddress.Value);
            }
            else
            {
                yield return emailAddresses.First();
            }
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
        private DateTime GetEndDateTime(DayOfWeek dayOfWeek)
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
        private Dictionary<string, ChangeNotificationSubscriptionUserPreference> GetChangeLogSubscriptionUserPreferences(NpgsqlTransaction transaction, Person person)
        {
            var userPreferenceDao = this.container.Resolve<IUserPreferenceDao>();

            var changeLogSubscriptions = new Dictionary<string, ChangeNotificationSubscriptionUserPreference>();

            var userPreferences =
                userPreferenceDao
                    .Read(transaction, "SiteDirectory", person.UserPreference, true)
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
