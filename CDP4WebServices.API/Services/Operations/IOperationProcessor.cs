// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Geren�, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations
{
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The injectable OperationProcessor interface that allows processing of a <see cref="CdpPostOperation"/>.
    /// </summary>
    public interface IOperationProcessor
    {
        /// <summary>
        /// Process the posted operation message.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="resolveFromCache">
        /// A value indicating whether meta data shall be resolved from the CAHCE tables or from the VIEWs
        /// </param>
        /// <param name="fileStore">
        /// The optional file binaries that were included in the request.
        /// </param>
        void Process(CdpPostOperation operation, NpgsqlTransaction transaction, string partition, bool resolveFromCache, Dictionary<string, Stream> fileStore = null);

        /// <summary>
        /// Gets the operation original <see cref="Thing"/> instance cache.
        /// </summary>
        /// <remarks>
        /// Do NOT use this cache for things that influence database concurrency,
        /// because that could lead to unexpected results.
        /// </remarks>
        IReadOnlyList<Thing> OperationOriginalThingCache { get; }
    }
}
