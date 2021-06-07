// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantSideEffect.cs" company="RHEA System S.A.">
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

    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParticipantSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public class ParticipantSideEffect : OperationSideEffect<Participant>
    {
        /// <summary>
        /// The selected domain key.
        /// </summary>
        private const string SelectedDomainKey = "SelectedDomain";

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
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
        /// <param name="rawUpdateInfo">
        /// The update info that was serialized directly from the user request. 
        /// The raw <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be edited likely: it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(Participant thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            this.ValidateSelectedDomain(thing, rawUpdateInfo);
        }

        /// <summary>
        /// Checks whether a valid selected domain is supplied <see cref="Participant"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The update info that was serialized directly from the user request. 
        /// The raw <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be edited likely: it can/will change the operation processor outcome.
        /// </param>
        private void ValidateSelectedDomain(Participant thing, ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey(SelectedDomainKey))
            {
                Guid selectedDomainUpdate = default;
                if (rawUpdateInfo[SelectedDomainKey] != null)
                {
                    Guid.TryParse(rawUpdateInfo[SelectedDomainKey].ToString(), out selectedDomainUpdate);
                }

                if (!thing.Domain.Contains(selectedDomainUpdate))
                {
                    throw new InvalidOperationException("Participant selected domain must be contained in participant domain list.");
                }
            }
        }
    }
}
