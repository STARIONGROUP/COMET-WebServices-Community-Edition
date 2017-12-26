// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategorySideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="CategorySideEffect"/>
    /// </summary>
    [TestFixture]
    public class CategorySideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService;

        private Mock<ICategoryService> categoryService;

        private CategorySideEffect sideEffect;

        private Category categoryA;

        private Category categoryB;

        private Category categoryC;

        private Category categoryD;

        private Category categoryE;

        private Category categoryF;

        private Category categoryG;

        private ReferenceDataLibrary referenceDataLibraryA;

        private ReferenceDataLibrary referenceDataLibraryB;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "SuperCategory";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            // There is a chain d -> e -> f and e -> g -> c
            this.categoryA = new Category { Iid = Guid.NewGuid() };
            this.categoryB = new Category { Iid = Guid.NewGuid() };
            this.categoryC = new Category { Iid = Guid.NewGuid() };
            this.categoryG = new Category { Iid = Guid.NewGuid(), SuperCategory = { this.categoryC.Iid } };
            this.categoryF = new Category { Iid = Guid.NewGuid() };
            this.categoryE = new Category
                                 {
                                     Iid = Guid.NewGuid(),
                                     SuperCategory = {
                                                        this.categoryF.Iid, this.categoryG.Iid 
                                                     }
                                 };
            this.categoryD = new Category { Iid = Guid.NewGuid(), SuperCategory = { this.categoryE.Iid } };

            // There is a chain librayA -> LibraryB
            this.referenceDataLibraryB = new SiteReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 DefinedCategory =
                                                     {
                                                         this.categoryC.Iid,
                                                         this.categoryD.Iid,
                                                         this.categoryE.Iid,
                                                         this.categoryF.Iid,
                                                         this.categoryG.Iid
                                                     }
                                             };
            this.referenceDataLibraryA = new ModelReferenceDataLibrary
                                             {
                                                 Iid = Guid.NewGuid(),
                                                 DefinedCategory = {
                                                                      this.categoryA.Iid 
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

            this.categoryService = new Mock<ICategoryService>();
            this.categoryService
                .Setup(
                    x => x.Get(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid>
                            {
                                this.categoryC.Iid,
                                this.categoryD.Iid,
                                this.categoryE.Iid,
                                this.categoryF.Iid,
                                this.categoryG.Iid,
                                this.categoryA.Iid
                            },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<Category>
                        {
                            this.categoryC,
                            this.categoryD,
                            this.categoryE,
                            this.categoryF,
                            this.categoryG,
                            this.categoryA
                        });
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenSuperCategoryIsCategoryItself()
        {
            this.sideEffect = new CategorySideEffect()
                                  {
                                      CategoryService = this.categoryService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         { TestKey, new List<Guid> { this.categoryA.Iid } }
                                     };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.categoryA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenSuperCategoryIsOutOfChainOrLeadsToCircularDependency()
        {
            this.sideEffect = new CategorySideEffect()
                                  {
                                      CategoryService = this.categoryService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // Out of chain
            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         { TestKey, new List<Guid> { this.categoryB.Iid } }
                                     };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.categoryA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         { TestKey, new List<Guid> { this.categoryD.Iid } }
                                     };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(
                    this.categoryC,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenCategoryHasRepeatedSuperCategoriesWithoutCircularDependency()
        {
            this.sideEffect = new CategorySideEffect()
                                  {
                                      CategoryService = this.categoryService.Object,
                                      SiteReferenceDataLibraryService =
                                          this.siteReferenceDataLibraryService.Object
                                  };

            // There is a chain d -> e -> f and e -> g -> c
            // category will reference a -> d and -> g which means that it references g twice, but without circular dependency
            this.rawUpdateInfo = new ClasslessDTO(null)
                                     {
                                         { TestKey, new List<Guid> { this.categoryD.Iid, this.categoryG.Iid } }
                                     };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.categoryA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}