﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveServiceTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.Services;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="ResolveService"/> class
    /// </summary>
    [TestFixture]
    public class ResolveServiceTestFixture
    {
        /// <summary>
        /// The site directory partition.
        /// </summary>
        private const string SiteDirectoryPartition = "SiteDirectory";

        /// <summary>
        /// The EngineeringModel partition.
        /// </summary>
        private const string EngineeringModelPartition = "EngineeringModel";

        /// <summary>
        /// The iteration partition.
        /// </summary>
        private const string IterationPartition = "Iteration";

        private ResolveService resolveService;
        private IDataModelUtils dataModelUtils;
        private RequestUtils requestUtils;

        // SiteDirectory concepts
        private DtoInfo siteDirectoryInfo;
        private DtoInfo simpleQuantityKindInfo; 

        // EngineeringModel concepts
        private DtoInfo engineeringModelInfo;
        private DtoInfo bookInfo;
                
        // Iteration concepts
        private DtoInfo optionInfo;
        private DtoInfo parameterInfo;

        private DtoInfo aliasInfo;

        private Guid engineeringModelIid;

        [SetUp]
        public void TestSetup()
        {
            this.resolveService = new ResolveService();
            this.dataModelUtils = new DataModelUtils();
            this.requestUtils = new RequestUtils();

            this.resolveService.DataModelUtils = this.dataModelUtils;

            this.engineeringModelIid = Guid.NewGuid();

            this.siteDirectoryInfo = new DtoInfo(nameof(SiteDirectory), Guid.NewGuid());
            this.simpleQuantityKindInfo = new DtoInfo(nameof(SimpleQuantityKind), Guid.NewGuid());
        
            this.engineeringModelInfo = new DtoInfo(nameof(EngineeringModel), Guid.NewGuid());
            this.bookInfo = new DtoInfo(nameof(Book), Guid.NewGuid());
        
            this.optionInfo = new DtoInfo(nameof(Option), Guid.NewGuid());
            this.parameterInfo = new DtoInfo(nameof(Parameter), Guid.NewGuid());
            this.aliasInfo = new DtoInfo(nameof(Alias), Guid.NewGuid());
        }

        [Test]
        public void VerifyStaticSiteDirectoryConceptPartitionResolvement()
        {
            var partitionString = SiteDirectoryPartition;
            var expectedPartition = partitionString;
            var unresolvedItems = new List<DtoResolveHelper>
                                      {
                                          new(this.siteDirectoryInfo),
                                          new(this.simpleQuantityKindInfo),
                                      };

            this.resolveService.ResolvePartitionStatically(partitionString, unresolvedItems);

            Assert.That(
                unresolvedItems,
                Is.All.Matches((DtoResolveHelper x) => x.Partition != null && x.Partition == expectedPartition));
        }
        
        [Test]
        public void VerifyStaticEngineeringModelConceptPartitionResolvement()
        {
            var partitionString = this.requestUtils.GetEngineeringModelPartitionString(this.engineeringModelIid);
            var expectedPartition = partitionString;

            var unresolvedItems = new List<DtoResolveHelper>
                                      {
                                          new(this.engineeringModelInfo),
                                          new(this.bookInfo),
                                      };

            this.resolveService.ResolvePartitionStatically(partitionString, unresolvedItems);

            Assert.That(
                unresolvedItems,
                Is.All.Matches(
                    (DtoResolveHelper x) => x.Partition != null && x.Partition == expectedPartition));
        }

        [Test]
        public void VerifyStaticIterationConceptPartitionResolvement()
        {
            var partitionString = this.requestUtils.GetEngineeringModelPartitionString(this.engineeringModelIid);
            var expectedPartition = partitionString.Replace(EngineeringModelPartition, IterationPartition);

            var unresolvedItems = new List<DtoResolveHelper>
                                      {
                                          new(this.optionInfo),
                                          new(this.parameterInfo),
                                      };

            this.resolveService.ResolvePartitionStatically(partitionString, unresolvedItems);

            Assert.That(
                unresolvedItems,
                Is.All.Matches(
                    (DtoResolveHelper x) => x.Partition != null && x.Partition == expectedPartition));
        }

        [Test]
        public void VerifyStaticIterationConceptPartitionNotFound()
        {
            var partitionString = this.requestUtils.GetEngineeringModelPartitionString(this.engineeringModelIid);

            var unresolvedItems = new List<DtoResolveHelper>
                                      {
                                          new(this.aliasInfo)
                                      };

            this.resolveService.ResolvePartitionStatically(partitionString, unresolvedItems);

            Assert.That(
                unresolvedItems,
                Is.All.Matches(
                    (DtoResolveHelper x) => x.Partition == null));
        }

        [Test]
        public void VerifyPartitionResolvementFromDirectParentContainment()
        {
            var item1 = new ContainerInfo("SomeType", Guid.NewGuid());
            var item2 = new ContainerInfo("SomeOtherType", Guid.NewGuid());

            var item1ResolveHelper = new DtoResolveHelper(item1) { ContainerInfo = item2 };
            var item2ResolveHelper = new DtoResolveHelper(item2) { Partition = "SomePartition" };
            var resolvableInfo = new Dictionary<DtoInfo, DtoResolveHelper>
                                     {
                                         { item1, item1ResolveHelper },
                                         { item2, item2ResolveHelper }
                                     };

            ResolveService.ResolvePartitionFromContainmentTree(
                "BasePartition",
                new[] { item1ResolveHelper },
                resolvableInfo);

            Assert.That(item2ResolveHelper.Partition, Is.EqualTo(item1ResolveHelper.Partition));
        }

        [Test]
        public void VerifyPartitionResolvementFromTopContainer()
        {
            var basePartition = "BasePartition";
            var item1 = new ContainerInfo("SomeType", Guid.NewGuid());
            var item2 = new ContainerInfo("SomeOtherType", Guid.NewGuid());
            var item3 = new ContainerInfo("SomeThirdType", Guid.NewGuid());

            var item1ResolveHelper = new DtoResolveHelper(item1) { ContainerInfo = item2 };
            var item2ResolveHelper = new DtoResolveHelper(item2) { ContainerInfo = item3 };
            var item3ResolveHelper = new DtoResolveHelper(item3) { Partition = basePartition };
            var resolvableInfo = new Dictionary<DtoInfo, DtoResolveHelper>
                                     {
                                         { item1, item1ResolveHelper },
                                         { item2, item2ResolveHelper },
                                         { item3, item3ResolveHelper }
                                     };

            ResolveService.ResolvePartitionFromContainmentTree(
                basePartition,
                new[] { item1ResolveHelper, item2ResolveHelper },
                resolvableInfo);

            Assert.That(item2ResolveHelper.Partition, Is.EqualTo(item1ResolveHelper.Partition));
            Assert.That(item3ResolveHelper.Partition, Is.EqualTo(item2ResolveHelper.Partition));
        }
    }
}
