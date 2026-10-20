// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCertificateFactory.cs" company="Starion Group S.A.">
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
    using System.Net;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Creates certificates for tests: certificate authorities, intermediate CAs, server and client certificates
    /// </summary>
    public static class TestCertificateFactory
    {
        /// <summary>
        /// Creates a self-signed certificate authority, including its private key
        /// </summary>
        /// <param name="subject">
        /// The subject, e.g. CN=COMET Test CA
        /// </param>
        /// <returns>
        /// The certificate authority
        /// </returns>
        public static X509Certificate2 CreateCertificateAuthority(string subject)
        {
            var key = RSA.Create(2048);

            var request = new CertificateRequest(subject, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            return request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-30), DateTimeOffset.Now.AddDays(30));
        }

        /// <summary>
        /// Creates a certificate signed by the issuer, including its private key. The validity period is not checked against
        /// the validity of the issuer, so a valid certificate issued by an expired CA can be created
        /// </summary>
        /// <param name="issuer">
        /// The issuing certificate authority, must have a private key
        /// </param>
        /// <param name="subject">
        /// The subject, e.g. CN=comet-db
        /// </param>
        /// <param name="notBefore">
        /// The start of the validity period
        /// </param>
        /// <param name="notAfter">
        /// The end of the validity period
        /// </param>
        /// <param name="isCertificateAuthority">
        /// true to create an (intermediate) certificate authority
        /// </param>
        /// <param name="dnsNames">
        /// The DNS names for the subject alternative name extension
        /// </param>
        /// <param name="ipAddresses">
        /// The IP addresses for the subject alternative name extension
        /// </param>
        /// <returns>
        /// The certificate
        /// </returns>
        public static X509Certificate2 CreateCertificate(X509Certificate2 issuer, string subject, DateTimeOffset notBefore, DateTimeOffset notAfter, bool isCertificateAuthority = false, string[] dnsNames = null, IPAddress[] ipAddresses = null)
        {
            var key = RSA.Create(2048);

            var request = new CertificateRequest(subject, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(isCertificateAuthority, false, 0, isCertificateAuthority));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
            request.CertificateExtensions.Add(X509AuthorityKeyIdentifierExtension.CreateFromCertificate(issuer, true, false));

            if (isCertificateAuthority)
            {
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            }

            if (dnsNames?.Length > 0 || ipAddresses?.Length > 0)
            {
                var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();

                foreach (var dnsName in dnsNames ?? [])
                {
                    subjectAlternativeNameBuilder.AddDnsName(dnsName);
                }

                foreach (var ipAddress in ipAddresses ?? [])
                {
                    subjectAlternativeNameBuilder.AddIpAddress(ipAddress);
                }

                request.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build());
            }

            var serialNumber = new byte[8];
            RandomNumberGenerator.Fill(serialNumber);

            var signatureGenerator = X509SignatureGenerator.CreateForRSA(issuer.GetRSAPrivateKey(), RSASignaturePadding.Pkcs1);

            using var certificate = request.Create(issuer.SubjectName, signatureGenerator, notBefore, notAfter, serialNumber);

            return certificate.CopyWithPrivateKey(key);
        }

        /// <summary>
        /// Creates a server certificate valid from yesterday until tomorrow
        /// </summary>
        /// <param name="issuer">
        /// The issuing certificate authority
        /// </param>
        /// <param name="dnsNames">
        /// The DNS names, the first one is also used as common name
        /// </param>
        /// <returns>
        /// The certificate
        /// </returns>
        public static X509Certificate2 CreateServerCertificate(X509Certificate2 issuer, params string[] dnsNames)
        {
            return CreateCertificate(issuer, $"CN={dnsNames[0]}", DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1), dnsNames: dnsNames);
        }

        /// <summary>
        /// Loads a copy of the certificate from PFX data, so that its private key can be used by an <see cref="System.Net.Security.SslStream"/>
        /// on every platform
        /// </summary>
        /// <param name="certificate">
        /// The certificate with a private key
        /// </param>
        /// <returns>
        /// The copy
        /// </returns>
        public static X509Certificate2 ToPfxLoaded(X509Certificate2 certificate)
        {
            return X509CertificateLoader.LoadPkcs12(certificate.Export(X509ContentType.Pfx), null);
        }
    }
}
