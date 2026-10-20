// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BacktierConfig.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.Extensions.Configuration;

    using Npgsql;

    /// <summary>
    /// The back tier configuration.
    /// </summary>
    public class BacktierConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BacktierConfig"/> class.
        /// </summary>
        public BacktierConfig()
        {
            // set defaults
            this.HostName = "127.0.0.1";
            this.Port = 5432;
            this.UserName = "cdp4";
            this.Password = "cdp4";
            this.Database = "cdp4server";
            this.DatabaseRestore = "cdp4serverrestore";
            this.DatabaseManage = "cdp4manage";
            this.StatementTimeout = 120;
            this.ConnectionTimeout = 15;
            this.Keepalive = 0; // No keepalive by default
            this.MaximumPoolSize = 100; // Default maximum pool size
            this.IsDbImportEnabled = false;
            this.IsDbSeedEnabled = false;
            this.IsDbRestoreEnabled = false;
            this.SslMode = SslMode.Prefer;
            this.CheckCertificateRevocation = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BacktierConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public BacktierConfig(IConfiguration configuration)
        {
            this.HostName = configuration["Backtier:HostName"];

            if (int.TryParse(configuration["Backtier:Port"], out var port))
            {
                this.Port = port;
            }

            this.UserName = configuration["Backtier:UserName"];
            this.Password = configuration["Backtier:Password"];
            this.Database = configuration["Backtier:Database"];
            this.DatabaseRestore = configuration["Backtier:DatabaseRestore"];
            this.DatabaseManage = configuration["Backtier:DatabaseManage"];

            if (int.TryParse(configuration["Backtier:ConnectionTimeout"], out var connectionTimeout))
            {
                this.ConnectionTimeout = connectionTimeout;
            }

            if (int.TryParse(configuration["Backtier:Keepalive"], out var keepAlive))
            {
                this.Keepalive = keepAlive;
            }

            if (int.TryParse(configuration["Backtier:MaximumPoolSize"], out var maximumPoolSize))
            {
                this.MaximumPoolSize = maximumPoolSize;
            }

            if (int.TryParse(configuration["Backtier:StatementTimeout"], out var statementTimeout))
            {
                this.StatementTimeout = statementTimeout;
            }

            if (bool.TryParse(configuration["Backtier:IsDbImportEnabled"], out var isDbImportEnabled))
            {
                this.IsDbImportEnabled = isDbImportEnabled;
            }

            if (bool.TryParse(configuration["Backtier:IsDbSeedEnabled"], out var isDbSeedEnabled))
            {
                this.IsDbSeedEnabled = isDbSeedEnabled;
            }

            if (bool.TryParse(configuration["Backtier:IsDbRestoreEnabled"], out var isDbRestoreEnabled))
            {
                this.IsDbRestoreEnabled = isDbRestoreEnabled;
            }

            this.SslMode = ParseSslMode(configuration["Backtier:SslMode"]);

            this.RootCertificate = configuration["Backtier:RootCertificate"];
            this.ClientCertificate = configuration["Backtier:ClientCertificate"];
            this.ClientCertificateKey = configuration["Backtier:ClientCertificateKey"];
            this.ClientCertificatePassword = configuration["Backtier:ClientCertificatePassword"];

            this.CheckCertificateRevocation = ParseCheckCertificateRevocation(configuration["Backtier:CheckCertificateRevocation"]);
        }

        /// <summary>
        /// Gets or sets the host name of the back tier.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the back tier.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password of the back tier.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier for a restore database.
        /// </summary>
        public string DatabaseRestore { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier for a manage database to control other databases from.
        /// </summary>
        public string DatabaseManage { get; set; }

        /// <summary>
        /// Gets or sets the time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error. Set to zero for infinity.
        /// </summary>
        /// <remarks>
        /// Default is 120 seconds
        /// </remarks>
        public int StatementTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time to wait (in seconds) while trying to get a connection from the connection pool before terminating the attempt and generating an error.
        /// </summary>
        /// <remarks>
        /// Default is 15 seconds
        /// </remarks>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds of connection inactivity before Npgsql sends a keepalive query.
        /// </summary>
        /// <remarks>
        /// Default is 0 seconds
        /// </remarks>
        public int Keepalive { get; set; }

        /// <summary>
        /// Gets or sets the maximum connection pool size.
        /// </summary>
        /// <remarks>
        /// Default is 100
        /// </remarks>
        public int MaximumPoolSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether database import is enabled.
        /// </summary>
        public bool IsDbImportEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether database seed is enabled.
        /// </summary>
        public bool IsDbSeedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether database restore is enabled.
        /// </summary>
        public bool IsDbRestoreEnabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Npgsql.SslMode"/> used for the connection to the PostgreSQL server.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="SslMode.Prefer"/>: encrypted when the server supports it, the server certificate is not verified.
        /// Use <see cref="SslMode.VerifyFull"/> to encrypt and verify both the certificate chain and the host name.
        /// </remarks>
        public SslMode SslMode { get; set; }

        /// <summary>
        /// Gets or sets the path to the (PEM) CA certificate used to verify the certificate of the PostgreSQL server.
        /// When empty, the trust store of the operating system is used.
        /// </summary>
        public string RootCertificate { get; set; }

        /// <summary>
        /// Gets or sets the path to the optional client certificate used to authenticate against the PostgreSQL server.
        /// </summary>
        public string ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets the path to the (PEM) private key of the <see cref="ClientCertificate"/>.
        /// Not needed when the <see cref="ClientCertificate"/> is a PFX file that contains the key.
        /// </summary>
        public string ClientCertificateKey { get; set; }

        /// <summary>
        /// Gets or sets the password of the <see cref="ClientCertificate"/> or its <see cref="ClientCertificateKey"/>.
        /// </summary>
        public string ClientCertificatePassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the revocation status of the server certificate is checked.
        /// </summary>
        /// <remarks>
        /// Default is false
        /// </remarks>
        public bool CheckCertificateRevocation { get; set; }

        /// <summary>
        /// Validates the TLS related settings that can only be checked at runtime, such as the existence of the certificate files.
        /// </summary>
        /// <returns>
        /// The validation errors, empty when the settings are valid
        /// </returns>
        public IEnumerable<string> ValidateTlsSettings()
        {
            var errors = new List<string>();

            var rootCertificate = this.GetEffectiveRootCertificate();
            var isRootCertificateReadable = AddFileError(errors, rootCertificate.Source, rootCertificate.Path);
            var isClientCertificateReadable = AddFileError(errors, $"Backtier:{nameof(this.ClientCertificate)}", this.ClientCertificate);
            var isClientCertificateKeyReadable = AddFileError(errors, $"Backtier:{nameof(this.ClientCertificateKey)}", this.ClientCertificateKey);

            if (!string.IsNullOrWhiteSpace(this.ClientCertificateKey) && string.IsNullOrWhiteSpace(this.ClientCertificate))
            {
                errors.Add($"Backtier:{nameof(this.ClientCertificateKey)} is set, but Backtier:{nameof(this.ClientCertificate)} is not");
            }

            if (isRootCertificateReadable && !string.IsNullOrWhiteSpace(rootCertificate.Path))
            {
                AddRootCertificateError(errors, rootCertificate.Source, rootCertificate.Path);
            }

            if (isClientCertificateReadable && isClientCertificateKeyReadable && !string.IsNullOrWhiteSpace(this.ClientCertificate))
            {
                this.AddClientCertificateError(errors);
            }

            return errors;
        }

        /// <summary>
        /// Gets the CA certificate file that Npgsql uses to verify the PostgreSQL server: <see cref="RootCertificate"/>, otherwise
        /// the PGSSLROOTCERT environment variable, otherwise the default root.crt of the user when that file exists
        /// </summary>
        /// <returns>
        /// The path (null when the trust store of the operating system is used) and a description of where it comes from
        /// </returns>
        public (string Path, string Source) GetEffectiveRootCertificate()
        {
            var defaultRootCertificate = OperatingSystem.IsWindows()
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "postgresql", "root.crt")
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".postgresql", "root.crt");

            return this.GetEffectiveRootCertificate(Environment.GetEnvironmentVariable("PGSSLROOTCERT"), defaultRootCertificate);
        }

        /// <summary>
        /// Gets the CA certificate file that Npgsql uses to verify the PostgreSQL server
        /// </summary>
        /// <param name="environmentRootCertificate">
        /// The value of the PGSSLROOTCERT environment variable
        /// </param>
        /// <param name="defaultRootCertificate">
        /// The default root.crt location of the user, used when it exists
        /// </param>
        /// <returns>
        /// The path (null when the trust store of the operating system is used) and a description of where it comes from
        /// </returns>
        internal (string Path, string Source) GetEffectiveRootCertificate(string environmentRootCertificate, string defaultRootCertificate)
        {
            if (!string.IsNullOrWhiteSpace(this.RootCertificate))
            {
                return (this.RootCertificate, $"Backtier:{nameof(this.RootCertificate)}");
            }

            if (!string.IsNullOrWhiteSpace(environmentRootCertificate))
            {
                return (environmentRootCertificate, "the PGSSLROOTCERT environment variable");
            }

            if (!string.IsNullOrWhiteSpace(defaultRootCertificate) && File.Exists(defaultRootCertificate))
            {
                return (defaultRootCertificate, "the default root.crt location");
            }

            return (null, "the trust store of the operating system");
        }

        /// <summary>
        /// Loads the CA certificates from a PEM file, or a single certificate from a DER file
        /// </summary>
        /// <param name="path">
        /// The path of the file
        /// </param>
        /// <returns>
        /// The certificates, empty when the file contains none
        /// </returns>
        /// <exception cref="CryptographicException">
        /// Thrown when the file contains a PEM block that is not a valid certificate
        /// </exception>
        public static X509Certificate2Collection LoadRootCertificates(string path)
        {
            var certificates = new X509Certificate2Collection();
            certificates.ImportFromPemFile(path);

            if (certificates.Count == 0 && !File.ReadAllText(path).Contains("-----BEGIN", StringComparison.Ordinal))
            {
                certificates.Add(X509CertificateLoader.LoadCertificateFromFile(path));
            }

            return certificates;
        }

        /// <summary>
        /// Parses the configured <see cref="Npgsql.SslMode"/>
        /// </summary>
        /// <param name="value">
        /// The configured value, the default <see cref="SslMode.Prefer"/> is used when empty
        /// </param>
        /// <returns>
        /// The parsed <see cref="Npgsql.SslMode"/>
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Thrown when the value is not a valid <see cref="Npgsql.SslMode"/>. An invalid value must not silently fall back to
        /// a mode that is less secure than the one intended.
        /// </exception>
        private static SslMode ParseSslMode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return SslMode.Prefer;
            }

            // match on the exact name only: Enum.TryParse also accepts numbers and comma separated
            // combinations (e.g. "Allow, Prefer" equals Require), which would hide configuration mistakes
            var sslModeName = Enum.GetNames<SslMode>().SingleOrDefault(name => string.Equals(name, value.Trim(), StringComparison.OrdinalIgnoreCase));

            if (sslModeName != null)
            {
                return Enum.Parse<SslMode>(sslModeName);
            }

            throw new ConfigurationErrorsException($"The Backtier:SslMode setting '{value}' is invalid, valid values are: {string.Join(", ", Enum.GetNames<SslMode>())}");
        }

        /// <summary>
        /// Parses the configured CheckCertificateRevocation setting
        /// </summary>
        /// <param name="value">
        /// The configured value, false is used when empty
        /// </param>
        /// <returns>
        /// The parsed value
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Thrown when the value is not true or false, an invalid value must not silently disable the check
        /// </exception>
        private static bool ParseCheckCertificateRevocation(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (bool.TryParse(value.Trim(), out var checkCertificateRevocation))
            {
                return checkCertificateRevocation;
            }

            throw new ConfigurationErrorsException($"The Backtier:CheckCertificateRevocation setting '{value}' is invalid, valid values are: true, false");
        }

        /// <summary>
        /// Adds an error to the list of errors when a configured file does not exist or cannot be read
        /// </summary>
        /// <param name="errors">
        /// The list of errors
        /// </param>
        /// <param name="source">
        /// Where the path is configured, for instance Backtier:RootCertificate
        /// </param>
        /// <param name="path">
        /// The configured path, nothing is checked when empty
        /// </param>
        /// <returns>
        /// true when the path is empty or the file can be read, false when an error was added
        /// </returns>
        private static bool AddFileError(List<string> errors, string source, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return true;
            }

            if (!File.Exists(path))
            {
                errors.Add($"The file '{path}' configured in {source} does not exist");
                return false;
            }

            try
            {
                using var stream = File.OpenRead(path);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                errors.Add($"The file '{path}' configured in {source} cannot be read: {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds an error to the list of errors when the CA certificate file does not contain a valid certificate
        /// </summary>
        /// <param name="errors">
        /// The list of errors
        /// </param>
        /// <param name="source">
        /// Where the path is configured
        /// </param>
        /// <param name="path">
        /// The path of the CA certificate file
        /// </param>
        private static void AddRootCertificateError(List<string> errors, string source, string path)
        {
            try
            {
                if (LoadRootCertificates(path).Count == 0)
                {
                    errors.Add($"The file '{path}' configured in {source} contains no certificate (PEM or DER)");
                }
            }
            catch (CryptographicException ex)
            {
                errors.Add($"The file '{path}' configured in {source} is not a valid PEM or DER certificate: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds an error to the list of errors when the client certificate cannot be loaded together with its private key,
        /// either from PEM files (certificate and key, optionally encrypted) or from a PFX file
        /// </summary>
        /// <param name="errors">
        /// The list of errors
        /// </param>
        private void AddClientCertificateError(List<string> errors)
        {
            var isPem = File.ReadAllText(this.ClientCertificate).Contains("-----BEGIN", StringComparison.Ordinal);
            var keyPath = string.IsNullOrWhiteSpace(this.ClientCertificateKey) ? null : this.ClientCertificateKey;
            var password = string.IsNullOrEmpty(this.ClientCertificatePassword) ? null : this.ClientCertificatePassword;

            try
            {
                X509Certificate2 clientCertificate;

                if (isPem || keyPath != null)
                {
                    clientCertificate = password == null
                        ? X509Certificate2.CreateFromPemFile(this.ClientCertificate, keyPath)
                        : X509Certificate2.CreateFromEncryptedPemFile(this.ClientCertificate, password, keyPath);
                }
                else
                {
                    clientCertificate = X509CertificateLoader.LoadPkcs12FromFile(this.ClientCertificate, password);
                }

                using (clientCertificate)
                {
                    if (!clientCertificate.HasPrivateKey)
                    {
                        errors.Add($"The client certificate '{this.ClientCertificate}' configured in Backtier:{nameof(this.ClientCertificate)} has no private key. Set Backtier:{nameof(this.ClientCertificateKey)}, or use a PFX file that contains the key");
                    }
                }
            }
            catch (Exception ex) when (ex is CryptographicException or ArgumentException)
            {
                errors.Add($"The client certificate '{this.ClientCertificate}' configured in Backtier:{nameof(this.ClientCertificate)} could not be loaded" +
                           (keyPath == null ? string.Empty : $" with the key '{keyPath}' configured in Backtier:{nameof(this.ClientCertificateKey)}") +
                           $". Check the files and Backtier:{nameof(this.ClientCertificatePassword)}: {ex.Message}");
            }
        }
    }
}