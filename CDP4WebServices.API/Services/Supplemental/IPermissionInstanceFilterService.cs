// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionInstanceFilterService.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Supplemental
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// The permission instance filter service.
    /// </summary>
    public interface IPermissionInstanceFilterService
    {
        /// <summary>
        /// Filter out permission depending on the supported version
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing"/> to filter</param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <returns>The filtered collection of <see cref="Thing"/></returns>
        IEnumerable<Thing> FilterOutPermissions(
            IReadOnlyCollection<Thing> things,
            Version requestDataModelVersion);
    }
}