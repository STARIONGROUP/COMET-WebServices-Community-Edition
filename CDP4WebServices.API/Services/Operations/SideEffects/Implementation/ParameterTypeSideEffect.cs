// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeComponentSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects.Implementation
{
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services.Authorization;
    using Npgsql;

    public sealed class ParameterTypeSideEffect : OperationSideEffect<ParameterType>
    {
        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful create operation.
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
        public override void AfterCreate(ParameterType thing, Thing container, ParameterType originalThing,
            NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // reset the IDefaultValueArrayFactory, this may be used during the same transaction, any update to the ParameterTypeComponents needs to reset the IDefaultValueArrayFactory cache
            this.DefaultValueArrayFactory.Reset();
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful delete operation.
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
        public override void AfterDelete(ParameterType thing, Thing container, ParameterType originalThing,
            NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // reset the IDefaultValueArrayFactory, this may be used during the same transaction, any update to the ParameterTypeComponents needs to reset the IDefaultValueArrayFactory cache
            this.DefaultValueArrayFactory.Reset();
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
        public override void AfterUpdate(ParameterType thing, Thing container, ParameterType originalThing, NpgsqlTransaction transaction,
            string partition, ISecurityContext securityContext)
        {
            // reset the IDefaultValueArrayFactory, this may be used during the same transaction, any update to the ParameterTypeComponents needs to reset the IDefaultValueArrayFactory cache
            this.DefaultValueArrayFactory.Reset();
        }
    }
}