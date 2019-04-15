// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Tests
{
    using System;
    using System.Linq;
    using CDP4Orm.Dao;
    using MigrationEngine;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="MigrationBase"/> class
    /// </summary>
    [TestFixture]
    public class MigrationTestFixture
    {
        [Test]
        public void VerifyThatAllScriptsAreRegistered()
        {
            var migrations = MigrationService.GetMigrations(true);
            Assert.IsNotEmpty(migrations);
        }
    }
}
