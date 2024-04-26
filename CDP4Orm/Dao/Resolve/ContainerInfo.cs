// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerInfo.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
