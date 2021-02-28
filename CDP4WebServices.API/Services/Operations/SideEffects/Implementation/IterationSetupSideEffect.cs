// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupSideEffect.cs" company="RHEA System S.A.">
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
    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Services.Authentication;

    using Helpers;

    using NLog;

    using Npgsql;

    using System;
    using System.Linq;

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
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="IterationSetup"/> instance that will be inspected.
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
            IterationSetup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (!rawUpdateInfo.ContainsKey("SourceIterationSetup"))
            {
                return;
            }

            var sourceIterationSetupIid = (Guid)rawUpdateInfo["SourceIterationSetup"];
            var engineeringModelSetup = (EngineeringModelSetup)container;

            // Check that sourceIterationSetup is from the same EngineeringModelSetup
            if (!engineeringModelSetup.IterationSetup.Contains(sourceIterationSetupIid))
            {
                throw new AcyclicValidationException($"IterationSetup {thing.Iid} cannot have " +
                                                     $"a source IterationSetup from outside the current EngineeringModelSetup.");
            }

            var iterationSetups = this.IterationSetupService
                .Get(transaction, partition, engineeringModelSetup.IterationSetup, securityContext)
                .Cast<IterationSetup>()
                .ToList();

            Guid? next = sourceIterationSetupIid;

            do
            {
                if (next == thing.Iid)
                {
                    throw new AcyclicValidationException($"IterationSetup {thing.Iid} cannot have " +
                                                         $"a source IterationSetup that leads to cyclic dependency.");
                }

                next = iterationSetups.FirstOrDefault(x => x.Iid == next)?.SourceIterationSetup;
            } while(next != null);
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
            var iterationSetups = this.IterationSetupService.GetShallow(transaction, partition, new [] {thing.Iid}, securityContext)
                                                            .OfType<IterationSetup>();

            var iterationSetup = iterationSetups.SingleOrDefault();

            if (iterationSetup == null)
            {
                throw new InvalidOperationException("IterationSetup is null.");
            }

            if (iterationSetup.FrozenOn == null)
            {
                throw new InvalidOperationException("It is not possible to delete the current iteration.");
            }

            if (iterationSetup.IsDeleted != false)
            {
                return;
            }

            // Swtitch partition to EngineeringModel
            var engineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(((EngineeringModelSetup)container).EngineeringModelIid);

            // Make sure to switch security context to participant based (as we're going to operate on engineeringmodel data)
            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            credentials.EngineeringModelSetup = (EngineeringModelSetup)container;
            this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
            this.PermissionService.Credentials = credentials;

            var iteration = this.IterationService.GetShallow(transaction, engineeringModelPartition, new [] {iterationSetup.IterationIid}, securityContext)
                                                 .Cast<Iteration>()
                                                 .SingleOrDefault();

            this.IterationService.DeleteConcept(transaction, engineeringModelPartition, iteration, container);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
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
        public override void AfterDelete(IterationSetup thing, Thing container, IterationSetup originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var iterationSetup = this.IterationSetupService.GetShallow(transaction, partition, new[] { thing.Iid }, securityContext)
                                                           .OfType<IterationSetup>()
                                                           .SingleOrDefault();

            if (iterationSetup!=null)
            {
                iterationSetup.IsDeleted = true;
                this.IterationSetupService.UpdateConcept(transaction, partition, iterationSetup, container);

                // Swtitch partition to EngineeringModel
                var engineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(((EngineeringModelSetup)container).EngineeringModelIid);

                // Create revisions for deleted Iteration and updated EngineeringModel
                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
                var actor = credentials.Person.Iid;
                var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, engineeringModelPartition);
                this.RevisionService.SaveRevisions(transaction, engineeringModelPartition, actor, transactionRevision);
            }
            else 
            {
                throw new InvalidOperationException("IterationSetup is null.");
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