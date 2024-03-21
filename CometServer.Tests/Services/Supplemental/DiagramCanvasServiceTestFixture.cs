// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramCanvasServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Protocol;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DiagramCanvasService"/>
    /// </summary>
    [TestFixture]
    public class DiagramCanvasServiceTestFixture
    {
        private DiagramCanvas diagramCanvas;

        private DiagramCanvasService diagramCanvasService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<IDiagramCanvasDao> diagramCanvasDao;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<IRequestUtils> requestUtils;
        private Mock<IArchitectureDiagramService> architectureDiagramService;
        private Guid personIid;

        [SetUp]
        public void Setup()
        {
            this.diagramCanvas = new DiagramCanvas(Guid.NewGuid(), 0);
            this.personIid = Guid.NewGuid();
            this.diagramCanvasDao = new Mock<IDiagramCanvasDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.requestUtils = new Mock<IRequestUtils>();
            this.architectureDiagramService = new Mock<IArchitectureDiagramService>();

            this.diagramCanvasService = new DiagramCanvasService
            {
                PermissionService = this.permissionService.Object,
                CredentialsService = this.credentialsService.Object,
                DiagramCanvasDao = this.diagramCanvasDao.Object,
                TransactionManager = this.transactionManager.Object,
                RequestUtils = this.requestUtils.Object,
                ArchitectureDiagramService = this.architectureDiagramService.Object
            };

            this.diagramCanvas.IsHidden = false;

            this.requestUtils.Setup(x => x.QueryParameters).Returns(new QueryParameters() { ExtentDeep = false });
            this.architectureDiagramService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(new[] { new ArchitectureDiagram(Guid.NewGuid(), 0) });

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), this.diagramCanvas)).Returns(true);

            this.credentialsService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(this.personIid, 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            this.diagramCanvasDao
                .Setup(
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), It.IsAny<DateTime?>()))
                .Returns(new[] { this.diagramCanvas });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
            this.transactionManager.Setup(x => x.GetRawSessionInstant(It.IsAny<NpgsqlTransaction>())).Returns(DateTime.MaxValue);
        }

        [Test]
        public void VerifyHasReadAccess()
        {
            Assert.That(this.diagramCanvasService.HasReadAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            this.diagramCanvas.LockedBy = this.personIid;

            Assert.That(() => this.diagramCanvasService.HasReadAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            this.diagramCanvas.IsHidden = true;

            Assert.That(() => this.diagramCanvasService.HasReadAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            this.diagramCanvas.LockedBy = Guid.NewGuid();

            Assert.That(() => this.diagramCanvasService.HasReadAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.False);

            this.diagramCanvas.IsHidden = false;

            Assert.That(() => this.diagramCanvasService.HasReadAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);
        }

        [Test]
        public void VerifyHasWriteAccess()
        {
            Assert.That(this.diagramCanvasService.HasWriteAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            //Check if correct diagramCanvasDao was called
            this.diagramCanvasDao.Verify(x => x.Read(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), It.IsAny<DateTime?>()));

            this.diagramCanvas.LockedBy = this.personIid;

            Assert.That(() => this.diagramCanvasService.HasWriteAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            this.diagramCanvas.IsHidden = true;

            Assert.That(() => this.diagramCanvasService.HasWriteAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Is.True);

            this.diagramCanvas.LockedBy = Guid.NewGuid();

            Assert.That(() => this.diagramCanvasService.HasWriteAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Throws.TypeOf<SecurityException>());

            this.diagramCanvas.IsHidden = false;

            Assert.That(() => this.diagramCanvasService.HasWriteAccess(
                this.diagramCanvas,
                It.IsAny<NpgsqlTransaction>(),
                ""), Throws.TypeOf<SecurityException>());
        }
    }
}
