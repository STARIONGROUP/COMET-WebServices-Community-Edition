// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupService.cs" company="RHEA System S.A.">
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
    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="ParameterGroupService"/>
    /// </summary>
    public partial class ParameterGroupService
    {
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
        public override void Copy(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo,
            Dictionary<Guid, Guid> sourceToCopyMap, IReadOnlyList<ReferenceDataLibrary> rdls, EngineeringModelSetup targetEngineeringModelSetup, ISecurityContext securityContext)
        {
            if (!(sourceThing is ParameterGroup sourceParameterGroup))
            {
                throw new InvalidOperationException("The source is not of the right type");
            }

            var copy = sourceParameterGroup.DeepClone<ParameterGroup>();
            copy.Iid = sourceToCopyMap[sourceParameterGroup.Iid];

            if (copy.ContainingGroup.HasValue)
            {
                copy.ContainingGroup = sourceToCopyMap[copy.ContainingGroup.Value];
            }

            this.ParameterGroupDao.Write(transaction, partition, copy, targetContainer);
        }
    }
}
