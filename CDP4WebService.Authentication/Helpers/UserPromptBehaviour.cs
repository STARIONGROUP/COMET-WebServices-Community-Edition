// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPromptBehaviour.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebService.Authentication
{
    /// <summary>
    /// Options to control when the browser prompts the user for credentials
    /// </summary>
    public enum UserPromptBehaviour
    {
        /// <summary>
        /// Never present user with login prompt
        /// </summary>
        Never,

        /// <summary>
        /// Always present user with login prompt
        /// </summary>
        Always,

        /// <summary>
        /// Only prompt the user for credentials on non-ajax requests
        /// </summary>
        NonAjax
    }
}
