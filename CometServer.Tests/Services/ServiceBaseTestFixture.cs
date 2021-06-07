// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBaseTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Services;

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