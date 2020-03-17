// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionRegistryInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Revision
{
    using System;

    /// <summary>
    /// Holds information about a single RevisionRegistry Entry
    /// </summary>
    public class RevisionRegistryInfo
    {
        /// <summary>
        /// Gets or sets the revision number
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the datetime on which the revision was created
        /// </summary>
        public DateTime Instant { get; set; }

        /// <summary>
        /// Gets or sets the id of the person that created the revision
        /// </summary>
        public Guid Actor { get; set; }
    }
}
