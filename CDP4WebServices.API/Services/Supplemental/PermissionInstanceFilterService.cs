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

    using CDP4Common.DTO;

    using NLog;

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
        /// Filter out permission depending on the supported version
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing"/> to filter</param>
        /// <param name="requestUtils">The <see cref="IRequestUtils"/></param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <returns>The filtered collection of <see cref="Thing"/></returns>
        public IEnumerable<Thing> FilterOutPermissions(
            IReadOnlyCollection<Thing> things,
            IRequestUtils requestUtils,
            Version requestDataModelVersion)
        {
            var timer = Stopwatch.StartNew();

            var personRoles = things.OfType<PersonRole>().ToArray();
            var participantRoles = things.OfType<ParticipantRole>().ToArray();
            var personPermissions = things.OfType<PersonPermission>().ToArray();
            var participantPermissions = things.OfType<ParticipantPermission>().ToArray();

            var excludedPersonPermission = new List<PersonPermission>();
            var excludedParticipantPermission = new List<ParticipantPermission>();

            foreach (var personPermission in personPermissions)
            {
                var metainfo = requestUtils.MetaInfoProvider.GetMetaInfo(personPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion)
                    || requestDataModelVersion >= new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedPersonPermission.Add(personPermission);
            }

            foreach (var participantPermission in participantPermissions)
            {
                var metainfo = requestUtils.MetaInfoProvider.GetMetaInfo(participantPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion)
                    || requestDataModelVersion >= new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedParticipantPermission.Add(participantPermission);
            }

            foreach (var personRole in personRoles)
            {
                personRole.PersonPermission.RemoveAll(x => excludedPersonPermission.Select(pp => pp.Iid).Contains(x));
            }

            foreach (var participantRole in participantRoles)
            {
                participantRole.ParticipantPermission.RemoveAll(
                    x => excludedParticipantPermission.Select(pp => pp.Iid).Contains(x));
            }

            foreach (var thing in things.Except(excludedParticipantPermission).Except(excludedPersonPermission))
            {
                yield return thing;
            }

            Logger.Info(string.Format("permission filter operation took {0} ms", timer.ElapsedMilliseconds));
        }
    }
}