// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;

    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;
    using Npgsql;

    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;

    /// <summary>
    /// The purpose of the <see cref="ParameterSubscriptionSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSubscriptionSideEffect : OperationSideEffect<ParameterSubscription>
    {
        #region Injected services
        
        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

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
        public override void BeforeCreate(
            ParameterSubscription thing, 
            Thing container, 
            NpgsqlTransaction transaction, 
            string partition, 
            ISecurityContext securityContext)
        {
            this.CheckOwnership(thing, container);
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
            this.CreateParameterSubscriptionValueSets(thing, container, transaction, partition);
        }

        /// <summary>
        /// Creates ParameterSubscriptionValueSets for the supplied ParameterValueSets.
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
        private void CreateParameterSubscriptionValueSets(
            ParameterSubscription thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition)
        {
            foreach (var parameterValueSetId in ((CDP4Common.DTO.ParameterOrOverrideBase)container).ValueSets)
            {
                var parameterSubscriptionValueSet =
                    new ParameterSubscriptionValueSet(Guid.NewGuid(), 0)
                        {
                            SubscribedValueSet = parameterValueSetId,
                            ValueSwitch = ParameterSwitchKind.COMPUTED
                        };

                var defaultValue = new List<string>(1);
                defaultValue.Add("-");

                var valueArray = new ValueArray<string>(defaultValue);
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
    }
}