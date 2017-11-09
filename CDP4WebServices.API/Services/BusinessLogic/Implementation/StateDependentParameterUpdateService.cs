// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateDependentParameterUpdateService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services.Authorization;
    using Npgsql;

    /// <summary>
    /// A service that handles update on <see cref="ActualFiniteStateList"/>, <see cref="ActualFiniteState"/> and <see cref="ParameterBase"/> and their value-set
    /// </summary>
    public class StateDependentParameterUpdateService : IStateDependentParameterUpdateService
    {
        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the parameter <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the parameter <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICompoundParameterTypeService"/>
        /// </summary>
        public ICompoundParameterTypeService CompoundParameterTypeService { get; set; }

        /// <summary>
        /// Update all the relevant <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="actualFiniteStateList">The updated <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldActualStateMap">The map that links the new to old <see cref="ActualFiniteState"/></param>
        public void UpdateAllStateDependentParameters(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyDictionary<ActualFiniteState, ActualFiniteState> newOldActualStateMap)
        {
            if (iteration == null)
            {
                throw new ArgumentNullException("iteration");
            }

            var parameters = this.ParameterService.GetShallow(transaction, partition, null, securityContext).Where(i => i.GetType() == typeof(Parameter)).Cast<Parameter>()
                .Where(x => x.StateDependence == actualFiniteStateList.Iid).ToList();
            var parameterOverrides = this.ParameterOverrideService.GetShallow(transaction, partition, null, securityContext).Where(i => i.GetType() == typeof(ParameterOverride)).Cast<ParameterOverride>()
                .Where(x => parameters.Select(p => p.Iid).Contains(x.Parameter)).ToList();

            // update the parameters with the new actual states
            var newOldParameterValueSetMap = new Dictionary<Parameter, IDictionary<ParameterValueSet, ParameterValueSet>>();
            foreach (var parameter in parameters)
            {
                var tmpMap = new Dictionary<ParameterValueSet, ParameterValueSet>();
                this.UpdateParameter(parameter, iteration, transaction, partition, securityContext, newOldActualStateMap, ref tmpMap);
                newOldParameterValueSetMap.Add(parameter, tmpMap);
            }

            // update the parameter override from the updated parameters
            var parameterOrOVerrideValueSetMap = new Dictionary<ParameterOrOverrideBase, IReadOnlyDictionary<ParameterValueSetBase, ParameterValueSetBase>>();
            foreach (var pair in newOldParameterValueSetMap)
            {
                parameterOrOVerrideValueSetMap.Add(pair.Key, pair.Value.ToDictionary(newSet => (ParameterValueSetBase)newSet.Key, oldSet => (ParameterValueSetBase)oldSet.Value));
            }

            foreach (var parameterOverride in parameterOverrides)
            {
                var tmpMap = new Dictionary<ParameterValueSetBase, ParameterValueSetBase>();
                var overridenParameter = parameters.Single(x => x.Iid == parameterOverride.Parameter);
                this.UpdateParameterOverride(parameterOverride, transaction, partition, securityContext, newOldParameterValueSetMap[overridenParameter], ref tmpMap);
                parameterOrOVerrideValueSetMap.Add(parameterOverride, tmpMap);
            }

            // update the parameter subscription from the updated parameter/overide value sets
            var parameterOrOverrides = parameters.Cast<ParameterOrOverrideBase>().Union(parameterOverrides).ToList();
            var parameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, partition, null, securityContext).Where(i => i.GetType() == typeof(ParameterSubscription)).Cast<ParameterSubscription>()
                .Where(x => parameterOrOverrides.SelectMany(p => p.ParameterSubscription).Contains(x.Iid));
            foreach (var parameterSubscription in parameterSubscriptions)
            {
                var subscribedParameterOrOverride = parameterOrOverrides.Single(x => x.ParameterSubscription.Contains(parameterSubscription.Iid));
                this.UpdateParameterSubscription(parameterSubscription, transaction, partition, securityContext, parameterOrOVerrideValueSetMap[subscribedParameterOrOverride]);
            }
        }

        #region Update Parameters
        /// <summary>
        /// Update a <see cref="Parameter"/> with new <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldActualStateMap">The map that links the new to old <see cref="ActualFiniteState"/></param>
        /// <param name="newOldValueSetMap">The resulting map that links the new to old <see cref="ParameterValueSet"/></param>
        private void UpdateParameter(Parameter parameter, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyDictionary<ActualFiniteState, ActualFiniteState> newOldActualStateMap, ref Dictionary<ParameterValueSet, ParameterValueSet> newOldValueSetMap)
        {
            var oldValueSets = this.ParameterValueSetService.GetShallow(transaction, partition, parameter.ValueSet, securityContext)
                    .Where(i => i.GetType() == typeof(ParameterValueSet)).Cast<ParameterValueSet>().ToList();

            if (parameter.IsOptionDependent)
            {
                foreach (var orderedItem in iteration.Option.OrderBy(x => x.K))
                {
                    var actualOption = Guid.Parse(orderedItem.V.ToString());
                    this.CreateParameterValueSets(parameter, actualOption, transaction, partition, securityContext, newOldActualStateMap, oldValueSets, ref newOldValueSetMap);
                }
            }
            else
            {
                this.CreateParameterValueSets(parameter, null, transaction, partition, securityContext, newOldActualStateMap, oldValueSets, ref newOldValueSetMap);
            }
        }

        /// <summary>
        /// Create new <see cref="ParameterValueSet"/> for a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        /// <param name="actualOption">The actual <see cref="Option"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldActualStateMap">The map that links the new <see cref="ActualFiniteState"/> to the old ones</param>
        /// <param name="oldValueSets">The old <see cref="ParameterValueSet"/></param>
        /// <param name="newOldValueSetMap">The map that links the new to old <see cref="ParameterValueSet"/></param>
        private void CreateParameterValueSets(Parameter parameter, Guid? actualOption, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyDictionary<ActualFiniteState, ActualFiniteState> newOldActualStateMap, IEnumerable<ParameterValueSet> oldValueSets, ref Dictionary<ParameterValueSet, ParameterValueSet> newOldValueSetMap)
        {
            if (newOldActualStateMap == null || !newOldActualStateMap.Any())
            {
                // the parameter lost the state dependency
                var oldValueSet = oldValueSets.FirstOrDefault(x => x.ActualOption == actualOption);
                var newValueset = this.CreateParameterValueSet(oldValueSet, parameter, actualOption, null, transaction, partition, securityContext);
                newOldValueSetMap.Add(newValueset, oldValueSet);
                return;
            }

            foreach (var newOldStatePair in newOldActualStateMap)
            {
                ParameterValueSet oldValueSet;
                if (newOldStatePair.Value == null)
                {
                    oldValueSet = null;
                }
                else
                {
                    oldValueSet =
                        oldValueSets.SingleOrDefault(
                            x =>
                                x.ActualOption == actualOption &&
                                x.ActualState == newOldStatePair.Value.Iid);
                }

                var newValueSet = this.CreateParameterValueSet(oldValueSet, parameter, actualOption, newOldStatePair.Key.Iid, transaction, partition, securityContext);
                newOldValueSetMap.Add(newValueSet, oldValueSet);
            }
        }

        /// <summary>
        /// Create a <see cref="ParameterValueSet"/> with a specific option and state. The old value set is used if it exists, otherwise default values are used
        /// </summary>
        /// <param name="oldValue">The old <see cref="ParameterValueSet"/></param>
        /// <param name="parameter">The current <see cref="Parameter"/></param>
        /// <param name="actualOption">The actual <see cref="CDP4Common.DTO.Option"/></param>
        /// <param name="actualState">The <see cref="ActualFiniteState"/></param>
        /// <param name="transaction">The transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContex">The security context</param>
        /// <returns>The created <see cref="ParameterValueSet"/></returns>
        private ParameterValueSet CreateParameterValueSet(ParameterValueSet oldValue, Parameter parameter, Guid? actualOption, Guid? actualState, NpgsqlTransaction transaction, string partition, ISecurityContext securityContex)
        {
            var numberOfComponent = 0;
            if (oldValue == null)
            {
                numberOfComponent = this.GetNumberOfComponent(parameter, transaction, securityContex);
            }

            var defaultValue = new List<string>(numberOfComponent);
            for (var i = 0; i < numberOfComponent; i++)
            {
                defaultValue.Add("-");
            }

            var isOldValueNull = oldValue == null;
            var valueSet = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(isOldValueNull ? (IEnumerable<string>)defaultValue : oldValue.Manual),
                Computed = new ValueArray<string>(isOldValueNull ? (IEnumerable<string>)defaultValue : oldValue.Computed),
                Reference = new ValueArray<string>(isOldValueNull ? (IEnumerable<string>)defaultValue : oldValue.Reference),
                Published = new ValueArray<string>(isOldValueNull ? (IEnumerable<string>)defaultValue : oldValue.Published),
                Formula = new ValueArray<string>(isOldValueNull ? (IEnumerable<string>)defaultValue : oldValue.Formula),
                ActualOption = actualOption,
                ActualState = actualState,
                ValueSwitch = isOldValueNull ? CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL : oldValue.ValueSwitch
            };

            this.ParameterValueSetService.CreateConcept(transaction, partition, valueSet, parameter);
            return valueSet;
        }
        #endregion

        #region ParameterOVerride
        /// <summary>
        /// Update a <see cref="ParameterOverride"/> with new <see cref="ParameterOverrideValueSet"/>
        /// </summary>
        /// <param name="parameterOverride">The <see cref="ParameterOverride"/></param>
        /// <param name="transaction">The transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldParameterValueSetMap">The map that links the new <see cref="ParameterValueSet"/> to the old ones</param>
        /// <param name="newOldValueSetMap">A map that links the created <see cref="ParameterOverrideValueSet"/> to the old ones</param>
        private void UpdateParameterOverride(ParameterOverride parameterOverride, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IDictionary<ParameterValueSet, ParameterValueSet> newOldParameterValueSetMap, ref Dictionary<ParameterValueSetBase, ParameterValueSetBase> newOldValueSetMap)
        {
            var oldValueSets =
                this.ParameterOverrideValueSetService.GetShallow(transaction, partition, parameterOverride.ValueSet, securityContext)
                    .Where(i => i.GetType() == typeof(ParameterOverrideValueSet)).Cast<ParameterOverrideValueSet>().ToList();

            foreach (var newOldParameterValueSetPair in newOldParameterValueSetMap)
            {
                ParameterOverrideValueSet newValueSet;
                if (newOldParameterValueSetPair.Value != null)
                {
                    // there should be a override value set counter-part
                    var oldValueSet = oldValueSets.SingleOrDefault(x => x.ParameterValueSet == newOldParameterValueSetPair.Value.Iid);
                    newValueSet = this.CreateParameterOverrideValueSet(oldValueSet, newOldParameterValueSetPair.Key, parameterOverride, transaction, partition);
                    newOldValueSetMap.Add(newValueSet, oldValueSet);
                }
                else
                {
                    newValueSet = this.CreateParameterOverrideValueSet(null, newOldParameterValueSetPair.Key, parameterOverride, transaction, partition);
                    newOldValueSetMap.Add(newValueSet, null);
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="ParameterOverrideValueSet"/> from the old values
        /// </summary>
        /// <param name="oldValue">The old <see cref="ParameterOverrideValueSet"/></param>
        /// <param name="parameterValueSet">The <see cref="ParameterValueSet"/></param>
        /// <param name="container">The <see cref="ParameterOverride"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <returns>The new <see cref="ParameterOverrideValueSet"/></returns>
        private ParameterOverrideValueSet CreateParameterOverrideValueSet(ParameterOverrideValueSet oldValue, ParameterValueSet parameterValueSet, ParameterOverride container, NpgsqlTransaction transaction, string partition)
        {
            var isOldValueNull = oldValue == null;
            var newValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(isOldValueNull ? parameterValueSet.Manual : oldValue.Manual),
                Computed = new ValueArray<string>(isOldValueNull ? parameterValueSet.Computed : oldValue.Computed),
                Reference = new ValueArray<string>(isOldValueNull ? parameterValueSet.Reference : oldValue.Reference),
                Published = new ValueArray<string>(isOldValueNull ? parameterValueSet.Published : oldValue.Published),
                Formula = new ValueArray<string>(isOldValueNull ? parameterValueSet.Formula : oldValue.Formula),
                ParameterValueSet = parameterValueSet.Iid,
                ValueSwitch = isOldValueNull ? CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL : oldValue.ValueSwitch
            };

            this.ParameterOverrideValueSetService.CreateConcept(transaction, partition, newValueSet, container);
            return newValueSet;
        }
        #endregion

        #region Parameter Subscription
        /// <summary>
        /// Update a <see cref="ParameterSubscription"/> with new <see cref="ParameterSubscriptionValueSet"/>
        /// </summary>
        /// <param name="parameterSubscription">The <see cref="ParameterSubscription"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldValueSetBaseMap">The map linking the old subscribed <see cref="ParameterValueSetBase"/> to the new ones</param>
        private void UpdateParameterSubscription(ParameterSubscription parameterSubscription, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyDictionary<ParameterValueSetBase, ParameterValueSetBase> newOldValueSetBaseMap)
        {
            var oldValueSets = this.ParameterSubscriptionValueSetService.GetShallow(transaction, partition, parameterSubscription.ValueSet, securityContext)
                    .Where(i => i.GetType() == typeof(ParameterSubscriptionValueSet)).Cast<ParameterSubscriptionValueSet>().ToList();

            foreach (var newOldParameterValueSetPair in newOldValueSetBaseMap)
            {
                if (newOldParameterValueSetPair.Value != null)
                {
                    var oldValueSet = oldValueSets.SingleOrDefault(x => x.SubscribedValueSet == newOldParameterValueSetPair.Value.Iid);
                    this.CreateParameterSubscriptionValueSet(oldValueSet, newOldParameterValueSetPair.Key, parameterSubscription, transaction, partition);
                }
                else
                {
                    this.CreateParameterSubscriptionValueSet(null, newOldParameterValueSetPair.Key, parameterSubscription, transaction, partition);
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="ParameterSubscriptionValueSet"/> from the updated <see cref="ParameterValueSetBase"/>
        /// </summary>
        /// <param name="oldValue">The old <see cref="ParameterSubscriptionValueSet"/></param>
        /// <param name="valueSetBase">The subscribed <see cref="ParameterValueSetBase"/></param>
        /// <param name="container">The container</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        private void CreateParameterSubscriptionValueSet(ParameterSubscriptionValueSet oldValue, ParameterValueSetBase valueSetBase, ParameterSubscription container, NpgsqlTransaction transaction, string partition)
        {
            var isOldValueNull = oldValue == null;
            var newValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>((oldValue == null) ? valueSetBase.Manual : oldValue.Manual),
                ValueSwitch = isOldValueNull ? CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL : oldValue.ValueSwitch,
                SubscribedValueSet = valueSetBase.Iid
            };

            this.ParameterSubscriptionValueSetService.CreateConcept(transaction, partition, newValueSet, container);
        }
        #endregion

        /// <summary>
        /// Gets the number of component related to a <see cref="ParameterType"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        /// <returns>The number of component</returns>
        private int GetNumberOfComponent(Parameter parameter, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var cptParameterType =
                this.CompoundParameterTypeService.GetShallow(transaction, "SiteDirectory", new List<Guid> { parameter.ParameterType }, securityContext).Cast<CompoundParameterType>().SingleOrDefault();

            // considering the compnents are all scalar
            return cptParameterType == null ? 1 : cptParameterType.Component.Count;
        }
    }
}