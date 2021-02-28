// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContributorLocationResolver.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.ContributorsLocation
{
    using System;
    using System.Net.Http;
    using System.Runtime.Caching;

    using CDP4Orm.Dao;

    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services.Authentication;

    using Newtonsoft.Json;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The contributor location resolver.
    /// </summary>
    public class ContributorLocationResolver : IContributorLocationResolver
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the transaction manager for this request.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the CDP4 request context.
        /// </summary>
        public ICdp4RequestContext Context { get; set; }

        /// <summary>
        /// Gets or sets the person dao.
        /// </summary>
        public IPersonDao PersonDao { get; set; }

        /// <summary>
        /// Gets location data of a contributor and saves it into cache.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws when an expected Person could not be retrieved.
        /// </exception>
        public void GetLocationDataAndSave()
        {
            var ip = this.GetUserIpAddress();
            var client = new HttpClient();
            var res = client.GetAsync(AppConfig.Current.Defaults.LocationServicePath + ip);

            if (res.Result.IsSuccessStatusCode)
            {
                var content = res.Result.Content.ReadAsStringAsync().Result;
                var contibutorLocationData = JsonConvert.DeserializeObject<ContributorLocationData>(content);

                var credentials = this.Context.Context.CurrentUser as Credentials;

                NpgsqlConnection connection = null;
                NpgsqlTransaction transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

                if (credentials != null)
                {
                    var personGivenName = this.PersonDao.GivenName(transaction, credentials.Person.Iid);
                    contibutorLocationData.Name = personGivenName;
                    ContributorLocationCache.GetContributorsCache()
                        .Set(
                            credentials.Person.Iid.ToString(),
                            contibutorLocationData,
                            new CacheItemPolicy
                                {
                                    SlidingExpiration =
                                        TimeSpan.FromMinutes(AppConfig.Current.Defaults.ContributorsCacheTimeout)
                                });
                }
            }
            else
            {
                Logger.Warn("Location service did not respond for a request.");
            }
        }

        /// <summary>
        /// Gets a user's IP address.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="HttpRequestException">
        /// Throws when a user's IP address cannot be identified.
        /// </exception>
        private string GetUserIpAddress()
        {
            var ip = this.Context.Context.Request.Headers["HTTP_X_FORWARDED_FOR"].SingleOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = this.Context.Context.Request.Headers["REMOTE_ADDR"].ToString();
            }
            else
            {
                // Using X-Forwarded-For last address
                ip = ip.Split(',').Last().Trim();
            }

            return ip;
        }
    }
}
