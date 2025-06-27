// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactory.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    using CDP4Common.Types;
    
    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IDefaultValueArrayFactory" /> is to create a default <see cref="ValueArray{T}" />
    /// where the number of slots is equal to to number of values associated to a <see cref="ParameterType" /> and where
    /// each slot has the value "-"
    /// </summary>
    public class DefaultValueArrayFactory : IDefaultValueArrayFactory
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<DefaultValueArrayFactory> Logger { get; set; }

        /// <summary>
        /// A cache of <see cref="ValueArray{String}" /> that has been computed for <see cref="ParameterType" />
        /// that is used for fast access so taht the default ValueArray does not have to be recomputed.
        /// </summary>
        private readonly Dictionary<Guid, ValueArray<string>> defaultValueArrayCache = new();

        /// <summary>
        /// a cache of <see cref="ParameterType" />s that is populated in the context of the current
        /// <see cref="DefaultValueArrayFactory" />
        /// </summary>
        private Dictionary<Guid, ParameterType> parameterTypeCache = new();

        /// <summary>
        /// a cache of <see cref="IParameterTypeAssignment" />s that is populated in the context of the current
        /// <see cref="DefaultValueArrayFactory" />
        /// </summary>
        private readonly Dictionary<Guid, IParameterTypeAssignment> parameterTypeAssignmentCache = new();

        /// <summary>
        /// a cache of <see cref="ParameterTypeComponent" />s that is populated in the context of the current
        /// <see cref="DefaultValueArrayFactory" />
        /// </summary>
        private Dictionary<Guid, ParameterTypeComponent> parameterTypeComponentCache = new();

        /// <summary>
        /// a cache of the <see cref="ParameterType" />s and their number-of-values in the context of the current
        /// <see cref="DefaultValueArrayFactory" />
        /// </summary>
        private readonly Dictionary<ParameterType, int> parameterTypeNumberOfValuesMap = new();

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICachedReferenceDataService"/>
        /// </summary>
        public ICachedReferenceDataService CachedReferenceDataService { get; set; }

        /// <summary>
        /// Load required data from the database, i.e. <see cref="ParameterType" /> and <see cref="ParameterTypeComponent" />
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext" /> used for permission checking.
        /// </param>
        public async Task LoadAsync(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            this.Logger.LogTrace("Loading reference data");

            var sw = Stopwatch.StartNew();

            this.parameterTypeCache = await this.CachedReferenceDataService.QueryParameterTypesAsync(transaction, securityContext);
            this.parameterTypeComponentCache = await this.CachedReferenceDataService.QueryParameterTypeComponentsAsync(transaction, securityContext);

            this.parameterTypeAssignmentCache.Clear();

            var dependentParameterTypeAssignments = await this.CachedReferenceDataService.QueryDependentParameterTypeAssignmentsAsync(transaction, securityContext);

            foreach (var kvp in dependentParameterTypeAssignments)
            {
                this.parameterTypeAssignmentCache.Add(kvp.Key, kvp.Value);
            }

            var independentParameterTypeAssignments = await this.CachedReferenceDataService.QueryIndependentParameterTypeAssignmentsAsync(transaction, securityContext);

            foreach (var kvp in independentParameterTypeAssignments)
            {
                this.parameterTypeAssignmentCache.Add(kvp.Key, kvp.Value);
            }

            this.Logger.LogTrace("Cache initialized with {ParameterTypeCache} ParameterTypes, {DependentParameterTypeAssignments} DependentParameterTypeAssignments, {IndependentParameterTypeAssignments} IndependentParameterTypeAssignments and {ParameterTypeComponentCache} ParameterTypeComponents in {ElapsedMilliseconds}",
                this.parameterTypeCache.Count,
                dependentParameterTypeAssignments.Count,
                independentParameterTypeAssignments.Count,
                this.parameterTypeComponentCache.Count,
                sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Resets the <see cref="IDefaultValueArrayFactory" />.
        /// </summary>
        /// <remarks>
        /// After the <see cref="IDefaultValueArrayFactory" /> has been reset the data needs to be loaded again using the
        /// <see cref="Load" /> method.
        /// </remarks>
        public void Reset()
        {
            this.defaultValueArrayCache.Clear();
            this.parameterTypeCache.Clear();
            this.parameterTypeAssignmentCache.Clear();
            this.parameterTypeComponentCache.Clear();
            this.parameterTypeNumberOfValuesMap.Clear();
        }

        /// <summary>
        /// Creates a <see cref="ValueArray{String}" /> where the number of slots is equal to to number of values associated to a
        /// <see cref="ParameterType" /> and where
        /// each slot has the value "-"
        /// </summary>
        /// <param name="parameterTypeIid">
        /// The unique id of the <see cref="ParameterType" /> for which a default <see cref="ValueArray{T}" /> needs to be created.
        /// </param>
        /// <returns>
        /// an instance of <see cref="ValueArray{T}" />
        /// </returns>
        public ValueArray<string> CreateDefaultValueArray(Guid parameterTypeIid)
        {
            if (this.defaultValueArrayCache.TryGetValue(parameterTypeIid, out var defaultValueArray))
            {
                return defaultValueArray;
            }

            if (!this.parameterTypeCache.TryGetValue(parameterTypeIid, out var parameterType))
            {
                var execptionMessage = $"The ParameterType with iid {parameterTypeIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                this.Logger.LogError(execptionMessage);
                throw new KeyNotFoundException(execptionMessage);
            }

            var numberOfValues = this.ComputeNumberOfValues(parameterType);
            var valueArray = this.CreateDefaultValueArray(numberOfValues);

            this.defaultValueArrayCache.Add(parameterTypeIid, valueArray);

            return valueArray;
        }

        /// <summary>
        /// Creates a <see cref="ValueArray{String}" /> with as many slots containing "-" as the provided
        /// <paramref name="numberOfValues" />
        /// </summary>
        /// <param name="numberOfValues">
        /// An integer denoting the number of slots
        /// </param>
        /// <returns>
        /// An instance of <see cref="ValueArray{String}" />
        /// </returns>
        public ValueArray<string> CreateDefaultValueArray(int numberOfValues)
        {
            var defaultValue = new List<string>(numberOfValues);

            for (var i = 0; i < numberOfValues; i++)
            {
                defaultValue.Add("-");
            }

            var result = new ValueArray<string>(defaultValue);

            return result;
        }

        /// <summary>
        /// Computes the number of values for a given <see cref="ParameterType" />
        /// </summary>
        /// <param name="parameterType">
        /// The <see cref="ParameterType" /> for which the number of values needs to be computed
        /// </param>
        /// <returns>
        /// an integer representing the number of values, this is always 1 in case of a <see cref="ScalarParameterType" />
        /// </returns>
        private int ComputeNumberOfValues(ParameterType parameterType)
        {
            if (this.parameterTypeNumberOfValuesMap.TryGetValue(parameterType, out var numberOfValues))
            {
                return numberOfValues;
            }

            if (parameterType is ScalarParameterType)
            {
                this.parameterTypeNumberOfValuesMap.Add(parameterType, 1);
                return 1;
            }

            if (parameterType is CompoundParameterType compoundParameterType)
            {
                numberOfValues = this.ComputeNumberOfValuesForCompoundParamterType(compoundParameterType);
            }
            else if (parameterType is SampledFunctionParameterType sampledFunctionParameterType)
            {
                numberOfValues = this.ComputeNumberOfValuesForSampledFunctionParameterType(sampledFunctionParameterType);
            }

            this.parameterTypeNumberOfValuesMap.Add(parameterType, numberOfValues);
            return numberOfValues;
        }

        /// <summary>
        /// Recursive function that computes the number of values for a <see cref="SampledFunctionParameterType" />
        /// </summary>
        /// <param name="sampledFunctionParameterType">
        /// The <see cref="SampledFunctionParameterType" /> for which the number of values needs to be computed
        /// </param>
        /// <returns>
        /// an integer representing the number of values
        /// </returns>
        private int ComputeNumberOfValuesForSampledFunctionParameterType(SampledFunctionParameterType sampledFunctionParameterType)
        {
            var result = 0;

            if (sampledFunctionParameterType.IndependentParameterType.Count == 0)
            {
                this.Logger.LogWarning("The SampledFunctionParameterType with Iid {SampledFunctionParameterType} does not contain any IndependetnParameterTypeAssignments", sampledFunctionParameterType.Iid);
                return 0;
            }

            if (sampledFunctionParameterType.DependentParameterType.Count == 0)
            {
                this.Logger.LogWarning("The SampledFunctionParameterType with Iid {SampledFunctionParameterType} does not contain any DependetnParameterTypeAssignments", sampledFunctionParameterType.Iid);
                return 0;
            }

            // handle all independent
            foreach (var parameterTypeComponentKeyVaulePair in sampledFunctionParameterType.IndependentParameterType)
            {
                var parameterTypeAssignmentIid = Guid.Parse(parameterTypeComponentKeyVaulePair.V.ToString());

                if (!this.parameterTypeAssignmentCache.TryGetValue(parameterTypeAssignmentIid, out var parameterTypeAssignment))
                {
                    var exceptionMessage = $"The ParameterTypeAssignment with Iid {parameterTypeAssignmentIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (!this.parameterTypeCache.TryGetValue(parameterTypeAssignment.ParameterType, out var parameterType))
                {
                    var exceptionMessage = $"The ParameterType {parameterTypeAssignment.ParameterType} of the ParameterTypeAssignment {((Thing)parameterTypeAssignment).Iid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (parameterType is ScalarParameterType)
                {
                    result++;
                }
                else
                {
                    result += this.ComputeNumberOfValuesForCompoundParamterType(parameterType as CompoundParameterType);
                }
            }

            // handle all dependent
            foreach (var parameterTypeComponentKeyVaulePair in sampledFunctionParameterType.DependentParameterType)
            {
                var parameterTypeAssignmentIid = Guid.Parse(parameterTypeComponentKeyVaulePair.V.ToString());

                if (!this.parameterTypeAssignmentCache.TryGetValue(parameterTypeAssignmentIid, out var parameterTypeAssignment))
                {
                    var exceptionMessage = $"The ParameterTypeAssignment with Iid {parameterTypeAssignmentIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (!this.parameterTypeCache.TryGetValue(parameterTypeAssignment.ParameterType, out var parameterType))
                {
                    var exceptionMessage = $"The ParameterType {parameterTypeAssignment.ParameterType} of the ParameterTypeAssignment {((Thing)parameterTypeAssignment).Iid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (parameterType is ScalarParameterType)
                {
                    result++;
                }
                else
                {
                    result += this.ComputeNumberOfValuesForCompoundParamterType(parameterType as CompoundParameterType);
                }
            }

            return result;
        }

        /// <summary>
        /// Recursive function that computes the number of values for a <see cref="CompoundParameterType" />
        /// </summary>
        /// <param name="compoundParameterType">
        /// The <see cref="CompoundParameterType" /> for which the number of values needs to be computed
        /// </param>
        /// <returns>
        /// an integer representing the number of values
        /// </returns>
        private int ComputeNumberOfValuesForCompoundParamterType(CompoundParameterType compoundParameterType)
        {
            var result = 0;

            if (compoundParameterType.Component.Count == 0)
            {
                this.Logger.LogWarning("The CompoundParameterType with Iid {CompoundParameterType} does not contain any ParameterTypeComponents", compoundParameterType.Iid);
                return 0;
            }

            foreach (var parameterTypeComponentKeyVaulePair in compoundParameterType.Component)
            {
                var parameterTypeComponentIid = Guid.Parse(parameterTypeComponentKeyVaulePair.V.ToString());

                if (!this.parameterTypeComponentCache.TryGetValue(parameterTypeComponentIid, out var parameterTypeComponent))
                {
                    var exceptionMessage = $"The ParameterTypeComponent with Iid {parameterTypeComponentIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (!this.parameterTypeCache.TryGetValue(parameterTypeComponent.ParameterType, out var parameterType))
                {
                    var exceptionMessage = $"The ParameterType {parameterTypeComponent.ParameterType} of the ParameterTypeComponent {parameterTypeComponent.Iid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    this.Logger.LogError(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                if (parameterType is ScalarParameterType)
                {
                    result++;
                }
                else
                {
                    result += this.ComputeNumberOfValuesForCompoundParamterType(parameterType as CompoundParameterType);
                }
            }

            return result;
        }
    }
}
