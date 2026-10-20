// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPostgreSqlTlsDiagnostics.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;

    /// <summary>
    /// The purpose of the <see cref="IPostgreSqlTlsDiagnostics"/> is to explain why a TLS connection to the PostgreSQL server
    /// failed, for instance which property of the server certificate does not match the configuration
    /// </summary>
    public interface IPostgreSqlTlsDiagnostics
    {
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
        Task<IReadOnlyList<string>> DiagnoseAsync(BacktierConfig backtierConfig, CancellationToken cancellationToken);
    }
}
