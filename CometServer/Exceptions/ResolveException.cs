// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveException.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Exceptions
{
    using System;

    using CDP4Orm.Dao.Resolve;

    /// <summary>
    /// The <see cref="ResolveException"/> is thrown when a <see cref="ResolveInfo"/> could not be found
    /// where it was expected to exist.
    /// </summary>
    public class ResolveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveException"/> class.
        /// </summary>
        public ResolveException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        public ResolveException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        /// <param name="innerException">
        /// A reference to the inner <see cref="Exception"/>
        /// </param>
        public ResolveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
