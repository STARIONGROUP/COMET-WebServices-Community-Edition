// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateListSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="PossibleFiniteStateListSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PossibleFiniteStateListSideEffect : OperationSideEffect<PossibleFiniteStateList>
    {
        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStateDependentParameterUpdateService"/>
        /// </summary>
        public IStateDependentParameterUpdateService StateDependentParameterUpdateService { get; set; }

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
        public override void BeforeDelete(PossibleFiniteStateList thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var actualFiniteStateListCollectionToUpdate =
                this.ActualFiniteStateListService.GetShallow(transaction, partition, null, securityContext)
                    .Where(i => i is ActualFiniteStateList).Cast<ActualFiniteStateList>()
                    .Where(x => x.PossibleFiniteStateList.Select(oi => Guid.Parse(oi.V.ToString())).Contains(thing.Iid))
                    .ToList();

            foreach (var actualFiniteStateList in actualFiniteStateListCollectionToUpdate)
            {
                // delete all actual lists that only have the deleted PossibleList as PossibleList and update all parameters that depend on them
                // do it before as otherwise the ActualStateList would not contain the PossibleFiniteStateList anymore
                if (actualFiniteStateList.PossibleFiniteStateList.Count == 1)
                {
                    this.StateDependentParameterUpdateService.UpdateAllStateDependentParameters(actualFiniteStateList, (Iteration)container, transaction, partition, securityContext, null);
                    if (!this.ActualFiniteStateListService.DeleteConcept(transaction, partition, actualFiniteStateList, container))
                    {
                        throw new InvalidOperationException($"The actual finite state list {actualFiniteStateList.Iid} could not be deleted");
                    }
                }
            }
        }
    }
}
