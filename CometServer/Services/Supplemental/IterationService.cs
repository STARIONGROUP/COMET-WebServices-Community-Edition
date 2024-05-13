// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
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
    using System.Diagnostics;
    using System.Linq;

    using Authorization;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Exceptions;

    using Helpers;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The Iteration Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class IterationService
    {
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
            Iteration iteration = null;

            if (this.activeIterationId != Guid.Empty)
            {
                // If there is a cached activeIterationId try to find that first.
                var activeIterations = this.GetShallow(transaction, partition, new [] { this.activeIterationId }, securityContext).OfType<Iteration>().ToArray();

                if (activeIterations.Length > 1)
                {
                    throw new ThingNotFoundException($"The active iteration-setup could not be found for partition {partition}.");
                }

                if (activeIterations.Length == 1)
                {
                    iteration =  activeIterations.Single();
                }
            }

            if (iteration == null)
            {
                // Might also be that the cached activeIterationId cannot be found anymoreF
                // For example as a result of copy EngineeringModelSetup based on an existing EngineeringModelSetup where the active Iteration.Iid is replaced at some point. The Iteration having the cached activeIterationId as its Iid will not be there anymore.
                // In this case we try to find the new active Iteration.Iid.
                var iterations = this.GetShallow(transaction, partition, null, securityContext).OfType<Iteration>().ToArray();

                var activeIterationSetups =
                    this.IterationSetupService
                        .GetShallow(transaction, Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, iterations.Select(x => x.IterationSetup), securityContext)
                        .OfType<IterationSetup>()
                        .Where(x => x.FrozenOn == null)
                        .ToArray();

                if (activeIterationSetups.Length != 1)
                {
                    throw new ThingNotFoundException($"The active iteration-setup could not be found for partition {partition}.");
                }

                var activeIterations = iterations.Where(x => x.Iid == activeIterationSetups.Single().IterationIid).ToArray();
                if (activeIterations.Length != 1)
                {
                    throw new ThingNotFoundException($"The active iteration could not be found for partition {partition}.");
                }

                var activeIteration = activeIterations.Single();
                this.activeIterationId = activeIteration.Iid;
                iteration = activeIteration;
            }

            return iteration;
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
            Logger.Info("End populate data for new iteration. Operation took {sw} ms", start.ElapsedMilliseconds);
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
            Logger.Info("Creating new iteration using the source iteration number {sourceIterationSetup}", sourceIterationSetup.IterationNumber);
            var start = Stopwatch.StartNew();
            var engineeringModelPartition = iterationPartition.Replace(Cdp4TransactionManager.ITERATION_PARTITION_PREFIX, Cdp4TransactionManager.ENGINEERING_MODEL_PARTITION_PREFIX);

            // Set end-validity for all current data
            this.IterationDao.SetIterationValidityEnd(transaction, iterationPartition);

            Logger.Info("Setting end validity took {start} ms", start.ElapsedMilliseconds);
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

            Logger.Info("Inserting data took {sw} ms", start.ElapsedMilliseconds);

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
    }
}
