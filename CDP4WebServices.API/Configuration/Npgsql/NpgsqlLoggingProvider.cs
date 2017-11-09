// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlLoggingProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// source: http://www.npgsql.org/doc/logging.html
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Configuration.Npgsql
{
    using global::Npgsql.Logging;

    /// <summary>
    /// The NPGSQL logging provider.
    /// </summary>
    public class NpgsqlLoggingProvider : INpgsqlLoggingProvider
    {
        /// <summary>
        /// Create logger factory method.
        /// </summary>
        /// <param name="name">
        /// Name of the logger
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlLogger"/>.
        /// </returns>
        public NpgsqlLogger CreateLogger(string name)
        {
            return new NpgsqlNlogLogger(name);
        }
    }
}
