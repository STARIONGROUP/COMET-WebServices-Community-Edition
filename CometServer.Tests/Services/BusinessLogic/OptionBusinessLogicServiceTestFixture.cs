// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionBusinessLogicServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="OptionBusinessLogicService"/>.
    /// </summary>
    [TestFixture]
    public class OptionBusinessLogicServiceTestFixture
    {
        [Test]
        public void InitializeThrowsWhenCountsDoNotMatch()
        {
            var iteration = new Iteration
            {
                Option = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var service = new OptionBusinessLogicService();

            Assert.Throws<InvalidOperationException>(() => service.Initialize(iteration, new List<Option> { new Option() }));
        }

        [Test]
        public void GetDefaultOptionReturnsConfiguredOption()
        {
            var defaultOptionId = Guid.NewGuid();
            var iteration = new Iteration
            {
                Option = new List<Guid> { defaultOptionId },
                DefaultOption = defaultOptionId
            };

            var option = new Option { Iid = defaultOptionId };
            var service = new OptionBusinessLogicService();

            service.Initialize(iteration, new List<Option> { option });

            Assert.That(service.GetDefaultOption(), Is.EqualTo(option));
        }

        [Test]
        public void GetDefaultOptionReturnsNullWhenNotDefined()
        {
            var iteration = new Iteration
            {
                Option = new List<Guid> { Guid.NewGuid() }
            };

            var option = new Option { Iid = Guid.NewGuid() };
            var service = new OptionBusinessLogicService();

            service.Initialize(iteration, new List<Option> { option });

            Assert.That(service.GetDefaultOption(), Is.Null);
        }
    }
}
