// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetFactoryTestFixture.cs" company="RHEA System S.A.">
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
    class ParameterSubscriptionValueSetFactoryTestFixture
    {
        private ParameterSubscriptionValueSetFactory parameterSubscriptionValueSetFactory;

        private ValueArray<string> defaultValueArray;

        private ValueArray<string> nonDefaultValueArray;

        [SetUp]
        public void SetUp()
        {
            var defaultValues = this.createValues("-");
            this.defaultValueArray = new ValueArray<string>(defaultValues);

            var nonDefaultValues = this.createValues("1");
            this.nonDefaultValueArray = new ValueArray<string>(nonDefaultValues);

            this.parameterSubscriptionValueSetFactory = new ParameterSubscriptionValueSetFactory();
        }

        [Test]
        public void VerifyThatIfNonDefaultValueArrayIsProvidedArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => this.parameterSubscriptionValueSetFactory.CreateWithDefaultValueArray(Guid.NewGuid(), this.nonDefaultValueArray));
        }

        [Test]
        public void VerifyThatParameterValueSetIsCreatedWithDefaultValues()
        {
            var subscribedValueSetIid = Guid.NewGuid();

            var parameterSubscriptionValueSet = this.parameterSubscriptionValueSetFactory.CreateWithDefaultValueArray(subscribedValueSetIid, this.defaultValueArray);

            Assert.AreEqual(subscribedValueSetIid, parameterSubscriptionValueSet.SubscribedValueSet);
            Assert.AreEqual(this.defaultValueArray, parameterSubscriptionValueSet.Manual);
            Assert.AreEqual(CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL, parameterSubscriptionValueSet.ValueSwitch);
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
