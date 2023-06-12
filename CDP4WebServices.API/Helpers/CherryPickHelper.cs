// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CherryPickHelper.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Extensions;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Helper for the cherry picking features
    /// </summary>
    public static class CherryPickHelper
    {
        /// <summary>
        /// Cherry pick <see cref="Thing"/> where the <see cref="ClassKind"/> is <paramref name="classKind"/> and where a filtering on <see cref="Category"/> can
        /// be applied based on provided <paramref name="categoriesShortName"/>
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing"/></param>
        /// <param name="classKind">The <see cref="ClassKind"/></param>
        /// <param name="categoriesShortName">The a collection of <see cref="Category"/>s shortname</param>
        /// <returns>A collection of retrieved <see cref="Thing"/></returns>
        public static IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, ClassKind classKind, IEnumerable<string> categoriesShortName)
        {
            var cherryPickedThings = new List<Thing>();

            var categories = things.OfType<Category>().Where(x => categoriesShortName.Any(c => x.ShortName == c)).ToList();
            cherryPickedThings.AddRange(categories);
            var categoriesId = categories.Select(x => x.Iid).ToList();

            switch (classKind)
            {
                case ClassKind.ParameterOrOverrideBase:
                    cherryPickedThings.AddRange(CherryPickParameterOrOverrideBase(things, categoriesId));
                    break;
                case ClassKind.ElementBase:
                    cherryPickedThings.AddRange(CherryPickElementBase(things, categoriesId));
                    break;
                default:
                    cherryPickedThings.AddRange(CherryPickCategorizableThings(things, categoriesId, classKind));
                    break;
            }

            return cherryPickedThings;
        }

        /// <summary>
        /// Cherry pick <see cref="Thing"/> where the <see cref="ClassKind"/> is one of the provided <paramref name="classKinds"/> and where a filtering on <see cref="Category"/> can
        /// be applied based on provided <paramref name="categoriesShortName"/>
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing"/></param>
        /// <param name="classKinds">A collection of <see cref="ClassKind"/></param>
        /// <param name="categoriesShortName">The a collection of <see cref="Category"/>s shortname</param>
        /// <returns>A collection of retrieved <see cref="Thing"/></returns>
        public static IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, IEnumerable<ClassKind> classKinds, IEnumerable<string> categoriesShortName)
        {
            var cherryPickedThings = new List<Thing>();
            categoriesShortName = categoriesShortName.ToList();

            foreach (var classKind in classKinds)
            {
                cherryPickedThings.AddRange(CherryPick(things, classKind, categoriesShortName));
            }
           
            return cherryPickedThings.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Cherry pick <see cref="ParameterOrOverrideBase"/>s where a filtering on <see cref="Category"/> can
        /// be applied based on provided <paramref name="categoriesId"/>
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing"/></param>
        /// <param name="categoriesId">The a collection of <see cref="Category"/>s id</param>
        /// <returns>A collection of retrieved <see cref="ParameterOrOverrideBase"/></returns>
        private static IEnumerable<Thing> CherryPickParameterOrOverrideBase(IReadOnlyList<Thing> things, IReadOnlyList<Guid> categoriesId)
        {
            var categorizedParameterTypes = things.OfType<ParameterType>().Where(x => x.Category.Any(categoriesId.Contains))
                .Select(x => x.Iid);

            var parameterOrOverrideBase = things.OfType<ParameterOrOverrideBase>().Where(x => categorizedParameterTypes.Contains(x.ParameterType));
            return parameterOrOverrideBase;
        }

        /// <summary>
        /// Cherry pick <see cref="ElementBase"/>s where a filtering on <see cref="Category"/> can
        /// be applied based on provided <paramref name="categoriesId"/>
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing"/></param>
        /// <param name="categoriesId">The a collection of <see cref="Category"/>s id</param>
        /// <returns>A collection of retrieved <see cref="ElementBase"/></returns>
        private static IEnumerable<Thing> CherryPickElementBase(IReadOnlyList<Thing> things, IReadOnlyList<Guid> categoriesId)
        {
            var elementBases = new List<Thing>();
            elementBases.AddRange(things.OfType<ElementBase>().Where(x => x.Category.Any(categoriesId.Contains)));

            foreach (var elementUsage in things.OfType<ElementUsage>())
            {
                var elementDefinition = (ElementDefinition)things.First(x => x.Iid == elementUsage.ElementDefinition);

                if (elementDefinition.Category.Any(categoriesId.Contains) && !elementBases.Contains(elementUsage))
                {
                    elementBases.Add(elementUsage);
                }
            }

            return elementBases;
        }

        /// <summary>
        /// Cherry pick <see cref="ICategorizableThing"/>s  where the <see cref="ClassKind"/> is <paramref name="classKind"/> and where a filtering on <see cref="Category"/> can
        /// be applied based on provided <paramref name="categoriesId"/>
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing"/></param>
        /// <param name="categoriesId">The a collection of <see cref="Category"/>s id</param>
        /// <param name="classKind">The <see cref="ClassKind"/></param>
        /// <returns>A collection of retrieved <see cref="ICategorizableThing"/></returns>
        private static IEnumerable<Thing> CherryPickCategorizableThings(IEnumerable<Thing> things, IReadOnlyList<Guid> categoriesId, ClassKind classKind)
        {
            return things.Where(x => x.ClassKind == classKind).OfType<ICategorizableThing>().Where(x => x.Category.Any(categoriesId.Contains))
                .Cast<Thing>();
        }
    }
}
