// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationServiceTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using API.Helpers;
    using API.Services;
    using API.Services.Authorization;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
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
            this.iterationService = new IterationService
            {
                IterationDao = this.iterationDaoService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                TransactionManager = this.transactionManager.Object
            };
        }

        [Test]
        public void TestGetActiveIteration()
        {
            var setup1 = new IterationSetup(Guid.NewGuid(), 0) {IterationIid = Guid.NewGuid(), FrozenOn = DateTime.Now };
            var setup2 = new IterationSetup(Guid.NewGuid(), 0) {IterationIid = Guid.NewGuid()};

            var it1 = new Iteration(setup1.IterationIid, 0) { IterationSetup = setup1.Iid};
            var it2 = new Iteration(setup2.IterationIid, 0) { IterationSetup = setup2.Iid};

            this.iterationDaoService.Setup(x => x.Read(It.IsAny<NpgsqlTransaction>(), "", It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>())).Returns(new[] {it1, it2});
            this.iterationSetupService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(new[] {setup1, setup2});

            var active = this.iterationService.GetActiveIteration(null, "", null);

            Assert.AreEqual(active.Iid, it2.Iid);
        }
    }
}
