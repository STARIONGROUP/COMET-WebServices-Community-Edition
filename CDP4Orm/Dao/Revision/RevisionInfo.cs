// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Revision
{
    using System;

    /// <summary>
    /// The revision thing.
    /// </summary>
    public class RevisionInfo
    {
        /// <summary>
        /// Gets or sets the type info.
        /// </summary>
        public string TypeInfo { get; set; }

        /// <summary>
        /// Gets or sets the id of the thing.
        /// </summary>
        public Guid Iid { get; set; }

        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        public string Partition { get; set; }
    }
}
