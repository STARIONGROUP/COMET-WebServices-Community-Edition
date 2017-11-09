// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModule.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System.Collections.Generic;

    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Services.ContributorsLocation;

    using Nancy;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;
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

            #if DEBUG
            this.Get["/"] = _ => new RedirectResponse(AppConfig.Current.Defaults.DevServerPath);
            this.Get["/contribution"] = _ =>
                {
                    var contributorList = new List<ContributorLocationData>();
                    foreach (var contributorLocationData in ContributorLocationCache.GetContributorsCache())
                    {
                        contributorList.Add((ContributorLocationData)contributorLocationData.Value);
                    }

                    return JsonConvert.SerializeObject(contributorList);
                };
            this.Get["/{*}"] = _ => new RedirectResponse(AppConfig.Current.Defaults.DevServerPath);
            #else
                this.Get["/"] = _ => this.AppHomePageView();
                this.Get["/contribution"] = _ =>
                {
                    var contributorList = new List<ContributorLocationData>();
                    foreach (var contributorLocationData in ContributorLocationCache.GetContributorsCache())
                    {
                        contributorList.Add((ContributorLocationData)contributorLocationData.Value);
                    }

                    return JsonConvert.SerializeObject(contributorList);
                };
                this.Get["/{*}"] = _ => this.AppHomePageView();
            #endif
        }

        /// <summary>
        /// The app home page view.
        /// </summary>
        /// <returns>
        /// The <see cref="Negotiator"/>.
        /// </returns>
        private Negotiator AppHomePageView()
        {
            return this.View["index"];
        }
    }
}