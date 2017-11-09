// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveServiceTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This is the Resolve Service test class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CDP4WebServices.API.Services;

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
        private IRequestUtils requestUtils;

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

            this.siteDirectoryInfo = new DtoInfo(typeof(SiteDirectory).Name, Guid.NewGuid());
            this.simpleQuantityKindInfo = new DtoInfo(typeof(SimpleQuantityKind).Name, Guid.NewGuid());
        
            this.engineeringModelInfo = new DtoInfo(typeof(EngineeringModel).Name, Guid.NewGuid());
            this.bookInfo = new DtoInfo(typeof(Book).Name, Guid.NewGuid());
        
            this.optionInfo = new DtoInfo(typeof(Option).Name, Guid.NewGuid());
            this.parameterInfo = new DtoInfo(typeof(Parameter).Name, Guid.NewGuid());
            this.aliasInfo = new DtoInfo(typeof(Alias).Name, Guid.NewGuid());
        }

        #region static partition resolvement tests
        [Test]
        public void VerifyStaticSiteDirectoryConceptPartitionResolvement()
        {
            var partitionString = SiteDirectoryPartition;
            var expectedPartition = partitionString;
            var unresolvedItems = new List<DtoResolveHelper>
                                      {
                                          new DtoResolveHelper(this.siteDirectoryInfo),
                                          new DtoResolveHelper(this.simpleQuantityKindInfo),
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
                                          new DtoResolveHelper(this.engineeringModelInfo),
                                          new DtoResolveHelper(this.bookInfo),
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
                                          new DtoResolveHelper(this.optionInfo),
                                          new DtoResolveHelper(this.parameterInfo),
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
                                          new DtoResolveHelper(this.aliasInfo)
                                      };

            this.resolveService.ResolvePartitionStatically(partitionString, unresolvedItems);

            Assert.That(
                unresolvedItems,
                Is.All.Matches(
                    (DtoResolveHelper x) => x.Partition == null));
        }
        #endregion

        #region partition resolvement from containment tree
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

            this.resolveService.ResolvePartitionFromContainmentTree(
                "BasePartition",
                new[] { item1ResolveHelper },
                resolvableInfo);

            Assert.AreEqual(item1ResolveHelper.Partition, item2ResolveHelper.Partition);
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

            this.resolveService.ResolvePartitionFromContainmentTree(
                basePartition,
                new[] { item1ResolveHelper, item2ResolveHelper },
                resolvableInfo);

            Assert.AreEqual(item1ResolveHelper.Partition, item2ResolveHelper.Partition);
            Assert.AreEqual(item2ResolveHelper.Partition, item3ResolveHelper.Partition);
        }
        #endregion
    }
}
