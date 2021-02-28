// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceProcessor.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.Types;

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
        /// The utils helper class for this request.
        /// </param>
        public ResourceProcessor(
            IServiceProvider serviceProvider,
            NpgsqlTransaction transaction,
            IRequestUtils utils)
        {
            this.ServiceProvider = serviceProvider;
            this.Transaction = transaction;
            this.RequestUtils = utils;
        }

        /// <summary>
        /// Gets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public NpgsqlTransaction Transaction { get; private set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public IRequestUtils RequestUtils { get; private set; }

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
                throw new Exception(string.Format("{0} is not a valid containment property", propertyName));
            }

            var containerTypeName = containment.Last().GetType().Name;
            return
                this.RequestUtils.MetaInfoProvider.GetMetaInfo(containerTypeName)
                    .GetContainmentType(Utils.CapitalizeFirstLetter(propertyName)).TypeName;
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
                throw new Exception("Resource not found");
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

            var containmentPropertyValue = this.RequestUtils.MetaInfoProvider.GetMetaInfo(container)
                .GetValue(Utils.CapitalizeFirstLetter(propertyName), container);

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
        public IEnumerable<Thing> GetResource(string serviceType, string topContainer, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.ServiceProvider.MapToReadService(serviceType)
                                 .Get(this.Transaction, topContainer, ids, containerSecurityContext);
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
        public Thing GetContainmentResource(string serviceType, string topContainer, Guid identifier, ISecurityContext containerSecurityContext)
        {
            // override query parameters
            this.RequestUtils.OverrideQueryParameters = new QueryParameters();
            
            // collect the specified containment resource
            var containementResource = this.GetResource(serviceType, topContainer, new[] { identifier }, containerSecurityContext).Single();

            // reset query parameters
            this.RequestUtils.OverrideQueryParameters = null;
            return containementResource;
        }
    }
}