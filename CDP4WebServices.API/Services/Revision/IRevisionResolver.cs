// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionResolver.cs" company="RHEA System S.A.">
//   Copyright (c) 2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Orm.Dao.Revision;

    using CDP4WebServices.API.Services.Protocol;

    using Npgsql;

    /// <summary>
    /// From the webservices' perspective, the <see cref="IQueryParameters"/> class can contain <see cref="int"/>, or <see cref="DateTime"/> type values for the revisionFrom, and revisionTo query parameters.
    /// For the <see cref="DateTime"/> type a call has to be made to the datastore, to get the closest revision (before, or after) the specified DateTime value.
    /// </summary>
    public interface IRevisionResolver
    {
        /// <summary>
        /// Try to resolve the revision numbers that belong to the revisionFrom and revisionTo parameters.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revisionFrom"><see cref="int"/> or <see cref="DateTime"/> type parameter that indicates the From revision number, or timestamp.</param>
        /// <param name="revisionTo"><see cref="int"/> or <see cref="DateTime"/> type parameter that indicates the To revision number, or timestamp.</param>
        /// <param name="resolvedValues">A <see cref="ValueTuple"/> containing the resolved From revision number and To revision number.</param>
        /// <returns></returns>
        bool TryResolve(NpgsqlTransaction transaction, string partition, object revisionFrom, object revisionTo, out (int FromRevision, int ToRevision, IEnumerable<RevisionRegistryInfo> RevisionRegistryInfoList) resolvedValues);
    }
}
