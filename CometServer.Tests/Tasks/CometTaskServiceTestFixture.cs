// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometTaskServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Tasks
{
    using System;
    using System.Threading.Tasks;

    using CDP4DalCommon.Protocol.Tasks;

    using CometServer.Configuration;
    using CometServer.Tasks;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="CometTaskService"/> class
    /// </summary>
    [TestFixture]
    public class CometTaskServiceTestFixture
    {
        private MemoryCache memoryCache;

        private CometTaskService cometTaskService;

        [SetUp]
        public void SetUp()
        {
            var appConfig = new AppConfig();
            appConfig.LongRunningTasksConfig.RetentionTime = 1;

            var appConfigService = new Mock<IAppConfigService>();
            appConfigService.Setup(x => x.AppConfig).Returns(appConfig);

            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
            this.cometTaskService = new CometTaskService(this.memoryCache, appConfigService.Object, NullLogger<CometTaskService>.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            this.memoryCache.Dispose();
        }

        [Test]
        public async Task VerifyThatATaskThatIsStillRunningIsNotEvictedAfterTheRetentionTime()
        {
            var cometTask = CreateRunningTask();

            this.cometTaskService.AddOrUpdateTask(cometTask);

            await Task.Delay(TimeSpan.FromSeconds(1.5));

            Assert.That(this.cometTaskService.QueryTask(cometTask.Id)?.Id, Is.EqualTo(cometTask.Id));
        }

        [Test]
        public async Task VerifyThatTheRetentionTimeOfASuccessfulTaskStartsWhenItHasFinished()
        {
            var cometTask = CreateRunningTask();

            this.cometTaskService.AddOrUpdateTask(cometTask);

            // longer than the retention time: the task must still be there when it finishes
            await Task.Delay(TimeSpan.FromSeconds(1.5));

            this.cometTaskService.AddOrUpdateSuccessfulTask(cometTask, 42);

            // CometTask is a struct: the success must be stored, not only set on the copy that was passed in
            var queriedTask = this.cometTaskService.QueryTask(cometTask.Id);

            Assert.That(queriedTask, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(queriedTask.Value.StatusKind, Is.EqualTo(StatusKind.SUCCEEDED));
                Assert.That(queriedTask.Value.Revision, Is.EqualTo(42));
                Assert.That(queriedTask.Value.FinishedAt, Is.Not.Null);
            });

            await Task.Delay(TimeSpan.FromSeconds(1.5));

            Assert.That(this.cometTaskService.QueryTask(cometTask.Id), Is.Null);
        }

        [Test]
        public async Task VerifyThatAFailedTaskIsEvictedAfterTheRetentionTime()
        {
            var cometTask = CreateRunningTask();

            this.cometTaskService.AddOrUpdateTask(cometTask);
            this.cometTaskService.AddOrUpdateTask(cometTask, DateTime.Now, StatusKind.FAILED, "BAD REQUEST - test");

            Assert.That(this.cometTaskService.QueryTask(cometTask.Id)?.Error, Is.EqualTo("BAD REQUEST - test"));

            await Task.Delay(TimeSpan.FromSeconds(1.5));

            Assert.That(this.cometTaskService.QueryTask(cometTask.Id), Is.Null);
        }

        private static CometTask CreateRunningTask()
        {
            return new CometTask
            {
                Id = Guid.NewGuid(),
                Actor = Guid.NewGuid(),
                StatusKind = StatusKind.PROCESSING,
                StartedAt = DateTime.Now,
                TopContainer = "EngineeringModel"
            };
        }
    }
}
