// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationContent.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.IntegrationTests.EngineeringModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The content of an iteration, as returned by a deep GET, normalized so that the content of two iterations
    /// can be compared: the properties that are expected to differ between a source iteration and an iteration that
    /// is created from it are removed, and the unordered collections are sorted.
    /// </summary>
    public sealed class IterationContent
    {
        /// <summary>
        /// The properties that are removed from every thing: a copied thing gets the revision number of the
        /// transaction that copied it
        /// </summary>
        private static readonly string[] IgnoredThingProperties = ["revisionNumber", "modifiedOn"];

        /// <summary>
        /// The properties of the Iteration itself that identify the iteration rather than its content. Publications
        /// are not carried over to a new iteration.
        /// </summary>
        private static readonly string[] IgnoredIterationProperties = ["iid", "iterationSetup", "sourceIterationIid", "publication"];

        /// <summary>
        /// Initializes a new instance of the <see cref="IterationContent"/> class
        /// </summary>
        /// <param name="deepIteration">The result of a deep GET of an iteration</param>
        public IterationContent(JArray deepIteration)
        {
            var things = deepIteration.Cast<JObject>().ToList();

            this.Iids = things.Select(x => x["iid"].Value<string>()).ToList();
            this.Things = things.Where(x => x["classKind"].Value<string>() != "Iteration").ToList();
            // duplicate things are tolerated here (the first one is used), so that a test reports them through Iids
            this.Iteration = things.First(x => x["classKind"].Value<string>() == "Iteration");

            var excluded = GetThingsNotCarriedOver(things);

            this.Normalized = things
                .Where(x => !excluded.Contains(x["iid"].Value<string>()))
                .GroupBy(x => x["classKind"].Value<string>() == "Iteration" ? "Iteration" : x["iid"].Value<string>())
                .ToDictionary(
                    x => x.Key,
                    x => x.Key == "Iteration"
                        ? Normalize(x.First(), IgnoredThingProperties.Concat(IgnoredIterationProperties), excluded)
                        : Normalize(x.First(), IgnoredThingProperties));
        }

        /// <summary>
        /// Gets the iids of all returned things, including duplicates
        /// </summary>
        public IReadOnlyList<string> Iids { get; }

        /// <summary>
        /// Gets all returned things except the Iteration itself
        /// </summary>
        public IReadOnlyList<JObject> Things { get; }

        /// <summary>
        /// Gets the Iteration itself
        /// </summary>
        public JObject Iteration { get; }

        /// <summary>
        /// Gets the normalized JSON per thing iid, the Iteration itself is keyed as "Iteration"
        /// </summary>
        public IReadOnlyDictionary<string, string> Normalized { get; }

        /// <summary>
        /// Gets the thing with the given iid, or null
        /// </summary>
        /// <param name="iid">The iid</param>
        /// <returns>The thing or null</returns>
        public JObject Get(Guid iid)
        {
            return this.Things.FirstOrDefault(x => x["iid"].Value<string>() == iid.ToString());
        }

        /// <summary>
        /// Gets whether a thing with the given iid is present
        /// </summary>
        /// <param name="iid">The iid</param>
        /// <returns>True when present</returns>
        public bool Contains(Guid iid)
        {
            return this.Get(iid) != null;
        }

        /// <summary>
        /// Describes the differences between this content and the expected content
        /// </summary>
        /// <param name="expected">The expected content</param>
        /// <returns>The differences, empty when the contents are equivalent</returns>
        public IReadOnlyList<string> DifferencesWith(IterationContent expected)
        {
            var differences = new List<string>();

            foreach (var missing in expected.Normalized.Keys.Except(this.Normalized.Keys))
            {
                differences.Add($"missing: {expected.Normalized[missing]}");
            }

            foreach (var unexpected in this.Normalized.Keys.Except(expected.Normalized.Keys))
            {
                differences.Add($"unexpected: {this.Normalized[unexpected]}");
            }

            foreach (var key in this.Normalized.Keys.Intersect(expected.Normalized.Keys).Where(key => this.Normalized[key] != expected.Normalized[key]))
            {
                differences.Add($"different:{Environment.NewLine}  expected: {expected.Normalized[key]}{Environment.NewLine}  actual:   {this.Normalized[key]}");
            }

            return differences;
        }

        /// <summary>
        /// Gets the iids of the things that the server intentionally does not carry over to a new iteration: the
        /// Publications, and the Relationships that relate a Publication (removed by the cascading delete of the
        /// Publication)
        /// </summary>
        /// <param name="things">All things of the iteration</param>
        /// <returns>The iids</returns>
        private static HashSet<string> GetThingsNotCarriedOver(IReadOnlyList<JObject> things)
        {
            var publications = things
                .Where(x => x["classKind"].Value<string>() == "Publication")
                .Select(x => x["iid"].Value<string>())
                .ToHashSet();

            var relationshipsToPublications = things
                .Where(x => RelatedThings(x).Any(publications.Contains))
                .Select(x => x["iid"].Value<string>());

            publications.UnionWith(relationshipsToPublications.ToList());

            return publications;
        }

        /// <summary>
        /// Gets the iids of the things related by a BinaryRelationship or MultiRelationship
        /// </summary>
        /// <param name="thing">The thing</param>
        /// <returns>The related iids, empty when the thing is not a relationship</returns>
        private static IEnumerable<string> RelatedThings(JObject thing)
        {
            switch (thing["classKind"].Value<string>())
            {
                case "BinaryRelationship":
                    return [thing["source"]?.Value<string>(), thing["target"]?.Value<string>()];
                case "MultiRelationship":
                    return thing["relatedThing"]?.Values<string>() ?? [];
                default:
                    return [];
            }
        }

        /// <summary>
        /// Removes the ignored properties, orders the properties by name and sorts the unordered collections
        /// </summary>
        /// <param name="thing">The thing</param>
        /// <param name="ignoredProperties">The names of the properties to remove</param>
        /// <param name="excludedReferences">The iids to remove from the reference collections</param>
        /// <returns>The normalized JSON</returns>
        private static string Normalize(JObject thing, IEnumerable<string> ignoredProperties, IReadOnlySet<string> excludedReferences = null)
        {
            var ignored = ignoredProperties.ToHashSet();
            var normalized = new JObject();

            foreach (var property in thing.Properties().Where(x => !ignored.Contains(x.Name)).OrderBy(x => x.Name, StringComparer.Ordinal))
            {
                var value = property.Value;

                if (excludedReferences != null && value is JArray references)
                {
                    value = new JArray(references.Where(x => x.Type != JTokenType.String || !excludedReferences.Contains(x.Value<string>())));
                }

                normalized.Add(property.Name, value is JArray array ? SortCollection(array) : value.DeepClone());
            }

            return normalized.ToString(Formatting.None);
        }

        /// <summary>
        /// Sorts an ordered collection (items with a "k" key) by key, and any other collection by value
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <returns>The sorted collection</returns>
        private static JArray SortCollection(JArray collection)
        {
            if (collection.All(x => x is JObject item && item["k"] != null))
            {
                return new JArray(collection.OrderBy(x => x["k"].Value<long>()).Select(x => x.DeepClone()));
            }

            return new JArray(collection.OrderBy(x => x.ToString(Formatting.None), StringComparer.Ordinal).Select(x => x.DeepClone()));
        }
    }
}
