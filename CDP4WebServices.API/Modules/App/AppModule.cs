// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModule.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System.Collections.Generic;

    using CDP4WebServices.API.Services.ContributorsLocation;

    using Nancy;
    using Nancy.Security;

    using Newtonsoft.Json;

    /// <summary>
    /// The root module handler.
    /// </summary>
    public class AppModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModule"/> class.
        /// </summary>
        public AppModule()
            : base("/app")
        {
            this.RequiresAuthentication();

            this.Get["/contribution"] = _ =>
                {
                    var contributorList = new List<ContributorLocationData>();
                    foreach (var contributorLocationData in ContributorLocationCache.GetContributorsCache())
                    {
                        contributorList.Add((ContributorLocationData)contributorLocationData.Value);
                    }

                    return JsonConvert.SerializeObject(contributorList);
                };
            this.Get["/{*}"] = _ => (Response)HttpStatusCode.NotFound;
        }
    }
}