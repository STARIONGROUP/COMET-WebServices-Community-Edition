// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationSetupDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The IterationSetup Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationSetupDao
    {
        /// <summary>
        /// Read the data from the database based on <see cref="IterationSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration Id.
        /// </param>
        /// <param name="instant">
        /// The instant as a <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByIteration(NpgsqlTransaction transaction, string partition, Guid iterationId, DateTime? instant = null);

        /// <summary>
        /// Read the data from the database based on <see cref="EngineeringModelSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup"/> Id.
        /// </param>
        /// <param name="instant">
        /// The instant as a <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid engineeringModelSetupId, DateTime? instant = null);
    }
}
