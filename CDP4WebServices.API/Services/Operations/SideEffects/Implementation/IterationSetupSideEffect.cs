// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authorization;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CDP4WebServices.API.Services.Authentication;
    using Helpers;
    using NLog;
    using Npgsql;

    /// <summary>
    /// The iteration setup side effect.
    /// </summary>
    public sealed class IterationSetupSideEffect : OperationSideEffect<IterationSetup>
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelService"/>
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationSetupService"/>
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRevisionService"/>
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonResolver"/>
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelDao"/>
        /// </summary>
        public IEngineeringModelDao EngineeringModelDao { get; set; }

        /// <summary>
        /// Execute additional logic before a create operation.
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
        public override bool BeforeCreate(IterationSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            thing.CreatedOn = this.TransactionManager.GetTransactionTime(transaction);

            if (string.IsNullOrWhiteSpace(thing.Description))
            {
                thing.Description = "-";
            }

            var engineeringModelSetup = (EngineeringModelSetup)container;

            // switch partition to engineeringModel
            var engineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(engineeringModelSetup.EngineeringModelIid);

            // set the next iteration number
            thing.IterationNumber = this.EngineeringModelDao.GetNextIterationNumber(transaction, engineeringModelPartition);
            return true;
        }

        /// <summary>
        /// Executes additional logic after a successful create IterationSetup operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that was created.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that was created.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
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
        public override void AfterCreate(IterationSetup thing, Thing container, IterationSetup originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // Freeze all other iterationSetups contained by this EngineeringModelSetup that are not frozen yet
            var engineeringModelSetup = (EngineeringModelSetup)container;
            var iterationSetupIidsToUpdate = engineeringModelSetup.IterationSetup.Except(new[] { thing.Iid });
            var iterationSetupsToUpdate = this.IterationSetupService.GetShallow(transaction, partition, iterationSetupIidsToUpdate, securityContext).OfType<IterationSetup>();

            foreach (var iterationSetup in iterationSetupsToUpdate.Where(x => x.FrozenOn == null && x.Iid != thing.Iid))
            {
                iterationSetup.FrozenOn = this.TransactionManager.GetTransactionTime(transaction);
                this.IterationSetupService.UpdateConcept(transaction, partition, iterationSetup, container);
            }

            // Create the iteration for the IterationSetup
            var iteration = new Iteration(thing.IterationIid, 1) { IterationSetup = thing.Iid };
            var engineeringModelIid = engineeringModelSetup.EngineeringModelIid;

            // switch partition to engineeringModel
            var engineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(engineeringModelIid);
            
            // make sure to switch security context to participant based (as we're going to operate on engineeringmodel data)
            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            credentials.EngineeringModelSetup = engineeringModelSetup;
            this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
            this.PermissionService.Credentials = credentials;

            var engineeringModel = this.EngineeringModelService.GetShallow(transaction, engineeringModelPartition, new[] { engineeringModelIid }, securityContext).SingleOrDefault() as EngineeringModel;

            var sourceIterationSetups = this.IterationSetupService.GetShallow(
                transaction,
                Cdp4TransactionManager.SITE_DIRECTORY_PARTITION,
                engineeringModelSetup.IterationSetup,
                securityContext).Where(x => x.Iid != thing.Iid).Cast<IterationSetup>().ToList();

            // update iteration partition with source data if applicable
            if (thing.SourceIterationSetup.HasValue)
            {
                var sourceIterationSetup = sourceIterationSetups.SingleOrDefault(x => x.Iid == thing.SourceIterationSetup.Value);
                if (sourceIterationSetup == null)
                {
                    throw new InvalidOperationException("The source iteration-setup could not be found.");
                }

                var lastIterationNumber = sourceIterationSetups.Max(x => x.IterationNumber);
                var iterationPartition = $"{Cdp4TransactionManager.ITERATION_PARTITION_PREFIX}{engineeringModelIid.ToString().Replace("-", "_")}";
                if (sourceIterationSetup.IterationNumber != lastIterationNumber)
                {
                    this.IterationService.PopulateDataFromOlderIteration(transaction, iterationPartition, thing, sourceIterationSetup, engineeringModel, securityContext);
                }
                else
                {
                    this.IterationService.PopulateDataFromLastIteration(transaction, iterationPartition, thing, sourceIterationSetup, engineeringModel, securityContext);
                }
            }
            else if (sourceIterationSetups.Count > 0)
            {
                throw new InvalidOperationException("The source iteration is null. Please set the source of the new iteration to create.");
            }

            // Create revisions for created Iteration and updated EngineeringModel
            var actor = credentials.Person.Iid;

            this.TransactionManager.SetDefaultContext(transaction);
            this.TransactionManager.SetCachedDtoReadEnabled(false);
            var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, engineeringModelPartition);
            this.RevisionService.SaveRevisions(transaction, engineeringModelPartition, actor, transactionRevision);
        }

        /// <summary>
        /// Check before actually deleting the <see cref="IterationSetup"/> that it is frozen
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
        public override void BeforeDelete(IterationSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (thing.FrozenOn == null)
            {
                throw new InvalidOperationException("It is not possible to delete the current iteration.");
            }

        }

        /// <summary>
        /// Executes additional logic after a successful delete IterationSetup operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that was deleted.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that was deleted.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted from.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterDelete(IterationSetup thing, Thing container, IterationSetup originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var modelSetup = (EngineeringModelSetup)container;
            var engineeringModelIid = modelSetup.EngineeringModelIid;

            // make sure to switch security context to participant based (as we're going to operate on engineeringmodel data)
            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            credentials.EngineeringModelSetup = modelSetup;
            this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
            this.PermissionService.Credentials = credentials;

            // switch partition to engineeringModel
            var engineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(engineeringModelIid);
            var iteration = this.IterationService.GetShallow(transaction, engineeringModelPartition, new List<Guid> { thing.IterationIid }, securityContext).OfType<Iteration>().SingleOrDefault();

            if (iteration == null)
            {
                Logger.Warn($"The iteration {thing.IterationIid} was not found in the database.");
                return;
            }

            var engineeringModel = this.EngineeringModelService.GetShallow(transaction, engineeringModelPartition, new List<Guid> { engineeringModelIid }, securityContext).OfType<EngineeringModel>().SingleOrDefault();

            if (engineeringModel == null)
            {
                throw new InvalidOperationException($"The Engineering Model with iid {engineeringModelIid} could not be found in {engineeringModelPartition}");
            }

            // Remove the iteration 
            if (!this.IterationService.DeleteConcept(transaction, engineeringModelPartition, iteration, engineeringModel))
            {
                throw new InvalidOperationException($"There was a problem deleting the Iteration: {thing.IterationIid} contained by EngineeringModel: {engineeringModelIid}");
            }
        }

        /// <summary>
        /// Read the current state of the top container.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// A top container instance.
        /// </returns>
        private Thing GetTopContainer(NpgsqlTransaction transaction, string partition)
        {
            return this.EngineeringModelService.GetShallow(transaction, partition, null, new RequestSecurityContext { ContainerReadAllowed = true }).FirstOrDefault();
        }
    }
}