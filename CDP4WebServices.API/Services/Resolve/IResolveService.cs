// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResolveService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
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

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Orm.Dao.Resolve;

    using Npgsql;

    /// <summary>
    /// The ResolveService interface.
    /// </summary>
    public interface IResolveService
    {
        /// <summary>
        /// Resolve the information in the supplied collection of <see cref="DtoResolveHelper"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info placeholders.
        /// </param>
        void ResolveItems(NpgsqlTransaction transaction, string partition, Dictionary<DtoInfo, DtoResolveHelper> resolvableInfo);

        /// <summary>
        /// Resolve missing containers from data store.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iid">
        /// The guid of the item to resolve type name for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> type name of the supplied item.
        /// </returns> 
        string ResolveTypeNameByGuid(NpgsqlTransaction transaction, string partition, Guid iid);
    }
}