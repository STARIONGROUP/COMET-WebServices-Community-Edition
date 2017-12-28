// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="FolderSideEffect"/>
    /// </summary>
    [TestFixture]
    public class FolderSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IFolderService> folderService;

        private FolderSideEffect sideEffect;

        private Folder folderA;

        private Folder folderB;

        private Folder folderC;

        private Folder folderD;

        private DomainFileStore fileStore;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ContainingFolder";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain a -> b -> c
            this.folderD = new Folder { Iid = Guid.NewGuid() };
            this.folderC = new Folder { Iid = Guid.NewGuid() };
            this.folderB = new Folder { Iid = Guid.NewGuid(), ContainingFolder = this.folderC.Iid };
            this.folderA = new Folder { Iid = Guid.NewGuid(), ContainingFolder = this.folderB.Iid };

            this.fileStore = new DomainFileStore
                                 {
                                     Iid = Guid.NewGuid(),
                                     Folder = {
                                                 this.folderA.Iid, this.folderB.Iid, this.folderC.Iid 
                                              }
                                 };

            this.folderService = new Mock<IFolderService>();
            this.folderService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.folderA.Iid, this.folderB.Iid, this.folderC.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new List<Folder> { this.folderA, this.folderB, this.folderC });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsFolderItselfOnUpdate()
        {
            this.sideEffect = new FolderSideEffect() { FolderService = this.folderService.Object };

            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.folderA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsFolderItselfOnCreate()
        {
            this.sideEffect = new FolderSideEffect() { FolderService = this.folderService.Object };

            var id = this.folderA.Iid;
            this.folderA.ContainingFolder = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsOutOfChainOrLeadsToCircularDependencyOnUpdate()
        {
            this.sideEffect = new FolderSideEffect() { FolderService = this.folderService.Object };

            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.folderD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.folderA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.folderC,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsOutOfChainOrLeadsToCircularDependencyOnCreate()
        {
            this.sideEffect = new FolderSideEffect() { FolderService = this.folderService.Object };

            // Out of the store
            var id = this.folderD.Iid;
            this.folderA.ContainingFolder = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
            this.folderA.ContainingFolder = this.folderB.Iid;

            // Leads to circular dependency
            id = this.folderA.Iid;
            this.folderC.ContainingFolder = id;

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreate(
                    this.folderC,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }
    }
}