// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonFileStoreService.cs" company="Starion Group S.A.">
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
    
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    /// <summary>
    /// The handcoded part of the <see cref="CommonFileStore"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class CommonFileStoreService
    {
        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelService"/>.
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> that uses a <see cref="Type"/> as it's key and a <see cref="Func{TResult}"/> that returns a <see cref="Func{TResult}"/>
        /// that can be used as a predicate on a list of <see cref="CommonFileStore"/>s.
        /// </summary>
        private readonly Dictionary<Type, Func<Guid, Func<CommonFileStore, bool>>> commonFileStoreSelectors = new()
        {
            {
                typeof(File),
                guid => 
                    commonfilestore => 
                        commonfilestore.File.Select(y => y.ToString()).Contains(guid.ToString())
            },
            {
                typeof(Folder),
                guid => 
                    commonfilestore => 
                        commonfilestore.Folder.Select(y => y.ToString()).Contains(guid.ToString())
            },
            {
                typeof(CommonFileStore),
                guid =>
                    commonfilestore =>
                        commonfilestore.Iid.ToString().Equals(guid.ToString())
            },
        };

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
        /// Without a commonFileStoreSelector, SingleOrDefault(commonFileStoreSelector) could fail, because multiple <see cref="CommonFileStore"/>s could exist.
        /// Also if a new CommonFileStore including Files and Folders are created in the same webservice call, then GetShallow for the new DommonFileStores might not return
        /// the to be created <see cref="CommonFileStore"/>. The isHidden check will then be ignored.
        /// </remarks>
        public void HasWriteAccess(Thing thing, IDbTransaction transaction, string partition)
        {
            if (!partition.Contains("EngineeringModel"))
            {
                throw new Cdp4ModelValidationException("Wrong partition was resolved for the CommonFileStore");
            }

            var thingType = thing.GetType();
                
            if (!this.commonFileStoreSelectors.ContainsKey(thingType))
            {
                throw new Cdp4ModelValidationException($"Incompatible ClassType found when checking CommonFileStore security: {thingType.Name}");
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
            if (!partition.Contains("EngineeringModel"))
            {
                throw new Cdp4ModelValidationException("Wrong partition was resolved for the CommonFileStore");
            }

            var thingType = thing.GetType();
            
            if (!this.commonFileStoreSelectors.ContainsKey(thingType))
            {
                throw new Cdp4ModelValidationException($"Incompatible ClassType found when checking CommonFileStore security: {thingType.Name}");
            }

            return true;
        }
    }
}
