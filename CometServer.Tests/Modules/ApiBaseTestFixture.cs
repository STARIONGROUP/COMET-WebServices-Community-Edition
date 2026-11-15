// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiBaseTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Modules
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using CometServer.Modules;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="ApiBase"/> class
    /// </summary>
    [TestFixture]
    public class ApiBaseTestFixture
    {
        [Test]
        public async Task VerifyThatWaitForCompletionWaitsUntilAsynchronousWorkHasCompleted()
        {
            var workCompleted = false;

            // the work yields before it is done, like a POST that awaits the database; waiting for the
            // delegate to return instead of for its task would report completion right away
            var completedWithinWaitTime = await ApiBase.WaitForCompletionAsync(
                async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                    workCompleted = true;
                },
                TimeSpan.FromSeconds(30));

            Assert.Multiple(() =>
            {
                Assert.That(completedWithinWaitTime, Is.True);
                Assert.That(workCompleted, Is.True);
            });
        }

        [Test]
        public async Task VerifyThatWaitForCompletionDoesNotWaitForTheWholeWaitTime()
        {
            var stopwatch = Stopwatch.StartNew();

            var completedWithinWaitTime = await ApiBase.WaitForCompletionAsync(() => Task.CompletedTask, TimeSpan.FromSeconds(30));

            Assert.Multiple(() =>
            {
                Assert.That(completedWithinWaitTime, Is.True);
                Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(10)));
            });
        }

        [Test]
        public async Task VerifyThatWaitForCompletionReturnsFalseWhenTheWorkTakesLongerThanTheWaitTime()
        {
            var release = new TaskCompletionSource();
            var workDone = new TaskCompletionSource();
            var workCompleted = false;

            var completedWithinWaitTime = await ApiBase.WaitForCompletionAsync(
                async () =>
                {
                    await release.Task;
                    workCompleted = true;
                    workDone.SetResult();
                },
                TimeSpan.FromMilliseconds(100));

            Assert.Multiple(() =>
            {
                Assert.That(completedWithinWaitTime, Is.False);
                Assert.That(workCompleted, Is.False);
            });

            // the work is not cancelled or abandoned when the wait time has elapsed: it carries on and completes
            release.SetResult();
            await workDone.Task.WaitAsync(TimeSpan.FromSeconds(30));

            Assert.That(workCompleted, Is.True);
        }

        [Test]
        public async Task VerifyThatWaitForCompletionAcceptsAWaitTimeLongerThanTaskDelaySupports()
        {
            var workCompleted = false;

            // Task.Delay throws for a delay this long; the wait time is capped instead, so the work is still awaited
            var completedWithinWaitTime = await ApiBase.WaitForCompletionAsync(
                async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    workCompleted = true;
                },
                TimeSpan.FromSeconds(int.MaxValue));

            Assert.Multiple(() =>
            {
                Assert.That(completedWithinWaitTime, Is.True);
                Assert.That(workCompleted, Is.True);
            });
        }

        [Test]
        public void VerifyThatWaitForCompletionDoesNotStartTheWorkWhenTheWaitTimeIsNegative()
        {
            var workStarted = false;

            Assert.That(
                async () => await ApiBase.WaitForCompletionAsync(
                    () =>
                    {
                        workStarted = true;
                        return Task.CompletedTask;
                    },
                    TimeSpan.FromSeconds(-5)),
                Throws.TypeOf<ArgumentOutOfRangeException>());

            Assert.That(workStarted, Is.False);
        }

        [Test]
        public void VerifyThatWaitForCompletionRethrowsAnExceptionOfTheWork()
        {
            var failure = new InvalidOperationException("the work failed");

            Assert.That(
                async () => await ApiBase.WaitForCompletionAsync(
                    async () =>
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(50));
                        throw failure;
                    },
                    TimeSpan.FromSeconds(30)),
                Throws.Exception.SameAs(failure));
        }

        [Test]
        public void VerifyThatWaitForCompletionRejectsNullWork()
        {
            Assert.That(async () => await ApiBase.WaitForCompletionAsync(null, TimeSpan.FromSeconds(1)), Throws.ArgumentNullException);
        }
    }
}
