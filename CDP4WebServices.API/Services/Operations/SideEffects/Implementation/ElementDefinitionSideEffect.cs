// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ElementDefinitionSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ElementDefinitionSideEffect : OperationSideEffect<ElementDefinition>
    {
        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService"/>
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService"/>
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ElementDefinition"/> instance that will be inspected.
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
            ElementDefinition thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ContainedElement"))
            {
                var containedElementsId = (IEnumerable<Guid>)rawUpdateInfo["ContainedElement"];

                var elementDefinitions = this.ElementDefinitionService
                    .Get(transaction, partition, null, securityContext).Cast<ElementDefinition>().ToList();

                var elementUsages = this.ElementUsageService.Get(transaction, partition, null, securityContext)
                    .Cast<ElementUsage>().ToList();

                // Check every contained element that it is acyclic
                foreach (var containedElementId in containedElementsId)
                {
                    if (!this.IsElementDefinitionAcyclic(
                            elementDefinitions,
                            elementUsages,
                            containedElementId,
                            thing.Iid))
                    {
                        throw new ArgumentException(
                            string.Format(
                                "ElementDefinition {0} {1} cannot have an ElementUsage {2} that leads to cyclic dependency",
                                thing.Name,
                                thing.Iid,
                                containedElementId));
                    }
                }
            }
        }

        /// <summary>
        /// Is element definition acyclic.
        /// </summary>
        /// <param name="elementDefinitions">
        /// Available element definitions from iteration.
        /// </param>
        /// <param name="elementUsages">
        /// Available element usages from iteration.
        /// </param>
        /// <param name="containedElementId">
        /// The element usage id to check for being acyclic.
        /// </param>
        /// <param name="elementDefinitionId">
        /// The element definition id to set element usage to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied element usage will not lead to cyclic dependency.
        /// </returns>
        private bool IsElementDefinitionAcyclic(
            List<ElementDefinition> elementDefinitions,
            List<ElementUsage> elementUsages,
            Guid containedElementId,
            Guid elementDefinitionId)
        {
            var elementUsage = elementUsages.Find(x => x.Iid == containedElementId);
            if (elementUsage.ElementDefinition != elementDefinitionId)
            {
                if (!this.IsReferencedElementDefinitionAcyclic(
                    elementDefinitions,
                    elementUsages,
                    elementUsage.ElementDefinition,
                    elementDefinitionId))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Is referenced element definition acyclic.
        /// </summary>
        /// <param name="elementDefinitions">
        /// Available element definitions from iteration.
        /// </param>
        /// <param name="elementUsages">
        /// Available element usages from iteration.
        /// </param>
        /// <param name="referencedElementDefinitionId">
        /// The referenced element definition id to check for cyclic dependency.
        /// </param>
        /// <param name="elementDefinitionId">
        /// The root element definition id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether referenced element definition will not lead to cyclic dependency.
        /// </returns>
        private bool IsReferencedElementDefinitionAcyclic(
            List<ElementDefinition> elementDefinitions,
            List<ElementUsage> elementUsages,
            Guid referencedElementDefinitionId,
            Guid elementDefinitionId)
        {
            var elementDefinition = elementDefinitions.Find(x => x.Iid == referencedElementDefinitionId);

            foreach (var elementUsageId in elementDefinition.ContainedElement)
            {
                var elementUsage = elementUsages.Find(x => x.Iid == elementUsageId);
                if (elementUsage.ElementDefinition != elementDefinitionId)
                {
                    if (!this.IsReferencedElementDefinitionAcyclic(
                        elementDefinitions,
                        elementUsages,
                        elementUsage.ElementDefinition,
                        elementDefinitionId))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}