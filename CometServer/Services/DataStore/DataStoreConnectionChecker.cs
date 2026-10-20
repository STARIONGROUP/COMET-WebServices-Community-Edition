// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreConnectionChecker.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DataStoreConnectionChecker"/> is to check whether a connection can be made to the databse
    /// and wait for it before returning
    /// </summary>
    public class DataStoreConnectionChecker : IDataStoreConnectionChecker
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<DataStoreConnectionChecker> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IDataSource"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the DataSource manager.
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IPostgreSqlTlsDiagnostics"/> used to explain TLS connection failures
        /// </summary>
        public IPostgreSqlTlsDiagnostics PostgreSqlTlsDiagnostics { get; set; }

        /// <summary>
        /// Gets or sets the time to wait between two connection attempts, 5 seconds by default
        /// </summary>
        /// <remarks>
        /// internal so that it is not property injected, and can be shortened by unit tests
        /// </remarks>
        internal TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Checks whether a connection to the Data store can be made
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that can be used to cancel the operation
        /// </param>
        /// <returns>
        /// <see cref="DataStoreConnectionStatus.Connected"/> when a connection can be made within the <see cref="MidtierConfig.BacktierWaitTime"/>,
        /// <see cref="DataStoreConnectionStatus.TimedOut"/> when not, and <see cref="DataStoreConnectionStatus.ConfigurationError"/>
        /// as soon as an error occurs that waiting will not resolve
        /// </returns>
        public async Task<DataStoreConnectionStatus> CheckConnectionAsync(CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            var backtierConfig = this.AppConfigService.AppConfig.Backtier;
            var backtierWaitTime = this.AppConfigService.AppConfig.Midtier.BacktierWaitTime;
            var remainingSeconds = backtierWaitTime;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(backtierWaitTime) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using var connection = await this.DataSource.OpenNewConnectionAsync();

                    await this.LogConnectionSecurityAsync(connection, cancellationToken);

                    return DataStoreConnectionStatus.Connected;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var configurationError = DescribeConfigurationError(ex, backtierConfig);

                    if (configurationError != null)
                    {
                        await this.LogConfigurationErrorAsync(configurationError, ex, cancellationToken);

                        return DataStoreConnectionStatus.ConfigurationError;
                    }

                    this.Logger.LogInformation("Waiting for the data store at {HostName}:{Port} to become available, {RemainingSeconds} [s] remaining. Reason: {Reason}",
                        backtierConfig.HostName,
                        backtierConfig.Port,
                        remainingSeconds,
                        GetWaitingReason(ex, backtierConfig));

                    this.Logger.LogDebug(ex, "The connection to the data store at {HostName}:{Port} failed", backtierConfig.HostName, backtierConfig.Port);

                    await Task.Delay(this.RetryDelay, cancellationToken);
                    remainingSeconds -= (int)this.RetryDelay.TotalSeconds;
                }
            }

            return DataStoreConnectionStatus.TimedOut;
        }

        /// <summary>
        /// Determines whether the exception is caused by a configuration error that waiting will not resolve,
        /// such as a rejected certificate or a wrong password, and describes it. Errors that occur while the
        /// data store is starting up (connection refused, 57P03 the database system is starting up) are not
        /// configuration errors.
        /// </summary>
        /// <param name="exception">
        /// The exception thrown while connecting
        /// </param>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/> used to connect
        /// </param>
        /// <returns>
        /// A description of the configuration error, or null when the exception is not caused by one
        /// </returns>
        public static string DescribeConfigurationError(Exception exception, BacktierConfig backtierConfig)
        {
            var sslMode = backtierConfig.SslMode;

            for (var current = exception; current != null; current = current.InnerException)
            {
                if (current is AuthenticationException)
                {
                    return sslMode is SslMode.VerifyCA or SslMode.VerifyFull
                        ? $"The TLS handshake failed or the server certificate was rejected (Backtier:SslMode {sslMode})"
                        : $"The TLS handshake failed (Backtier:SslMode {sslMode})";
                }

                if (current is PostgresException { SqlState: PostgresErrorCodes.InvalidAuthorizationSpecification } postgresException
                    && postgresException.MessageText.EndsWith("no encryption", StringComparison.Ordinal))
                {
                    return $"The PostgreSQL server only accepts encrypted connections, but Backtier:SslMode is {sslMode}. Set Backtier:SslMode to VerifyFull (recommended) or Require";
                }

                if (current is NpgsqlException and not PostgresException
                    && current.Message.StartsWith("SSL connection requested", StringComparison.Ordinal))
                {
                    return $"Backtier:SslMode is {sslMode}, which requires TLS, but the PostgreSQL server does not accept TLS connections. Enable TLS on the server (ssl = on), or lower Backtier:SslMode to Prefer";
                }

                if (current is PostgresException serverException)
                {
                    switch (serverException.SqlState)
                    {
                        case PostgresErrorCodes.InvalidPassword:
                            return $"The PostgreSQL server rejected the credentials of user '{backtierConfig.UserName}'. Check Backtier:UserName and Backtier:Password, and that this user exists on the server";
                        case PostgresErrorCodes.InvalidAuthorizationSpecification:
                            // the "no encryption" check above relies on an English server message, so mention the SslMode here as well
                            var sslModeHint = sslMode is SslMode.Disable or SslMode.Allow
                                ? $". The connection is not encrypted (Backtier:SslMode {sslMode}): if the server only accepts encrypted connections, set Backtier:SslMode to VerifyFull (recommended) or Require"
                                : string.Empty;

                            return $"The PostgreSQL server does not allow user '{backtierConfig.UserName}' to connect to database '{backtierConfig.Database}' from this host. Check Backtier:UserName and Backtier:Database, and the pg_hba.conf of the server{sslModeHint}";
                        case PostgresErrorCodes.InvalidCatalogName when IsDatabaseCreatedByExchangeFile(backtierConfig):
                            // not an error: the database is created when an Annex C.3 file is posted to /Data/Exchange, keep waiting
                            return null;
                        case PostgresErrorCodes.InvalidCatalogName:
                            return $"The database '{backtierConfig.Database}' does not exist on the PostgreSQL server. Check Backtier:Database. " +
                                   "To create it from an Annex C.3 file posted to /Data/Exchange, enable Backtier:IsDbSeedEnabled or Backtier:IsDbImportEnabled";
                        case PostgresErrorCodes.InsufficientPrivilege:
                            return $"User '{backtierConfig.UserName}' is not allowed to connect to database '{backtierConfig.Database}'. Grant the CONNECT privilege on the server, or check Backtier:UserName";
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the reason why the connection is retried, with an explanation when the database does not exist yet
        /// and will be created from an Annex C.3 file
        /// </summary>
        /// <param name="exception">
        /// The exception thrown while connecting
        /// </param>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/> used to connect
        /// </param>
        /// <returns>
        /// The reason
        /// </returns>
        public static string GetWaitingReason(Exception exception, BacktierConfig backtierConfig)
        {
            var reason = GetReason(exception);

            for (var current = exception; current != null; current = current.InnerException)
            {
                if (current is PostgresException { SqlState: PostgresErrorCodes.InvalidCatalogName } && IsDatabaseCreatedByExchangeFile(backtierConfig))
                {
                    return $"{reason}. The database '{backtierConfig.Database}' is created when an Annex C.3 file is posted to /Data/Exchange";
                }
            }

            return reason;
        }

        /// <summary>
        /// Gets a one line reason from the exception and its inner exceptions, without stack traces
        /// </summary>
        /// <param name="exception">
        /// The exception
        /// </param>
        /// <returns>
        /// The reason
        /// </returns>
        public static string GetReason(Exception exception)
        {
            var messages = new List<string>();

            for (var current = exception; current != null; current = current.InnerException)
            {
                var message = current is PostgresException postgresException
                    ? $"{postgresException.SqlState}: {postgresException.MessageText}"
                    : current.Message;

                if (!messages.Contains(message))
                {
                    messages.Add(message);
                }
            }

            return string.Join(" -> ", messages);
        }

        /// <summary>
        /// Determines whether a missing database can still be created while the server is running, by posting an
        /// Annex C.3 file to /Data/Exchange (seed or import)
        /// </summary>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/>
        /// </param>
        /// <returns>
        /// true when seeding or importing is enabled
        /// </returns>
        private static bool IsDatabaseCreatedByExchangeFile(BacktierConfig backtierConfig)
        {
            return backtierConfig.IsDbSeedEnabled || backtierConfig.IsDbImportEnabled;
        }

        /// <summary>
        /// Logs a configuration error, including the findings of the <see cref="IPostgreSqlTlsDiagnostics"/>
        /// when the server certificate or the TLS handshake is involved
        /// </summary>
        /// <param name="configurationError">
        /// The description of the configuration error
        /// </param>
        /// <param name="exception">
        /// The exception thrown while connecting
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        private async Task LogConfigurationErrorAsync(string configurationError, Exception exception, CancellationToken cancellationToken)
        {
            var backtierConfig = this.AppConfigService.AppConfig.Backtier;

            this.Logger.LogCritical("The connection to the data store at {HostName}:{Port} failed: {ConfigurationError}. Reason: {Reason}",
                backtierConfig.HostName,
                backtierConfig.Port,
                configurationError,
                GetReason(exception));

            this.Logger.LogDebug(exception, "The connection to the data store at {HostName}:{Port} failed", backtierConfig.HostName, backtierConfig.Port);

            var isHandshakeFailure = false;

            for (var current = exception; current != null; current = current.InnerException)
            {
                isHandshakeFailure |= current is AuthenticationException;
            }

            if (!isHandshakeFailure || this.PostgreSqlTlsDiagnostics == null)
            {
                return;
            }

            var findings = await this.PostgreSqlTlsDiagnostics.DiagnoseAsync(backtierConfig, cancellationToken);

            foreach (var finding in findings)
            {
                if (finding.StartsWith("PROBLEM:", StringComparison.Ordinal))
                {
                    this.Logger.LogCritical("TLS diagnostics: {Finding}", finding);
                }
                else
                {
                    this.Logger.LogInformation("TLS diagnostics: {Finding}", finding);
                }
            }
        }

        /// <summary>
        /// Determines whether a warning about the security of the data store connection has to be logged
        /// </summary>
        /// <param name="sslMode">
        /// The configured <see cref="SslMode"/>
        /// </param>
        /// <param name="isSecure">
        /// A value indicating whether the connection is encrypted
        /// </param>
        /// <returns>
        /// true when the connection is not encrypted, or when the identity of the server is not verified
        /// </returns>
        public static bool IsConnectionSecurityWarningRequired(SslMode sslMode, bool isSecure)
        {
            return !isSecure || sslMode != SslMode.VerifyFull;
        }

        /// <summary>
        /// Logs whether the connection to the data store is encrypted, and with which TLS version and cipher
        /// </summary>
        /// <param name="connection">
        /// The open <see cref="NpgsqlConnection"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        private async Task LogConnectionSecurityAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            var backtierConfig = this.AppConfigService.AppConfig.Backtier;

            bool? isSecure = null;
            string tlsVersion = null;
            string cipher = null;

            try
            {
                await using var command = new NpgsqlCommand("SELECT ssl, version, cipher FROM pg_stat_ssl WHERE pid = pg_backend_pid();", connection);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                if (await reader.ReadAsync(cancellationToken))
                {
                    isSecure = !reader.IsDBNull(0) && reader.GetBoolean(0);
                    tlsVersion = reader.IsDBNull(1) ? null : reader.GetString(1);
                    cipher = reader.IsDBNull(2) ? null : reader.GetString(2);
                }
            }
            catch (Exception ex) when (ex is NpgsqlException or InvalidOperationException)
            {
                this.Logger.LogDebug(ex, "The TLS state of the data store connection could not be read from pg_stat_ssl");
            }

            if (isSecure is null)
            {
                this.Logger.LogWarning("Could not determine whether the connection to the data store at {HostName}:{Port} is encrypted (Backtier:SslMode {SslMode})",
                    backtierConfig.HostName,
                    backtierConfig.Port,
                    backtierConfig.SslMode);

                return;
            }

            if (isSecure == false)
            {
                this.Logger.LogWarning("The connection to the data store at {HostName}:{Port} is NOT encrypted (Backtier:SslMode {SslMode}). Credentials and data are sent in plain text",
                    backtierConfig.HostName,
                    backtierConfig.Port,
                    backtierConfig.SslMode);

                return;
            }

            this.Logger.LogInformation("The connection to the data store at {HostName}:{Port} is encrypted using {TlsVersion} {Cipher} (Backtier:SslMode {SslMode})",
                backtierConfig.HostName,
                backtierConfig.Port,
                tlsVersion,
                cipher,
                backtierConfig.SslMode);

            if (IsConnectionSecurityWarningRequired(backtierConfig.SslMode, true))
            {
                this.Logger.LogWarning("The identity of the data store server is not fully verified (Backtier:SslMode {SslMode}). Use VerifyFull to verify the certificate chain and the host name",
                    backtierConfig.SslMode);
            }
        }
    }
}
