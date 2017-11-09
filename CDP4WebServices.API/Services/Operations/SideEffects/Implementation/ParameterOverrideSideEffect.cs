// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
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
    /// The purpose of the <see cref="ParameterOverrideSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterOverrideSideEffect : OperationSideEffect<ParameterOverride>
    {
        #region Injected services

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetFactory"/>
        /// </summary>
        public IParameterOverrideValueSetFactory ParameterOverrideValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetFactory"/>
        /// </summary>
        public IParameterSubscriptionValueSetFactory ParameterSubscriptionValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

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
        public override void AfterCreate(ParameterOverride thing, Thing container, ParameterOverride originalThing,
            NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var newValueSet = this.ComputeValueSets(thing, transaction, partition, securityContext);
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
        public override void AfterUpdate(ParameterOverride thing, Thing container, ParameterOverride originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var isOwnerChanged = thing.Owner != originalThing.Owner;

            // Remove the subscriptions owned by the new owner of the ParameterOverride
            var parameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, partition, thing.ParameterSubscription, securityContext).OfType<ParameterSubscription>().ToArray();
            if (isOwnerChanged && parameterSubscriptions.Any(s => s.Owner == thing.Owner))
            {
                var parameterSubscriptionToRemove = parameterSubscriptions.SingleOrDefault(
                        s => s.Owner == thing.Owner && thing.ParameterSubscription.Contains(s.Iid));
                if (parameterSubscriptionToRemove != null)
                {
                    if (!this.ParameterSubscriptionService.DeleteConcept(transaction, partition, parameterSubscriptionToRemove, thing))
                    {
                        throw new InvalidOperationException(
                            string.Format("The update operation of the parameter override value set {0} failed.", parameterSubscriptionToRemove.Iid));
                    }
                }
            }
        }

        /// <summary>
        /// Compute the <see cref="ParameterValueSet"/> for a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameterOverride">
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
        /// <returns>
        /// The new <see cref="ParameterOverrideValueSet"/>s
        /// </returns>
        private IEnumerable<ParameterOverrideValueSet> ComputeValueSets(ParameterOverride parameterOverride, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var parameters = this.ParameterService.GetShallow(transaction, partition, new[] {parameterOverride.Parameter}, securityContext).OfType<Parameter>().ToArray();
            if (parameters.Length != 1)
            {
                throw new InvalidOperationException("None or more than one parameters were returned");
            }

            var parameter = parameters.Single();

            var parameterValueSets = this.ParameterValueSetService.GetShallow(transaction, partition, parameter.ValueSet, securityContext).OfType<ParameterValueSet>().ToArray();
            if (parameterValueSets.Length != parameter.ValueSet.Count)
            {
                throw new InvalidOperationException("All the value sets could not be retrieved.");
            }

            foreach (var set in parameterValueSets)
            {
                yield return this.ParameterOverrideValueSetFactory.CreateParameterOverrideValueSet(set, parameterOverride.Owner);
            }
        }

        /// <summary>
        /// Write new <see cref="ParameterOverrideValueSet"/> into the database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="parameterOverride">The current <see cref="ParameterOverride"/></param>
        /// <param name="newValueSet">The <see cref="ParameterOverrideValueSet"/> to write</param>
        private void WriteValueSet(NpgsqlTransaction transaction, string partition, ParameterOverride parameterOverride, IEnumerable<ParameterOverrideValueSet> newValueSet)
        {
            foreach (var parameterValueSet in newValueSet)
            {
                this.ParameterOverrideValueSetService.CreateConcept(transaction, partition, parameterValueSet, parameterOverride);
            }
        }
    }
}