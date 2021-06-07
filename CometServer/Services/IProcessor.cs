// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessor.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The processor interface.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Gets the request utils.
        /// </summary>
        IRequestUtils RequestUtils { get; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        NpgsqlTransaction Transaction { get; }

        /// <summary>
        /// Returns the containment type based from the supplied container property.
        /// </summary>
        /// <param name="containment">
        /// The containment collection.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The type name of the containment property.
        /// </returns>
        /// <exception cref="Exception">
        /// If property is not supported or the containment collection is null
        /// </exception>
        string GetContainmentType(IReadOnlyCollection<Thing> containment, string propertyName);

        /// <summary>
        /// Get the containment id collection from the container property.
        /// </summary>
        /// <param name="containers">
        /// The containers collection.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// null if there is no containment otherwise the a list of contained item ids
        /// </returns>
        IEnumerable<Guid> GetContainment(List<Thing> containers, string propertyName);

        /// <summary>
        /// validate that the container property collection contains an entry for the supplied id.
        /// </summary>
        /// <param name="containers">
        /// The containers collection.
        /// </param>
        /// <param name="propertyName">
        /// The containment property name.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        void ValidateContainment(List<Thing> containers, string propertyName, Guid id);

        /// <summary>
        /// Get resource(s) from the back tier.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="ids">
        /// The list of ids to retrieve.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The container Security Context.
        /// </param>
        /// <returns>
        /// Collection of retrieved resources.
        /// </returns>
        IEnumerable<Thing> GetResource(string serviceType, string topContainer, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext);

        /// <summary>
        /// The get containment resource.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The container Security Context.
        /// </param>
        /// <returns>
        /// The <see cref="Thing"/>.
        /// </returns>
        Thing GetContainmentResource(string serviceType, string topContainer, Guid identifier, ISecurityContext containerSecurityContext);
    }
}