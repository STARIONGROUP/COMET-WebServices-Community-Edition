// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingRevisionInfo.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using CDP4Common.DTO;

    using CDP4Orm.Dao.Revision;

    /// <summary>
    /// A class that concatenate a <see cref="Thing"/> with a <see cref="RevisionInfo"/>
    /// </summary>
    internal class ThingRevisionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingRevisionInfo"/> class
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="revisionInfo">The associated <see cref="RevisionInfo"/></param>
        public ThingRevisionInfo(Thing thing, RevisionInfo revisionInfo)
        {
            this.Thing = thing;
            this.RevisionInfo = revisionInfo;
        }

        /// <summary>
        /// Gets or sets the <see cref="Thing"/>
        /// </summary>
        public Thing Thing { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="RevisionInfo"/>
        /// </summary>
        public RevisionInfo RevisionInfo { get; private set; }
    }
}