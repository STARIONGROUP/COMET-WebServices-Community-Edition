﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultValueArrayFactory.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IDefaultValueArrayFactory"/> is to create a default <see cref="ValueArray{String}"/>
    /// where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
    /// each slot has the value "-"
    /// </summary>
    public interface IDefaultValueArrayFactory : IBusinessLogicService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ICachedReferenceDataService"/>
        /// </summary>
        ICachedReferenceDataService CachedReferenceDataService { get; set; }

        /// <summary>
        /// Initializes the <see cref="DefaultValueArrayFactory"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        Task LoadAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Resets the <see cref="IDefaultValueArrayFactory"/>.
        /// </summary>
        /// <remarks>
        /// After the <see cref="IDefaultValueArrayFactory"/> has been reset the data needs to be loaded again using the <see cref="LoadAsync"/> method.
        /// </remarks>
        void Reset();

        /// <summary>
        /// Creates a <see cref="ValueArray{String}"/> where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
        /// each slot has the value "-"
        /// </summary>
        /// <param name="parameterTypeIid">
        /// The unique id of the <see cref="ParameterType"/> for which a default <see cref="ValueArray{T}"/> needs to be created.
        /// </param>
        /// <returns>
        /// an instance of <see cref="ValueArray{T}"/>
        /// </returns>
        ValueArray<string> CreateDefaultValueArray(Guid parameterTypeIid);

        /// <summary>
        /// Creates a <see cref="ValueArray{String}"/> with as many slots containing "-" as the provided <paramref name="numberOfValues"/>
        /// </summary>
        /// <param name="numberOfValues">
        /// An integer denoting the number of slots
        /// </param>
        /// <returns>
        /// An instance of <see cref="ValueArray{String}"/>
        /// </returns>
        ValueArray<string> CreateDefaultValueArray(int numberOfValues);
    }
}