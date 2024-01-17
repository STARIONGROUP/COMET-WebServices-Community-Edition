// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="RHEA System S.A.">
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

namespace CometServer
{
    using System;
    using System.Linq;

    using Autofac;
    
    using Carter;

    using CDP4Authentication;

    using CDP4Common.Helpers;
    using CDP4Common.MetaInfo;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Authentication;
    using CDP4Orm.Dao.Cache;
    using CDP4Orm.Dao.Resolve;
    using CDP4Orm.Dao.Revision;
    using CDP4Orm.Helper;
    using CDP4Orm.MigrationEngine;

    using CometServer.Authentication;
    using CometServer.Authorization;
    using CometServer.ChangeNotification;
    using CometServer.ChangeNotification.Data;
    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Resources;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.CherryPick;
    using CometServer.Services.DataStore;
    using CometServer.Services.Email;
    using CometServer.Services.Operations;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Supplemental;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    
    using Prometheus;

    using Serilog;

    /// <summary>
    /// The <see cref="Startup"/> used to configure the application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The Cookie Scheme used by the COMET API
        /// </summary>
        public const string CookieScheme = "CDP4";

        /// <summary>
        /// The (injected) <see cref="IWebHostEnvironment"/> that provides access to environment variables
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="environment">
        /// The <see cref="IWebHostEnvironment"/> of the application
        /// </param>
        public Startup(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">
        /// 
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHangfire(config =>
                config.UseMemoryStorage());

            services.AddHangfireServer();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.WithExposedHeaders("*");
                    });
            });

            services.AddAuthentication("CDP4").AddCookie(CookieScheme);

            services.AddCarter();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<CometStartUpService>().As<IHostedService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CometHasStartedService>().As<ICometHasStartedService>().SingleInstance();
            builder.RegisterType<AppConfigService>().As<IAppConfigService>().SingleInstance();
            builder.RegisterType<AuthenticationPluginInjector>().As<IAuthenticationPluginInjector>().SingleInstance();
            builder.RegisterType<ResourceLoader>().As<IResourceLoader>().SingleInstance();
            builder.RegisterType<TokenGeneratorService>().As<ITokenGeneratorService>().SingleInstance();

            // 10-25 helpers
            builder.RegisterType<DataModelUtils>().As<IDataModelUtils>().SingleInstance();
            builder.RegisterType<DefaultPermissionProvider>().As<IDefaultPermissionProvider>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(IMetaInfo).Assembly).Where(x => typeof(IMetaInfo).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<MetaInfoProvider>().As<IMetaInfoProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<Cdp4JsonSerializer>().As<ICdp4JsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<MessagePackSerializer>().As<IMessagePackSerializer>().InstancePerLifetimeScope();

            // authentication services
            builder.RegisterType<AuthenticationPersonDao>().As<IAuthenticationPersonDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationPersonAuthenticator>().As<IAuthenticationPersonAuthenticator>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // authentication services
            builder.RegisterType<CredentialsService>().As<ICredentialsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // authorization services
            builder.RegisterType<PermissionService>().As<IPermissionService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<AccessRightKindService>().As<IAccessRightKindService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ObfuscationService>().As<IObfuscationService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<OrganizationalParticipationResolverService>().As<IOrganizationalParticipationResolverService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<AccessRightKindValidationService>().As<IAccessRightKindValidationService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<PermissionInstanceFilterService>().As<IPermissionInstanceFilterService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            
            // request-response services
            builder.RegisterType<HeaderInfoProvider>().As<IHeaderInfoProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            var carterModulessInAssembly = typeof(Startup).Assembly.GetExportedTypes().Where(type => typeof(CarterModule).IsAssignableFrom(type)).ToArray();
            builder.RegisterTypes(carterModulessInAssembly).PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<Services.ServiceProvider>().As<CometServer.Services.IServiceProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<RequestUtils>().As<IRequestUtils>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<OperationSideEffectProcessor>().As<IOperationSideEffectProcessor>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<OperationProcessor>().As<IOperationProcessor>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            //  database services
            builder.RegisterType<MigrationEngine>().As<IMigrationEngine>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DataStoreConnectionChecker>().As<IDataStoreConnectionChecker>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<Cdp4TransactionManager>().As<ICdp4TransactionManager>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(BaseDao).Assembly).Where(x => typeof(BaseDao).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<RevisionDao>().As<IRevisionDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ResolveDao>().As<IResolveDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ContainerDao>().As<IContainerDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<CacheService>().As<ICacheService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<CacheDao>().As<ICacheDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<DataStoreController>().As<IDataStoreController>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<RevisionService>().As<IRevisionService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<RevisionResolver>().As<IRevisionResolver>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<DaoResolver>().As<IDaoResolver>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            
            // COMET services
            builder.RegisterType<ResolveService>().As<IResolveService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ServiceBase).Assembly).Where(x => typeof(ServiceBase).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IBusinessLogicService).Assembly).Where(x => typeof(IBusinessLogicService).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IOperationSideEffect).Assembly).Where(x => typeof(IOperationSideEffect).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // COMET extra business logic services
            builder.RegisterType<EmailService>().As<IEmailService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ChangeLogService>().As<IChangeLogService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<JsonExchangeFileReader>().As<IJsonExchangeFileReader>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<JsonExchangeFileWriter>().As<IJsonExchangeFileWriter>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ZipArchiveWriter>().As<IZipArchiveWriter>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ModelCreatorManager>().As<IModelCreatorManager>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<MigrationService>().As<IMigrationService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

            // CherryPick support
            builder.RegisterType<CherryPickService>().As<ICherryPickService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<ContainmentService>().As<IContainmentService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            // change notification
            builder.RegisterType<ChangeNoticationService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<ModelLogEntryDataCreator>().As<IModelLogEntryDataCreator>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<ChangelogBodyComposer>().As<IChangelogBodyComposer>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            if (this.environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/errorhandler");
            }

            app.UseSerilogRequestLogging();

            app.UseStaticFiles();

            app.UseHangfireDashboard("/hangfire");

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseBasicAuthenticatonMiddleware();

            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseEndpoints(builder =>
            {
                // map carter routes
                builder.MapCarter();
            });

            appLifetime.ApplicationStarted.Register(() =>
            {
                RecurringJob.AddOrUpdate<ChangeNoticationService>("ChangeNotificationService.Execute", notificationService => notificationService.Execute(), Cron.Weekly(DayOfWeek.Monday, 0, 15));
            });
        }
    }
}
