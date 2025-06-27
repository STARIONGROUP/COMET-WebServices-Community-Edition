// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
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
    using System.Threading.Tasks;

    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="ParameterService" />
    /// </summary>
    public partial class ParameterService
    {
        /// <summary>
        /// Gets or sets the operation side effect processor.
        /// </summary>
        public IOperationSideEffectProcessor OperationSideEffectProcessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOldParameterContextProvider" />
        /// </summary>
        public IOldParameterContextProvider OldParameterContextProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory" />
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="IIterationService" />
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="ICachedReferenceDataService" />, used to query SiteDirectory things
        /// </summary>
        public ICachedReferenceDataService CachedReferenceDataService { get; set; }

        /// <summary>
        /// Queries all referenced <see cref="Thing" /> from the SiteDirectory (<see cref="ParameterType" />,
        /// <see cref="MeasurementScale" />, ...) for a <see cref="Parameter" />
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter" /> to use to retrieve SiteDirectory <see cref="Thing" />s</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction" /></param>
        /// <param name="securityContext">The <see cref="ISecurityContext" /></param>
        /// <returns>A collection of referenced <see cref="Thing" />s</returns>
        /// <exception cref="ThingNotFoundException">If one of the referenced <see cref="Thing" /> can not be retrieved</exception>
        public async Task<IReadOnlyCollection<Thing>> QueryReferencedSiteDirectoryThingsAsync(Parameter parameter, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var things = new List<Thing>();

            if (!(await this.CachedReferenceDataService.QueryParameterTypesAsync(transaction, securityContext)).TryGetValue(parameter.ParameterType, out var parameterType))
            {
                throw new ThingNotFoundException($"ParameterType {parameter.ParameterType} does not exist");
            }

            things.Add(parameterType);

            if (parameter.Scale.HasValue)
            {
                if (!(await this.CachedReferenceDataService.QueryMeasurementScalesAsync(transaction, securityContext)).TryGetValue(parameter.Scale.Value, out var measurementScale))
                {
                    throw new ThingNotFoundException($"MeasurementScale {parameter.Scale.Value} does not exist");
                }

                things.Add(measurementScale);
            }

            switch (parameterType)
            {
                case EnumerationParameterType enumerationParameterType:
                    things.AddRange(await this.QueryEnumerationValueDefinitions(enumerationParameterType, transaction, securityContext));
                    break;
                case SampledFunctionParameterType sampledFunctionParameterType:
                    things.AddRange(await this.QueryParameterTypeAssignmentsAsync(sampledFunctionParameterType, transaction, securityContext));
                    break;
                case CompoundParameterType compoundParameterType:
                    things.AddRange(await this.QueryParameterTypeComponentsAsync(compoundParameterType, transaction, securityContext));
                    break;
            }

            return things.DistinctBy(x => x.Iid).ToList();
        }

        /// <summary>
        /// Copy the <paramref name="sourceThing" /> into the target <paramref name="partition" />
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="sourceThing">The source <see cref="Thing" /> to copy</param>
        /// <param name="targetContainer">The target container <see cref="Thing" /></param>
        /// <param name="allSourceThings">All source <see cref="Thing" />s in the current copy operation</param>
        /// <param name="copyinfo">The <see cref="CopyInfo" /></param>
        /// <param name="sourceToCopyMap">A dictionary mapping identifiers of original to copy</param>
        /// <param name="rdls">The <see cref="ReferenceDataLibrary" /></param>
        /// <param name="targetEngineeringModelSetup"></param>
        /// <param name="securityContext">The <see cref="ISecurityContext" /></param>
        public override async Task CopyAsync(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo,
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
                    || (copyinfo.Options.KeepOwner.Value
                        && !targetEngineeringModelSetup.ActiveDomain.Contains(copy.Owner))
                )
               )
            {
                copy.Owner = copyinfo.ActiveOwner;
            }

            if (!await this.OperationSideEffectProcessor.BeforeCreateAsync(copy, targetContainer, transaction, partition, securityContext))
            {
                return;
            }

            await this.ParameterDao.WriteAsync(transaction, partition, copy, targetContainer);
            await this.OperationSideEffectProcessor.AfterCreateAsync(copy, targetContainer, null, transaction, partition, securityContext);

            var newparameter = (await this.ParameterDao.ReadAsync(transaction, partition, [copy.Iid])).Single();

            if (copyinfo.Options.KeepValues.HasValue && copyinfo.Options.KeepValues.Value)
            {
                var valuesets = (await this.ValueSetService
                    .GetShallowAsync(transaction, partition, newparameter.ValueSet, securityContext))
                    .OfType<ParameterValueSet>().ToList();

                var topcontainerPartition = $"EngineeringModel_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";
                await this.TransactionManager.SetIterationContextAsync(transaction, topcontainerPartition, copyinfo.Source.IterationId.Value);
                var sourcepartition = $"Iteration_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";
                var iteration = (Iteration)(await this.IterationService.GetAsync(transaction, topcontainerPartition, [copyinfo.Source.IterationId.Value], securityContext)).SingleOrDefault();

                if (iteration == null)
                {
                    throw new InvalidOperationException($"The source iteration {copyinfo.Source.IterationId.Value} could not be found.");
                }

                await this.OldParameterContextProvider.InitializeAsync(sourceParameter, transaction, sourcepartition, securityContext, iteration);

                // switch back to request context
                var engineeringModelPartition = partition.Replace("Iteration", "EngineeringModel");
                await this.TransactionManager.SetIterationContextAsync(transaction, engineeringModelPartition, copyinfo.Target.IterationId.Value);

                // update all value-set
                await this.DefaultValueArrayFactory.LoadAsync(transaction, securityContext);

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

                    var fullAccesEnabled = this.TransactionManager.IsFullAccessEnabled();

                    if (!fullAccesEnabled)
                    {
                        this.TransactionManager.SetFullAccessState(true);
                    }

                    try
                    {
                        await this.ValueSetService.UpdateConceptAsync(transaction, partition, valueset, copy);
                    }
                    finally
                    {
                        this.TransactionManager.SetFullAccessState(fullAccesEnabled);
                    }
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

                await ((ServiceBase)this.ParameterSubscriptionService).CopyAsync( transaction, partition, sourceSubscription, newparameter, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
            }
        }

        /// <summary>
        /// Queries all <see cref="ParameterTypeComponent" />s with referenced <see cref="ParameterType" /> and
        /// <see cref="MeasurementScale" /> linked to the provided <see cref="CompoundParameterType" />
        /// </summary>
        /// <param name="compoundParameterType">
        /// The <see cref="CompoundParameterType" /> to use to query
        /// <see cref="ParameterTypeComponent" />s
        /// </param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction" /></param>
        /// <param name="securityContext">The <see cref="ISecurityContext" /></param>
        /// <returns>
        /// A collection of linked <see cref="ParameterTypeComponent" />s with associated <see cref="ParameterType" /> and
        /// <see cref="MeasurementScale" />
        /// </returns>
        /// <exception cref="ThingNotFoundException">
        /// If one of the referenced <see cref="ParameterTypeComponent" />,
        /// <see cref="ParameterType" /> or <see cref="MeasurementScale" /> cannot be retrieved
        /// </exception>
        private async Task<List<Thing>> QueryParameterTypeComponentsAsync(CompoundParameterType compoundParameterType, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var things = new List<Thing>();
            var parameterTypeComponents = await this.CachedReferenceDataService.QueryParameterTypeComponentsAsync(transaction, securityContext);
            var parameterTypes = await this.CachedReferenceDataService.QueryParameterTypesAsync(transaction, securityContext);
            var measurementScales = await this.CachedReferenceDataService.QueryMeasurementScalesAsync(transaction, securityContext);

            foreach (var componentId in compoundParameterType.Component.Select(x => x.V).OfType<Guid>())
            {
                if (!parameterTypeComponents.TryGetValue(componentId, out var parameterTypeComponent))
                {
                    throw new ThingNotFoundException($"ParameterTypeComponent {componentId} does not exist");
                }

                things.Add(parameterTypeComponent);
            }

            foreach (var parameterTypeId in things.OfType<ParameterTypeComponent>().Select(x => x.ParameterType).Distinct().ToList())
            {
                if (!parameterTypes.TryGetValue(parameterTypeId, out var parameterType))
                {
                    throw new ThingNotFoundException($"ParameterType {parameterTypeId} does not exist");
                }

                things.Add(parameterType);
            }

            foreach (var measurementScaleId in things.OfType<ParameterTypeComponent>().Where(x => x.Scale.HasValue).Select(x => x.Scale.Value).Distinct().ToList())
            {
                if (!measurementScales.TryGetValue(measurementScaleId, out var measurementScale))
                {
                    throw new ThingNotFoundException($"MeasurementScale {measurementScaleId} does not exist");
                }

                things.Add(measurementScale);
            }

            return things;
        }

        /// <summary>
        /// Queries all <see cref="IParameterTypeAssignment" />s with referenced <see cref="ParameterType" /> and
        /// <see cref="MeasurementScale" /> linked to the provided <see cref="SampledFunctionParameterType" />
        /// </summary>
        /// <param name="sampledFunctionParameterType">
        /// The <see cref="SampledFunctionParameterType" /> to use to query
        /// <see cref="IParameterTypeAssignment" />
        /// </param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction" /></param>
        /// <param name="securityContext">The <see cref="ISecurityContext" /></param>
        /// <returns>
        /// A collection of linked <see cref="IParameterTypeAssignment" />s with associated <see cref="ParameterType" />
        /// and <see cref="MeasurementScale" />
        /// </returns>
        /// <exception cref="ThingNotFoundException">
        /// If one of the referenced <see cref="IParameterTypeAssignment" />,
        /// <see cref="ParameterType" /> or <see cref="MeasurementScale" /> cannot be retrieved
        /// </exception>
        private async Task<List<Thing>> QueryParameterTypeAssignmentsAsync(SampledFunctionParameterType sampledFunctionParameterType, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var things = new List<Thing>();
            var independentParameterTypeAssignements = await this.CachedReferenceDataService.QueryIndependentParameterTypeAssignmentsAsync(transaction, securityContext);
            var parameterTypes = await this.CachedReferenceDataService.QueryParameterTypesAsync(transaction, securityContext);
            var measurementScales = await this.CachedReferenceDataService.QueryMeasurementScalesAsync(transaction, securityContext);

            foreach (var independentParameterTypeId in sampledFunctionParameterType.IndependentParameterType.Select(x => x.V).OfType<Guid>())
            {
                if (!independentParameterTypeAssignements.TryGetValue(independentParameterTypeId, out var independentParameterTypeAssignment))
                {
                    throw new ThingNotFoundException($"IndependentParameterTypeAssignment {independentParameterTypeId} does not exist");
                }

                things.Add(independentParameterTypeAssignment);
            }

            var dependentParameterTypeAssignements = await this.CachedReferenceDataService.QueryDependentParameterTypeAssignmentsAsync(transaction, securityContext);

            foreach (var dependentParameterTypeId in sampledFunctionParameterType.DependentParameterType.Select(x => x.V).OfType<Guid>())
            {
                if (!dependentParameterTypeAssignements.TryGetValue(dependentParameterTypeId, out var dependentParameterTypeAssignment))
                {
                    throw new ThingNotFoundException($"DependentParameterTypeAssignment {dependentParameterTypeId} does not exist");
                }

                things.Add(dependentParameterTypeAssignment);
            }

            foreach (var parameterTypeId in things.OfType<IParameterTypeAssignment>().Select(x => x.ParameterType).Distinct().ToList())
            {
                if (!parameterTypes.TryGetValue(parameterTypeId, out var parameterType))
                {
                    throw new ThingNotFoundException($"ParameterType {parameterTypeId} does not exist");
                }

                things.Add(parameterType);
            }

            foreach (var measurementScaleId in things.OfType<IParameterTypeAssignment>().Where(x => x.MeasurementScale.HasValue).Select(x => x.MeasurementScale.Value).Distinct().ToList())
            {
                if (!measurementScales.TryGetValue(measurementScaleId, out var measurementScale))
                {
                    throw new ThingNotFoundException($"MeasurementScale {measurementScaleId} does not exist");
                }

                things.Add(measurementScale);
            }

            return things;
        }

        /// <summary>
        /// Queries all <see cref="EnumerationValueDefinition" />s linked to the provided <see cref="EnumerationParameterType" />
        /// </summary>
        /// <param name="enumerationParameterType">
        /// The <see cref="EnumerationParameterType" /> to use to query <see cref="EnumerationValueDefinition"/>
        /// </param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction" /></param>
        /// <param name="securityContext">The <see cref="ISecurityContext" /></param>
        /// <returns>
        /// A collection of linked <see cref="EnumerationValueDefinition" />s
        /// </returns>
        /// <exception cref="ThingNotFoundException">
        /// If one of the referenced <see cref="EnumerationValueDefinition" /> cannot be retrieved
        /// </exception>
        private async Task<IEnumerable<Thing>> QueryEnumerationValueDefinitions(EnumerationParameterType enumerationParameterType, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var valueDefinitions = await this.CachedReferenceDataService.QueryEnumerationValueDefinitionsAsync(transaction, securityContext);

            var result = new List<Thing>();

            foreach (var valueDefinitionId in enumerationParameterType.ValueDefinition.Select(x => x.V).OfType<Guid>())
            {
                if (!valueDefinitions.TryGetValue(valueDefinitionId, out var enumerationValueDefinition))
                {
                    throw new ThingNotFoundException($"EnumerationValueDefinition {valueDefinitionId} does not exist");
                }

                result.Add(enumerationValueDefinition);
            }

            return result;
        }
    }
}
