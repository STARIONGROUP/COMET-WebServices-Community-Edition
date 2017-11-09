// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileInitiator.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;

    /// <summary>
    /// The initiator (person) of an exchange file export.
    /// This is included in the <see cref="ExchangeFileHeader"/>
    /// </summary>
    public class ExchangeFileInitiator
    {
        /// <summary>
        /// Gets or sets an optional unique identifier of the person who initiated the export.
        /// </summary>
        public Guid? Iid { get; set; }

        /// <summary>
        /// Gets or sets the given name of the person who initiated the export.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the surname of the person who initiated the export.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the optional email address of the person who initiated the export.
        /// </summary>
        public string Email { get; set; }
    }
}
