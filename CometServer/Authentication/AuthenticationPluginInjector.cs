// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPluginInjector.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using CDP4Authentication;

    using Microsoft.Extensions.Logging;

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
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AuthenticationPluginInjector> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPluginInjector"/> class.
        /// </summary>
        public AuthenticationPluginInjector(ILogger<AuthenticationPluginInjector> logger)
        {
            this.logger = logger;

            this.Plugins = this.LoadPlugins().ToList();

            foreach (var authenticatorConnector in this.Plugins.SelectMany(authenticatorPlugin => authenticatorPlugin.Connectors))
            {
                logger.LogInformation(authenticatorConnector.IsUp ? "The {name} is loaded and is Up" : "The {name} is loaded and is Down", authenticatorConnector.Name);
            }
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
        /// <returns>
        /// The list of directories which contain the authenticator plugins.
        /// </returns>
        private IEnumerable<string> GetFolders()
        {
            return Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath ?? Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, AuthenticatorPluginFolder)).ToList();
        }

        /// <summary>
        /// Retrieves all plugins that inherit the <see cref="IAuthenticatorPlugin"/> interface
        /// </summary>
        /// <returns>The <see cref="List{T}"/> of <see cref="IAuthenticatorPlugin"/> modules</returns>
        private IEnumerable<IAuthenticatorPlugin> LoadPlugins()
        {
            var sw = Stopwatch.StartNew();

            this.logger.LogInformation("Loading authentication plugins");

            var pluginFolders = this.GetFolders();

            var builder = new ContainerBuilder();

            // load all assemblies types encountered in the plugin folders that implement the IAuthenticatorPlugin interface
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

            var container = builder.Build();

            foreach (var authenticatorPlugin in container.Resolve<IEnumerable<IAuthenticatorPlugin>>())
            {
                yield return authenticatorPlugin;
            }

            this.logger.LogInformation("Authentication plugins loaded in {sw} [ms]", sw.ElapsedMilliseconds);
        }
    }
}
