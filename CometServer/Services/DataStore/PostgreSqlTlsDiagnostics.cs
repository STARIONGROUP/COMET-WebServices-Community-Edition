// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlTlsDiagnostics.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="PostgreSqlTlsDiagnostics"/> is to explain why a TLS connection to the PostgreSQL server
    /// failed. Npgsql reports every rejected certificate with the same message, this class finds out which property of the
    /// certificate is the problem (issuer, host name, validity period, ...)
    /// </summary>
    public class PostgreSqlTlsDiagnostics : IPostgreSqlTlsDiagnostics
    {
        /// <summary>
        /// The PostgreSQL SSLRequest message: length 8 followed by the SSL request code 80877103
        /// </summary>
        private static readonly byte[] SslRequest = [0, 0, 0, 8, 0x04, 0xD2, 0x16, 0x2F];

        /// <summary>
        /// The maximum time the diagnostic handshake may take
        /// </summary>
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Performs a TLS handshake with the PostgreSQL server, without sending any credentials, and reports the
        /// server certificate and every problem found with it
        /// </summary>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/> that provides the host, port and TLS settings
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The findings, each one a human readable explanation
        /// </returns>
        public async Task<IReadOnlyList<string>> DiagnoseAsync(BacktierConfig backtierConfig, CancellationToken cancellationToken)
        {
            var findings = new List<string>();

            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(Timeout);

            X509Certificate2 serverCertificate = null;
            var presentedCertificates = new List<X509Certificate2>();

            try
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(backtierConfig.HostName, backtierConfig.Port, timeoutSource.Token);

                await using var networkStream = tcpClient.GetStream();
                await networkStream.WriteAsync(SslRequest, timeoutSource.Token);

                var response = new byte[1];
                var bytesRead = await networkStream.ReadAsync(response, timeoutSource.Token);

                if (bytesRead != 1 || response[0] != (byte)'S')
                {
                    findings.Add($"The PostgreSQL server at {backtierConfig.HostName}:{backtierConfig.Port} does not accept TLS connections. " +
                                 $"Enable TLS on the server (ssl = on in postgresql.conf), or set Backtier:SslMode to Prefer or Disable to connect without encryption");

                    return findings;
                }

                var sslClientAuthenticationOptions = new SslClientAuthenticationOptions
                {
                    TargetHost = backtierConfig.HostName,
                    RemoteCertificateValidationCallback = (_, certificate, chain, sslPolicyErrors) =>
                    {
                        // record the certificate so it can be explained, but do not accept an invalid one:
                        // the handshake then fails and the recorded certificate is evaluated in the catch below
                        if (certificate != null)
                        {
                            serverCertificate = X509CertificateLoader.LoadCertificate(certificate.GetRawCertData());
                        }

                        if (chain != null)
                        {
                            presentedCertificates.AddRange(chain.ChainElements.Select(element => X509CertificateLoader.LoadCertificate(element.Certificate.RawData)));
                        }

                        return sslPolicyErrors == SslPolicyErrors.None;
                    }
                };

                await using var sslStream = new SslStream(networkStream, false);
                await sslStream.AuthenticateAsClientAsync(sslClientAuthenticationOptions, timeoutSource.Token);

                findings.Add($"The PostgreSQL server negotiated {sslStream.SslProtocol} {sslStream.NegotiatedCipherSuite}");

                if (serverCertificate == null)
                {
                    findings.Add("The PostgreSQL server did not present a certificate");
                    return findings;
                }

                findings.AddRange(EvaluateCertificate(backtierConfig, serverCertificate, presentedCertificates, DateTime.Now));
            }
            catch (AuthenticationException) when (serverCertificate != null)
            {
                // the certificate was rejected by the TLS validation of the operating system, explain why
                findings.AddRange(EvaluateCertificate(backtierConfig, serverCertificate, presentedCertificates, DateTime.Now));
            }
            catch (AuthenticationException ex)
            {
                findings.Add($"The TLS handshake with {backtierConfig.HostName}:{backtierConfig.Port} failed before the certificate could be checked: {ex.Message} {ex.InnerException?.Message}. " +
                             "This usually means the client and server have no TLS version or cipher in common (see ssl_min_protocol_version and ssl_ciphers on the server)");
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                findings.Add($"The TLS diagnostics did not complete within {Timeout.TotalSeconds} seconds");
            }
            catch (Exception ex) when (ex is SocketException or IOException)
            {
                findings.Add($"The TLS diagnostics could not connect to {backtierConfig.HostName}:{backtierConfig.Port}: {ex.Message}");
            }

            return findings;
        }

        /// <summary>
        /// Checks the server certificate against the configured TLS settings in the same way Npgsql does,
        /// and explains every problem that is found
        /// </summary>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/> that provides the host name and TLS settings
        /// </param>
        /// <param name="serverCertificate">
        /// The certificate presented by the server
        /// </param>
        /// <param name="presentedCertificates">
        /// The other certificates presented by the server (intermediate certificates)
        /// </param>
        /// <param name="now">
        /// The current local time
        /// </param>
        /// <returns>
        /// The findings, the first one describes the certificate
        /// </returns>
        public static IReadOnlyList<string> EvaluateCertificate(BacktierConfig backtierConfig, X509Certificate2 serverCertificate, IEnumerable<X509Certificate2> presentedCertificates, DateTime now)
        {
            var findings = new List<string>();

            var names = GetCertificateNames(serverCertificate);

            findings.Add($"The PostgreSQL server presented the certificate '{serverCertificate.Subject}' issued by '{serverCertificate.Issuer}', " +
                         $"valid from {serverCertificate.NotBefore:yyyy-MM-dd HH:mm} to {serverCertificate.NotAfter:yyyy-MM-dd HH:mm}, " +
                         $"for the names [{string.Join(", ", names)}], thumbprint {serverCertificate.Thumbprint}");

            if (backtierConfig.SslMode is not (SslMode.VerifyCA or SslMode.VerifyFull))
            {
                findings.Add($"Backtier:SslMode is {backtierConfig.SslMode}, the server certificate is not verified");
                return findings;
            }

            if (now < serverCertificate.NotBefore)
            {
                findings.Add($"PROBLEM: the server certificate is not valid yet, it is valid from {serverCertificate.NotBefore:yyyy-MM-dd HH:mm}. Check the clock of both machines");
            }

            if (now > serverCertificate.NotAfter)
            {
                findings.Add($"PROBLEM: the server certificate expired on {serverCertificate.NotAfter:yyyy-MM-dd HH:mm}. Renew the certificate of the PostgreSQL server");
            }

            findings.AddRange(EvaluateChain(backtierConfig, serverCertificate, presentedCertificates));

            if (backtierConfig.SslMode == SslMode.VerifyFull && !serverCertificate.MatchesHostname(backtierConfig.HostName))
            {
                findings.Add($"PROBLEM: the server certificate is not valid for the host name '{backtierConfig.HostName}' configured in Backtier:HostName. " +
                             $"It is valid for [{string.Join(", ", names)}]. Connect using one of these names, or issue a certificate that includes '{backtierConfig.HostName}'");
            }

            if (!findings.Any(finding => finding.StartsWith("PROBLEM:", StringComparison.Ordinal)))
            {
                findings.Add("No problem was found with the server certificate");
            }

            return findings;
        }

        /// <summary>
        /// Builds the certificate chain using the configured <see cref="BacktierConfig.RootCertificate"/>, or the trust store
        /// of the operating system when none is configured, and explains why the chain is not trusted
        /// </summary>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/>
        /// </param>
        /// <param name="serverCertificate">
        /// The certificate presented by the server
        /// </param>
        /// <param name="presentedCertificates">
        /// The other certificates presented by the server
        /// </param>
        /// <returns>
        /// The findings about the chain
        /// </returns>
        private static List<string> EvaluateChain(BacktierConfig backtierConfig, X509Certificate2 serverCertificate, IEnumerable<X509Certificate2> presentedCertificates)
        {
            var findings = new List<string>();

            using var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = backtierConfig.CheckCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck;

            foreach (var presentedCertificate in presentedCertificates.Where(x => x.Thumbprint != serverCertificate.Thumbprint))
            {
                chain.ChainPolicy.ExtraStore.Add(presentedCertificate);
            }

            string trustSource;

            var rootCertificate = backtierConfig.GetEffectiveRootCertificate();

            if (string.IsNullOrWhiteSpace(rootCertificate.Path))
            {
                trustSource = "the trust store of the operating system (Backtier:RootCertificate and PGSSLROOTCERT are not set)";
            }
            else
            {
                X509Certificate2Collection rootCertificates;

                try
                {
                    rootCertificates = BacktierConfig.LoadRootCertificates(rootCertificate.Path);
                }
                catch (Exception ex) when (ex is CryptographicException or IOException or UnauthorizedAccessException)
                {
                    findings.Add($"PROBLEM: the file '{rootCertificate.Path}' configured in {rootCertificate.Source} could not be read as a PEM or DER certificate: {ex.Message}");
                    return findings;
                }

                if (rootCertificates.Count == 0)
                {
                    findings.Add($"PROBLEM: the file '{rootCertificate.Path}' configured in {rootCertificate.Source} contains no certificate (PEM or DER)");
                    return findings;
                }

                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.AddRange(rootCertificates);

                trustSource = $"{rootCertificate.Source} '{rootCertificate.Path}' containing [{string.Join(", ", rootCertificates.Select(x => $"'{x.Subject}'"))}]";
            }

            if (chain.Build(serverCertificate))
            {
                return findings;
            }

            // the validity of the server certificate itself is reported by EvaluateCertificate, here only the CA certificates
            var expiredCertificateAuthorities = chain.ChainElements.Skip(1)
                .Where(element => element.ChainElementStatus.Any(status => status.Status == X509ChainStatusFlags.NotTimeValid))
                .Select(element => element.Certificate);

            foreach (var expiredCertificateAuthority in expiredCertificateAuthorities)
            {
                findings.Add($"PROBLEM: the CA certificate '{expiredCertificateAuthority.Subject}' in the chain of the server certificate is not valid now, " +
                             $"it is valid from {expiredCertificateAuthority.NotBefore:yyyy-MM-dd HH:mm} to {expiredCertificateAuthority.NotAfter:yyyy-MM-dd HH:mm}. " +
                             "Renew the CA certificate, both on the PostgreSQL server and in the configured CA file");
            }

            var statusFlags = chain.ChainStatus.Aggregate(X509ChainStatusFlags.NoError, (flags, status) => flags | status.Status);

            if (statusFlags.HasFlag(X509ChainStatusFlags.UntrustedRoot) || statusFlags.HasFlag(X509ChainStatusFlags.PartialChain))
            {
                findings.Add($"PROBLEM: the server certificate issued by '{serverCertificate.Issuer}' is not trusted by {trustSource}. " +
                             "Configure the CA that issued the server certificate in Backtier:RootCertificate, or include missing intermediate certificates in ssl_cert_file on the server");
            }

            if (statusFlags.HasFlag(X509ChainStatusFlags.Revoked))
            {
                findings.Add("PROBLEM: the server certificate has been revoked");
            }

            if (statusFlags.HasFlag(X509ChainStatusFlags.RevocationStatusUnknown) || statusFlags.HasFlag(X509ChainStatusFlags.OfflineRevocation))
            {
                findings.Add("PROBLEM: the revocation status of the server certificate could not be checked. Make sure the CRL/OCSP endpoints are reachable, or set Backtier:CheckCertificateRevocation to false");
            }

            var otherStatuses = chain.ChainStatus
                .Where(x => x.Status is not (X509ChainStatusFlags.UntrustedRoot or X509ChainStatusFlags.PartialChain or X509ChainStatusFlags.Revoked
                    or X509ChainStatusFlags.RevocationStatusUnknown or X509ChainStatusFlags.OfflineRevocation or X509ChainStatusFlags.NotTimeValid))
                .Select(x => $"{x.Status}: {x.StatusInformation.Trim()}")
                .Distinct()
                .ToList();

            if (otherStatuses.Count > 0)
            {
                findings.Add($"PROBLEM: the certificate chain is not valid: {string.Join("; ", otherStatuses)}");
            }

            return findings;
        }

        /// <summary>
        /// Gets the DNS names and IP addresses the certificate is valid for, or the common name when there are none
        /// </summary>
        /// <param name="certificate">
        /// The certificate
        /// </param>
        /// <returns>
        /// The names
        /// </returns>
        private static List<string> GetCertificateNames(X509Certificate2 certificate)
        {
            var names = new List<string>();

            foreach (var subjectAlternativeName in certificate.Extensions.OfType<X509SubjectAlternativeNameExtension>())
            {
                names.AddRange(subjectAlternativeName.EnumerateDnsNames());
                names.AddRange(subjectAlternativeName.EnumerateIPAddresses().Select(x => x.ToString()));
            }

            if (names.Count == 0)
            {
                names.Add(certificate.GetNameInfo(X509NameType.SimpleName, false));
            }

            return names;
        }
    }
}
