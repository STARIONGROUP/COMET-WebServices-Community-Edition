// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeComponentSideEffect.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="ParameterTypeComponentSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterTypeComponentSideEffect : OperationSideEffect<ParameterTypeComponent>
    {
        /// <summary>
        /// Gets or sets the <see cref="IModelReferenceDataLibraryService"/>
        /// </summary>
        public IModelReferenceDataLibraryService ModelReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICompoundParameterTypeService"/>
        /// </summary>
        public ICompoundParameterTypeService CompoundParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IArrayParameterTypeService"/>
        /// </summary>
        public IArrayParameterTypeService ArrayParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterTypeComponentService"/>
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ParameterTypeComponent"/> instance that will be inspected.
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
            ParameterTypeComponent thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ParameterType"))
            {
                var parameterTypeId = (Guid)rawUpdateInfo["ParameterType"];

                // Get RDL chain and collect types' ids
                var parameterTypeIdsFromChain = this.GetParameterTypeIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    container.Iid);

                // Get all CompoundParameterTypes
                var parameterTypes = this.CompoundParameterTypeService
                    .Get(transaction, partition, parameterTypeIdsFromChain, securityContext)
                    .Cast<CompoundParameterType>().ToList();

                // Add all ArrayParameterTypes
                parameterTypes.AddRange(
                    this.ArrayParameterTypeService
                        .Get(transaction, partition, parameterTypeIdsFromChain, securityContext)
                        .Cast<ArrayParameterType>().ToList());

                if (!this.IsParameterTypeComponentAcyclic(
                        transaction,
                        partition,
                        securityContext,
                        parameterTypes,
                        container.Iid,
                        parameterTypeId))
                {
                    throw new AcyclicValidationException(
                        string.Format(
                            "{0} {1} cannot have a ParameterType {2} that leads to cyclic dependency",
                            thing.ClassKind.ToString(),
                            thing.Iid,
                            parameterTypeId));
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
        /// <param name="compoundParameterTypeId">
        /// The Iid of compound parameter type to start from.
        /// </param>
        /// <returns>
        /// The list of parameter types ids.
        /// </returns>
        private List<Guid> GetParameterTypeIdsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid compoundParameterTypeId)
        {
            List<ReferenceDataLibrary> availableRdls = this.ModelReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                .Cast<ReferenceDataLibrary>().ToList();
            availableRdls.AddRange(this.SiteReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                    .Cast<ReferenceDataLibrary>().ToList());

            Guid? requiredRdl = availableRdls.Find(x => x.ParameterType.Contains(compoundParameterTypeId)).Iid;
            var parameterTypeIds = new List<Guid>();

            while (requiredRdl != null)
            {
                var rdl = availableRdls.Find(x => x.Iid == requiredRdl);
                parameterTypeIds.AddRange(rdl.ParameterType);
                requiredRdl = rdl.RequiredRdl;
            }

            return parameterTypeIds;
        }

        /// <summary>
        /// Is parameter type component acyclic.
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
        /// <param name="parameterTypes">
        /// The parameterTypes from RDL chain.
        /// </param>
        /// <param name="compoundParameterTypeId">
        /// The compound or array parameter type id to set parameter type component to.
        /// </param>
        /// <param name="firstParameterTypeId">
        /// Parameter type id to start from. Is used for the first iteration.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied parameter type component will not lead to cyclic dependency.
        /// </returns>
        private bool IsParameterTypeComponentAcyclic(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<CompoundParameterType> parameterTypes,
            Guid compoundParameterTypeId,
            Guid firstParameterTypeId)
        {
            var cyclicParameterTypeList = new List<Guid>();
            this.SetCyclicParameterTypeIdToList(
                transaction,
                partition,
                securityContext,
                parameterTypes,
                Guid.Empty,
                compoundParameterTypeId,
                cyclicParameterTypeList,
                firstParameterTypeId);

            return cyclicParameterTypeList.Count == 0;
        }

        /// <summary>
        /// Set cyclic parameter type id to the supplied list.
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
        /// <param name="parameterTypes">
        /// The parameterTypes from RDL chain.
        /// </param>
        /// <param name="parameterTypeComponentId">
        /// The parameter type component id to check for being acyclic.
        /// </param>
        /// <param name="compoundParameterTypeId">
        /// The compound or array parameter type id to set parameter type component to.
        /// </param>
        /// <param name="cyclicParameterTypeList">
        /// The list to set a cyclic derived unit id if found.
        /// </param>
        /// <param name="firstParameterTypeId">
        /// Parameter type id to start from. Is used for the first iteration.
        /// </param>
        private void SetCyclicParameterTypeIdToList(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<CompoundParameterType> parameterTypes,
            Guid parameterTypeComponentId,
            Guid compoundParameterTypeId,
            List<Guid> cyclicParameterTypeList,
            Guid firstParameterTypeId)
        {
            if (cyclicParameterTypeList.Count > 0)
            {
                return;
            }

            var parameterTypeId = firstParameterTypeId;
            if (firstParameterTypeId == Guid.Empty)
            {
                parameterTypeId = this.ParameterTypeComponentService.Get(
                    transaction,
                    partition,
                    new List<Guid> { parameterTypeComponentId },
                    securityContext).Cast<ParameterTypeComponent>().ToList()[0].ParameterType;
            }

            if (parameterTypeId == compoundParameterTypeId)
            {
                cyclicParameterTypeList.Add(compoundParameterTypeId);
                return;
            }

            var parameterType = parameterTypes.Find(x => x.Iid == parameterTypeId);
            if (parameterType != null)
            {
                foreach (var orderedItem in parameterType.Component)
                {
                    this.SetCyclicParameterTypeIdToList(
                        transaction,
                        partition,
                        securityContext,
                        parameterTypes,
                        (Guid)orderedItem.V,
                        compoundParameterTypeId,
                        cyclicParameterTypeList,
                        Guid.Empty);
                }
            }
        }
    }
}