// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOldParameterContextProvider.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// A service interface that provides context on the original version of an updated <see cref="Parameter"/>
    /// </summary>
    public interface IOldParameterContextProvider : IBusinessLogicService
    {
        /// <summary>
        /// Initializes the service with a previous version of an updated <see cref="Parameter"/>
        /// </summary>
        /// <param name="oldParameter">The old <see cref="Parameter"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="iteration">The current <see cref="Iteration"/> (nullable)</param>
        void Initialize(Parameter oldParameter, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, Iteration iteration);

        /// <summary>
        /// Gets the source <see cref="ParameterValueSet"/> for the new one to be created for a specified option and state
        /// </summary>
        /// <param name="option">The identifier of the option</param>
        /// <param name="state">The identifier of the state</param>
        /// <returns>The source <see cref="ParameterValueSet"/></returns>
        ParameterValueSet GetsourceValueSet(Guid? option, Guid? state);

        /// <summary>
        /// Gets the default value 
        /// </summary>
        /// <returns>The default <see cref="ParameterValueSet"/></returns>
        ParameterValueSet GetDefaultValue();
    }
}
