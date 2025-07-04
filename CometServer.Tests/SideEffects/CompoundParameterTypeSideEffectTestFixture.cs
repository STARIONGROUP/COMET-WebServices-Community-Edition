// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompoundParameterTypeSideEffectTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="CompoundParameterTypeSideEffect"/>
    /// </summary>
    [TestFixture]
    public class CompoundParameterTypeSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<ICompoundParameterTypeService> compoundParameterTypeService;

        private Mock<IArrayParameterTypeService> arrayParameterTypeService;

        private Mock<IParameterTypeComponentService> parameterTypeComponentService;

        private CompoundParameterTypeSideEffect sideEffect;

        private CompoundParameterType compoundParameterTypeA;

        private CompoundParameterType compoundParameterTypeB;

        private BooleanParameterType booleanParameterTypeD;

        private ParameterTypeComponent parameterTypeComponentA;

        private ParameterTypeComponent parameterTypeComponentB;

        private ParameterTypeComponent parameterTypeComponentC;

        private ParameterTypeComponent parameterTypeComponentD;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "Component";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain cptA -> ptcA -> cptB -> ptcB -> bptD
            this.booleanParameterTypeD = new BooleanParameterType { Iid = Guid.NewGuid() };
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

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB =
                new SiteReferenceDataLibrary { Iid = Guid.NewGuid(), ParameterType = { this.booleanParameterTypeD.Iid } };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 ParameterType =
                                                     {
                                                         this.compoundParameterTypeA.Iid,
                                                         this.compoundParameterTypeB.Iid
                                                     },
                                                 RequiredRdl = this.referenceDataLibraryB.Iid
                                             };

            this.siteReferenceDataLibraryService = new Mock<ISiteReferenceDataLibraryService>();
            this.siteReferenceDataLibraryService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .ReturnsAsync(new List<ReferenceDataLibrary> { this.referenceDataLibraryB });

            this.compoundParameterTypeService = new Mock<ICompoundParameterTypeService>();
            this.compoundParameterTypeService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.booleanParameterTypeD.Iid,
                                this.compoundParameterTypeA.Iid,
                                this.compoundParameterTypeB.Iid
                            },
                        It.IsAny<ISecurityContext>())).ReturnsAsync(
                    new List<CompoundParameterType> { this.compoundParameterTypeA, this.compoundParameterTypeB });

            this.arrayParameterTypeService = new Mock<IArrayParameterTypeService>();
            this.arrayParameterTypeService.Setup(
                x => x.GetAsync(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    new List<Guid>
                        {
                            this.booleanParameterTypeD.Iid,
                            this.compoundParameterTypeA.Iid,
                            this.compoundParameterTypeB.Iid
                        },
                    It.IsAny<ISecurityContext>())).ReturnsAsync(new List<ArrayParameterType>());

            this.parameterTypeComponentService = new Mock<IParameterTypeComponentService>();
            this.parameterTypeComponentService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentA.Iid },
                        It.IsAny<ISecurityContext>())).ReturnsAsync(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentA });
            this.parameterTypeComponentService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentB.Iid },
                        It.IsAny<ISecurityContext>())).ReturnsAsync(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentB });
            this.parameterTypeComponentService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentC.Iid },
                        It.IsAny<ISecurityContext>())).ReturnsAsync(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentC });
            this.parameterTypeComponentService
                .Setup(
                    x => x.GetAsync(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.parameterTypeComponentD.Iid },
                        It.IsAny<ISecurityContext>())).ReturnsAsync(
                    new List<ParameterTypeComponent> { this.parameterTypeComponentD });

            this.sideEffect = new CompoundParameterTypeSideEffect()
                                  {
                                      CompoundParameterTypeService =
                                          this.compoundParameterTypeService.Object,
                                      ArrayParameterTypeService =
                                          this.arrayParameterTypeService.Object,
                                      ParameterTypeComponentService =
                                          this.parameterTypeComponentService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenParameterTypeComponentLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<OrderedItem>
                                                 {
                                                     new()
                                                     {
                                                             K = 3,
                                                             V = this
                                                                 .parameterTypeComponentC
                                                                 .Iid
                                                         }
                                                 }
                                         }
                                     };

            Assert.ThrowsAsync<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.compoundParameterTypeB,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenParameterTypeComponentDoesNotLeadToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         {
                                             TestKey,
                                             new List<OrderedItem>
                                                 {
                                                     new()
                                                     {
                                                             K = 4,
                                                             V = this
                                                                 .parameterTypeComponentD
                                                                 .Iid
                                                         }
                                                 }
                                         }
                                     };

            Assert.DoesNotThrowAsync(
                () => this.sideEffect.BeforeUpdateAsync(
                    this.compoundParameterTypeB,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}