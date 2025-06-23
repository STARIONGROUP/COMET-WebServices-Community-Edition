// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
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

namespace CDP4Orm.Dao.Revision
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The CacheDao interface.
    /// </summary>
    public interface ICacheDao
    {
        /// <summary>
        /// Save a <see cref="Thing"/> to a cache table 
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        Task Write(NpgsqlTransaction transaction, string partition, Thing thing);

        /// <summary>
        /// Save a collection of <see cref="Thing"/> to cache tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="things">The collection of revised <see cref="Thing"/>s</param>
        Task BulkWrite(NpgsqlTransaction transaction, string partition, IReadOnlyCollection<Thing> things);
    }
}