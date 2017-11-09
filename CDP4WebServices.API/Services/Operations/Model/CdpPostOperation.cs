// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CdpPostOperation.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations
{
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Operations;

    using Newtonsoft.Json;

    /// <summary>
    /// The CDP POST operation.
    /// </summary>
    public class CdpPostOperation : PostOperation
    {
        /// <summary>
        /// Gets or sets the collection of DTO's that need to be deleted
        /// </summary>
        [JsonProperty("_delete")]
        public override List<ClasslessDTO> Delete { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be create
        /// </summary>
        [JsonProperty("_create")]
        public override List<Thing> Create { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be updated
        /// </summary>
        [JsonProperty("_update")]
        public override List<ClasslessDTO> Update { get; set; }

        /// <summary>
        /// Gets or sets the collection of DTO's that need to be copied
        /// </summary>
        [JsonProperty("_copy")]
        public override List<ClasslessDTO> Copy { get; set; }

        /// <summary>
        /// The construct from operation.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <remarks>
        /// Not used
        /// </remarks>
        public override void ConstructFromOperation(Operation operation)
        {
            // do nothing
        }
    }
}
