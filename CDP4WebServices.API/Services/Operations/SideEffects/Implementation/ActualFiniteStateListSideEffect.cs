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
            this.AddOrUpdateActualFiniteStates(thing, container as Iteration, transaction, partition, securityContext);
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
                this.AddOrUpdateActualFiniteStates(thing, container as Iteration, transaction, partition, securityContext);
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

        /// <summary>
        /// Add <see cref="ActualFiniteState"/> to the <see cref="ActualFiniteStateList"/> when it is created or update a <see cref="ActualFiniteStateList"/> after a <see cref="PossibleFiniteStateList"/> add operation
        /// </summary>
        /// <param name="actualFiniteStateList">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        private void AddOrUpdateActualFiniteStates(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // delete the old ActualFiniteState
            var oldActualStates = this.ActualFiniteStateService.GetShallow(transaction, partition, actualFiniteStateList.ActualState, securityContext)
                .Where(i => i is ActualFiniteState).OfType<ActualFiniteState>().ToList();

            // Gets the possible finite state list of the current processed ActualFiniteStateList
            var pslCollection =
                this.PossibleFiniteStateListService
                    .GetShallow(transaction, partition, actualFiniteStateList.PossibleFiniteStateList.Select(item => Guid.Parse(item.V.ToString())), securityContext)
                    .OfType<PossibleFiniteStateList>()
                    .ToList();

            if (pslCollection.Count != actualFiniteStateList.PossibleFiniteStateList.Count)
            {
                throw new InvalidOperationException("All the Possible finite state lists could not be retrieved.");
            }

            // Build the ordered collection of PossibleFiniteStateList for the current ActualFiniteStateList
            var orderedPslCollection = new List<PossibleFiniteStateList>(pslCollection.Count);
            foreach (var item in actualFiniteStateList.PossibleFiniteStateList.OrderBy(x => x.K))
            {
                var psl = pslCollection.Single(x => x.Iid.ToString() == item.V.ToString());
                orderedPslCollection.Add(psl);
            }

            var newOldActualStateMap = new Dictionary<ActualFiniteState, ActualFiniteState>();
            this.CreateActualStates(orderedPslCollection, 0, null, actualFiniteStateList, transaction, partition, oldActualStates, ref newOldActualStateMap);

            this.StateDependentParameterUpdateService.UpdateAllStateDependentParameters(actualFiniteStateList, iteration, transaction, partition, securityContext, newOldActualStateMap);

            // delete old actual states which will clean up all value sets that depend on it
            foreach (var actualState in oldActualStates)
            {
                if (!this.ActualFiniteStateService.DeleteConcept(transaction, partition, actualState, actualFiniteStateList))
                {
                    throw new InvalidOperationException($"The actual finite state {actualState.Iid} could not be deleted");
                }
            }
        }

        /// <summary>
        /// Create the <see cref="ActualFiniteState"/>s for a <see cref="ActualFiniteStateList"/>
        /// </summary>
        /// <param name="pslCollection">The ordered collection of <see cref="PossibleFiniteStateList"/></param>
        /// <param name="pslIndex">The index of the <see cref="PossibleFiniteStateList"/> in the collection to process</param>
        /// <param name="possibleStateIds">The current collection of <see cref="Guid"/> to use for the <see cref="ActualFiniteState"/> to create</param>
        /// <param name="container">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition in the database</param>
        /// <param name="oldActualStates">The old <see cref="ActualFiniteState"/></param>
        /// <param name="newOldStateMap">The resulting map that links the new to old <see cref="ActualFiniteState"/></param>
        private void CreateActualStates(IReadOnlyList<PossibleFiniteStateList> pslCollection, int pslIndex, IEnumerable<Guid> possibleStateIds, ActualFiniteStateList container, NpgsqlTransaction transaction, string partition, IReadOnlyList<ActualFiniteState> oldActualStates, ref Dictionary<ActualFiniteState, ActualFiniteState> newOldStateMap)
        {
            var currentPossibleStateIds = possibleStateIds == null ? new List<Guid>() : possibleStateIds.ToList();

            foreach (var orderedItem in pslCollection[pslIndex].PossibleState.OrderBy(x => x.K))
            {
                var newPossibleStateIds = currentPossibleStateIds.ToList();
                newPossibleStateIds.Add(Guid.Parse(orderedItem.V.ToString()));

                // Last PossibleFiniteStateList in the ordered collection
                if (pslIndex == pslCollection.Count - 1)
                {
                    // create actual state
                    var newActualstate = new ActualFiniteState(Guid.NewGuid(), 1);
                    newActualstate.PossibleState.AddRange(newPossibleStateIds);

                    this.ActualFiniteStateService.CreateConcept(transaction, partition, newActualstate, container);

                    var oldActualState =
                        oldActualStates.SingleOrDefault(x => x.PossibleState.SequenceEqual(newPossibleStateIds));

                    newOldStateMap.Add(newActualstate, oldActualState);
                    continue;
                }

                this.CreateActualStates(pslCollection, pslIndex + 1, newPossibleStateIds, container, transaction, partition, oldActualStates, ref newOldStateMap);
            }
        }
    }
}