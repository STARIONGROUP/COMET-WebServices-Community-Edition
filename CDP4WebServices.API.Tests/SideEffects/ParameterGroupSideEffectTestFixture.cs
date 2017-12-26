// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterGroupSideEffect"/>
    /// </summary>
    [TestFixture]
    public class ParameterGroupSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IParameterGroupService> parameterGroupService;

        private ParameterGroupSideEffect sideEffect;

        private ParameterGroup parameterGroupA;

        private ParameterGroup parameterGroupB;

        private ParameterGroup parameterGroupC;

        private ParameterGroup parameterGroupD;

        private ElementDefinition elementDefinition;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ContainingGroup";

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
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsParameterGroupItselfOnUpdate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect() { ParameterGroupService = this.parameterGroupService.Object };

            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.parameterGroupA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterGroupA,
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
                new ParameterGroupSideEffect() { ParameterGroupService = this.parameterGroupService.Object };

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
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsOutOfChainOrLeadsToCircularDependencyOnUpdate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect() { ParameterGroupService = this.parameterGroupService.Object };

            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.parameterGroupD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterGroupA,
                    this.elementDefinition,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.parameterGroupA.Iid } };

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
        public void VerifyThatExceptionIsThrownWhenContainingGroupIsOutOfChainOrLeadsToCircularDependencyOnCreate()
        {
            this.sideEffect =
                new ParameterGroupSideEffect() { ParameterGroupService = this.parameterGroupService.Object };

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
    }
}