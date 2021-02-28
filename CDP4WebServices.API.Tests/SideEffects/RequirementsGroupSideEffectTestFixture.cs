// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementsGroupSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="RequirementsGroupSideEffect"/>
    /// </summary>
    [TestFixture]
    public class RequirementsGroupSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IRequirementsSpecificationService> requirementsSpecificationService;

        private RequirementsGroupSideEffect sideEffect;

        private RequirementsGroup requirementsGroupA;

        private RequirementsGroup requirementsGroupB;

        private RequirementsGroup requirementsGroupC;

        private RequirementsGroup requirementsGroupD;

        private RequirementsGroup requirementsGroupE;

        private RequirementsGroup requirementsGroupF;

        private RequirementsSpecification requirementsSpecification;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "Group";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain d -> e -> f
            this.requirementsGroupA = new RequirementsGroup { Iid = Guid.NewGuid() };
            this.requirementsGroupB = new RequirementsGroup { Iid = Guid.NewGuid() };
            this.requirementsGroupC = new RequirementsGroup { Iid = Guid.NewGuid() };

            this.requirementsGroupF = new RequirementsGroup { Iid = Guid.NewGuid() };
            this.requirementsGroupE =
                new RequirementsGroup { Iid = Guid.NewGuid(), Group = { this.requirementsGroupF.Iid } };
            this.requirementsGroupD =
                new RequirementsGroup { Iid = Guid.NewGuid(), Group = { this.requirementsGroupE.Iid } };

            this.requirementsSpecification = new RequirementsSpecification
                                                 {
                                                     Iid = Guid.NewGuid(),
                                                     Group =
                                                         {
                                                             this.requirementsGroupA.Iid,
                                                             this.requirementsGroupB.Iid,
                                                             this.requirementsGroupC.Iid,
                                                             this.requirementsGroupD.Iid,
                                                             this.requirementsGroupE.Iid,
                                                             this.requirementsGroupF.Iid
                                                         }
                                                 };

            this.requirementsSpecificationService = new Mock<IRequirementsSpecificationService>();
            this.requirementsSpecificationService
                .Setup(
                    x => x.GetDeep(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<RequirementsGroup>
                        {
                            this.requirementsGroupA,
                            this.requirementsGroupB,
                            this.requirementsGroupC,
                            this.requirementsGroupD,
                            this.requirementsGroupE,
                            this.requirementsGroupF
                        });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainedGroupIsGroupItself()
        {
            this.sideEffect =
                new RequirementsGroupSideEffect() { RequirementsSpecificationService = this.requirementsSpecificationService.Object };

            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<Guid>
                                                 {
                                                     this.requirementsGroupA.Iid,
                                                     this.requirementsGroupC.Iid
                                                 }
                                         }
                                     };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.requirementsGroupA,
                    this.requirementsSpecification,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenRequirementsGroupLeadsToCircularDependency()
        {
            this.sideEffect =
                new RequirementsGroupSideEffect() { RequirementsSpecificationService = this.requirementsSpecificationService.Object };
            
            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<Guid>
                                                 {
                                                     this.requirementsGroupB.Iid,
                                                     this.requirementsGroupD.Iid,
                                                     this.requirementsGroupF.Iid
                                                 }
                                         }
                                     };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.requirementsGroupA,
                    this.requirementsSpecification,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<Guid>
                                                 {
                                                     this.requirementsGroupB.Iid,
                                                     this.requirementsGroupD.Iid
                                                 }
                                         }
                                     };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.requirementsGroupF,
                    this.requirementsSpecification,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenContainedGroupsTreeIsNormal()
        {
            this.sideEffect =
                new RequirementsGroupSideEffect() { RequirementsSpecificationService = this.requirementsSpecificationService.Object };

            // There is a chain d -> e -> f
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<Guid>
                                                 {
                                                     this.requirementsGroupB.Iid,
                                                     this.requirementsGroupD.Iid,
                                                     this.requirementsGroupC.Iid
                                                 }
                                         }
                                     };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.requirementsGroupA,
                    this.requirementsSpecification,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}