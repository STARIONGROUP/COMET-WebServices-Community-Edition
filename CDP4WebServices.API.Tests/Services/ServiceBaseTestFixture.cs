// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBaseTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CDP4WebServices.API.Services;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ServiceBaseTestFixture"/>
    /// </summary>
    [TestFixture]
    public class ServiceBaseTestFixture : ServiceBase
    {
        ParameterTypeComponent parameterTypeComponent1 = new ParameterTypeComponent(
            Guid.Parse("7c59e9ff-f673-4543-bb8b-71f32c4b2f19"),
            1);

        ParameterTypeComponent parameterTypeComponent2 = new ParameterTypeComponent(
            Guid.Parse("fa9c0bc9-5172-483f-8ecf-6ee8b825b57f"),
            1);

        ParameterTypeComponent parameterTypeComponent3 = new ParameterTypeComponent(
            Guid.Parse("88bda1de-0b5e-4094-af0c-c51ccf15bca6"),
            1);

        [SetUp]
        public void Setup()
        {
            this.RequestUtils = new RequestUtils();
            this.RequestUtils.Cache.Add(parameterTypeComponent1);
            this.RequestUtils.Cache.Add(parameterTypeComponent2);
            this.RequestUtils.Cache.Add(parameterTypeComponent3);
        }

        [Test]
        public void VerifyThatCorrectIEnumerableIsReturnedWhenResolveFromRequestCacheIsCalled()
        {
            var list = new List<OrderedItem>
                           {
                               new OrderedItem
                                   {
                                       K = 1,
                                       V = Guid.Parse(
                                           "7c59e9ff-f673-4543-bb8b-71f32c4b2f19")
                                   },
                               new OrderedItem
                                   {
                                       K = 2,
                                       V = Guid.Parse(
                                           "fa9c0bc9-5172-483f-8ecf-6ee8b825b57f")
                                   },
                               new OrderedItem
                                   {
                                       K = 3,
                                       V = Guid.Parse(
                                           "88bda1de-0b5e-4094-af0c-c51ccf15bca6")
                                   }
                           };

            var resolvedList = this.ResolveFromRequestCache(list);

            var outputResolvedList = new List<OrderedItem>
                                         {
                                             new OrderedItem { K = 3, V = parameterTypeComponent3 },
                                             new OrderedItem { K = 2, V = parameterTypeComponent2 },
                                             new OrderedItem { K = 1, V = parameterTypeComponent1 }
                                         };

            CollectionAssert.AreEquivalent(resolvedList, outputResolvedList);
        }
    }
}