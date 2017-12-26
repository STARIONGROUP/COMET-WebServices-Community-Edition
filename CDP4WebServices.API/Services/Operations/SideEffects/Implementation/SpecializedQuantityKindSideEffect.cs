// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecializedQuantityKindSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="SpecializedQuantityKindSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class SpecializedQuantityKindSideEffect : OperationSideEffect<SpecializedQuantityKind>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISpecializedQuantityKindService"/>
        /// </summary>
        public ISpecializedQuantityKindService SpecializedQuantityKindService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="SpecializedQuantityKind"/> instance that will be inspected.
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
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(
            SpecializedQuantityKind thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("General"))
            {
                var kindId = (Guid)rawUpdateInfo["General"];

                // Check for itself
                if (kindId == thing.Iid)
                {
                    throw new AcyclicValidationException(
                        string.Format(
                            "SpecializedQuantityKind {0} {1} cannot have itself as a general quantity kind.",
                            thing.Name,
                            thing.Iid));
                }

                // Get RDL chain and collect types' ids
                var parameterTypeIdsFromChain = this.GetParameterTypeIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    ((ReferenceDataLibrary)container).RequiredRdl);
                parameterTypeIdsFromChain.AddRange(((ReferenceDataLibrary)container).ParameterType);

                // Check that qantity kind is from the same RDL chain
                if (!parameterTypeIdsFromChain.Contains(kindId))
                {
                    throw new AcyclicValidationException(
                        string.Format(
                            "SpecializedQuantityKind {0} {1} cannot have a general quantity kind from outside the RDL chain.",
                            thing.Name,
                            thing.Iid));
                }

                // Get all SpecializedQuantityKinds
                var parameterTypes = this.SpecializedQuantityKindService
                    .Get(transaction, partition, parameterTypeIdsFromChain, securityContext)
                    .Cast<SpecializedQuantityKind>().ToList();

                // Check whether containing folder is acyclic
                if (!this.IsSpecializedQuantityKindAcyclic(parameterTypes, kindId, thing.Iid))
                {
                    throw new AcyclicValidationException(
                        string.Format(
                            "Folder {0} {1} cannot have a containing Folder {2} that leads to cyclic dependency",
                            thing.Name,
                            thing.Iid,
                            kindId));
                }
            }
        }

        /// <summary>
        /// Get parameter types ids from an rdl chain.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rdlId">
        /// The Iid of RDL to start from.
        /// </param>
        /// <returns>
        /// The list of parameter types ids.
        /// </returns>
        private List<Guid> GetParameterTypeIdsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? rdlId)
        {
            var availableRdls = this.SiteReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                .Cast<SiteReferenceDataLibrary>().ToList();
            var parameterTypeIds = new List<Guid>();
            var requiredRdl = rdlId;

            while (requiredRdl != null)
            {
                var rdl = availableRdls.Find(x => x.Iid == requiredRdl);
                parameterTypeIds.AddRange(rdl.ParameterType);
                requiredRdl = rdl.RequiredRdl;
            }

            return parameterTypeIds;
        }

        /// <summary>
        /// Is specialized quantity kind acyclic.
        /// </summary>
        /// <param name="parameterTypes">
        /// The parameterTypes from the rdl chain.
        /// </param>
        /// <param name="generalKindId">
        /// The quantity kind id to check for being acyclic.
        /// </param>
        /// <param name="specializedQuantityKindId">
        /// The specialized quantity kind id to set a quantity kind to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether a specialized quantity kind will not lead to cyclic dependency.
        /// </returns>
        private bool IsSpecializedQuantityKindAcyclic(
            List<SpecializedQuantityKind> parameterTypes,
            Guid generalKindId,
            Guid specializedQuantityKindId)
        {
            Guid? nextGeneralKindId = generalKindId;

            while (nextGeneralKindId != null)
            {
                if (nextGeneralKindId == specializedQuantityKindId)
                {
                    return false;
                }

                nextGeneralKindId = parameterTypes.FirstOrDefault(x => x.Iid == nextGeneralKindId)?.General;
            }

            return true;
        }
    }
}