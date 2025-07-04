// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DerivedQuantityKindSideEffectTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Exceptions;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DerivedUnitSideEffectTestFixture"/>
    /// </summary>
    [TestFixture]
    public class DerivedQuantityKindSideEffectTestFixture
    {
        private SimpleQuantityKind simpleQk;
        private SimpleQuantityKind outsideRdlQk;

        private DerivedQuantityKind derivedQk;
        private QuantityKindFactor derivedQkFactor;
        private QuantityKindFactor derivedQkCyclicFactor;
        private QuantityKindFactor derivedQkOutsideRdlFactor;

        private SpecializedQuantityKind specializedQk;

        private DerivedQuantityKind rootQk;
        private QuantityKindFactor rootQkFactor;

        private SiteReferenceDataLibrary srdl;
        private ModelReferenceDataLibrary mrdl;

        private NpgsqlTransaction npgsqlTransaction;
        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;
        private Mock<IQuantityKindFactorService> quantityKindFactorService;
        private Mock<IQuantityKindService> quantityKindService;

        private DerivedQuantityKindSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            // QuantityKind tree: rootQk -> specializedQk -> derivedQk -> simpleQk
            this.simpleQk = new SimpleQuantityKind(Guid.NewGuid(), 0);

            this.derivedQkFactor = new QuantityKindFactor(Guid.NewGuid(), 0)
            {
                QuantityKind = this.simpleQk.Iid
            };

            this.derivedQk = new DerivedQuantityKind(Guid.NewGuid(), 0)
            {
                QuantityKindFactor =
                {
                    new OrderedItem { K = 1, V = this.derivedQkFactor.Iid.ToString() }
                }
            };

            this.specializedQk = new SpecializedQuantityKind(Guid.NewGuid(), 0)
            {
                General = this.derivedQk.Iid
            };

            this.rootQkFactor = new QuantityKindFactor(Guid.NewGuid(), 0)
            {
                QuantityKind = this.specializedQk.Iid
            };

            this.rootQk = new DerivedQuantityKind(Guid.NewGuid(), 0)
            {
                QuantityKindFactor =
                {
                    new OrderedItem { K = 1, V = this.rootQkFactor.Iid.ToString() }
                }
            };

            // cyclic factor
            this.derivedQkCyclicFactor = new QuantityKindFactor(Guid.NewGuid(), 0)
            {
                QuantityKind = this.rootQk.Iid
            };

            // outside RDL factor
            this.outsideRdlQk = new SimpleQuantityKind(Guid.NewGuid(), 0);

            this.derivedQkOutsideRdlFactor = new QuantityKindFactor(Guid.NewGuid(), 0)
            {
                QuantityKind = this.outsideRdlQk.Iid
            };

            // RDL chain: mrdl -> srdl
            this.srdl = new SiteReferenceDataLibrary(Guid.NewGuid(), 0)
            {
                ParameterType =
                {
                    this.simpleQk.Iid
                }
            };

            this.mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), 0)
            {
                ParameterType =
                {
                    this.derivedQk.Iid,
                    this.specializedQk.Iid,
                    this.rootQk.Iid
                },
                RequiredRdl = this.srdl.Iid
            };

            // setup services
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();

            this.siteReferenceDataLibraryService
                .Setup(x => x.GetAsync(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    null,
                    It.IsAny<ISecurityContext>()))
                .ReturnsAsync(new List<ReferenceDataLibrary>
                {
                    this.srdl
                });

            this.quantityKindFactorService = new Mock<IQuantityKindFactorService>();

            this.quantityKindFactorService
                .Setup(x => x.GetAsync(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(
                    (transaction, partition, iids, context) =>
                    {
                        iids = iids.ToList();

                        return Task.FromResult(new List<Thing>
                        {
                            this.derivedQkFactor,
                            this.rootQkFactor,
                            this.derivedQkCyclicFactor,
                            this.derivedQkOutsideRdlFactor
                        }.Where(qkf => iids.Contains(qkf.Iid)));
                    });

            this.quantityKindService = new Mock<IQuantityKindService>();

            this.quantityKindService
                .Setup(x => x.GetAsync(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(
                    (transaction, partition, iids, context) =>
                    {
                        iids = iids.ToList();

                        return Task.FromResult<IEnumerable<Thing>>(new List<Thing>
                        {
                            this.simpleQk,
                            this.derivedQk,
                            this.specializedQk,
                            this.rootQk,
                            this.outsideRdlQk
                        }.Where(qk => iids.Contains(qk.Iid)));
                    });

            this.sideEffect = new DerivedQuantityKindSideEffect
            {
                SiteReferenceDataLibraryService = this.siteReferenceDataLibraryService.Object,
                QuantityKindFactorService = this.quantityKindFactorService.Object,
                QuantityKindService = this.quantityKindService.Object
            };
        }

        [Test]
        public void VerifyOutOfRdlChainError()
        {
            var rawUpdateInfo = new ClasslessDTO
            {
                { "QuantityKindFactor", new List<OrderedItem> { new() { K = 2, V = this.derivedQkOutsideRdlFactor.Iid } } }
            };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.derivedQk,
                    this.mrdl,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    rawUpdateInfo));
        }

        [Test]
        public void VerifyCircularDependencyError()
        {
            var rawUpdateInfo = new ClasslessDTO
            {
                { "QuantityKindFactor", new List<OrderedItem> { new() { K = 2, V = this.derivedQkCyclicFactor.Iid } } }
            };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.derivedQk,
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
                { "QuantityKindFactor", new List<OrderedItem> { new() { K = 2, V = this.derivedQkFactor.Iid } } }
            };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.rootQk,
                    this.mrdl,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    rawUpdateInfo));
        }
    }
}
