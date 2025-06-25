﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlossarySideEffect.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="GlossarySideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class GlossarySideEffect : OperationSideEffect<Glossary>
    {
        /// <summary>
        /// Gets or sets the <see cref="ITermService"/>.
        /// </summary>
        public ITermService TermService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="glossary">
        /// The <see cref="Glossary"/> instance that will be inspected.
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
        /// <remarks>
        /// When a <see cref="Glossary"/> is deprecated the <see cref="Term"/>s that are contained by that <see cref="Glossary"/> shall be deprecated as well.
        /// When the <see cref="Person"/> does not have the permission to write to <see cref="Term"/>s, this will be ignored.
        /// </remarks>
        public override Task AfterUpdate(Glossary glossary, Thing container, Glossary originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (!glossary.IsDeprecated)
            {
                return Task.CompletedTask;
            }

            if (glossary.Term.Count == 0)
            {
                return Task.CompletedTask;
            }

            if (!this.PermissionService.CanWrite(transaction, originalThing, nameof(Term), partition, "update", securityContext))
            {
                return Task.CompletedTask;
            }

            var terms = this.TermService.GetShallow(transaction, partition, glossary.Term, securityContext)
                .Where(i => i.GetType() == typeof(Term)).Cast<Term>();

            foreach (var term in terms)
            {
                term.IsDeprecated = true;
                this.TermService.UpdateConcept(transaction, partition, term, glossary);
            }

            return Task.CompletedTask;
        }
    }
}
