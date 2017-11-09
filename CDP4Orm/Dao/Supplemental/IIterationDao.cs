// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;

    using Npgsql;

    /// <summary>
    /// The Iteration Dao Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationDao
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
    }
}