// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactoryTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Moq;

    using Npgsql;

    using NUnit.Framework;
    
    [TestFixture]
    public class DefaultValueArrayFactoryTestFixture
    {
        private Guid massIid;
        private Guid lengthIid;
        private Guid vectorIid;
        private Guid xIid;
        private Guid yIid;
        private Guid zIid;
        private Guid jaggedArrayIid;
        private Guid jaggedComponentOneIid;
        private Guid jaggedComponentTwoIid;
        private Guid incompleteCoundParameterTypeIid;
        private Guid invalidReferencedCompoundParameterTypeIid;
        private Guid invalidReferencedParameterTypeFromComponentIid;

        private Dictionary<Guid, ParameterType> parameterTypes;
        private Dictionary<Guid, ParameterTypeComponent> parameterTypeComponents;
        private Dictionary<Guid, IndependentParameterTypeAssignment> independentParameterTypeAssignments;
        private Dictionary<Guid, DependentParameterTypeAssignment> dependentParameterTypeAssignments;

        private Mock<ICachedReferenceDataService> cachedReferenceDataService;
        
        private Mock<ISecurityContext> secutrityContext;
        private NpgsqlTransaction transaction;

        private DefaultValueArrayFactory defaultValueArrayFactory;
        
        [SetUp]
        public void SetUp()
        {
            this.secutrityContext = new Mock<ISecurityContext>();
            this.transaction = null;
            
            this.PopulateParameterTypes();

            this.cachedReferenceDataService = new Mock<ICachedReferenceDataService>();
            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypes(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(this.parameterTypes);
            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypeComponents(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(this.parameterTypeComponents);
            this.cachedReferenceDataService.Setup(x => x.QueryDependentParameterTypeAssignments(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(this.dependentParameterTypeAssignments);
            this.cachedReferenceDataService.Setup(x => x.QueryIndependentParameterTypeAssignments(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(this.independentParameterTypeAssignments);

            this.defaultValueArrayFactory = new DefaultValueArrayFactory();
            this.defaultValueArrayFactory.CachedReferenceDataService = this.cachedReferenceDataService.Object;
        }

        private void PopulateParameterTypes()
        {
            this.parameterTypes = new Dictionary<Guid, ParameterType>();
            this.parameterTypeComponents = new Dictionary<Guid, ParameterTypeComponent>();
            this.independentParameterTypeAssignments = new Dictionary<Guid, IndependentParameterTypeAssignment>();
            this.dependentParameterTypeAssignments = new Dictionary<Guid, DependentParameterTypeAssignment>();

            this.massIid = Guid.NewGuid();
            this.lengthIid = Guid.NewGuid();
            this.vectorIid = Guid.NewGuid();
            this.xIid = Guid.NewGuid();
            this.yIid = Guid.NewGuid();
            this.zIid = Guid.NewGuid();
            this.jaggedArrayIid = Guid.NewGuid();
            this.jaggedComponentOneIid = Guid.NewGuid();
            this.jaggedComponentTwoIid = Guid.NewGuid();

            var mass = new SimpleQuantityKind(this.massIid, 0);
            var length = new SimpleQuantityKind(this.lengthIid, 0);

            // create a vector
            var x = new ParameterTypeComponent(this.xIid, 0) { ParameterType = length.Iid };
            var y = new ParameterTypeComponent(this.yIid, 0) { ParameterType = length.Iid };
            var z = new ParameterTypeComponent(this.zIid, 0) { ParameterType = length.Iid };

            var orderedItemX = new OrderedItem { K = 1, V = x.Iid };
            var orderedItemY = new OrderedItem { K = 2, V = y.Iid };
            var orderedItemZ = new OrderedItem { K = 3, V = z.Iid };
            
            var vector = new  CDP4Common.DTO.ArrayParameterType(this.vectorIid, 0);
            vector.Component.Add(orderedItemX);
            vector.Component.Add(orderedItemY);
            vector.Component.Add(orderedItemZ);
            this.parameterTypes.Add(mass.Iid, mass);
            this.parameterTypes.Add(length.Iid, length);
            this.parameterTypes.Add(vector.Iid, vector);

            this.parameterTypeComponents.Add(x.Iid, x);
            this.parameterTypeComponents.Add(y.Iid, y);
            this.parameterTypeComponents.Add(z.Iid, z);

            // create a jagged array            
            var jaggedComponentOne = new ParameterTypeComponent(this.jaggedComponentOneIid, 0) { ParameterType = length.Iid };
            var jaggedComponentTwo = new ParameterTypeComponent(this.jaggedComponentTwoIid, 0) { ParameterType = vector.Iid };
            
            var jaggedOrderedItemOne = new OrderedItem { K = 1, V = jaggedComponentOne.Iid };
            var jaggedOrderedItemTwo = new OrderedItem { K = 1, V = jaggedComponentTwo.Iid };

            var jaggedArray = new CDP4Common.DTO.CompoundParameterType(this.jaggedArrayIid, 0);
            jaggedArray.Component.Add(jaggedOrderedItemOne);
            jaggedArray.Component.Add(jaggedOrderedItemTwo);

            this.parameterTypes.Add(jaggedArray.Iid, jaggedArray);
            this.parameterTypeComponents.Add(jaggedComponentOne.Iid, jaggedComponentOne);
            this.parameterTypeComponents.Add(jaggedComponentTwo.Iid, jaggedComponentTwo);

            // incomplete compound
            this.incompleteCoundParameterTypeIid = Guid.NewGuid();
            var incompleteCoundParameterType = new CompoundParameterType(this.incompleteCoundParameterTypeIid, 0);
            this.parameterTypes.Add(incompleteCoundParameterType.Iid, incompleteCoundParameterType);

            // incorrect component referenced compound
            this.invalidReferencedCompoundParameterTypeIid = Guid.NewGuid();
            var invalidReferencedCompoundParameterType = new CompoundParameterType(this.invalidReferencedCompoundParameterTypeIid, 0);
            var invalidReferencedCompoundParameterTypeOrderedItemOne = new OrderedItem { K = 1, V = Guid.NewGuid() };
            invalidReferencedCompoundParameterType.Component.Add(invalidReferencedCompoundParameterTypeOrderedItemOne);
            this.parameterTypes.Add(invalidReferencedCompoundParameterType.Iid, invalidReferencedCompoundParameterType);

            // incorrect referenced parametertype from component
            this.invalidReferencedParameterTypeFromComponentIid = Guid.NewGuid();
            var invalidReferencedParameterTypeFromComponent = new CompoundParameterType(this.invalidReferencedParameterTypeFromComponentIid, 0);
            var invalidComponent = new ParameterTypeComponent(Guid.NewGuid(), 0) { ParameterType = Guid.NewGuid() };
            var invalidComponentItemOne = new OrderedItem { K = 1, V = invalidComponent.Iid };
            invalidReferencedParameterTypeFromComponent.Component.Add(invalidComponentItemOne);

            this.parameterTypes.Add(invalidReferencedParameterTypeFromComponent.Iid, invalidReferencedParameterTypeFromComponent);
            this.parameterTypeComponents.Add(invalidComponent.Iid, invalidComponent);
        }

        [Test]
        public void VerifyThatScalarParameterTypeReturnsExpectedValueArray()
        {
            var value = new List<string>() {"-"};
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.Load(transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypes(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponents(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.massIid);

            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.massIid);
            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);
        }

        [Test]
        public void VerifyThatArrayParameterTypeReturnsExpectedValueArray()
        {
            var value = new List<string>() { "-", "-", "-" };
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.Load(transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypes(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponents(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.vectorIid);

            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.vectorIid);
            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);
        }

        [Test]
        public void VerifyThatJaggedArrayReturnsExpectedValueArray()
        {
            var value = new List<string>() { "-", "-", "-", "-" };
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.Load(transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypes(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponents(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            
            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.jaggedArrayIid);

            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.jaggedArrayIid);
            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);
        }

        [Test]
        public void VerifyThatAComponentlessCompoundReturnsAnEmptyValueArray()
        {
            var value = new List<string>(0);
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.Load(transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypes(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponents(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignments(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.incompleteCoundParameterTypeIid);
            CollectionAssert.AreEquivalent(expectedValueArray, valueArray);
        }

        [Test]
        public void VerifyThatWhenParameterTypeIsUnknowExceptionIsThrown()
        {
            var randomIid = Guid.NewGuid();
            Assert.Throws<KeyNotFoundException>(() => this.defaultValueArrayFactory.CreateDefaultValueArray(randomIid));
        }

        [Test]
        public void VerifyThatWhenComponentIsUnknownExceptionIsThrown()
        {
            Assert.Throws<KeyNotFoundException>(() => this.defaultValueArrayFactory.CreateDefaultValueArray(this.invalidReferencedCompoundParameterTypeIid));
        }

        [Test]
        public void VerifyThatWhenComponentReferencesUnknownParameterTypeExceptionIsThrown()
        {
            Assert.Throws<KeyNotFoundException>(() => this.defaultValueArrayFactory.CreateDefaultValueArray(this.invalidReferencedParameterTypeFromComponentIid));
        }
    }
}