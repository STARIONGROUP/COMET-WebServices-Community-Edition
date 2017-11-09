// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetaInfoProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    /// <summary>
    /// The Service Registry interface.
    /// </summary>
    public interface IMetaInfoProvider : IMetaDataProvider
    {
        /// <summary>
        /// Returns a factory info instance based on the passed in <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance.
        /// </param>
        /// <returns>
        /// A concrete factory info instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If type name not supported
        /// </exception>
        IMetaInfo GetMetaInfo(Thing thing);
    }
}