// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrefixedUnitSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="PrefixedUnitSideEffect"/>
    /// </summary>
    [TestFixture]
    public class PrefixedUnitSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<IConversionBasedUnitService> conversionBasedUnitService;

        private PrefixedUnitSideEffect sideEffect;

        private PrefixedUnit prefixedUnitA;

        private PrefixedUnit prefixedUnitB;

        private PrefixedUnit prefixedUnitC;

        private PrefixedUnit prefixedUnitD;

        private PrefixedUnit prefixedUnitE;

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
            this.prefixedUnitC = new PrefixedUnit { Iid = Guid.NewGuid() };
            this.prefixedUnitB = new PrefixedUnit { Iid = Guid.NewGuid(), ReferenceUnit = this.prefixedUnitC.Iid };
            this.prefixedUnitA = new PrefixedUnit { Iid = Guid.NewGuid(), ReferenceUnit = this.prefixedUnitB.Iid };
            this.prefixedUnitD = new PrefixedUnit { Iid = Guid.NewGuid() };

            // Outside the rdl chain
            this.prefixedUnitE = new PrefixedUnit { Iid = Guid.NewGuid() };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), Unit = { this.prefixedUnitD.Iid } };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 Unit =
                                                     {
                                                         this.prefixedUnitA.Iid,
                                                         this.prefixedUnitB.Iid,
                                                         this.prefixedUnitC.Iid
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
                                this.prefixedUnitD.Iid,
                                this.prefixedUnitA.Iid,
                                this.prefixedUnitB.Iid,
                                this.prefixedUnitC.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ConversionBasedUnit>
                        {
                            this.prefixedUnitD,
                            this.prefixedUnitA,
                            this.prefixedUnitB,
                            this.prefixedUnitC
                        });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenReferenceIsUnitItself()
        {
            this.sideEffect = new PrefixedUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.prefixedUnitA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.prefixedUnitA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenReferenceUnitIsOutOfChainOrLeadsToCircularDependency()
        {
            this.sideEffect = new PrefixedUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // Out of chain
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.prefixedUnitE.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.prefixedUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.prefixedUnitA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.prefixedUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenReferenceUnitDoesNotLeadToCircularDependency()
        {
            this.sideEffect = new PrefixedUnitSideEffect
                                  {
                                      ConversionBasedUnitService =
                                          this.conversionBasedUnitService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // There is a chain a -> b -> c
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.prefixedUnitD.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.prefixedUnitC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}