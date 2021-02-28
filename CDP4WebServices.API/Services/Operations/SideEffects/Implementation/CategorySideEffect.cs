// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategorySideEffect.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="CategorySideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class CategorySideEffect : OperationSideEffect<Category>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICategoryService"/>
        /// </summary>
        public ICategoryService CategoryService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Category"/> instance that will be inspected.
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
            Category thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("SuperCategory"))
            {
                var superCategoriesId = (IEnumerable<Guid>)rawUpdateInfo["SuperCategory"];

                // Check for itself in super categories list
                if (superCategoriesId.Contains(thing.Iid))
                {
                    throw new AcyclicValidationException(
                        string.Format("Category {0} {1} cannot have itself as a SuperCategory", thing.Name, thing.Iid));
                }

                // Get RDL chain and collect categories' ids
                var categoryIdsFromChain = this.GetCategoryIdsFromRdlChain(
                    transaction,
                    partition,
                    securityContext,
                    ((ReferenceDataLibrary)container).RequiredRdl);
                categoryIdsFromChain.AddRange(((ReferenceDataLibrary)container).DefinedCategory);

                // Check that super categories are present in the chain
                foreach (var superCategoryId in superCategoriesId)
                {
                    if (!categoryIdsFromChain.Contains(superCategoryId))
                    {
                        throw new AcyclicValidationException(
                            string.Format(
                                "Category {0} {1} cannot have a SuperCategory from outside the RDL chain",
                                thing.Name,
                                thing.Iid));
                    }
                }

                var categories = this.CategoryService.Get(transaction, partition, categoryIdsFromChain, securityContext)
                    .Cast<Category>().ToList();

                // Check every super category that it is acyclic
                foreach (var superCategoryId in superCategoriesId)
                {
                    if (!this.IsSuperCategoryAcyclic(categories, superCategoryId, thing.Iid))
                    {
                        throw new AcyclicValidationException(
                            string.Format(
                                "Category {0} {1} cannot have a SuperCategory {2} that leads to cyclic dependency",
                                thing.Name,
                                thing.Iid,
                                superCategoryId));
                    }
                }
            }
        }

        /// <summary>
        /// Get category ids from an rdl chain.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rdlId">
        /// The Iid of RDL to start from.
        /// </param>
        /// <returns>
        /// The list of Category ids.
        /// </returns>
        private List<Guid> GetCategoryIdsFromRdlChain(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? rdlId)
        {
            var availableRdls = this.SiteReferenceDataLibraryService.Get(transaction, partition, null, securityContext)
                .Cast<SiteReferenceDataLibrary>().ToList();
            var categoryIds = new List<Guid>();
            var requiredRdl = rdlId;

            while (requiredRdl != null)
            {
                var rdl = availableRdls.Find(x => x.Iid == requiredRdl);
                categoryIds.AddRange(rdl.DefinedCategory);
                requiredRdl = rdl.RequiredRdl;
            }

            return categoryIds;
        }

        /// <summary>
        /// Is super category acyclic.
        /// </summary>
        /// <param name="categories">
        /// The categories from RDL chain.
        /// </param>
        /// <param name="superCategoryId">
        /// The super category id to check for being acyclic.
        /// </param>
        /// <param name="categoryId">
        /// The category id to set super category to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied super category will not lead to cyclic dependency.
        /// </returns>
        private bool IsSuperCategoryAcyclic(List<Category> categories, Guid superCategoryId, Guid categoryId)
        {
            var superCategoryTreeList = new List<Guid>();
            this.SetSuperCategoryTree(categories, superCategoryTreeList, superCategoryId);

            return !superCategoryTreeList.Contains(categoryId);
        }

        /// <summary>
        /// Set super category tree to the supplied list.
        /// </summary>
        /// <param name="categories">
        /// The categories  from RDL chain.
        /// </param>
        /// <param name="superCategoryTreeList">
        /// The super category tree list to set found ids to.
        /// </param>
        /// <param name="superCategoryId">
        /// The super category id to set tree of ids for.
        /// </param>
        private void SetSuperCategoryTree(List<Category> categories, List<Guid> superCategoryTreeList, Guid superCategoryId)
        {
            var superCategory = categories.Find(x => x.Iid == superCategoryId);

            foreach (var id in superCategory.SuperCategory)
            {
                this.SetSuperCategoryTree(categories, superCategoryTreeList, id);
            }

            superCategoryTreeList.Add(superCategoryId);
        }
    }
}