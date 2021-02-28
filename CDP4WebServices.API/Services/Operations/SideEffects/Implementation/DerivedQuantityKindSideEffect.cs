// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DerivedQuantityKindSideEffect.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DerivedQuantityKindSideEffect"/> class is to execute additional logic before and
    /// after a specific operation is performed.
    /// </summary>
    public sealed class DerivedQuantityKindSideEffect : OperationSideEffect<DerivedQuantityKind>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQuantityKindFactorService"/>
        /// </summary>
        public IQuantityKindFactorService QuantityKindFactorService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQuantityKindService"/>
        /// </summary>
        public IQuantityKindService QuantityKindService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DerivedQuantityKind"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <paramref name="thing"/> that is inspected.
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
            DerivedQuantityKind thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (!rawUpdateInfo.ContainsKey("QuantityKindFactor"))
            {
                return;
            }

            var quantityKindFactorIids = (List<OrderedItem>)rawUpdateInfo["QuantityKindFactor"];
            var referenceDataLibrary = (ReferenceDataLibrary)container;

            // Check that all referenced QuantityKinds are from the same RDL chain
            var availableQuantityKindIids = this.GetQuantityKindIidsFromRdlChain(
                transaction,
                partition,
                securityContext,
                referenceDataLibrary.RequiredRdl);

            availableQuantityKindIids.AddRange(this.QuantityKindService
                .Get(transaction, partition, referenceDataLibrary.ParameterType, securityContext)
                .Select(x => x.Iid));

            var quantityKindFactors = this.QuantityKindFactorService
                .Get(transaction, partition, quantityKindFactorIids.Select(x => Guid.Parse(x.V.ToString())), securityContext)
                .Cast<QuantityKindFactor>();

            var quantityKinds = this.QuantityKindService
                .Get(transaction, partition, quantityKindFactors.Select(x => x.QuantityKind), securityContext)
                .Cast<QuantityKind>()
                .ToList();

            if (quantityKinds.Any(x => !availableQuantityKindIids.Contains(x.Iid)))
            {
                throw new AcyclicValidationException($"DerivedQuantityKind {thing.Name} {thing.Iid} cannot have " +
                                                     $"a QuantityKind factor from outside the current RDL chain.");
            }

            this.CheckCycleDeep(transaction, partition, securityContext, thing, quantityKinds);
        }

        /// <summary>
        /// Gets <see cref="QuantityKind"/> iids from an <see cref="SiteReferenceDataLibrary"/> chain.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security context used for permission checking.
        /// </param>
        /// <param name="srdlIid">
        /// The iid of the <see cref="SiteReferenceDataLibrary"/> to start from.
        /// </param>
        /// <returns>
        /// The list of <see cref="QuantityKind"/> iids.
        /// </returns>
        private List<Guid> GetQuantityKindIidsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? srdlIid)
        {
            var parameterTypeIids = new List<Guid>();

            if (srdlIid == null)
            {
                return parameterTypeIids;
            }

            var availableRdls = this.SiteReferenceDataLibraryService
                .Get(transaction, partition, null, securityContext)
                .Cast<SiteReferenceDataLibrary>().ToList();

            var next = srdlIid;

            do
            {
                var rdl = availableRdls.First(x => x.Iid == next);
                parameterTypeIids.AddRange(rdl.ParameterType);
                next = rdl.RequiredRdl;
            } while (next != null);

            return this.QuantityKindService
                .Get(transaction, partition, parameterTypeIids, securityContext)
                .Cast<QuantityKind>()
                .Select(x => x.Iid)
                .ToList();
        }

        /// <summary>
        /// Checks if the given <paramref name="thing"/> is cyclic.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security context used for permission checking.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DerivedQuantityKind"/> instance that must not be cyclic.
        /// </param>
        /// <param name="quantityKinds">
        /// The <see cref="QuantityKind"/>s that will be inspected.
        /// </param>
        private void CheckCycleDeep(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            DerivedQuantityKind thing,
            IEnumerable<QuantityKind> quantityKinds)
        {
            foreach (var quantityKind in quantityKinds)
            {
                if (quantityKind.Iid == thing.Iid)
                {
                    throw new AcyclicValidationException($"DerivedQuantityKind {thing.Name} {thing.Iid} cannot have " +
                                                         $"a QuantityKind factor that leads to cyclic dependency.");
                }

                IEnumerable<QuantityKind> nextQuantityKinds = null;

                switch (quantityKind)
                {
                    case SpecializedQuantityKind specializedQuantityKind:
                        nextQuantityKinds = this.QuantityKindService
                            .Get(transaction, partition, new List<Guid> { specializedQuantityKind.General }, securityContext)
                            .Cast<QuantityKind>();

                        break;

                    case DerivedQuantityKind derivedQuantityKind:
                        var quantityKindFactors = this.QuantityKindFactorService
                            .Get(transaction, partition, derivedQuantityKind.QuantityKindFactor.ToIdList(), securityContext)
                            .Cast<QuantityKindFactor>();

                        nextQuantityKinds = this.QuantityKindService
                            .Get(transaction, partition, quantityKindFactors.Select(x => x.QuantityKind), securityContext)
                            .Cast<QuantityKind>();

                        break;
                }

                if (nextQuantityKinds == null)
                {
                    continue;
                }

                this.CheckCycleDeep(transaction, partition, securityContext, thing, nextQuantityKinds);
            }
        }
    }
}
