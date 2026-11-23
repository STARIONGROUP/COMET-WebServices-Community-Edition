// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometRestClient.cs" company="Starion Group S.A.">
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

namespace CometServer.IntegrationTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    /// <summary>
    /// A minimal ECSS-E-TM-10-25 Annex C.2 JSON client for the in-process CDP4-COMET REST API
    /// </summary>
    public sealed class CometRestClient : IDisposable
    {
        /// <summary>
        /// The <see cref="HttpClient"/> connected to the in-process server
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CometRestClient"/> class
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> connected to the in-process server</param>
        /// <param name="userName">The user name used for basic authentication</param>
        /// <param name="password">The password used for basic authentication</param>
        public CometRestClient(HttpClient httpClient, string userName, string password)
        {
            this.httpClient = httpClient;

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Performs a GET request and asserts that it succeeds
        /// </summary>
        /// <param name="relativeUri">The relative uri, e.g. EngineeringModel/{iid}/iteration/{iid}?extent=deep</param>
        /// <returns>The returned things as a <see cref="JArray"/></returns>
        public async Task<JArray> GetAsync(string relativeUri)
        {
            using var response = await this.httpClient.GetAsync(relativeUri);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"GET {relativeUri} failed: {content}");

            return JArray.Parse(content);
        }

        /// <summary>
        /// Performs a POST request and asserts that it succeeds
        /// </summary>
        /// <param name="relativeUri">The relative uri, either SiteDirectory/{iid} or EngineeringModel/{iid}/iteration/{iid}</param>
        /// <param name="postBody">The body with the _delete, _create and _update parts</param>
        /// <returns>The returned things as a <see cref="JArray"/></returns>
        public async Task<JArray> PostAsync(string relativeUri, JObject postBody)
        {
            var (statusCode, content) = await this.TryPostAsync(relativeUri, postBody);

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), $"POST {relativeUri} failed: {content}");

            return JArray.Parse(content);
        }

        /// <summary>
        /// Performs a POST request without asserting the result
        /// </summary>
        /// <param name="relativeUri">The relative uri, either SiteDirectory/{iid} or EngineeringModel/{iid}/iteration/{iid}</param>
        /// <param name="postBody">The body with the _delete, _create and _update parts</param>
        /// <returns>The <see cref="HttpStatusCode"/> and the content of the response</returns>
        public async Task<(HttpStatusCode StatusCode, string Content)> TryPostAsync(string relativeUri, JObject postBody)
        {
            using var requestContent = new StringContent(postBody.ToString(Formatting.None), Encoding.UTF8, "application/json");
            using var response = await this.httpClient.PostAsync(relativeUri, requestContent);

            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Disposes the <see cref="HttpClient"/>
        /// </summary>
        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
