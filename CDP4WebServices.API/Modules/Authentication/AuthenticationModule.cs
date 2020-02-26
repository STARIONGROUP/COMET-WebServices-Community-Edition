// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationModule.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using CDP4WebService.Authentication;
    using CDP4WebServices.API.Services.ContributorsLocation;

    using Nancy;
    using Nancy.Responses;
    using Nancy.Security;

    /// <summary>
    /// The authentication module handler. Handles routes to do with authentication settings.
    /// </summary>
    public class AuthenticationModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationModule"/> class.
        /// </summary>
        /// <param name="webServiceAuthentication">
        /// The web Service Authentication.
        /// </param>
        public AuthenticationModule(ICDP4WebServiceAuthentication webServiceAuthentication)
        {
            this.Get["/login"] = _ =>
                {
                    if (this.Context.CurrentUser == null)
                    {
                        return HttpStatusCode.Unauthorized;
                    }

                    // Identify user's location and save this data
                    // TODO revise geo ip service, it does not work properly, that is why it is commented
                    // this.ContributorLocationResolver.GetLocationDataAndSave();

                    return HttpStatusCode.Accepted;
                };

            this.Get["/logout"] = _ =>
                {
                   return  webServiceAuthentication.LogOutResponse(this.Context);
                };
        }

        /// <summary>
        /// Gets or sets the contributor location resolver.
        /// </summary>
        public IContributorLocationResolver ContributorLocationResolver { get; set; }
    }
}