// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionRegistryInfo.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
