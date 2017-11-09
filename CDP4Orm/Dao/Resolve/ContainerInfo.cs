// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Resolve
{
    using System;

    /// <summary>
    /// The container information helper class.
    /// </summary>
    public class ContainerInfo : DtoInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInfo"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type Name of the DTO instance.
        /// </param>
        /// <param name="iid">
        /// The id of the DTO instance.
        /// </param>
        /// <param name="sequence">
        /// An optional containment order sequence.
        /// </param>
        public ContainerInfo(string typeName, Guid iid, long sequence = -1)
            : base(typeName, iid)
        {
            this.ContainmentSequence = sequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInfo"/> class.
        /// </summary>
        /// <remarks>
        /// this will initialize a to-be resolved placeholder.
        /// </remarks>
        public ContainerInfo()
            : base(null, Guid.Empty)
        {
        }

        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// Gets the containment sequence.
        /// </summary>
        public long ContainmentSequence { get; private set; }
    }
}
