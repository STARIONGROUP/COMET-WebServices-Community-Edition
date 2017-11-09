// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4DatabaseAuthenticatorPlugin.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4DatabaseAuthentication
{
    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    /// <summary>
    /// Defines an authentication plugin against the standard CDP4 database.
    /// </summary>
    public class Cdp4DatabaseAuthenticatorPlugin : AuthenticatorPlugin<AuthenticatorProperties, Cdp4DatabaseAuthenticatorConnector>
    {
    }
}