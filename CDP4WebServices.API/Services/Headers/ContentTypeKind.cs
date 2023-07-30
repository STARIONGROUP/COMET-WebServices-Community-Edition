// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeKind.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski
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
    /// <summary>
    /// enumeration datatype that defines the possible kinds of Content
    /// </summary>
    public enum ContentTypeKind
    {
        /// <summary>
        /// Assertion that the content is of type JSON
        /// </summary>
        JSON,

        /// <summary>
        /// Assertion that the content is of type MessagePack
        /// </summary>
        MESSAGEPACK,

        /// <summary>
        /// Assertion that the content is of type multipart mixed
        /// </summary>
        MULTIPARTMIXED,

        /// <summary>
        /// Assertion that the Content-Type header does not need to be set
        /// since it has already been added in a previous operation.
        /// </summary>
        IGNORE
    }
}
