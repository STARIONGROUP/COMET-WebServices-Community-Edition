﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonExchangeFileWriter.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The <see cref="IJsonExchangeFileWriter"/> interface that defines methods to write data to
    /// an E-TM-10-25 Annex C3 archive
    /// </summary>
    public interface IJsonExchangeFileWriter
    {
        /// <summary>
        /// Creates an E-TM-10-25 Annex-C3 zip archive and stores this on disk. The path to this file is returned
        /// </summary>
        /// <param name="transaction">
        /// The <see cref="NpgsqlTransaction"/> used to read 10-25 data from the database
        /// </param>
        /// <param name="exportDirectory">
        /// the path to the export directory where the temporary zip file will be created
        /// </param>
        /// <param name="engineeringModelSetups">
        /// An <see cref="IEnumerable{EngineeringModelSetup}"/> that is to be included in the E-TM-10-25 Annex-C3 zip archive
        /// </param>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET data model that is requested
        /// </param>
        /// <returns>
        /// The path to the temporary E-TM-10-25 Annex-C3 zip archive
        /// </returns>
        Task<string> CreateAsync(NpgsqlTransaction transaction, string exportDirectory, IEnumerable<EngineeringModelSetup> engineeringModelSetups, Version version);
    }
}
