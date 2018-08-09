// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActualFiniteStateListSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ActualFiniteStateListSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ActualFiniteStateListSideEffect : OperationSideEffect<ActualFiniteStateList>
    {
        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateService"/>
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStateDependentParameterUpdateService"/>
        /// </summary>
        public IStateDependentParameterUpdateService StateDependentParameterUpdateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFiniteStateLogicService"/>
        /// </summary>
        public IFiniteStateLogicService FiniteStateLogicService { get; set; }

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
        public override void AfterCreate(ActualFiniteStateList thing, Thing container, ActualFiniteStateList originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.FiniteStateLogicService.UpdateActualFinisteStateList(thing, (Iteration)container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Execute additional logic after a successful update operation.
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
        public override void AfterUpdate(ActualFiniteStateList thing, Thing container, ActualFiniteStateList originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (!thing.PossibleFiniteStateList.All(x => originalThing.PossibleFiniteStateList.Any(y => y.K == x.K && y.V == x.V))
                || thing.PossibleFiniteStateList.Count != originalThing.PossibleFiniteStateList.Count)
            {
                // Update all actualFiniteStates
                this.FiniteStateLogicService.UpdateActualFinisteStateList(thing, (Iteration)container, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
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
        public override void BeforeDelete(ActualFiniteStateList thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // Get all associated state dependent parameters and re-create value set without the state dependency
            this.StateDependentParameterUpdateService.UpdateAllStateDependentParameters(thing, (Iteration)container, transaction, partition, securityContext, null);
        }
    }
}