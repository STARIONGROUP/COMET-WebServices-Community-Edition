// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Tests
{
    using System;
    using System.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="Program"/> class
    /// </summary>
    [TestFixture]
    public class ProgramTestFixture
    {
        [Test]
        public void Verify_that_a_wrapped_configuration_error_is_found()
        {
            var configurationError = new ConfigurationErrorsException("The Backtier:SslMode setting 'VerifyFul' is invalid");
            var wrapped = new InvalidOperationException("activating AppConfigService", new InvalidOperationException("invoking the constructor", configurationError));

            Assert.That(Program.FindConfigurationError(wrapped), Is.SameAs(configurationError));
        }

        [Test]
        public void Verify_that_other_exceptions_are_not_configuration_errors()
        {
            Assert.That(Program.FindConfigurationError(new InvalidOperationException("boom", new ArgumentException("bad"))), Is.Null);
        }
    }
}
