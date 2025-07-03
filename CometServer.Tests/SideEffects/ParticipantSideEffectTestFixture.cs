// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantSideEffectTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParticipantSideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class ParticipantSideEffectTestFixture
    {
        private const string SelectedDomainKey = "SelectedDomain";

        private Mock<ISecurityContext> securityContext;

        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;

        private Mock<IParticipantService> participantService;

        private Mock<IPersonService> personService;

        private NpgsqlTransaction npgsqlTransaction;

        private ClasslessDTO rawUpdateInfo;

        private ParticipantSideEffect participantSideEffect;

        private Participant participant;

        private EngineeringModelSetup originalEngineeringModelSetup;

        private EngineeringModelSetup engineeringModelSetup;
        private Person person;
        private Participant originalParticipant;

        [SetUp]
        public void SetUp()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.participantService = new Mock<IParticipantService>();
            this.personService = new Mock<IPersonService>();

            this.npgsqlTransaction = null;
            this.participantSideEffect = new ParticipantSideEffect
            {
                PersonService = this.personService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                ParticipantService = this.participantService.Object
            };

            this.person = new Person(Guid.NewGuid(), 0)
            {
                ShortName = "TestPerson"
            };

            this.participant = new Participant(Guid.NewGuid(), 0)
            {
                Person = this.person.Iid
            };

            this.participant.Domain.Add(Guid.NewGuid());

            this.originalParticipant = new Participant(Guid.NewGuid(), 0)
            {
                Person = this.person.Iid
            };

            this.originalParticipant.Domain.Add(Guid.NewGuid());

            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);

            this.originalEngineeringModelSetup = new EngineeringModelSetup(this.engineeringModelSetup.Iid, 0);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidOrNullSelectedDomain()
        {
            //null selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, null }
            };

            Assert.That(
                () =>
                    this.participantSideEffect.BeforeUpdateAsync(
                        this.participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo), Throws.Exception.TypeOf<InvalidOperationException>());

            //invalid selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, null }
            };

            Assert.That(
                () =>
                    this.participantSideEffect.BeforeUpdateAsync(
                        this.participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenContainerIsNull()
        {
            Assert.That(
                () =>
                this.participantSideEffect.AfterCreateAsync(this.participant, null, null, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Exception.TypeOf<Cdp4ModelValidationException>()
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenContainerIsNotAnEngineeringModelSetup()
        {
            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.participant, null, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Exception.TypeOf<Cdp4ModelValidationException>()
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenEngineeringModelSetupIsNotFound()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(Array.Empty<Thing>()));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Exception.TypeOf<Cdp4ModelValidationException>()
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenParticipantIsNotFound()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> {this.engineeringModelSetup}));

            this.participantService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(Array.Empty<Thing>()));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Nothing
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenNoNewParticipantIsAdded()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.engineeringModelSetup }));

            this.participantService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.participant }));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Nothing
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenNewParticipantIsAdded()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.engineeringModelSetup }));

            this.participantService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.participant }));

            this.personService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new [] {this.participant.Person}, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(Array.Empty<Thing>()));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Nothing
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsWhenSameNewParticipantIsAddedButPersonIsNotFound()
        {
            this.engineeringModelSetup.Participant.Add(this.participant.Iid);
            this.engineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.originalEngineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.engineeringModelSetup }));

            this.participantService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", this.engineeringModelSetup.Participant, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.participant, this.originalParticipant }));

            this.personService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.participant.Person }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(Array.Empty<Thing>()));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                    , Throws.Exception.TypeOf<Cdp4ModelValidationException>()
                    .With.Message.Contains($"{nameof(Person)} itself was not found.")
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsWhenSameNewParticipantIsAdded()
        {
            this.engineeringModelSetup.Participant.Add(this.participant.Iid);
            this.engineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.originalEngineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.engineeringModelSetupService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.engineeringModelSetup }));

            this.participantService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", this.engineeringModelSetup.Participant, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> { this.participant, this.originalParticipant }));

            this.personService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", new[] { this.participant.Person }, this.securityContext.Object)).Returns(Task.FromResult<IEnumerable<Thing>>(new List<Thing> {this.person}));

            Assert.That(
                () =>
                    this.participantSideEffect.AfterCreateAsync(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
                , Throws.Exception.TypeOf<Cdp4ModelValidationException>()
                    .With.Message.Contains($"is already a {nameof(Participant)}")
            );
        }
    }
}
