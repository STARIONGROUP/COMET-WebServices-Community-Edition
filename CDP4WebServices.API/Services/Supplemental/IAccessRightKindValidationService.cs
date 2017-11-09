// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccessRightKindValidationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;

    using CDP4Common.DTO;

    /// <summary>
    /// The AccessRightKindValidation Service Interface which validates permissions for things.
    /// </summary>
    public interface IAccessRightKindValidationService
    {
        /// <summary>
        /// Check whether person permission valid.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check access right kind of.</param>
        /// <returns>
        /// The <see cref="bool"/> whether the set access right is valid.
        /// </returns>
        /// <exception cref="InvalidCastException"> If the supplied thing is not an instance of <see cref="PersonPermission"/>
        /// </exception>
        bool IsPersonPermissionValid(Thing thing);

        /// <summary>
        /// Check whether participant permission valid.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to check access right kind of.</param>
        /// <returns>
        /// The <see cref="bool"/> whether the set access right is valid.
        /// </returns>
        /// <exception cref="InvalidCastException"> If the supplied thing is not an instance of <see cref="ParticipantPermission"/>
        /// </exception>
        bool IsParticipantPermissionValid(Thing thing);
    }
}