// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheDaoTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CDP4Orm.Tests
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4Orm.Dao.Cache;

    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="CacheDao"/> class
    /// </summary>
    [TestFixture]
    public class CacheDaoTestFixture
    {
        private CacheDao cacheDao;

        [SetUp]
        public void SetUp()
        {
            this.cacheDao = new CacheDao { Logger = NullLogger<CacheDao>.Instance };
        }

        [Test]
        public void VerifyThatBulkWriteRejectsDuplicateIidsBeforeAccessingTheDatabase()
        {
            var iid = Guid.NewGuid();

            var things = new List<Thing>
            {
                new Person(iid, 1),
                new Person(Guid.NewGuid(), 1),
                new Person(iid, 2)
            };

            // the transaction is null: the check must run before any statement is sent to the database
            Assert.That(
                async () => await this.cacheDao.BulkWriteAsync(null, "SiteDirectory", things),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("things").And.Message.Contains(iid.ToString()));
        }
    }
}
