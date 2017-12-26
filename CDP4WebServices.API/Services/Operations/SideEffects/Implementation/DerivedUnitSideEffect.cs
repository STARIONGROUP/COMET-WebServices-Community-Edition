// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DerivedUnitSideEffect.cs" company="RHEA System S.A.">
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
    using CDP4Common.Types;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DerivedUnitSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class DerivedUnitSideEffect : OperationSideEffect<DerivedUnit>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDerivedUnitService"/>
        /// </summary>
        public IDerivedUnitService DerivedUnitService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUnitFactorService"/>
        /// </summary>
        public IUnitFactorService UnitFactorService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DerivedUnit"/> instance that will be inspected.
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
            DerivedUnit thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("UnitFactor"))
            {
                var unitFactorsId = (List<OrderedItem>)rawUpdateInfo["UnitFactor"];

                // Get RDL chain and collect units' ids
                var unitIdsFromChain = this.GetUnitIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    ((ReferenceDataLibrary)container).RequiredRdl);
                unitIdsFromChain.AddRange(((ReferenceDataLibrary)container).Unit);

                // Get all Derived units
                var units = this.DerivedUnitService.Get(transaction, partition, unitIdsFromChain, securityContext)
                    .Cast<DerivedUnit>().ToList();

                // Check every unit factor
                foreach (var orderedItem in unitFactorsId)
                {
                    if (!this.IsUnitFactorAcyclic(
                            transaction,
                            partition,
                            securityContext,
                            units,
                            Guid.Parse(orderedItem.V.ToString()),
                            thing.Iid))
                    {
                        throw new ArgumentException(
                            string.Format(
                                "DerivedUnit {0} {1} cannot have a UnitFactor {2} that leads to cyclic dependency",
                                thing.Name,
                                thing.Iid,
                                Guid.Parse(orderedItem.V.ToString())));
                    }
                }
            }
        }

        /// <summary>
        /// Get unit ids from an rdl chain.
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
        /// The list of unit ids.
        /// </returns>
        private List<Guid> GetUnitIdsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? rdlId)
        {
            var availableRdls = this.SiteReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                .Cast<SiteReferenceDataLibrary>().ToList();
            var unitIds = new List<Guid>();
            var requiredRdl = rdlId;

            while (requiredRdl != null)
            {
                var rdl = availableRdls.Find(x => x.Iid == requiredRdl);
                unitIds.AddRange(rdl.Unit);
                requiredRdl = rdl.RequiredRdl;
            }

            return unitIds;
        }

        /// <summary>
        /// Is unit factor acyclic.
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
        /// <param name="units">
        /// The units from RDL chain.
        /// </param>
        /// <param name="unitFactorId">
        /// The unit factor id to check for being acyclic.
        /// </param>
        /// <param name="unitId">
        /// The unit id to set unit factor to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied unit factor will not lead to cyclic dependency.
        /// </returns>
        private bool IsUnitFactorAcyclic(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<DerivedUnit> units,
            Guid unitFactorId,
            Guid unitId)
        {
            var cyclicDerivedUnitList = new List<Guid>();
            this.SetSetCyclicDerivedUnitIdToList(
                transaction,
                partition,
                securityContext,
                units,
                unitFactorId,
                unitId,
                cyclicDerivedUnitList);

            return cyclicDerivedUnitList.Count == 0;
        }

        /// <summary>
        /// Set cyclic derived unit id to the supplied list.
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
        /// <param name="units">
        /// The units from RDL chain.
        /// </param>
        /// <param name="unitFactorId">
        /// The unit factor id to check for being acyclic.
        /// </param>
        /// <param name="unitId">
        /// The unit id to set unit factor to.
        /// </param>
        /// <param name="cyclicDerivedUnitList">
        /// The list to set a cyclic derived unit id if found
        /// </param>
        private void SetSetCyclicDerivedUnitIdToList(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<DerivedUnit> units,
            Guid unitFactorId,
            Guid unitId,
            List<Guid> cyclicDerivedUnitList)
        {
            if (cyclicDerivedUnitList.Count > 0)
            {
                return;
            }

            var unitFactor = this.UnitFactorService
                .Get(transaction, partition, new List<Guid> { unitFactorId }, securityContext).Cast<UnitFactor>()
                .ToList()[0];

            if (unitFactor.Unit == unitId)
            {
                cyclicDerivedUnitList.Add(unitId);
                return;
            }

            var unit = units.Find(x => x.Iid == unitFactor.Unit);
            if (unit != null)
            {
                foreach (var orderedItem in unit.UnitFactor)
                {
                    this.SetSetCyclicDerivedUnitIdToList(
                        transaction,
                        partition,
                        securityContext,
                        units,
                        (Guid)orderedItem.V,
                        unitId,
                        cyclicDerivedUnitList);
                }
            }
        }
    }
}