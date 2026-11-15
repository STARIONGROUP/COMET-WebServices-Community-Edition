// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileImportApiTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.Tests.Modules
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Protocol;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="ExchangeFileImportApi"/> class
    /// </summary>
    [TestFixture]
    public class ExchangeFileImportApiTestFixture
    {
        private ExchangeFileImportApi exchangeFileImportApi;

        private Mock<IRequestUtils> requestUtils;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<IRevisionService> revisionService;

        private Mock<IPersonService> personService;

        private Mock<ISiteDirectoryService> siteDirectoryService;

        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;

        [SetUp]
        public void SetUp()
        {
            this.requestUtils = new Mock<IRequestUtils>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.revisionService = new Mock<IRevisionService>();
            this.personService = new Mock<IPersonService>();
            this.siteDirectoryService = new Mock<ISiteDirectoryService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();

            this.personService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .ReturnsAsync([new Person(Guid.NewGuid(), 1)]);

            this.exchangeFileImportApi = new ExchangeFileImportApi(
                new Mock<IAppConfigService>().Object,
                new Mock<ICometHasStartedService>().Object,
                new Mock<ITokenGeneratorService>().Object,
                NullLogger<ExchangeFileImportApi>.Instance,
                new Mock<IDataSource>().Object);
        }

        [Test]
        public void VerifyThatAFailureToCreateTheRevisionHistoryIsRaised()
        {
            // the error from GH384, which used to be logged while the seed still answered 200
            var failure = new NpgsqlException("A statement cannot have more than 65535 parameters");

            this.revisionService.Setup(x => x.SaveRevisionsAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .ThrowsAsync(failure);

            Assert.That(
                async () => await this.exchangeFileImportApi.CreateRevisionHistoryForEachEntryAsync(this.requestUtils.Object, this.transactionManager.Object, this.revisionService.Object, this.personService.Object, this.siteDirectoryService.Object, this.engineeringModelSetupService.Object),
                Throws.Exception.SameAs(failure));

            this.transactionManager.Verify(x => x.TryDisposeTransaction(It.IsAny<NpgsqlTransaction>()), Times.Once);
            this.siteDirectoryService.Verify(x => x.GetAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()), Times.Never);
        }
    }
}
