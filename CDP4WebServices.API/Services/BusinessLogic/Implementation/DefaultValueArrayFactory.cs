// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueArrayFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services.Authorization;
    using NLog;
    using Npgsql;

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
        /// A cache of <see cref="ValueArray{string}"/> that has been computed for <see cref="ParameterType"/>
        /// that is used for fast access so taht the default ValueArray does not have to be recomputed.
        /// </summary>
        private readonly Dictionary<Guid, ValueArray<string>> defaultValueArrayCache = new Dictionary<Guid, ValueArray<string>>();

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
        /// Gets or sets the <see cref="IParameterTypeService"/>
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeComponentService"/>
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Load required data from the database, i.e. <see cref="ParameterType"/> and <see cref="ParameterTypeComponent"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        public void Load(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.IsLoaded)
            {
                Logger.Trace("The DefaultValueArrayFactory has already been loaded, no need to hit the database again");
                return;
            }

            Logger.Trace("Loading ParameterTypes and ParameterTypeComponents");

            var sw = Stopwatch.StartNew();

            var parameterTypes = this.ParameterTypeService.GetShallow(
                transaction,
                CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                null,
                securityContext).Cast<ParameterType>();

            Logger.Trace("ParameterTypes loaded in {0} [ms]", sw.ElapsedMilliseconds);

            sw.Restart();

            var parameterTypeComponents = this.ParameterTypeComponentService.GetShallow(
                transaction,
                CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                null,
                securityContext).Cast<ParameterTypeComponent>();

            Logger.Trace("ParameterTypeComponents loaded in {0} [ms]", sw.ElapsedMilliseconds);
            
            Logger.Trace("Initializing ParameterType cache");

            sw.Restart();

            foreach (var parameterType in parameterTypes)
            {
                this.parameterTypeCache.Add(parameterType.Iid, parameterType);
            }
            
            Logger.Trace("Initializing ParameterTypeComponent cache");
            
            foreach (var parameterTypeComponent in parameterTypeComponents)
            {
                this.parameterTypeComponentCache.Add(parameterTypeComponent.Iid, parameterTypeComponent);
            }
            
            this.IsLoaded = true;

            Logger.Trace("Cache initialized with {0} ParameterTypes and {1} ParameterTypeComponents in {2}", this.parameterTypeCache.Count, this.parameterTypeComponentCache.Count, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Resets the <see cref="IDefaultValueArrayFactory"/>.
        /// </summary>
        /// <remarks>
        /// After the <see cref="IDefaultValueArrayFactory"/> has been reset the data needs to be loaded again using the <see cref="Load"/> method.
        /// </remarks>
        public void Reset()
        {
            this.defaultValueArrayCache.Clear();
            this.parameterTypeCache.Clear();
            this.parameterTypeComponentCache.Clear();
            this.parameterTypeNumberOfValuesMap.Clear();
            this.IsLoaded = false;
        }

        /// <summary>
        /// Assertion that determines whether the <see cref="DefaultValueArrayFactory"/> is loaded.
        /// </summary>
        public bool IsLoaded { get; private set; }
        
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
            ValueArray<string> defaultValueArray;
            if (this.defaultValueArrayCache.TryGetValue(parameterTypeIid, out defaultValueArray))
            {
                return defaultValueArray;
            }

            ParameterType parameterType;
            if (!this.parameterTypeCache.TryGetValue(parameterTypeIid, out parameterType))
            {
                var execptionMessage = $"The ParameterType with iid {parameterTypeIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                Logger.Error(execptionMessage);
                throw new KeyNotFoundException(execptionMessage);
            }

            var numberOfValues = this.ComputeNumberOfValues(parameterType);
            var valueArray = this.CreateDefaultValueArray(numberOfValues);

            this.defaultValueArrayCache.Add(parameterTypeIid, valueArray);

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
                    var exceptionMessage = $"The ParameterTypeComponent with Iid {parameterTypeComponentIid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
                    Logger.Error(exceptionMessage);
                    throw new KeyNotFoundException(exceptionMessage);
                }

                ParameterType parameterType;
                if (!this.parameterTypeCache.TryGetValue(parameterTypeComponent.ParameterType, out parameterType))
                {
                    var exceptionMessage = $"The ParameterType {parameterTypeComponent.ParameterType} of the ParameterTypeComponent {parameterTypeComponent.Iid} could not be found in the DefaultValueArrayFactory cache. A Default ValueArray could not be created";
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