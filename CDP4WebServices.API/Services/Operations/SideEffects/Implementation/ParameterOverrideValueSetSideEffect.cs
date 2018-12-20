// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideValueSetSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using Authorization;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParameterOverrideValueSetSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterOverrideValueSetSideEffect : OperationSideEffect<ParameterOverrideValueSet>
    {
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
            ParameterOverrideValueSet thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            return false;
        }

        /// <summary>
        /// Execute additional logic before a delete operation
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="container">The container of the <see cref="Thing"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void BeforeDelete(ParameterOverrideValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            throw new InvalidOperationException("ParameterOverrideValueSet Cannot be deleted");
        }
    }
}