// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Resolve
{
    /// <summary>
    /// The resolve information helper class.
    /// </summary>
    public class ResolveInfo
    {
        /// <summary>
        /// Gets or sets the instance info of the item.
        /// </summary>
        public DtoInfo InstanceInfo { get; set; }

        /// <summary>
        /// Gets or sets the partition where the <see cref="CDP4Common.DTO.Thing"/> is stored.
        /// </summary>
        public string Partition { get; set; }
    }
}
