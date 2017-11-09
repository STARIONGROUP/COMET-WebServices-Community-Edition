// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authorization;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4Orm.Dao;
    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParameterSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSideEffect : OperationSideEffect<Parameter>
    {
        #region Injected services

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOptionService"/>
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeService"/>
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeComponentService"/>
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetFactory"/>
        /// </summary>
        public IParameterValueSetFactory ParameterValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetFactory"/>
        /// </summary>
        public IParameterOverrideValueSetFactory ParameterOverrideValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetFactory"/>
        /// </summary>
        public IParameterSubscriptionValueSetFactory ParameterSubscriptionValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService"/>
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        #endregion

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation
        {
            get
            {
                return new[] {"ValueSet"};
            }
        }

        /// <summary>
        /// Execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterCreate(Parameter thing, Thing container, Parameter originalThing,
            NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (originalThing.ValueSet.Any())
            {
                // we return here to avoid creating default value-sets during a copy, ie avoid duplicates
                return;
            }

            // creates default value-arrays for the parameter
            this.InitializeDefaultValueArrayFactory(transaction, securityContext);
            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(thing.ParameterType);

            var newValueSet = this.ComputeValueSets(thing, transaction, partition, securityContext, defaultValueArray);
            this.WriteValueSet(transaction, partition, thing, newValueSet);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterUpdate(Parameter thing, Thing container, Parameter originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var isOptionDependencyChanged = thing.IsOptionDependent != originalThing.IsOptionDependent;
            var isStateDependencyChanged = thing.StateDependence != originalThing.StateDependence;
            var isOwnerChanged = thing.Owner != originalThing.Owner;

            // do nothing is the option and state dependency has not changed
            if (!isOwnerChanged && !isOptionDependencyChanged && !isStateDependencyChanged)
            {
                return;
            }

            this.InitializeDefaultValueArrayFactory(transaction, securityContext);
            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(thing.ParameterType);

            var newValueSet = this.ComputeValueSets(thing, transaction, partition, securityContext, defaultValueArray).ToList();
            this.WriteValueSet(transaction, partition, thing, newValueSet);

            var parameterOverrides =
                this.ParameterOverrideService.GetShallow(transaction, partition, null, securityContext)
                    .Where(i => i.GetType() == typeof(ParameterOverride)).Cast<ParameterOverride>()
                    .Where(x => x.Parameter == thing.Iid)
                    .ToList();

            var elementUsages = this.ElementUsageService.GetShallow(transaction, partition, null, securityContext)
                .Cast<ElementUsage>()
                .Where(x => x.ElementDefinition == container.Iid)
                .ToList();

            var parameterSubscriptionIds = new List<Guid>(thing.ParameterSubscription);
            parameterSubscriptionIds.AddRange(parameterOverrides.SelectMany(x => x.ParameterSubscription));

            var parameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, partition, parameterSubscriptionIds, securityContext)
                    .Where(i => i.GetType() == typeof(ParameterSubscription)).Cast<ParameterSubscription>().ToList();

            foreach (var parameterOverride in parameterOverrides)
            {
                if (!thing.AllowDifferentOwnerOfOverride)
                {
                    // Set the owner of the ParameterOverride to the new owner of the Parameter
                    parameterOverride.Owner = thing.Owner;
                    var elementUsage = elementUsages.SingleOrDefault(x => x.ParameterOverride.Contains(parameterOverride.Iid));
                    if (elementUsage == null)
                    {
                        throw new InvalidOperationException("The ElementUsage could not be retrieved.");
                    }

                    this.ParameterOverrideService.UpdateConcept(transaction, partition, parameterOverride, elementUsage);
                }

                this.UpdateParameterOverrideAndSubscription(parameterOverride, newValueSet, parameterSubscriptions,
                    defaultValueArray, transaction, partition);
            }

            // Remove the subscriptions owned by the new owner of the Parameter
            if (isOwnerChanged && parameterSubscriptions.Any(s => s.Owner == thing.Owner))
            {
                var parameterSubscriptionToRemove =
                    parameterSubscriptions.SingleOrDefault(
                        s => s.Owner == thing.Owner && thing.ParameterSubscription.Contains(s.Iid));
                if (parameterSubscriptionToRemove != null)
                {
                    if (this.ParameterSubscriptionService.DeleteConcept(transaction, partition,
                        parameterSubscriptionToRemove, thing))
                    {
                        parameterSubscriptions.Remove(parameterSubscriptionToRemove);
                    }
                }

                // Remove the subscriptions owned by the new owner of the Parameter on all parameterOverrides
                foreach (var parameterOverride in parameterOverrides.Where(o => o.ParameterSubscription.Any()))
                {
                    var subscriptionToRemove =
                        parameterSubscriptions.SingleOrDefault(
                            s =>
                                parameterOverride.ParameterSubscription.Contains(s.Iid) &&
                                s.Owner == parameterOverride.Owner);
                    if (subscriptionToRemove != null)
                    {
                        if (this.ParameterSubscriptionService.DeleteConcept(transaction, partition, subscriptionToRemove,
                            thing))
                        {
                            parameterSubscriptions.Remove(subscriptionToRemove);
                        }
                    }
                }
            }

            this.UpdateParameterSubscriptions(thing, newValueSet, parameterSubscriptions, defaultValueArray, transaction,
                partition);

            // clean up old values
            this.DeleteOldValueSet(transaction, partition, originalThing);
        }

        /// <summary>
        /// Update the associated <see cref="ParameterOverride"/> and their <see cref="ParameterSubscription"/>
        /// </summary>
        /// <param name="parameterOverride">The <see cref="ParameterOverride"/></param>
        /// <param name="newValueSet">The new <see cref="ParameterValueSet"/></param>
        /// <param name="parameterSubscriptions">The <see cref="ParameterSubscription"/></param>
        /// <param name="defaultValueArray">The default value array</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        private void UpdateParameterOverrideAndSubscription(ParameterOverride parameterOverride,
            IEnumerable<ParameterValueSet> newValueSet, IEnumerable<ParameterSubscription> parameterSubscriptions,
            ValueArray<string> defaultValueArray, NpgsqlTransaction transaction, string partition)
        {
            var overrideSubscription =
                parameterSubscriptions.Where(x => parameterOverride.ParameterSubscription.Contains(x.Iid)).ToList();
            foreach (var parameterValueSet in newValueSet)
            {
                var newValueSetOverride =
                    this.ParameterOverrideValueSetFactory.CreateWithDefaultValueArray(parameterValueSet.Iid,
                        defaultValueArray);
                this.ParameterOverrideValueSetService.CreateConcept(transaction, partition, newValueSetOverride,
                    parameterOverride);
                foreach (var parameterSubscription in overrideSubscription)
                {
                    var newSubscriptionValueSet =
                        this.ParameterSubscriptionValueSetFactory.CreateWithDefaultValueArray(newValueSetOverride.Iid,
                            defaultValueArray);
                    this.ParameterSubscriptionValueSetService.CreateConcept(transaction, partition,
                        newSubscriptionValueSet, parameterSubscription);
                }
            }
        }

        /// <summary>
        /// Update the <see cref="ParameterSubscription"/> associated to a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        /// <param name="newValueSet">The new <see cref="ParameterValueSet"/></param>
        /// <param name="parameterSubscriptions">The <see cref="ParameterSubscription"/></param>
        /// <param name="defaultValueArray">The default value array</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        private void UpdateParameterSubscriptions(Parameter parameter, IEnumerable<ParameterValueSet> newValueSet,
            IEnumerable<ParameterSubscription> parameterSubscriptions, ValueArray<string> defaultValueArray,
            NpgsqlTransaction transaction, string partition)
        {
            var subscriptions = parameterSubscriptions.Where(x => parameter.ParameterSubscription.Contains(x.Iid)).ToList();
            foreach (var parameterValueSet in newValueSet)
            {
                foreach (var parameterSubscription in subscriptions)
                {
                    var newSubscriptionValueSet =
                        this.ParameterSubscriptionValueSetFactory.CreateWithDefaultValueArray(parameterValueSet.Iid,
                            defaultValueArray);
                    this.ParameterSubscriptionValueSetService.CreateConcept(transaction, partition,
                        newSubscriptionValueSet, parameterSubscription);
                }
            }
        }

        /// <summary>
        /// Queries the data-store for all the <see cref="ParameterType"/>s and <see cref="ParameterTypeComponent"/>s to initialize the <see cref="DefaultValueArrayFactory"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        private void InitializeDefaultValueArrayFactory(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var parameterTypes =
                this.ParameterTypeService.GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null,
                    securityContext).Cast<ParameterType>();
            var parameterTypeComponents =
                this.ParameterTypeComponentService.GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                    null, securityContext).Cast<ParameterTypeComponent>();

            this.DefaultValueArrayFactory.Initialize(parameterTypes, parameterTypeComponents);
        }

        /// <summary>
        /// Compute the <see cref="ParameterValueSet"/> for a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="Parameter"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The current partition
        /// </param>
        /// <param name="securityContext">
        /// The security context
        /// </param>
        /// <param name="defaultValueArray">
        /// The default Value Array.
        /// </param>
        /// <returns>
        /// The new <see cref="ParameterValueSet"/>s
        /// </returns>
        private IEnumerable<ParameterValueSet> ComputeValueSets(Parameter parameter, NpgsqlTransaction transaction,
            string partition, ISecurityContext securityContext, ValueArray<string> defaultValueArray)
        {
            var newValueSet = new List<ParameterValueSet>();
            if (parameter.IsOptionDependent)
            {
                // redirect out the iteration partition to the engineeringmodel one, as that contains the iteration information
                var engineeringModelPartition = partition.Replace(
                    Utils.IterationSubPartition,
                    Utils.EngineeringModelPartition);
                var iteration =
                    this.IterationService.GetShallow(transaction, engineeringModelPartition, null, securityContext)
                        .Where(i => i.GetType() == typeof(Iteration)).Cast<Iteration>()
                        .SingleOrDefault();

                if (iteration == null)
                {
                    throw new KeyNotFoundException("The iteration could not be found");
                }

                newValueSet.AddRange(this.CreateDefaultOptionDependentValueSetCollection(parameter, iteration,
                    transaction, partition, securityContext, defaultValueArray));
            }
            else if (parameter.StateDependence != null)
            {
                var actualList =
                    this.ActualFiniteStateListService.GetShallow(transaction, partition,
                            new[] {parameter.StateDependence.Value}, securityContext)
                        .Where(i => i.GetType() == typeof(ActualFiniteStateList)).Cast<ActualFiniteStateList>()
                        .SingleOrDefault();
                if (actualList == null)
                {
                    throw new KeyNotFoundException(string.Format("The ActualFiniteStateList {0} could not be found.",
                        parameter.StateDependence));
                }

                newValueSet.AddRange(this.CreateDefaultStateDependentValueSetCollection(actualList, null,
                    defaultValueArray));
            }
            else
            {
                newValueSet.Add(this.CreateDefaultParameterValueSet(null, null, defaultValueArray));
            }

            return newValueSet;
        }

        /// <summary>
        /// Create default option-dependent <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="Parameter"/>
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <param name="securityContext">
        /// The security context
        /// </param>
        /// <param name="defaultValueArray">
        /// The default Value Array.
        /// </param>
        /// <returns>
        /// The new <see cref="ParameterValueSet"/>
        /// </returns>
        private IEnumerable<ParameterValueSet> CreateDefaultOptionDependentValueSetCollection(Parameter parameter,
            Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext,
            ValueArray<string> defaultValueArray)
        {
            var newValueSet = new List<ParameterValueSet>();
            var options = iteration.Option.OrderBy(x => x.K);
            if (parameter.StateDependence != null)
            {
                var actualFiniteStateList =
                    this.ActualFiniteStateListService.GetShallow(transaction, partition,
                            new List<Guid> {parameter.StateDependence.Value}, securityContext)
                        .Where(i => i.GetType() == typeof(ActualFiniteStateList))
                        .Cast<ActualFiniteStateList>()
                        .SingleOrDefault();
                if (actualFiniteStateList == null)
                {
                    throw new KeyNotFoundException(
                        string.Format("The ActualFiniteStateList with id {0} could not be found",
                            parameter.StateDependence));
                }

                foreach (var option in options)
                {
                    newValueSet.AddRange(this.CreateDefaultStateDependentValueSetCollection(actualFiniteStateList,
                        Guid.Parse(option.V.ToString()), defaultValueArray));
                }
            }
            else
            {
                foreach (var option in options)
                {
                    newValueSet.Add(this.CreateDefaultParameterValueSet(Guid.Parse(option.V.ToString()), null,
                        defaultValueArray));
                }
            }

            return newValueSet;
        }

        /// <summary>
        /// Create default state-dependent <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="actualFiniteStateList">
        /// The <see cref="ActualFiniteStateList"/>
        /// </param>
        /// <param name="actualOption">
        /// The actual option
        /// </param>
        /// <param name="defaultValueArray">
        /// The default Value Array.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ParameterValueSet}"/>.
        /// </returns>
        private IEnumerable<ParameterValueSet> CreateDefaultStateDependentValueSetCollection(
            ActualFiniteStateList actualFiniteStateList, Guid? actualOption, ValueArray<string> defaultValueArray)
        {
            foreach (var state in actualFiniteStateList.ActualState)
            {
                yield return this.CreateDefaultParameterValueSet(actualOption, state, defaultValueArray);
            }
        }

        /// <summary>
        /// Delete the old <see cref="ParameterValueSet"/> that are obsolete
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The current partition
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        private void DeleteOldValueSet(NpgsqlTransaction transaction, string partition, Parameter originalThing)
        {
            foreach (var valueset in originalThing.ValueSet)
            {
                if (
                    !this.ParameterValueSetService.DeleteConcept(transaction, partition,
                        new ParameterValueSet(valueset, 0), originalThing))
                {
                    throw new InvalidOperationException(
                        string.Format("The delete operation of the parameter value set {0} failed.", valueset));
                }
            }
        }

        /// <summary>
        /// Write new <see cref="ParameterValueSet"/> into the database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="parameter">The current <see cref="Parameter"/></param>
        /// <param name="newValueSet">The <see cref="ParameterValueSet"/> to write</param>
        private void WriteValueSet(NpgsqlTransaction transaction, string partition, Parameter parameter,
            IEnumerable<ParameterValueSet> newValueSet)
        {
            foreach (var parameterValueSet in newValueSet)
            {
                this.ParameterValueSetService.CreateConcept(transaction, partition, parameterValueSet, parameter);
            }
        }

        /// <summary>
        /// Create a <see cref="ParameterValueSet"/> with default values, an owner and potentially actual option and state
        /// </summary>
        /// <param name="actualOption">
        /// The identifier of the actual <see cref="Option"/>
        /// </param>
        /// <param name="actualState">
        /// The identifier of the actual <see cref="ActualFiniteState"/>
        /// </param>
        /// <param name="defaultValueArray">
        /// The default values for the <see cref="ParameterValueSet"/>
        /// </param>
        /// <returns>
        /// The created <see cref="ParameterValueSet"/>.
        /// </returns>
        private ParameterValueSet CreateDefaultParameterValueSet(Guid? actualOption, Guid? actualState,
            ValueArray<string> defaultValueArray)
        {
            return this.ParameterValueSetFactory.CreateWithDefaultValueArray(actualOption, actualState,
                defaultValueArray);
        }
    }
}