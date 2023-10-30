// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataModelUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System.Collections.Generic;

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
        /// A collection of possible partitions as string
        /// </remarks>
        IList<string> GetSourcePartition(string typeName);
    }
}
