// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionPropertyFilterService.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Supplemental
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// Permission property filter service that is used to filter dtos personPermission and participantPermission properties.
    /// </summary>
    public interface IPermissionPropertyFilterService
    {
        /// <summary>
        /// Filters personPermission property of a personRole dto.
        /// </summary>
        /// <param name="personRoles">
        /// Person role dtos.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        void FilterPersonPermissionProperty(IEnumerable<PersonRole> personRoles, Version requestDataModelVersion);

        /// <summary>
        /// Filters participantPermission property of a participantRole dto.
        /// </summary>
        /// <param name="participantRoles">
        /// Participant role dtos.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        void FilterParticipantPermissionProperty(IEnumerable<ParticipantRole> participantRoles, Version requestDataModelVersion);
    }
}