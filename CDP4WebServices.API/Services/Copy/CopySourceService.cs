// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopySourceService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using Authorization;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using Helpers;
    using Npgsql;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A service that handles data access to the source of a copy operation (of iteration data)
    /// </summary>
    public class CopySourceService : ICopySourceService
    {
        /// <summary>
        /// Gets or sets the <see cref="ICdp4TransactionManager"/>
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets the source data for the copy operation
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="copyinfo">The <see cref="CopyInfo"/></param>
        /// <param name="requestPartition">The contextual partition</param>
        /// <returns>The source data</returns>
        public IReadOnlyList<Thing> GetCopySourceData(NpgsqlTransaction transaction, CopyInfo copyinfo, string requestPartition)
        {
            if (copyinfo.Source.IterationId.HasValue && copyinfo.Source.IterationId.Value != Guid.Empty)
            {
                var topcontainerPartition = $"EngineeringModel_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";
                this.TransactionManager.SetIterationContext(transaction, topcontainerPartition, copyinfo.Source.IterationId.Value);
            }

            // transaction shall be contextual to source iteration if applicable (get new transaction)
            var partition = copyinfo.Source.IterationId.HasValue && copyinfo.Source.IterationId.Value != Guid.Empty
                ? $"Iteration_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}"
                : $"EngineeringModel_{copyinfo.Source.TopContainer.Iid.ToString().Replace("-", "_")}";

            var readservice = this.ServiceProvider.MapToReadService(copyinfo.Source.Thing.ClassKind.ToString());

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true, ContainerWriteAllowed = true };

            var sourceThings = copyinfo.Options.CopyKind == CopyKind.Deep
                ? readservice.GetDeep(transaction, partition, new[] {copyinfo.Source.Thing.Iid}, securityContext)
                : readservice.GetShallow(transaction, partition, new[] {copyinfo.Source.Thing.Iid}, securityContext);

            var source = sourceThings.ToList();

            // also get all referenced element-definition as they dont exist in target iteration
            if (copyinfo.Source.IterationId.Value != copyinfo.Target.IterationId.Value)
            {
                source.AddRange(this.GetElementDefinitionTreeFromRootDefinition(transaction, partition, securityContext, source, readservice, source.Select(x => x.Iid).ToList()));
            }

            // revert context to current
            this.TransactionManager.SetIterationContext(transaction, requestPartition, copyinfo.Target.IterationId.Value);

            return source;
        }

        /// <summary>
        /// Generates the copy references
        /// </summary>
        /// <param name="allSourceThings">The source <see cref="Thing"/></param>
        /// <returns>The identifier maps</returns>
        public Dictionary<Guid, Guid> GenerateCopyReference(IReadOnlyList<Thing> allSourceThings)
        {
            var map = new Dictionary<Guid, Guid>();
            foreach (var thing in allSourceThings)
            {
                map.Add(thing.Iid, Guid.NewGuid());
            }

            return map;
        }

        /// <summary>
        /// Gets Additional <see cref="Thing"/> related to the <see cref="ElementUsage"/> to copy
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="source">The current source that may contain <see cref="ElementUsage"/> to copy</param>
        /// <param name="readservice">The element-definition read service</param>
        /// <param name="allSourcesId">A list containing the identifier of all things to copy</param>
        /// <returns>A list containing additional <see cref="Thing"/> to copy</returns>guid
        private IReadOnlyList<Thing> GetElementDefinitionTreeFromRootDefinition(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyList<Thing> source, IReadService readservice, IReadOnlyList<Guid> allSourcesId)
        {
            var additionalSources = new List<Thing>();
            var usages = source.OfType<ElementUsage>().ToArray();
            if (usages.Length > 0)
            {
                var getResults = readservice.GetDeep(transaction, partition, usages.Select(x => x.ElementDefinition).Distinct().Where(x => !allSourcesId.Contains(x)).ToArray(), securityContext).ToList();
                additionalSources.AddRange(getResults);
                additionalSources.AddRange(this.GetElementDefinitionTreeFromRootDefinition(transaction, partition, securityContext, getResults, readservice, allSourcesId.Union(getResults.Select(x => x.Iid)).ToList()));
            }

            return additionalSources;
        }
    }
}
