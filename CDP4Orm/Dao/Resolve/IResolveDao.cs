// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResolveDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
//
//    Author: Sam Geren�, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Th�ate
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

namespace CDP4Orm.Dao.Resolve
{
    using System;
    using System.Collections.Generic;

    using Npgsql;

    /// <summary>
    /// The ContainerDao interface.
    /// </summary>
    public interface IResolveDao
    {
        /// <summary>
        /// Resolve the information from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource are to be resolved from.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances to resolve information for.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        IEnumerable<ResolveInfo> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids);
    }
}