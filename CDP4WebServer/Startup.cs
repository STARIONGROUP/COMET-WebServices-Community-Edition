// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A. All rights reserverd
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServer
{
    using Nancy;
    using Nancy.Owin;

    using Owin;

    /// <summary>
    /// Provides the entry point for the ASP.NET application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Specifies how the ASP.NET application will respond to individual HTTP requests.
        /// </summary>
        /// <param name="app">
        /// Application pipeline
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(options => options.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound));
        }
    }
}