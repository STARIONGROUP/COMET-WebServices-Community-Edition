// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceProvider.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;

    using CDP4Common.DTO;

    /// <summary>
    /// The Service Registry interface.
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        IReadService MapToReadService(string typeName);

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        TService MapToReadService<TService>(string typeName) where TService : IReadService;

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        IReadService MapToReadService(Thing thing);

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        TService MapToReadService<TService>(Thing thing) where TService : IReadService;

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        IPersistService MapToPersitableService(string typeName);

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        TService MapToPersitableService<TService>(string typeName) where TService : IPersistService;

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        IPersistService MapToPersitableService(Thing thing);

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        TService MapToPersitableService<TService>(Thing thing) where TService : IPersistService;
    }
}