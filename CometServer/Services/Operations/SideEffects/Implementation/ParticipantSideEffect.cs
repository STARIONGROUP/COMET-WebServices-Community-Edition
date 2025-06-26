// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantSideEffect.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParticipantSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public class ParticipantSideEffect : OperationSideEffect<Participant>
    {
        /// <summary>
        /// The selected domain key.
        /// </summary>
        private const string SelectedDomainKey = "SelectedDomain";

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupService"/>
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParticipantService"/>
        /// </summary>
        public IParticipantService ParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonService"/>
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The update info that was serialized directly from the user request. 
        /// The raw <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be edited likely: it can/will change the operation processor outcome.
        /// </param>
        public override async Task BeforeUpdate(Participant thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            await ValidateSelectedDomain(thing, rawUpdateInfo);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override async Task AfterCreate(Participant thing, Thing container, Participant originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await base.AfterUpdate(thing, container, originalThing, transaction, partition, securityContext);

            if (container is not EngineeringModelSetup originalEngineeringModelSetup)
            {
                throw new Cdp4ModelValidationException($"{nameof(EngineeringModelSetup)} was not found.");
            }

            if (this.EngineeringModelSetupService
                    .GetShallowAsync(transaction, partition, [originalEngineeringModelSetup.Iid], securityContext).FirstOrDefault() is not EngineeringModelSetup engineeringModelSetup)
            {
                throw new Cdp4ModelValidationException($"{nameof(EngineeringModelSetup)} was not found.");
            }

            var participants = this.ParticipantService.GetShallowAsync(transaction, partition, engineeringModelSetup.Participant, securityContext).OfType<Participant>().ToList();

            var newParticipantGuids = participants.Select(x => x.Iid).Except(originalEngineeringModelSetup.Participant).ToList();

            if (newParticipantGuids.Count != 0)
            {
                var duplicatePersons =
                    participants
                        .Select(x => new { Participant = x, x.Person })
                        .Where(x => x.Person != Guid.Empty)
                        .GroupBy(x => x.Person)
                        .Where(g => g.Count() > 1)
                        .ToList();

                if (duplicatePersons.Count != 0)
                {
                    foreach (var duplicatePerson in duplicatePersons)
                    {
                        var duplicateParticipantGuids = duplicatePerson.Select(x => x.Participant.Iid).ToList();

                        if (newParticipantGuids.Intersect(duplicateParticipantGuids).Any())
                        {
                            var person = this.PersonService
                                .GetShallowAsync(transaction, partition, [duplicatePerson.Key], securityContext).OfType<Person>().FirstOrDefault();

                            if (person == null)
                            {
                                throw new Cdp4ModelValidationException($"Duplicate {nameof(Person)} in {nameof(Participant)} list detected, but the {nameof(Person)} itself was not found.");
                            }

                            throw new Cdp4ModelValidationException($"{person.ShortName} is already a {nameof(Participant)} in {nameof(EngineeringModel)} {engineeringModelSetup.Name}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether a valid selected domain is supplied <see cref="Participant"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The update info that was serialized directly from the user request. 
        /// The raw <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be edited likely: it can/will change the operation processor outcome.
        /// </param>
        private static Task ValidateSelectedDomain(Participant thing, ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey(SelectedDomainKey))
            {
                var selectedDomainUpdate = Guid.Empty;

                if (rawUpdateInfo[SelectedDomainKey] != null)
                {
                    Guid.TryParse(rawUpdateInfo[SelectedDomainKey].ToString(), out selectedDomainUpdate);
                }

                if (!thing.Domain.Contains(selectedDomainUpdate))
                {
                    throw new InvalidOperationException("Participant selected domain must be contained in participant domain list.");
                }
            }

            return Task.CompletedTask;
        }
    }
}
