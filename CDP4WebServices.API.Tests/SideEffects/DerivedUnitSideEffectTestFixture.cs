// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DerivedUnitSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DerivedUnitSideEffect"/>
    /// </summary>
    [TestFixture]
    public class DerivedUnitSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<IDerivedUnitService> derivedUnitService;

        private Mock<IUnitFactorService> unitFactorService;

        private DerivedUnitSideEffect sideEffect;

        private DerivedUnit derivedUnitA;

        private DerivedUnit derivedUnitB;

        private DerivedUnit derivedUnitC;

        private SimpleUnit simpleUnitD;

        private UnitFactor unitFactorA;

        private UnitFactor unitFactorB;

        private UnitFactor unitFactorC;

        private UnitFactor unitFactorD;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "UnitFactor";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain duA -> ufA -> duB -> ufB -> suD
            this.simpleUnitD = new SimpleUnit { Iid = Guid.NewGuid() };
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

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), Unit = { this.simpleUnitD.Iid } };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 Unit =
                                                     {
                                                         this.derivedUnitA.Iid,
                                                         this.derivedUnitB.Iid
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

            this.derivedUnitService = new Mock<IDerivedUnitService>();
            this.derivedUnitService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.simpleUnitD.Iid, this.derivedUnitA.Iid, this.derivedUnitB.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new List<DerivedUnit> { this.derivedUnitA, this.derivedUnitB });

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
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenUnitFactorLeadsToCircularDependency()
        {
            this.sideEffect = new DerivedUnitSideEffect()
                                  {
                                      DerivedUnitService = this.derivedUnitService.Object,
                                      UnitFactorService = this.unitFactorService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, new List<OrderedItem> { new OrderedItem{K = 3, V = this.unitFactorC.Iid} } } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.derivedUnitB,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenUnitFactorDoesNotLeadToCircularDependency()
        {
            this.sideEffect = new DerivedUnitSideEffect()
                                  {
                                      DerivedUnitService = this.derivedUnitService.Object,
                                      UnitFactorService = this.unitFactorService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, new List<OrderedItem> { new OrderedItem { K = 4, V = this.unitFactorD.Iid } } } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.derivedUnitB,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}