// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionResolverTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Orm.Dao.Revision;

    using CometServer.Services;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="RevisionResolver"/> class
    /// </summary>
    [TestFixture]
    public class RevisionResolverTestFixture
    {
        private RevisionResolver revisionResolver;
        private Mock<IRevisionDao> revisionDao;
        private RevisionRegistryInfo revision1;
        private RevisionRegistryInfo revision2;
        private RevisionRegistryInfo revision3;
        private RevisionRegistryInfo revision4;
        private RevisionRegistryInfo revision5;
        private static readonly DateTime timestamp1 = new DateTime(2020, 1, 1);
        private static readonly DateTime timestamp2 = new DateTime(2020, 1, 2);
        private static readonly DateTime timestamp3 = new DateTime(2020, 1, 2, 10, 0, 0);
        private static readonly DateTime timestamp4 = new DateTime(2020, 1, 2, 14, 0, 0);
        private static readonly DateTime timestamp5 = new DateTime(2020, 1, 3);

        [SetUp]
        public void TestSetup()
        {
            this.revisionDao = new Mock<IRevisionDao>();
            this.revisionResolver = new RevisionResolver { RevisionDao = this.revisionDao.Object };

            this.revision1 = new RevisionRegistryInfo
            {
                Actor = Guid.NewGuid(),
                Instant = timestamp1,
                Revision = 1
            };

            this.revision2 = new RevisionRegistryInfo
            {
                Actor = Guid.NewGuid(),
                Instant = timestamp2,
                Revision = 2
            };

            this.revision3 = new RevisionRegistryInfo
            {
                Actor = Guid.NewGuid(),
                Instant = timestamp3,
                Revision = 3
            };

            this.revision4 = new RevisionRegistryInfo
            {
                Actor = Guid.NewGuid(),
                Instant = timestamp4,
                Revision = 4
            };

            this.revision5 = new RevisionRegistryInfo
            {
                Actor = Guid.NewGuid(),
                Instant = timestamp5,
                Revision = 5
            };

            var revisionRegistryInfoList = new List<RevisionRegistryInfo>
            {
                this.revision1,
                this.revision2,
                this.revision3,
                this.revision4,
                this.revision5
            };

            this.revisionDao.Setup(x => x.ReadRevisionRegistry(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>())).Returns(revisionRegistryInfoList);
        }

        [Test]
        [TestCaseSource(nameof(IntTestCases))]
        public void VerifyThatCorrectRevisionsAreReturnedWhenRevisionNumbersAreUsed(int? fromRevision, int? toRevision, int checkFromRevision, int checkToRevision, int checkRevisionCount)
        {
            this.revisionResolver.TryResolve(It.IsAny<NpgsqlTransaction>(), "", fromRevision, toRevision, out var resolvedValues);

            Assert.AreEqual(checkFromRevision, resolvedValues.FromRevision);
            Assert.AreEqual(checkToRevision, resolvedValues.ToRevision);
            Assert.AreEqual(checkRevisionCount, resolvedValues.RevisionRegistryInfoList.Count());
        }

        [Test]
        [TestCaseSource(nameof(DateTimeTestCases))]
        public void VerifyThatCorrectRevisionsAreReturnedWhenRevisionTimestampsAreUsed(DateTime? fromRevision, DateTime? toRevision, int checkFromRevision, int checkToRevision, int checkRevisionCount)
        {
            this.revisionResolver.TryResolve(It.IsAny<NpgsqlTransaction>(), "", fromRevision, toRevision, out var resolvedValues);

            Assert.AreEqual(checkFromRevision, resolvedValues.FromRevision);
            Assert.AreEqual(checkToRevision, resolvedValues.ToRevision);
            Assert.AreEqual(checkRevisionCount, resolvedValues.RevisionRegistryInfoList.Count());
        }

        public static IEnumerable IntTestCases()
        {
            yield return new object[] { 1, 5, 1, 5, 5 };
            yield return new object[] { 2, 4, 2, 4, 3 };
            yield return new object[] { 2, null, 2, 5, 4 };
            yield return new object[] { null, 3, 1, 3, 3 };
            yield return new object[] { 0, int.MaxValue, 1, 5, 5 };
        }

        public static IEnumerable DateTimeTestCases()
        {
            yield return new object[] { timestamp1, timestamp5, 1, 5, 5 };
            yield return new object[] { timestamp2, timestamp4, 2, 4, 3 };
            yield return new object[] { timestamp2, null, 2, 5, 4 };
            yield return new object[] { null, timestamp3, 1, 3, 3 };
            yield return new object[] { timestamp1.AddDays(-1), timestamp2.AddDays(1), 1, 5, 5 };

            yield return new object[] { timestamp1.AddSeconds(1), timestamp5.AddSeconds(-1), 2, 4, 3 };
            yield return new object[] { timestamp2.AddSeconds(1), timestamp4.AddSeconds(-1), 3, 3, 1 };
        }
    }
}
