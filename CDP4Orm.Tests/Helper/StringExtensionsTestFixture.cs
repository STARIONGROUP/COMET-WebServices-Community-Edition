// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Tests.Helper
{
    using NUnit.Framework;

    using CDP4Orm.Helper;

    [TestFixture]
    public class StringExtensionsTestFixture
    {
        [Test]
        public void Verify_that_valid_partitions_return_true()
        {
            var validSiteDirectory = "SiteDirectory";
            var validEngineeringModel = "EngineeringModel_9ec982e4_ef72_4953_aa85_b158a95d8d56";
            var validIteration = "Iteration_9ec982e4_ef72_4953_aa85_b158a95d8d56";

            Assert.That(validSiteDirectory.IsValidPartitionName, Is.True);
            Assert.That(validEngineeringModel.IsValidPartitionName, Is.True);
            Assert.That(validIteration.IsValidPartitionName, Is.True);
        }

        [Test]
        public void Verify_that_invalid_partitions_return_true()
        {
            var inValidSiteDirectory = "SiteDirectorys";
            var inValidEngineeringModel = "EngineeringModels_9ec982e4_ef72_4953_aa85_b158a95d8d56";
            var inValidIteration = "Iteration_9ec982e4_ef72_4953_aa85_";

            Assert.That(inValidSiteDirectory.IsValidPartitionName, Is.False);
            Assert.That(inValidEngineeringModel.IsValidPartitionName, Is.False);
            Assert.That(inValidIteration.IsValidPartitionName, Is.False);
        }
    }
}
