// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieSessionCache.cs" company="RHEA System S.A.">
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
    using System.Collections.Specialized;
    using System.Runtime.Caching;

    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Cache of session cookies.
    /// </summary>
    public class CookieSessionCache : MemoryCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieSessionCache"/> class. 
        /// </summary>
        /// <param name="name">
        /// The name to use to look up configuration information. Note: It is not required for configuration information to exist for every name.If a matching configuration entry exists, the configuration information is used to configure the <see cref="T:System.Runtime.Caching.MemoryCache"/> instance. If a matching configuration entry does not exist, the name can be accessed through the <see cref="P:System.Runtime.Caching.MemoryCache.Name"/> property, because the specified name is associated with the <see cref="T:System.Runtime.Caching.MemoryCache"/> instance. For information about memory cache configuration, see <see cref="T:System.Runtime.Caching.Configuration.MemoryCacheElement"/>.
        /// </param>
        /// <param name="config">
        /// A collection of name/value pairs of configuration information to use for configuring the cache. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="name"/> is an empty string. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The string value "default" (case insensitive) is assigned to <paramref name="name"/>. The value "default" cannot be assigned to a new <see cref="T:System.Runtime.Caching.MemoryCache"/> instance, because the value is reserved for use by the <see cref="P:System.Runtime.Caching.MemoryCache.Default"/> property.
        /// </exception>
        /// <exception cref="T:System.Configuration.ConfigurationException">
        /// A value in the <paramref name="config"/> collection is invalid. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// A name or value in the <paramref name="config"/> parameter could not be parsed.
        /// </exception>
        public CookieSessionCache(string name, NameValueCollection config = null)
            : base(name, config)
        {
        }
    }
}
