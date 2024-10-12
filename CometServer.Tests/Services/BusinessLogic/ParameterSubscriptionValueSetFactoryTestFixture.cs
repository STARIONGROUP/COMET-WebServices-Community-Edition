﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetFactoryTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

    using CDP4Common.Types;

    using CometServer.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ParameterSubscriptionValueSetFactoryTestFixture
    {
        private ParameterSubscriptionValueSetFactory parameterSubscriptionValueSetFactory;

        private ValueArray<string> defaultValueArray;

        private ValueArray<string> nonDefaultValueArray;

        [SetUp]
        public void SetUp()
        {
            var defaultValues = CreateValues("-");
            this.defaultValueArray = new ValueArray<string>(defaultValues);

            var nonDefaultValues = CreateValues("1");
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

            Assert.That(parameterSubscriptionValueSet.SubscribedValueSet, Is.EqualTo(subscribedValueSetIid));
            Assert.That(parameterSubscriptionValueSet.Manual, Is.EqualTo(this.defaultValueArray));
            Assert.That(parameterSubscriptionValueSet.ValueSwitch, Is.EqualTo(CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL));
        }

        private static List<string> CreateValues(string value)
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
