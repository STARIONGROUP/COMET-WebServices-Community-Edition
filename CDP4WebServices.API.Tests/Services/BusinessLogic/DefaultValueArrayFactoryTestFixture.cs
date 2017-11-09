// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactoryTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services;
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

        private DefaultValueArrayFactory defaultValueArrayFactory;

        private List<ParameterType> parameterTypes;

        private List<ParameterTypeComponent> parameterTypeComponents;

        [SetUp]
        public void SetUp()
        {
            this.PopulateParameterTypes();
            
            this.defaultValueArrayFactory = new DefaultValueArrayFactory();
            this.defaultValueArrayFactory.Initialize(this.parameterTypes, this.parameterTypeComponents);
        }

        private void PopulateParameterTypes()
        {
            this.parameterTypes = new List<ParameterType>();
            this.parameterTypeComponents = new List<ParameterTypeComponent>();

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
            this.parameterTypes.Add(mass);
            this.parameterTypes.Add(length);
            this.parameterTypes.Add(vector);

            this.parameterTypeComponents.Add(x);
            this.parameterTypeComponents.Add(y);
            this.parameterTypeComponents.Add(z);

            // create a jagged array            
            var jaggedComponentOne = new ParameterTypeComponent(this.jaggedComponentOneIid, 0) { ParameterType = length.Iid };
            var jaggedComponentTwo = new ParameterTypeComponent(this.jaggedComponentTwoIid, 0) { ParameterType = vector.Iid };
            
            var jaggedOrderedItemOne = new OrderedItem { K = 1, V = jaggedComponentOne.Iid };
            var jaggedOrderedItemTwo = new OrderedItem { K = 1, V = jaggedComponentTwo.Iid };

            var jaggedArray = new CDP4Common.DTO.CompoundParameterType(this.jaggedArrayIid, 0);
            jaggedArray.Component.Add(jaggedOrderedItemOne);
            jaggedArray.Component.Add(jaggedOrderedItemTwo);

            this.parameterTypes.Add(jaggedArray);
            this.parameterTypeComponents.Add(jaggedComponentOne);
            this.parameterTypeComponents.Add(jaggedComponentTwo);

            // incomplete compound
            this.incompleteCoundParameterTypeIid = Guid.NewGuid();
            var incompleteCoundParameterType = new CompoundParameterType(this.incompleteCoundParameterTypeIid, 0);
            this.parameterTypes.Add(incompleteCoundParameterType);

            // incorrect component referenced compound
            this.invalidReferencedCompoundParameterTypeIid = Guid.NewGuid();
            var invalidReferencedCompoundParameterType = new CompoundParameterType(this.invalidReferencedCompoundParameterTypeIid, 0);
            var invalidReferencedCompoundParameterTypeOrderedItemOne = new OrderedItem { K = 1, V = Guid.NewGuid() };
            invalidReferencedCompoundParameterType.Component.Add(invalidReferencedCompoundParameterTypeOrderedItemOne);
            this.parameterTypes.Add(invalidReferencedCompoundParameterType);

            // incorrect referenced parametertype from component
            this.invalidReferencedParameterTypeFromComponentIid = Guid.NewGuid();
            var invalidReferencedParameterTypeFromComponent = new CompoundParameterType(this.invalidReferencedParameterTypeFromComponentIid, 0);
            var invalidComponent = new ParameterTypeComponent(Guid.NewGuid(), 0) { ParameterType = Guid.NewGuid() };
            var invalidComponentItemOne = new OrderedItem { K = 1, V = invalidComponent.Iid };
            invalidReferencedParameterTypeFromComponent.Component.Add(invalidComponentItemOne);

            this.parameterTypes.Add(invalidReferencedParameterTypeFromComponent);
            this.parameterTypeComponents.Add(invalidComponent);
        }

        [Test]
        public void VerifyThatScalarParameterTypeReturnsExpectedValueArray()
        {
            var value = new List<string>() {"-"};
            var expectedValueArray = new ValueArray<string>(value);

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
