// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteReferenceDataLibrarySideEffect.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="SiteReferenceDataLibrarySideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class SiteReferenceDataLibrarySideEffect : OperationSideEffect<SiteReferenceDataLibrary>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override bool BeforeCreate(
            SiteReferenceDataLibrary thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            if (thing.RequiredRdl != null)
            {
                this.ValidateRequiredRdl(
                    thing,
                    container,
                    transaction,
                    partition,
                    securityContext,
                    (Guid)thing.RequiredRdl);
            }

            return true;
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(
            SiteReferenceDataLibrary thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("RequiredRdl"))
            {
                var requiredRdlId = (Guid)rawUpdateInfo["RequiredRdl"];

                this.ValidateRequiredRdl(thing, container, transaction, partition, securityContext, requiredRdlId);
            }
        }

        /// <summary>
        /// Validate required rdl for being acyclic.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="SiteReferenceDataLibrary"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="requiredRdlId">
        /// The required rdl id to check for being acyclic
        /// </param>
        public void ValidateRequiredRdl(
            SiteReferenceDataLibrary thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid requiredRdlId)
        {
            // Check for itself
            if (requiredRdlId == thing.Iid)
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "SiteReferenceDataLibrary {0} {1} cannot have itself as a required SiteReferenceDataLibrary.",
                        thing.Name,
                        thing.Iid));
            }

            // Check that required rdl is from the same siteDirectory
            if (!((SiteDirectory)container).SiteReferenceDataLibrary.Contains(requiredRdlId))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "SiteReferenceDataLibrary {0} {1} cannot have a SiteReferenceDataLibrary from outside the current SiteDirectory.",
                        thing.Name,
                        thing.Iid));
            }

            // Get all rdls from the SiteDirectory
            var rdls = this.SiteReferenceDataLibraryService.Get(
                transaction,
                partition,
                ((SiteDirectory)container).SiteReferenceDataLibrary,
                securityContext).Cast<SiteReferenceDataLibrary>().ToList();

            // Check whether required rdl is acyclic
            if (!this.IsRdlAcyclic(rdls, requiredRdlId, thing.Iid))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "SiteReferenceDataLibrary {0} {1} cannot have a requred rdl {2} that leads to cyclic dependency",
                        thing.Name,
                        thing.Iid,
                        requiredRdlId));
            }
        }

        /// <summary>
        /// Is requred rdl acyclic.
        /// </summary>
        /// <param name="rdls">
        /// The rdls from SiteDirectory.
        /// </param>
        /// <param name="requiredRdlId">
        /// The required rdl to check for being acyclic.
        /// </param>
        /// <param name="rdlId">
        /// The rdl id to set a requred rdl to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether a required rdl will not lead to cyclic dependency.
        /// </returns>
        private bool IsRdlAcyclic(List<SiteReferenceDataLibrary> rdls, Guid requiredRdlId, Guid rdlId)
        {
            Guid? nextRequiredRdlId = requiredRdlId;

            while (nextRequiredRdlId != null)
            {
                if (nextRequiredRdlId == rdlId)
                {
                    return false;
                }

                nextRequiredRdlId = rdls.FirstOrDefault(x => x.Iid == nextRequiredRdlId)?.RequiredRdl;
            }

            return true;
        }
    }
}