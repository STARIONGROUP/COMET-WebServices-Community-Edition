// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using Authorization;
    
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    
    using Npgsql;
    
    using Operations.SideEffects;

    /// <summary>
    /// Extension for the code-generated <see cref="ParameterOverrideService"/>
    /// </summary>
    public partial class ParameterOverrideService
    {
        /// <summary>
        /// Gets or sets the operation side effect processor.
        /// </summary>
        public IOperationSideEffectProcessor OperationSideEffectProcessor { get; set; }

        /// <summary>
        /// Copy the <paramref name="sourceThing"/> into the target <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="sourceThing">The source <see cref="Thing"/> to copy</param>
        /// <param name="targetContainer">The target container <see cref="Thing"/></param>
        /// <param name="allSourceThings">All source <see cref="Thing"/>s in the current copy operation</param>
        /// <param name="copyinfo">The <see cref="CopyInfo"/></param>
        /// <param name="sourceToCopyMap">A dictionary mapping identifiers of original to copy</param>
        /// <param name="rdls">The <see cref="ReferenceDataLibrary"/></param>
        /// <param name="targetEngineeringModelSetup"></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public override void Copy(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo,
            Dictionary<Guid, Guid> sourceToCopyMap, IReadOnlyList<ReferenceDataLibrary> rdls, EngineeringModelSetup targetEngineeringModelSetup, ISecurityContext securityContext)
        {
            if (!(sourceThing is ParameterOverride sourceParameterOverride))
            {
                throw new InvalidOperationException("The source is not of the right type");
            }

            var copy = sourceParameterOverride.DeepClone<ParameterOverride>();
            copy.Iid = sourceToCopyMap[sourceParameterOverride.Iid];

            if (copyinfo.Options.KeepOwner.HasValue
                && (!copyinfo.Options.KeepOwner.Value
                    || copyinfo.Options.KeepOwner.Value
                    && !targetEngineeringModelSetup.ActiveDomain.Contains(copy.Owner)
                )
            )
            {
                copy.Owner = copyinfo.ActiveOwner;
            }

            if (copyinfo.Source.IterationId.Value != copyinfo.Target.IterationId.Value)
            {
                copy.Parameter = sourceToCopyMap[sourceParameterOverride.Parameter];
            }

            if (!this.OperationSideEffectProcessor.BeforeCreate(copy, targetContainer, transaction, partition, securityContext))
            {
                return;
            }

            this.ParameterOverrideDao.Write(transaction, partition, copy, targetContainer);
            this.OperationSideEffectProcessor.AfterCreate(copy, targetContainer, null, transaction, partition, securityContext);

            var newparameterOverride = this.ParameterOverrideDao.Read(transaction, partition, new[] { copy.Iid }).Single();


            if (copyinfo.Options.KeepValues.HasValue && copyinfo.Options.KeepValues.Value)
            {
                var valuesets = this.ValueSetService
                    .GetShallow(transaction, partition, newparameterOverride.ValueSet, securityContext)
                    .OfType<ParameterOverrideValueSet>().ToList();

                // update all value-set
                foreach (var valueset in valuesets)
                {
                    var sourceToCopyOverridenValueSetPair = sourceToCopyMap.SingleOrDefault(x => x.Value == valueset.ParameterValueSet);

                    var sourceOverridenValueSet = allSourceThings.OfType<ParameterOverrideValueSet>()
                        .FirstOrDefault(x => x.ParameterValueSet == sourceToCopyOverridenValueSetPair.Key
                                             && sourceParameterOverride.ValueSet.Contains(x.Iid)
                                             || x.ParameterValueSet == valueset.ParameterValueSet); // an override may be copied without its parameter (secod condition)

                    if (sourceOverridenValueSet == null)
                    {
                        continue;
                    }

                    sourceToCopyMap[sourceOverridenValueSet.Iid] = valueset.Iid;

                    valueset.Manual = sourceOverridenValueSet.Manual;
                    valueset.Computed = sourceOverridenValueSet.Computed;
                    valueset.Reference = sourceOverridenValueSet.Reference;
                    valueset.Published = new ValueArray<string>(Enumerable.Repeat("-", valueset.Manual.Count));
                    valueset.ValueSwitch = sourceOverridenValueSet.ValueSwitch;

                    this.ValueSetService.UpdateConcept(transaction, partition, valueset, copy);
                }
            }

            var sourceSubscriptions = allSourceThings.OfType<ParameterSubscription>().Where(x => sourceParameterOverride.ParameterSubscription.Contains(x.Iid)).ToList();
            foreach (var sourceSubscription in sourceSubscriptions)
            {
                if (sourceSubscription.Owner == newparameterOverride.Owner)
                {
                    // do not create subscriptions
                    continue;
                }

                ((ServiceBase)this.ParameterSubscriptionService).Copy(transaction, partition, sourceSubscription, newparameterOverride, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
            }
        }
    }
}
