// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPersonAuthenticator.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
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
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using CDP4Orm.Dao.Authentication;

    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="AuthenticationPersonAuthenticator"/> is to authenticate a <see cref="AuthenticationPerson"/>
    /// against the E-TM-10-25 datasource and associated authentication plugin
    /// </summary>
    public class AuthenticationPersonAuthenticator : IAuthenticationPersonAuthenticator
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        public ILogger<AuthenticationPersonAuthenticator> Logger { get; set; }

        /// <summary>
        /// Gets or sets the authentication plugin injector that holds all the authentication plugins.
        /// </summary>
        public IAuthenticationPluginInjector AuthenticationPluginInjector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the authentication dao.
        /// </summary>
        public IAuthenticationPersonDao AuthenticationPersonDao { get; set; }

        /// <summary>
        /// Authenticates the <see cref="AuthenticationPerson"/> from the E-TM-10-25 datasource
        /// </summary>
        /// <param name="username">
        /// the username of the <see cref="Person"/> that is to be authenticated
        /// </param>
        /// <param name="password"></param>
        /// the password of the <see cref="Person"/> that is to be authenticated
        /// <returns>
        /// an instance of <see cref="AuthenticationPerson"/> or null if not found
        /// </returns>
        public async Task<AuthenticationPerson> Authenticate(string username, string password)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                connection = new NpgsqlConnection(Utils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, this.AppConfigService.AppConfig.Backtier.Database));
                await connection.OpenAsync();
                transaction = await connection.BeginTransactionAsync();

                var authenticationPerson = (await this.AuthenticationPersonDao.Read(transaction, "SiteDirectory", username, null)).SingleOrDefault();
                
                await transaction.CommitAsync();

                if (authenticationPerson == null)
                {
                    return null;
                }

                if (this.Authenticate(authenticationPerson, password))
                {
                    // here we set the password and salt to null, we don't want this to travel
                    // through the app after the user has been authenticated
                    authenticationPerson.Password = null;
                    authenticationPerson.Salt = null;

                    return authenticationPerson;
                }

                return null;
            }
            catch (Exception ex)
            {
                transaction?.RollbackAsync();

                this.Logger.LogError(ex, "There was an error while authenticating the user credentials");
                return null;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// Authenticates the <see cref="AuthenticationPerson"/> and password against all providers until first pass is found.
        /// </summary>
        /// <param name="authenticationPerson">
        /// The <see cref="AuthenticationPerson"/> to authenticate
        /// </param>
        /// <param name="password">
        /// the password of the <see cref="AuthenticationPerson"/> that is to be authenticated
        /// </param>
        /// <returns>
        /// True when the authentication was sucessful, false if not
        /// </returns>
        private bool Authenticate(AuthenticationPerson authenticationPerson, string password)
        {
            var authenticationConnectors = this.AuthenticationPluginInjector.Connectors
                .Where(c => c.Properties.IsEnabled)
                .OrderBy(c => c.Properties.Rank);

            foreach (var authenticationConnector in authenticationConnectors)
            {
                if (authenticationConnector.Authenticate(authenticationPerson, password))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
