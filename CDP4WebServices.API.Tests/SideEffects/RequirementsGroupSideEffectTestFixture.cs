// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementsGroupSideEffectTestFixture.cs" company="RHEA System S.A.">
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

            this.rawUpdateInfo = new ClasslessDTO(null)
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
            this.rawUpdateInfo = new ClasslessDTO(null)
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

            this.rawUpdateInfo = new ClasslessDTO(null)
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
            this.rawUpdateInfo = new ClasslessDTO(null)
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