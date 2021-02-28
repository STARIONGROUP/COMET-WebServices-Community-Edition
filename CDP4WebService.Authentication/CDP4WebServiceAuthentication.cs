// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CDP4WebServiceAuthentication.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.Text;

    using NLog;

    /// <summary>
    /// The CDP 4 web service authentication. Mimics Nancy.Authentication.Basic with extended support for cookies.
    /// </summary>
    /// <remarks>
    /// <see ref="https://github.com/NancyFx/Nancy/blob/master/src/Nancy.Authentication.Basic/BasicAuthentication.cs"/>
    /// Nancy source distributed under MIT license.
    /// </remarks>
    public class CDP4WebServiceAuthentication : ICDP4WebServiceAuthentication
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The scheme name. Used by Nancy
        /// </summary>
        private const string Scheme = "Basic";

        /// <summary>
        /// The session cache expiry time in minutes.
        /// </summary>
        private readonly int sessionCacheExpiry = 30;

        /// <summary>
        /// The current configuration.
        /// </summary>
        private CDP4WebServiceAuthenticationConfiguration currentConfiguration;

        /// <summary>
        /// The cookie session manager
        /// </summary>
        private ICookieSessionManager cookieSessionManager;

        /// <summary>
        /// The CDP4 authentication cookie name
        /// </summary>
        private string cdp4AuthenticationCookieName = "_cdp4a";

        /// <summary>
        /// Gets or sets the CDP4 authentication cookie name
        /// </summary>
        public string CDP4AuthenticationCookieName
        {
            get
            {
                return this.cdp4AuthenticationCookieName;
            }

            set
            {
                this.cdp4AuthenticationCookieName = value;
            }
        }

        /// <summary>
        /// Enables basic authentication for the application
        /// </summary>
        /// <param name="pipelines">
        /// Pipelines to add handlers to (usually "this")
        /// </param>
        /// <param name="configuration">
        /// Forms authentication configuration
        /// </param>
        /// <param name="basicAuthenticationPaths">
        /// The basic Authentication Paths. Each supplied path shall be preceded by a '/' character.
        /// </param>
        public void Enable(
            IPipelines pipelines,
            CDP4WebServiceAuthenticationConfiguration configuration,
            IEnumerable<string> basicAuthenticationPaths)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.currentConfiguration = configuration;
            this.cookieSessionManager = new CookieSessionManager();

            pipelines.BeforeRequest.AddItemToStartOfPipeline(this.GetCredentialRetrievalHook);
            pipelines.AfterRequest.AddItemToEndOfPipeline(
                context => this.GetAuthenticationPromptHook(context, basicAuthenticationPaths));
        }

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details from the authentication header.
        /// </summary>
        /// <param name="context">
        /// The current <see cref="NancyContext"/>
        /// </param>
        /// <returns>
        /// null response
        /// </returns>
        public Response GetCredentialRetrievalHook(NancyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var cookieValue = this.GetAuthenticatedUserFromCookie(context);

            if (!string.IsNullOrEmpty(cookieValue))
            {
                var cachedSession = this.cookieSessionManager.Cache.Get(cookieValue);
                if (cachedSession != null)
                {
                    context.CurrentUser = cachedSession as IUserIdentity;
                    return null;
                }
            }

            this.RetrieveCredentials(context);
            return null;
        }

        /// <summary>
        /// Creates a response that sets the authentication cookie and redirects
        /// the user back to where they came from.
        /// </summary>
        /// <param name="context">
        /// Current context
        /// </param>
        /// <param name="userIdentity">
        /// User identifier name
        /// </param>
        /// <param name="cookieExpiry">
        /// Optional expiry date for the cookie (for 'Remember me')
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        public Response SetUserLoggedInRedirectResponse(
            NancyContext context,
            IUserIdentity userIdentity,
            DateTime? cookieExpiry = null)
        {
            var response = context.Response;

            var authenticationCookie = this.BuildCookie(userIdentity.UserName, cookieExpiry);
            context.Response = response.WithCookie(authenticationCookie);

            // set cookie to the session cache
            this.cookieSessionManager.Cache.Set(
                authenticationCookie.Value,
                userIdentity,
                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(this.sessionCacheExpiry) });

            return response;
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <param name="context">
        /// Current context
        /// </param>
        /// <returns>
        /// Nancy response
        /// </returns>
        public Response LogOutResponse(NancyContext context)
        {
            var response = (Response)HttpStatusCode.NoContent;
            var userName = context.CurrentUser != null
                               ? context.CurrentUser.UserName
                               : string.Empty;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                // remove the existing cookie
                if (this.EvictCredentialFromCache(userName))
                {
                    Logger.Info("User '{0}' has logged out", userName);
                }
            }

            var authenticationCookie = this.BuildLogoutCookie(this.currentConfiguration);
            response.WithCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context">
        /// Current context
        /// </param>
        /// <param name="redirectUrl">
        /// URL to redirect to
        /// </param>
        /// <returns>
        /// Nancy response
        /// </returns>
        public Response LogOutAndRedirectResponse(NancyContext context, string redirectUrl)
        {
            var response = context.GetRedirect(redirectUrl);
            var userName = context.CurrentUser != null
                               ? context.CurrentUser.UserName
                               : string.Empty;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                // remove the existing cookie
                if (this.EvictCredentialFromCache(userName))
                {
                    Logger.Info("User '{0}' has logged out", userName);
                }
            }
            
            var authenticationCookie = this.BuildLogoutCookie(this.currentConfiguration);
            response.WithCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// The evict credential from cache.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// True if the credentials were successfully evicted.
        /// </returns>
        public bool EvictCredentialFromCache(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    // nothing to do
                    return false;
                }

                // construct the cache key that points to the cached credential entry
                var cacheKey = this.EncryptAndSignCookie(userName);

                // try remove the user credentials from the cache
                this.cookieSessionManager.Cache.Remove(cacheKey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Invalidate all entries in the credential cache.
        /// </summary>
        public void ResetCredentialCache()
        {
            this.cookieSessionManager.ResetCache();
        }

        /// <summary>
        /// Retrieves the credentials.
        /// </summary>
        /// <param name="context">
        /// The current <see cref="NancyContext"/>.
        /// </param>
        private void RetrieveCredentials(NancyContext context)
        {
            var credentials = this.ExtractCredentialsFromHeaders(context.Request);

            if (credentials == null || credentials.Length != 2)
            {
                // nothing to do as the credentials are not supplied
                return;
            }

            var user = this.currentConfiguration.UserValidator.Validate(credentials[0], credentials[1]);
            if (user != null)
            {
                context.CurrentUser = user;
            }
        }

        /// <summary>
        /// Gets the authentication prompt hook.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="basicAuthenticationPaths">
        /// The basic Authentication Paths.
        /// </param>
        private void GetAuthenticationPromptHook(NancyContext context, IEnumerable<string> basicAuthenticationPaths)
        {
            if (context.Response.StatusCode == HttpStatusCode.Unauthorized && this.SendAuthenticateResponseHeader(context))
            {
                if (basicAuthenticationPaths.Any(basicPath => context.Request.Path.StartsWith(basicPath)))
                {
                    // SiteDirectory or EngineeringModel
                    context.Response.Headers["WWW-Authenticate"] = string.Format("{0} realm=\"{1}\"", Scheme, this.currentConfiguration.Realm);
                }
                else
                {
                    // any other path: unauthorized status
                    context.Response = (Response)HttpStatusCode.Unauthorized;
                }
            }
            else if ((context.Response.StatusCode == HttpStatusCode.OK || context.Response.StatusCode == HttpStatusCode.Accepted) && context.CurrentUser != null)
            {
                this.SetUserLoggedInRedirectResponse(context, context.CurrentUser);
            }
        }

        /// <summary>
        /// Gets the authenticated user name from the incoming request cookie if it exists
        /// and is valid.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>
        /// Returns user name, or string.Empty if not present or invalid
        /// </returns>
        private string GetAuthenticatedUserFromCookie(NancyContext context)
        {
            if (!context.Request.Cookies.ContainsKey(this.cdp4AuthenticationCookieName))
            {
                return string.Empty;
            }

            var cookieValueEncrypted = context.Request.Cookies[this.cdp4AuthenticationCookieName];

            return !string.IsNullOrEmpty(cookieValueEncrypted)
                       ? this.DecryptAndValidateAuthenticationCookie(cookieValueEncrypted)
                       : string.Empty;
        }

        /// <summary>
        /// Extracts credentials from request headers.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The string array with the headers.
        /// </returns>
        private string[] ExtractCredentialsFromHeaders(Request request)
        {
            var authorization = request.Headers.Authorization;
            
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith(Scheme))
            {
                return null;
            }

            try
            {
                var encodedUserPass = authorization.Substring(Scheme.Length).Trim();
                var userPass = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserPass));

                // credentials are in the form user:pass, need to be split
                return !string.IsNullOrWhiteSpace(userPass) 
                    ? userPass.Split(new[] { ':' }, 2) 
                    : null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends the authentication response header.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        private bool SendAuthenticateResponseHeader(NancyContext context)
        {
            var isNonAjaxRequest = this.currentConfiguration.UserPromptBehaviour == UserPromptBehaviour.NonAjax
                                   && !context.Request.IsAjaxRequest();

            var isAlwaysUserPrompt = this.currentConfiguration.UserPromptBehaviour == UserPromptBehaviour.Always;

            return isAlwaysUserPrompt || isNonAjaxRequest;
        }

        /// <summary>
        /// Build the forms authentication cookie
        /// </summary>
        /// <param name="userIdentifier">
        /// Authenticated user identifier
        /// </param>
        /// <param name="cookieExpiry">
        /// Optional expiry date for the cookie (for 'Remember me')
        /// </param>
        /// <returns>
        /// Nancy cookie instance
        /// </returns>
        private INancyCookie BuildCookie(string userIdentifier, DateTime? cookieExpiry)
        {
            var cookieContents = this.EncryptAndSignCookie(userIdentifier);

            var cookie = new NancyCookie(this.cdp4AuthenticationCookieName, cookieContents, true, this.currentConfiguration.RequiresSsl, cookieExpiry);

            if (!string.IsNullOrEmpty(this.currentConfiguration.Domain))
            {
                cookie.Domain = this.currentConfiguration.Domain;
            }

            if (!string.IsNullOrEmpty(this.currentConfiguration.Path))
            {
                cookie.Path = this.currentConfiguration.Path;
            }

            return cookie;
        }

        /// <summary>
        /// Encrypt and sign the cookie contents
        /// </summary>
        /// <param name="cookieValue">
        /// Plain text cookie value
        /// </param>
        /// <returns>
        /// Encrypted and signed string
        /// </returns>
        private string EncryptAndSignCookie(string cookieValue)
        {
            var encryptedCookie = this.currentConfiguration.CryptographyConfiguration.EncryptionProvider.Encrypt(cookieValue);
            var hmacBytes = this.GenerateHmac(encryptedCookie);
            var hmacString = Convert.ToBase64String(hmacBytes);

            return string.Format("{1}{0}", encryptedCookie, hmacString);
        }

        /// <summary>
        /// Generate a HMAC for the encrypted cookie string
        /// </summary>
        /// <param name="encryptedCookie">
        /// Encrypted cookie string
        /// </param>
        /// <returns>
        /// HMAC byte array
        /// </returns>
        private byte[] GenerateHmac(string encryptedCookie)
        {
            return this.currentConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedCookie);
        }

        /// <summary>
        /// Decrypt and validate an encrypted and signed cookie value
        /// </summary>
        /// <param name="cookieValue">
        /// Encrypted and signed cookie value
        /// </param>
        /// <returns>
        /// Decrypted value, or empty on error or if failed validation
        /// </returns>
        private string DecryptAndValidateAuthenticationCookie(string cookieValue)
        {
            var hmacStringLength = Base64Helpers.GetBase64Length(this.currentConfiguration.CryptographyConfiguration.HmacProvider.HmacLength);

            var encryptedCookie = cookieValue.Substring(hmacStringLength);
            var hmacString = cookieValue.Substring(0, hmacStringLength);

            // Check the hmacs, but don't early exit if they don't match
            var hmacBytes = Convert.FromBase64String(hmacString);
            var newHmac = this.GenerateHmac(encryptedCookie);
            var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, this.currentConfiguration.CryptographyConfiguration.HmacProvider.HmacLength);

            // Only return the decrypted result if the hmac was ok
            return hmacValid ? cookieValue : string.Empty;
        }

        /// <summary>
        /// Builds a cookie for logging a user out
        /// </summary>
        /// <param name="configuration">
        /// Current configuration
        /// </param>
        /// <returns>
        /// Nancy cookie instance
        /// </returns>
        private INancyCookie BuildLogoutCookie(CDP4WebServiceAuthenticationConfiguration configuration)
        {
            var cookie = new NancyCookie(
                             this.cdp4AuthenticationCookieName,
                             string.Empty,
                             true,
                             this.currentConfiguration.RequiresSsl,
                             DateTime.Now.AddDays(-1));

            if (!string.IsNullOrEmpty(configuration.Domain))
            {
                cookie.Domain = configuration.Domain;
            }

            if (!string.IsNullOrEmpty(configuration.Path))
            {
                cookie.Path = configuration.Path;
            }
            
            return cookie;
        }
    }
}
