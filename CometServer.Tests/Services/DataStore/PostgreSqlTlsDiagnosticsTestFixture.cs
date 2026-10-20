// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlTlsDiagnosticsTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.DataStore
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Services.DataStore;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="PostgreSqlTlsDiagnostics"/>
    /// </summary>
    [TestFixture]
    public class PostgreSqlTlsDiagnosticsTestFixture
    {
        private static readonly byte[] ExpectedSslRequest = [0, 0, 0, 8, 0x04, 0xD2, 0x16, 0x2F];

        private string tempDirectory;

        private X509Certificate2 certificateAuthority;

        private X509Certificate2 otherCertificateAuthority;

        private string certificateAuthorityFile;

        private string otherCertificateAuthorityFile;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(this.tempDirectory);

            this.certificateAuthority = TestCertificateFactory.CreateCertificateAuthority("CN=COMET Test CA");
            this.otherCertificateAuthority = TestCertificateFactory.CreateCertificateAuthority("CN=Other CA");

            this.certificateAuthorityFile = this.WriteFile("ca.crt", this.certificateAuthority.ExportCertificatePem());
            this.otherCertificateAuthorityFile = this.WriteFile("other-ca.crt", this.otherCertificateAuthority.ExportCertificatePem());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.certificateAuthority.Dispose();
            this.otherCertificateAuthority.Dispose();
            Directory.Delete(this.tempDirectory, true);
        }

        [Test]
        public void Verify_that_a_valid_certificate_has_no_problems()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db", "localhost");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(findings[0], Does.Contain("CN=comet-db").And.Contain("CN=COMET Test CA").And.Contain("[comet-db, localhost]"));
                Assert.That(findings, Has.None.StartsWith("PROBLEM:"));
                Assert.That(findings[^1], Is.EqualTo("No problem was found with the server certificate"));
            });
        }

        [Test]
        public void Verify_that_a_certificate_from_another_CA_is_reported_with_both_issuers()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.otherCertificateAuthorityFile), serverCertificate, [], DateTime.Now);

            var problems = Problems(findings);

            Assert.Multiple(() =>
            {
                Assert.That(problems, Has.Count.EqualTo(1));
                Assert.That(problems[0], Does.Contain("is not trusted by Backtier:RootCertificate").And.Contain("'CN=COMET Test CA'").And.Contain("'CN=Other CA'"));
            });
        }

        [Test]
        public void Verify_that_an_intermediate_presented_by_the_server_completes_the_chain()
        {
            using var intermediate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=COMET Intermediate CA", DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddDays(10), isCertificateAuthority: true);
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(intermediate, "comet-db");

            var config = CreateConfig("comet-db", this.certificateAuthorityFile);

            var withIntermediate = PostgreSqlTlsDiagnostics.EvaluateCertificate(config, serverCertificate, [serverCertificate, intermediate], DateTime.Now);
            var withoutIntermediate = PostgreSqlTlsDiagnostics.EvaluateCertificate(config, serverCertificate, [], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(withIntermediate, Has.None.StartsWith("PROBLEM:"));
                Assert.That(Problems(withoutIntermediate), Has.One.Contains("is not trusted").And.Contains("intermediate certificates"));
            });
        }

        [Test]
        public void Verify_that_an_expired_CA_is_reported_even_when_the_server_certificate_is_valid()
        {
            using var expiredIntermediate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=Expired Intermediate CA", DateTimeOffset.Now.AddDays(-20), DateTimeOffset.Now.AddDays(-2), isCertificateAuthority: true);
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(expiredIntermediate, "comet-db");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.certificateAuthorityFile), serverCertificate, [expiredIntermediate], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(Problems(findings), Has.Some.Contains("the CA certificate 'CN=Expired Intermediate CA'").And.Contains("is not valid now"));
                Assert.That(findings, Has.None.EqualTo("No problem was found with the server certificate"));
            });
        }

        [Test]
        public void Verify_that_without_a_root_certificate_the_operating_system_trust_store_is_reported()
        {
            Assume.That(Environment.GetEnvironmentVariable("PGSSLROOTCERT"), Is.Null.Or.Empty, "PGSSLROOTCERT is set on this machine");

            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var config = CreateConfig("comet-db", null);
            Assume.That(config.GetEffectiveRootCertificate().Path, Is.Null, "a default root.crt exists on this machine");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(config, serverCertificate, [], DateTime.Now);

            Assert.That(Problems(findings), Has.One.Contains("the trust store of the operating system"));
        }

        [Test]
        public void Verify_that_a_root_certificate_file_without_certificate_is_reported()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.WriteFile("not-a-certificate.crt", "this is not a certificate")), serverCertificate, [], DateTime.Now);

            Assert.That(Problems(findings), Has.One.Contains("could not be read as a PEM or DER certificate"));
        }

        [Test]
        public void Verify_that_a_root_certificate_file_with_a_corrupt_PEM_block_is_reported()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var corruptFile = this.WriteFile("corrupt.crt", "-----BEGIN CERTIFICATE-----\nbm90IGEgY2VydGlmaWNhdGU=\n-----END CERTIFICATE-----\n");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", corruptFile), serverCertificate, [], DateTime.Now);

            Assert.That(Problems(findings), Has.One.Contains(corruptFile));
        }

        [Test]
        public void Verify_that_a_host_name_mismatch_lists_the_valid_names()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db", "localhost");

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("127.0.0.1", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            var problems = Problems(findings);

            Assert.Multiple(() =>
            {
                Assert.That(problems, Has.Count.EqualTo(1));
                Assert.That(problems[0], Does.Contain("not valid for the host name '127.0.0.1'").And.Contain("[comet-db, localhost]"));
            });
        }

        [Test]
        public void Verify_that_an_IP_address_in_the_certificate_matches_the_host_name()
        {
            using var serverCertificate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=comet-db", DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1), dnsNames: ["comet-db"], ipAddresses: [IPAddress.Loopback]);

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("127.0.0.1", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(findings, Has.None.StartsWith("PROBLEM:"));
                Assert.That(findings[0], Does.Contain("[comet-db, 127.0.0.1]"));
            });
        }

        [Test]
        public void Verify_that_the_common_name_is_listed_when_there_is_no_subject_alternative_name()
        {
            using var serverCertificate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=comet-db", DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(findings[0], Does.Contain("[comet-db]"));
                Assert.That(findings, Has.None.StartsWith("PROBLEM:"));
            });
        }

        [Test]
        public void Verify_that_a_host_name_mismatch_is_not_a_problem_with_VerifyCA()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var backtierConfig = CreateConfig("127.0.0.1", this.certificateAuthorityFile);
            backtierConfig.SslMode = SslMode.VerifyCA;

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(backtierConfig, serverCertificate, [], DateTime.Now);

            Assert.That(findings, Has.None.StartsWith("PROBLEM:"));
        }

        [Test]
        public void Verify_that_an_expired_certificate_is_reported()
        {
            using var serverCertificate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=comet-db", DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddDays(-2), dnsNames: ["comet-db"]);

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            var problems = Problems(findings);

            Assert.Multiple(() =>
            {
                Assert.That(problems, Has.Count.EqualTo(1));
                Assert.That(problems[0], Does.Contain("expired on"));
            });
        }

        [Test]
        public void Verify_that_a_certificate_that_is_not_valid_yet_is_reported()
        {
            using var serverCertificate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=comet-db", DateTimeOffset.Now.AddDays(2), DateTimeOffset.Now.AddDays(5), dnsNames: ["comet-db"]);

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(CreateConfig("comet-db", this.certificateAuthorityFile), serverCertificate, [], DateTime.Now);

            Assert.That(findings, Has.One.StartsWith("PROBLEM: the server certificate is not valid yet"));
        }

        [Test]
        public void Verify_that_an_unknown_revocation_status_is_reported_when_revocation_is_checked()
        {
            using var serverCertificate = TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "comet-db");

            var backtierConfig = CreateConfig("comet-db", this.certificateAuthorityFile);
            backtierConfig.CheckCertificateRevocation = true;

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(backtierConfig, serverCertificate, [], DateTime.Now);

            Assert.That(Problems(findings), Has.One.Contains("revocation status of the server certificate could not be checked"));
        }

        [Test]
        public void Verify_that_the_certificate_is_not_verified_with_Require()
        {
            using var serverCertificate = TestCertificateFactory.CreateCertificate(this.otherCertificateAuthority, "CN=other-host", DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddDays(-2));

            var backtierConfig = CreateConfig("comet-db", this.certificateAuthorityFile);
            backtierConfig.SslMode = SslMode.Require;

            var findings = PostgreSqlTlsDiagnostics.EvaluateCertificate(backtierConfig, serverCertificate, [], DateTime.Now);

            Assert.Multiple(() =>
            {
                Assert.That(findings, Has.None.StartsWith("PROBLEM:"));
                Assert.That(findings, Has.One.Contains("the server certificate is not verified"));
            });
        }

        [Test]
        public async Task Verify_that_a_server_that_refuses_TLS_is_reported_and_receives_the_SSLRequest()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var receivedRequest = new byte[8];

            var serverTask = Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                await stream.ReadExactlyAsync(receivedRequest);
                await stream.WriteAsync("N"u8.ToArray());
            });

            var findings = await new PostgreSqlTlsDiagnostics().DiagnoseAsync(CreateConfig("localhost", this.certificateAuthorityFile, Port(listener)), CancellationToken.None);

            await serverTask;

            Assert.Multiple(() =>
            {
                Assert.That(receivedRequest, Is.EqualTo(ExpectedSslRequest));
                Assert.That(findings, Has.Count.EqualTo(1));
                Assert.That(findings[0], Does.Contain("does not accept TLS connections"));
            });
        }

        [Test]
        public async Task Verify_that_a_closed_port_is_reported()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = Port(listener);
            listener.Stop();

            var findings = await new PostgreSqlTlsDiagnostics().DiagnoseAsync(CreateConfig("127.0.0.1", this.certificateAuthorityFile, port), CancellationToken.None);

            Assert.That(findings, Has.One.StartsWith("The TLS diagnostics could not connect to"));
        }

        [Test]
        public async Task Verify_that_the_TLS_handshake_collects_and_evaluates_the_server_certificate()
        {
            using var serverCertificate = TestCertificateFactory.ToPfxLoaded(TestCertificateFactory.CreateServerCertificate(this.certificateAuthority, "localhost"));

            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var serverTask = Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                await stream.ReadExactlyAsync(new byte[8]);
                await stream.WriteAsync("S"u8.ToArray());

                await using var sslStream = new SslStream(stream, false);
                await sslStream.AuthenticateAsServerAsync(serverCertificate);
            });

            var findings = await new PostgreSqlTlsDiagnostics().DiagnoseAsync(CreateConfig("localhost", this.otherCertificateAuthorityFile, Port(listener)), CancellationToken.None);

            await serverTask;

            Assert.Multiple(() =>
            {
                // the certificate is not trusted, so the handshake is rejected and the recorded certificate is evaluated
                Assert.That(findings, Has.None.StartsWith("The PostgreSQL server negotiated"));
                Assert.That(findings, Has.None.Contains("failed before the certificate could be checked"));
                Assert.That(findings, Has.Some.Contains("presented the certificate 'CN=localhost' issued by 'CN=COMET Test CA'"));
                Assert.That(Problems(findings), Has.One.Contains("is not trusted by Backtier:RootCertificate"));
            });
        }

        private static BacktierConfig CreateConfig(string hostName, string rootCertificate, int port = 5432)
        {
            return new BacktierConfig
            {
                HostName = hostName,
                Port = port,
                SslMode = SslMode.VerifyFull,
                RootCertificate = rootCertificate
            };
        }

        private static System.Collections.Generic.List<string> Problems(System.Collections.Generic.IEnumerable<string> findings)
        {
            return findings.Where(x => x.StartsWith("PROBLEM:", StringComparison.Ordinal)).ToList();
        }

        private static int Port(TcpListener listener)
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }

        private string WriteFile(string name, string content)
        {
            var path = Path.Combine(this.tempDirectory, name);
            File.WriteAllText(path, content);
            return path;
        }
    }
}
