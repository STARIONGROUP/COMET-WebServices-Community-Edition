// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveElementDefinitionTreeResult.cs" company="Starion Group S.A.">
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

namespace CometServer.Authorization
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// Represents the result of resolving an element definition tree.
    /// </summary>
    public class ResolveElementDefinitionTreeResult(List<ElementDefinition> elementDefinitions, List<ElementDefinition> relevantOpenDefinitions, IEnumerable<Thing> fullTree)
    {
        /// <summary>
        /// Gets a collection of ElementDefinitions that are relevant to the resolved tree.
        /// </summary>
        public List<ElementDefinition> ElementDefinitions { get; } = elementDefinitions;

        /// <summary>
        /// Gets a collection of ElementDefinitions that are relevant to the open definitions in the tree.
        /// </summary>
        public List<ElementDefinition> RelevantOpenDefinitions { get; } = relevantOpenDefinitions;

        /// <summary>
        /// Gets a collection of all <see cref="Thing"/>s in the tree.
        /// </summary>
        public IEnumerable<Thing> FullTree { get; } = fullTree;
    }
}
