// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationalExpressionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="RelationalExpressionSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class RelationalExpressionSideEffect : OperationSideEffect<RelationalExpression>
    {
        public IParametricConstraintService ParametricConstraintService { get; set; }

        public IRelationalExpressionService RelationalExpressionService { get; set; }

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
        public override void BeforeDelete(RelationalExpression thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var parametricConstraintThatContainsRelationalExpression =
                this.ParametricConstraintService.GetShallow(transaction, partition, new[] { container.Iid }, securityContext)
                    .SingleOrDefault(x => x.Iid == container.Iid);

            if (parametricConstraintThatContainsRelationalExpression is ParametricConstraint parametricConstraint)
            {
                var relationalExpressions =
                    this.RelationalExpressionService.GetShallow(transaction, partition, parametricConstraint.Expression, securityContext).ToList();

                if (relationalExpressions.Any(x => x.Iid == thing.Iid) && relationalExpressions.Count == 1)
                {
                    throw new Cdp4ModelValidationException($"A {nameof(ParametricConstraint)} must contain at least 1 {nameof(RelationalExpression)}");
                }
            }
        }
    }
}
