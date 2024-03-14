// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometHasStartedServiceTestFixture" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Tests.Health
{
    using System;

    using CometServer.Health;

    using Microsoft.Extensions.Caching.Memory;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CometHasStartedServiceTestFixture
    {
        private CometHasStartedService cometHasStartedService;

        [SetUp]
        public void SetUp()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            this.cometHasStartedService = new CometHasStartedService(memoryCache);
        }

        [Test]
        public void Verify_that_after_GetHasStartedAndIsReady_returns_and_can_be_updated()
        {
            var result = this.cometHasStartedService.GetHasStartedAndIsReady();
            var dateTime = result.DateTime;

            Assert.That(result.IsHealthy , Is.False);
            Assert.That(result.DateTime, Is.EqualTo(DateTime.MinValue));

            this.cometHasStartedService.SetHasStartedAndIsReady(true);

            result = this.cometHasStartedService.GetHasStartedAndIsReady();
            
            Assert.That(result.IsHealthy, Is.True);
            Assert.That(result.DateTime, Is.GreaterThan(dateTime));

            dateTime = result.DateTime;

            this.cometHasStartedService.SetHasStartedAndIsReady(false);

            result = this.cometHasStartedService.GetHasStartedAndIsReady();

            Assert.That(result.IsHealthy, Is.False);
            Assert.That(result.DateTime, Is.GreaterThan(dateTime));
        }
    }
}
