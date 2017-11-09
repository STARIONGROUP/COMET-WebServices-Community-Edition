// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessor.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The processor interface.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Gets the request utils.
        /// </summary>
        IRequestUtils RequestUtils { get; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        NpgsqlTransaction Transaction { get; }

        /// <summary>
        /// Returns the containment type based from the supplied container property.
        /// </summary>
        /// <param name="containment">
        /// The containment collection.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The type name of the containment property.
        /// </returns>
        /// <exception cref="Exception">
        /// If property is not supported or the containment collection is null
        /// </exception>
        string GetContainmentType(IReadOnlyCollection<Thing> containment, string propertyName);

        /// <summary>
        /// Get the containment id collection from the container property.
        /// </summary>
        /// <param name="containers">
        /// The containers collection.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// null if there is no containment otherwise the a list of contained item ids
        /// </returns>
        IEnumerable<Guid> GetContainment(List<Thing> containers, string propertyName);

        /// <summary>
        /// validate that the container property collection contains an entry for the supplied id.
        /// </summary>
        /// <param name="containers">
        /// The containers collection.
        /// </param>
        /// <param name="propertyName">
        /// The containment property name.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        void ValidateContainment(List<Thing> containers, string propertyName, Guid id);

        /// <summary>
        /// Get resource(s) from the back tier.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="ids">
        /// The list of ids to retrieve.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The container Security Context.
        /// </param>
        /// <returns>
        /// Collection of retrieved resources.
        /// </returns>
        IEnumerable<Thing> GetResource(string serviceType, string topContainer, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext);

        /// <summary>
        /// The get containment resource.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The container Security Context.
        /// </param>
        /// <returns>
        /// The <see cref="Thing"/>.
        /// </returns>
        Thing GetContainmentResource(string serviceType, string topContainer, Guid identifier, ISecurityContext containerSecurityContext);
    }
}