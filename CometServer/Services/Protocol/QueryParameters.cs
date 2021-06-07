// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryParameters.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using CDP4Common.DTO;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The query parameters of the current request.
    /// </summary>
    public class QueryParameters : IQueryParameters
    {
        /// <summary>
        /// The extent query parameter.
        /// </summary>
        public const string ExtentQuery = "extent";

        /// <summary>
        /// The include reference data query parameter.
        /// </summary>
        public const string IncludeReferenceDataQuery = "includeReferenceData";

        /// <summary>
        /// The include all containers query parameter.
        /// </summary>
        public const string IncludeAllContainersQuery = "includeAllContainers";

        /// <summary>
        /// The include file data query parameter.
        /// </summary>
        public const string IncludeFileDataQuery = "includeFileData";

        /// <summary>
        /// The export query parameter.
        /// </summary>
        public const string ExportQuery = "export";

        /// <summary>
        /// The revision number query parameter.
        /// </summary>
        public const string RevisionNumberQuery = "revisionNumber";

        /// <summary>
        /// The revision number FROM which the request to get the revisions of a <see cref="Thing"/> is done
        /// </summary>
        public const string RevisionFromQuery = "revisionFrom";

        /// <summary>
        /// The revision number TO which the request to get the revisions of a <see cref="Thing"/> is done
        /// </summary>
        public const string RevisionToQuery = "revisionTo";

        /// <summary>
        /// The query parameter definitions.
        /// </summary>
        private readonly Dictionary<string, string[]> queryParameterDefinitions = new Dictionary<string, string[]>
                               {
                                   { ExtentQuery, new[] { "deep", "shallow" } },
                                   { IncludeReferenceDataQuery, new[] { "true", "false" } },
                                   { IncludeAllContainersQuery, new[] { "true", "false" } },
                                   { IncludeFileDataQuery, new[] { "true", "false" } },
                                   { ExportQuery, new[] { "true", "false" } }
                               };

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters"/> class.
        /// </summary>
        public QueryParameters()
        {
            this.SetupQueryParameterDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters"/> class.
        /// </summary>
        /// <param name="queryCollection">
        /// The query Parameters.
        /// </param>
        public QueryParameters(IQueryCollection queryCollection)
        {
            this.SetupQueryParameterDefaults();

            // process the query information against the supported query parameters
            this.ExtentDeep = this.ProcessQueryParameter(queryCollection, ExtentQuery, "deep");
            this.IncludeReferenceData = this.ProcessQueryParameter(queryCollection, IncludeReferenceDataQuery, "true");
            this.IncludeAllContainers = this.ProcessQueryParameter(queryCollection, IncludeAllContainersQuery, "true");
            this.IncludeFileData = this.ProcessQueryParameter(queryCollection, IncludeFileDataQuery, "true");
            this.Export = this.ProcessQueryParameter(queryCollection, ExportQuery, "true");
            this.RevisionNumber = this.ProcessQueryParameter(queryCollection, RevisionNumberQuery);
            this.RevisionFrom = this.ProcessRevisionHistoryQueryParameter(queryCollection, RevisionFromQuery);
            this.RevisionTo = this.ProcessRevisionHistoryQueryParameter(queryCollection, RevisionToQuery);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to extent deep.
        /// </summary>
        public bool ExtentDeep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include reference data.
        /// </summary>
        public bool IncludeReferenceData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include all containers.
        /// </summary>
        public bool IncludeAllContainers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include file data.
        /// </summary>
        public bool IncludeFileData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to export data.
        /// </summary>
        public bool Export { get; set; }

        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision number, or DateTime from which the request is done
        /// </summary>
        public object RevisionFrom { get; set; }

        /// <summary>
        /// Gets or sets the revision number, or DateTime to which the request is done
        /// </summary>
        public object RevisionTo { get; set; }

        /// <summary>
        /// The validate query parameter.
        /// </summary>
        /// <param name="queryParameter">
        /// The query parameter.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="Exception">
        /// If unknown query parameter or value is passed
        /// </exception>
        public void ValidateQueryParameter(string queryParameter, string value)
        {
            if (!this.queryParameterDefinitions.ContainsKey(queryParameter))
            {
                throw new Exception($"Query parameter {queryParameter} is not supported");
            }

            if (!this.queryParameterDefinitions[queryParameter].Contains(value))
            {
                throw new Exception("Invalid query parameter value supplied");
            }
        }

        /// <summary>
        /// Extracts a QueryParameter based on the <paramref name="key"/> from the <see cref="IQueryCollection"/>
        /// </summary>
        /// <param name="queryCollection">
        /// The <see cref="IQueryCollection"/> that may contain query parameters
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="trueValue">
        /// The true value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ProcessQueryParameter(IQueryCollection queryCollection, string key, string trueValue)
        {
            if (!queryCollection.TryGetValue(key, out var queryParameterValue))
            {
                return false;
            }

            this.ValidateQueryParameter(key, queryParameterValue);

            return queryParameterValue == trueValue;
        }

        /// <summary>
        /// Extracts the Revision QueryParameter from the <see cref="IQueryCollection"/>
        /// </summary>
        /// <param name="queryCollection">
        /// The <see cref="IQueryCollection"/> that may contain query parameters
        /// </param>
        /// <param name="key">
        /// The key of the Query Parameter to process
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected int ProcessQueryParameter(IQueryCollection queryCollection, string key)
        {
            if (!queryCollection.TryGetValue(key, out var queryParameterValue))
            {
                return -1;
            }

            if (!int.TryParse(queryParameterValue, out var revisionNumber))
            {
                return -1;
            }

            return revisionNumber;
        }

        /// <summary>
        /// Extracts the RevisionHistory QueryParameter from the <see cref="IQueryCollection"/>
        /// </summary>
        /// <param name="queryCollection">
        /// The <see cref="IQueryCollection"/> that may contain query parameters
        /// </param>
        /// <param name="key">
        /// The key of the Query Parameter to process
        /// </param>
        /// <returns>
        /// The revisionhistory query parameter as <see cref="int"/> or as <see cref="DateTime"/>
        /// </returns>
        protected object ProcessRevisionHistoryQueryParameter(IQueryCollection queryCollection, string key)
        {
            if (!queryCollection.TryGetValue(key, out var queryParameterValue))
            {
                return null;
            }

            if (int.TryParse(queryParameterValue, out var revNumber))
            {
                return revNumber;
            }

            if (!DateTime.TryParseExact(queryParameterValue, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp))
            {
                throw new ArgumentOutOfRangeException(key);
            }

            return timestamp;
        }

        /// <summary>
        /// Setup query parameter defaults.
        /// </summary>
        private void SetupQueryParameterDefaults()
        {
            this.ExtentDeep = false;
            this.IncludeReferenceData = false;
            this.IncludeAllContainers = false;
            this.IncludeFileData = false;            
            this.Export = false;            
        }
    }
}
