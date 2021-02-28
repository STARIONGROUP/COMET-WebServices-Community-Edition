// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoResolveHelper.cs" company="RHEA System S.A.">
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
    using CDP4Common.DTO;

    /// <summary>
    /// Helper class that holds resolved information for a DTO instance
    /// </summary>
    public class DtoResolveHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DtoResolveHelper"/> class.
        /// </summary>
        /// <param name="instanceInfo">
        /// The instance Info tuple that holds the type name and identifier.
        /// </param>
        public DtoResolveHelper(DtoInfo instanceInfo)
        {
            this.InstanceInfo = instanceInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DtoResolveHelper"/> class.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="Thing"/> instance.
        /// </param>
        public DtoResolveHelper(Thing instance)
        {
            this.InstanceInfo = instance.GetInfoPlaceholder();
            this.Thing = instance;
        }

        /// <summary>
        /// Gets a value indicating whether the partition is resolved.
        /// </summary>
        public bool IsPartitionResolved
        {
            get
            {
                return this.Partition != null;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the partition is resolved.
        /// </summary>
        public bool IsInstanceResolved
        {
            get
            {
                return this.Thing != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the info was resolved.
        /// </summary>
        public bool IsResolved
        {
            get
            {
                return this.IsPartitionResolved && this.IsInstanceResolved;
            }
        }

        /// <summary>
        /// Gets or sets the partition for this instance.
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// Gets the instance information.
        /// </summary>
        public DtoInfo InstanceInfo { get; private set; }

        /// <summary>
        /// Gets or sets the container reference info.
        /// </summary>
        public ContainerInfo ContainerInfo { get; set; }

        /// <summary>
        /// Gets or sets the resolved <see cref="Thing"/>.
        /// </summary>
        public Thing Thing { get; set; }
    }
}
