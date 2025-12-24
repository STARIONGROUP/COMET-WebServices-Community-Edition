// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostRequestData.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CDP4DalCommon.Protocol.Operations;

    using CometServer.Services;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The purpose of hte <see cref="PostRequestData"/> class is to capture data present in an <see cref="HttpRequest"/>
    /// such that the POST operations that are long running do not depend on a potentialy disposed <see cref="HttpRequest"/>
    /// </summary>
    public class PostRequestData
    {
        /// <summary>
        /// Gets or sets the <see cref="ContentTypeKind"/>
        /// </summary>
        public ContentTypeKind ContentType { get; set; }

        /// <summary>
        /// Gets or sets a string that contains the user name, method and path, typically used for logging
        /// </summary>
        public string MethodPathName { get; set; }

        /// <summary>
        /// Gets or sets the portion of the request path that identifies the requested resource.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the supported CDP4-COMET meta model <see cref="Version"/>
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the POSt request is multipart or not
        /// </summary>
        public bool IsMultiPart { get; set; }

        /// <summary>
        /// Gets or sets the multipart boundary from the <c>Content-Type</c> header.
        /// </summary>
        public string MultiPartBoundary { get; set; }

        /// <summary>
        /// Gets or sets the files captured in the multipart POST <see cref="HttpRequest"/>
        /// </summary>
        public Dictionary<string, Stream> Files;

        /// <summary>
        /// Gets or sets the <see cref="PostOperation"/>
        /// </summary>
        public PostOperation OperationData { get; set; }
    }
}
