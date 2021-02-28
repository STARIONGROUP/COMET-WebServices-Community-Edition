// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationSideEffect.cs" company="RHEA System S.A.">
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

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    using ParameterSwitchKind = CDP4Common.EngineeringModelData.ParameterSwitchKind;

    /// <summary>
    /// The publication side effect.
    /// </summary>
    public sealed class PublicationSideEffect : OperationSideEffect<Publication>
    {
        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// Process the update of corresponding value-set
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
        public override bool BeforeCreate(Publication thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            thing.CreatedOn = this.TransactionManager.GetTransactionTime(transaction);

            // gets all parameter/override which value-set to update
            var parameterToUpdate = this.ParameterService.GetShallow(transaction, partition, thing.PublishedParameter, securityContext).OfType<Parameter>().ToArray();
            var overridesToUpdate = this.ParameterOverrideService.GetShallow(transaction, partition, thing.PublishedParameter, securityContext).OfType<ParameterOverride>().ToArray();
            if (parameterToUpdate.Length + overridesToUpdate.Length != thing.PublishedParameter.Count)
            {
                throw new InvalidOperationException("All the parameter/override could not be retrieved for update on a publication.");
            }

            this.UpdatePublishedParameter(thing, parameterToUpdate, transaction, partition, securityContext);
            this.UpdatePublishedOverride(thing, overridesToUpdate, transaction, partition, securityContext);
            return true;
        }

        /// <summary>
        /// Update the <see cref="ParameterValueSet"/> associated to the <see cref="Parameter"/>s to publish
        /// </summary>
        /// <param name="thing">The <see cref="Publication"/></param>
        /// <param name="parameterToUpdate">The <see cref="Parameter"/>to publish</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        private void UpdatePublishedParameter(Publication thing, IReadOnlyCollection<Parameter> parameterToUpdate, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var parameterValueSets = this.ParameterValueSetService.GetShallow(transaction, partition, parameterToUpdate.SelectMany(po => po.ValueSets), securityContext).
                OfType<ParameterValueSet>().
                ToArray();

            foreach (var parameterOrOverrideBase in parameterToUpdate)
            {
                var sets = parameterValueSets.Where(v => parameterOrOverrideBase.ValueSets.Contains(v.Iid)).ToArray();
                foreach (var set in sets)
                {
                    switch (set.ValueSwitch)
                    {
                        case ParameterSwitchKind.COMPUTED:
                            set.Published = set.Computed;
                            break;
                        case ParameterSwitchKind.MANUAL:
                            set.Published = set.Manual;
                            break;
                        case ParameterSwitchKind.REFERENCE:
                            set.Published = set.Reference;
                            break;
                    }

                    if (!this.ParameterValueSetService.UpdateConcept(transaction, partition, set, parameterOrOverrideBase))
                    {
                        throw new InvalidOperationException(string.Format("The parameter value set {0} could not be updated", set.Iid));
                    }
                }

                if (!thing.Domain.Contains(parameterOrOverrideBase.Owner) && sets.Length > 0)
                {
                    thing.Domain.Add(parameterOrOverrideBase.Owner);
                }
            }
        }

        /// <summary>
        /// Update the <see cref="ParameterOverrideValueSet"/> associated to the <see cref="ParameterOverride"/>s to publish
        /// </summary>
        /// <param name="thing">The <see cref="Publication"/></param>
        /// <param name="overrideToUpdate">The <see cref="ParameterOverride"/>to publish</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        private void UpdatePublishedOverride(Publication thing, IReadOnlyCollection<ParameterOverride> overrideToUpdate, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var overrideValueSets = this.ParameterOverrideValueSetService.GetShallow(transaction, partition, overrideToUpdate.SelectMany(po => po.ValueSets), securityContext).
                OfType<ParameterOverrideValueSet>().
                ToArray();

            foreach (var parameterOrOverrideBase in overrideToUpdate)
            {
                var sets = overrideValueSets.Where(v => parameterOrOverrideBase.ValueSets.Contains(v.Iid)).ToArray();
                foreach (var set in sets)
                {
                    switch (set.ValueSwitch)
                    {
                        case ParameterSwitchKind.COMPUTED:
                            set.Published = set.Computed;
                            break;
                        case ParameterSwitchKind.MANUAL:
                            set.Published = set.Manual;
                            break;
                        case ParameterSwitchKind.REFERENCE:
                            set.Published = set.Reference;
                            break;
                    }

                    if (!this.ParameterOverrideValueSetService.UpdateConcept(transaction, partition, set, parameterOrOverrideBase))
                    {
                        throw new InvalidOperationException(string.Format("The parameter override value set {0} could not be updated", set.Iid));
                    }
                }

                if (!thing.Domain.Contains(parameterOrOverrideBase.Owner) && sets.Length > 0)
                {
                    thing.Domain.Add(parameterOrOverrideBase.Owner);
                }
            }
        }
    }
}