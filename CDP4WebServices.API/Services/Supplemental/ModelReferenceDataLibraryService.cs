// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="ModelReferenceDataLibraryService"/>
    /// </summary>
    public partial class ModelReferenceDataLibraryService
    {
        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryDao"/>
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDato { get; set; }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="iteration"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <returns>The <see cref="ReferenceDataLibrary"/></returns>
        public IEnumerable<ReferenceDataLibrary> QueryReferenceDataLibrary(NpgsqlTransaction transaction, Iteration iteration)
        {
            var engineeringModelSetup = this.EngineeringModelSetupDao.Read(transaction, typeof(SiteDirectory).Name).FirstOrDefault(ems => ems.IterationSetup.Contains(iteration.IterationSetup));
            if (engineeringModelSetup == null)
            {
                throw new InvalidOperationException($"Could not find the associated engineering-modem-setup for iteration {iteration.Iid}");
            }

            var mrdl = this.ModelReferenceDataLibraryDao.Read(transaction, typeof(SiteDirectory).Name, engineeringModelSetup.RequiredRdl).FirstOrDefault();
            if (mrdl == null)
            {
                throw new InvalidOperationException($"Could not find the associated reference-data-library for iteration {iteration.Iid}");
            }

            var list = new List<ReferenceDataLibrary> {mrdl};
            list.AddRange(this.GetRequiredRdl(transaction, mrdl));
            return list;
        }

        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="rdl"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="rdl">The <see cref="ReferenceDataLibrary"/></param>
        /// <returns>The required <see cref="ReferenceDataLibrary"/></returns>
        private IReadOnlyList<ReferenceDataLibrary> GetRequiredRdl(NpgsqlTransaction transaction, ReferenceDataLibrary rdl)
        {
            var requiredRdls = new List<ReferenceDataLibrary>();

            if (!rdl.RequiredRdl.HasValue)
            {
                return requiredRdls;
            }

            var requiredRdl = (ReferenceDataLibrary)this.ModelReferenceDataLibraryDao.Read(transaction, typeof(SiteDirectory).Name, new [] { rdl.RequiredRdl.Value }).FirstOrDefault();
            if (requiredRdl != null)
            {
                requiredRdls.Add(requiredRdl);
                requiredRdls.AddRange(this.GetRequiredRdl(transaction, requiredRdl));
            }
            else
            {
                requiredRdl = this.SiteReferenceDataLibraryDato.Read(transaction, typeof(SiteDirectory).Name, new[] { rdl.RequiredRdl.Value }).FirstOrDefault();
                if (requiredRdl != null)
                {
                    requiredRdls.Add(requiredRdl);
                }
            }

            return requiredRdls;
        }
    }
}
