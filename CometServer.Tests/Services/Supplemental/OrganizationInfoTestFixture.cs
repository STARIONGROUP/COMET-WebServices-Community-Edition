// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationInfoTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;

    using CometServer.Services;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="OrganizationInfo"/>.
    /// </summary>
    [TestFixture]
    public class OrganizationInfoTestFixture
    {
        [Test]
        public void OrganizationInfoStoresValues()
        {
            var iid = Guid.NewGuid();
            var info = new OrganizationInfo
            {
                Iid = iid,
                Name = "Starion",
                Unit = "R&D",
                Site = "Luxembourg"
            };

            Assert.That(info.Iid, Is.EqualTo(iid));
            Assert.That(info.Name, Is.EqualTo("Starion"));
            Assert.That(info.Unit, Is.EqualTo("R&D"));
            Assert.That(info.Site, Is.EqualTo("Luxembourg"));
        }
    }
}
