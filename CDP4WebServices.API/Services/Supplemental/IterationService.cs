// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.Security;
    using Authorization;
    using CDP4Common.DTO;
    using Helpers;
    using NLog;
    using Npgsql;

    /// <summary>
    /// The Iteration Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class IterationService
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The cached active iteration identifier in the context of the current request
        /// </summary>
        private Guid activeIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationSetupService"/>
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        public void CopyIteration(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            this.IterationDao.CopyIteration(transaction, sourcePartition, targetPartition);
        }

        /// <summary>
        /// Delete all organizational participations from element definitions
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        public void DeleteAllrganizationalParticipantThings(NpgsqlTransaction transaction, string targetPartition)
        {
            this.IterationDao.DeleteAllrganizationalParticipantThings(transaction, targetPartition);
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        public void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            this.IterationDao.ModifyUserTrigger(transaction, sourcePartition, enable);
        }

        /// <summary>
        /// Gets the active <see cref="Iteration"/> for the given <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <returns>The active <see cref="Iteration"/></returns>
        public Iteration GetActiveIteration(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.activeIterationId != Guid.Empty)
            {
                var activeIterations = this.GetShallow(transaction, partition, new [] { this.activeIterationId }, securityContext).OfType<Iteration>().ToArray();
                if (activeIterations.Length != 1)
                {
                    throw new ServerException($"The active iteration could not be found for partition {partition}.");
                }

                return activeIterations.Single();
            }
            else
            {
                var iterations = this.GetShallow(transaction, partition, null, securityContext).OfType<Iteration>().ToArray();

                var activeIterationSetups =
                    this.IterationSetupService
                        .GetShallow(transaction, Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, iterations.Select(x => x.IterationSetup), securityContext)
                        .OfType<IterationSetup>()
                        .Where(x => x.FrozenOn == null)
                        .ToArray();

                if (activeIterationSetups.Length != 1)
                {
                    throw new ServerException($"The active iteration-setup could not be found for partition {partition}.");
                }

                var activeIterations = iterations.Where(x => x.Iid == activeIterationSetups.Single().IterationIid).ToArray();
                if (activeIterations.Length != 1)
                {
                    throw new ServerException($"The active iteration could not be found for partition {partition}.");
                }

                var activeIteration = activeIterations.Single();
                this.activeIterationId = activeIteration.Iid;
                return activeIteration;
            }
        }

        /// <summary>
        /// Populates the data of a new iteration using the last iteration
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public void PopulateDataFromLastIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup, EngineeringModel engineeringModel, ISecurityContext securityContext)
        {
            Logger.Info("Creating new iteration using the last iteration");
            var start = Stopwatch.StartNew();

            var newiteration = this.CreateIterationObjectFromSource(transaction, iterationPartition, iterationSetup, sourceIterationSetup, engineeringModel, securityContext);

            this.IterationDao.MoveToNextIterationFromLast(transaction, iterationPartition, newiteration);
            this.PublicationService.DeleteAll(transaction, iterationPartition);
            Logger.Info("End populate data for new iteration. Operation took {0} ms", start.ElapsedMilliseconds);
        }

        /// <summary>
        /// Populates the data of a new iteration using a specific source <paramref name="sourceIterationSetup"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public void PopulateDataFromOlderIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup, EngineeringModel engineeringModel, ISecurityContext securityContext)
        {
            Logger.Info("Creating new iteration using the source iteration number {0}", sourceIterationSetup.IterationNumber);
            var start = Stopwatch.StartNew();
            var engineeringModelPartition = iterationPartition.Replace(Cdp4TransactionManager.ITERATION_PARTITION_PREFIX, Cdp4TransactionManager.ENGINEERING_MODEL_PARTITION_PREFIX);

            // Set end-validity for all current data
            this.IterationDao.SetIterationValidityEnd(transaction, iterationPartition);

            Logger.Info("Setting end validity took {0} ms", start.ElapsedMilliseconds);
            start.Reset();
            start.Start();

            // Get source data to copy
            this.TransactionManager.SetIterationContext(transaction, engineeringModelPartition, sourceIterationSetup.IterationIid);
            var sourceInstant = this.TransactionManager.GetSessionInstant(transaction);

            // disable triggers to delete all current data in the context of this transaction
            this.ModifyUserTrigger(transaction, iterationPartition, false);

            // delete current data
            this.IterationDao.DeleteAllIterationThings(transaction, iterationPartition);

            // re-enable triggers
            this.ModifyUserTrigger(transaction, iterationPartition, true);

            this.IterationDao.InsertDataFromAudit(transaction, iterationPartition, sourceInstant);

            var newiteration = this.CreateIterationObjectFromSource(transaction, iterationPartition, iterationSetup, sourceIterationSetup, engineeringModel, securityContext);
            this.IterationDao.MoveToNextIterationFromLast(transaction, iterationPartition, newiteration);

            Logger.Info("Inserting data took {0} ms", start.ElapsedMilliseconds);

            // Delete Publications (cascading all things that references them)
            this.PublicationService.DeleteAll(transaction, iterationPartition);

            Logger.Info("End populate data for new iteration");
        }

        /// <summary>
        /// Create and return a new <see cref="Iteration"/> from a source
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/> for the <see cref="Iteration"/> to create</param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The security-context</param>
        /// <returns>The new <see cref="Iteration"/></returns>
        private Iteration CreateIterationObjectFromSource(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup, EngineeringModel engineeringModel, ISecurityContext securityContext)
        {
            var engineeringModelPartition = iterationPartition.Replace(Cdp4TransactionManager.ITERATION_PARTITION_PREFIX, Cdp4TransactionManager.ENGINEERING_MODEL_PARTITION_PREFIX);
            var sourceIteration = (Iteration)this.GetShallow(transaction, engineeringModelPartition, new[] { sourceIterationSetup.IterationIid }, securityContext).SingleOrDefault();
            if (sourceIteration == null)
            {
                throw new InvalidOperationException("The source iteration could not be found.");
            }

            var iteration = new Iteration(iterationSetup.IterationIid, 1)
            {
                IterationSetup = iterationSetup.Iid,
                DefaultOption = sourceIteration.DefaultOption,
                SourceIterationIid = sourceIteration.Iid,
                TopElement = sourceIteration.TopElement,
            };

            // create from last iteration
            if (!this.IterationDao.Write(transaction, engineeringModelPartition, iteration, engineeringModel))
            {
                throw new InvalidOperationException($"There was a problem creating the new Iteration: {iteration.Iid} contained by EngineeringModel: {engineeringModel.Iid}");
            }

            return iteration;
        }

        /// <summary>
        /// Persist the supplied <see cref="Iteration"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Iteration"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Iteration"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool CreateConceptFromImportedIteration(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Thing thing, Thing container, long sequence = -1)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var iteration = thing as Iteration;
            var createSuccesful = this.IterationDao.Write(transaction, partition, iteration, container);
            return createSuccesful && this.CreateContaimentFromImportedIteration(transaction, partition, iteration, sequence);
        }

        /// <summary>
        /// Persist the imported(AnnexC3 file) <see cref="Iteration"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContaimentFromImportedIteration(NpgsqlTransaction transaction, string partition, Iteration iteration, long sequence)
        {
            var results = new List<bool>();
            var iterationPartition = partition.Replace("EngineeringModel", "Iteration");

            foreach (var actualFiniteStateList in this.ResolveFromRequestCache(iteration.ActualFiniteStateList))
            {
                results.Add(this.ActualFiniteStateListService.UpsertConcept(transaction, iterationPartition, actualFiniteStateList, iteration));
            }

            if (sequence != 1)
            {
                return true;
            }

            foreach (var diagramCanvas in this.ResolveFromRequestCache(iteration.DiagramCanvas))
            {
                results.Add(this.DiagramCanvasService.CreateConcept(transaction, iterationPartition, diagramCanvas, iteration));
            }

            foreach (var domainFileStore in this.ResolveFromRequestCache(iteration.DomainFileStore))
            {
                results.Add(this.DomainFileStoreService.CreateConcept(transaction, iterationPartition, domainFileStore, iteration));
            }

            foreach (var element in this.ResolveFromRequestCache(iteration.Element))
            {
                results.Add(this.ElementService.CreateConcept(transaction, iterationPartition, element, iteration));
            }

            foreach (var externalIdentifierMap in this.ResolveFromRequestCache(iteration.ExternalIdentifierMap))
            {
                results.Add(this.ExternalIdentifierMapService.CreateConcept(transaction, iterationPartition, externalIdentifierMap, iteration));
            }

            foreach (var goal in this.ResolveFromRequestCache(iteration.Goal))
            {
                results.Add(this.GoalService.CreateConcept(transaction, iterationPartition, goal, iteration));
            }

            foreach (var option in this.ResolveFromRequestCache(iteration.Option))
            {
                results.Add(this.OptionService.CreateConcept(transaction, iterationPartition, (Option)option.V, iteration, option.K));
            }

            foreach (var possibleFiniteStateList in this.ResolveFromRequestCache(iteration.PossibleFiniteStateList))
            {
                results.Add(this.PossibleFiniteStateListService.CreateConcept(transaction, iterationPartition, possibleFiniteStateList, iteration));
            }

            foreach (var publication in this.ResolveFromRequestCache(iteration.Publication))
            {
                results.Add(this.PublicationService.CreateConcept(transaction, iterationPartition, publication, iteration));
            }

            foreach (var relationship in this.ResolveFromRequestCache(iteration.Relationship))
            {
                results.Add(this.RelationshipService.CreateConcept(transaction, iterationPartition, relationship, iteration));
            }

            foreach (var requirementsSpecification in this.ResolveFromRequestCache(iteration.RequirementsSpecification))
            {
                results.Add(this.RequirementsSpecificationService.CreateConcept(transaction, iterationPartition, requirementsSpecification, iteration));
            }

            foreach (var ruleVerificationList in this.ResolveFromRequestCache(iteration.RuleVerificationList))
            {
                results.Add(this.RuleVerificationListService.CreateConcept(transaction, iterationPartition, ruleVerificationList, iteration));
            }

            foreach (var sharedDiagramStyle in this.ResolveFromRequestCache(iteration.SharedDiagramStyle))
            {
                results.Add(this.SharedDiagramStyleService.CreateConcept(transaction, iterationPartition, sharedDiagramStyle, iteration));
            }

            foreach (var stakeholder in this.ResolveFromRequestCache(iteration.Stakeholder))
            {
                results.Add(this.StakeholderService.CreateConcept(transaction, iterationPartition, stakeholder, iteration));
            }

            foreach (var stakeholderValue in this.ResolveFromRequestCache(iteration.StakeholderValue))
            {
                results.Add(this.StakeholderValueService.CreateConcept(transaction, iterationPartition, stakeholderValue, iteration));
            }

            foreach (var stakeholderValueMap in this.ResolveFromRequestCache(iteration.StakeholderValueMap))
            {
                results.Add(this.StakeholderValueMapService.CreateConcept(transaction, iterationPartition, stakeholderValueMap, iteration));
            }

            foreach (var valueGroup in this.ResolveFromRequestCache(iteration.ValueGroup))
            {
                results.Add(this.ValueGroupService.CreateConcept(transaction, iterationPartition, valueGroup, iteration));
            }

            return results.All(x => x);
        }
    }
}
