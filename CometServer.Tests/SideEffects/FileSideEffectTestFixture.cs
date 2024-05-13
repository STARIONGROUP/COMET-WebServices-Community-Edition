// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSideEffectTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Security;

    using CDP4Common.DTO;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="FileSideEffect"/>
    /// </summary>
    [TestFixture]
    public class FileSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;
        private Mock<IDomainFileStoreService> domainFileStoreService;
        private File file;
        private DomainFileStore fileStore;
        private FileSideEffect sideEffect;
        private Mock<IFileService> fileService;

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.file = new File(Guid.NewGuid(), 0);

            this.fileStore = new DomainFileStore
                                 {
                                     Iid = Guid.NewGuid(),
                                     File = {
                                                 this.file.Iid
                                              }
                                 };

            this.domainFileStoreService = new Mock<IDomainFileStoreService>();
            this.fileService = new Mock<IFileService>();

            this.sideEffect = new FileSideEffect
            {
                DomainFileStoreService = this.domainFileStoreService.Object,
                FileService = this.fileService.Object,
            };
        }

        [Test]
        public void VerifyThatBeforeDeleteCheckSecurityWorks()
        {
           this.sideEffect.BeforeDelete(this.file, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeUpdateCheckSecurityWorks()
        {
            this.sideEffect.BeforeUpdate(this.file, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null);

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeCreateCheckSecurityWorks()
        {
            this.sideEffect.BeforeCreate(this.file, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void VerifyThatAdditionalCheckSecurityChecksWork()
        {
            //Locked by me
            this.sideEffect.BeforeUpdate(this.file, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null);
            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);

            //Locked by someone else
            this.fileService.Setup(x => x.CheckFileLock(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), this.file)).Throws<SecurityException>();
            Assert.Throws<SecurityException>(() => this.sideEffect.BeforeUpdate(this.file, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null));
        }
    }
}