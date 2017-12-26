// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ElementDefinitionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class ElementDefinitionSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IElementDefinitionService> elementDefinitionService;

        private Mock<IElementUsageService> elementUsageService;

        private ElementDefinitionSideEffect sideEffect;

        private ElementDefinition edA;

        private ElementDefinition edB;

        private ElementDefinition edC;

        private ElementDefinition edD;

        private ElementUsage euA;

        private ElementUsage euB;

        private ElementUsage euC;

        private ElementUsage euD;

        private ElementUsage euE;

        private Iteration iteration;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ContainedElement";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            ////                    edA
            ////                   /    \
            ////                 euA    euB
            ////                 /         \
            ////              edB           edC
            ////                           /   \
            ////                         euC    euD   euE will be added. euE-edD does not throw an exception, but euD-edA does throw
            ////                        /         \     \
            ////                      edD          edA   edD
            this.edD = new ElementDefinition { Iid = Guid.NewGuid() };
            this.euE = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edD.Iid };

            this.euC = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edD.Iid };

            this.edC = new ElementDefinition
                           {
                               Iid = Guid.NewGuid(),
                               ContainedElement = new List<Guid> { this.euC.Iid }
                           };
            this.euB = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edC.Iid };

            this.edB = new ElementDefinition { Iid = Guid.NewGuid() };
            this.euA = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edB.Iid };

            this.edA = new ElementDefinition
                           {
                               Iid = Guid.NewGuid(),
                               ContainedElement = new List<Guid> { this.euA.Iid, this.euB.Iid }
                           };
            this.euD = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edA.Iid };

            this.elementDefinitionService = new Mock<IElementDefinitionService>();
            this.elementDefinitionService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing> { this.edA, this.edB, this.edC, this.edD });

            this.elementUsageService = new Mock<IElementUsageService>();
            this.elementUsageService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing> { this.euA, this.euB, this.euC, this.euD, this.euE });

            this.iteration = new Iteration
                                 {
                                     Iid = Guid.NewGuid(),
                                     Element = new List<Guid>
                                                   {
                                                       this.edA.Iid,
                                                       this.edB.Iid,
                                                       this.edC.Iid,
                                                       this.edD.Iid
                                                   }
                                 };

            this.sideEffect = new ElementDefinitionSideEffect()
                                  {
                                      ElementDefinitionService =
                                          this.elementDefinitionService.Object,
                                      ElementUsageService =
                                          this.elementUsageService.Object
                                  };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenElementUsageLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, new List<Guid> { this.euD.Iid } } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.edC,
                    this.iteration,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhennElementUsageDoesNotLeadToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, new List<Guid> { this.euE.Iid } } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.edC,
                    this.iteration,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}