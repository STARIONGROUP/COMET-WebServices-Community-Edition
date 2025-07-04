﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderSideEffectTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
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
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Exceptions;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

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
        private Mock<IDomainFileStoreService> domainFileStoreService;
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

            this.domainFileStoreService = new Mock<IDomainFileStoreService>();
            this.folderService = new Mock<IFolderService>();

            this.folderService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.folderA.Iid, this.folderB.Iid, this.folderC.Iid },
                        It.IsAny<ISecurityContext>()))
                .ReturnsAsync(new List<Folder> { this.folderA, this.folderB, this.folderC });

            this.sideEffect = new FolderSideEffect
            {
                FolderService = this.folderService.Object,
                DomainFileStoreService = this.domainFileStoreService.Object
            };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsFolderItselfOnUpdate()
        {
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.folderA.Iid } };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
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
            var id = this.folderA.Iid;
            this.folderA.ContainingFolder = id;

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreateAsync(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenContainingFolderIsOutOfChainOrLeadsToCircularDependencyOnUpdate()
        {
            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.folderD.Iid } };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.folderA.Iid } };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
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
            // Out of the store
            var id = this.folderD.Iid;
            this.folderA.ContainingFolder = id;

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreateAsync(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));

            this.folderA.ContainingFolder = this.folderB.Iid;

            // Leads to circular dependency
            id = this.folderA.Iid;
            this.folderC.ContainingFolder = id;

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeCreateAsync(
                    this.folderC,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatBeforeDeleteCheckSecurityWorks()
        {
           this.sideEffect.BeforeDeleteAsync(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccessAsync(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task VerifyThatBeforeUpdateCheckSecurityWorks()
        {
            this.rawUpdateInfo = new ClasslessDTO();

            await this.sideEffect.BeforeUpdateAsync(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), this.rawUpdateInfo);

            this.domainFileStoreService.Verify(x => x.HasWriteAccessAsync(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task VerifyThatBeforeCreateCheckSecurityWorks()
        {
            await this.sideEffect.BeforeCreateAsync(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccessAsync(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Never);
        }
    }
}