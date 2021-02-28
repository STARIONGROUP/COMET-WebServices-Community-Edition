// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPluginInjector.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using CDP4Authentication;

    /// <summary>
    /// The injector loads up authenticator plugins.
    /// </summary>
    public class AuthenticationPluginInjector : IAuthenticationPluginInjector
    {
        /// <summary>
        /// The name of the folder where all authentication modules reside.
        /// </summary>
        private const string AuthenticatorPluginFolder = "Authentication";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPluginInjector"/> class.
        /// </summary>
        public AuthenticationPluginInjector()
        {
            this.Plugins = this.LoadPlugins().ToList();
        }

        /// <summary>
        /// Gets the list of plugins.
        /// </summary>
        public List<IAuthenticatorPlugin> Plugins { get; private set; }

        /// <summary>
        /// Gets the connectors
        /// </summary>
        public List<IAuthenticatorConnector> Connectors
        {
            get
            {
                return
                    this.Plugins.SelectMany(p => p.Connectors)
                        .OrderByDescending(c => c.Properties.Rank).ToList();
            }
        }

        /// <summary>
        /// Gets the list of folders from which to load the authentication plugins.
        /// </summary>
        /// <returns>The list of directories which contain the authenticator plugins.</returns>
        private static List<string> GetFolders()
        {
            return Directory.GetDirectories(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, AuthenticatorPluginFolder)).ToList();
        }

        /// <summary>
        /// Retrieves all plugins that inherit the <see cref="IAuthenticatorPlugin"/> interface
        /// </summary>
        /// <returns>The <see cref="List{T}"/> of <see cref="IAuthenticatorPlugin"/> modules</returns>
        private IEnumerable<IAuthenticatorPlugin> LoadPlugins()
        {
            var pluginFolders = GetFolders();
            var container = new ContainerBuilder().Build();

            // load all assemblies types encountered in the plugin folders that implement the IAuthenticatorPlugin interface
            container.Update(
                builder =>
                    {
                        foreach (var pluginFolder in pluginFolders)
                        {
                            foreach (var assembly in new DirectoryInfo(pluginFolder).GetFiles().Where(file => file.Extension == ".dll")
                                    .Select(file => Assembly.LoadFile(file.FullName)))
                            {
                                builder.RegisterAssemblyTypes(assembly)
                                    .Where(x => typeof(IAuthenticatorPlugin).IsAssignableFrom(x))
                                    .AsImplementedInterfaces()
                                    .PropertiesAutowired()
                                    .SingleInstance();
                            }
                        }
                    });

            foreach (var authenticatorPlugin in container.Resolve<IEnumerable<IAuthenticatorPlugin>>())
            {
                yield return authenticatorPlugin;
            }
        }
    }
}
