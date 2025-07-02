// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    public class IterationServiceTestFixture
    {
        private Mock<IIterationSetupService> iterationSetupService;
        private Mock<IIterationDao> iterationDaoService;
        private Mock<ICdp4TransactionManager> transactionManager;

        private IterationService iterationService;

        [SetUp]
        public void Setup()
        {
            this.iterationDaoService = new Mock<IIterationDao>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
            this.transactionManager.Setup(x => x.GetRawSessionInstantAsync(It.IsAny<NpgsqlTransaction>())).Returns(Task.FromResult<object>(DateTime.MaxValue));

            this.iterationService = new IterationService
            {
                IterationDao = this.iterationDaoService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                TransactionManager = this.transactionManager.Object
            };
        }

        [Test]
        public async Task Verify_that_GetActiveIteration_returns_the_expected_result()
        {
            var setup1 = new IterationSetup(Guid.NewGuid(), 0) { IterationIid = Guid.NewGuid(), FrozenOn = DateTime.Now };
            var setup2 = new IterationSetup(Guid.NewGuid(), 0) { IterationIid = Guid.NewGuid() };

            var it1 = new Iteration(setup1.IterationIid, 0) { IterationSetup = setup1.Iid };
            var it2 = new Iteration(setup2.IterationIid, 0) { IterationSetup = setup2.Iid };

            this.iterationDaoService.Setup(x => x.ReadAsync(It.IsAny<NpgsqlTransaction>(), "", It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), DateTime.MaxValue)).Returns(Task.FromResult<IEnumerable<Iteration>>([it1, it2]));
            this.iterationSetupService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult<IEnumerable<Thing>>([setup1, setup2]));

            var active = await this.iterationService.GetActiveIterationAsync(null, "", null);

            Assert.That(it2.Iid, Is.EqualTo(active.Iid));
        }
    }
}
