// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainFileStoreService.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Security;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The handcoded part of the <see cref="DomainFileStore"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class DomainFileStoreService
    {
        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>.
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> that uses a <see cref="Type"/> as it's key and a <see cref="Func{TResult}"/> that returns a <see cref="Func{TResult}"/>
        /// that can be used as a predicate on a list of <see cref="DomainFileStore"/>s.
        /// </summary>
        private readonly Dictionary<Type, Func<Guid, Func<DomainFileStore, bool>>> domainFileStoreSelectors = new()
        {
            {
                typeof(File),
                guid => 
                    domainfilestore => 
                        domainfilestore.File.Select(y => y.ToString()).Contains(guid.ToString())
            },
            {
                typeof(Folder),
                guid => 
                    domainfilestore => 
                        domainfilestore.Folder.Select(y => y.ToString()).Contains(guid.ToString())
            },
            {
                typeof(DomainFileStore),
                guid =>
                    domainfilestore =>
                        domainfilestore.Iid.ToString().Equals(guid.ToString())
            },
        };

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
        protected override bool IsInstanceModifyAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation)
        {
            var result = base.IsInstanceModifyAllowed(transaction, thing, partition, modifyOperation);

            if (result)
            {
                this.CheckAllowedAccordingToIsHidden(transaction, thing);
            }

            return result;
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
        protected override bool IsInstanceReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            var result = base.IsInstanceReadAllowed(transaction, thing, partition);

            if (result)
            {
                result = this.IsAllowedAccordingToIsHidden(transaction, thing);
            }

            return result;
        }

        /// <summary>
        /// Checks if the <see cref="Participant"/> is allowed to read (and therefore also write to) a <see cref="DomainFileStore"/>
        /// based on the state of the <see cref="DomainFileStore"/>'s <see cref="DomainFileStore.IsHidden"/> property.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to check.
        /// </param>
        /// <remarks>
        /// Throws an error if <see cref="DomainFileStore"/> is hidden and the current user does not have ownership.
        /// </remarks>
        public void CheckAllowedAccordingToIsHidden(IDbTransaction transaction, Thing thing)
        {
            if (!this.IsAllowedAccordingToIsHidden(transaction, thing))
            {
                throw new ThingNotFoundException("Resource not found");
            }
        }

        /// <summary>
        /// Checks if the <see cref="Participant"/> is allowed to read (and therefore also write to) a <see cref="DomainFileStore"/>
        /// based on the state of the <see cref="DomainFileStore"/>'s <see cref="DomainFileStore.IsHidden"/> property.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to check.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAllowedAccordingToIsHidden(IDbTransaction transaction, Thing thing)
        {
            if (thing is DomainFileStore domainFileStore)
            {
                if (domainFileStore.IsHidden && !this.PermissionService.IsOwner(transaction as NpgsqlTransaction, domainFileStore))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check the security related functionality
        /// </summary>
        /// <param name="thing">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <remarks>
        /// Without a domainFileStoreSelector, SingleOrDefault(domainFileStoreSelector) could fail, because multiple <see cref="DomainFileStore"/>s could exist.
        /// Also if a new DomainFileStore including Files and Folders are created in the same webservice call, then GetShallow for the new DomainFileStores might not return
        /// the to be created <see cref="DomainFileStore"/>. The isHidden check will then be ignored.
        /// </remarks>
        public void HasWriteAccess(Thing thing, IDbTransaction transaction, string partition)
        {
            //am I owner of the file?
            if (!this.PermissionService.IsOwner(transaction as NpgsqlTransaction, thing))
            {
                throw new SecurityException($"The person {this.CredentialsService.Credentials.Person.UserName} does not have an appropriate permission for {thing.GetType().Name}.");
            }

            if (partition.Contains("Iteration"))
            {
                var thingType = thing.GetType();
                
                if (!this.domainFileStoreSelectors.TryGetValue(thingType, out var value))
                {
                    throw new Cdp4ModelValidationException($"Incompatible ClassType found when checking DomainFileStore security: {thingType.Name}");
                }

                var domainFileStoreSelector = value.Invoke(thing.Iid);

                //is DomainFileStore hidden
                var engineeringModelPartition = partition.Replace(
                    CDP4Orm.Dao.Utils.IterationSubPartition,
                    CDP4Orm.Dao.Utils.EngineeringModelPartition);

                var iteration = this.IterationService.GetActiveIteration(transaction as NpgsqlTransaction, engineeringModelPartition, new RequestSecurityContext());

                var domainFileStore =
                    this.GetShallow(transaction as NpgsqlTransaction, partition, iteration.DomainFileStore, new RequestSecurityContext { ContainerReadAllowed = true })
                        .Cast<DomainFileStore>()
                        .SingleOrDefault(domainFileStoreSelector);

                if (domainFileStore != null && !this.IsAllowedAccordingToIsHidden(transaction, domainFileStore))
                {
                    throw new SecurityException($"{nameof(DomainFileStore)} {domainFileStore.Name ?? "<No Name>"} is a private {nameof(DomainFileStore)}");
                }
            }
        }

        /// <summary>
        /// Check the security related functionality
        /// </summary>
        /// <param name="thing">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        public bool HasReadAccess(Thing thing, IDbTransaction transaction, string partition)
        {
            if (!partition.Contains("Iteration"))
            {
                throw new Cdp4ModelValidationException("Wrong partition was resolved for the DomainFileStore");
            }

            var thingType = thing.GetType();
            
            if (!this.domainFileStoreSelectors.TryGetValue(thingType, out var value))
            {
                throw new Cdp4ModelValidationException($"Incompatible ClassType found when checking DomainFileStore security: {thingType.Name}");
            }

            var domainFileStoreSelector = value.Invoke(thing.Iid);

            //is DomainFileStore hidden
            var engineeringModelPartition = partition.Replace(
                CDP4Orm.Dao.Utils.IterationSubPartition,
                CDP4Orm.Dao.Utils.EngineeringModelPartition);

            var iteration = this.IterationService.GetActiveIteration(transaction as NpgsqlTransaction, engineeringModelPartition, new RequestSecurityContext());

            var domainFileStore =
                this.GetShallow(transaction as NpgsqlTransaction, partition, iteration.DomainFileStore, new RequestSecurityContext())
                    .Cast<DomainFileStore>()
                    .SingleOrDefault(domainFileStoreSelector);

            return domainFileStore != null && this.IsAllowedAccordingToIsHidden(transaction, domainFileStore);
        }
    }
}
