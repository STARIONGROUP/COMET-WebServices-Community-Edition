// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitFactorSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="UnitFactorSideEffect"/>
    /// </summary>
    [TestFixture]
    public class UnitFactorSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<IModelReferenceDataLibraryService> modelReferenceDataLibraryService;

        private Mock<IDerivedUnitService> derivedUnitService;

        private Mock<IUnitFactorService> unitFactorService;

        private UnitFactorSideEffect sideEffect;

        private DerivedUnit derivedUnitA;

        private DerivedUnit derivedUnitB;

        private DerivedUnit derivedUnitC;

        private SimpleUnit simpleUnitD;

        private SimpleUnit simpleUnitE;

        private UnitFactor unitFactorA;

        private UnitFactor unitFactorB;

        private UnitFactor unitFactorC;

        private UnitFactor unitFactorD;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "Unit";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain duA -> ufA -> duB -> ufB -> suD
            this.simpleUnitD = new SimpleUnit { Iid = Guid.NewGuid() };
            this.simpleUnitE = new SimpleUnit { Iid = Guid.NewGuid() };
            this.unitFactorD = new UnitFactor { Iid = Guid.NewGuid(), Unit = this.simpleUnitD.Iid };
            this.unitFactorB = new UnitFactor { Iid = Guid.NewGuid(), Unit = this.simpleUnitD.Iid };

            var unitFactorForB = new OrderedItem { K = 1, V = this.unitFactorB.Iid };
            var unitFactorsForB = new List<OrderedItem> { unitFactorForB };

            this.derivedUnitB = new DerivedUnit { Iid = Guid.NewGuid(), UnitFactor = unitFactorsForB };

            this.unitFactorA = new UnitFactor { Iid = Guid.NewGuid(), Unit = this.derivedUnitB.Iid };

            var unitFactorForA = new OrderedItem { K = 1, V = this.unitFactorA.Iid };
            var unitFactorsForA = new List<OrderedItem> { unitFactorForA };

            this.derivedUnitA = new DerivedUnit { Iid = Guid.NewGuid(), UnitFactor = unitFactorsForA };

            this.unitFactorC = new UnitFactor { Iid = Guid.NewGuid(), Unit = this.derivedUnitA.Iid };

            var unitFactorForC = new OrderedItem { K = 1, V = this.unitFactorC.Iid };
            var unitFactorsForC = new List<OrderedItem> { unitFactorForC };
            this.derivedUnitC = new DerivedUnit { Iid = Guid.NewGuid(), UnitFactor = unitFactorsForC };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB =
                new SiteReferenceDataLibrary
                    {
                        Iid = Guid.NewGuid(),
                        Unit = {
                                  this.simpleUnitD.Iid, this.simpleUnitE.Iid 
                               }
                    };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 Unit =
                                                     {
                                                         this.derivedUnitA.Iid,
                                                         this.derivedUnitB.Iid,
                                                         this.derivedUnitC.Iid
                                                     },
                                                 RequiredRdl = this.referenceDataLibraryB.Iid
                                             };

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary> { this.referenceDataLibraryA, this.referenceDataLibraryB });

            this.modelReferenceDataLibraryService = new Mock<IModelReferenceDataLibraryService>();
            this.modelReferenceDataLibraryService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary>());

            this.derivedUnitService = new Mock<IDerivedUnitService>();
            this.derivedUnitService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.derivedUnitA.Iid,
                                this.derivedUnitB.Iid,
                                this.derivedUnitC.Iid,
                                this.simpleUnitD.Iid,
                                this.simpleUnitE.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<DerivedUnit> { this.derivedUnitA, this.derivedUnitB, this.derivedUnitC });

            this.unitFactorService = new Mock<IUnitFactorService>();
            this.unitFactorService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.unitFactorA.Iid },
                        It.IsAny<ISecurityContext>())).Returns(new List<UnitFactor> { this.unitFactorA });
            this.unitFactorService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.unitFactorB.Iid },
                        It.IsAny<ISecurityContext>())).Returns(new List<UnitFactor> { this.unitFactorB });
            this.unitFactorService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.unitFactorC.Iid },
                        It.IsAny<ISecurityContext>())).Returns(new List<UnitFactor> { this.unitFactorC });
            this.unitFactorService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.unitFactorD.Iid },
                        It.IsAny<ISecurityContext>())).Returns(new List<UnitFactor> { this.unitFactorD });

            this.sideEffect = new UnitFactorSideEffect()
                                  {
                                      DerivedUnitService = this.derivedUnitService.Object,
                                      UnitFactorService = this.unitFactorService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object,
                                      ModelReferenceDataLibraryService =
                                          this.modelReferenceDataLibraryService.Object
                                  };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenUnitFactorLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.derivedUnitC.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.unitFactorB,
                    this.derivedUnitB,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenUnitFactorDoesNotLeadToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.simpleUnitE.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.unitFactorB,
                    this.derivedUnitB,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}