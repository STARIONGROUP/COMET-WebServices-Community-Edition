// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderSideEffectTestFixture.cs" company="RHEA System S.A.">
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
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.folderA.Iid, this.folderB.Iid, this.folderC.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new List<Folder> { this.folderA, this.folderB, this.folderC });

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
            // Out of the store
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.folderD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.folderA,
                    this.fileStore,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.folderA.Iid } };

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

        [Test]
        public void VerifyThatBeforeDeleteCheckSecurityWorks()
        {
           this.sideEffect.BeforeDelete(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.CheckSecurity(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeUpdateCheckSecurityWorks()
        {
            this.rawUpdateInfo = new ClasslessDTO();

            this.sideEffect.BeforeUpdate(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), this.rawUpdateInfo);

            this.domainFileStoreService.Verify(x => x.CheckSecurity(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeCreateCheckSecurityWorks()
        {
            this.sideEffect.BeforeCreate(this.folderA, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.CheckSecurity(It.IsAny<Folder>(), null, It.IsAny<string>()), Times.Never);
        }
    }
}