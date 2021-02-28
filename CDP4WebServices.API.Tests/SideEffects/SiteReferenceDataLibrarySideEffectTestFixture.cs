// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibrarySideEffectTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="SiteReferenceDataLibrarySideEffect"/>
    /// </summary>
    [TestFixture]
    public class SiteReferenceDataLibrarySideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private SiteReferenceDataLibrarySideEffect sideEffect;

        private SiteReferenceDataLibrary siteReferenceDataLibraryA;

        private SiteReferenceDataLibrary siteReferenceDataLibraryB;

        private SiteReferenceDataLibrary siteReferenceDataLibraryC;

        private SiteReferenceDataLibrary siteReferenceDataLibraryD;

        private SiteDirectory siteDirectory;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "RequiredRdl";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain a -> b -> c
            this.siteReferenceDataLibraryD = new SiteReferenceDataLibrary { Iid = Guid.NewGuid() };
            this.siteReferenceDataLibraryC = new SiteReferenceDataLibrary { Iid = Guid.NewGuid() };
            this.siteReferenceDataLibraryB =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), RequiredRdl = this.siteReferenceDataLibraryC.Iid };
            this.siteReferenceDataLibraryA =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), RequiredRdl = this.siteReferenceDataLibraryB.Iid };

            this.siteDirectory = new SiteDirectory
                                     {
                                         Iid = Guid.NewGuid(),
                                         SiteReferenceDataLibrary =
                                             {
                                                 this.siteReferenceDataLibraryA.Iid,
                                                 this.siteReferenceDataLibraryB.Iid,
                                                 this.siteReferenceDataLibraryC.Iid
                                             }
                                     };

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.siteReferenceDataLibraryA.Iid,
                                this.siteReferenceDataLibraryB.Iid,
                                this.siteReferenceDataLibraryC.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<SiteReferenceDataLibrary>
                        {
                            this.siteReferenceDataLibraryA,
                            this.siteReferenceDataLibraryB,
                            this.siteReferenceDataLibraryC
                        });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenRequiredRdlIsRdlItselfOnUpdate()
        {
            this.sideEffect =
                new SiteReferenceDataLibrarySideEffect()
                    {
                        SiteReferenceDataLibraryService =
                            this.siteReferenceDataLibraryService.Object
                    };

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.siteReferenceDataLibraryA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.siteReferenceDataLibraryA,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenRequiredRdlIsRdlItselfOnCreate()
        {
            this.sideEffect =
                new SiteReferenceDataLibrarySideEffect()
                    {
                        SiteReferenceDataLibraryService =
                            this.siteReferenceDataLibraryService.Object
                    };

            var id = this.siteReferenceDataLibraryA.Iid;
            this.siteReferenceDataLibraryA.RequiredRdl = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.siteReferenceDataLibraryA,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenRequiredRdlIsOutOfSiteDirectoryOrLeadsToCircularDependencyOnUpdate()
        {
            this.sideEffect =
                new SiteReferenceDataLibrarySideEffect()
                    {
                        SiteReferenceDataLibraryService =
                            this.siteReferenceDataLibraryService.Object
                    };

            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.siteReferenceDataLibraryD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.siteReferenceDataLibraryA,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.siteReferenceDataLibraryA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.siteReferenceDataLibraryC,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenRequiredRdlIsOutOfSiteDirectoryOrLeadsToCircularDependencyOnCreate()
        {
            this.sideEffect =
                new SiteReferenceDataLibrarySideEffect()
                    {
                        SiteReferenceDataLibraryService =
                            this.siteReferenceDataLibraryService.Object
                    };

            // Out of the store
            var id = this.siteReferenceDataLibraryD.Iid;
            this.siteReferenceDataLibraryA.RequiredRdl = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.siteReferenceDataLibraryA,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
            this.siteReferenceDataLibraryA.RequiredRdl = this.siteReferenceDataLibraryB.Iid;

            // Leads to circular dependency
            id = this.siteReferenceDataLibraryA.Iid;
            this.siteReferenceDataLibraryC.RequiredRdl = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.siteReferenceDataLibraryC,
                    this.siteDirectory,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }
    }
}