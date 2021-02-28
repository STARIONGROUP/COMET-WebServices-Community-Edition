// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterService.cs" company="RHEA System S.A.">
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

    using BusinessLogic;

    using CDP4Common.Dto;
    using CDP4Common.DTO;

    using Npgsql;

    using Operations.SideEffects;

    /// <summary>
    /// Extension for the code-generated <see cref="ParameterService"/>
    /// </summary>
    public partial class ParameterService
    {
        /// <summary>
        /// Gets or sets the operation side effect processor.
        /// </summary>
        public IOperationSideEffectProcessor OperationSideEffectProcessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOldParameterContextProvider"/>
        /// </summary>
        public IOldParameterContextProvider OldParameterContextProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

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
            if (!(sourceThing is Parameter sourceParameter))
            {
                throw new InvalidOperationException("The source is not of the right type");
            }

            var copy = sourceParameter.DeepClone<Parameter>();
            copy.Iid = sourceToCopyMap[sourceParameter.Iid];

            if (copy.Group.HasValue)
            {
                copy.Group = sourceToCopyMap[copy.Group.Value];
            }

            // if source and target iteration are different, remove state dependency. Keep option dependency
            if (copyinfo.Source.IterationId.Value != copyinfo.Target.IterationId.Value)
            {
                copy.StateDependence = null;
            }

            // check parameter type validity if different top-container
            if (copyinfo.Source.TopContainer.Iid != copyinfo.Target.TopContainer.Iid && !rdls.SelectMany(x => x.ParameterType).Contains(copy.ParameterType))
            {
                throw new InvalidOperationException($"Cannot copy {copy.ClassKind} with parameter-type id {copy.ParameterType}. The parameter-type is unavailable in the targetted model.");
            }

            if (copyinfo.Options.KeepOwner.HasValue
                && (!copyinfo.Options.KeepOwner.Value
                    || copyinfo.Options.KeepOwner.Value
                    && !targetEngineeringModelSetup.ActiveDomain.Contains(copy.Owner)
                )
            )
            {
                copy.Owner = copyinfo.ActiveOwner;
            }

            if (!this.OperationSideEffectProcessor.BeforeCreate(copy, targetContainer, transaction, partition, securityContext))
            {
                return;
            }

            this.ParameterDao.Write(transaction, partition, copy, targetContainer);
            this.OperationSideEffectProcessor.AfterCreate(copy, targetContainer, null, transaction, partition, securityContext);

            var newparameter = this.ParameterDao.Read(transaction, partition, new[] { copy.Iid }).Single();

            if (copyinfo.Options.KeepValues.HasValue && copyinfo.Options.KeepValues.Value)
            {
                var valuesets = this.ValueSetService
                    .GetShallow(transaction, partition, newparameter.ValueSet, securityContext)
                    .OfType<ParameterValueSet>().ToList();

                var topcontainerPartition = $"EngineeringModel_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";
                this.TransactionManager.SetIterationContext(transaction, topcontainerPartition, copyinfo.Source.IterationId.Value);
                var sourcepartition = $"Iteration_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";
                var iteration = (Iteration)this.IterationService.Get(transaction, topcontainerPartition, new Guid[] {copyinfo.Source.IterationId.Value}, securityContext).SingleOrDefault();
                if (iteration == null)
                {
                    throw new InvalidOperationException($"The source iteration {copyinfo.Source.IterationId.Value} could not be found.");
                }

                this.OldParameterContextProvider.Initialize(sourceParameter, transaction, sourcepartition, securityContext, iteration);
                
                // switch back to request context
                var engineeringModelPartition = partition.Replace("Iteration", "EngineeringModel");
                this.TransactionManager.SetIterationContext(transaction, engineeringModelPartition, copyinfo.Target.IterationId.Value);

                // update all value-set
                this.DefaultValueArrayFactory.Load(transaction, securityContext);
                foreach (var valueset in valuesets)
                {
                    var sourceValueset = this.OldParameterContextProvider.GetsourceValueSet(valueset.ActualOption, valueset.ActualState) ?? this.OldParameterContextProvider.GetDefaultValue();
                    if (sourceValueset == null)
                    {
                        Logger.Warn("No source value-set was found for the copy operation.");
                        continue;
                    }

                    sourceToCopyMap[sourceValueset.Iid] = valueset.Iid;

                    valueset.Manual = sourceValueset.Manual;
                    valueset.Computed = sourceValueset.Computed;
                    valueset.Reference = sourceValueset.Reference;
                    valueset.ValueSwitch = sourceValueset.ValueSwitch;
                    valueset.Formula = this.DefaultValueArrayFactory.CreateDefaultValueArray(newparameter.ParameterType);
                    valueset.Published = this.DefaultValueArrayFactory.CreateDefaultValueArray(newparameter.ParameterType);

                    this.ValueSetService.UpdateConcept(transaction, partition, valueset, copy);
                }
            }

            var sourceSubscriptions = allSourceThings.OfType<ParameterSubscription>().Where(x => sourceParameter.ParameterSubscription.Contains(x.Iid)).ToList();
            foreach (var sourceSubscription in sourceSubscriptions)
            {
                if (sourceSubscription.Owner == newparameter.Owner)
                {
                    // do not create subscriptions
                    continue;
                }

                ((ServiceBase)this.ParameterSubscriptionService).Copy(transaction, partition, sourceSubscription, newparameter, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
            }
        }
    }
}
