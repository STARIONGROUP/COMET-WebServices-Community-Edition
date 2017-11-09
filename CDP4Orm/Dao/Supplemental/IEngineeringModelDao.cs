// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEngineeringModelDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CDP4Orm.Dao
{
    using System;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The EngineeringModel Dao Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IEngineeringModelDao
    {
        /// <summary>
        /// The get next iteration number.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws if an iterationNumber could not be retrieved
        /// </exception>
        int GetNextIterationNumber(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// The reset iteration number sequence start number.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="iterationStartNumber">
        /// The start number.
        /// </param>
        void ResetIterationNumberSequenceStartNumber(NpgsqlTransaction transaction, string partition, int iterationStartNumber);

        /// <summary>
        /// Copy the tables from a source to a target Engineering-Model partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model
        /// </param>
        /// <param name="targetPartition">
        /// The target Engineering-Model
        /// </param>
        void CopyEngineeringModel(NpgsqlTransaction transaction, string sourcePartition, string targetPartition);

        /// <summary>
        /// Modify the identifier of all records in a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The egineering-model partition to modify</param>
        void ModifyIdentifier(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Modify the identifier of the <paramref name="thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="thing">The updated <see cref="Thing"/></param>
        /// <param name="oldIid">The old identifier</param>
        void ModifyIdentifier(NpgsqlTransaction transaction, string partition, Thing thing, Guid oldIid);

        /// <summary>
        /// Copy the tables from a source to an EngineeringModel partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source engineering-model partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable);
    }
}