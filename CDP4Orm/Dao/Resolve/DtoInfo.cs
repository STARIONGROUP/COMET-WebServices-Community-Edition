// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoInfo.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Resolve
{
    using System;

    /// <summary>
    /// The <see cref="DtoInfo"/> class that acts as a tuple placeholder.
    /// </summary>
    public class DtoInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DtoInfo"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type Name of the DTO instance.
        /// </param>
        /// <param name="iid">
        /// The id of the DTO instance.
        /// </param>
        public DtoInfo(string typeName, Guid iid)
        {
            this.TypeName = typeName;
            this.Iid = iid;
        }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public Guid Iid { get; private set; }

        /// <summary>
        /// Override the equals in support of this type.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DtoInfo);
        }

        /// <summary>
        /// The override of the hash code to allow this type to be used in hashed collections.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + this.TypeName.GetHashCode();
            hash = (hash * 7) + this.Iid.GetHashCode();

            return hash;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(DtoInfo obj)
        {
            return obj != null && obj.TypeName == this.TypeName && obj.Iid == this.Iid;
        }
    }
}
