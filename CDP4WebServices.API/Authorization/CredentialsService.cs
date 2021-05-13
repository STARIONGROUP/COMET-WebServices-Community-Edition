// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IiCredentialsResolver.cs" company="RHEA System S.A.">
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

namespace CometServer.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Authentication;

    using CometServer.Exceptions;
    using CometServer.Services;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The prupose of the <see cref="CredentialsService"/> is to resolve a username into an instance
    /// of the <see cref="Credentials"/> class. Once these have been resolved the <see cref="CredentialsService"/>,
    /// or rather the <see cref="ICredentialsService"/> exposes this as a property for use in authorization logic
    /// </summary>
    public class CredentialsService : ICredentialsService
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the (injected) <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="PersonPermissionDao"/>
        /// </summary>
        public IPersonPermissionDao PersonPermissionDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="PersonRoleDao"/>
        /// </summary>
        public IPersonRoleDao PersonRoleDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ParticipantRoleDao"/>
        /// </summary>
        public IParticipantRoleDao ParticipantRoleDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="DomainOfExpertiseDao"/>
        /// </summary>
        public IDomainOfExpertiseDao DomainOfExpertiseDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ParticipantPermissionDao"/>
        /// </summary>
        public IParticipantPermissionDao ParticipantPermissionDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="OrganizationalParticipantDao"/>
        /// </summary>
        public IOrganizationalParticipantDao OrganizationalParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="AuthenticationPersonDao"/>
        /// </summary>
        public IAuthenticationPersonDao AuthenticationPersonDao { get; set; }

        /// <summary>
        /// The backing field for the <see cref="Credentials"/> property
        /// </summary>
        private Credentials credentials;

        /// <summary>
        /// Gets the <see cref="Credentials"/> that
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// thrown when the <see cref="Credentials"/> is null
        /// </exception>
        public Credentials Credentials
        {
            get
            {
                if (this.credentials == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return this.credentials;
                }
            }
        }

        /// <summary>
        /// Resolves the username to <see cref="Credentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="username">
        /// The supplied username
        /// </param>
        public async Task ResolveCredentials(NpgsqlTransaction transaction, string username)
        {
            var persons = await this.AuthenticationPersonDao.Read(transaction, "SiteDirectory", username);
            var person = persons.SingleOrDefault();

            if (person == null)
            {
                throw new AuthorizationException($"The user {username} could not be authorized");
            }

            this.credentials = this.ResolveCredentials(transaction, person);

            if (this.credentials == null)
            {
                throw new AuthorizationException($"The user {username} could not be authorized");
            }
        }

        /// <summary>
        /// Resolves all the needed information on from the <see cref="Person"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="Person"/> that needs to be resolved.
        /// </param>
        /// <returns>
        /// A completely resolved <see cref="Credentials"/> class.
        /// </returns>
        private Credentials ResolveCredentials(NpgsqlTransaction transaction, AuthenticationPerson person)
        {
            var personPermissions = new List<PersonPermission>();
            var engineeringModelSetups = this.GetEngineeringModelSetups(transaction, person).ToList();

            if (person.Role != null)
            {
                personPermissions = this.GetPersonPermissions(transaction, person).ToList();
            }

            var allOrganizationalParticipants = engineeringModelSetups.SelectMany(ems => ems.OrganizationalParticipant).ToList();

            var personOrganizationalParticipants = new List<OrganizationalParticipant>();

            // get org participation if the models have it enabled and person is part of an organization
            if (allOrganizationalParticipants.Any() && person.Organization != null)
            {
                personOrganizationalParticipants = this.GetPersonOrganizationalParticipants(person.Organization.Value, allOrganizationalParticipants, transaction);
            }

            return new Credentials
            {
                UserName = person.UserName,
                Person = person,
                PersonPermissions = personPermissions,
                ParticipantPermissions = new List<ParticipantPermission>(),
                EngineeringModelSetups = engineeringModelSetups,
                OrganizationIid = person.Organization,
                OrganizationalParticipants = personOrganizationalParticipants
            };
        }

        /// <summary>
        /// Resolve and set participant information for the passed in <see cref="Credentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        public void ResolveParticipantCredentials(NpgsqlTransaction transaction)
        {
            if (this.credentials?.Person == null || this.credentials.EngineeringModelSetup == null)
            {
                return;
            }

            var participant = this.GetParticipant(transaction, this.credentials.Person, this.credentials.EngineeringModelSetup);
            if (participant == null)
            {
                return;
            }

            // Set participant information for the resolved participant
            this.credentials.ParticipantPermissions = this.GetParticipantPermissions(transaction, participant);
            this.credentials.DomainOfExpertise = this.GetSelectedDomain(transaction, participant);
            this.credentials.IsParticipant = true;

            // take care of organizational participation
            // reset to default
            this.credentials.OrganizationalParticipant = null;
            this.credentials.IsDefaultOrganizationalParticipant = false;

            // recompute organizational participation
            var engineeringModelSetups = this.GetEngineeringModelSetups(transaction, this.credentials.Person).ToList();
            var allOrganizationalParticipants = engineeringModelSetups.SelectMany(ems => ems.OrganizationalParticipant).ToList();

            // get org participation if the models have it enabled and person is part of an organization
            if (allOrganizationalParticipants.Any() && this.credentials.OrganizationIid != null)
            {
                var personOrganizationalParticipants = this.GetPersonOrganizationalParticipants(this.credentials.OrganizationIid.Value, allOrganizationalParticipants, transaction);
                this.credentials.OrganizationalParticipants = personOrganizationalParticipants;
            }

            if (this.credentials.EngineeringModelSetup.OrganizationalParticipant.Any() && this.credentials.OrganizationIid != null && this.credentials.OrganizationalParticipants.Any())
            {
                // transient settings for the particular EMS
                // find the org participant
                var organizationalParticipantIid = this.credentials.EngineeringModelSetup.OrganizationalParticipant.Intersect(this.credentials.OrganizationalParticipants.Select(op => op.Iid)).SingleOrDefault();

                if (organizationalParticipantIid == Guid.Empty)
                {
                    return;
                }

                this.credentials.OrganizationalParticipant = this.credentials.OrganizationalParticipants.First(op => op.Iid.Equals(organizationalParticipantIid));
                this.credentials.IsDefaultOrganizationalParticipant = this.credentials.OrganizationalParticipant.Iid.Equals(this.credentials.EngineeringModelSetup.DefaultOrganizationalParticipant);
            }
        }

        /// <summary>
        /// Retrieves the <see cref="Participant"/> connected to this person and <see cref="EngineeringModelSetup"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> to resolve permission for.
        /// </param>
        /// <param name="modelSetup">
        /// The <see cref="EngineeringModelSetup"/> for which participant information is needed.
        /// </param>
        /// <returns>
        /// The <see cref="Participant"/> of the specified <see cref="AuthenticationPerson"/> in the <see cref="EngineeringModelSetup"/>.
        /// </returns>
        private Participant GetParticipant(NpgsqlTransaction transaction, AuthenticationPerson person, EngineeringModelSetup modelSetup)
        {
            try
            {
                var participants = this.ParticipantDao.Read(transaction, "SiteDirectory", modelSetup.Participant).ToList();
                return participants.FirstOrDefault(p => p.Person.Equals(person.Iid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the participant from the backtier");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="DomainOfExpertise"/> selected by a <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="participant">
        /// The <see cref="Participant"/> for which domain information is needed.
        /// </param>
        /// <returns>
        /// The <see cref="DomainOfExpertise"/> of the specified <see cref="Participant"/>.
        /// </returns>
        private DomainOfExpertise GetSelectedDomain(NpgsqlTransaction transaction, Participant participant)
        {
            try
            {
               return this.DomainOfExpertiseDao.Read(transaction, "SiteDirectory", new List<Guid> { participant.SelectedDomain }, false).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the selected domain from the backtier");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="ParticipantPermission"/>s connected to this person.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="participant">
        /// The <see cref="Participant"/> to resolve permissions for.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ParticipantPermission}"/> of the <see cref="ParticipantRole"/> connected to this
        /// <see cref="AuthenticationPerson"/> in the supplied <see cref="EngineeringModelSetup"/>.
        /// </returns>
        private IEnumerable<ParticipantPermission> GetParticipantPermissions(NpgsqlTransaction transaction, Participant participant)
        {
            // retrieve PersonRole
            var paricipantRole = this.GetParticipantRole(transaction, participant);

            try
            {
                return this.ParticipantPermissionDao.Read(transaction, "SiteDirectory", paricipantRole.ParticipantPermission).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the participant permissions from the backtier");
                return null;
            }
        }

        /// <summary>
        /// Returns the list of OrganizationalParticipants relevant to the Person for all EngineeringModelSetups.
        /// </summary>
        /// <param name="personOrganization">The organization of the person.</param>
        /// <param name="allOrganizationalParticipants">The list of all Organizationa Participant Iids</param>
        /// <param name="transaction">The transaction</param>
        /// <returns>The list of all applicable OrganizationalParticipations</returns>
        private List<OrganizationalParticipant> GetPersonOrganizationalParticipants(Guid personOrganization, List<Guid> allOrganizationalParticipants, NpgsqlTransaction transaction)
        {
            try
            {
                return this.OrganizationalParticipantDao.Read(transaction, "SiteDirectory", allOrganizationalParticipants).Where(op => op.Organization.Equals(personOrganization)).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the person's organizational participation");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="PersonPermission"/>s connected to this person.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> to resolve permission for.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PersonPermission}"/> of the <see cref="PersonRole"/> connected to this <see cref="AuthenticationPerson"/>.
        /// </returns>
        private IEnumerable<PersonPermission> GetPersonPermissions(NpgsqlTransaction transaction, AuthenticationPerson person)
        {
            // retrieve PersonRole
            var personRole = this.GetPersonRole(transaction, person);

            try
            {
                return this.PersonPermissionDao.Read(transaction, "SiteDirectory", personRole.PersonPermission).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the person permissions from the backtier");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="EngineeringModelSetup"/>s connected to this person.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> to resolve permission for.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EngineeringModelSetup}"/> of the <see cref="Person"/>.
        /// </returns>
        private IEnumerable<EngineeringModelSetup> GetEngineeringModelSetups(NpgsqlTransaction transaction, AuthenticationPerson person)
        {
            try
            {
                var engineeringModelSetupDao = (EngineeringModelSetupDao)this.EngineeringModelSetupDao;
                return engineeringModelSetupDao.ReadByPerson(transaction, "SiteDirectory", person.Iid).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the person permissions from the backtier");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="PersonRole"/> connected to this person.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> to resolve permission for.
        /// </param>
        /// <returns>
        /// The <see cref="PersonRole"/> of the specified <see cref="AuthenticationPerson"/>.
        /// </returns>
        private PersonRole GetPersonRole(NpgsqlTransaction transaction, AuthenticationPerson person)
        {
            if (person.Role == null)
            {
                return null;
            }

            try
            {
                return this.PersonRoleDao.Read(transaction, "SiteDirectory", new List<Guid> { (Guid)person.Role }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, LoggerUtils.GetLogMessage(person, string.Empty, false, "There was an error while retrieving the person roles from the backtier"));
                return null;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="ParticipantRole"/> connected to a <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="participant">
        /// The participant.
        /// </param>
        /// <returns>
        /// The <see cref="ParticipantRole"/> of the specified <see cref="Participant"/>.
        /// </returns>
        private ParticipantRole GetParticipantRole(NpgsqlTransaction transaction, Participant participant)
        {
            try
            {
                return this.ParticipantRoleDao.Read(transaction, "SiteDirectory", new List<Guid> { participant.Role }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while retrieving the participant role from the backtier");
                return null;
            }
        }
    }
}
