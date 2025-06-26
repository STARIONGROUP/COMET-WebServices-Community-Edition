// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompoundParameterTypeBaseSideEffect.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Exceptions;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// Abstract super class from which all CompoundParameterType and sub classes derive.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type T that must be of type <see cref="Thing"/>
    /// </typeparam>
    public abstract class CompoundParameterTypeBaseSideEffect<T> : OperationSideEffect<T>
        where T : CompoundParameterType
    {
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
        /// The <see cref="T"/> instance that will be inspected.
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
        public override Task BeforeUpdate(
            T thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.TryGetValue("Component", out var value))
            {
                var componentsId = (IEnumerable<OrderedItem>)value;

                // Get RDL chain and collect types' ids
                var parameterTypeIdsFromChain = this.GetParameterTypeIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    ((ReferenceDataLibrary)container).RequiredRdl);

                parameterTypeIdsFromChain.AddRange(((ReferenceDataLibrary)container).ParameterType);

                // Get all CompoundParameterTypes
                var parameterTypes = this.CompoundParameterTypeService
                    .GetAsync(transaction, partition, parameterTypeIdsFromChain, securityContext)
                    .Cast<CompoundParameterType>().ToList();

                // Add all ArrayParameterTypes
                parameterTypes.AddRange(
                    this.ArrayParameterTypeService
                        .GetAsync(transaction, partition, parameterTypeIdsFromChain, securityContext)
                        .Cast<ArrayParameterType>().ToList());

                // Check every component
                foreach (var orderedItem in componentsId)
                {
                    if (!this.IsParameterTypeComponentAcyclic(
                            transaction,
                            partition,
                            securityContext,
                            parameterTypes,
                            Guid.Parse(orderedItem.V.ToString()),
                            thing.Iid))
                    {
                        throw new AcyclicValidationException(
                            $"{thing.ClassKind} {thing.Name} {thing.Iid} cannot have a ParameterTypeComponent {Guid.Parse(orderedItem.V.ToString())} that leads to cyclic dependency");
                    }
                }
            }

            return Task.CompletedTask;
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
            var availableRdls = this.SiteReferenceDataLibraryService.GetAsync(transaction, partition, null, securityContext)
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
        /// <param name="parameterTypeComponentId">
        /// The parameter type component id to check for being acyclic.
        /// </param>
        /// <param name="compoundParameterTypeId">
        /// The compound or array parameter type id to set parameter type component to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied parameter type component will not lead to cyclic dependency.
        /// </returns>
        private bool IsParameterTypeComponentAcyclic(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<CompoundParameterType> parameterTypes,
            Guid parameterTypeComponentId,
            Guid compoundParameterTypeId)
        {
            var cyclicParameterTypeList = new List<Guid>();

            this.SetCyclicParameterTypeIdToList(
                transaction,
                partition,
                securityContext,
                parameterTypes,
                parameterTypeComponentId,
                compoundParameterTypeId,
                cyclicParameterTypeList);

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
        /// The list to set a cyclic derived unit id if found
        /// </param>
        private void SetCyclicParameterTypeIdToList(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<CompoundParameterType> parameterTypes,
            Guid parameterTypeComponentId,
            Guid compoundParameterTypeId,
            List<Guid> cyclicParameterTypeList)
        {
            if (cyclicParameterTypeList.Count > 0)
            {
                return;
            }

            var parameterTypeComponent = this.ParameterTypeComponentService.GetAsync(
                transaction,
                partition,
                new List<Guid> { parameterTypeComponentId },
                securityContext).Cast<ParameterTypeComponent>().ToList()[0];

            if (parameterTypeComponent.ParameterType == compoundParameterTypeId)
            {
                cyclicParameterTypeList.Add(compoundParameterTypeId);
                return;
            }

            var parameterType = parameterTypes.Find(x => x.Iid == parameterTypeComponent.ParameterType);

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
                        cyclicParameterTypeList);
                }
            }
        }
    }
}