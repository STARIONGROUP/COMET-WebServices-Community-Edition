// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlNlogLogger.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// source: http://www.npgsql.org/doc/logging.html
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Configuration.Npgsql
{
    using System;

    using NLog;

    using global::Npgsql.Logging;

    /// <summary>
    /// The NGGSQL NLOG logger.
    /// </summary>
    public class NpgsqlNlogLogger : NpgsqlLogger
    {
        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlNlogLogger"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        internal NpgsqlNlogLogger(string name)
        {
            this.logger = LogManager.GetLogger(name);
        }

        /// <summary>
        /// NPGSQL Log level to NLOG log level mapping.
        /// </summary>
        /// <param name="level">
        /// The NPGSQL log level.
        /// </param>
        /// <returns>
        /// The <see cref="LogLevel"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If log level could not be mapped.
        /// </exception>
        public static LogLevel ToNLogLogLevel(NpgsqlLogLevel level)
        {
            switch (level)
            {
                case NpgsqlLogLevel.Trace:
                    return LogLevel.Trace;
                case NpgsqlLogLevel.Debug:
                    return LogLevel.Debug;
                case NpgsqlLogLevel.Info:
                    return LogLevel.Info;
                case NpgsqlLogLevel.Warn:
                    return LogLevel.Warn;
                case NpgsqlLogLevel.Error:
                    return LogLevel.Error;
                case NpgsqlLogLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        /// <summary>
        /// Check if logger is enabled.
        /// </summary>
        /// <param name="level">
        /// The log level.
        /// </param>
        /// <returns>
        /// true if enabled.
        /// </returns>
        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return this.logger.IsEnabled(ToNLogLogLevel(level));
        }

        /// <summary>
        /// Write log entry to NLOG.
        /// </summary>
        /// <param name="level">
        /// The log level.
        /// </param>
        /// <param name="connectorId">
        /// The connector id.
        /// </param>
        /// <param name="msg">
        /// The log message string.
        /// </param>
        /// <param name="exception">
        /// Exception instance to include in log entry.
        /// </param>
        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            var ev = new LogEventInfo(ToNLogLogLevel(level), string.Empty, msg);
            if (exception != null)
            {
                ev.Exception = exception;
            }

            if (connectorId != 0)
            {
                ev.Properties["ConnectorId"] = connectorId;
            }

            this.logger.Log(ev);
        }
    }
}
