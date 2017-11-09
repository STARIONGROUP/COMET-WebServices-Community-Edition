// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParameterGroupSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterGroupSideEffect : OperationSideEffect<ParameterGroup>
    {
        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

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
        public override void BeforeDelete(ParameterGroup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // Get all Parameters that reference the given ParameterGroup and set references to null
            var parameters = this.ParameterService.Get(transaction, partition, null, securityContext).OfType<Parameter>().ToList().Where(x => x.Group == thing.Iid);
            foreach (var parameter in parameters)
            {
                parameter.Group = null;
                this.ParameterService.UpdateConcept(transaction, partition, parameter, container);
            }
        }
    }
}