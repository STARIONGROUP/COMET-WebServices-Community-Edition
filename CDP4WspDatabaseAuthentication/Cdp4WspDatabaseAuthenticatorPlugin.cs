// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4WspDatabaseAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication
{
    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    /// <summary>
    /// Defines an authentication plugin against the standard CDP4 database using wsp hash algorithm.
    /// </summary>
    public class Cdp4WspDatabaseAuthenticatorPlugin : AuthenticatorPlugin<AuthenticatorWspProperties, Cdp4WspDatabaseAuthenticatorConnector>
    {
    }
}
