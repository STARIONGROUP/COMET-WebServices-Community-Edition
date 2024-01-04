// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceLoader.cs" company="RHEA System S.A.">
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

namespace CometServer.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    /// <summary>
    /// Class responsible for loading embedded resources.
    /// </summary>
    public class ResourceLoader : IResourceLoader
    {
        /// <summary>
        /// Load an embedded resource
        /// </summary>
        /// <param name="path">
        /// The path of the embedded resource
        /// </param>
        /// <returns>
        /// a string containing the contents of the embedded resource
        /// </returns>
        public string LoadEmbeddedResource(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(path);

            using var reader = new StreamReader(stream ?? throw new MissingManifestResourceException());

            return reader.ReadToEnd();
        }

        /// <summary>
        /// queries the version number from the executing assembly
        /// </summary>
        /// <returns>
        /// a string representation of the version of the application
        /// </returns>
        public string QueryVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (infoVersion != null)
            {
                var plusIndex = infoVersion.InformationalVersion.IndexOf('+');

                if (plusIndex != -1)
                {
                    return infoVersion.InformationalVersion.Substring(0, plusIndex);
                }

                return infoVersion.InformationalVersion;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fileVersion = fileVersionInfo.FileVersion;
            return fileVersion;
        }

        /// <summary>
        /// queries the supported model versions from the executing assembly
        /// </summary>
        /// <returns>
        /// a collection of string representations of the supported model versions of the application
        /// </returns>
        public IEnumerable<Version> QueryModelVersions()
        {
            var versions = new MetaDataProvider().QuerySupportedModelVersions();

            return versions;
        }

        /// <summary>
        /// queries the version number from the CDP4Common library
        /// </summary>
        /// <returns>
        /// a string representation of the version of the CDP4-COMET SDK
        /// </returns>
        public string QuerySDKVersion()
        {
            var assembly = typeof(Thing).Assembly;
            return assembly.GetName().Version?.ToString();
        }

        /// <summary>
        /// queries the template HTML of the root page
        /// </summary>
        /// <returns>
        /// a string representation of the template HTML of the root page
        /// </returns>
        public string QueryRootPage()
        {
            var rootPage = this.LoadEmbeddedResource("CometServer.Resources.RootPage.html");

            return rootPage;
        }

        /// <summary>
        /// Queries the logo with version info from the embedded resources
        /// </summary>
        /// <returns>
        /// the logo
        /// </returns>
        public string QueryLogo()
        {
            var version = this.QueryVersion();

            var logo = this.LoadEmbeddedResource("CometServer.Resources.ascii-art.txt")
                .Replace("COMETWebServicesVersion", version);

            return logo;
        }
    }
}
