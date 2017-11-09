// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestUtils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Helpers;
    using CDP4JsonSerializer;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Protocol;

    using Nancy;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The RequestUtils interface.
    /// </summary>
    public interface IRequestUtils
    {
        /// <summary>
        /// Gets or sets the cache for this request.
        /// </summary>
        List<Thing> Cache { get; set; }

        /// <summary>
        /// Gets or sets the meta info provider for this request.
        /// </summary>
        IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        IQueryParameters QueryParameters { get; set; }

        /// <summary>
        /// Sets the override query parameters to be used for specific processing logic.
        /// set to null to use request query parameters
        /// </summary>
        IQueryParameters OverrideQueryParameters { set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultPermissionProvider"/> for this request
        /// </summary>
        IDefaultPermissionProvider DefaultPermissionProvider { get; set; }
        
        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        ICdp4RequestContext Context { get; set; }

        /// <summary>
        /// Gets the get request data model version.
        /// </summary>
        Version GetRequestDataModelVersion { get; }

        /// <summary>
        /// Convenience method to return a Http <see cref="Response"/> object with serialized JSON content.
        /// </summary>
        /// <param name="jsonArray">
        /// A <see cref="JArray"/> instance that is to be serialized into the <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A HTTP <see cref="Response"/> that can be returned by a web service API endpoint.
        /// </returns>
        Response GetJsonResponse(JArray jsonArray);

        /// <summary>
        /// Construct the engineering model partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        string GetEngineeringModelPartitionString(Guid engineeringModelIid);
    }
}
