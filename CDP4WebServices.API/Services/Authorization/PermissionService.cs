// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;
    using System.Linq;
    using Authentication;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using NLog;
    using Npgsql;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Handles permissions per read/write
    /// </summary>
    public class PermissionService : IPermissionService
    {
        /// <summary>
        /// The SiteDirectory top container.
        /// </summary>
        private const string SiteDirectory = "SiteDirectory";

        /// <summary>
        /// The create type of modify operation.
        /// </summary>
        private const string CreateOperation = "create";

        /// <summary>
        /// Gets or sets a <see cref="IParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModelReferenceDataLibraryDao"/>
        /// </summary>
        public IModelReferenceDataLibraryDao ModelReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Gets or sets the request utils for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the <see cref="Credentials"/> assigned to this service.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Determines whether the typeName can be read.
        /// </summary>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <param name="securityContext">
        /// The security context of the current request.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// True if the given typeName can be read.
        /// </returns>
        public bool CanRead(string typeName, ISecurityContext securityContext, string partition)
        {
            Logger.Debug("{0}:{1}", typeName, partition);

            if (partition == SiteDirectory)
            {
                // Get the person's permission and if found use it. If not, use the default.
                var personAccessRightKind = this.GetPersonAccessRightKind(typeName);

                switch (personAccessRightKind)
                {
                    case PersonAccessRightKind.SAME_AS_CONTAINER:
                        {
                            return securityContext.ContainerReadAllowed;
                        }

                    case PersonAccessRightKind.SAME_AS_SUPERCLASS:
                        {
                            return this.CanRead(this.RequestUtils.MetaInfoProvider.BaseType(typeName), securityContext, partition);
                        }

                    case PersonAccessRightKind.READ_IF_PARTICIPANT:
                    case PersonAccessRightKind.MODIFY_IF_PARTICIPANT:
                    case PersonAccessRightKind.READ:
                    case PersonAccessRightKind.MODIFY:
                        {
                            return true;
                        }

                    case PersonAccessRightKind.MODIFY_OWN_PERSON:
                        {
                            return true;
                        }

                    default:
                        {
                            return false;
                        }
                }
            }

            // Get the person's permission and if found use it. If not, use the default.
            var participantAccessRightKind = this.GetParticipantAccessRightKind(typeName);

            switch (participantAccessRightKind)
            {
                case ParticipantAccessRightKind.SAME_AS_CONTAINER:
                    {
                        return securityContext.ContainerReadAllowed;
                    }

                case ParticipantAccessRightKind.SAME_AS_SUPERCLASS:
                    {
                        return this.CanRead(this.RequestUtils.MetaInfoProvider.BaseType(typeName), securityContext, partition);
                    }

                case ParticipantAccessRightKind.MODIFY:
                case ParticipantAccessRightKind.MODIFY_IF_OWNER:
                case ParticipantAccessRightKind.READ:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Thing"/> can be read. This method is exclusively to be used in the after hook of the services to determine
        /// permission based on ownership. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>True if the given <see cref="Thing"/> can be read.</returns>
        public bool CanRead(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind = this.GetPersonAccessRightKind(thing.GetType().Name);

                switch (personAccessRightKind)
                {
                    case PersonAccessRightKind.READ_IF_PARTICIPANT:
                    {
                        if (thing is Person person)
                        {
                            return this.PersonIsParticipantWithinCurrentUserModel(transaction, person);
                        }

                        if (!(thing is IParticipantAffectedAccessThing))
                        {
                            return false;
                        }

                        if (thing is EngineeringModelSetup modelSetup)
                        {
                            return this.Credentials.EngineeringModelSetups.Any(ems => ems.Iid == modelSetup.Iid);

                        }

                        if (thing is IterationSetup iterationSetup)
                        {
                            return this.Credentials.EngineeringModelSetups.Any(ems => ems.IterationSetup.Contains(iterationSetup.Iid));
                        }

                        if (thing is Participant)
                        {
                            return this.Credentials.EngineeringModelSetups.Any(ems => ems.Participant.Contains(thing.Iid));
                        }

                        if (thing is ModelReferenceDataLibrary)
                        {
                            return this.Credentials.EngineeringModelSetups.Any(ems => ems.RequiredRdl.Contains(thing.Iid));
                        }

                        if (thing is SiteReferenceDataLibrary)
                        {
                            var rdlDependency = this.ModelReferenceDataLibraryDao.GetSiteReferenceDataLibraryDependency(this.Credentials.EngineeringModelSetups, transaction);
                            return rdlDependency.Contains(thing.Iid);
                        }

                        throw new NotImplementedException($"No implementation for type {thing.ClassKind} for CanRead");
                    }

                    case PersonAccessRightKind.MODIFY_OWN_PERSON:
                    {
                        if (thing is Person person)
                        {
                            return this.PersonIsParticipantWithinCurrentUserModel(transaction, person) || person.Iid == this.Credentials.Person.Iid;
                        }

                        // That should only be applied on Person
                        return false;
                    }

                    default: return true;
                }
            }

            // EngineeringModel context
            // Check for excluded Domains
            var isExcludedDomain = this.IsExcludedDomain(transaction, thing);

            if (isExcludedDomain)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the <see cref="Thing"/> can be written. This method is exclusively to be used in the after hook of the services to determine
        /// permission based on ownership. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <param name="securityContext">
        /// The security context of the current request.
        /// </param>
        /// <returns>True if the given <see cref="Thing"/> can be written.</returns>
        public bool CanWrite(NpgsqlTransaction transaction, Thing thing, string typeName, string partition, string modifyOperation, ISecurityContext securityContext)
        {
            Logger.Debug("{0}:{1}", typeName, partition);

            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind = this.GetPersonAccessRightKind(typeName);

                switch (personAccessRightKind)
                {
                    case PersonAccessRightKind.SAME_AS_CONTAINER:
                        {
                            return securityContext.ContainerWriteAllowed;
                        }

                    case PersonAccessRightKind.SAME_AS_SUPERCLASS:
                        {
                            return this.CanWrite(transaction, thing, this.RequestUtils.MetaInfoProvider.BaseType(typeName), partition, modifyOperation, securityContext);
                        }

                    case PersonAccessRightKind.MODIFY_IF_PARTICIPANT:
                        {
                            return this.IsEngineeringModelSetupModifyAllowed(transaction, thing, partition, modifyOperation);
                        }

                    case PersonAccessRightKind.MODIFY_OWN_PERSON:
                        {
                            return this.IsOwnPerson(thing);
                        }

                    case PersonAccessRightKind.MODIFY:
                        {
                            return true;
                        }

                    default:
                        {
                            return false;
                        }
                }
            }

            // EngineeringModel context
            // Check for excluded Domains
            var isExcludedDomain = this.IsExcludedDomain(transaction, thing);

            if (isExcludedDomain)
            {
                return false;
            }

            // Get the person's permission and if found use it. If not, use the default.
            var participantAccessRightKind = this.GetParticipantAccessRightKind(typeName);

            switch (participantAccessRightKind)
            {
                case ParticipantAccessRightKind.SAME_AS_CONTAINER:
                    {
                        return securityContext.ContainerWriteAllowed;
                    }

                case ParticipantAccessRightKind.SAME_AS_SUPERCLASS:
                    {
                        return this.CanWrite(transaction, thing, this.RequestUtils.MetaInfoProvider.BaseType(typeName), partition, modifyOperation, securityContext);
                    }

                case ParticipantAccessRightKind.MODIFY_IF_OWNER:
                    {
                        return this.IsOwner(transaction, thing);
                    }

                case ParticipantAccessRightKind.MODIFY:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Determines whether it is allowed for the current <see cref="Participant"/> to modify <see cref="EngineeringModelSetup"/>
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <returns>True if the modification  of the <see cref="EngineeringModelSetup"/> by the current <see cref="Participant"/> is allowed.</returns>
        public bool IsEngineeringModelSetupModifyAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation)
        {
            if (modifyOperation == CreateOperation)
            {
                return false;
            }

            if (thing is EngineeringModelSetup modelSetup)
            {
                return this.Credentials.EngineeringModelSetups.Any(ems => ems.Iid == modelSetup.Iid);
            }

            return false;
        }

        /// <summary>
        /// Determines whether it is allowed for the current <see cref="Person"/> to modify <see cref="Person"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check whether it is own <see cref="Person"/>.</param>
        /// <returns>True if it is own <see cref="Person"/>.</returns>
        private bool IsOwnPerson(Thing thing)
        {
            var person = thing as Person;

            if (person != null)
            {
                return person.Iid == this.Credentials.Person.Iid;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to check whether it is own <see cref="Person"/>.</param>
        /// <returns>True if a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.</returns>
        private bool IsOwner(NpgsqlTransaction transaction, Thing thing)
        {
            // Check if owned thing
            var ownedThing = thing as IOwnedThing;

            if (ownedThing == null)
            {
                return true;
            }

            var currentParticipant = this.ParticipantDao
                .Read(transaction, SiteDirectory, null).ToList()
                .Where(participant => participant.Person == this.Credentials.Person.Iid && this.Credentials.EngineeringModelSetup.Participant.Contains(participant.Iid));

            return currentParticipant.SelectMany(x => x.Domain).Contains(ownedThing.Owner);
        }

        /// <summary>
        /// Determines whether a supplied <see cref="Thing"/> excludes a domain of the current <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to check whether it is own <see cref="Person"/>.</param>
        /// <returns>True if a supplied <see cref="Thing"/> excludes a domain of the current <see cref="Participant"/>.</returns>
        private bool IsExcludedDomain(NpgsqlTransaction transaction, Thing thing)
        {
            var currentParticipant = this.ParticipantDao
                .Read(transaction, SiteDirectory, null).ToList()
                .Where(participant => participant.Person == this.Credentials.Person.Iid && this.Credentials.EngineeringModelSetup.Participant.Contains(participant.Iid));

            var isExcludedDomain = true;

            foreach (var domainId in currentParticipant.SelectMany(x => x.Domain))
            {
                if (!thing.ExcludedDomain.Contains(domainId))
                {
                    isExcludedDomain = false;
                    break;
                }
            }

            return isExcludedDomain;
        }

        /// <summary>
        /// Get <see cref="PersonAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="PersonAccessRightKind"/> for the supplied object type.</returns>
        private PersonAccessRightKind GetPersonAccessRightKind(string typeName)
        {
            // Get the person's permission and if found use it. If not, use the default.
            var personPermission = this.Credentials.PersonPermissions.FirstOrDefault(pp => pp.ObjectClass.ToString().Equals(typeName));
            var personAccessRightKind = personPermission == null
                                            ? this.RequestUtils.DefaultPermissionProvider.GetDefaultPersonPermission(typeName)
                                            : personPermission.AccessRight;

            return personAccessRightKind;
        }

        /// <summary>
        /// Get <see cref="ParticipantAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="ParticipantAccessRightKind"/> for the supplied object type.</returns>
        private ParticipantAccessRightKind GetParticipantAccessRightKind(string typeName)
        {
            // Get the particpant's permission and if found use it. If not, use the default.
            var participantPermission = this.Credentials.ParticipantPermissions.FirstOrDefault(
                    pp => pp.ObjectClass.ToString().Equals(typeName));
            var participantAccessRightKind = participantPermission == null
                                                 ? this.RequestUtils.DefaultPermissionProvider.GetDefaultParticipantPermission(typeName)
                                                 : participantPermission.AccessRight;

            return participantAccessRightKind;
        }

        /// <summary>
        /// Check whether the <paramref name="person"/> is a participant in any model where the current user is participating
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="person">The <see cref="Person"/></param>
        /// <returns>True if that is the case</returns>
        private bool PersonIsParticipantWithinCurrentUserModel(NpgsqlTransaction transaction, Person person)
        {
            var participantsIds = this.Credentials.EngineeringModelSetups.SelectMany(x => x.Participant).ToArray();
            if (participantsIds.Length == 0)
            {
                return false;
            }

            return this.ParticipantDao
                            .Read(transaction, SiteDirectory, participantsIds)
                            .Any(p => p.Person == person.Iid);
        }
    }
}