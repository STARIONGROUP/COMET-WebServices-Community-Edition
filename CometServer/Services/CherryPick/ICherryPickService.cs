﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICherryPickService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer.Services.CherryPick
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The <see cref="ICherryPickService"/> provides capabilities on cherry picking <see cref="CDP4Common.DTO.Thing"/> inside an <see cref="Iteration"/>
    /// </summary>
    public interface ICherryPickService
    {
        /// <summary>
        /// Cherry pick <see cref="Thing" /> where the <see cref="ClassKind" /> is <paramref name="classKind" /> and where a filtering on
        /// <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <param name="categoriesId">A collection of <see cref="Category"/> id</param>
        /// <returns>A collection of retrieved <see cref="Thing" /></returns>
        IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, ClassKind classKind, IEnumerable<Guid> categoriesId);

        /// <summary>
        /// Cherry pick <see cref="Thing" /> where the <see cref="ClassKind" /> is one of the provided
        /// <paramref name="classKinds" /> and where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="classKinds">A collection of <see cref="ClassKind" /></param>
        /// <param name="categoriesId">A collection of <see cref="Category"/> id</param>
        /// <returns>A collection of retrieved <see cref="Thing" /></returns>
        IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, IEnumerable<ClassKind> classKinds, IEnumerable<Guid> categoriesId);
    }
}
