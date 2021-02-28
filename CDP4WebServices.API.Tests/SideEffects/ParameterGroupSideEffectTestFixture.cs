// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterGroupSideEffect" />
    /// </summary>
    [TestFixture]
    public class ParameterGroupSideEffectTestFixture
    {
        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain a -> b -> c
            this.parameterGroupD = new ParameterGroup { Iid = Guid.NewGuid() };
            this.parameterGroupC = new ParameterGroup { Iid = Guid.NewGuid() };

            this.parameterGroupB =
                new ParameterGroup { Iid = Guid.NewGuid(), ContainingGroup = this.parameterGroupC.Iid };

            this.parameterGroupA =
                new ParameterGroup { Iid = Guid.NewGuid(), ContainingGroup = this.parameterGroupB.Iid };

            this.elementDefinition = new ElementDefinition
            {
                Iid = Guid.NewGuid(),
                ParameterGroup =
                {
                    this.parameterGroupA.Iid,
                    this.parameterGroupB.Iid,
                    this.parameterGroupC.Iid
                }
            };

            this.parameterGroupService = new Mock<IParameterGroupService>();

            this.parameterGroupService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterGroupA.Iid, this.parameterGroupB.Iid, this.parameterGroupC.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ParameterGroup> { this.parameterGroupA, this.parameterGroupB, this.parameterGroupC });

            this.organizationalParticipationResolverService = new Mock<IOrganizationalParticipationResolverService>();
            this.organizationalParticipationResolverService.Setup(x => x.ValidateCreateOrganizationalParticipation(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<ISecurityContext>(), this.npgsqlTransaction, It.IsAny<string>()));
        }

        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IParameterGroupService> parameterGroupService;

        private Mock<IOrganizationalParticipationResolverService> organizationalParticipationResolverService;

        private ParameterGroupSideEffect sideEffect;

        private ParameterGroup parameterGroupA;

        private ParameterGroup parameterGroupB;

        private ParameterGroup parameterGroupC;

        private ParameterGroup parameterGroupD;

        private ElementDefinition elementDefinition;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ContainingGroup";

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsOutOfChainOrLeadsToCircularDependencyOnCreate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect { ParameterGroupService = this.parameterGroupService.Object, OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object };

            // Out of the store
            var id = this.parameterGroupD.Iid;
            this.parameterGroupA.ContainingGroup = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.parameterGroupA,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));

            this.parameterGroupA.ContainingGroup = this.parameterGroupB.Iid;

            // Leads to circular dependency
            id = this.parameterGroupA.Iid;
            this.parameterGroupC.ContainingGroup = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.parameterGroupC,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsOutOfChainOrLeadsToCircularDependencyOnUpdate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect { ParameterGroupService = this.parameterGroupService.Object, OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object };

            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.parameterGroupD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterGroupA,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.parameterGroupA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterGroupC,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsParameterGroupItselfOnCreate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect { ParameterGroupService = this.parameterGroupService.Object, OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object };

            var id = this.parameterGroupA.Iid;
            this.parameterGroupA.ContainingGroup = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.parameterGroupA,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsParameterGroupItselfOnUpdate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect { ParameterGroupService = this.parameterGroupService.Object, OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object };

            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.parameterGroupA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterGroupA,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}
