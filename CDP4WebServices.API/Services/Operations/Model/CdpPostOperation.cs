// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CdpPostOperation.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common;
    using CDP4Common.DTO;
    using Newtonsoft.Json;

    /// <summary>
    /// The CDP POST operation.
    /// </summary>
    public class CdpPostOperation
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CdpPostOperation"/>
        /// </summary>
        public CdpPostOperation()
        {
            this.Delete = new List<ClasslessDTO>();
            this.Create = new List<Thing>();
            this.Update = new List<ClasslessDTO>();
            this.Copy = new List<ClasslessDTO>();
        }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be deleted
        /// </summary>
        [JsonProperty("_delete")]
        public List<ClasslessDTO> Delete { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be create
        /// </summary>
        [JsonProperty("_create")]
        public List<Thing> Create { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be updated
        /// </summary>
        [JsonProperty("_update")]
        public List<ClasslessDTO> Update { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be copied
        /// </summary>
        [JsonProperty("_copy")]
        public List<ClasslessDTO> Copy { get; set; }
    }
}
