// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompoundParameterTypeSideEffectTestFixture.cs" company="RHEA System S.A.">
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

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

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

        private CompoundParameterType compoundParameterTypeC;

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
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new List<ReferenceDataLibrary> { this.referenceDataLibraryB });

            this.compoundParameterTypeService = new Mock<ICompoundParameterTypeService>();
            this.compoundParameterTypeService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.booleanParameterTypeD.Iid,
                                this.compoundParameterTypeA.Iid,
                                this.compoundParameterTypeB.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<CompoundParameterType> { this.compoundParameterTypeA, this.compoundParameterTypeB });

            this.arrayParameterTypeService = new Mock<IArrayParameterTypeService>();
            this.arrayParameterTypeService.Setup(
                x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    new List<Guid>
                        {
                            this.booleanParameterTypeD.Iid,
                            this.compoundParameterTypeA.Iid,
                            this.compoundParameterTypeB.Iid
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
            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         {
                                             TestKey,
                                             new List<OrderedItem>
                                                 {
                                                     new OrderedItem
                                                         {
                                                             K = 3,
                                                             V = this
                                                                 .parameterTypeComponentC
                                                                 .Iid
                                                         }
                                                 }
                                         }
                                     };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
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
            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         {
                                             TestKey,
                                             new List<OrderedItem>
                                                 {
                                                     new OrderedItem
                                                         {
                                                             K = 4,
                                                             V = this
                                                                 .parameterTypeComponentD
                                                                 .Iid
                                                         }
                                                 }
                                         }
                                     };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.compoundParameterTypeB,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}