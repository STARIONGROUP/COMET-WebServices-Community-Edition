// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="RHEA System S.A.">
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

    using CDP4Common;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The service base.
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// The create type of modify operation.
        /// </summary>
        public const string CreateOperation = "create";

        /// <summary>
        /// The update type of modify operation.
        /// </summary>
        public const string UpdateOperation = "update";

        /// <summary>
        /// The delete type of modify operation.
        /// </summary>
        public const string DeleteOperation = "delete";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the permission service.
        /// </summary>
        public IPermissionService PermissionService { get; set; }

        /// <summary>
        /// Gets or sets the Utils instance for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Copy the <paramref name="sourceThing"/> into the target <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="sourceThing">The source <see cref="Thing"/> to copy</param>
        /// <param name="targetContainer">The target container <see cref="Thing"/></param>
        /// <param name="allSourceThings">All source <see cref="Thing"/>s in the current copy operation</param>
        /// <param name="copyinfo">The <see cref="CopyInfo"/></param>
        /// <param name="sourceToCopyMap">A dictionary mapping identifiers of original to copy</param>
        /// <param name="rdls">The <see cref="ReferenceDataLibrary"/></param>
        /// <param name="targetEngineeringModelSetup">The target <see cref="EngineeringModelSetup"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public virtual void Copy(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo,
            Dictionary<Guid, Guid> sourceToCopyMap, IReadOnlyList<ReferenceDataLibrary> rdls, EngineeringModelSetup targetEngineeringModelSetup, ISecurityContext securityContext)
        {
        }

        /// <summary>
        /// Execute additional logic before each get function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from which the requested resource is to be retrieved.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="includeReferenceData">
        /// Control flag to indicate if reference library data should be retrieved extent=deep or extent=shallow.
        /// </param>
        /// <returns>
        /// Return true to continue logic flow, or false to stop further concept retrieval logic.
        /// </returns>
        public virtual bool BeforeGet(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            return true;
        }

        /// <summary>
        /// Execute additional logic after each get function call.
        /// </summary>
        /// <param name="resultCollection">
        /// An instance collection that was retrieved from the persistence layer.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from which the requested resource is to be retrieved.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="includeReferenceData">
        /// Control flag to indicate if reference library data should be retrieved extent=deep or extent=shallow.
        /// </param>
        /// <returns>
        /// A post filtered instance of the passed in resultCollection.
        /// </returns>
        public virtual IEnumerable<Thing> AfterGet(IEnumerable<Thing> resultCollection, NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            return resultCollection;
        }

        /// <summary>
        /// Resolve a collection of ids to their respective DTO instances from the cache.
        /// </summary>
        /// <param name="resolvables">
        /// An Enumerable collection resolvable Ids.
        /// </param>
        /// <returns>
        /// An Enumerable collection of <see cref="Thing"/>.
        /// </returns>
        /// <exception cref="ContainmentException">
        /// Throws <see cref="ContainmentException"/> if a resolvable id was not present in the cache
        /// </exception>
        protected IEnumerable<Thing> ResolveFromRequestCache(IEnumerable<Guid> resolvables)
        {
            return resolvables
                .Select(resolvable => this.RequestUtils.Cache.SingleOrDefault(x => x.Iid == resolvable))
                .Where(dto => dto != null);
        }

        /// <summary>
        /// Resolve a collection of ordered item ids to their respective DTO instances from the cache.
        /// </summary>
        /// <param name="resolvables">
        /// An enumerable collection of <see cref="OrderedItem"/> for which to resolve the Id.
        /// </param>
        /// <returns>
        /// An Enumerable collection of <see cref="OrderedItem"/>.
        /// </returns>
        /// <exception cref="ContainmentException">
        /// Throws <see cref="ContainmentException"/> if a resolvable id was not present in the cache
        /// </exception>
        protected IEnumerable<OrderedItem> ResolveFromRequestCache(IEnumerable<OrderedItem> resolvables)
        {
            foreach (var resolvable in resolvables)
            {
                var dto = this.RequestUtils.Cache.SingleOrDefault(x => x.Iid == Guid.Parse(resolvable.V.ToString()));
                if (dto != null)
                {
                    yield return new OrderedItem { K = resolvable.K, V = dto };
                }
            }
        }

        /// <summary>
        /// Authorize a read request based on the type name and security context.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The <see cref="ISecurityContext"/>.
        /// </returns>
        protected ISecurityContext AuthorizeReadRequest(string typeName, ISecurityContext containerSecurityContext, string partition)
        {
            if (this.TransactionManager.IsFullAccessEnabled())
            {
                return new RequestSecurityContext { ContainerReadAllowed = true };
            }

            var authorized = this.PermissionService.CanRead(typeName, containerSecurityContext, partition);
            return new RequestSecurityContext { ContainerReadAllowed = authorized };
        }

        /// <summary>
        /// Check whether a read operation is allowed based on the object instance.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        ///  <param name="thing">
        /// The Thing to authorize a read request.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool IsInstanceReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            return this.TransactionManager.IsFullAccessEnabled() || this.PermissionService.CanRead(transaction, thing, partition);
        }

        /// <summary>
        /// Check whether a modify operation is allowed based on the object instance.
        /// </summary>
        /// <param name="transaction">
        /// The transaction to the database.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ActionItem"/> to delete.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool IsInstanceModifyAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation)
        {
            return this.TransactionManager.IsFullAccessEnabled() || this.PermissionService.CanWrite(transaction, thing, thing.GetType().Name, partition, modifyOperation, new RequestSecurityContext { ContainerWriteAllowed = true });
        }
    }
}