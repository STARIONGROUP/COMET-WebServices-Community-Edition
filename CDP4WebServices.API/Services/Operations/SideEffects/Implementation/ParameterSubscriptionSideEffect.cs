// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authorization;
    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;
    using CDP4Common.Types;
    using NLog;
    using Npgsql;
    using ParameterOrOverrideBase = CDP4Common.DTO.ParameterOrOverrideBase;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;
    using ParameterValueSetBase = CDP4Common.DTO.ParameterValueSetBase;

    /// <summary>
    /// The purpose of the <see cref="ParameterSubscriptionSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSubscriptionSideEffect : OperationSideEffect<ParameterSubscription>
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Injected services
        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }
        #endregion

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation
        {
            get
            {
                return new[] { "ValueSet" };
            }
        }
        
        /// <summary>
        /// Execute additional logic  before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
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
        public override bool BeforeCreate(
            ParameterSubscription thing, 
            Thing container, 
            NpgsqlTransaction transaction, 
            string partition, 
            ISecurityContext securityContext)
        {
            if (thing.Owner == Guid.Empty)
            {
                throw new InvalidOperationException("The owner cannot be empty.");
            }

            this.CheckOwnership(thing, container);

            return this.IsUniqueSubscription(transaction, partition, securityContext, thing, container);
        }

        /// <summary>
        /// Execute additional logic  a successful create operation.
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
        public override void AfterCreate(
            ParameterSubscription thing,
            Thing container,
            ParameterSubscription originalThing,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            this.CreateParameterSubscriptionValueSets(thing, container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Creates ParameterSubscriptionValueSets for the supplied ParameterValueSets.
        /// </summary>
        /// <param name="thing">
        ///     The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        ///     The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        ///     The current transaction to the database.
        /// </param>
        /// <param name="partition">
        ///     The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        ///     The security-context
        /// </param>
        private void CreateParameterSubscriptionValueSets(
            ParameterSubscription thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            if (!(container is CDP4Common.DTO.ParameterOrOverrideBase parameterOrOverrideBase))
            {
                throw new InvalidOperationException("The container of a ParameterSubscription can only be a ParameterOrOverrideBase.");
            }

            var paramContainer = container is CDP4Common.DTO.Parameter
                    ? (ParameterOrOverrideBase)this.ParameterService.GetShallow(transaction, partition, new[] {container.Iid}, securityContext).OfType<CDP4Common.DTO.Parameter>().SingleOrDefault()
                    : this.ParameterOverrideService.GetShallow(transaction, partition, new[] {container.Iid}, securityContext).OfType<CDP4Common.DTO.ParameterOverride>().SingleOrDefault();
            

            if (paramContainer == null || !paramContainer.ValueSets.Any())
            {
                throw new InvalidOperationException($"Could not determine the value-set to subscribe on for the parameter-subscription {thing.Iid} to create on parameter/override {container.Iid}");
            }

            var parameterValueSets = parameterOrOverrideBase is CDP4Common.DTO.Parameter
                ? this.ParameterValueSetService.GetShallow(transaction, partition, paramContainer.ValueSets, securityContext).OfType<CDP4Common.DTO.ParameterValueSetBase>().ToArray()
                : this.ParameterOverrideValueSetService.GetShallow(transaction, partition, paramContainer.ValueSets, securityContext).OfType<CDP4Common.DTO.ParameterValueSetBase>().ToArray();

            foreach (var parameterValueSet in parameterValueSets)
            {
                var parameterSubscriptionValueSet =
                    new ParameterSubscriptionValueSet(Guid.NewGuid(), 0)
                        {
                            SubscribedValueSet = parameterValueSet.Iid,
                            ValueSwitch = ParameterSwitchKind.COMPUTED
                        };

                var valueArray = new ValueArray<string>(this.DefaultValueArrayFactory.CreateDefaultValueArray(parameterValueSet.Manual.Count));
                parameterSubscriptionValueSet.Manual = valueArray;

                this.ParameterSubscriptionValueSetService.CreateConcept(
                    transaction,
                    partition,
                    parameterSubscriptionValueSet,
                    thing);
            }
        }

        /// <summary>
        /// Execute additional logic  before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
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
        /// <param name="rawUpdateInfo">
        /// The raw Update Info.
        /// </param>
        public override void BeforeUpdate(
            ParameterSubscription thing, 
            Thing container, 
            NpgsqlTransaction transaction, 
            string partition, 
            ISecurityContext securityContext, 
            ClasslessDTO rawUpdateInfo)
        {
            this.CheckOwnership(thing, container);
        }
        
        /// <summary>
        /// Checks whether a Thing and its container have different owners.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        private void CheckOwnership(ParameterSubscription thing, Thing container)
        {
            if (thing.Owner == ((CDP4Common.DTO.ParameterOrOverrideBase)container).Owner)
            {
                throw new Cdp4ModelValidationException(
                    "Parameter and ParameterSubscription cannot have the same owner.");
            }
        }

        /// <summary>
        /// Check whether the subscription to create on a <paramref name="container"/> is unique owner-wise
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security-context</param>
        /// <param name="newSubscription">The subscription that is being created</param>
        /// <param name="container">The container</param>
        /// <returns>
        /// Returns true if the create operation may continue, otherwise it shall be skipped.
        /// </returns>
        private bool IsUniqueSubscription(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ParameterSubscription newSubscription, Thing container)
        {
            var parameterOrOverride =
                (ParameterOrOverrideBase)this.ParameterService.GetShallow(transaction, partition, new[] {container.Iid}, securityContext).SingleOrDefault()
                ?? (ParameterOrOverrideBase)this.ParameterOverrideService.GetShallow(transaction, partition, new[] {container.Iid}, securityContext).SingleOrDefault();

            if (parameterOrOverride == null)
            {
                throw new InvalidOperationException("The container of a new parameter-subscription can only be a ParameterOrOverrideBase");
            }

            var existingSubscription = this.ParameterSubscriptionService.GetShallow(transaction, partition, parameterOrOverride.ParameterSubscription, securityContext).OfType<ParameterSubscription>().
                FirstOrDefault(x => x.Owner == newSubscription.Owner);

            if (existingSubscription != null)
            {
                Logger.Warn("A subscription already exist on parameter {0}.", container.Iid);
                return false;
            }

            return true;
        }
    }
}