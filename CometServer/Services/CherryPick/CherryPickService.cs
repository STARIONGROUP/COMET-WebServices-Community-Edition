// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CherryPickService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.CherryPick
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The <see cref="CherryPickService" /> provides capabilities on cherry picking <see cref="CDP4Common.DTO.Thing" /> inside an
    /// <see cref="Iteration" />
    /// </summary>
    public class CherryPickService : ICherryPickService
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
        public IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, ClassKind classKind, IEnumerable<Guid> categoriesId)
        {
            var cherryPickedThings = new List<Thing>();

            var categories = things.OfType<Category>().Where(x => categoriesId.Contains(x.Iid)).ToList();
            var existingCategories = categories.Select(x => x.Iid).ToList();

            switch (classKind)
            {
                case ClassKind.ParameterOverride:
                    cherryPickedThings.AddRange(CherryPickParameterOverrides(things, existingCategories));
                    break;
                case ClassKind.Parameter:
                    cherryPickedThings.AddRange(CherryPickParameters(things, existingCategories));
                    break;
                case ClassKind.ElementUsage:
                    cherryPickedThings.AddRange(CherryPickElementUsages(things, existingCategories));
                    break;
                default:
                    cherryPickedThings.AddRange(CherryPickCategorizableThings(things, existingCategories, classKind));
                    break;
            }

            return cherryPickedThings;
        }

        /// <summary>
        /// Cherry pick <see cref="Thing" /> where the <see cref="ClassKind" /> is one of the provided
        /// <paramref name="classKinds" /> and where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="classKinds">A collection of <see cref="ClassKind" /></param>
        /// <param name="categoriesId">A collection of <see cref="Category"/> id</param>
        /// <returns>A collection of retrieved <see cref="Thing" /></returns>
        public IEnumerable<Thing> CherryPick(IReadOnlyList<Thing> things, IEnumerable<ClassKind> classKinds, IEnumerable<Guid> categoriesId)
        {
            var cherryPickedThings = new List<Thing>();
            categoriesId = categoriesId.ToList();

            foreach (var classKind in classKinds)
            {
                cherryPickedThings.AddRange(this.CherryPick(things, classKind, categoriesId));
            }

            return cherryPickedThings.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Cherry pick <see cref="Parameter" />s where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="categoriesId">The a collection of <see cref="Category" />s id</param>
        /// <returns>A collection of retrieved <see cref="Parameter" /></returns>
        private static IEnumerable<Thing> CherryPickParameters(IReadOnlyList<Thing> things, IReadOnlyList<Guid> categoriesId)
        {
            var categorizedParameterTypes = things.OfType<ParameterType>().Where(x => x.Category.Any(categoriesId.Contains))
                .Select(x => x.Iid);

            var parameters = things.OfType<Parameter>().Where(x => categorizedParameterTypes.Contains(x.ParameterType));
            return parameters;
        }

        /// <summary>
        /// Cherry pick <see cref="ParameterOverride" />s where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="categoriesId">The a collection of <see cref="Category" />s id</param>
        /// <returns>A collection of retrieved <see cref="ParameterOverride" /></returns>
        private static IEnumerable<Thing> CherryPickParameterOverrides(IReadOnlyList<Thing> things, IReadOnlyList<Guid> categoriesId)
        {
            var categorizedParameterTypes = things.OfType<ParameterType>().Where(x => x.Category.Any(categoriesId.Contains))
                .Select(x => x.Iid)
                .ToList();

            var parameterOverrides = new List<Thing>();

            foreach (var parameterOverride in things.OfType<ParameterOverride>())
            {
                var associatedParameter = things.OfType<Parameter>().FirstOrDefault(x => x.Iid == parameterOverride.Parameter);

                if (associatedParameter != null && categorizedParameterTypes.Contains(associatedParameter.ParameterType))
                {
                    parameterOverrides.Add(parameterOverride);
                }
            }

            return parameterOverrides;
        }

        /// <summary>
        /// Cherry pick <see cref="ElementBase" />s where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="categoriesId">The a collection of <see cref="Category" />s id</param>
        /// <returns>A collection of retrieved <see cref="ElementBase" /></returns>
        private static IEnumerable<Thing> CherryPickElementUsages(IReadOnlyList<Thing> things, IReadOnlyList<Guid> categoriesId)
        {
            var elementUsages = new List<Thing>();
            elementUsages.AddRange(things.OfType<ElementUsage>().Where(x => x.Category.Any(categoriesId.Contains)));

            foreach (var elementUsage in things.OfType<ElementUsage>())
            {
                var elementDefinition = (ElementDefinition)things.First(x => x.Iid == elementUsage.ElementDefinition);

                if (elementDefinition.Category.Any(categoriesId.Contains) && !elementUsages.Contains(elementUsage))
                {
                    elementUsages.Add(elementUsage);
                }
            }

            return elementUsages;
        }

        /// <summary>
        /// Cherry pick <see cref="ICategorizableThing" />s  where the <see cref="ClassKind" /> is <paramref name="classKind" />
        /// and where a filtering on <see cref="Category" /> can
        /// be applied based on provided <paramref name="categoriesId" />
        /// </summary>
        /// <param name="things">The source collection of <see cref="Thing" /></param>
        /// <param name="categoriesId">The a collection of <see cref="Category" />s id</param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A collection of retrieved <see cref="ICategorizableThing" /></returns>
        private static IEnumerable<Thing> CherryPickCategorizableThings(IEnumerable<Thing> things, IReadOnlyList<Guid> categoriesId, ClassKind classKind)
        {
            return things.Where(x => x.ClassKind == classKind).OfType<ICategorizableThing>().Where(x => x.Category.Any(categoriesId.Contains))
                .Cast<Thing>();
        }
    }       
}
