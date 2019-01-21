// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using NLog;
    using Npgsql;

    /// <summary>
    /// The Publication Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class PublicationService
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Deletes all data from the current publication table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        public void DeleteAll(NpgsqlTransaction transaction, string partition)
        {
             this.PublicationDao.DeleteAll(transaction, partition);
        }
    }
}
