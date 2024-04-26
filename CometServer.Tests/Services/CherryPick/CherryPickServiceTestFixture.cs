// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CherryPickServiceTestFixture.cs" company="Starion Group S.A.">
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
    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Thing = CDP4Common.DTO.Thing;

    using CometServer.Services.CherryPick;

    [TestFixture]
    public class CherryPickServiceTestFixture
    {
        private CherryPickService cherryPickService;
        private List<Thing> things;
        private Category sensorCategory;
        private Category equipmentCategory;

        [SetUp]
        public void Setup()
        {
            this.cherryPickService = new CherryPickService();
            this.things = new List<Thing>();

            this.sensorCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = "Sensor",
                ShortName = "sensor"
            };

            this.equipmentCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = "Equipment",
                ShortName = "equipment"
            };

            var textParameterType = new TextParameterType()
            {
                Iid = Guid.NewGuid(),
                Name = "Text",
                ShortName = "text",
                Category = new List<Guid> { this.sensorCategory.Iid }
            };

            var textParameter = new Parameter()
            {
                Iid = Guid.NewGuid(),
                ParameterType = textParameterType.Iid
            };

            var booleanParameterType = new BooleanParameterType()
            {
                Iid = Guid.NewGuid(),
                Name = "bool",
                ShortName = "b"
            };

            var boolParameter = new Parameter()
            {
                Iid = Guid.NewGuid(),
                ParameterType = booleanParameterType.Iid
            };

            var dateParameterType = new DateParameterType()
            {
                Iid = Guid.NewGuid(),
                Name = "date",
                ShortName = "date",
                Category = new List<Guid> { Guid.NewGuid() }
            };

            var dateParameter = new Parameter()
            {
                Iid = Guid.NewGuid(),
                ParameterType = dateParameterType.Iid
            };

            var elementDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Parameter = new List<Guid> { boolParameter.Iid, dateParameter.Iid, textParameter.Iid }
            };

            var categorizedElementDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid> { this.equipmentCategory.Iid }
            };

            var categorizedElementUsage = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid> { this.equipmentCategory.Iid },
                ElementDefinition = elementDefinition.Iid
            };

            var elementUsage = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                ElementDefinition = categorizedElementDefinition.Iid
            };

            var parameterOverride = new ParameterOverride()
            {
                Iid = Guid.NewGuid(),
                Parameter = textParameter.Iid
            };

            this.things.AddRange(new List<Thing>
            {
                elementUsage, categorizedElementUsage, categorizedElementDefinition, elementDefinition, dateParameter, dateParameterType,
                boolParameter, booleanParameterType, textParameter, textParameterType, this.sensorCategory, this.equipmentCategory,
                new Category{Iid = Guid.NewGuid()}, parameterOverride
            });
        }

        [Test]
        public void VerifyCherryPickParameter()
        {
            var cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.Parameter, new List<Guid> { this.equipmentCategory.Iid }).ToList();
            Assert.That(cherryPicked, Is.Empty);

            cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.Parameter, new List<Guid> { this.sensorCategory.Iid }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cherryPicked, Has.Count.EqualTo(1));
                Assert.That(cherryPicked[0], Is.TypeOf<Parameter>());
            });
        }

        [Test]
        public void VerifyCherryPickParameterOverride()
        {
            var cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ParameterOverride, new List<Guid> { this.equipmentCategory.Iid }).ToList();
            Assert.That(cherryPicked, Is.Empty);

            cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ParameterOverride, new List<Guid> { this.sensorCategory.Iid }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cherryPicked, Has.Count.EqualTo(1));
                Assert.That(cherryPicked[0], Is.TypeOf<ParameterOverride>());
            });
        }

        [Test]
        public void VerifyCherryPickElementUsage()
        {
            var cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ElementUsage, new List<Guid> { this.sensorCategory.Iid }).ToList();
            Assert.That(cherryPicked, Is.Empty);

            cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ElementUsage, new List<Guid> { this.equipmentCategory.Iid }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cherryPicked, Has.Count.EqualTo(2));
                Assert.That(cherryPicked[0], Is.TypeOf<ElementUsage>());
                Assert.That(cherryPicked[1], Is.TypeOf<ElementUsage>());
            });
        }

        [Test]
        public void VerifyCherryPickElementDefinition()
        {
            var cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ElementDefinition, new List<Guid> { this.sensorCategory.Iid }).ToList();
            Assert.That(cherryPicked, Is.Empty);

            cherryPicked = this.cherryPickService.CherryPick(this.things, ClassKind.ElementDefinition, new List<Guid> { this.equipmentCategory.Iid }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cherryPicked, Has.Count.EqualTo(1));
                Assert.That(cherryPicked[0], Is.TypeOf<ElementDefinition>());
            });
        }

        [Test]
        public void VerifyCanCherryPickMultipleClassKinds()
        {
            var cherryPicked = this.cherryPickService.CherryPick(this.things, new List<ClassKind>
            {
                ClassKind.ElementDefinition,
                ClassKind.ElementUsage,
                ClassKind.ParameterOverride,
                ClassKind.Parameter
            }, new List<Guid> { this.equipmentCategory.Iid, this.sensorCategory.Iid }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cherryPicked, Has.Count.EqualTo(5));
                Assert.That(cherryPicked.OfType<ElementUsage>().ToList(), Has.Count.EqualTo(2));
                Assert.That(cherryPicked.OfType<ElementDefinition>().ToList(), Has.Count.EqualTo(1));
                Assert.That(cherryPicked.OfType<Parameter>().ToList(), Has.Count.EqualTo(1));
                Assert.That(cherryPicked.OfType<ParameterOverride>().ToList(), Has.Count.EqualTo(1));
            });
        }
    }
}
