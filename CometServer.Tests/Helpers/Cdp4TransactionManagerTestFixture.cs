// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4TransactionManagerTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="Cdp4TransactionManager"/>.
    /// </summary>
    [TestFixture]
    public class Cdp4TransactionManagerTestFixture
    {
        private const string Partition = "EngineeringModel_partition";

        private Mock<IIterationSetupDao> iterationSetupDao;

        private TestCdp4TransactionManager transactionManager;

        private NpgsqlTransaction transaction;

        private Guid iterationIid;

        [SetUp]
        public void SetUp()
        {
            this.iterationIid = Guid.NewGuid();
            this.transaction = CreateTransaction();

            this.iterationSetupDao = new Mock<IIterationSetupDao>();

            this.iterationSetupDao
                .Setup(x => x.ReadByIterationAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), this.iterationIid, It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<IterationSetup> { new(Guid.NewGuid(), 1) { IterationIid = this.iterationIid } });

            this.transactionManager = new TestCdp4TransactionManager
            {
                IterationSetupDao = this.iterationSetupDao.Object
            };
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsReadOnceForRepeatedCallsOnTheSameTransaction()
        {
            this.transactionManager.SetCachedDtoReadEnabled(true);

            Assert.That(await this.transactionManager.GetRawSessionInstantAsync(this.transaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.True);
            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.True);

            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(1));
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsNotReadWhenCachedDtoReadIsDisabled()
        {
            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.False);

            Assert.That(this.transactionManager.SessionInstantReadCount, Is.Zero);
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsReadPerTransaction()
        {
            var otherTransaction = CreateTransaction();
            var otherInstant = new DateTime(2020, 1, 1);

            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            this.transactionManager.SessionInstant = otherInstant;

            Assert.That(await this.transactionManager.GetSessionInstantAsync(otherTransaction), Is.EqualTo(otherInstant));
            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(await this.transactionManager.GetSessionInstantAsync(otherTransaction), Is.EqualTo(otherInstant));

            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(2));
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsReadAgainAfterSetDefaultContext()
        {
            var frozenInstant = new DateTime(2020, 1, 1);
            this.transactionManager.SessionInstant = frozenInstant;
            this.transactionManager.SetCachedDtoReadEnabled(true);

            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.False);

            this.transactionManager.SessionInstant = DateTime.MaxValue;

            await this.transactionManager.SetDefaultContextAsync(this.transaction);

            Assert.That(this.transactionManager.DefaultContextWriteCount, Is.EqualTo(1));
            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.True);
            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(2));
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsReadAgainAfterSetIterationContext()
        {
            var frozenInstant = new DateTime(2020, 1, 1);
            this.transactionManager.SetCachedDtoReadEnabled(true);

            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.True);
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(1));

            // setting the iteration context reads the instant once in the default context, to retrieve the IterationSetup
            await this.transactionManager.SetIterationContextAsync(this.transaction, Partition, this.iterationIid);

            Assert.That(this.transactionManager.IterationSetup.IterationIid, Is.EqualTo(this.iterationIid));
            Assert.That(this.transactionManager.DefaultContextWriteCount, Is.EqualTo(1));
            Assert.That(this.transactionManager.IterationContextWriteCount, Is.EqualTo(1));
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(2));

            this.transactionManager.SessionInstant = frozenInstant;

            Assert.That(await this.transactionManager.IsCachedDtoReadEnabledAsync(this.transaction), Is.False);
            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(frozenInstant));
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(3));

            // applying the iteration context that was set before invalidates the instant as well
            this.transactionManager.SessionInstant = DateTime.MaxValue;

            await this.transactionManager.SetIterationContextAsync(this.transaction, Partition);

            Assert.That(this.transactionManager.IterationContextWriteCount, Is.EqualTo(2));
            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(4));
        }

        [Test]
        public async Task VerifyThatSetIterationContextWithoutIterationSetupKeepsTheSessionInstant()
        {
            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            await this.transactionManager.SetIterationContextAsync(this.transaction, Partition);

            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            Assert.That(this.transactionManager.IterationContextWriteCount, Is.Zero);
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(1));
        }

        [Test]
        public async Task VerifyThatTheSessionInstantIsReadAgainWhenSettingTheContextFails()
        {
            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            this.transactionManager.ThrowOnWrite = true;

            Assert.That(() => this.transactionManager.SetDefaultContextAsync(this.transaction), Throws.InvalidOperationException);

            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(2));
        }

        [Test]
        public async Task VerifyThatANewTransactionDoesNotReuseTheSessionInstantOfAnEarlierTransaction()
        {
            var frozenInstant = new DateTime(2020, 1, 1);
            this.transactionManager.SessionInstant = frozenInstant;

            Assert.That(await this.transactionManager.GetSessionInstantAsync(this.transaction), Is.EqualTo(frozenInstant));

            // Npgsql hands out the same NpgsqlTransaction object again for a later transaction on the same connector
            this.transactionManager.NewTransaction = this.transaction;
            this.transactionManager.SessionInstant = DateTime.MaxValue;

            var newTransaction = await this.transactionManager.SetupTransactionAsync(new Credentials());

            Assert.That(newTransaction, Is.SameAs(this.transaction));
            Assert.That(await this.transactionManager.GetSessionInstantAsync(newTransaction), Is.EqualTo(DateTime.MaxValue));
            Assert.That(this.transactionManager.SessionInstantReadCount, Is.EqualTo(2));
        }

        [Test]
        public async Task VerifyThatANewTransactionWithIterationContextDoesNotReuseTheSessionInstantOfAnEarlierTransaction()
        {
            var frozenInstant = new DateTime(2020, 1, 1);
            this.transactionManager.SessionInstant = frozenInstant;

            await this.transactionManager.GetSessionInstantAsync(this.transaction);

            this.transactionManager.NewTransaction = this.transaction;
            this.transactionManager.SessionInstant = DateTime.MaxValue;

            await this.transactionManager.SetupTransactionAsync(new Credentials(), this.iterationIid);

            Assert.That(this.transactionManager.IterationSetup.IterationIid, Is.EqualTo(this.iterationIid));

            this.iterationSetupDao.Verify(x => x.ReadByIterationAsync(this.transaction, "SiteDirectory", this.iterationIid, DateTime.MaxValue), Times.Once);
            this.iterationSetupDao.Verify(x => x.ReadByIterationAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Guid>(), frozenInstant), Times.Never);
        }

        /// <summary>
        /// Creates an <see cref="NpgsqlTransaction"/> that is not connected to a database, to be used as identity only
        /// </summary>
        /// <returns>
        /// An <see cref="NpgsqlTransaction"/>
        /// </returns>
        private static NpgsqlTransaction CreateTransaction()
        {
            return (NpgsqlTransaction)RuntimeHelpers.GetUninitializedObject(typeof(NpgsqlTransaction));
        }

        /// <summary>
        /// A <see cref="Cdp4TransactionManager"/> of which the database statements are replaced by counters
        /// </summary>
        private sealed class TestCdp4TransactionManager : Cdp4TransactionManager
        {
            /// <summary>
            /// Gets or sets the session instant that the database returns
            /// </summary>
            public DateTime SessionInstant { get; set; } = DateTime.MaxValue;

            /// <summary>
            /// Gets or sets the <see cref="NpgsqlTransaction"/> that is handed out as new transaction
            /// </summary>
            public NpgsqlTransaction NewTransaction { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether writing the temporal context fails
            /// </summary>
            public bool ThrowOnWrite { get; set; }

            /// <summary>
            /// Gets the number of times the session instant has been read from the database
            /// </summary>
            public int SessionInstantReadCount { get; private set; }

            /// <summary>
            /// Gets the number of times the default context has been written to the database
            /// </summary>
            public int DefaultContextWriteCount { get; private set; }

            /// <summary>
            /// Gets the number of times the iteration context has been written to the database
            /// </summary>
            public int IterationContextWriteCount { get; private set; }

            /// <inheritdoc />
            internal override Task<NpgsqlTransaction> CreateTransactionAsync(Credentials credentials)
            {
                return Task.FromResult(this.NewTransaction);
            }

            /// <inheritdoc />
            internal override Task<object> ReadSessionInstantAsync(NpgsqlTransaction transaction)
            {
                this.SessionInstantReadCount++;

                return Task.FromResult<object>(this.SessionInstant);
            }

            /// <inheritdoc />
            internal override Task WriteDefaultContextAsync(NpgsqlTransaction transaction)
            {
                if (this.ThrowOnWrite)
                {
                    throw new InvalidOperationException("the write failed");
                }

                this.DefaultContextWriteCount++;

                return Task.CompletedTask;
            }

            /// <inheritdoc />
            internal override Task WriteIterationContextAsync(NpgsqlTransaction transaction, string partition, IterationSetup iterationSetup)
            {
                this.IterationContextWriteCount++;

                return Task.CompletedTask;
            }
        }
    }
}
