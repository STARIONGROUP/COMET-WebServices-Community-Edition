// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpConstants.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
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

namespace CometServer.Services
{
    /// <summary>
    /// Definitions of CDP4-COMET constants that are used to support the HTTP standard
    /// </summary>
    public static class HttpConstants
    {
        /// <summary>
        /// The Multipart message boundary string <see href="https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html"/>
        /// </summary>
        public static readonly string BoundaryString = "----Boundary";

        /// <summary>
        /// The mime type JSON.
        /// </summary>
        public static readonly string MimeTypeJson = "application/json";

        /// <summary>
        /// The MessagePack mime type.
        /// </summary>
        public static readonly string MimeTypeMessagePack = "application/msgpack";

        /// <summary>
        /// The mime type octet stream.
        /// </summary>
        public static readonly string MimeTypeOctetStream = "application/octet-stream";

        /// <summary>
        /// The content type header.
        /// </summary>
        public static readonly string ContentTypeHeader = "Content-Type";

        /// <summary>
        /// The content disposition header.
        /// </summary>
        public static readonly string ContentDispositionHeader = "Content-Disposition";

        /// <summary>
        /// The content length header.
        /// </summary>
        public static readonly string ContentLengthHeader = "Content-Length";

        /// <summary>
        /// The name of the CDP4 common response header.
        /// </summary>
        public static readonly string Cdp4CommonHeader = "CDP4-Common";

        /// <summary>
        /// The name of the CDP4 Server response header.
        /// </summary>
        public static readonly string Cdp4ServerHeader = "CDP4-Server";

        /// <summary>
        /// The name of the CDP4 Server response header.
        /// </summary>
        public static readonly string CometServerHeader = "COMET-Server";

        /// <summary>
        /// The accept CDP version header.
        /// </summary>
        public static readonly string AcceptCdpVersionHeader = "Accept-CDP";

        /// <summary>
        /// The default data model version.
        /// </summary>
        public static readonly string DefaultDataModelVersion = "1.0.0";
    }
}
