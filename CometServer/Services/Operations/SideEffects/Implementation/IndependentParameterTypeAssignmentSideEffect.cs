// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndependentParameterTypeAssignmentSideEffect.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DependentParameterTypeAssignmentSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public class IndependentParameterTypeAssignmentSideEffect : OperationSideEffect<IndependentParameterTypeAssignment>
    {
        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICachedReferenceDataService"/>
        /// </summary>
        public ICachedReferenceDataService CachedReferenceDataService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
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
        public override async Task AfterCreate(IndependentParameterTypeAssignment thing, Thing container, IndependentParameterTypeAssignment originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.ResetData();
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
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
        public override async Task AfterDelete(IndependentParameterTypeAssignment thing, Thing container, IndependentParameterTypeAssignment originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.ResetData();
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
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
        public override async Task AfterUpdate(IndependentParameterTypeAssignment thing, Thing container, IndependentParameterTypeAssignment originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.ResetData();
        }

        /// <summary>
        /// Resets all necessary data
        /// </summary>
        private Task ResetData()
        {
            // reset the IDefaultValueArrayFactory, this may be used during the same transaction, any update to the ParameterTypeComponents needs to reset the IDefaultValueArrayFactory cache
            this.DefaultValueArrayFactory.Reset();

            // reset the ICachedReferenceDataService, this may be used during the same transaction, any update to the ParameterTypeComponents needs to reset the IDefaultValueArrayFactory cache
            this.CachedReferenceDataService.Reset();

            return Task.CompletedTask;
        }
    }
}
