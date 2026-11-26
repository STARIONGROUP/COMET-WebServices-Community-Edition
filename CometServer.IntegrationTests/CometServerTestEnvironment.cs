// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometServerTestEnvironment.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Autofac.Extensions.DependencyInjection;

    using DotNet.Testcontainers.Builders;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using Npgsql;

    using NUnit.Framework;

    using Serilog;
    using Serilog.Events;

    using Testcontainers.PostgreSql;

    /// <summary>
    /// Starts, once for all integration tests, a PostgreSQL database in a Docker container (Testcontainers) and the
    /// CDP4-COMET REST API in-process (<see cref="TestServer"/>), and seeds the database with the ECSS-E-TM-10-25
    /// Annex C.3 test data set. The tests are ignored when Docker is not available.
    /// </summary>
    [SetUpFixture]
    public class CometServerTestEnvironment
    {
        /// <summary>
        /// The Docker image of the database, the same one as used by the CI workflow
        /// </summary>
        public const string DatabaseImage = "stariongroup/cdp4-test-database-community-edition:4.2.0";

        /// <summary>
        /// The user name of the seeded administrator
        /// </summary>
        public const string AdminUserName = "admin";

        /// <summary>
        /// The password of the seeded administrator
        /// </summary>
        public const string AdminPassword = "pass";

        /// <summary>
        /// The maximum time to wait for the REST API to become healthy
        /// </summary>
        private static readonly TimeSpan StartupTimeout = TimeSpan.FromMinutes(2);

        /// <summary>
        /// The PostgreSQL container
        /// </summary>
        private static PostgreSqlContainer databaseContainer;

        /// <summary>
        /// The in-process host of the REST API
        /// </summary>
        private static IHost host;

        /// <summary>
        /// The folder that holds the uploaded, stored and exported files of the REST API
        /// </summary>
        private static string workingDirectory;

        /// <summary>
        /// Gets the connection string to the cdp4server database with the PostgreSQL super user, used to verify the
        /// persisted data directly. Pooling is disabled so that no connection blocks the drop of the database on restore.
        /// </summary>
        public static string DatabaseConnectionString { get; private set; }

        /// <summary>
        /// Starts the database container and the REST API and seeds the data
        /// </summary>
        [OneTimeSetUp]
        public async Task StartAsync()
        {
            try
            {
                databaseContainer = new PostgreSqlBuilder(DatabaseImage)
                    .WithUsername("postgres")
                    .WithPassword("postgres")
                    .WithDatabase("postgres")
                    .WithCommand("-c", "max_locks_per_transaction=1024")
                    .Build();

                await databaseContainer.StartAsync();
            }
            catch (DockerUnavailableException exception)
            {
                Assert.Ignore($"The integration tests require Docker: {exception.Message}");
            }

            DatabaseConnectionString = new NpgsqlConnectionStringBuilder(databaseContainer.GetConnectionString())
            {
                Database = "cdp4server",
                Pooling = false
            }.ConnectionString;

            workingDirectory = Path.Combine(Path.GetTempPath(), "comet-integration-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workingDirectory);

            var settings = new Dictionary<string, string>
            {
                ["Backtier:HostName"] = databaseContainer.Hostname,
                ["Backtier:Port"] = databaseContainer.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort).ToString(),
                ["Midtier:UploadDirectory"] = Path.Combine(workingDirectory, "upload"),
                ["Midtier:FileStorageDirectory"] = Path.Combine(workingDirectory, "storage"),
                ["Midtier:TemporaryFileStorageDirectory"] = Path.Combine(workingDirectory, "tempstorage"),
                ["Midtier:ExportDirectory"] = Path.Combine(workingDirectory, "export")
            };

            host = await new HostBuilder()
                .UseEnvironment(Environments.Development)
                .UseContentRoot(AppContext.BaseDirectory)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.Sources.Clear();
                    configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.IntegrationTests.json"), false);
                    configuration.AddInMemoryCollection(settings);
                })
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer(options => options.AllowSynchronousIO = true)
                        .UseStartup<Startup>();
                })
                .UseSerilog((_, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .MinimumLevel.Warning()
                        .MinimumLevel.Override("Hangfire", LogEventLevel.Error)
                        .WriteTo.Console();
                })
                .StartAsync();

            await WaitUntilHealthyAsync();
            await SeedAsync();
        }

        /// <summary>
        /// Stops the REST API and the database container
        /// </summary>
        [OneTimeTearDown]
        public async Task StopAsync()
        {
            if (host != null)
            {
                await host.StopAsync();
                host.Dispose();
            }

            if (databaseContainer != null)
            {
                await databaseContainer.DisposeAsync();
            }

            if (workingDirectory != null && Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, true);
            }
        }

        /// <summary>
        /// Creates a <see cref="CometRestClient"/> that is authenticated with basic authentication
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>The <see cref="CometRestClient"/></returns>
        public static CometRestClient CreateClient(string userName = AdminUserName, string password = AdminPassword)
        {
            return new CometRestClient(host.GetTestClient(), userName, password);
        }

        /// <summary>
        /// Restores the database to the seeded state, using the restore snapshot created by the seed
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        public static async Task RestoreAsync()
        {
            using var httpClient = host.GetTestClient();
            using var response = await httpClient.PostAsync("/Data/Restore", null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"The restore of the database failed: {await response.Content.ReadAsStringAsync()}");
        }

        /// <summary>
        /// Waits until the healthz endpoint of the REST API returns OK
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private static async Task WaitUntilHealthyAsync()
        {
            using var httpClient = host.GetTestClient();
            var deadline = DateTime.UtcNow + StartupTimeout;

            while (DateTime.UtcNow < deadline)
            {
                using var response = await httpClient.GetAsync("/healthz");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }

                await Task.Delay(500);
            }

            Assert.Fail($"The CDP4-COMET REST API did not become healthy within {StartupTimeout}");
        }

        /// <summary>
        /// Seeds the database with the Annex C.3 test data set, this also creates the restore snapshot
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private static async Task SeedAsync()
        {
            var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "Data.zip");

            using var httpClient = host.GetTestClient();
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(seedFilePath));
            content.Add(fileContent, "file", "Data.zip");

            using var response = await httpClient.PostAsync("/Data/Exchange", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"The seed of the database failed: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
