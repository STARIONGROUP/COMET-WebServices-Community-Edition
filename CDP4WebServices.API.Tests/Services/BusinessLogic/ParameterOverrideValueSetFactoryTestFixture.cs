// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideValueSetFactoryTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services;
    using NUnit.Framework;

    [TestFixture]
    public class ParameterOverrideValueSetFactoryTestFixture
    {
        private ParameterOverrideValueSetFactory parameterOverrideValueSetFactory;

        private ValueArray<string> defaultValueArray;

        private ValueArray<string> nonDefaultValueArray;

        [SetUp]
        public void SetUp()
        {
            var defaultValues = this.createValues("-");
            this.defaultValueArray = new ValueArray<string>(defaultValues);

            var nonDefaultValues = this.createValues("1");
            this.nonDefaultValueArray = new ValueArray<string>(nonDefaultValues);

            this.parameterOverrideValueSetFactory = new ParameterOverrideValueSetFactory();
        }
        
        [Test]
        public void VerifyThatIfNonDefaultValueArrayIsProvidedArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => this.parameterOverrideValueSetFactory.CreateWithDefaultValueArray(Guid.NewGuid(), this.nonDefaultValueArray));
        }
            
        [Test]
        public void VerifyThatParameterValueSetIsCreatedWithDefaultValues()
        {
            var parameterValueSetIid = Guid.NewGuid();

            var parameterOverrideValueSet = this.parameterOverrideValueSetFactory.CreateWithDefaultValueArray(parameterValueSetIid, this.defaultValueArray);
            
            Assert.AreEqual(parameterValueSetIid, parameterOverrideValueSet.ParameterValueSet);

            Assert.AreEqual(this.defaultValueArray, parameterOverrideValueSet.Manual);
            Assert.AreEqual(this.defaultValueArray, parameterOverrideValueSet.Computed);
            Assert.AreEqual(this.defaultValueArray, parameterOverrideValueSet.Reference);
            Assert.AreEqual(this.defaultValueArray, parameterOverrideValueSet.Formula);
            Assert.AreEqual(this.defaultValueArray, parameterOverrideValueSet.Published);
            Assert.AreEqual(CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL, parameterOverrideValueSet.ValueSwitch);
        }

        private IEnumerable<string> createValues(string value)
        {
            var defaultValue = new List<string>(3);
            for (var i = 0; i < 3; i++)
            {
                defaultValue.Add(value);
            }

            return defaultValue;
        }
    }
}
