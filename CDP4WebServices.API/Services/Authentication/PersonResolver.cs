// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonResolver.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Authentication;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Authentication;

    using CDP4WebService.Authentication;

    using Nancy.Security;
    using NLog;
    using Npgsql;

    /// <summary>
    /// Resolves the Person from the database.
    /// </summary>
    public class PersonResolver : IPersonResolver
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the engineeringModelSetup dao.
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the PersonPermissionDao.
        /// </summary>
        public IPersonPermissionDao PersonPermissionDao { get; set; }

        /// <summary>
        /// Gets or sets the PersonRoleDao
        /// </summary>
        public IPersonRoleDao PersonRoleDao { get; set; }

        /// <summary>
        /// Gets or sets a ParticipantDao
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets a ParticipantRoleDao
        /// </summary>
        public IParticipantRoleDao ParticipantRoleDao { get; set; }

        /// <summary>
        /// Gets or sets a DomainOfExpertiseDao
        /// </summary>
        public IDomainOfExpertiseDao DomainOfExpertiseDao { get; set; }

        /// <summary>
        /// Gets or sets the ParticipantPermissionDao
        /// </summary>
        public IParticipantPermissionDao ParticipantPermissionDao { get; set; }

        /// <summary>
        /// Gets or sets the authentication dao.
        /// </summary>
        public IAuthenticationDao AuthenticationDao { get; set; }

        /// <summary>
        /// Resolves the username to <see cref="IUserIdentity"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="username">
        /// The supplied username
        /// </param>
        /// <returns>
        /// A <see cref="IUserIdentity"/> representing the resolved user, null if the user was not found.
        /// </returns>
        public IUserIdentity ResolvePerson(NpgsqlTransaction transaction, string username)
        {
            var person = this.AuthenticationDao.Read(transaction, "SiteDirectory", username).SingleOrDefault();

            if (person != null)
            {
                return this.ResolveCredentials(transaction, person);
            }

            Logger.Warn("User request with username: {0} could not be authenticated", username);
            return null;
        }

        /// <summary>
        /// Resolve and set participant information for the passed in <see cref="Credentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="credentials">
        /// The credentials Interface.
        /// </param>
        public void ResolveParticipantCredentials(NpgsqlTransaction transaction, ICredentials credentials)
        {
            var creds = credentials as Credentials;

            if (creds == null || creds.Person == null || creds.EngineeringModelSetup == null)
            {
                return;
            }

            var participant = this.GetParticipant(transaction, creds.Person, creds.EngineeringModelSetup);
            if (participant == null)
            {
                return;
            }

            // Set participant information for the resolved participant
            creds.ParticipantPermissions = this.GetParticipantPermissions(transaction, participant);
            creds.DomainOfExpertise = this.GetSelectedDomain(transaction, participant);
            creds.IsParticipant = true;
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
        private IUserIdentity ResolveCredentials(NpgsqlTransaction transaction, AuthenticationPerson person)
        {
            var personPermissions = new List<PersonPermission>();
            var engineeringModelSetups = this.GetEngineeringModelSetups(transaction, person).ToList();

            if (person.Role != null)
            {
                personPermissions = this.GetPersonPermissions(transaction, person).ToList();
            }

            Logger.Trace("Info {0} was authenticated", person.UserName);

            return new Credentials
            {
                UserName = person.UserName,
                Person = person,
                PersonPermissions = personPermissions,
                ParticipantPermissions = new List<ParticipantPermission>(),
                EngineeringModelSetups = engineeringModelSetups
            };
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
