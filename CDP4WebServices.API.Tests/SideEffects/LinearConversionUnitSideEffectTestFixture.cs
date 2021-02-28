// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearConversionUnitSideEffectTestFixture.cs" company="RHEA System S.A.">
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

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.linearConversionUnitA.Iid } };

            Assert.Throws<AcyclicValidationException>(
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
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.linearConversionUnitE.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.linearConversionUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.linearConversionUnitA.Iid } };

            Assert.Throws<AcyclicValidationException>(
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
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.linearConversionUnitD.Iid } };

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