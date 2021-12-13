// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServer.IIS
{
    using CDP4WebServices.API.Configuration;

    using Microsoft.Owin.Extensions;

    using Owin;

    /// <summary>
    /// Provides the entry point for the ASP.NET application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Specifies how the ASP.NET application will respond to individual HTTP requests.
        /// </summary>
        /// <param name="app">
        /// Application pipeline
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            // load application configuration from file
            AppConfig.Load();

            app.UseNancy();
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}
