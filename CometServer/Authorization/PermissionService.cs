// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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

namespace CometServer.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    
    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

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
        /// Gets or sets the (injected) <see cref="IParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IChainOfRdlComputationService"/> used to compute the allowed <see cref="SiteReferenceDataLibrary"/> unique identifiers
        /// </summary>
        public IChainOfRdlComputationService ChainOfRdlComputationService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMetaInfoProvider"/>
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IOrganizationalParticipationResolverService"/>.
        /// </summary>
        public IOrganizationalParticipationResolverService OrganizationalParticipationResolverService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IAccessRightKindService"/> for this request
        /// </summary>
        public IAccessRightKindService AccessRightKindService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) resolve service.
        /// </summary>
        public IResolveService ResolveService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICredentialsService"/>
        /// </summary>
        public ICredentialsService CredentialsService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<PermissionService> Logger { get; set; }

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
            this.Logger.LogDebug("Type CanRead: {typeName}:{partition}", typeName, partition);

            if (partition == SiteDirectory)
            {
                // Get the person's permission and if found use it. If not, use the default.
                var personAccessRightKind = this.AccessRightKindService.QueryPersonAccessRightKind(this.CredentialsService.Credentials, typeName);

                switch (personAccessRightKind)
                {
                    case PersonAccessRightKind.SAME_AS_CONTAINER:
                        {
                            return securityContext.ContainerReadAllowed;
                        }

                    case PersonAccessRightKind.SAME_AS_SUPERCLASS:
                        {
                            return this.CanRead(this.MetaInfoProvider.BaseType(typeName), securityContext, partition);
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
            var participantAccessRightKind = this.AccessRightKindService.QueryParticipantAccessRightKind(this.CredentialsService.Credentials, typeName);

            switch (participantAccessRightKind)
            {
                case ParticipantAccessRightKind.SAME_AS_CONTAINER:
                    {
                        return securityContext.ContainerReadAllowed;
                    }

                case ParticipantAccessRightKind.SAME_AS_SUPERCLASS:
                    {
                        return this.CanRead(this.MetaInfoProvider.BaseType(typeName), securityContext, partition);
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
            this.Logger.LogDebug("Database CanRead: {ClassKind}:{IId}:{partition}", thing.ClassKind, thing.Iid, partition);

            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.CredentialsService.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind = this.AccessRightKindService.QueryPersonAccessRightKind(this.CredentialsService.Credentials, thing.GetType().Name);

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
                            return this.CredentialsService.Credentials.EngineeringModelSetups.Any(ems => ems.Iid == modelSetup.Iid);
                        }

                        if (thing is IterationSetup iterationSetup)
                        {
                            return this.CredentialsService.Credentials.EngineeringModelSetups.Any(ems => ems.IterationSetup.Contains(iterationSetup.Iid));
                        }

                        if (thing is Participant)
                        {
                            return this.CredentialsService.Credentials.EngineeringModelSetups.Any(ems => ems.Participant.Contains(thing.Iid));
                        }

                        if (thing is ModelReferenceDataLibrary)
                        {
                            return this.CredentialsService.Credentials.EngineeringModelSetups.Any(ems => ems.RequiredRdl.Contains(thing.Iid));
                        }

                        if (thing is SiteReferenceDataLibrary)
                        {
                            var rdlDependency = this.ChainOfRdlComputationService.QueryReferenceDataLibraryDependency(transaction, this.CredentialsService.Credentials.EngineeringModelSetups);
                            return rdlDependency.Contains(thing.Iid);
                        }

                        throw new NotImplementedException($"No implementation for type {thing.ClassKind} for CanRead");
                    }

                    case PersonAccessRightKind.MODIFY_OWN_PERSON:
                    {
                        if (thing is Person person)
                        {
                            return this.PersonIsParticipantWithinCurrentUserModel(transaction, person) || person.Iid == this.CredentialsService.Credentials.Person.Iid;
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
            this.Logger.LogDebug("Database CanWrite: {ClassKind}:{IId}:{partition}", thing.ClassKind, thing.Iid, partition);

            // Check for excluded Persons
            if (thing.ExcludedPerson.Contains(this.CredentialsService.Credentials.Person.Iid))
            {
                return false;
            }

            if (partition == SiteDirectory)
            {
                var personAccessRightKind =  this.AccessRightKindService.QueryPersonAccessRightKind(this.CredentialsService.Credentials, typeName);

                switch (personAccessRightKind)
                {
                    case PersonAccessRightKind.SAME_AS_CONTAINER:
                        {
                            return this.ComputeSameAsContainerWriteAllowed(transaction, thing, partition, modifyOperation, securityContext);
                        }

                    case PersonAccessRightKind.SAME_AS_SUPERCLASS:
                        {
                            return this.CanWrite(transaction, thing, this.MetaInfoProvider.BaseType(typeName), partition, modifyOperation, securityContext);
                        }

                    case PersonAccessRightKind.MODIFY_IF_PARTICIPANT:
                        {
                            return this.IsEngineeringModelSetupModifyAllowed(thing, modifyOperation) || 
                                   IsCreateAllowedForOperationOnThing(thing, modifyOperation);
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
            if (this.CredentialsService.Credentials.EngineeringModelSetup.OrganizationalParticipant.Count != 0 && (
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
                    if (this.CredentialsService.Credentials.OrganizationalParticipant == null)
                    {
                        return false;
                    }

                    // model is protected
                    if (!this.CredentialsService.Credentials.IsDefaultOrganizationalParticipant)
                    {
                        // is not default organizational participant
                        if (thing.ClassKind == ClassKind.ElementDefinition)
                        {
                            // directly get the participation
                            return ((ElementDefinition)thing).OrganizationalParticipant.Contains(this.CredentialsService.Credentials.OrganizationalParticipant.Iid);
                        }
                        else
                        {
                            // walk up the container chain until the ElementDefinition is found
                            var isOrganizationallyAllowed = this.OrganizationalParticipationResolverService.ResolveApplicableOrganizationalParticipations(transaction, partition, this.CredentialsService.Credentials.Iteration, thing, this.CredentialsService.Credentials.OrganizationalParticipant.Iid);

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
                    if (this.CredentialsService.Credentials.OrganizationalParticipant == null)
                    {
                        return false;
                    }
                }
            }

            // Get the person's participant permission and if found use it. If not, use the default.
            var participantAccessRightKind = this.AccessRightKindService.QueryParticipantAccessRightKind(this.CredentialsService.Credentials, typeName);

            switch (participantAccessRightKind)
            {
                case ParticipantAccessRightKind.SAME_AS_CONTAINER:
                    {
                        return this.ComputeSameAsContainerWriteAllowed(transaction, thing, partition, modifyOperation, securityContext);
                    }

                case ParticipantAccessRightKind.SAME_AS_SUPERCLASS:
                    {
                        return this.CanWrite(transaction, thing, this.MetaInfoProvider.BaseType(typeName), partition, modifyOperation, securityContext);
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
        /// Check if (overridden) Create operation is allowed for an EngineeringModelSetup, when the user has MODIFY_IF_PARTICIPANT access rights
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check</param>
        /// <param name="modifyOperation">The kind of operation we are performing</param>
        /// <returns>True if create is allowed, otherwise false</returns>
        private static bool IsCreateAllowedForOperationOnThing(Thing thing, string modifyOperation)
        {
            return modifyOperation == CreateOperation && thing is EngineeringModelSetup;
        }

        /// <summary>
        /// Compute WRITE permission for <see cref="ParticipantAccessRightKind.SAME_AS_CONTAINER"/>, or <see cref="PersonAccessRightKind.SAME_AS_CONTAINER"/>
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to compute permissions for.
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
        /// <returns></returns>
        private bool ComputeSameAsContainerWriteAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation, ISecurityContext securityContext)
        {
            if (modifyOperation != ServiceBase.UpdateOperation)
            {
                return securityContext.ContainerWriteAllowed;
            }

            var dtoInfo = thing.GetInfoPlaceholder();

            var operationThingContainerCache =
                new Dictionary<DtoInfo, DtoResolveHelper>
                {
                    {
                        dtoInfo,
                        new DtoResolveHelper(dtoInfo)
                    }
                };

            var resolvePartition = partition.StartsWith(CDP4Orm.Dao.Utils.IterationSubPartition)
                ? partition.Replace(CDP4Orm.Dao.Utils.IterationSubPartition, CDP4Orm.Dao.Utils.EngineeringModelPartition)
                : partition;

            this.ResolveService.ResolveItems(transaction, resolvePartition, operationThingContainerCache);

            var containerThing = operationThingContainerCache.SingleOrDefault(x => x.Key is ContainerInfo).Value;

            return containerThing != null
                   &&
                   this.CanWrite(
                       transaction,
                       containerThing.Thing,
                       containerThing.Thing.ClassKind.ToString(),
                       containerThing.Partition,
                       modifyOperation,
                       securityContext);
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
                return this.CredentialsService.Credentials.EngineeringModelSetups.Any(ems => ems.Iid == modelSetup.Iid);
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
                return person.Iid == this.CredentialsService.Credentials.Person.Iid;
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
                    .Read(transaction, SiteDirectory, null, true).ToList()
                    .Where(participant => participant.Person == this.CredentialsService.Credentials.Person.Iid && this.CredentialsService.Credentials.EngineeringModelSetup.Participant.Contains(participant.Iid));

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
            var participantsIds = this.CredentialsService.Credentials.EngineeringModelSetups.SelectMany(x => x.Participant).ToArray();
            if (participantsIds.Length == 0)
            {
                return false;
            }

            return this.ParticipantDao
                            .Read(transaction, SiteDirectory, participantsIds, true)
                            .Any(p => p.Person == person.Iid);
        }
    }
}
