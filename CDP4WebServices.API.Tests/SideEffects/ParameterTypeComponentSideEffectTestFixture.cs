// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeComponentSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterTypeComponentSideEffect"/>
    /// </summary>
    [TestFixture]
    public class ParameterTypeComponentSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<IModelReferenceDataLibraryService> modelReferenceDataLibraryService;

        private Mock<ICompoundParameterTypeService> compoundParameterTypeService;

        private Mock<IArrayParameterTypeService> arrayParameterTypeService;

        private Mock<IParameterTypeComponentService> parameterTypeComponentService;

        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;

        private ParameterTypeComponentSideEffect sideEffect;

        private CompoundParameterType compoundParameterTypeA;

        private CompoundParameterType compoundParameterTypeB;

        private CompoundParameterType compoundParameterTypeC;

        private BooleanParameterType booleanParameterTypeD;

        private BooleanParameterType booleanParameterTypeE;

        private ParameterTypeComponent parameterTypeComponentA;

        private ParameterTypeComponent parameterTypeComponentB;

        private ParameterTypeComponent parameterTypeComponentC;

        private ParameterTypeComponent parameterTypeComponentD;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "ParameterType";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();

            // There is a chain cptA -> ptcA -> cptB -> ptcB -> bptD
            this.booleanParameterTypeD = new BooleanParameterType { Iid = Guid.NewGuid() };
            this.booleanParameterTypeE = new BooleanParameterType { Iid = Guid.NewGuid() };
            this.parameterTypeComponentD =
                new ParameterTypeComponent { Iid = Guid.NewGuid(), ParameterType = this.booleanParameterTypeD.Iid };
            this.parameterTypeComponentB =
                new ParameterTypeComponent { Iid = Guid.NewGuid(), ParameterType = this.booleanParameterTypeD.Iid };

            var parameterTypeComponentForB = new OrderedItem { K = 1, V = this.parameterTypeComponentB.Iid };
            var parameterTypeComponentsForB = new List<OrderedItem> { parameterTypeComponentForB };

            this.compoundParameterTypeB =
                new CompoundParameterType { Iid = Guid.NewGuid(), Component = parameterTypeComponentsForB };

            this.parameterTypeComponentA =
                new ParameterTypeComponent { Iid = Guid.NewGuid(), ParameterType = this.compoundParameterTypeB.Iid };

            var parameterTypeComponentForA = new OrderedItem { K = 1, V = this.parameterTypeComponentA.Iid };
            var parameterTypeComponentsForA = new List<OrderedItem> { parameterTypeComponentForA };

            this.compoundParameterTypeA =
                new CompoundParameterType { Iid = Guid.NewGuid(), Component = parameterTypeComponentsForA };

            this.parameterTypeComponentC =
                new ParameterTypeComponent { Iid = Guid.NewGuid(), ParameterType = this.compoundParameterTypeA.Iid };

            var parameterTypeComponentForC = new OrderedItem { K = 1, V = this.parameterTypeComponentC.Iid };
            var parameterTypeComponentsForC = new List<OrderedItem> { parameterTypeComponentForC };
            this.compoundParameterTypeC =
                new CompoundParameterType { Iid = Guid.NewGuid(), Component = parameterTypeComponentsForC };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB = new SiteReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 ParameterType =
                                                     {
                                                         this.booleanParameterTypeD
                                                             .Iid,
                                                         this.booleanParameterTypeE
                                                             .Iid
                                                     }
                                             };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 ParameterType =
                                                     {
                                                         this
                                                             .compoundParameterTypeA
                                                             .Iid,
                                                         this
                                                             .compoundParameterTypeB
                                                             .Iid,
                                                         this
                                                             .compoundParameterTypeC
                                                             .Iid
                                                     },
                                                 RequiredRdl = this.referenceDataLibraryB.Iid
                                             };

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary> { this.referenceDataLibraryA, this.referenceDataLibraryB });

            this.modelReferenceDataLibraryService = new Mock<IModelReferenceDataLibraryService>();
            this.modelReferenceDataLibraryService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary>());

            this.compoundParameterTypeService = new Mock<ICompoundParameterTypeService>();
            this.compoundParameterTypeService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.compoundParameterTypeA.Iid,
                                this.compoundParameterTypeB.Iid,
                                this.compoundParameterTypeC.Iid,
                                this.booleanParameterTypeD.Iid,
                                this.booleanParameterTypeE.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<CompoundParameterType>
                        {
                            this.compoundParameterTypeA,
                            this.compoundParameterTypeB,
                            this.compoundParameterTypeC
                        });

            this.arrayParameterTypeService = new Mock<IArrayParameterTypeService>();
            this.arrayParameterTypeService.Setup(
                x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    new List<Guid>
                        {
                            this.compoundParameterTypeA.Iid,
                            this.compoundParameterTypeB.Iid,
                            this.compoundParameterTypeC.Iid,
                            this.booleanParameterTypeD.Iid,
                            this.booleanParameterTypeE.Iid
                        },
                    It.IsAny<ISecurityContext>())).Returns(new List<ArrayParameterType>());

            this.parameterTypeComponentService = new Mock<IParameterTypeComponentService>();
            this.parameterTypeComponentService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentA.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentA });
            this.parameterTypeComponentService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentB.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentB });
            this.parameterTypeComponentService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentC.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentC });
            this.parameterTypeComponentService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentD.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentD });

            this.sideEffect = new ParameterTypeComponentSideEffect()
                                  {
                                      CompoundParameterTypeService =
                                          this.compoundParameterTypeService.Object,
                                      ArrayParameterTypeService =
                                          this.arrayParameterTypeService.Object,
                                      ParameterTypeComponentService =
                                          this.parameterTypeComponentService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService
                                              .Object,
                                      ModelReferenceDataLibraryService =
                                          this.modelReferenceDataLibraryService
                                              .Object
                                  };

            this.sideEffect.DefaultValueArrayFactory = this.defaultValueArrayFactory.Object;
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenParameterTypeComponentUpdateLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.compoundParameterTypeC.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterTypeComponentB,
                    this.compoundParameterTypeB,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenParameterTypeComponentUpdateDoesNotLeadToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.booleanParameterTypeE.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterTypeComponentB,
                    this.compoundParameterTypeB,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void Verify_that_upon_AfterCreate_DefaultValueArrayFactory_is_reset()
        {
            this.sideEffect.AfterCreate(It.IsAny<ParameterTypeComponent>(), It.IsAny<Thing>(), It.IsAny<ParameterTypeComponent>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterDelete_DefaultValueArrayFactory_is_reset()
        {
            this.sideEffect.AfterDelete(It.IsAny<ParameterTypeComponent>(), It.IsAny<Thing>(), It.IsAny<ParameterTypeComponent>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterUpdate_DefaultValueArrayFactory_is_reset()
        {
            this.sideEffect.AfterUpdate(It.IsAny<ParameterTypeComponent>(), It.IsAny<Thing>(), It.IsAny<ParameterTypeComponent>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }
    }
}