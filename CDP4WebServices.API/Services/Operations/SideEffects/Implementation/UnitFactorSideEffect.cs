// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitFactorSideEffect.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="UnitFactorSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class UnitFactorSideEffect : OperationSideEffect<UnitFactor>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModelReferenceDataLibraryService"/>
        /// </summary>
        public IModelReferenceDataLibraryService ModelReferenceDataLibraryService { get; set; }

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
            UnitFactor thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("Unit"))
            {
                var unitId = (Guid)rawUpdateInfo["Unit"];

                // Get RDL chain and collect units' ids
                var unitIdsFromChain = this.GetUnitIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    container.Iid);

                // Get all Derived units
                var units = this.DerivedUnitService.Get(transaction, partition, unitIdsFromChain, securityContext)
                    .Cast<DerivedUnit>().ToList();

                if (!this.IsUnitFactorAcyclic(transaction, partition, securityContext, units, container.Iid, unitId))
                {
                    throw new AcyclicValidationException(
                        string.Format(
                            "UnitFactor {0} cannot have a UnitFactor {1} that leads to cyclic dependency",
                            thing.Iid,
                            unitId));
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
        /// <param name="derivedUnitId">
        /// The Iid of a derived unit to start from.
        /// </param>
        /// <returns>
        /// The list of unit ids.
        /// </returns>
        private List<Guid> GetUnitIdsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid derivedUnitId)
        {
            List<ReferenceDataLibrary> availableRdls = this.ModelReferenceDataLibraryService
                .Get(transaction, partition, null, securityContext).Cast<ReferenceDataLibrary>().ToList();
            availableRdls.AddRange(
                this.SiteReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                    .Cast<ReferenceDataLibrary>().ToList());

            Guid? requiredRdl = availableRdls.Find(x => x.Unit.Contains(derivedUnitId)).Iid;
            var unitIds = new List<Guid>();

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
        /// <param name="unitId">
        /// The unit id to check for being acyclic.
        /// </param>
        /// <param name="firstUnitId">
        /// Unit id to start from. Is used for the first iteration.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied unit factor will not lead to cyclic dependency.
        /// </returns>
        private bool IsUnitFactorAcyclic(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<DerivedUnit> units,
            Guid unitId,
            Guid firstUnitId)
        {
            var cyclicDerivedUnitList = new List<Guid>();
            this.SetSetCyclicDerivedUnitIdToList(
                transaction,
                partition,
                securityContext,
                units,
                Guid.Empty,
                unitId,
                cyclicDerivedUnitList,
                firstUnitId);

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
        /// <param name="firstUnitId">
        /// Unit id to start from. Is used for the first iteration.
        /// </param>
        private void SetSetCyclicDerivedUnitIdToList(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<DerivedUnit> units,
            Guid unitFactorId,
            Guid unitId,
            List<Guid> cyclicDerivedUnitList,
            Guid firstUnitId)
        {
            if (cyclicDerivedUnitList.Count > 0)
            {
                return;
            }

            var measurementUnitId = firstUnitId;
            if (measurementUnitId == Guid.Empty)
            {
                measurementUnitId = this.UnitFactorService.Get(
                    transaction,
                    partition,
                    new List<Guid> { unitFactorId },
                    securityContext).Cast<UnitFactor>().ToList()[0].Unit;
            }

            if (measurementUnitId == unitId)
            {
                cyclicDerivedUnitList.Add(unitId);
                return;
            }

            var unit = units.Find(x => x.Iid == measurementUnitId);
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
                        cyclicDerivedUnitList,
                        Guid.Empty);
                }
            }
        }
    }
}