// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerInfo.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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
