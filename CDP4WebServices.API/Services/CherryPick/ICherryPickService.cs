// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICherryPickService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.CherryPick
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
