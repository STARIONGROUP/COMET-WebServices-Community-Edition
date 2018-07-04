// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionInstanceFilterService.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;
    using CDP4Orm.Dao;
    using Helpers;
    using NLog;
    using Npgsql;
    using Protocol;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The permission instance filter service.
    /// </summary>
    public class PermissionInstanceFilterService : IPermissionInstanceFilterService
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The site directory data.
        /// </summary>
        protected const string SiteDirectoryData = "SiteDirectory";

        /// <summary>
        /// Gets or sets the <see cref="IMetaInfoProvider"/>
        /// </summary>
        public IMetaInfoProvider MetadataProvider { get; set; }

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
        /// Filter out permission depending on the supported version
        /// </summary>
        /// <param name="things">The collection of <see cref="CDP4Common.DTO.Thing"/> to filter</param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <returns>The filtered collection of <see cref="CDP4Common.DTO.Thing"/></returns>
        public IEnumerable<Thing> FilterOutPermissions(IReadOnlyCollection<Thing> things, Version requestDataModelVersion)
        {
            var sw = Stopwatch.StartNew();

            var personRoles = things.OfType<PersonRole>().ToArray();
            var participantRoles = things.OfType<ParticipantRole>().ToArray();
            var personPermissions = things.OfType<PersonPermission>().ToArray();
            var participantPermissions = things.OfType<ParticipantPermission>().ToArray();

            // do not do anything if no role/permission in things
            if (personRoles.Length == 0 && participantRoles.Length == 0 && personPermissions.Length == 0 && participantPermissions.Length == 0)
            {
                Logger.Info("Filtering-out permissions took {0} ms (no filtering required)", sw.ElapsedMilliseconds);
                return things;
            }

            IReadOnlyList<Guid> excludedPersonPermission = new List<Guid>();
            IReadOnlyList<Guid> excludedParticipantPermission = new List<Guid>();
            if (personRoles.Length != 0 || personPermissions.Length != 0)
            {
                excludedPersonPermission = this.GetIgnoredPersonPermissionIds(requestDataModelVersion, personPermissions, personRoles);
                if (excludedPersonPermission.Count > 0)
                {
                    foreach (var personRole in personRoles)
                    {
                        personRole.PersonPermission.RemoveAll(x => excludedPersonPermission.Contains(x));
                    }
                }
            }

            if (participantRoles.Length != 0 || participantPermissions.Length != 0)
            {
                excludedParticipantPermission = this.GetIgnoredParticipantPermissionIds(requestDataModelVersion, participantPermissions, participantRoles);
                if (excludedParticipantPermission.Count > 0)
                {
                    foreach (var participantRole in participantRoles)
                    {
                        participantRole.ParticipantPermission.RemoveAll(x => excludedParticipantPermission.Contains(x));
                    }
                }
            }

            // filter-out permissions
            Logger.Info("Filtering-out permissions took {0} ms", sw.ElapsedMilliseconds);

            return things.Where(
                x => (x.ClassKind != ClassKind.PersonPermission && x.ClassKind != ClassKind.ParticipantPermission) ||
                     (!excludedParticipantPermission.Contains(x.Iid) && !excludedPersonPermission.Contains(x.Iid)));
        }

        /// <summary>
        /// Gets the ignored <see cref="PersonPermission"/>
        /// </summary>
        /// <param name="requestDataModelVersion">The requested version</param>
        /// <param name="inPersonPermissions">The source <see cref="PersonPermission"/></param>
        /// <param name="inPersonRoles">The source <see cref="PersonRole"/></param>
        /// <returns>The ignored identifier</returns>
        private IReadOnlyList<Guid> GetIgnoredPersonPermissionIds(Version requestDataModelVersion, IReadOnlyList<PersonPermission> inPersonPermissions, IReadOnlyList<PersonRole> inPersonRoles)
        {
            var excludedPersonPermission = new List<Guid>();

            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            // get prepared data source transaction
            var queryPersonPermissions = inPersonRoles.SelectMany(x => x.PersonPermission).Except(inPersonPermissions.Select(x => x.Iid)).ToArray();

            transaction = this.TransactionManager.SetupTransaction(ref connection, null);

            // if all permissions are in then dont do extra db query
            var personPermissions = queryPersonPermissions.Length > 0 
                ? this.PersonPermissionDao.Read(transaction, SiteDirectoryData, queryPersonPermissions, true).Union(inPersonPermissions)
                : inPersonPermissions;

            foreach (var personPermission in personPermissions)
            {
                var metainfo = this.MetadataProvider.GetMetaInfo(personPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion) || requestDataModelVersion >= new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedPersonPermission.Add(personPermission.Iid);
            }

            return excludedPersonPermission;
        }

        /// <summary>
        /// Gets the ignored <see cref="ParticipantPermission"/>
        /// </summary>
        /// <param name="requestDataModelVersion">The requested version</param>
        /// <param name="inParticipantPermissions">The source <see cref="PersonPermission"/></param>
        /// <param name="inParticipantRoles">The source <see cref="PersonRole"/></param>
        /// <returns>The ignored identifier</returns>
        private IReadOnlyList<Guid> GetIgnoredParticipantPermissionIds(Version requestDataModelVersion, IReadOnlyList<ParticipantPermission> inParticipantPermissions, IReadOnlyList<ParticipantRole> inParticipantRoles)
        {
            var excludedParticipantPermission = new List<Guid>();
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            // get prepared data source transaction
            transaction = this.TransactionManager.SetupTransaction(ref connection, null);

            var queryPersonPermissions = inParticipantRoles.SelectMany(x => x.ParticipantPermission).Except(inParticipantPermissions.Select(x => x.Iid)).ToArray();
            var participantPermissions = queryPersonPermissions.Length > 0
                ? this.ParticipantPermissionDao.Read(transaction, SiteDirectoryData, null, true).Union(inParticipantPermissions)
                : inParticipantPermissions;

            foreach (var participantPermission in participantPermissions)
            {
                var metainfo = this.MetadataProvider.GetMetaInfo(participantPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion) || requestDataModelVersion >= new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedParticipantPermission.Add(participantPermission.Iid);
            }

            return excludedParticipantPermission;
        }
    }
}