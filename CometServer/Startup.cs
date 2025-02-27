// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Starion Group S.A.">
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

namespace CometServer
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

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

    using CDP4ServicesMessaging;

    using CometServer.Authentication;
    using CometServer.Authentication.Anonymous;
    using CometServer.Authentication.Basic;
    using CometServer.Authentication.Bearer;
    using CometServer.Authorization;
    using CometServer.ChangeNotification;
    using CometServer.ChangeNotification.Data;
    using CometServer.Configuration;
    using CometServer.Enumerations;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules.Constraints;
    using CometServer.Resources;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.CherryPick;
    using CometServer.Services.DataStore;
    using CometServer.Services.Email;
    using CometServer.Services.Operations;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Supplemental;
    using CometServer.Tasks;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;

    using Prometheus;

    using Serilog;

    /// <summary>
    /// The <see cref="Startup"/> used to configure the application
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// The (injected) <see cref="IWebHostEnvironment"/> that provides access to environment variables
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// The (injected) <see cref="IConfiguration"/> used to read from the appsettings.json file
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="environment">
        /// The <see cref="IWebHostEnvironment"/> of the application
        /// </param>
        /// <param name="configuration">
        /// The (injected) <see cref="IConfiguration"/> used to read from the appsettings.json file
        /// </param>
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>
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

            services.AddRouting(options => options.ConstraintMap.Add("EnumerableOfGuid", typeof(EnumerableOfGuidRouteConstraint)));
            services.ConfigureServicesMessaging();
            SetUpAuthentication(services, this.configuration);
            
            services.AddCarter();
        }

        /// <summary>
        /// setup the authentication schemes as per configuration
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>
        /// </param>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to read the appsettings.json file
        /// </param>
        /// <exception cref="ConfigurationException"></exception>
        private static void SetUpAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var isBasicAuthEnabledString = configuration["Authentication:Basic:IsEnabled"];

            if (string.IsNullOrEmpty(isBasicAuthEnabledString))
            {
                throw new ConfigurationErrorsException("The Authentication:Basic:IsEnabled setting must be available");
            }

            var isBasicAuthEnabled = bool.Parse(isBasicAuthEnabledString);

            var isLocalJwtBearerEnabledString = configuration["Authentication:LocalJwtBearer:IsEnabled"];

            if (string.IsNullOrEmpty(isLocalJwtBearerEnabledString))
            {
                throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:IsEnabled setting must be available");
            }

            var isLocalJwtBearerEnabled = bool.Parse(isLocalJwtBearerEnabledString);

            var isExternalJwtBearerEnabledString = configuration["Authentication:ExternalJwtBearer:IsEnabled"];
            var isExternalJwtBearerEnabled = !string.IsNullOrEmpty(isExternalJwtBearerEnabledString) && bool.Parse(isExternalJwtBearerEnabledString);

            if (!isBasicAuthEnabled && !isLocalJwtBearerEnabled && !isExternalJwtBearerEnabled)
            {
                throw new ConfigurationErrorsException("At least one authentication must be enabled");
            }

            if (isExternalJwtBearerEnabled && (isBasicAuthEnabled || isLocalJwtBearerEnabled))
            {
                throw new ConfigurationErrorsException("Both local and external authentication are enabled, local one is not supported while external authentication is enabled");
            }

            var authenticationBuilder = services
                .AddAuthentication()
                .AddAnonymousAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(configure: options =>
                {
                    options.IsWWWAuthenticateHeaderSuppressed = false;
                    options.Realm = "CDP4-COMET";
                });

            if (isLocalJwtBearerEnabled)
            {
                var validIssuer = configuration["Authentication:LocalJwtBearer:ValidIssuer"];

                if (string.IsNullOrEmpty(validIssuer))
                {
                    throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:ValidIssuer setting must be available");
                }

                var validAudience = configuration["Authentication:LocalJwtBearer:ValidAudience"];

                if (string.IsNullOrEmpty(validAudience))
                {
                    throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:ValidAudience setting must be available");
                }

                var issuerSigningKey = configuration["Authentication:LocalJwtBearer:SymmetricSecurityKey"];

                if (string.IsNullOrEmpty(issuerSigningKey))
                {
                    throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:SymmetricSecurityKey setting must be available");
                }

                var expirationTime = configuration["Authentication:LocalJwtBearer:TokenExpirationMinutes"];

                if (!string.IsNullOrEmpty(expirationTime))
                {
                    if (!int.TryParse(expirationTime, out var tokenExpirationMinutes))
                    {
                        throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:TokenExpirationMinutes setting must an integer value");
                    }

                    if (tokenExpirationMinutes <= 0)
                    {
                        throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:TokenExpirationMinutes setting must be strickly greater than 0");
                    }
                }

                var refreshExpirationTime = configuration["Authentcation:LocalJwtBearer:RefreshExpirationMinutes"];

                if (!string.IsNullOrEmpty(refreshExpirationTime))
                {
                    if (!int.TryParse(refreshExpirationTime, out var refreshExpirationMinutes))
                    {
                        throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:RefreshExpirationMinutes setting must an integer value");
                    }

                    if (refreshExpirationMinutes <= 0)
                    {
                        throw new ConfigurationErrorsException("The Authentication:LocalJwtBearer:RefreshExpirationMinutes setting must be strickly greater than 0");
                    }
                }
            
                authenticationBuilder.AddLocalJwtBearerAuthentication(configure: options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = validIssuer,
                            ValidAudience = validAudience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }
            else
            {
                authenticationBuilder.AddLocalJwtBearerAuthentication();
            }

            if (isExternalJwtBearerEnabled)
            {
                var validIssuer = configuration["Authentication:ExternalJwtBearer:ValidIssuer"];

                if (string.IsNullOrEmpty(validIssuer))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:ValidIssuer setting must be available");
                }

                var validAudience = configuration["Authentication:ExternalJwtBearer:ValidAudience"];

                if (string.IsNullOrEmpty(validAudience))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:ValidAudience setting must be available");
                }

                var authority = configuration["Authentication:ExternalJwtBearer:Authority"];

                if (string.IsNullOrEmpty(authority))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:Authority setting must be available");
                }

                var claimName = configuration["Authentication:ExternalJwtBearer:IdentifierClaimName"];

                if (string.IsNullOrEmpty(claimName))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:IdentifierClaimName setting must be available");
                }

                var personIdentifierPropertyKind = configuration["Authentication:ExternalJwtBearer:PersonIdentifierPropertyKind"];

                if (string.IsNullOrEmpty(personIdentifierPropertyKind))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:PersonIdentifierPropertyKind setting must be available");
                }

                if (!Enum.TryParse<PersonIdentifierPropertyKind>(personIdentifierPropertyKind, out _))
                {
                    throw new ConfigurationErrorsException($"Invalid value for Authentication:ExternalJwtBearer:PersonIdentifierPropertyKind," +
                                                           $" should be one of: {string.Join(", ",Enum.GetValues<PersonIdentifierPropertyKind>())}");
                }

                var cliendId = configuration["Authentication:ExternalJwtBearer:ClientId"];
                
                if(string.IsNullOrEmpty(cliendId))
                {
                    throw new ConfigurationErrorsException("The Authentication:ExternalJwtBearer:CleintId setting must be available");
                }
                
                authenticationBuilder.AddExternalJwtBearerAuthentication(configure: options =>
                {
                    options.Authority = authority;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        ValidAudience = validAudience,
                        ValidIssuer = validIssuer,
                        SignatureValidator = (token, _) => new JsonWebToken(token),
                        ClockSkew = TimeSpan.Zero
                    };
                });

                var pluginInjector = new AuthenticationPluginInjector(new NullLogger<AuthenticationPluginInjector>());

                if (!pluginInjector.Connectors.Any(x => x.Name == "CDP4ExternalJwtAuthentication" && x.Properties.IsEnabled))
                {
                    throw new ConfigurationErrorsException("External JWT Authentication plugin is not present, please contact administrator to include Enterprise Edition plugins.");
                }
            }
            else
            {
                authenticationBuilder.AddExternalJwtBearerAuthentication();    
            }

            services.AddAuthorization(options =>
            {
                var anonymousAuthenticationPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(AnonymousAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(AnonymousAuthenticationDefaults.AuthenticationScheme, anonymousAuthenticationPolicy);

                var basicAuthenticationPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(BasicAuthenticationDefaults.AuthenticationScheme, basicAuthenticationPolicy);

                var localJwtBearerAuthenticationPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(Authentication.Bearer.JwtBearerDefaults.LocalAuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(Authentication.Bearer.JwtBearerDefaults.LocalAuthenticationScheme, localJwtBearerAuthenticationPolicy);

                var externalJwtBearerAuthenticationPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(Authentication.Bearer.JwtBearerDefaults.ExternalAuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(Authentication.Bearer.JwtBearerDefaults.ExternalAuthenticationScheme, externalJwtBearerAuthenticationPolicy);
            });
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
            builder.RegisterType<CometTaskService>().As<ICometTaskService>().SingleInstance();

            // 10-25 helpers
            builder.RegisterType<DataModelUtils>().As<IDataModelUtils>().SingleInstance();
            builder.RegisterType<DefaultPermissionProvider>().As<IDefaultPermissionProvider>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(IMetaInfo).Assembly).Where(x => typeof(IMetaInfo).IsAssignableFrom(x)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<MetaInfoProvider>().As<IMetaDataProvider>().As<IMetaInfoProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<Cdp4JsonSerializer>().As<ICdp4JsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<MessagePackSerializer>().As<IMessagePackSerializer>().InstancePerLifetimeScope();

            // authentication services
            builder.RegisterType<AuthenticationPersonDao>().As<IAuthenticationPersonDao>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationPersonAuthenticator>().As<IAuthenticationPersonAuthenticator>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<CredentialsService>().As<ICredentialsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<JwtTokenService>().As<IJwtTokenService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();

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
            builder.RegisterType<Services.ServiceProvider>().As<Services.IServiceProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
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
            app.UseAuthorization();

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
