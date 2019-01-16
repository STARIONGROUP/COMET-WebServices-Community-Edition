// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authorization;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using Npgsql;
    using Operations.SideEffects;

    /// <summary>
    /// Extension for the code-generated service
    /// </summary>
    public partial class ElementDefinitionService
    {
        /// <summary>
        /// Gets or sets the operation side effect processor.
        /// </summary>
        public IOperationSideEffectProcessor OperationSideEffectProcessor { get; set; }

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
        public override void Copy(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo, Dictionary<Guid, Guid> sourceToCopyMap, IReadOnlyList<ReferenceDataLibrary> rdls, EngineeringModelSetup targetEngineeringModelSetup, ISecurityContext securityContext)
        {
            if (!(sourceThing is ElementDefinition sourceElementDef))
            {
                throw new InvalidOperationException("The source is not of the right type");
            }

            var copy = sourceElementDef.DeepClone<ElementDefinition>();
            copy.Iid = sourceToCopyMap[sourceElementDef.Iid];

            // remove references that are not being copied over
            copy.ReferencedElement.Clear();

            // check reference data validity in target iteration if models are different
            if (copyinfo.Source.TopContainer.Iid != copyinfo.Target.TopContainer.Iid)
            {
                var possibleCats = rdls.SelectMany(x => x.DefinedCategory).ToList();
                var ignoredCategories = copy.Category.Except(possibleCats).ToList();

                foreach (var ignoredCategory in ignoredCategories)
                {
                    copy.Category.Remove(ignoredCategory);
                }
            }

            if (copyinfo.Options.KeepOwner.HasValue 
                && (!copyinfo.Options.KeepOwner.Value
                    || copyinfo.Options.KeepOwner.Value 
                        && !targetEngineeringModelSetup.ActiveDomain.Contains(copy.Owner)
                    )
                )
            {
                copy.Owner = copyinfo.ActiveOwner;
            }

            if (!this.OperationSideEffectProcessor.BeforeCreate(copy, targetContainer, transaction, partition, securityContext))
            {
                return;
            }

            this.ElementDefinitionDao.Write(transaction, partition, copy, targetContainer);

            this.OperationSideEffectProcessor.AfterCreate(copy, targetContainer, null, transaction, partition, securityContext);
            
            // copy contained things
            foreach (var group in allSourceThings.Where(x => sourceElementDef.ParameterGroup.Contains(x.Iid)))
            {
                ((ServiceBase)this.ParameterGroupService).Copy(transaction, partition, @group, copy, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
            }

            foreach (var parameter in allSourceThings.Where(x => sourceElementDef.Parameter.Contains(x.Iid)))
            {
                ((ServiceBase)this.ParameterService).Copy(transaction, partition, parameter, copy, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
            }

            // only copy directly usage if in same iteration, otherwise extra actions are required at a higher level
            if (copyinfo.Source.IterationId.Value == copyinfo.Target.IterationId.Value)
            {
                foreach (var usage in allSourceThings.Where(x => sourceElementDef.ContainedElement.Contains(x.Iid)))
                {
                    ((ServiceBase)this.ElementUsageService).Copy(transaction, partition, usage, copy, allSourceThings, copyinfo, sourceToCopyMap, rdls, targetEngineeringModelSetup, securityContext);
                }
            }
        }
    }
}
