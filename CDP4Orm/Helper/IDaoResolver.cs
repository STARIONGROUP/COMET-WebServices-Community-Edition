// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDaoResolver.cs" company="Starion Group S.A.">
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

namespace CDP4Orm.Helper
{
    using System;

    using CDP4Orm.Dao;

    /// <summary>
    /// The <see cref="IDaoResolver" /> provides resolve capabilites to retrieve <see cref="IDao" /> based on the type name
    /// <remarks>This interface is registered as InstancePerLifetimeScope</remarks>
    /// </summary>
    public interface IDaoResolver
    {
        /// <summary>
        /// Queries an <see cref="IDao" /> based on the <see cref="Type" /> name
        /// </summary>
        /// <param name="typeName">The <see cref="Type" /> name</param>
        /// <returns>The retrieved <see cref="IDao" /></returns>
        IDao QueryDaoByTypeName(string typeName);
    }
}
