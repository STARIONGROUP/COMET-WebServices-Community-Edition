// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModule.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Modules
{
    using System.Collections.Generic;
    using System.Net;

    using Carter;

    using CometServer.Services.ContributorsLocation;

    using Microsoft.AspNetCore.Http;

    using Newtonsoft.Json;

    /// <summary>
    /// The root module handler.
    /// </summary>
    public class AppModule : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModule"/> class.
        /// </summary>
        public AppModule() : base("/app")
        {
            this.Get("/contribution", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.WriteAsJsonAsync("not authenticated");
                }
                else
                {
                    var contributorList = new List<ContributorLocationData>();
                    foreach (var contributorLocationData in ContributorLocationCache.GetContributorsCache())
                    {
                        contributorList.Add((ContributorLocationData)contributorLocationData.Value);
                    }

                    var json = JsonConvert.SerializeObject(contributorList);

                    res.ContentType = "application/json";
                    await res.WriteAsJsonAsync(json);
                }
            });

            this.Get("/{*}", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.WriteAsJsonAsync("not authenticated");
                }
                else
                {
                    res.StatusCode = (int) HttpStatusCode.NotFound;
                    await res.WriteAsync("");
                }
            });
        }
    }
}
