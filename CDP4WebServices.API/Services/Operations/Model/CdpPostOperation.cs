// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CdpPostOperation.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Operations
{
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.Dto;
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
            this.Copy = new List<CopyInfo>();
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
        public List<CopyInfo> Copy { get; set; }
    }
}
