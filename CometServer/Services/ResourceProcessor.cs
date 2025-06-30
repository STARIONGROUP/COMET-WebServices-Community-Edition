// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceProcessor.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;
    using CDP4Common.Types;

    using CometServer.Extensions;
    using CometServer.Services.Authorization;
    using CometServer.Services.Protocol;

    using Npgsql;

    /// <summary>
    /// This is a processor class which returns requested resources
    /// </summary>
    public class ResourceProcessor : IProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProcessor"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service registry
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="utils">
        /// The <see cref="IRequestUtils"/> for this request.
        /// </param>
        /// <param name="meetaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide meta-data for any kinf of <see cref="Thing"/>
        /// </param>
        public ResourceProcessor(NpgsqlTransaction transaction, IServiceProvider serviceProvider, IRequestUtils utils, IMetaInfoProvider meetaInfoProvider)
        {
            this.Transaction = transaction;

            this.ServiceProvider = serviceProvider;
            
            this.RequestUtils = utils;

            this.MetaInfoProvider = meetaInfoProvider;
        }

        /// <summary>
        /// Gets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public IRequestUtils RequestUtils { get; private set; }

        /// <summary>
        /// Gets the <see cref="IMetaInfoProvider"/> used to provide meta-data for any kinf of <see cref="Thing"/>
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; private set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public NpgsqlTransaction Transaction { get; private set; }

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
        public string GetContainmentType(IReadOnlyCollection<Thing> containment, string propertyName)
        {
            if (containment.Count == 0 || !char.IsLower(propertyName.First()))
            {
                throw new ArgumentException($"{propertyName} is not a valid containment property", nameof(propertyName));
            }

            var containerTypeName = containment.Last().GetType().Name;

            return
                this.MetaInfoProvider.GetMetaInfo(containerTypeName)
                    .GetContainmentType(propertyName.CapitalizeFirstLetter()).TypeName;
        }

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
        public IEnumerable<Guid> GetContainment(List<Thing> containers, string propertyName)
        {
            if (containers.Count == 0)
            {
                return null;
            }

            var container = containers.Last();
            return this.GetContainmentProperty(container, propertyName);
        }

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
        public void ValidateContainment(List<Thing> containers, string propertyName, Guid id)
        {
            if (containers.Count == 0)
            {
                return;
            }

            var containment = this.GetContainmentProperty(containers.Last(), propertyName);

            if (!containment.Contains(id))
            {
                throw new ThingNotFoundException($"Resource not found: {propertyName}.{id}");
            }
        }

        /// <summary>
        /// Returns the containment property value from the supplied <see cref="CDP4Common.DTO.Thing"/>.
        /// </summary>
        /// <param name="container">
        /// The <see cref="CDP4Common.DTO.Thing"/> for which to return the containment property.
        /// </param>
        /// <param name="propertyName">
        /// Name of the containment property for which to return the value.
        /// </param>
        /// <returns>
        /// A collection of containment <see cref="Guid"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If propName is not supported for the <see cref="CDP4Common.DTO.Thing"/>
        /// </exception>
        public IEnumerable<Guid> GetContainmentProperty(Thing container, string propertyName)
        {
            // TODO reenable when json deserialisation problem is solved (task T2780 CDP4WEBSERVICES) and remove the below code.
            //return this.RequestUtils.MetaInfoProvider.GetMetaInfo(container).GetContainmentIds(
            //    container, Utils.CapitalizeFirstLetter(propertyName));

            var containmentPropertyValue = this.MetaInfoProvider.GetMetaInfo(container)
                .GetValue(propertyName.CapitalizeFirstLetter(), container);

            if (containmentPropertyValue.GetType() == typeof(List<Guid>))
            {
                return containmentPropertyValue as List<Guid>;
            }
            else if (containmentPropertyValue.GetType() == typeof(List<OrderedItem>))
            {
                return (containmentPropertyValue as List<OrderedItem>).Select(x => Guid.Parse(x.V.ToString()));
            }

            return null; 
        }

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
        public Task<IEnumerable<Thing>> GetResourceAsync(string serviceType, string topContainer, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.ServiceProvider.MapToReadService(serviceType)
                                 .GetAsync(this.Transaction, topContainer, ids, containerSecurityContext);
        }

        /// <summary>
        /// Get the containment resource.
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
        public async Task<Thing> GetContainmentResource(string serviceType, string topContainer, Guid identifier, ISecurityContext containerSecurityContext)
        {
            // override query parameters
            this.RequestUtils.OverrideQueryParameters = new QueryParameters();
            
            // collect the specified containment resource
            var containementResource = (await this.GetResourceAsync(serviceType, topContainer, [identifier], containerSecurityContext)).SingleOrDefault();

            if (containementResource == null)
            {
                throw new ThingNotFoundException($"{serviceType} {identifier} not found");
            }

            // reset query parameters
            this.RequestUtils.OverrideQueryParameters = null;
            return containementResource;
        }
    }
}
