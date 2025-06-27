// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICachedReferenceDataService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ICachedReferenceDataService"/> is to provide a caching service for data that is reusable accross
    /// various services during the course of one request
    /// </summary>
    /// <remarks>
    /// The <see cref="ICachedReferenceDataService"/> is non-thread safe and should only be used per HTTP reequest
    /// </remarks>
    public interface ICachedReferenceDataService : IBusinessLogicService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="IParameterTypeService"/> used to query the <see cref="ParameterType"/>s from the data-store
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IParameterTypeComponentService"/> used to query the <see cref="ParameterTypeComponent"/>s from the data-store
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDependentParameterTypeAssignmentService" />
        /// </summary>
        public IDependentParameterTypeAssignmentService DependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIndependentParameterTypeAssignmentService" />
        /// </summary>
        public IIndependentParameterTypeAssignmentService IndependentParameterTypeAssignmentService { get; set; }
        
        /// <summary>
        /// Gets or sets the (injected) <see cref="IMeasurementScaleService"/> used to query the <see cref="MeasurementScale"/>s from the data-store
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMeasurementUnitService"/> used to query the <see cref="MeasurementUnit"/>s from the data-store
        /// </summary>
        public IMeasurementUnitService MeasurementUnitService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IEnumerationValueDefinitionService"/> used to query the <see cref="EnumerationValueDefinition"/>s from the data-store
        /// </summary>
        public IEnumerationValueDefinitionService EnumerationValueDefinitionService { get; set; }

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterType}"/>
        /// </returns>
        public Task<Dictionary<Guid, ParameterType>> QueryParameterTypesAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Queries the <see cref="ParameterTypeComponent"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterTypeComponent}"/>
        /// </returns>
        public Task<Dictionary<Guid, ParameterTypeComponent>> QueryParameterTypeComponentsAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterType}"/>
        /// </returns>
        public Task<Dictionary<Guid, DependentParameterTypeAssignment>> QueryDependentParameterTypeAssignmentsAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterType}"/>
        /// </returns>
        public Task<Dictionary<Guid, IndependentParameterTypeAssignment>> QueryIndependentParameterTypeAssignmentsAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Queries the <see cref="MeasurementScale"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, MeasurementScale}"/>
        /// </returns>
        public Task<Dictionary<Guid, MeasurementScale>> QueryMeasurementScalesAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Resets (clears) the cached data from the <see cref="ICachedReferenceDataService" />.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Queries the <see cref="MeasurementUnit"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, MeasurementUnit}"/>
        /// </returns>
        Task<Dictionary<Guid, EnumerationValueDefinition>> QueryEnumerationValueDefinitionsAsync(NpgsqlTransaction transaction, ISecurityContext securityContext);
    }
}
