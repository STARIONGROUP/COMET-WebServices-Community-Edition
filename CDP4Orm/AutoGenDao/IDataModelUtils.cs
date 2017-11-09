// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataModelUtils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    /// <summary>
    /// The Data model utils interface.
    /// </summary>
    public interface IDataModelUtils
    {
        /// <summary>
        /// Check if the property for the given className is derived.
        /// </summary>
        /// <param name="className">
        /// The class name.
        /// </param>
        /// <param name="property">
        /// The property to check.
        /// </param>
        /// <returns>
        /// True if property is derived.
        /// </returns>
        bool IsDerived(string className, string property);
 
        /// <summary>
        /// Get the source partition for a passed in concrete type.
        /// </summary>
        /// <param name="typeName">
        /// The concrete type name.
        /// </param>
        /// <returns>
        /// The partition name for the passed in concrete type, otherwise null
        /// </returns>
        /// <remarks>
        /// Null is returned if there is no type map entry or there are multiple
        /// </remarks>
        string GetSourcePartition(string typeName);
    }
}
