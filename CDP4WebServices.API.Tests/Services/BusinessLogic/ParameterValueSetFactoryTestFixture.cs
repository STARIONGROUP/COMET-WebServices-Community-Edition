// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetFactoryTestFixture.cs" company="RHEA System S.A.">
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
    public class ParameterValueSetFactoryTestFixture
    {
        private ParameterValueSetFactory parameterValueSetFactory;

        private ValueArray<string> defaultValueArray;

        private ValueArray<string> nonDefaultValueArray;

        [SetUp]
        public void SetUp()
        {
            var defaultValues = this.createValues("-");
            this.defaultValueArray = new ValueArray<string>(defaultValues);

            var nonDefaultValues = this.createValues("1");
            this.nonDefaultValueArray = new ValueArray<string>(nonDefaultValues);

            this.parameterValueSetFactory = new ParameterValueSetFactory();
        }

        [Test]
        public void VerifyThatIfNonDefaultValueArrayIsProvidedArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => this.parameterValueSetFactory.CreateNewParameterValueSetFromSource(null, null, null, this.nonDefaultValueArray));
        }

        [Test]
        public void VerifyThatParameterValueSetIsCreatedWithDefaultValues()
        {
            var optionIid = Guid.NewGuid();
            var actualStateIid = Guid.NewGuid();

            var parameterValueSet =  this.parameterValueSetFactory.CreateNewParameterValueSetFromSource(optionIid, actualStateIid, null, this.defaultValueArray);

            Assert.AreEqual(optionIid, parameterValueSet.ActualOption);
            Assert.AreEqual(actualStateIid, parameterValueSet.ActualState);

            Assert.AreEqual(this.defaultValueArray, parameterValueSet.Manual);
            Assert.AreEqual(this.defaultValueArray, parameterValueSet.Computed);
            Assert.AreEqual(this.defaultValueArray, parameterValueSet.Reference);
            Assert.AreEqual(this.defaultValueArray, parameterValueSet.Formula);
            Assert.AreEqual(this.defaultValueArray, parameterValueSet.Published);
            Assert.AreEqual(CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL, parameterValueSet.ValueSwitch);
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
