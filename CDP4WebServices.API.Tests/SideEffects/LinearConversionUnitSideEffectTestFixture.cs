// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearConversionUnitSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="LinearConversionUnitSideEffect"/>
    /// </summary>
    [TestFixture]
    public class LinearConversionUnitSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<IConversionBasedUnitService> conversionBasedUnitService;

        private LinearConversionUnitSideEffect sideEffect;

        private LinearConversionUnit linearConversionUnitA;

        private LinearConversionUnit linearConversionUnitB;

        private LinearConversionUnit linearConversionUnitC;

        private LinearConversionUnit linearConversionUnitD;

        private LinearConversionUnit linearConversionUnitE;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ReferenceUnit";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain a -> b -> c 
            this.linearConversionUnitC = new LinearConversionUnit { Iid = Guid.NewGuid() };
            this.linearConversionUnitB =
                new LinearConversionUnit { Iid = Guid.NewGuid(), ReferenceUnit = this.linearConversionUnitC.Iid };
            this.linearConversionUnitA =
                new LinearConversionUnit { Iid = Guid.NewGuid(), ReferenceUnit = this.linearConversionUnitB.Iid };
            this.linearConversionUnitD = new LinearConversionUnit { Iid = Guid.NewGuid() };

            // Outside the rdl chain
            this.linearConversionUnitE = new LinearConversionUnit { Iid = Guid.NewGuid() };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), Unit = { this.linearConversionUnitD.Iid } };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 Unit =
                                                     {
                                                         this.linearConversionUnitA.Iid,
                                                         this.linearConversionUnitB.Iid,
                                                         this.linearConversionUnitC.Iid
                                                     },
                                                 RequiredRdl = this.referenceDataLibraryB.Iid
                                             };

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary> { this.referenceDataLibraryB });

            this.conversionBasedUnitService = new Mock<IConversionBasedUnitService>();
            this.conversionBasedUnitService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.linearConversionUnitD.Iid,
                                this.linearConversionUnitA.Iid,
                                this.linearConversionUnitB.Iid,
                                this.linearConversionUnitC.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ConversionBasedUnit>
                        {
                            this.linearConversionUnitD,
                            this.linearConversionUnitA,
                            this.linearConversionUnitB,
                            this.linearConversionUnitC
                        });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenReferenceIsUnitItself()
        {
            this.sideEffect = new LinearConversionUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.linearConversionUnitA.Iid } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.linearConversionUnitA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenReferenceUnitIsOutOfChainOrLeadsToCircularDependency()
        {
            this.sideEffect = new LinearConversionUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // Out of chain
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.linearConversionUnitE.Iid } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.linearConversionUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.linearConversionUnitA.Iid } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.linearConversionUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenReferenceUnitDoesNotLeadToCircularDependency()
        {
            this.sideEffect = new LinearConversionUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // There is a chain a -> b -> c
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.linearConversionUnitD.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.linearConversionUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}