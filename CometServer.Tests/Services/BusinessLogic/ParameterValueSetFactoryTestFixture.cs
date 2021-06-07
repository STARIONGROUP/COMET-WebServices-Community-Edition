// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetFactoryTestFixture.cs" company="RHEA System S.A.">
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

    using CDP4Common.Types;

    using CometServer.Services;

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
