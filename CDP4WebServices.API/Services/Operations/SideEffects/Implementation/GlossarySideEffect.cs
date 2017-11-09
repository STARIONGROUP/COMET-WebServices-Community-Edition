// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlossarySideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System.Linq;

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
        public override void AfterUpdate(Glossary glossary, Thing container, Glossary originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (!glossary.IsDeprecated)
            {
                return;
            }

            if (!glossary.Term.Any())
            {
                return;
            }

            if (!this.PermissionService.CanWrite(transaction, originalThing, typeof(Term).Name, partition, "update", securityContext))
            {
                return;
            }
            
            var terms = this.TermService.GetShallow(transaction, partition, glossary.Term, securityContext)
                .Where(i => i.GetType() == typeof(Term)).Cast<Term>();
            foreach (var term in terms)
            {
                term.IsDeprecated = true;
                this.TermService.UpdateConcept(transaction, partition, term, glossary);
            }
        }
    }
}
