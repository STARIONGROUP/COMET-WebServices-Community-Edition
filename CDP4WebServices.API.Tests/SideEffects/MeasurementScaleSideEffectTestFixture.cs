// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Suite of tests for the <see cref="MeasurementScaleSideEffectTestFixture"/>
    /// </summary>
    [TestFixture]
    public class MeasurementScaleSideEffectTestFixture
    {
        private MeasurementScale rootMeasurementScale;
        private MappingToReferenceScale rootMappingToReferenceScale;
        private ScaleValueDefinition scaleValueDefinition;
        private MeasurementScale containerMeasurementScale;

        private SiteReferenceDataLibrary srdl;
        private ModelReferenceDataLibrary mrdl;

        private NpgsqlTransaction npgsqlTransaction;
        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;
        private Mock<IMappingToReferenceScaleService> mappingToReferenceScaleService;
        private Mock<IScaleValueDefinitionService> scaleValueDefinitionService;
        private Mock<IMeasurementScaleService> measurementScaleService;

        private MeasurementScaleSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.scaleValueDefinition = new ScaleValueDefinition(Guid.NewGuid(), 0);

            this.containerMeasurementScale = new OrdinalScale(Guid.NewGuid(), 0)
            {
                ValueDefinition =
                {
                    this.scaleValueDefinition.Iid
                }
            };

            this.rootMappingToReferenceScale = new MappingToReferenceScale(Guid.NewGuid(), 0)
            {
                ReferenceScaleValue = this.scaleValueDefinition.Iid,
                DependentScaleValue = this.scaleValueDefinition.Iid
            };

            this.rootMeasurementScale = new OrdinalScale(Guid.NewGuid(), 0)
            {
                MappingToReferenceScale =
                {
                    this.rootMappingToReferenceScale.Iid
                }
            };

            // RDL chain: mrdl -> srdl
            this.srdl = new SiteReferenceDataLibrary(Guid.NewGuid(), 0);

            this.mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), 0)
            {
                Scale =
                {
                    this.rootMeasurementScale.Iid,
                    this.containerMeasurementScale.Iid,
                },
                RequiredRdl = this.srdl.Iid
            };

            // setup services
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    null,
                    It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary>
                {
                    this.srdl
                });

            this.mappingToReferenceScaleService = new Mock<IMappingToReferenceScaleService>();
            this.mappingToReferenceScaleService
                .Setup(x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(
                    (transaction, partition, iids, context) =>
                    {
                        iids = iids.ToList();

                        return new List<Thing>
                        {
                            this.rootMappingToReferenceScale
                        }.Where(qk => iids.Contains(qk.Iid));
                    });

            this.scaleValueDefinitionService = new Mock<IScaleValueDefinitionService>();
            this.scaleValueDefinitionService
                .Setup(x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(
                    (transaction, partition, iids, context) =>
                    {
                        iids = iids.ToList();

                        return new List<Thing>
                        {
                            this.scaleValueDefinition
                        }.Where(qk => iids.Contains(qk.Iid));
                    });

            this.measurementScaleService = new Mock<IMeasurementScaleService>();
            this.measurementScaleService
                .Setup(x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    null,
                    It.IsAny<ISecurityContext>()))
                .Returns(new List<MeasurementScale>
                {
                    this.rootMeasurementScale,
                    this.containerMeasurementScale
                });

            this.sideEffect = new MeasurementScaleSideEffect
            {
                SiteReferenceDataLibraryService = this.siteReferenceDataLibraryService.Object,
                MappingToReferenceScaleService = this.mappingToReferenceScaleService.Object,
                ScaleValueDefinitionService = this.scaleValueDefinitionService.Object,
                MeasurementScaleService = this.measurementScaleService.Object
            };
        }

        [Test]
        public void VerifyOutOfRdlChainError()
        {
            this.mrdl.Scale.Remove(this.containerMeasurementScale.Iid);

            var rawUpdateInfo = new ClasslessDTO
            {
                { "MappingToReferenceScale", new List<Guid> { this.rootMappingToReferenceScale.Iid } }
            };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.rootMeasurementScale,
                    this.mrdl,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    rawUpdateInfo));
        }

        [Test]
        public void VerifyCircularDependencyError()
        {
            this.containerMeasurementScale.ValueDefinition.Remove(this.scaleValueDefinition.Iid);
            this.rootMeasurementScale.ValueDefinition.Add(this.scaleValueDefinition.Iid);

            var rawUpdateInfo = new ClasslessDTO
            {
                { "MappingToReferenceScale", new List<Guid> { this.rootMappingToReferenceScale.Iid } }
            };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.rootMeasurementScale,
                    this.mrdl,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    rawUpdateInfo));
        }

        [Test]
        public void VerifyNoCircularDependency()
        {
            var rawUpdateInfo = new ClasslessDTO
            {
                { "MappingToReferenceScale", new List<Guid> { this.rootMappingToReferenceScale.Iid } }
            };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.rootMeasurementScale,
                    this.mrdl,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    rawUpdateInfo));
        }
    }
}
