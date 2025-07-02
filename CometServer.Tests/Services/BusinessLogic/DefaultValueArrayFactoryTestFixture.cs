// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactoryTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CometServer.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

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

        private Mock<ILogger<DefaultValueArrayFactory>> logger = new ();
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
            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypesAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(this.parameterTypes));
            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypeComponentsAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(this.parameterTypeComponents));
            this.cachedReferenceDataService.Setup(x => x.QueryDependentParameterTypeAssignmentsAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(this.dependentParameterTypeAssignments));
            this.cachedReferenceDataService.Setup(x => x.QueryIndependentParameterTypeAssignmentsAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(this.independentParameterTypeAssignments));

            this.defaultValueArrayFactory = new DefaultValueArrayFactory();
            this.defaultValueArrayFactory.Logger = this.logger.Object;
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
            
            var vector = new  ArrayParameterType(this.vectorIid, 0);
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

            var jaggedArray = new CompoundParameterType(this.jaggedArrayIid, 0);
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

            this.defaultValueArrayFactory.LoadAsync(this.transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypesAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.massIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.massIid);
            
            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));
        }

        [Test]
        public void VerifyThatArrayParameterTypeReturnsExpectedValueArray()
        {
            var value = new List<string>() { "-", "-", "-" };
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.LoadAsync(this.transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypesAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.vectorIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.vectorIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));
        }

        [Test]
        public void VerifyThatJaggedArrayReturnsExpectedValueArray()
        {
            var value = new List<string>() { "-", "-", "-", "-" };
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.LoadAsync(this.transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypesAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            
            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.jaggedArrayIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));

            valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.jaggedArrayIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));
        }

        [Test]
        public void VerifyThatAComponentlessCompoundReturnsAnEmptyValueArray()
        {
            var value = new List<string>(0);
            var expectedValueArray = new ValueArray<string>(value);

            this.defaultValueArrayFactory.LoadAsync(this.transaction, this.secutrityContext.Object);

            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypesAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryParameterTypeComponentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryDependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));
            this.cachedReferenceDataService.Verify(x => x.QueryIndependentParameterTypeAssignmentsAsync(this.transaction, this.secutrityContext.Object), Times.Exactly(1));

            var valueArray = this.defaultValueArrayFactory.CreateDefaultValueArray(this.incompleteCoundParameterTypeIid);

            Assert.That(valueArray, Is.EquivalentTo(expectedValueArray));
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