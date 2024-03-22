// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Services.Operations
{
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common.DTO;

    using CDP4DalCommon.Protocol.Operations;

    using Npgsql;

    /// <summary>
    /// The injectable OperationProcessor interface that allows processing of a <see cref="PostOperation"/>.
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
        /// <param name="fileStore">
        /// The optional file binaries that were included in the request.
        /// </param>
        void Process(PostOperation operation, NpgsqlTransaction transaction, string partition, Dictionary<string, Stream> fileStore = null);

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
