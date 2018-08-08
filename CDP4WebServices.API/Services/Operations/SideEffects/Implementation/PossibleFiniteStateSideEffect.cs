// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Orm.Dao;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;
    using ActualFiniteState = CDP4Common.DTO.ActualFiniteState;
    using ActualFiniteStateList = CDP4Common.DTO.ActualFiniteStateList;
    using Iteration = CDP4Common.DTO.Iteration;
    using PossibleFiniteState = CDP4Common.DTO.PossibleFiniteState;
    using PossibleFiniteStateList = CDP4Common.DTO.PossibleFiniteStateList;

    /// <summary>
    /// The purpose of the <see cref="PossibleFiniteStateSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PossibleFiniteStateSideEffect : OperationSideEffect<PossibleFiniteState>
    {
        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateService"/>
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStateDependentParameterUpdateService"/>
        /// </summary>
        public IStateDependentParameterUpdateService StateDependentParameterUpdateService { get; set; }

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
        public override void AfterCreate(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.UpdateAllRelevantActualFiniteStateList((PossibleFiniteStateList)container, transaction, partition, securityContext);
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
        public override void BeforeDelete(PossibleFiniteState thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // redirect out the iteration partition to the engineeringmodel one, as that contains the iteration information
            var engineeringModelPartition = partition.Replace(
                Utils.IterationSubPartition,
                Utils.EngineeringModelPartition);

            var iteration =
                this.IterationService.GetShallow(transaction, engineeringModelPartition, null, securityContext)
                    .Where(i => i is Iteration).Cast<Iteration>()
                    .SingleOrDefault();

            if (iteration == null)
            {
                throw new InvalidOperationException(string.Format("The iteration could not be found when deleting the Possible Finite State {0}", thing.Iid));
            }

            var possibleList = (PossibleFiniteStateList)container;
            if (possibleList.PossibleState.Count == 1 &&
                possibleList.PossibleState.Single().V.ToString() == thing.Iid.ToString())
            {
                throw new InvalidOperationException(string.Format("It is not allowed to delete the last Possible Finite State {0} from the list. Consider deleting the whole Possible Finite State List {1}", thing.Iid, possibleList.Iid));
            }
        }

        /// <summary>
        /// After a <see cref="PossibleFiniteState"/> is deleted, update related <see cref="ActualFiniteStateList"/> and related <see cref="ParameterBase"/>s
        /// </summary>
        /// <param name="thing">The deleted <see cref="PossibleFiniteState"/></param>
        /// <param name="container">The container</param>
        /// <param name="originalThing">The original <see cref="PossibleFiniteState"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void AfterDelete(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.AfterDelete(thing, container, originalThing, transaction, partition, securityContext);
            this.UpdateAllRelevantActualFiniteStateList((PossibleFiniteStateList)container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Get and update all relevant <see cref="CDP4Common.DTO.ActualFiniteStateList"/>
        /// </summary>
        /// <param name="container">
        /// The container instance of the <see cref="PossibleFiniteStateList"/> that is inspected.
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
        private void UpdateAllRelevantActualFiniteStateList(PossibleFiniteStateList container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // redirect out the iteration partition to the engineeringmodel one, as that contains the iteration information
            var engineeringModelPartition = partition.Replace(
                Utils.IterationSubPartition,
                Utils.EngineeringModelPartition);

            var iteration =
                this.IterationService.GetShallow(transaction, engineeringModelPartition, null, securityContext)
                    .Where(i => i is Iteration).Cast<Iteration>()
                    .SingleOrDefault();

            if (iteration == null)
            {
                throw new InvalidOperationException("The iteration could not be found.");
            }

            // get all actual finite state list
            var actualFiniteStateList = this.ActualFiniteStateListService.GetShallow(transaction, partition, null, securityContext).Cast<ActualFiniteStateList>();
            var actualFiniteStateListToUpdate = actualFiniteStateList.Where(x => x.PossibleFiniteStateList.Select(item => Guid.Parse(item.V.ToString())).Contains(container.Iid));

            foreach (var finiteStateList in actualFiniteStateListToUpdate)
            {
                this.UpdateActualFinisteStateList(finiteStateList, iteration, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Update a <see cref="ActualFiniteStateList"/> after a <see cref="PossibleFiniteState"/> create or remove operation
        /// </summary>
        /// <param name="actualFiniteStateList">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        private void UpdateActualFinisteStateList(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // delete the old ActualFiniteState
            var oldActualStates = this.ActualFiniteStateService.GetShallow(transaction, partition, actualFiniteStateList.ActualState, securityContext)
                .Where(i => i is ActualFiniteState).Cast<ActualFiniteState>().ToList();

            // Gets the possible finite state list of the current processed ActualFiniteStateList
            var pslCollection =
                this.PossibleFiniteStateListService
                    .GetShallow(transaction, partition, actualFiniteStateList.PossibleFiniteStateList.Select(item => Guid.Parse(item.V.ToString())), securityContext)
                    .Cast<PossibleFiniteStateList>()
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
                    throw new InvalidOperationException(string.Format("The actual finite state {0} could not be deleted", actualState.Iid));
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

            // build the different PossibleStates combination taken from the PossibleFiniteStateLists and create an ActualState for each of these combinations
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

                    newActualstate.Kind = oldActualState?.Kind ?? ActualFiniteStateKind.MANDATORY;

                    newOldStateMap.Add(newActualstate, oldActualState);
                    continue;
                }

                this.CreateActualStates(pslCollection, pslIndex + 1, newPossibleStateIds, container, transaction, partition, oldActualStates, ref newOldStateMap);
            }
        }
    }
}
