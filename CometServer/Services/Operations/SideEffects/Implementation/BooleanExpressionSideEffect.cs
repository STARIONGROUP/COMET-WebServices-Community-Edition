// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanExpressionSideEffect.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Exceptions;
    using CometServer.Services.Authorization;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Abstract super class from which all BooleanExpression classes derive.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type T that must be of type <see cref="BooleanExpression"/>
    /// </typeparam>
    public abstract class BooleanExpressionSideEffect<T> : OperationSideEffect<T>
        where T : BooleanExpression
    {
        /// <summary>
        /// Gets or sets the <see cref="IParametricConstraintService"/>
        /// </summary>
        public IParametricConstraintService ParametricConstraintService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="T"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="CDP4Common.DTO.Thing"/> that is inspected.
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
        public override Task BeforeUpdate(
            T thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.TryGetValue("Term", out var value))
            {
                List<Guid> termsId;

                switch (thing.ClassKind)
                {
                    case ClassKind.AndExpression:
                        {
                            termsId = (List<Guid>)value;
                            break;
                        }

                    case ClassKind.OrExpression:
                        {
                            termsId = (List<Guid>)value;
                            break;
                        }

                    case ClassKind.ExclusiveOrExpression:
                        {
                            termsId = (List<Guid>)value;
                            break;
                        }

                    case ClassKind.NotExpression:
                        {
                            termsId = [(Guid)value];
                            break;
                        }

                    default:
                        {
                            termsId = [];
                            break;
                        }
                }

                // Check for itself
                if (termsId.Contains(thing.Iid))
                {
                    throw new AcyclicValidationException($"BooleanExpression {thing.Iid} cannot have itself as a term.");
                }

                // Check that every term is present in the parametric constraint
                foreach (var id in termsId)
                {
                    if (!((ParametricConstraint)container).Expression.Contains(id))
                    {
                        throw new AcyclicValidationException(
                            $"BooleanExpression {thing.Iid} cannot have a term from outside the parametric constraint.");
                    }
                }

                // Get all BooleanExpressions
                var expressions = this.ParametricConstraintService
                    .GetDeep(transaction, partition, new List<Guid> { container.Iid }, securityContext)
                    .Where(x => x is BooleanExpression).Cast<BooleanExpression>().ToList();

                // Check every term that it is acyclic
                foreach (var termId in termsId)
                {
                    if (!IsTermAcyclic(expressions, termId, thing.Iid))
                    {
                        throw new AcyclicValidationException(
                            $"BooleanExpression {thing.Iid} cannot have a BooleanExpression {termId} that leads to cyclic dependency");
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Is term acyclic.
        /// </summary>
        /// <param name="expressions">
        /// The expressions from parametric constraint.
        /// </param>
        /// <param name="termId">
        /// The term id to check for being acyclic.
        /// </param>
        /// <param name="parentTermId">
        /// The parent term id to set a term to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied term will not lead to cyclic dependency.
        /// </returns>
        private static bool IsTermAcyclic(List<BooleanExpression> expressions, Guid termId, Guid parentTermId)
        {
            var termTreeList = new List<Guid>();
            SetTermTreeList(expressions, termTreeList, termId);

            return !termTreeList.Contains(parentTermId);
        }

        /// <summary>
        /// Set term tree to the supplied list.
        /// </summary>
        /// <param name="expressions">
        /// The expressions from parametric constraint.
        /// </param>
        /// <param name="termTreeList">
        /// The term tree list to set found ids to.
        /// </param>
        /// <param name="termId">
        /// The term id to set tree of ids for.
        /// </param>
        private static void SetTermTreeList(List<BooleanExpression> expressions, List<Guid> termTreeList, Guid termId)
        {
            var term = expressions.Find(x => x.Iid == termId);
            List<Guid> referencedTermsId;

            switch (term.ClassKind)
            {
                case ClassKind.AndExpression:
                    {
                        referencedTermsId = ((AndExpression)term).Term;
                        break;
                    }

                case ClassKind.OrExpression:
                    {
                        referencedTermsId = ((OrExpression)term).Term;
                        break;
                    }

                case ClassKind.ExclusiveOrExpression:
                    {
                        referencedTermsId = ((ExclusiveOrExpression)term).Term;
                        break;
                    }

                case ClassKind.NotExpression:
                    {
                        referencedTermsId = [((NotExpression)term).Term];
                        break;
                    }

                default:
                    {
                        referencedTermsId = [];
                        break;
                    }
            }

            foreach (var id in referencedTermsId)
            {
                SetTermTreeList(expressions, termTreeList, id);
            }

            termTreeList.Add(termId);
        }
    }
}