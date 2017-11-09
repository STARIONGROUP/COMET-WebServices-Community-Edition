// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootModule.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using Nancy;
    using Nancy.Security;

    /// <summary>
    /// The root module handler.
    /// </summary>
    public class RootModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootModule"/> class.
        /// </summary>
        public RootModule()
        {
            this.RequiresAuthentication();

            this.Get["/"] = _ => this.Response.AsRedirect("/app");
        }
    }
}