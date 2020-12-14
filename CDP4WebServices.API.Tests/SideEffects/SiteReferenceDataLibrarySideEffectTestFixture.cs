// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibrarySideEffectTestFixture.cs" company="RHEA System S.A.">
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