// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserValidator.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Authentication
{
    using System;
    using System.Linq;

    using CDP4Authentication;

    using CometServer.Authentication;
    using CometServer.Configuration;

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
                transaction?.Dispose();

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
        /// <returns>True if any authenticator passes.</returns>
        private bool Authenticate(Credentials credentials, string password)
        {
            return
                this.AuthenticationPluginInjector.Connectors.Where(c => c.Properties.IsEnabled)
                    .Any(authenticatorConnector => authenticatorConnector.Authenticate(credentials.Person, password));
        }
    }
}