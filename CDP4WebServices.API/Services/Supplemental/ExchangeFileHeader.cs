// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileHeader.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;

    /// <summary>
    /// The ECSS-E-TM-10-25-AnnexC mandated exchange file header.
    /// This is to be included in the root of any exchange archive.
    /// </summary>
    public class ExchangeFileHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileHeader"/> class.
        /// </summary>
        public ExchangeFileHeader()
        {
            this.MediaType = "application/ecss-e-tm-10-25+json";
            this.ExchangeFileFormatVersion = "1.0.0";
            this.CreatedOn = new ExchangeDateTime(DateTime.Now);
        }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the data model version.
        /// </summary>
        public string DataModelVersion { get; set; }

        /// <summary>
        /// Gets or sets the exchange file format version.
        /// </summary>
        public string ExchangeFileFormatVersion { get; set; }

        /// <summary>
        /// Gets or sets the creator organization.
        /// </summary>
        public OrganizationInfo CreatorOrganization { get; set; }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Gets or sets the creator person.
        /// </summary>
        public ExchangeFileInitiator CreatorPerson { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        public ExchangeDateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last modified on.
        /// </summary>
        public ExchangeDateTime LastModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the extensions.
        /// </summary>
        public string Extensions { get; set; }
    }
}
