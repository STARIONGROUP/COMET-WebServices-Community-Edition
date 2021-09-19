// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetFactoryTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
