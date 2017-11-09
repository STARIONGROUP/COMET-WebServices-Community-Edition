// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryParameters.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Protocol
{
    using System;

    /// <summary>
    /// The Query Parameters interface.
    /// </summary>
    public interface IQueryParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to extent deep.
        /// </summary>
        bool ExtentDeep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include reference data.
        /// </summary>
        bool IncludeReferenceData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include all containers.
        /// </summary>
        bool IncludeAllContainers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include file data.
        /// </summary>
        bool IncludeFileData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to export data.
        /// </summary>
        bool Export { get; set; }

        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision number from which the request is done
        /// </summary>
        int? RevisionFrom { get; set; }

        /// <summary>
        /// Gets or sets the revision number to which the request is done
        /// </summary>
        int? RevisionTo { get; set; }

        /// <summary>
        /// The validate query parameter.
        /// </summary>
        /// <param name="queryParameter">
        /// The query parameter.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="Exception">
        /// If unknown query parameter or value is passed
        /// </exception>
        void ValidateQueryParameter(string queryParameter, string value);
    }
}
