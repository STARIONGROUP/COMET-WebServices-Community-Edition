// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsernameModule.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using Nancy;
    using Nancy.Security;

    /// <summary>
    /// handle request on the logged-in users
    /// </summary>
    public class UsernameModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameModule"/> class.
        /// </summary>
        public UsernameModule()
        {
            this.RequiresAuthentication();
            this.Get["/username"] = _ =>
            {
                if (this.Context.CurrentUser == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                return this.Context.CurrentUser.UserName;
            };
        }
    }
}