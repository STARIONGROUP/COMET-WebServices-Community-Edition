// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecializedQuantityKindSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="SpecializedQuantityKindSideEffect"/>
    /// </summary>
    [TestFixture]
    public class SpecializedQuantityKindSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<ISpecializedQuantityKindService> specializedQuantityKindService;

        private SpecializedQuantityKindSideEffect sideEffect;

        private SpecializedQuantityKind specializedQuantityKindA;

        private SpecializedQuantityKind specializedQuantityKindB;

        private SpecializedQuantityKind specializedQuantityKindC;

        private SpecializedQuantityKind specializedQuantityKindD;

        private SpecializedQuantityKind specializedQuantityKindE;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "General";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain a -> b -> c 
            this.specializedQuantityKindC = new SpecializedQuantityKind { Iid = Guid.NewGuid() };
            this.specializedQuantityKindB =
                new SpecializedQuantityKind { Iid = Guid.NewGuid(), General = this.specializedQuantityKindC.Iid };
            this.specializedQuantityKindA =
                new SpecializedQuantityKind { Iid = Guid.NewGuid(), General = this.specializedQuantityKindB.Iid };
            this.specializedQuantityKindD = new SpecializedQuantityKind { Iid = Guid.NewGuid() };

            // Outside the rdl chain
            this.specializedQuantityKindE = new SpecializedQuantityKind { Iid = Guid.NewGuid() };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB = new SiteReferenceDataLibrary
            {
                Iid = Guid.NewGuid(),
                ParameterType =
                {
                    this.specializedQuantityKindD.Iid
                }
            };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
            {
                Iid = Guid.NewGuid(),
                ParameterType =
                {
                    this.specializedQuantityKindA.Iid,
                    this.specializedQuantityKindB.Iid,
                    this.specializedQuantityKindC.Iid
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

            this.specializedQuantityKindService = new Mock<ISpecializedQuantityKindService>();
            this.specializedQuantityKindService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.specializedQuantityKindD.Iid,
                                this.specializedQuantityKindA.Iid,
                                this.specializedQuantityKindB.Iid,
                                this.specializedQuantityKindC.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<SpecializedQuantityKind>
                        {
                            this.specializedQuantityKindD,
                            this.specializedQuantityKindA,
                            this.specializedQuantityKindB,
                            this.specializedQuantityKindC
                        });

            this.sideEffect = new SpecializedQuantityKindSideEffect
            {
                SpecializedQuantityKindService = this.specializedQuantityKindService.Object,
                SiteReferenceDataLibraryService = this.siteReferenceDataLibraryService.Object
            };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenGeneralReferenceIsKindItself()
        {
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.specializedQuantityKindA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.specializedQuantityKindA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenGeneralReferenceIsOutOfChain()
        {
            // Out of chain
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.specializedQuantityKindE.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.specializedQuantityKindC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenGeneralReferenceLeadsToCircularDependency()
        {
            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.specializedQuantityKindA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.specializedQuantityKindC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenGeneralReferenceDoesNotLeadToCircularDependency()
        {
            // There is a chain a -> b -> c
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, this.specializedQuantityKindD.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.specializedQuantityKindC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}
