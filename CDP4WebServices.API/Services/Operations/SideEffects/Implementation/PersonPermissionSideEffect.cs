// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonPermissionSideEffect.cs" company="RHEA System S.A.">
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
    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="PersonPermissionSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PersonPermissionSideEffect : OperationSideEffect<PersonPermission>
    {
        #region Injected services

        /// <summary>
        /// Gets or sets the <see cref="IAccessRightKindValidationService"/>
        /// </summary>
        public IAccessRightKindValidationService AccessRightKindValidationService { get; set; }

        #endregion

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation
        {
            get
            {
                return new[] { "accessRight" };
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
        public override bool BeforeCreate(
            PersonPermission thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            this.ValidateAccessRightKind(thing);
            return true;
        }

        /// <summary>
        /// execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="PersonPermission"/> instance that will be inspected.
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
        public override void AfterUpdate(PersonPermission thing, Thing container, PersonPermission originalThing, NpgsqlTransaction transaction,
            string partition, ISecurityContext securityContext)
        {
            this.ValidateAccessRightKind(thing);
        }

        /// <summary>
        /// Checks whether a set access right is valid for the supplied <see cref="PersonPermission"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        private void ValidateAccessRightKind(PersonPermission thing)
        {
            if (!this.AccessRightKindValidationService.IsPersonPermissionValid(thing))
            {
                throw new InvalidOperationException(
                    "The accessRight " + thing.AccessRight + " cannot be set for the class " + thing.ObjectClass + " .");
            }
        }
    }
}