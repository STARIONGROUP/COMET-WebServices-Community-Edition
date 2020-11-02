// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4WspDatabaseAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{

    /// <summary>
    /// Defines mechanism for update/store wsp server salts
    /// </summary>
    public interface IUpdateConfiguration
    {
        /// <summary>
        /// Update salt list inside config.json(WSP server specific)
        /// </summary>
        /// <param name="salt"></param>
        void UpdateSaltList(string salt);
    }

}
