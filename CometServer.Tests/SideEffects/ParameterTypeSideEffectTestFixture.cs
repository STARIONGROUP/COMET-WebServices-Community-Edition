// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeSideEffectTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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

namespace CometServer.Tests.SideEffects
{
    using CDP4Common.DTO;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects.Implementation;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    public class ParameterTypeSideEffectTestFixture
    {
        private ParameterTypeSideEffect parameterTypeSideEffect;

        private Mock<ICachedReferenceDataService> cachedReferenceDataService;
        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;

        [SetUp]
        public void SetUp()
        {
            this.cachedReferenceDataService = new Mock<ICachedReferenceDataService>();

            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();
            this.defaultValueArrayFactory.SetupProperty(x => x.CachedReferenceDataService, this.cachedReferenceDataService.Object);

            this.parameterTypeSideEffect = new ParameterTypeSideEffect();
            
            this.parameterTypeSideEffect.DefaultValueArrayFactory = this.defaultValueArrayFactory.Object;
            this.parameterTypeSideEffect.CachedReferenceDataService = this.cachedReferenceDataService.Object;
        }

        [Test]
        public void Verify_that_upon_AfterCreate_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterCreate(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);

            this.cachedReferenceDataService.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterDelete_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterDelete(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);

            this.cachedReferenceDataService.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterUpdate_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterUpdate(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);

            this.cachedReferenceDataService.Verify(x => x.Reset(), Times.Once);
        }
    }
}
