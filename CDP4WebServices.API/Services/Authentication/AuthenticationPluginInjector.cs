// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPluginInjector.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Autofac;

    using CDP4Authentication;

    using Nancy.Bootstrappers.Autofac;

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
