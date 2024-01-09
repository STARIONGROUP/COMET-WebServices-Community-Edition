// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace VersionFileCreator
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    [ExcludeFromCodeCoverage]
    internal class Program
    {
        /// <summary>
        /// Main entry for the versionFileCreator, handles exception and notify the user in the output build window of visual studio
        /// </summary>
        /// <param name="args">Array containing all command line arguments</param>
        public static async Task<int> Main(string[] args)
        {
            try
            {
                string path = null;
                var consolePrefix = "VersionFileCreator =>";

                Console.WriteLine($"{consolePrefix} Checking path variable.");

                if (args == null || !args.Any(Directory.Exists))
                {
                    path = Directory.GetCurrentDirectory();
                }
                else
                {
                    path = args.FirstOrDefault(Directory.Exists);
                }

                Console.WriteLine($"{consolePrefix} Path: {path}");

                var outputFolder = await GetOutputFolderFromArgs(args);

                Console.WriteLine($"{consolePrefix} Build configuration: {outputFolder}");

                var versionFile = Path.Combine(path, outputFolder, "VERSION");
                Console.WriteLine($"{consolePrefix} Version file location: {versionFile}");

                if (!File.Exists(versionFile))
                {
                    Console.WriteLine($"{consolePrefix} Version file {versionFile} was NOT FOUND.");
                    return -1;
                }

                Console.WriteLine($"{consolePrefix} Version file {versionFile} was found.");

                var versionData = await File.ReadAllTextAsync(versionFile);
                versionData = versionData.Replace("\r", "");
                var versionArray = versionData.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                if (versionArray.Length == 0)
                {
                    Console.WriteLine($"{consolePrefix} No version indicators found in {versionFile}.");
                    return -1;
                }

                var newVersionArray = new string[versionArray.Length];
                var newVersionArrayCounter = 0;

                foreach (var versionRow in versionArray)
                {
                    Console.WriteLine($"{consolePrefix} Find Version for file: {versionRow}");

                    var searchFile = Path.Combine(path, outputFolder, versionRow);

                    if (!File.Exists(searchFile))
                    {
                        Console.WriteLine($"{consolePrefix} File {searchFile} was NOT FOUND!.");
                        return -1;
                    }

                    var fileVersion = QueryVersion(searchFile);

                    newVersionArray[newVersionArrayCounter] = $"{Path.GetFileNameWithoutExtension(new FileInfo(searchFile).Name)}: {fileVersion}";

                    Console.WriteLine($"{consolePrefix} File {searchFile} version is {fileVersion}.");

                    newVersionArrayCounter++;
                }

                Console.WriteLine($"{consolePrefix} Writing version information to {versionFile}.");
                await File.WriteAllTextAsync(versionFile, string.Join("\n", newVersionArray));
                Console.WriteLine($"{consolePrefix} Version information was written to {versionFile}.");

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return -1;
            }
        }

        /// <summary>
        /// Gets the Build Configuration (Debug/Release) from the command line arguments
        /// </summary>
        /// <param name="args">The array of command line arguments</param>
        /// <returns>Build Configuration when found in arguments, otherwise null</returns>
        private static async Task<string> GetOutputFolderFromArgs(string[] args)
        {
            var configParameterPosition = Array.FindIndex(args, x => x.StartsWith("path:"));

            return
                configParameterPosition >= 0
                    ? args[configParameterPosition].Split(new[] { ':' }, StringSplitOptions.None)[1]
                    : null;
        }

        /// <summary>
        /// queries the version number from an assembly
        /// </summary>
        /// <param name="assemblyPath">
        /// The location of the <see cref="Assembly"/>
        /// </param>
        /// <returns>
        /// a string representation of the version of the assembly
        /// </returns>
        private static string QueryVersion(string assemblyPath)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
            var productVersion = fileVersionInfo.ProductVersion;

            if (productVersion != null)
            {
                var plusIndex = productVersion.IndexOf('+');

                if (plusIndex != -1)
                {
                    return productVersion.Substring(0, plusIndex);
                }
            }

            return productVersion ?? "unknown";
        }
    }
}