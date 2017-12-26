// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeComponentSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

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
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenParameterTypeComponentUpdateLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.compoundParameterTypeC.Iid } };

            Assert.Throws<ArgumentException>(
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
            this.rawUpdateInfo = new ClasslessDTO(null) { { TestKey, this.booleanParameterTypeE.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.parameterTypeComponentB,
                    this.compoundParameterTypeB,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}