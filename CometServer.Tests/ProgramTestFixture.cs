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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.InteropServices;

    using CometServer.Configuration;
    using CometServer.Resources;

    using Microsoft.Extensions.Logging;

    using Moq;

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

        [Test]
        public void Verify_that_the_version_and_the_runtime_are_logged()
        {
            var logger = new RecordingLogger();
            var resourceLoader = new Mock<IResourceLoader>();
            resourceLoader.Setup(x => x.QueryLogo()).Returns("the logo");
            resourceLoader.Setup(x => x.QueryVersion()).Returns("10.1.2");

            Program.LogVersion(logger, resourceLoader.Object);

            var messages = logger.Entries.Select(x => x.Message).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(messages, Does.Contain("the logo"));
                Assert.That(messages, Does.Contain("Starting CDP4-COMET Services v10.1.2"));
                Assert.That(messages, Does.Contain($"Runtime: {RuntimeInformation.FrameworkDescription} on {RuntimeInformation.OSDescription} ({RuntimeInformation.RuntimeIdentifier})"));
                Assert.That(logger.Entries.Select(x => x.Level), Is.All.EqualTo(LogLevel.Information));
            });
        }

        [Test]
        public void Verify_that_the_configuration_is_logged()
        {
            var logger = new RecordingLogger();
            var appConfig = new AppConfig();
            appConfig.Midtier.UploadDirectory = "/uploads";
            appConfig.Backtier.HostName = "comet-db";
            appConfig.Backtier.ClientCertificate = "/certs/client.crt";

            Program.LogConfiguration(logger, appConfig);

            var messages = logger.Entries.Select(x => x.Message).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(messages.First(), Is.EqualTo("Configuration Loaded:"));
                Assert.That(messages, Does.Contain("Midtier-UploadDirectory: /uploads"));
                Assert.That(messages, Does.Contain("Backtier-HostName: comet-db"));
                Assert.That(messages, Does.Contain($"Backtier-SslMode: {appConfig.Backtier.SslMode}"));
                Assert.That(messages, Has.One.StartsWith("Backtier-RootCertificate: "));
                Assert.That(messages, Does.Contain("Backtier-ClientCertificate: /certs/client.crt"));
                Assert.That(messages, Has.One.StartsWith("ThingsMessageProducer-IsEnabled: "));
                Assert.That(messages.Last(), Does.StartWith("CDP4-COMET REST API is starting"));
            });
        }

        [Test]
        public void Verify_that_a_missing_client_certificate_is_logged_as_none()
        {
            var logger = new RecordingLogger();
            var appConfig = new AppConfig();
            appConfig.Backtier.ClientCertificate = " ";

            Program.LogConfiguration(logger, appConfig);

            Assert.That(logger.Entries.Select(x => x.Message), Does.Contain("Backtier-ClientCertificate: none"));
        }

        [Test]
        public void Verify_that_a_clean_termination_returns_zero()
        {
            var logger = new RecordingLogger();

            Assert.Multiple(() =>
            {
                Assert.That(Program.LogTermination(logger, 0), Is.EqualTo(0));
                Assert.That(logger.Entries.Single().Level, Is.EqualTo(LogLevel.Information));
                Assert.That(logger.Entries.Single().Message, Is.EqualTo("Terminated CDP4-COMET WebServices cleanly"));
            });
        }

        [Test]
        public void Verify_that_a_failed_startup_returns_its_exit_code()
        {
            var logger = new RecordingLogger();

            Assert.Multiple(() =>
            {
                Assert.That(Program.LogTermination(logger, 3), Is.EqualTo(3));
                Assert.That(logger.Entries.Single().Level, Is.EqualTo(LogLevel.Critical));
                Assert.That(logger.Entries.Single().Message, Does.Contain("startup failed").And.Contain("exit code 3"));
            });
        }

        [Test]
        public void Verify_that_a_configuration_error_is_logged_without_stack_trace()
        {
            var logger = new RecordingLogger();
            var exception = new InvalidOperationException("activating AppConfigService", new ConfigurationErrorsException("The Backtier:SslMode setting 'VerifyFul' is invalid"));

            var exitCode = Program.LogStartupException(logger, exception);

            var critical = logger.Entries.Single(x => x.Level == LogLevel.Critical);
            var debug = logger.Entries.Single(x => x.Level == LogLevel.Debug);

            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(-1));
                Assert.That(critical.Message, Is.EqualTo("Terminated CDP4-COMET WebServices because of an invalid configuration: The Backtier:SslMode setting 'VerifyFul' is invalid"));
                Assert.That(critical.Exception, Is.Null);
                Assert.That(debug.Exception, Is.SameAs(exception));
            });
        }

        [Test]
        public void Verify_that_an_unexpected_exception_is_logged_with_stack_trace()
        {
            var logger = new RecordingLogger();
            var exception = new InvalidOperationException("boom");

            var exitCode = Program.LogStartupException(logger, exception);

            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(-1));
                Assert.That(logger.Entries.Single().Level, Is.EqualTo(LogLevel.Critical));
                Assert.That(logger.Entries.Single().Message, Is.EqualTo("An unhandled exception occurred during startup-bootstrapping"));
                Assert.That(logger.Entries.Single().Exception, Is.SameAs(exception));
            });
        }

        /// <summary>
        /// An <see cref="ILogger"/> that records the formatted messages
        /// </summary>
        private sealed class RecordingLogger : ILogger
        {
            public List<(LogLevel Level, string Message, Exception Exception)> Entries { get; } = [];

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                this.Entries.Add((logLevel, formatter(state, exception), exception));
            }
        }
    }
}
