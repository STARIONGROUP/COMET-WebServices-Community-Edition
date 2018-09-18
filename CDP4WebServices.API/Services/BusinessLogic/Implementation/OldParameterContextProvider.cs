// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OldParameterContextProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authorization;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// A service interface that provides context on the original version of an updated <see cref="Parameter"/>
    /// </summary>
    /// <remarks>
    /// BEWARE: This injected service cannot be used in parallel mode, this would need to be refactored
    /// </remarks>
    public class OldParameterContextProvider : IOldParameterContextProvider
    {
        #region injected services
        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateService"/>
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOptionService"/>
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOptionBusinessLogicService"/>
        /// </summary>
        public IOptionBusinessLogicService OptionBusinessLogicService { get; set; }
        #endregion

        #region Contextual properties - Initialized by Initialize method
        /// <summary>
        /// Gets the old parameter
        /// </summary>
        public Parameter OldParameter { get; private set; }

        /// <summary>
        /// Gets the old <see cref="ActualFiniteStateList"/> dependency
        /// </summary>
        public ActualFiniteStateList OldActualFiniteStateList { get; private set; }

        /// <summary>
        /// Gets the old default <see cref="ActualFiniteState"/> dependency
        /// </summary>
        public ActualFiniteState OldDefaultState { get; private set; }

        /// <summary>
        /// Gets the old default <see cref="Option"/> dependency
        /// </summary>
        public Option OldDefaultOption { get; private set; }

        /// <summary>
        /// Gets the old <see cref="ActualFiniteState"/> dependency
        /// </summary>
        public IReadOnlyList<ActualFiniteState> OldActualFiniteStates { get; private set; }

        /// <summary>
        /// Gets the old <see cref="ParameterValueSet"/>
        /// </summary>
        public IReadOnlyList<ParameterValueSet> OldValueSet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the old parameter is state dependent
        /// </summary>
        public bool IsOldStateDependent => this.OldParameter.StateDependence.HasValue;

        /// <summary>
        /// Gets a value indicating whether the old parameter is option dependent
        /// </summary>
        public bool IsOldOptionDependent => this.OldParameter.IsOptionDependent;
        #endregion

        /// <summary>
        /// Initializes the service with a previous version of an updated <see cref="Parameter"/>
        /// </summary>
        /// <param name="oldParameter">The old <see cref="Parameter"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="iteration">The current <see cref="Iteration"/> (nullable)</param>
        public void Initialize(Parameter oldParameter, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, Iteration iteration = null)
        {
            this.OldParameter = oldParameter;
            this.OldValueSet = this.ParameterValueSetService.GetShallow(transaction, partition, this.OldParameter.ValueSet, securityContext).Cast<ParameterValueSet>().ToList();

            if (this.OldParameter.StateDependence.HasValue)
            {
                this.OldActualFiniteStateList = this.ActualFiniteStateListService
                    .GetShallow(transaction, partition, new[] { this.OldParameter.StateDependence.Value }, securityContext)
                    .Cast<ActualFiniteStateList>()
                    .First();

                this.OldActualFiniteStates = this.ActualFiniteStateService
                    .GetShallow(transaction, partition, this.OldActualFiniteStateList.ActualState, securityContext)
                    .Cast<ActualFiniteState>()
                    .ToList();

                this.OldDefaultState = this.ActualFiniteStateListService.GetDefaultState(this.OldActualFiniteStateList, this.OldActualFiniteStates, partition, securityContext, transaction);
            }

            if (this.IsOldOptionDependent)
            {
                if (iteration == null)
                {
                    throw new ArgumentNullException(nameof(iteration));
                }

                var options = this.OptionService.GetShallow(transaction, partition, iteration.Option.Select(x => Guid.Parse(x.V.ToString())), securityContext).Cast<Option>().ToList();

                this.OptionBusinessLogicService.Initialize(iteration, options);
                this.OldDefaultOption = this.OptionBusinessLogicService.GetDefaultOption();
            }
        }

        /// <summary>
        /// Gets the source <see cref="ParameterValueSet"/> for the new one to be created for a specified option and state
        /// </summary>
        /// <param name="option">The identifier of the option</param>
        /// <param name="state">The identifier of the state</param>
        /// <returns>The source <see cref="ParameterValueSet"/></returns>
        public ParameterValueSet GetsourceValueSet(Guid? option, Guid? state)
        {
            if (this.OldParameter == null)
            {
                return null;
            }

            if (!this.IsOldStateDependent && !this.IsOldOptionDependent)
            {
                return this.OldValueSet.FirstOrDefault();
            }

            // new is neither option nor state dependent
            if (!option.HasValue && !state.HasValue)
            {
                // old is both option and state dependent
                if (this.IsOldStateDependent && this.IsOldOptionDependent)
                {
                    if (this.OldDefaultOption != null && this.OldDefaultState != null)
                    {
                        return this.OldValueSet.FirstOrDefault(x => x.ActualState == this.OldDefaultState.Iid && x.ActualOption == this.OldDefaultOption.Iid);
                    }

                    if (this.OldDefaultOption != null)
                    {
                        return this.OldValueSet.FirstOrDefault(x => x.ActualOption == this.OldDefaultOption.Iid);
                    }

                    return this.OldDefaultState != null ? this.OldValueSet.FirstOrDefault(x => x.ActualState == this.OldDefaultState.Iid) : this.OldValueSet.FirstOrDefault();
                }

                // old is only option dependent
                if (this.IsOldOptionDependent)
                {
                    return this.OldDefaultOption != null ? this.OldValueSet.FirstOrDefault(x => x.ActualOption == this.OldDefaultOption.Iid) : this.OldValueSet.FirstOrDefault();
                }

                // old is only state dependent
                return this.OldDefaultState != null ? this.OldValueSet.FirstOrDefault(x => x.ActualState == this.OldDefaultState.Iid) : this.OldValueSet.FirstOrDefault();
            }

            // new is only option dependent
            if (option.HasValue && !state.HasValue)
            {
                // old is both state/option dependent
                if (this.IsOldStateDependent && this.IsOldOptionDependent)
                {
                    return this.OldDefaultState != null ? this.OldValueSet.FirstOrDefault(x => x.ActualState == this.OldDefaultState.Iid && x.ActualOption == option.Value) : this.OldValueSet.FirstOrDefault(x => x.ActualOption == option.Value);
                }

                // old is only state dependent
                return this.OldDefaultState != null ? this.OldValueSet.FirstOrDefault(x => x.ActualState == this.OldDefaultState.Iid) : this.OldValueSet.FirstOrDefault();
            }

            // new is only state dependent
            if (!option.HasValue)
            {
                // old is option/state dependent
                if (this.IsOldStateDependent && this.IsOldOptionDependent)
                {
                    // by priority get the old value watching the same state, else the default state else get the first value
                    return this.OldDefaultOption != null
                        ? this.OldValueSet.FirstOrDefault(x => x.ActualOption == this.OldDefaultOption.Iid && x.ActualState == state.Value)
                            ?? this.OldValueSet.FirstOrDefault(x => x.ActualOption == this.OldDefaultOption.Iid && this.OldDefaultState != null && x.ActualState == this.OldDefaultState.Iid)
                            ?? this.OldValueSet.FirstOrDefault()
                        : this.OldValueSet.FirstOrDefault(x => x.ActualState == state.Value)
                          ?? this.OldValueSet.FirstOrDefault(x => this.OldDefaultState != null && x.ActualState == this.OldDefaultState.Iid)
                          ?? this.OldValueSet.FirstOrDefault();
                }

                // old is option dependent
                return this.OldDefaultOption != null ? this.OldValueSet.FirstOrDefault(x => x.ActualOption == this.OldDefaultOption.Iid) : this.OldValueSet.FirstOrDefault();
            }

            // new option/state dependent
            // old is option dependent
            if (this.IsOldOptionDependent)
            {
                return this.OldValueSet.FirstOrDefault(x => x.ActualOption == option.Value);
            }

            // old is state dependent
            // by priority get the old value watching the same state, else the default state else get the first value
            return this.OldValueSet.FirstOrDefault(x => x.ActualState == state.Value)
                    ?? this.OldValueSet.FirstOrDefault(x => this.OldDefaultState != null && x.ActualState == this.OldDefaultState.Iid)
                    ?? this.OldValueSet.FirstOrDefault();
        }
    }
}
