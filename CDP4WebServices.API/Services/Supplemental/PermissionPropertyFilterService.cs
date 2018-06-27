// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionPropertyFilterService.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Protocol;

    using NLog;

    using Npgsql;

    /// <summary>
    /// Permission property filter service that is used to filter dtos personPermission and participantPermission properties.
    /// </summary>
    public class PermissionPropertyFilterService : IPermissionPropertyFilterService
    {
        /// <summary>
        /// The site directory data.
        /// </summary>
        protected const string SiteDirectoryData = "SiteDirectory";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the Utils instance for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager for this request.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the participantPermission dao.
        /// </summary>
        public IParticipantPermissionDao ParticipantPermissionDao { get; set; }

        /// <summary>
        /// Gets or sets the participantPermission dao.
        /// </summary>
        public IPersonPermissionDao PersonPermissionDao { get; set; }

        /// <summary>
        /// Filters personPermission property of a personRole dto.
        /// </summary>
        /// <param name="personRoles">
        /// Person role dtos.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        public void FilterPersonPermissionProperty(IEnumerable<PersonRole> personRoles, Version requestDataModelVersion)
        {
            var personRolesArray = personRoles?.ToArray();
            if (personRolesArray != null && personRolesArray.Any())
            {
                NpgsqlConnection connection = null;
                NpgsqlTransaction transaction = null;
                this.RequestUtils.QueryParameters = new QueryParameters();

                try
                {
                    // get prepared data source transaction
                    transaction = this.TransactionManager.SetupTransaction(ref connection, null);

                    var personPermissions = this.PersonPermissionDao.Read(transaction, SiteDirectoryData, personRolesArray.SelectMany(x => x.PersonPermission).ToArray(), true);

                    // filter to have only prohibited ids
                    var prohibitedPersonPermissionIds = personPermissions.Where(
                        x =>
                            {
                                var metainfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(x.ObjectClass.ToString());
                                if (string.IsNullOrEmpty(metainfo.ClassVersion)
                                    || requestDataModelVersion >= new Version(metainfo.ClassVersion))
                                {
                                    return false;
                                }

                                return true;
                            }).Select(x => x.Iid).ToArray();

                    foreach (var personRole in personRolesArray)
                    {
                        personRole.PersonPermission.RemoveAll(x => prohibitedPersonPermissionIds.Contains(x));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed on person permission property filtering.");
                    throw new Exception("Something went wrong.");
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }

                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Filters participantPermission property of a participantRole dto.
        /// </summary>
        /// <param name="participantRoles">
        /// Participant role dtos.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        public void FilterParticipantPermissionProperty(IEnumerable<ParticipantRole> participantRoles, Version requestDataModelVersion)
        {
            var participantRolesArray = participantRoles?.ToArray();
            if (participantRolesArray != null && participantRolesArray.Any())
            {
                NpgsqlConnection connection = null;
                NpgsqlTransaction transaction = null;
                this.RequestUtils.QueryParameters = new QueryParameters();

                try
                {
                    // get prepared data source transaction
                    transaction = this.TransactionManager.SetupTransaction(ref connection, null);

                    var participantPermissions = this.ParticipantPermissionDao.Read(
                        transaction,
                        SiteDirectoryData,
                        participantRolesArray.SelectMany(x => x.ParticipantPermission).ToArray(),
                        true);

                    // filter to have only prohibited ids
                    var prohibitedparticipantPermissionIds = 
                        participantPermissions
                            .Where(
                                x =>
                                    {
                                        var metainfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(x.ObjectClass.ToString());
                                        return !string.IsNullOrEmpty(metainfo.ClassVersion) && requestDataModelVersion < new Version(metainfo.ClassVersion);
                                    })
                            .Select(x => x.Iid)
                            .ToArray();

                    foreach (var participantRole in participantRolesArray)
                    {
                        participantRole.ParticipantPermission.RemoveAll(x => prohibitedparticipantPermissionIds.Contains(x));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed on participant permission property filtering.");
                    throw new Exception("Something went wrong.");
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }

                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }
    }
}