// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainmentServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.CherryPick
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Services.CherryPick;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    public class ContainmentServiceTestFixture
    {
        private ContainmentService containmentService;
        private List<Thing> things;

        [SetUp]
        public void SetUp()
        {
            this.containmentService = new ContainmentService();
            this.things = new List<Thing>();

            var parameterValueSet = new ParameterValueSet()
            {
                Iid = Guid.NewGuid()
            };

            var parameter = new Parameter()
            {
                Iid = Guid.NewGuid(),
                ValueSet = new List<Guid> { parameterValueSet.Iid }
            };

            var parameterOverrideValueSet = new ParameterOverrideValueSet()
            {
                Iid = Guid.NewGuid()
            };

            var parameterOverride = new ParameterOverride()
            {
                Iid = Guid.NewGuid(),
                ValueSet = new List<Guid> { parameterOverrideValueSet.Iid }
            };

            var elementUsage = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                ParameterOverride = new List<Guid> { parameterOverride.Iid }
            };

            var elementDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Parameter = new List<Guid> { parameter.Iid },
                ContainedElement = new List<Guid> { elementUsage.Iid }
            };

            var iteration = new Iteration()
            {
                Iid = Guid.NewGuid(),
                Element = new List<Guid> { elementDefinition.Iid }
            };

            this.things.AddRange(new List<Thing> { iteration, elementDefinition, elementUsage, parameterOverride, parameter, parameterValueSet, parameterOverrideValueSet, new Category() { Iid = Guid.NewGuid() } });
        }

        [Test]
        public void VerifyRetrieveContainedThings()
        {
            var classKinds = new[] { ClassKind.ParameterOverride, ClassKind.Parameter, ClassKind.ParameterOverrideValueSet, ClassKind.ParameterValueSet, ClassKind.ElementUsage, ClassKind.ElementDefinition };
            var containedThings = this.containmentService.QueryContainedThings(this.things.OfType<Iteration>().ToList(), this.things, false, classKinds).ToList();
            Assert.That(containedThings, Has.Count.EqualTo(1));
            containedThings = this.containmentService.QueryContainedThings(this.things.OfType<ElementDefinition>().ToList(), this.things, false, classKinds).ToList();
            Assert.That(containedThings, Has.Count.EqualTo(2));
            containedThings = this.containmentService.QueryContainedThings(this.things.OfType<Iteration>().ToList(), this.things, true, classKinds).ToList();
            Assert.That(containedThings, Has.Count.EqualTo(6));
            containedThings = this.containmentService.QueryContainedThings(this.things.OfType<Parameter>().ToList(), this.things, true, classKinds).ToList();
            Assert.That(containedThings, Has.Count.EqualTo(1));
        }

        [Test]
        public void VerifyQueryContainersTree()
        {
            var tree = this.containmentService.QueryContainersTree(this.things.OfType<ParameterOverrideValueSet>().Single(), this.things).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(tree, Has.Count.EqualTo(4));
                Assert.That(tree.OfType<ParameterOverride>().ToList(), Has.Count.EqualTo(1));
                Assert.That(tree.OfType<ElementUsage>().ToList(), Has.Count.EqualTo(1));
                Assert.That(tree.OfType<ElementDefinition>().ToList(), Has.Count.EqualTo(1));
                Assert.That(tree.OfType<Iteration>().ToList(), Has.Count.EqualTo(1));
            });

            tree = this.containmentService.QueryContainersTree(this.things.OfType<ParameterOrOverrideBase>().ToList(), this.things).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(tree, Has.Count.EqualTo(3));
                Assert.That(tree.OfType<ElementUsage>().ToList(), Has.Count.EqualTo(1));
                Assert.That(tree.OfType<ElementDefinition>().ToList(), Has.Count.EqualTo(1));
                Assert.That(tree.OfType<Iteration>().ToList(), Has.Count.EqualTo(1));
            });
        }
    }

}
