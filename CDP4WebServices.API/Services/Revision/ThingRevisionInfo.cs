// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;
    using CDP4Orm.Dao.Revision;

    /// <summary>
    /// A class that concatenate a <see cref="Thing"/> with a <see cref="RevisionInfo"/>
    /// </summary>
    internal class ThingRevisionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingRevisionInfo"/> class
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="revisionInfo">The associated <see cref="RevisionInfo"/></param>
        public ThingRevisionInfo(Thing thing, RevisionInfo revisionInfo)
        {
            this.Thing = thing;
            this.RevisionInfo = revisionInfo;
        }

        /// <summary>
        /// Gets or sets the <see cref="Thing"/>
        /// </summary>
        public Thing Thing { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="RevisionInfo"/>
        /// </summary>
        public RevisionInfo RevisionInfo { get; private set; }
    }
}