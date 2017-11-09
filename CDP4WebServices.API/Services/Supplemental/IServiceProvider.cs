// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
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