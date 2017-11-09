// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;

    /// <summary>
    /// Holds the organization info that is included in an exchange header file.
    /// </summary>
    public class OrganizationInfo
    {
        /// <summary>
        /// Gets or sets an optional unique identifier of the organization.
        /// </summary>
        public Guid? Iid { get; set; }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional abbreviated name of a unit inside of the organization.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the optional geographic location of the organization.
        /// </summary>
        public string Site { get; set; }
    }
}
