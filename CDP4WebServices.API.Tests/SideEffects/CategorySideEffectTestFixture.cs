// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategorySideEffectTestFixture.cs" company="RHEA System S.A.">
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

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

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

            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         { TestKey, new List<Guid> { this.categoryA.Iid } }
                                     };

            Assert.Throws<AcyclicValidationException>(
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
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         { TestKey, new List<Guid> { this.categoryB.Iid } }
                                     };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.categoryA,
                    this.referenceDataLibraryA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO()
                                     {
                                         { TestKey, new List<Guid> { this.categoryD.Iid } }
                                     };

            Assert.Throws<AcyclicValidationException>(
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
            this.rawUpdateInfo = new ClasslessDTO()
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