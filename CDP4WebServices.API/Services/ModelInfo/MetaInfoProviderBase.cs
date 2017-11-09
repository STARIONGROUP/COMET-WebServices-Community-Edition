// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaInfoProviderBase.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    /// <summary>
    /// The meta info provider base class.
    /// </summary>
    public abstract class MetaInfoProviderBase
    {
        /// <summary>
        /// Gets the default model version.
        /// </summary>
        protected string DefaultModelVersion
        {
            get
            {
                return "1.0.0";
            }
        }
    }
}
