// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The EngineeringModel Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class EngineeringModelService
    {
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
        public void CopyEngineeringModel(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            this.EngineeringModelDao.CopyEngineeringModel(transaction, sourcePartition, targetPartition);
        }

        /// <summary>
        /// Modify the identifier of all records in a partition
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The egineering-model partition to modify</param>
        public void ModifyIdentifier(NpgsqlTransaction transaction, string partition)
        {
            this.EngineeringModelDao.ModifyIdentifier(transaction, partition);
        }

        /// <summary>
        /// Copy the tables from a source to an EngineeringModel partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source EngineeringModel partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        public void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            this.EngineeringModelDao.ModifyUserTrigger(transaction, sourcePartition, enable);
        }

        /// <summary>
        /// Modify the identifier of the <paramref name="thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="thing">The updated <see cref="Thing"/></param>
        /// <param name="oldIid">The old identifier</param>
        public void ModifyIdentifier(NpgsqlTransaction transaction, string partition, Thing thing, Guid oldIid)
        {
            this.EngineeringModelDao.ModifyIdentifier(transaction, partition, thing, oldIid);
        }
    }
}
