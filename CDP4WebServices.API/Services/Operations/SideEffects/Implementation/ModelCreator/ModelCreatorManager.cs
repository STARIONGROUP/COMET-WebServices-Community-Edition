﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelCreatorManager.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authentication;
    using Authorization;
    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;
    using Npgsql;

    /// <summary>
    /// Class that handles the creation of an <see cref="EngineeringModel"/> based on a source
    /// </summary>
    public class ModelCreatorManager : IModelCreatorManager
    {
        /// <summary>
        /// The Site-Directory partition name
        /// </summary>
        private const string SITE_DIRECTORY_PARTITION = "SiteDirectory";

        /// <summary>
        /// The collection os <see cref="Thing"/>s contained in the source <see cref="EngineeringModelSetup"/>
        /// </summary>
        private List<Thing> engineeringModelSetupThings;

        /// <summary>
        /// The dictionary that contains original and copies
        /// </summary>
        private Dictionary<Thing, Thing> originalToCopyMap;

        /// <summary>
        /// The <see cref="Guid"/> of the new engineering-model
        /// </summary>
        private Guid newModelIid;

        /// <summary>
        /// Gets a value that indicates whether tre user trigger were disabled
        /// </summary>
        public bool IsUserTriggerDisable { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="IMetaInfoProvider"/> (injected)
        /// </summary>
        public IMetaInfoProvider MetainfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupService"/> (injected)
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelService"/> (injected)
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/> (injected)
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or stes the <see cref="IServiceProvider"/> (injected)
        /// </summary>
        public CDP4WebServices.API.Services.IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRequestUtils"/> (injected)
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRevisionService"/> (injected)
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets the original and their copy
        /// </summary>
        public IReadOnlyDictionary<Thing, Thing> OriginalToCopyMap
        {
            get { return this.originalToCopyMap; }
        }

        /// <summary>
        /// Creates an <see cref="EngineeringModelSetup"/> from a source
        /// </summary>
        /// <param name="source">The identifier of the source <see cref="EngineeringModelSetup"/></param>
        /// <param name="setupToCreate">The new <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="securityContext">The security context</param>
        public void CreateEngineeringModelSetupFromSource(Guid source, EngineeringModelSetup setupToCreate, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            this.engineeringModelSetupThings = new List<Thing>();
            this.originalToCopyMap = new Dictionary<Thing, Thing>();

            // retrieve all the site-directory data to copy
            this.RequestUtils.QueryParameters.IncludeReferenceData = true; // set to true only to retrieve all data to copy. set back to false once query is over
            this.engineeringModelSetupThings.AddRange(this.EngineeringModelSetupService.GetDeep(transaction, SITE_DIRECTORY_PARTITION, new[] { source }, securityContext));
            this.RequestUtils.QueryParameters.IncludeReferenceData = false;

            this.newModelIid = setupToCreate.EngineeringModelIid;

            // create copy for all things, iteration-setup included
            foreach (var engineeringModelSetupThing in this.engineeringModelSetupThings)
            {
                if (engineeringModelSetupThing is EngineeringModelSetup)
                {
                    this.originalToCopyMap.Add(engineeringModelSetupThing, setupToCreate);
                    continue;
                }

                var metainfo = this.MetainfoProvider.GetMetaInfo(engineeringModelSetupThing.ClassKind.ToString());
                var copy = metainfo.InstantiateDto(Guid.NewGuid(), 0);
                this.originalToCopyMap.Add(engineeringModelSetupThing, copy);
            }

            var modelSetupBeforeResolve = setupToCreate.DeepClone<EngineeringModelSetup>();
            foreach (var originalToCopy in this.originalToCopyMap)
            {
                originalToCopy.Value.ResolveCopy(originalToCopy.Key, this.originalToCopyMap);
            }

            // reset value-type properties to use user's values
            setupToCreate.Iid = modelSetupBeforeResolve.Iid;
            setupToCreate.EngineeringModelIid = modelSetupBeforeResolve.EngineeringModelIid;
            setupToCreate.Kind = modelSetupBeforeResolve.Kind;
            setupToCreate.StudyPhase = modelSetupBeforeResolve.StudyPhase;
            setupToCreate.SourceEngineeringModelSetupIid = modelSetupBeforeResolve.SourceEngineeringModelSetupIid;
            setupToCreate.Name = modelSetupBeforeResolve.Name;
            setupToCreate.ShortName = modelSetupBeforeResolve.ShortName;

            foreach (var iterationSetup in this.originalToCopyMap.Keys.OfType<IterationSetup>())
            {
                var copy = (IterationSetup)this.originalToCopyMap[iterationSetup];
                copy.IterationIid = Guid.NewGuid();
                copy.SourceIterationSetup = iterationSetup.Iid;
            }

            // populate the RequestUtils cache so that contained things are created when the engineering-model setup is created
            this.RequestUtils.Cache.Clear();
            this.RequestUtils.Cache.AddRange(this.originalToCopyMap.Values);
        }

        /// <summary>
        /// Copy the engineering-model data from the source to the target <see cref="EngineeringModel"/>
        /// </summary>
        /// <param name="newModelSetup">The new <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="securityContext">The security context</param>
        public void CopyEngineeringModelData(EngineeringModelSetup newModelSetup, NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            // copy data of all concrete tables
            var modelSetupKvp = this.originalToCopyMap.SingleOrDefault(kvp => kvp.Key is EngineeringModelSetup);
            if (modelSetupKvp.Key == null)
            {
                throw new InvalidOperationException("The engineering model setup to copy could not be found.");
            }

            var sourceModelSetup = (EngineeringModelSetup)modelSetupKvp.Key;
            var sourcePartition = this.RequestUtils.GetEngineeringModelPartitionString(sourceModelSetup.EngineeringModelIid);
            var targetPartition = this.RequestUtils.GetEngineeringModelPartitionString(newModelSetup.EngineeringModelIid);

            var sourceIterationPartition = sourcePartition.Replace(CDP4Orm.Dao.Utils.EngineeringModelPartition, CDP4Orm.Dao.Utils.IterationSubPartition);
            var targetIterationPartition = targetPartition.Replace(CDP4Orm.Dao.Utils.EngineeringModelPartition, CDP4Orm.Dao.Utils.IterationSubPartition);

            // copy all data from the source to the target partition
            // disable user triggers
            this.DisableUserTrigger(transaction);

            this.EngineeringModelService.CopyEngineeringModel(transaction, sourcePartition, targetPartition);
            this.IterationService.CopyIteration(transaction, sourceIterationPartition, targetIterationPartition);

            // change id on Thing table for all other things
            this.EngineeringModelService.ModifyIdentifier(transaction, targetPartition);
            this.EngineeringModelService.ModifyIdentifier(transaction, targetIterationPartition);

            // update iid for engineering-model and iteration(s)
            var newEngineeringModel = this.EngineeringModelService.GetShallow(transaction, targetPartition, null, securityContext).OfType<EngineeringModel>().SingleOrDefault();
            if (newEngineeringModel == null)
            {
                throw new InvalidOperationException("No EngineeringModel was found.");
            }

            var oldIid = newEngineeringModel.Iid;
            newEngineeringModel.Iid = newModelSetup.EngineeringModelIid;
            newEngineeringModel.EngineeringModelSetup = newModelSetup.Iid;

            this.EngineeringModelService.ModifyIdentifier(transaction, targetPartition, newEngineeringModel, oldIid);

            if (!this.EngineeringModelService.UpdateConcept(transaction, targetPartition, newEngineeringModel, null))
            {
                throw new InvalidOperationException("Updating the copied EngineeringModel failed.");
            }

            // modify references of things contained in the new engineering-model-setup (rdl included)
            var modelThings = this.EngineeringModelService.GetDeep(transaction, targetPartition, null, securityContext).ToList();
            modelThings.AddRange(this.IterationService.GetDeep(transaction, targetPartition, null, securityContext));
            foreach (var modelThing in modelThings)
            {
                var model = modelThing as EngineeringModel;
                if (model != null)
                {
                    continue;
                }

                var iteration = modelThing as Iteration;
                if (iteration != null)
                {
                    var iterationSetupKvp = this.originalToCopyMap.SingleOrDefault(kvp => kvp.Key.Iid == iteration.IterationSetup);
                    if (iterationSetupKvp.Key == null)
                    {
                        throw new InvalidOperationException("The original/copy iteration-setup pair could not be found in the cache");
                    }

                    var oldIterationIid = iteration.Iid;
                    iteration.Iid = ((IterationSetup)iterationSetupKvp.Value).IterationIid;
                    iteration.IterationSetup = iterationSetupKvp.Value.Iid;
                    iteration.DefaultOption = modelThings.OfType<Option>().First().Iid;

                    this.EngineeringModelService.ModifyIdentifier(transaction, targetPartition, iteration, oldIterationIid);
                    if (!this.IterationService.UpdateConcept(transaction, targetPartition, iteration, newEngineeringModel))
                    {
                        throw new InvalidOperationException("Updating the copied Iteration failed.");
                    }

                    // save revision-registry
                    this.RevisionService.InsertRevisionRegistry(transaction, targetPartition, 1, this.RequestUtils.Context.AuthenticatedCredentials.Person.Iid);
                    this.RevisionService.InsertIterationRevisionLog(transaction, targetPartition, iteration.Iid, null, null);

                    continue;
                }

                // resolve
                if (modelThing.ResolveCopyReference(this.originalToCopyMap))
                {
                    var container = modelThings.SingleOrDefault(x => x.Contains(modelThing));
                    var metadata = this.MetainfoProvider.GetMetaInfo(modelThing.ClassKind.ToString());
                    PropertyMetaInfo propertyMetainfo;
                    var partition = metadata.TryGetContainerProperty("EngineeringModel", out propertyMetainfo) ? targetPartition : targetIterationPartition;

                    var service = this.ServiceProvider.MapToPersitableService(modelThing.ClassKind.ToString());
                    
                    service.UpdateConcept(transaction, partition, modelThing, container);
                }
            }


            // IMPORTANT: re-enable user trigger once the current transaction is commited commited
        }

        /// <summary>
        /// Disable the user-triggers in the engineering-model and iteration schema
        /// </summary>
        /// <param name="transaction">The transaction</param>
        private void DisableUserTrigger(NpgsqlTransaction transaction)
        {
            var partition = this.RequestUtils.GetEngineeringModelPartitionString(this.newModelIid);
            var iterationPartition = partition.Replace(CDP4Orm.Dao.Utils.EngineeringModelPartition, CDP4Orm.Dao.Utils.IterationSubPartition);

            this.EngineeringModelService.ModifyUserTrigger(transaction, partition, false);
            this.IterationService.ModifyUserTrigger(transaction, iterationPartition, false);

            this.IsUserTriggerDisable = true;
        }

        /// <summary>
        /// Enable the user-triggers in the engineering-model and iteration schema
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        public void EnableUserTrigger(NpgsqlTransaction transaction)
        {
            if (!this.IsUserTriggerDisable)
            {
                return;
            }

            var partition = this.RequestUtils.GetEngineeringModelPartitionString(this.newModelIid);
            var iterationPartition = partition.Replace(CDP4Orm.Dao.Utils.EngineeringModelPartition, CDP4Orm.Dao.Utils.IterationSubPartition);

            this.EngineeringModelService.ModifyUserTrigger(transaction, partition, true);
            this.IterationService.ModifyUserTrigger(transaction, iterationPartition, true);
        }
    }
}