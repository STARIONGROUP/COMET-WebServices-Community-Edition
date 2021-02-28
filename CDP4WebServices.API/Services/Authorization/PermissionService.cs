// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Authentication;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    
    using CDP4Orm.Dao;
    
    using NLog;
    
    using Npgsql;

    using IServiceProvider = CometServer.Services.IServiceProvider;
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
        /// Gets or sets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOrganizationalParticipationResolverService"/>.
        /// </summary>
        public IOrganizationalParticipationResolverService OrganizationalParticipationResolverService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IAccessRightKindService"/> for this request
        /// </summary>
        public IAccessRightKindService AccessRightKindService { get; set; }

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the <see cref="Credentials"/> assigned to this service.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Gets the list of <see cref="Participant"/> of the current <see cref="Person"/> that is represented by the current <see cref="Credentials"/> 
        /// </summary>
        private List<Participant> currentParticipantCache;

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
            Logger.Debug("Type CanRead: {0}:{1}", typeName, partition);

            if (partition == SiteDirectory)
            {
                // Get the person's permission and if found use it. If not, use the default.
                var personAccessRightKind = this.AccessRightKindService.QueryPersonAccessRightKind(this.Credentials, typeName);

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
            var participantAccessRightKind = this.AccessRightKindService.QueryParticipantAccessRightKind(this.Credentials, typeName);

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
            Logger.Debug("Database CanRead: {0}:{1}:{2}", thing.ClassKind, thing.Iid, partition);

            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind = this.AccessRightKindService.QueryPersonAccessRightKind(this.Credentials, thing.GetType().Name);

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
            Logger.Debug("Database CanWrite: {0}:{1}:{2}", thing.ClassKind, thing.Iid, partition);

            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind =  this.AccessRightKindService.QueryPersonAccessRightKind(this.Credentials, typeName);

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
                            return this.IsEngineeringModelSetupModifyAllowed(thing, modifyOperation);
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

            // obfuscation check. Regardless of other things, if a Thing is obfuscated for person, disallow write
            if (this.Credentials.EngineeringModelSetup.OrganizationalParticipant.Any() && (
                    thing.ClassKind == ClassKind.ElementDefinition ||
                    thing.ClassKind == ClassKind.ElementUsage ||
                    thing.ClassKind == ClassKind.Parameter ||
                    thing.ClassKind == ClassKind.ParameterValueSet ||
                    thing.ClassKind == ClassKind.ParameterSubscription ||
                    thing.ClassKind == ClassKind.ParameterSubscriptionValueSet ||
                    thing.ClassKind == ClassKind.ParameterOverride ||
                    thing.ClassKind == ClassKind.ParameterOverrideValueSet ||
                    thing.ClassKind == ClassKind.ParameterGroup ||
                    thing.ClassKind == ClassKind.Definition ||
                    thing.ClassKind == ClassKind.Citation ||
                    thing.ClassKind == ClassKind.Alias ||
                    thing.ClassKind == ClassKind.HyperLink))
            {
                if (modifyOperation != CreateOperation)
                {
                    // you have no access to any element definitions or contained thing
                    if (this.Credentials.OrganizationalParticipant == null)
                    {
                        return false;
                    }

                    // model is protected
                    if (!this.Credentials.IsDefaultOrganizationalParticipant)
                    {
                        // is not default organizational participant
                        if (thing.ClassKind == ClassKind.ElementDefinition)
                        {
                            // directly get the participation
                            return ((ElementDefinition)thing).OrganizationalParticipant.Contains(this.Credentials.OrganizationalParticipant.Iid);
                        }
                        else
                        {
                            // walk up the container chain until the ElementDefinition is found
                            var isOrganizationallyAllowed = this.OrganizationalParticipationResolverService.ResolveApplicableOrganizationalParticipations(transaction, partition, this.Credentials.Iteration, thing, this.Credentials.OrganizationalParticipant.Iid);

                            if (!isOrganizationallyAllowed)
                            {
                                return false;
                            }

                            // if check passes carry on with other checks
                        }
                    }
                }
                else
                {
                    // on create the 
                    // you have no access to any element definitions or contained thing, cant create anything
                    if (this.Credentials.OrganizationalParticipant == null)
                    {
                        return false;
                    }
                }
            }

            // Get the person's participant permission and if found use it. If not, use the default.
            var participantAccessRightKind = this.AccessRightKindService.QueryParticipantAccessRightKind(this.Credentials, typeName);

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
        /// <param name="thing">The <see cref="Thing"/> to compute permissions for.</param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <returns>True if the modification  of the <see cref="EngineeringModelSetup"/> by the current <see cref="Participant"/> is allowed.</returns>
        private bool IsEngineeringModelSetupModifyAllowed(Thing thing, string modifyOperation)
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
            if (thing is Person person)
            {
                return person.Iid == this.Credentials.Person.Iid;
            }

            return false;
        }

        /// <summary>
        /// Queries the <see cref="Participant"/>s of the current <see cref="Credentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The <see cref="NpgsqlTransaction"/> used to query the database
        /// </param>
        /// <returns>
        /// a list of <see cref="Participant"/>s for the current <see cref="Credentials"/>
        /// </returns>
        /// <remarks>
        /// The <see cref="Participant"/> objects are cached in the <see cref="currentParticipantCache"/> field of the current
        /// <see cref="PermissionService"/>. This can be done due to the fact that the <see cref="PermissionService"/> is valid for one request
        /// </remarks>
        private List<Participant> QueryCurrentParticipant(NpgsqlTransaction transaction)
        {
            if (this.currentParticipantCache == null)
            {
                this.currentParticipantCache = new List<Participant>();

                var participants = this.ParticipantDao
                    .Read(transaction, SiteDirectory, null).ToList()
                    .Where(participant => participant.Person == this.Credentials.Person.Iid && this.Credentials.EngineeringModelSetup.Participant.Contains(participant.Iid));

                this.currentParticipantCache.AddRange(participants);
            }

            return this.currentParticipantCache;
        }

        /// <summary>
        /// Determines whether a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">The <see cref="Thing"/> to check whether it is own <see cref="Person"/>.</param>
        /// <returns>True if a supplied <see cref="Thing"/> is owned by the current <see cref="Participant"/>.</returns>
        public bool IsOwner(NpgsqlTransaction transaction, Thing thing)
        {
            // Check if owned thing
            var ownedThing = thing as IOwnedThing;

            if (ownedThing == null)
            {
                return true;
            }

            var currentParticipant = this.QueryCurrentParticipant(transaction);

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
            var currentParticipant = this.QueryCurrentParticipant(transaction);

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
