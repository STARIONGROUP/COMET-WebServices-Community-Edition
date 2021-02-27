// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
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

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.Helpers;
    using CDP4JsonSerializer;
    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Protocol;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utils class available in the context of a request.
    /// </summary>
    public class RequestUtils : IRequestUtils
    {
        /// <summary>
        /// The default data model version.
        /// </summary>
        private const string DefaultDataModelVersion = "1.0.0";

        /// <summary>
        /// The accept CDP version header.
        /// </summary>
        private const string AcceptCdpVersionHeader = "Accept-CDP";

        /// <summary>
        /// The query parameters of the request.
        /// </summary>
        private IQueryParameters queryParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestUtils"/> class.
        /// </summary>
        public RequestUtils()
        {
            this.Cache = new List<Thing>();
        }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        public List<Thing> Cache { get; set; }

        /// <summary>
        /// Gets or sets the meta info provider for this request.
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultPermissionProvider"/> for this request
        /// </summary>
        public IDefaultPermissionProvider DefaultPermissionProvider { get; set; }

        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        public ICdp4RequestContext Context { get; set; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        public IQueryParameters QueryParameters
        {
            get
            {
                return this.OverrideQueryParameters ?? this.queryParameters;
            }

            set
            {
                this.queryParameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the override query parameters to be used for specific processing logic.
        /// set to null to use request query parameters
        /// </summary>
        public IQueryParameters OverrideQueryParameters { private get;  set; }

        /// <summary>
        /// Gets the get request data model version.
        /// </summary>
        public Version GetRequestDataModelVersion
        {
            get
            {
                var versionHeader =
                    this.Context.Context.Request.Headers.SingleOrDefault(
                        x =>
                            x.Key.ToLower(CultureInfo.InvariantCulture) ==
                            AcceptCdpVersionHeader.ToLower(CultureInfo.InvariantCulture));

                return !versionHeader.Equals(default(KeyValuePair<string, IEnumerable<string>>))
                    ? new Version(versionHeader.Value.First())
                    : new Version(DefaultDataModelVersion);
            }
        }

        /// <summary>
        /// Convenience method to return a Http <see cref="Response"/> object with serialized JSON content.
        /// </summary>
        /// <param name="jsonArray">
        /// A <see cref="JArray"/> instance that is to be serialized into the <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A HTTP <see cref="Response"/> that can be returned by a web service API endpoint.
        /// </returns>
        public Response GetJsonResponse(JArray jsonArray)
        {
            return new Response
                       {
                           ContentType = "application/json",
                           Contents = stream => this.WriteToStream(jsonArray, stream)
                       };
        }


        /// <summary>
        /// Construct the engineering model partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        public string GetEngineeringModelPartitionString(Guid engineeringModelIid)
        {
            return $"EngineeringModel_{engineeringModelIid.ToString().Replace("-", "_")}";
        }

        /// <summary>
        /// Construct the iteration partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineeringModel id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        public string GetIterationPartitionString(Guid engineeringModelIid)
        {
            return $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";
        }

        /// <summary>
        /// Helper method to Allow serialization to the passed in output stream.
        /// </summary>
        /// <param name="jsonArray">
        /// The JSON array that will be serialized.
        /// </param>
        /// <param name="outputStream">
        /// The output Stream used to write the serialization data.
        /// </param>
        private void WriteToStream(JArray jsonArray, Stream outputStream)
        {
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(outputStream)))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jsonWriter, jsonArray);
                jsonWriter.Flush();
            }
        }
    }
}
