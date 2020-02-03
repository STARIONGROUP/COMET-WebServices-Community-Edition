// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserValidator.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authentication
{
    using System;
    using System.Linq;
    using CDP4Authentication;
    using CDP4WebService.Authentication;
    using CDP4WebServices.API.Configuration;
    using Nancy.Security;
    using NLog;
    using Npgsql;

    /// <summary>
    /// Implements the Nancy Basic Authentication <see cref="IUserValidator"/> that performs
    /// the validation after a login.
    /// </summary>
    public class UserValidator : IUserValidator
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the authentication plugin injector that holds all the authentication plugins.
        /// </summary>
        public IAuthenticationPluginInjector AuthenticationPluginInjector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonResolver"/> that manages retrieving the <see cref="CDP4Common.DTO.Person"/> from the database. 
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }
       
        /// <summary>
        /// Validates the username and password
        /// </summary>
        /// <param name="username">
        /// The supplied authenticating username
        /// </param>
        /// <param name="password">
        /// The supplied password
        /// </param>
        /// <returns>
        /// A value representing the authenticated user, null if the user was not authenticated.
        /// </returns>
        public IUserIdentity Validate(string username, string password)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                connection = new NpgsqlConnection(Utils.GetConnectionString(AppConfig.Current.Backtier.Database));
                connection.Open();
                transaction = connection.BeginTransaction();

                // resolve person
                var credentials = this.PersonResolver.ResolvePerson(transaction, username) as Credentials;

                transaction.Commit();

                // if the person could not be resolved, drop straight away
                if (credentials == null)
                {
                    return null;
                }

                // authenticate against authentication providers
                return this.Authenticate(credentials, password) ? credentials : null;
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "There was an error while authenticating the user credentials");
                return null;
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Authenticates the <see cref="Credentials"/> and password against all providers until first pass is found.
        /// </summary>
        /// <param name="credentials">The resolved user credentials.</param>
        /// <param name="password">The password.</param>
        /// <returns>Try if any authenticator passes.</returns>
        private bool Authenticate(Credentials credentials, string password)
        {
            return
                this.AuthenticationPluginInjector.Connectors.Where(c => c.Properties.IsEnabled)
                    .Any(authenticatorConnector => authenticatorConnector.Authenticate(credentials.Person, password));
        }
    }
}