// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContributorLocationResolver.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.ContributorsLocation
{
    /// <summary>
    /// The ContributorLocationResolver interface.
    /// </summary>
    public interface IContributorLocationResolver
    {
        /// <summary>
        /// Gets location data of a contributor and saves it into cache.
        /// </summary>
        void GetLocationDataAndSave();
    }
}
