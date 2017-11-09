// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeDateTime.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;

    /// <summary>
    /// Exchange file date time format that holds the local and universal time.
    /// </summary>
    public class ExchangeDateTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDateTime"/> class.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        public ExchangeDateTime(DateTime dateTime)
        {
            this.Local = dateTime.ToLocalTime();
            this.Utc = dateTime.ToUniversalTime();
        }

        /// <summary>
        /// Gets or sets the local date time.
        /// </summary>
        public DateTime Local { get; set; }

        /// <summary>
        /// Gets or sets the universal date time.
        /// </summary>
        public DateTime Utc { get; set; }
    }
}
