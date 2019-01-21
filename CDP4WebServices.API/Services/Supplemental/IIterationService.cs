// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using Authorization;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The Iteration Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationService
    {
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
        void CopyIteration(NpgsqlTransaction transaction, string sourcePartition, string targetPartition);

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
        void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable);

        /// <summary>
        /// Gets the active <see cref="Iteration"/> for the given <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <returns>The active <see cref="Iteration"/></returns>
        Iteration GetActiveIteration(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Populates the data of a new iteration using the last iteration
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        void PopulateDataFromLastIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup, EngineeringModel engineeringModel, ISecurityContext securityContext);

        /// <summary>
        /// Populates the data of a new iteration using a specific source <paramref name="sourceIterationSetup"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        void PopulateDataFromOlderIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup,
            EngineeringModel engineeringModel, ISecurityContext securityContext);

    }
}