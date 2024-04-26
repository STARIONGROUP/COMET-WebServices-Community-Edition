// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
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

namespace CometServer.Services
{
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The Parameter Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IParameterService
    {
        /// <summary>
        /// Queries all referenced <see cref="Thing" /> from the SiteDirectory (<see cref="ParameterType" />,
        /// <see cref="MeasurementScale" />, ...) for a <see cref="Parameter" />
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/> to use to retrieve SiteDirectory <see cref="Thing"/>s</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        /// <returns>A collection of referenced <see cref="Thing"/>s</returns>
        /// <exception cref="ThingNotFoundException">If one of the referenced <see cref="Thing"/> can not be retrieved</exception>
        IReadOnlyCollection<Thing> QueryReferencedSiteDirectoryThings(Parameter parameter, NpgsqlTransaction transaction, ISecurityContext securityContext);
    }
}
