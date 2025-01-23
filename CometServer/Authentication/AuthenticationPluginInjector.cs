﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPluginInjector.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Collections.ObjectModel;
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
    public class AuthenticationPluginInjector : IAuthenticationPluginInjector, IDisposable
    {
        /// <summary>
        /// A collection of <see cref="Assembly" /> that has been discovered
        /// </summary>
        private List<Assembly> discoveredAssemblies = new List<Assembly>();

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
            AppDomain.CurrentDomain.AssemblyResolve += this.OnAssemblyResolve;

            this.Plugins = this.LoadPlugins();

            foreach (var authenticatorConnector in this.Plugins.SelectMany(authenticatorPlugin => authenticatorPlugin.Connectors))
            {
                logger.LogInformation(authenticatorConnector.IsUp ? "The {name} is loaded and is Up" : "The {name} is loaded and is Down", authenticatorConnector.Name);
            }
        }

        /// <summary>
        /// Gets the list of plugins.
        /// </summary>
        public ReadOnlyCollection<IAuthenticatorPlugin> Plugins { get; private set; }

        /// <summary>
        /// Gets the connectors
        /// </summary>
        public IEnumerable<IAuthenticatorConnector> Connectors
        {
            get
            {
                return
                    this.Plugins.SelectMany(p => p.Connectors)
                        .OrderByDescending(c => c.Properties.Rank);
            }
        }

        /// <summary>
        /// Gets the list of folders from which to load the authentication plugins.
        /// </summary>
        /// <returns>
        /// The list of directories which contain the authenticator plugins.
        /// </returns>
        private static string[] GetFolders()
        {
            return Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath ?? Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, AuthenticatorPluginFolder)).ToArray();
        }

        /// <summary>
        /// Retrieves all plugins that inherit the <see cref="IAuthenticatorPlugin"/> interface
        /// </summary>
        /// <returns>The <see cref="List{T}"/> of <see cref="IAuthenticatorPlugin"/> modules</returns>
        private ReadOnlyCollection<IAuthenticatorPlugin> LoadPlugins()
        {
            this.discoveredAssemblies.Clear();
            var sw = Stopwatch.StartNew();

            var result = new List<IAuthenticatorPlugin>();

            this.logger.LogInformation("Loading authentication plugins");

            var pluginFolders = GetFolders();

            var builder = new ContainerBuilder();

            // load all assemblies types encountered in the plugin folders that implement the IAuthenticatorPlugin interface
            foreach (var pluginFolder in pluginFolders)
            {
                var assemblies = new DirectoryInfo(pluginFolder).GetFiles().Where(file => file.Extension == ".dll")
                    .Select(file => Assembly.LoadFile(file.FullName))
                    .ToList();

                foreach (var assembly in assemblies)
                {
                    builder.RegisterAssemblyTypes(assembly)
                        .Where(x => typeof(IAuthenticatorPlugin).IsAssignableFrom(x))
                        .AsImplementedInterfaces()
                        .PropertiesAutowired()
                        .SingleInstance();
                }

                this.discoveredAssemblies.AddRange(assemblies);
            }

            var container = builder.Build();

            foreach (var authenticatorPlugin in container.Resolve<IEnumerable<IAuthenticatorPlugin>>())
            {
                result.Add(authenticatorPlugin);
            }

            this.logger.LogInformation("Authentication plugins loaded in {sw} [ms]", sw.ElapsedMilliseconds);

            return result.AsReadOnly();
        }
        
        /// <summary>
        /// Helps resolving assemblies that may be required for loaded plugins
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/></param>
        /// <returns>The resolved assemblies, if found</returns>
        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return this.discoveredAssemblies.FirstOrDefault(x => x.FullName == args.Name);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= this.OnAssemblyResolve;
        }
    }
}
    
