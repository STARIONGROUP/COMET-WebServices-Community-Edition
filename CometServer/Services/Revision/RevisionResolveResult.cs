// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionResolveResult.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System.Collections.Generic;

    using CDP4Orm.Dao.Revision;

    /// <summary>
    /// The holds the result of a call to the <see cref="IRevisionResolver.TryResolve"/> method.
    /// </summary>
    public readonly struct RevisionResolveResult(int fromRevision, int toRevision, IEnumerable<RevisionRegistryInfo> revisionRegistryInfoList)
    {
        /// <summary>
        /// Gets the From revision number.
        /// </summary>
        public int FromRevision { get; } = fromRevision;

        /// <summary>
        /// Gets the To revision number.
        /// </summary>
        public int ToRevision { get; } = toRevision;

        /// <summary>
        /// Gets the list of <see cref="RevisionRegistryInfo"/> objects that contain information about the revisions in the range from FromRevision to ToRevision.
        /// </summary>
        public IEnumerable<RevisionRegistryInfo> RevisionRegistryInfoList { get; } = revisionRegistryInfoList;
    }
}
