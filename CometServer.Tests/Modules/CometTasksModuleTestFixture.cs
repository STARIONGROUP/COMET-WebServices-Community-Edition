// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometTasksModuleTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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


namespace CometServer.Tests.Modules
{
    using System.Linq;

    using CometServer.Authorization;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Modules.Tasks;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="CometTasksModule"/> routes
    /// </summary>
    [TestFixture]
    public class CometTasksModuleTestFixture
    {
        [Test]
        public void VerifyThatEveryTaskRouteRequiresAuthorization()
        {
            // without authorization metadata the authentication handlers do not run, and every request is answered
            // with 401 "not authenticated", so a client can never poll its task
            var builder = WebApplication.CreateBuilder();

            // the route handlers take an ICredentialsService, which minimal APIs only bind when it is a registered service
            builder.Services.AddScoped(_ => new Mock<ICredentialsService>().Object);

            var application = builder.Build();

            var module = new CometTasksModule(NullLogger<CometTasksModule>.Instance, new Mock<ICometHasStartedService>().Object, new Mock<ICometTaskService>().Object, new Mock<IDataSource>().Object);
            module.AddRoutes(application);

            var endpoints = ((IEndpointRouteBuilder)application).DataSources.SelectMany(x => x.Endpoints).OfType<RouteEndpoint>().ToList();

            Assert.That(endpoints.Select(x => x.RoutePattern.RawText), Is.EquivalentTo(new[] { "/tasks", "/tasks/{taskId:guid}", "/tasks/{taskId:guid}" }));

            foreach (var endpoint in endpoints)
            {
                var policies = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Select(x => x.Policy).ToArray();

                Assert.That(policies, Is.EquivalentTo(ApiBase.AuthenticationSchemes), endpoint.DisplayName);
            }
        }
    }
}
