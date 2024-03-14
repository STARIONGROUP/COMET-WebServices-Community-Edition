// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangelogBodyComposer.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace CometServer.ChangeNotification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.ChangeNotification.Data;
    using CometServer.ChangeNotification.UserPreference;

    using Npgsql;

    /// <summary>
    /// of a change log body composer that composes the body of an email that contais change log information
    /// </summary>
    public class ChangelogBodyComposer : IChangelogBodyComposer
    {
        /// <summary>
        /// The (injected) <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// The (injected) <see cref="IParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// The (injected) <see cref="IModelLogEntryDao"/>
        /// </summary>
        public IModelLogEntryDao ModelLogEntryDao { get; set; }

        /// <summary>
        /// The (injected) <see cref="IModelLogEntryDataCreator"/>
        /// </summary>
        public IModelLogEntryDataCreator ModelLogEntryDataCreator { get; set; }

        /// <summary>
        /// Creates the body of the email in text form
        /// </summary>
        /// <param name="changeLogSections">
        /// The <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/> that contains all data to show in the HTML body
        /// </param>
        /// <returns>
        /// The email body text as plain text. 
        /// </returns>
        public string CreateTextBody(IEnumerable<ChangelogSection> changeLogSections)
        {
            var stringBuilder = new StringBuilder();

            foreach (var section in changeLogSections)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(section.Title);
                stringBuilder.AppendLine(section.SubTitle);
                stringBuilder.AppendLine(section.Description);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Creates the body of the email in html form
        /// </summary>
        /// <param name="changeLogSections">
        /// The <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/> that contains all data to show in the HTML body
        /// </param>
        /// <returns>
        /// The email body text as HTML. 
        /// </returns>
        public string CreateHtmlBody(IEnumerable<ChangelogSection> changeLogSections)
        {
            var stringBuilder = new StringBuilder();

            foreach (var section in changeLogSections)
            {
                stringBuilder.AppendLine("<hr />");
                stringBuilder.AppendLine($"<h2>{section.Title}</h2>");
                stringBuilder.AppendLine($"<h3>{section.SubTitle}</h3>");
                stringBuilder.AppendLine($"<p>{section.Description.Replace(Environment.NewLine, "<br />")}</p>");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Create an <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/>s to be used to compose the email body
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="engineeringModelIid">
        /// The <see cref="EngineeringModel.Iid"/> property of the related <see cref="EngineeringModel"/>
        /// </param>
        /// <param name="person">
        /// The <see cref="Person"/> for whom to compose the email
        /// </param>
        /// <param name="changeNotificationSubscriptionUserPreference">
        /// The <see cref="ChangeNotificationSubscriptionUserPreference"/> that contains the change notification subscriptions
        /// </param>
        /// <param name="startDateTime">
        /// The start <see cref="DateTime"/> of the period we want to collect change log rows for.
        /// </param>
        /// <param name="endDateTime">
        /// The end <see cref="DateTime"/> of the period we want to collect change log rows for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of type <see cref="ChangelogSection"/>s
        /// </returns>
        public IEnumerable<ChangelogSection> CreateChangelogSections(
            NpgsqlTransaction transaction, 
            Guid engineeringModelIid, 
            Person person, 
            ChangeNotificationSubscriptionUserPreference changeNotificationSubscriptionUserPreference,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            var partition = $"EngineeringModel_{engineeringModelIid.ToString().Replace("-", "_")}";

            // if a model does not exist anymore, do not send report
            var engineeringModelSetup = 
                this.EngineeringModelSetupDao.Read(transaction, "SiteDirectory")
                    .FirstOrDefault(x => x.EngineeringModelIid == engineeringModelIid);

            if (engineeringModelSetup == null)
            {
                yield return CreateEngineeringModelNotFoundSection(changeNotificationSubscriptionUserPreference);

                yield break;
            }

            // if a user is no longer a participant in a model, or if the participant is not active, then do not send report
            var participants =
                this.ParticipantDao
                    .Read(transaction, "SiteDirectory")
                    .Where(x => x.Person == person.Iid && x.IsActive)
                    .ToList();

            if (participants.Count == 0)
            {
                yield return CreateParticipantNotActiveSection(engineeringModelSetup);

                yield break;
            }

            var engineeringModelParticipants = 
                participants
                    .Where(x => engineeringModelSetup.Participant.Contains(x.Iid))
                    .ToList();

            if (engineeringModelParticipants.Count == 0)
            {
                yield return CreateNoEngineeringModelParticipantSection(engineeringModelSetup);

                yield break;
            }

            var domains =
                participants
                    .SelectMany(x => x.Domain)
                    .Distinct()
                    .ToList();

            if (domains.Count == 0)
            {
                yield return CreateNoDomainOfExpertiseSection(engineeringModelSetup);

                yield break;
            }

            var modelLogEntries = 
                this.ModelLogEntryDao
                    .Read(transaction, partition)
                    .Where(x => 
                        x.ModifiedOn >= startDateTime
                        && x.ModifiedOn < endDateTime)
                    .ToList();

            if (modelLogEntries.Count == 0 || !modelLogEntries.SelectMany(x => x.LogEntryChangelogItem).Any())
            {
                yield return CreateNoModelLogEntriesSection(engineeringModelSetup);

                yield break;
            }

            if (!modelLogEntries.Any(x => x.AffectedDomainIid.Intersect(domains).Any()))
            {
                yield return CreateNoRelevantChangesFoundSection(engineeringModelSetup);

                yield break;
            }

            var filteredModelLogEntries = FilterDomains(modelLogEntries, domains);

            var modelLogEntryData = this.ModelLogEntryDataCreator.Create(transaction, partition, filteredModelLogEntries, domains, changeNotificationSubscriptionUserPreference);

            var changeNotificationSubscriptionDataGroups =
                modelLogEntryData
                    .SelectMany(x =>
                        x.LogEntryChangelogItemData
                            .SelectMany(y =>
                                y.ChangeNotificationSubscriptionData))
                    .GroupBy(x => x.ChangeNotificationSubscription, x => x.LogEntryChangelogItemData)
                    .OrderBy(x => x.Key.ChangeNotificationSubscriptionType)
                    .ThenBy(x => x.Key.ClassKind)
                    .ThenBy(x => x.Key.Name);

            foreach (var changeNotificationSubscriptionGroup in changeNotificationSubscriptionDataGroups)
            {
                var changeNotificationSubscriptionData = changeNotificationSubscriptionGroup.Key;
                var subTitle = $"{changeNotificationSubscriptionData.Name} / {changeNotificationSubscriptionData.ClassKind} / {changeNotificationSubscriptionData.ChangeNotificationSubscriptionType}";
                var descriptionBuilder = new StringBuilder();

                foreach (var logEntryChangelogItemData in changeNotificationSubscriptionGroup.OrderBy(x => x.ModelLogEntryData.ModifiedOn))
                {
                    descriptionBuilder.AppendLine($"{logEntryChangelogItemData.ModelLogEntryData.ModifiedOn}");
                    descriptionBuilder.AppendLine($"{logEntryChangelogItemData.ModelLogEntryData.JustificationText}");
                    descriptionBuilder.AppendLine($"{logEntryChangelogItemData.ChangelogKind}");
                    descriptionBuilder.AppendLine($"{logEntryChangelogItemData.ChangeDescription}");
                    descriptionBuilder.AppendLine("");
                }

                yield return new ChangelogSection($"{engineeringModelSetup.Name}", subTitle, descriptionBuilder.ToString());
            }
        }

        /// <summary>
        /// Filter a <see cref="List{T}"/> of type <see cref="ModelLogEntry"/> on the presence of any <see cref="DomainOfExpertise.Iid"/>
        /// in its <see cref="ModelLogEntry.AffectedDomainIid"/>
        /// </summary>
        /// <param name="modelLogEntries">
        /// The <see cref="ModelLogEntry"/>s
        /// </param>
        /// <param name="domains">
        /// <see cref="DomainOfExpertise.Iid"/>s
        /// </param>
        /// <returns>
        /// The filtered <see cref="List{T}"/> of type <see cref="ModelLogEntry"/>.
        /// </returns>
        private static List<ModelLogEntry> FilterDomains(List<ModelLogEntry> modelLogEntries, List<Guid> domains)
        {
            return modelLogEntries.Where(x => x.AffectedDomainIid.Intersect(domains).Any()).ToList();
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that no relevant changes were found
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which to create the <see cref="ChangelogSection"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateNoRelevantChangesFoundSection(EngineeringModelSetup engineeringModelSetup)
        {
            return new ChangelogSection(
                $"{engineeringModelSetup.Name}",
                "Not Found",
                "No relevant change log items found.");
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that no change log entries were found
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which to create the <see cref="ChangelogSection"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateNoModelLogEntriesSection(EngineeringModelSetup engineeringModelSetup)
        {            
            return new ChangelogSection(
                $"{engineeringModelSetup.Name}", 
                "Not Found", 
                "No change log entries found.");
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that the <see cref="EngineeringModelSetup.ActiveDomain"/> doesn't contain a
        /// <see cref="DomainOfExpertise"/> that matches the <see cref="Person"/>s active <see cref="DomainOfExpertise"/>s.
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which to create the <see cref="ChangelogSection"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateNoDomainOfExpertiseSection(EngineeringModelSetup engineeringModelSetup)
        {
            return new ChangelogSection(
                    $"{engineeringModelSetup.Name}", 
                    "Failed", 
                    "No active DomainOfExpertise.");
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that the <see cref="Participant"/> wasn't found in <see cref="EngineeringModelSetup.Participant"/>
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which to create the <see cref="ChangelogSection"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateNoEngineeringModelParticipantSection(EngineeringModelSetup engineeringModelSetup)
        {
            return new ChangelogSection(
                    $"{engineeringModelSetup.Name}", 
                    "Failed", 
                    "Participant not found.");
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that the <see cref="Participant"/> found in <see cref="EngineeringModelSetup.Participant"/> isn't active
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which to create the <see cref="ChangelogSection"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateParticipantNotActiveSection(EngineeringModelSetup engineeringModelSetup)
        {
            return 
                new ChangelogSection(
                    $"{engineeringModelSetup.Name}", 
                    "Failed", 
                    "Participant not active.");
        }

        /// <summary>
        /// Create a <see cref="ChangelogSection"/> that states that the <see cref="EngineeringModel"/> wasn't found in the database (anymore)
        /// </summary>
        /// <param name="changeNotificationSubscriptionUserPreference">
        /// The <see cref="ChangeNotificationSubscriptionUserPreference"/> 
        /// </param>
        /// <returns>
        /// The created <see cref="ChangelogSection"/>
        /// </returns>
        private static ChangelogSection CreateEngineeringModelNotFoundSection(ChangeNotificationSubscriptionUserPreference changeNotificationSubscriptionUserPreference)
        {
            return 
                new ChangelogSection(
                    $"{changeNotificationSubscriptionUserPreference.Name}", 
                    "Failed", 
                    "EngineeringModel not found");
        }
    }
}
