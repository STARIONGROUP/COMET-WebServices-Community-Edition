// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting;
    using Authorization;
    using CDP4Common.DTO;
    using Helpers;
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
    }
}
