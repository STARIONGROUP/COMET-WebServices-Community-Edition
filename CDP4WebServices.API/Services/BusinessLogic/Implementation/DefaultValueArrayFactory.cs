// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using NLog;

    /// <summary>
    /// The purpose of the <see cref="IDefaultValueArrayFactory"/> is to create a default <see cref="ValueArray{String}"/>
    /// where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
    /// each slot has the value "-"
    /// </summary>
    public class DefaultValueArrayFactory : IDefaultValueArrayFactory
    {
        /// <summary>
        /// The NLog logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// a cache of <see cref="ParameterType"/>s that is populated in the context of the current <see cref="DefaultValueArrayFactory"/>
        /// </summary>
        private readonly Dictionary<Guid, ParameterType> parameterTypeCache = new Dictionary<Guid, ParameterType>();

        /// <summary>
        /// a cache of <see cref="ParameterTypeComponent"/>s that is populated in the context of the current <see cref="DefaultValueArrayFactory"/>
        /// </summary>
        private readonly Dictionary<Guid, ParameterTypeComponent> parameterTypeComponentCache = new Dictionary<Guid, ParameterTypeComponent>();

        /// <summary>
        /// a cache of the <see cref="ParameterType"/>s and their number-of-values in the context of the current <see cref="DefaultValueArrayFactory"/>
        /// </summary>
        private readonly Dictionary<ParameterType, int> parameterTypeNumberOfValuesMap = new Dictionary<ParameterType, int>();

        /// <summary>
        /// Initializes the <see cref="DefaultValueArrayFactory"/>.
        /// </summary>
        /// <param name="parameterTypes">
        /// The <see cref="ParameterType"/>s that are used to compute the default <see cref="ValueArray{T}"/>
        /// </param>
        /// <param name="parameterTypeComponents">
        /// The <see cref="ParameterTypeComponent"/>s that are used to compute the default <see cref="ValueArray{T}"/>
        /// </param>
        public void Initialize(IEnumerable<ParameterType> parameterTypes, IEnumerable<ParameterTypeComponent> parameterTypeComponents)
        {
            Logger.Trace("Initializing");

            if (this.parameterTypeCache.Count == 0)
            {
                foreach (var parameterType in parameterTypes)
                {
                    this.parameterTypeCache.Add(parameterType.Iid, parameterType);
                }
            }

            if (this.parameterTypeComponentCache.Count == 0)
            {
                foreach (var parameterTypeComponent in parameterTypeComponents)
                {
                    this.parameterTypeComponentCache.Add(parameterTypeComponent.Iid, parameterTypeComponent);
                }
            }

            Logger.Trace("Initialized with {0} ParameterTypes and {1} ParameterTypeComponents", this.parameterTypeCache.Count, this.parameterTypeComponentCache.Count);
        }

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
        public ValueArray<string> CreateDefaultValueArray(Guid parameterTypeIid)
        {
            ParameterType parameterType;
            if (!this.parameterTypeCache.TryGetValue(parameterTypeIid, out parameterType))
            {
                var execptionMessage = string.Format("The ParameterType with iid {0} could not be found", parameterTypeIid);
                Logger.Error(execptionMessage);
                throw new KeyNotFoundException(execptionMessage);
            }

            var numberOfValues = this.ComputeNumberOfValues(parameterType);
            var valueArray = this.CreateDefaultValueArray(numberOfValues);
            return valueArray;
        }

        /// <summary>
        /// Creates a <see cref="ValueArray{String}"/> with as many slots containing "-" as the provided <paramref name="numberOfValues"/>
        /// </summary>
        /// <param name="numberOfValues">
        /// An integer denoting the number of slots
        /// </param>
        /// <returns>
        /// An instance of <see cref="ValueArray{String}"/>
        /// </returns>
        private ValueArray<string> CreateDefaultValueArray(int numberOfValues)
        {
            var defaultValue = new List<string>(numberOfValues);

            for (int i = 0; i < numberOfValues; i++)
            {
                defaultValue.Add("-");
            }

            var result = new ValueArray<string>(defaultValue);

            return result;
        }

        /// <summary>
        /// Computes the number of values for a given <see cref="ParameterType"/>
        /// </summary>
        /// <param name="parameterType">
        /// The <see cref="ParameterType"/> for which the number of values needs to be computed
        /// </param>
        /// <returns>
        /// an integer representing the number of values, this is always 1 in case of a <see cref="ScalarParameterType"/>
        /// </returns>
        private int ComputeNumberOfValues(ParameterType parameterType)
        {
            int numberOfValues;
            if (this.parameterTypeNumberOfValuesMap.TryGetValue(parameterType, out numberOfValues))
            {
                return numberOfValues;
            }

            if (parameterType is ScalarParameterType)
            {
                this.parameterTypeNumberOfValuesMap.Add(parameterType, 1);
                return 1;
            }

            var compoundParameterType = (CompoundParameterType)parameterType;
            numberOfValues = this.ComputeNumberOfValuesForCompoundParamterType(compoundParameterType);
            this.parameterTypeNumberOfValuesMap.Add(compoundParameterType, numberOfValues);
            return numberOfValues;
        }

        /// <summary>
        /// Recursive function that computes the number of values for a <see cref="CompoundParameterType"/>
        /// </summary>
        /// <param name="compoundParameterType">
        /// The <see cref="CompoundParameterType"/> for which the number of values needs to be computed
        /// </param>
        /// <returns>
        /// an integer representing the number of values
        /// </returns>
        private int ComputeNumberOfValuesForCompoundParamterType(CompoundParameterType compoundParameterType)
        {
            var result = 0;

            if (compoundParameterType.Component.Count == 0)
            {
                Logger.Warn("The CompoundParameterType with Iid {0} does not contain any ParameterTypeComponents", compoundParameterType.Iid);
                return 0;
            }

            foreach (var parameterTypeComponentKeyVaulePair in compoundParameterType.Component)
            {
                var parameterTypeComponentIid = Guid.Parse(parameterTypeComponentKeyVaulePair.V.ToString());
                ParameterTypeComponent parameterTypeComponent;
                if (!this.parameterTypeComponentCache.TryGetValue(parameterTypeComponentIid, out parameterTypeComponent))
                {
                    var exceptionMessage = string.Format("The ParameterTypeComponent with Iid {0} could not be found", parameterTypeComponentIid);
                    Logger.Error(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                ParameterType parameterType;
                if (!this.parameterTypeCache.TryGetValue(parameterTypeComponent.ParameterType, out parameterType))
                {
                    var exceptionMessage = string.Format("The ParameterType {0} of the ParameterTypeComponent {1} could not be found", parameterTypeComponent.ParameterType, parameterTypeComponent.Iid);
                    Logger.Error(exceptionMessage);
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