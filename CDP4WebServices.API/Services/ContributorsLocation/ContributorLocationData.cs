// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContributorLocationData.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.ContributorsLocation
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// The contributor's location data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ContributorLocationData
    {
        /// <summary>
        /// Gets or sets the name of a contributor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of a country where a contributor is located.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the name of a city where a contributor is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the latitude of a contributor location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of a contributor location.
        /// </summary>
        public double Longitude { get; set; }
    }
}