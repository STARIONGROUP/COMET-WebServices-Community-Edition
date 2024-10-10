// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileImportyApi.cs" company="Starion Group S.A.">
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

namespace CometServer.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;

    using CDP4Common.DTO;
    using CDP4Common.Helpers;

    using CDP4Orm.Dao;
    using CDP4Orm.MigrationEngine;

    using CometServer.Configuration;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.DataStore;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;

    using Npgsql;
    using System.Text.RegularExpressions;
    
    /// <summary>
    /// This is an API endpoint class to support the ECSS-E-TM-10-25-AnnexC exchange file format import
    /// </summary>
    public class ExchangeFileImportyApi : CarterModule
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "SiteDirectory";

        /// <summary>
        /// Due to the behaviour of a sql sequence the first time the sequence nextval us queried the seemignly currentval
        /// is returned.
        /// </summary>
        private const int IterationNumberSequenceInitialization = 1;

        /// <summary>
        /// The (injected) <see cref="ILogger{ExchangeFileImportyApi}"/>
        /// </summary>
        private readonly ILogger<ExchangeFileImportyApi> logger;

        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/>
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The (injected) <see cref="ICometHasStartedService"/>
        /// </summary>
        private readonly ICometHasStartedService cometHasStartedService;

        /// <summary>
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </summary>
        private readonly ITokenGeneratorService tokenGeneratorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileImportyApi"/>
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/>
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        /// <param name="logger">
        /// The (injected) <see cref="ILogger{ExchangeFileImportyApi}"/>
        /// </param>
        public ExchangeFileImportyApi(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILogger<ExchangeFileImportyApi> logger)
        {
            this.appConfigService = appConfigService;
            this.cometHasStartedService = cometHasStartedService;
            this.tokenGeneratorService = tokenGeneratorService;
            this.logger = logger;
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Data/Exchange", async (HttpRequest req, HttpResponse res,
                IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService) =>
            {
                await this.SeedDataStore(req, res, requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, dataStoreController, siteDirectoryService, engineeringModelSetupService);
            });

            app.MapPost("/Data/Import", async (HttpRequest req, HttpResponse res,
                IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController) =>
            {
                await this.ImportDataStore(req, res, requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, dataStoreController);
            });

            app.MapPost("/Data/Restore", async (HttpRequest req, HttpResponse res, IDataStoreController dataStoreController) =>
            {
                await this.RestoreDatastore(req, res, dataStoreController);
            });
        }

        /// <summary>
        /// Restore the data store.
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="dataStoreController">
        /// The (injected) <see cref="IDataStoreController"/>
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        internal async Task RestoreDatastore(HttpRequest httpRequest, HttpResponse response, IDataStoreController dataStoreController)
        {
            var reqsw = Stopwatch.StartNew();
            var requestToken = this.tokenGeneratorService.GenerateRandomToken();

            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", Regex.Replace((httpRequest.QueryNameMethodPath()).ToString(), "\n|\r", "-"), requestToken);

            if (!this.appConfigService.AppConfig.Backtier.IsDbRestoreEnabled)
            {
                this.logger.LogInformation("Data restore API invoked but it was disabled from configuration, cancel further processing...");

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("restore is not allowed");
                return;
            }

            try
            {
                this.logger.LogDebug("{request}:{requestToken} - Starting data store rollback", httpRequest.QueryNameMethodPath(), requestToken);

                dataStoreController.RestoreDataStore();

                this.logger.LogDebug("{request}:{requestToken} - Finished data store rollback", httpRequest.QueryNameMethodPath(), requestToken);

                this.cometHasStartedService.SetHasStartedAndIsReady(true);

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.AsJson("DataStore restored");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "{request}:{requestToken} - Error occured during data store rollback", httpRequest.QueryNameMethodPath(), requestToken);

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("DataStore restored failed");
            }
            finally
            {
                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", Regex.Replace((httpRequest.QueryNameMethodPath()).ToString(), "\n|\r", "-"), requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Saves the seed file to disk and returns the file path where it has been saved
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that contains fhe file
        /// </param>
        /// <returns>
        /// the file path where the seed file has been stored
        /// </returns>
        private async Task<string> SaveTemporarySeedFile(HttpRequest request)
        {
            var uploadDirectory = this.appConfigService.AppConfig.Midtier.UploadDirectory;

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var tempFileName = Guid.NewGuid().ToString();

            var templFilePath = Path.Combine(uploadDirectory, tempFileName);

            await request.BindAndSaveFile(uploadDirectory, tempFileName);

            return templFilePath;
        }

        /// <summary>
        /// Asynchronously import the data store.
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="jsonExchangeFileReader">
        /// The (injected) <see cref="IJsonExchangeFileReader"/> used to read the provided Annex C3 archive
        /// </param>
        /// <returns>
        /// The <see cref="Task{Response}"/>.
        /// </returns>
        internal async Task ImportDataStore(HttpRequest httpRequest, HttpResponse response, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController)
        {
            var reqsw = Stopwatch.StartNew();
            var requestToken = this.tokenGeneratorService.GenerateRandomToken();

            if (!this.appConfigService.AppConfig.Backtier.IsDbImportEnabled)
            {
                this.logger.LogInformation("{request}:{requestToken} - Data store IMPORT API invoked but it was disabled from configuration, cancel further processing...", httpRequest.QueryNameMethodPath(), requestToken);

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("Data store IMPORT is not allowed");
            }

            this.logger.LogInformation("{request}:{requestToken} - Starting data store IMPORT", httpRequest.QueryNameMethodPath(), requestToken);
            
            var temporarysSeedFilePath = string.Empty;

            try
            {
                temporarysSeedFilePath = await this.SaveTemporarySeedFile(httpRequest);

                // drop existing data stores
                this.DropDataStoreAndPrepareNew(dataStoreController);

                var version = httpRequest.QueryDataModelVersion();

                // handle exchange processing
                if (!this.UpsertModelData(requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, version, temporarysSeedFilePath, null))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.AsJson("invalid seed file");
                    return;
                }

                this.logger.LogInformation("{request}:{requestToken} - Finished the data store import in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

                this.cometHasStartedService.SetHasStartedAndIsReady(true);

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.AsJson("Datastore imported");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unable to import the datastore");

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("Data store import failed");
            }
            finally
            {
                try
                {
                    // Remove the exchange file after processing (saving space)
                    System.IO.File.Delete(temporarysSeedFilePath);
                }
                catch (Exception ex)
                {
                    // swallow exception but log it
                    this.logger.LogWarning(ex, "{request}:{requestToken} - Unable to remove file {temporarysSeedFilePath}", httpRequest.QueryNameMethodPath(), requestToken, temporarysSeedFilePath);
                }

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Asynchronously seed the data store.
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="jsonExchangeFileReader">
        /// The (injected) <see cref="IJsonExchangeFileReader"/> used to read the provided Annex C3 archive
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        internal async Task SeedDataStore(HttpRequest request, HttpResponse response, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService)
        {
            var reqsw = Stopwatch.StartNew();
            var requestToken = this.tokenGeneratorService.GenerateRandomToken();

            if (!this.appConfigService.AppConfig.Backtier.IsDbSeedEnabled)
            {
                this.logger.LogInformation("{request}:{requestToken} - Data store SEED API invoked but it was disabled from configuration, cancel further processing...", request.QueryNameMethodPath(), requestToken);

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("SEED is not allowed");
                return;
            }

            this.logger.LogInformation("{request}:{requestToken} - Starting data store SEED", request.QueryNameMethodPath(), requestToken);
            
            var temporarysSeedFilePath = await this.SaveTemporarySeedFile(request);

            // drop existing data stores
            this.DropDataStoreAndPrepareNew(dataStoreController);

            var version = request.QueryDataModelVersion();

            this.logger.LogInformation("{request}:{requestToken} - Seeding version {version}", request.QueryNameMethodPath(), requestToken, version.ToString());

            // handle exchange processing
            if (!this.InsertModelData(requestUtils, transactionManager, jsonExchangeFileReader, migrationService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, version, temporarysSeedFilePath, null, this.appConfigService.AppConfig.Backtier.IsDbSeedEnabled))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await response.AsJson("invalid seed file");
                return;
            }

            // Remove the exchange file after processing (saving space)
            try
            {
                System.IO.File.Delete(temporarysSeedFilePath);
            }
            catch (Exception ex)
            {
                // swallow exception but log it
                this.logger.LogError(ex, "{request}:{requestToken} - Unable to remove file {temporarysSeedFilePath}", request.QueryNameMethodPath(), requestToken, temporarysSeedFilePath);
            }

            try
            {
                // Create a jsonb for each entry in the database
                this.CreateRevisionHistoryForEachEntry(requestUtils, transactionManager, revisionService, personService, siteDirectoryService, engineeringModelSetupService);

                // database was succesfully seeded
                // create a clone of the data store for future restore support
                dataStoreController.CloneDataStore();

                reqsw.Stop();

                this.logger.LogInformation("{request}:{requestToken} - Finished the data store SEED in {ElapsedMilliseconds}", request.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

                this.cometHasStartedService.SetHasStartedAndIsReady(true);

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.AsJson($"Datastore seeded in {reqsw.ElapsedMilliseconds} [ms]");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "{request}:{requestToken} - DataStore restored failed", request.QueryNameMethodPath(), requestToken);

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("DataStore restored failed");
            }
        }

        /// <summary>
        /// Parse the url segments and return the data as serialized JSON
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="fileName">
        /// The exchange file name.
        /// </param>
        /// <param name="password">
        /// The optional archive password as supplied by the request
        /// </param>
        /// <param name="seed">
        /// Optional seed indicator which removes any existing data first if true
        /// </param>
        /// <returns>
        /// True if successful, false if not
        /// </returns>
        private bool InsertModelData(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider,  IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, Version version, string fileName, string password = null, bool seed = true)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                var sw = new Stopwatch();
                if (seed)
                {
                    // clear database schemas if seeding
                    this.logger.LogInformation("Start clearing the current data store");
                    transaction = transactionManager.SetupTransaction(ref connection, null);
                    transactionManager.SetFullAccessState(true);
                    ClearDatabaseSchemas(transaction);
                    transaction.Commit();

                    // Flushes the type cache and reload the types for this connection
                    connection.ReloadTypes();
                }

                sw.Start();
                this.logger.LogInformation("Start seeding the data");

                // use new transaction to for inserting the data
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // important, make sure to defer the constraints
                var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                command.ExecuteNonQuery();

                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                transactionManager.SetAuditLoggingState(transaction, false);

                // get sitedirectory data
                var items = jsonExchangeFileReader.ReadSiteDirectoryFromfile(version, fileName, password).ToList();

                var siteRdlCount = items.OfType<SiteReferenceDataLibrary>().Count();
                var seededSiteRdlCount = 0;
                var engineeringModelCount = items.OfType<EngineeringModelSetup>().Count();
                var seededEngineeringModelCount = 0;

                this.logger.LogInformation("{siteRdlCount} Site Reference Data Libraries and {engineeringModelCount} Engineering Models will be seeded", siteRdlCount, engineeringModelCount);

                // assign default password to all imported persons.
                foreach (var person in items.OfType<Person>())
                {
                    person.Password = this.appConfigService.AppConfig.Defaults.PersonPassword;
                }

                var topContainer = items.SingleOrDefault(x => x.IsSameOrDerivedClass<TopContainer>()) as TopContainer;

                if (topContainer == null)
                {
                    this.logger.LogError("No Topcontainer item encountered");
                    throw new NoNullAllowedException("Topcontainer item needs to be present in the dataset");
                }

                requestUtils.Cache = new List<Thing>(items);

                // setup Site Directory schema
                using (var siteDirCommand = new NpgsqlCommand())
                {
                    this.logger.LogInformation("Start Site Directory structure");
                    siteDirCommand.Connection = transaction.Connection;
                    siteDirCommand.Transaction = transaction;

                    ExecuteSiteDirectorySchemaScripts(siteDirCommand);
                }

                // apply migration on new SiteDirectory partition
                migrationService.ApplyMigrations(transaction, typeof(SiteDirectory).Name, false);

                var result = false;

                if (topContainer.GetType().Name == "SiteDirectory")
                {
                    // make sure single iterationsetups are set to unfrozen before persitence
                    FixSingleIterationSetups(items);
                    var siteDirectoryService = serviceProvider.MapToPersitableService<SiteDirectoryService>("SiteDirectory");

                    result = siteDirectoryService.Insert(transaction, "SiteDirectory", topContainer);
                    seededSiteRdlCount = siteRdlCount;
                }

                if (result)
                {
                    requestUtils.QueryParameters = new QueryParameters();

                    // Get users credentials from migration.json file
                    var migrationCredentials = jsonExchangeFileReader.ReadMigrationJsonFromFile(fileName, password).ToList();

                    foreach (var person in items.OfType<Person>())
                    {
                        var credential = migrationCredentials.FirstOrDefault(mc => mc.Iid == person.Iid);
                        if (credential != null)
                        {
                            personService.UpdateCredentials(transaction, "SiteDirectory", person, credential);
                        }
                    }

                    // Add missing Person permissions
                    this.CreateMissingPersonPermissions(transaction, personRoleService, personPermissionService, defaultPermissionProvider);

                    this.logger.LogInformation("{seededSiteRdlCount}/{siteRdlCount} Site Reference Data Libraries and {seededEngineeringModelCount}/{engineeringModelCount} Engineering Models seeded", seededSiteRdlCount, siteRdlCount, seededEngineeringModelCount, engineeringModelCount);

                    var engineeringModelSetups =
                        items.OfType<EngineeringModelSetup>()
                            .ToList();
                    var engineeringModelService = serviceProvider.MapToPersitableService<EngineeringModelService>("EngineeringModel");

                    var iterationService = serviceProvider.MapToPersitableService<IterationService>("Iteration");

                    foreach (var engineeringModelSetup in engineeringModelSetups)
                    {
                        this.logger.LogInformation("Inserting data for EngineeringModelSetup {shortname}:{name}", engineeringModelSetup.ShortName, engineeringModelSetup.Name);

                        // cleanup before handling TopContainer
                        requestUtils.Cache.Clear();

                        // get referenced engineeringmodel data
                        var engineeringModelItems = jsonExchangeFileReader
                            .ReadEngineeringModelFromfile(version, fileName, password, engineeringModelSetup).ToList();

                        // should return one engineeringmodel topcontainer
                        var engineeringModel = engineeringModelItems.OfType<EngineeringModel>().Single();

                        if (engineeringModel == null)
                        {
                            this.logger.LogWarning("The EngineeringModel Thing for {shortname} could not be found, seeding will abort", engineeringModelSetup.ShortName);
                            result = false;
                            break;
                        }

                        var dataPartition = CDP4Orm.Dao.Utils.GetEngineeringModelSchemaName(engineeringModel.Iid);
                        requestUtils.Cache = new List<Thing>(engineeringModelItems);

                        if (!engineeringModelService.Insert(transaction, dataPartition, engineeringModel))
                        {
                            result = false;
                            break;
                        }

                        // Add missing Participant permissions
                        this.CreateMissingParticipantPermissions(transaction, participantRoleService, participantPermissionService, defaultPermissionProvider);

                        // extract any referenced file data to disk if not already present
                        PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, engineeringModelSetup, password);

                        var iterationSetups = items.OfType<IterationSetup>()
                            .Where(
                                x => engineeringModelSetup.IterationSetup.Contains(x.Iid)).ToList();

                        // get current maximum iterationNumber and increase by one for the next value
                        var maxIterationNumber = iterationSetups.Select(x => x.IterationNumber).Max() + IterationNumberSequenceInitialization;

                        // reset the start number of iterationNumber sequence
                        engineeringModelDao.ResetIterationNumberSequenceStartNumber(
                            transaction,
                            dataPartition,
                            maxIterationNumber);

                        var iterationInsertResult = true;

                        foreach (var iterationSetup in iterationSetups)
                        {
                            requestUtils.Cache.Clear();

                            var iterationItems = jsonExchangeFileReader
                                .ReadModelIterationFromFile(version, fileName, password, engineeringModelSetup, iterationSetup).ToList();

                            requestUtils.Cache = new List<Thing>(iterationItems);

                            // should return one iteration
                            var iteration = iterationItems.SingleOrDefault(x => x.ClassKind == CDP4Common.CommonData.ClassKind.Iteration) as Iteration;

                            if (iteration == null || !iterationService.CreateConcept(
                                    transaction,
                                    dataPartition,
                                    iteration,
                                    engineeringModel))
                            {
                                iterationInsertResult = false;
                                break;
                            }
                        }

                        if (!iterationInsertResult)
                        {
                            result = false;
                            break;
                        }

                        // extract any referenced file data to disk if not already present
                        PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, engineeringModelSetup, password);

                        seededEngineeringModelCount++;

                        this.logger.LogInformation("{seededSiteRdlCount}/{siteRdlCount} Site Reference Data Libraries and {seededEngineeringModelCount}/{engineeringModelCount} Engineering Models seeded", seededSiteRdlCount, siteRdlCount, seededEngineeringModelCount, engineeringModelCount);
                    }
                }

                var commitSw = new Stopwatch();
                commitSw.Start();
                this.logger.LogInformation("Committing transaction...");
                transaction.Commit();
                commitSw.Stop();
                this.logger.LogInformation("Transaction committed in {commitSw} [ms]", commitSw.ElapsedMilliseconds);
                sw.Stop();
                this.logger.LogInformation("Finished seeding the data store in {sw} [ms]", sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occured during data store seeding");

                return false;
            }
            finally
            {
                transaction?.Dispose();

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Import data and use Upsert flow to add/update data. Return the data as serialized JSON
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="fileName">
        /// The exchange file name.
        /// </param>
        /// <param name="password">
        /// The optional archive password as supplied by the request
        /// </param>
        /// <returns>
        /// True if successful
        /// </returns>
        private bool UpsertModelData(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, Services.IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, Version version, string fileName, string password = null)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                var sw = new Stopwatch();

                // clear database schemas if seeding
                this.logger.LogInformation("Start clearing the current data store");
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);
                ClearDatabaseSchemas(transaction);
                transaction.Commit();

                // Flushes the type cache and reload the types for this connection
                connection.ReloadTypes();

                sw.Start();
                this.logger.LogInformation("Start importing the data");

                // use new transaction to for inserting the data
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // important, make sure to defer the constraints
                var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                command.ExecuteNonQuery();

                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                transactionManager.SetAuditLoggingState(transaction, true);

                // get sitedirectory data
                var items = jsonExchangeFileReader.ReadSiteDirectoryFromfile(version, fileName, password).ToList();

                // assign default password to all imported persons.
                foreach (var person in items.OfType<Person>())
                {
                    person.Password = this.appConfigService.AppConfig.Defaults.PersonPassword;
                }

                var topContainer = items.SingleOrDefault(x => x.IsSameOrDerivedClass<TopContainer>()) as TopContainer;

                if (topContainer == null)
                {
                    this.logger.LogError("No Topcontainer item encountered");
                    throw new NoNullAllowedException("Topcontainer item needs to be present in the dataset");
                }

                requestUtils.Cache = new List<Thing>(items);

                // setup Site Directory schema
                using (var siteDirCommand = new NpgsqlCommand())
                {
                    this.logger.LogInformation("Start Site Directory structure");
                    siteDirCommand.Connection = transaction.Connection;
                    siteDirCommand.Transaction = transaction;

                    ExecuteSiteDirectorySchemaScripts(siteDirCommand);
                }

                // apply migration on new SiteDirectory partition
                migrationService.ApplyMigrations(transaction, nameof(SiteDirectory), false);

                var result = false;

                if (topContainer.GetType().Name == "SiteDirectory")
                {
                    // make sure single iterationsetups are set to unfrozen before persitence
                    FixSingleIterationSetups(items);

                    var siteDirectoryService = serviceProvider.MapToPersitableService<SiteDirectoryService>("SiteDirectory");

                    result = siteDirectoryService.Insert(transaction, "SiteDirectory", topContainer);
                }

                if (result)
                {
                    requestUtils.QueryParameters = new QueryParameters();

                    // Get users credentials from migration.json file
                    var migrationCredentials = jsonExchangeFileReader.ReadMigrationJsonFromFile(fileName, password).ToList();

                    foreach (var person in items.OfType<Person>())
                    {
                        var credential = migrationCredentials.FirstOrDefault(mc => mc.Iid == person.Iid);

                        if (credential != null)
                        {
                            personService.UpdateCredentials(transaction, "SiteDirectory", person, credential);
                        }
                    }

                    // Add missing Person permissions
                    this.CreateMissingPersonPermissions(transaction, personRoleService, personPermissionService, defaultPermissionProvider);

                    var engineeringModelSetups =
                        items.OfType<EngineeringModelSetup>()
                            .ToList();

                    var engineeringModelService =
                        serviceProvider.MapToPersitableService<EngineeringModelService>("EngineeringModel");

                    var iterationService = serviceProvider.MapToPersitableService<IterationService>("Iteration");
                    var createRevisionForSiteDirectory = true;
                    var actorId = Guid.Empty;

                    foreach (var engineeringModelSetup in engineeringModelSetups)
                    {
                        var revisionNumber = EngineeringModelSetupSideEffect.FirstRevision;

                        // cleanup before handling TopContainer
                        requestUtils.Cache.Clear();

                        // get referenced engineeringmodel data
                        var engineeringModelItems = jsonExchangeFileReader
                            .ReadEngineeringModelFromfile(version, fileName, password, engineeringModelSetup).ToList();

                        // should return one engineeringmodel topcontainer
                        var engineeringModel = engineeringModelItems.OfType<EngineeringModel>().Single();

                        var dataPartition = CDP4Orm.Dao.Utils.GetEngineeringModelSchemaName(engineeringModel.Iid);

                        var iterationPartition = CDP4Orm.Dao.Utils.GetEngineeringModelIterationSchemaName(engineeringModel.Iid);

                        requestUtils.Cache = new List<Thing>(engineeringModelItems);

                        if (!engineeringModelService.Insert(transaction, dataPartition, engineeringModel))
                        {
                            result = false;
                            break;
                        }

                        // Add missing Participant permissions
                        this.CreateMissingParticipantPermissions(transaction, participantRoleService, participantPermissionService, defaultPermissionProvider);

                        // extract any referenced file data to disk if not already present
                        PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, engineeringModelSetup, password);

                        var iterationSetups = items.OfType<IterationSetup>()
                            .Where(
                                x => engineeringModelSetup.IterationSetup.Contains(x.Iid)).ToList();

                        // get current maximum iterationNumber and increase by one for the next value
                        var maxIterationNumber = iterationSetups.Select(x => x.IterationNumber).Max() + IterationNumberSequenceInitialization;

                        // reset the start number of iterationNumber sequence
                        engineeringModelDao.ResetIterationNumberSequenceStartNumber(
                            transaction,
                            dataPartition,
                            maxIterationNumber);

                        var iterationInsertResult = true;
                        List<Thing> previousIterationItems = null;

                        foreach (var iterationSetup in iterationSetups.OrderBy(x => x.IterationNumber))
                        {
                            requestUtils.Cache.Clear();

                            var iterationItems = jsonExchangeFileReader
                                .ReadModelIterationFromFile(version, fileName, password, engineeringModelSetup, iterationSetup).ToList();

                            requestUtils.Cache = new List<Thing>(iterationItems);

                            // should return one iteration
                            // for the every model EngineeringModel schema ends with the same ID as Iteration schema
                            var iteration = iterationItems.SingleOrDefault(x => x.ClassKind == CDP4Common.CommonData.ClassKind.Iteration) as Iteration;

                            iterationInsertResult = false;

                            if (iteration == null)
                            {
                                break;
                            }

                            if (iterationService.UpsertConcept(
                                transaction,
                                dataPartition,
                                iteration,
                                engineeringModel))
                            {
                                iterationInsertResult = true;

                                if (previousIterationItems != null)
                                {
                                    // Compute differences between iterations
                                    var thingsToBeDeleted = previousIterationItems
                                        .Where(thing => thing.ClassKind != CDP4Common.CommonData.ClassKind.Iteration &&
                                                        !iterationItems.Select(id => id.Iid).Contains(thing.Iid)).ToList();

                                    // Remove differences between iterations
                                    foreach (var thing in thingsToBeDeleted)
                                    {
                                        var service = serviceProvider.MapToPersitableService<IPersistService>(thing.ClassKind.ToString());
                                        service.RawDeleteConcept(transaction, iterationPartition, thing);
                                    }
                                }

                                transaction.Commit();

                                if (createRevisionForSiteDirectory)
                                {
                                    // Create revision history only once for SiteDirectory
                                    this.CreateRevisionHistoryForSiteDirectory(requestUtils, transactionManager, revisionService, personService, ref actorId);
                                    createRevisionForSiteDirectory = false;
                                }

                                // Create revision history for each EngineeringModel
                                this.CreateRevisionHistoryForEngineeringModel(requestUtils, transactionManager, revisionService, actorId, revisionNumber, engineeringModelSetup.EngineeringModelIid);

                                // use new transaction to for inserting the data
                                transaction = transactionManager.SetupTransaction(ref connection, null);
                                transactionManager.SetFullAccessState(true);

                                // important, make sure to defer the constraints
                                var constraintcommand = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                                constraintcommand.ExecuteNonQuery();

                                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                                transactionManager.SetAuditLoggingState(transaction, true);

                                // revision number goes up for the next Iteration
                                revisionNumber += 1;
                            }

                            previousIterationItems = iterationItems;
                        }

                        if (!iterationInsertResult)
                        {
                            result = false;
                            break;
                        }

                        // extract any referenced file data to disk if not already present
                        PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, engineeringModelSetup, password);
                    }
                }

                transaction.Commit();

                sw.Stop();
                this.logger.LogInformation("Finished importing the data store in {sw} [ms]", sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                this.logger.LogError(ex, "Error occured during data store import");

                return false;
            }
            finally
            {
                transaction?.Dispose();

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes all SQL scripts to create SiteDirectory schema
        /// </summary>
        /// <param name="sqlCommand">The <see cref="NpgsqlCommand"/></param>
        private static void ExecuteSiteDirectorySchemaScripts(NpgsqlCommand sqlCommand)
        {
            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.01_SiteDirectory_setup.sql");
            sqlCommand.ExecuteNonQuery();

            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.02_SiteDirectory_structure.sql");
            sqlCommand.ExecuteNonQuery();

            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.03_SiteDirectory_triggers.sql");
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Persist the file binary data in the reference zip archive.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="fileName">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        private static void PersistFileBinaryData(IRequestUtils requestUtils, IJsonExchangeFileReader jsonExchangeFileReader,  string fileName, EngineeringModelSetup engineeringModelSetup, string password = null)
        {
            var fileRevisions = requestUtils.Cache.OfType<FileRevision>().ToList();
            if (fileRevisions.Count == 0)
            {
                // nothing to do
                return;
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                jsonExchangeFileReader.ReadAndStoreFileBinary(fileName, password, engineeringModelSetup, hash);
            }
        }

        /// <summary>
        /// Fix single iteration setups in the export.
        /// An exchange file can contain one or all iteration setups of a site directory.
        /// Make sure that incase one iteration setup is supplied it is unfrozen and has no source iteration set
        /// </summary>
        /// <param name="items">
        /// The read in items.
        /// </param>
        private static void FixSingleIterationSetups(List<Thing> items)
        {
            var engineeringModelSetups = items.OfType<EngineeringModelSetup>().ToList();

            // unset the FrozenOn and SourceIterationSetup properties for single iteration setup in the siteDirectory.
            foreach (var iterationSetups in engineeringModelSetups.Select(
                engineeringModelSetup =>
                    items.OfType<IterationSetup>().Where(
                            x => engineeringModelSetup.IterationSetup.Contains(x.Iid))
                         .ToList()).Where(iterationSetups => iterationSetups.Count == 1))
            {
                iterationSetups[0].FrozenOn = null;
                iterationSetups[0].SourceIterationSetup = null;
            }
        }

        /// <summary>
        /// The clear database schemas.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        private static void ClearDatabaseSchemas(NpgsqlTransaction transaction)
        {
            // clear the current database data (except public and pg_catalog schemas)
            using (var cleanSchemaCommand = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                // setup clear schema function if not existent
                // source: https://chawlasumit.wordpress.com/2014/07/29/drop-all-schemas-in-postgres/
                sqlBuilder.AppendLine("CREATE OR REPLACE FUNCTION public.clear_schemas()")
                    .AppendLine("    RETURNS VOID AS").AppendLine("    $$").AppendLine("    DECLARE rec RECORD;")
                    .AppendLine("    BEGIN").AppendLine("        -- Get all schemas").AppendLine("        FOR rec IN")
                    .AppendLine("        SELECT DISTINCT schema_name")
                    .AppendLine("        FROM information_schema.schemata").AppendLine("        -- exclude schemas")
                    .AppendLine(
                        "        WHERE schema_name NOT LIKE 'pg_%' AND schema_name NOT LIKE 'information_schema' AND schema_name <> 'public'")
                    .AppendLine("            LOOP")
                    .AppendLine("                EXECUTE 'DROP SCHEMA \"' || rec.schema_name || '\" CASCADE';")
                    .AppendLine("            END LOOP;").AppendLine("        RETURN;").AppendLine("    END;")
                    .AppendLine("    $$ LANGUAGE plpgsql;");

                sqlBuilder.AppendLine("SELECT clear_schemas();");

                cleanSchemaCommand.CommandText = sqlBuilder.ToString();
                cleanSchemaCommand.Connection = transaction.Connection;
                cleanSchemaCommand.Transaction = transaction;
                cleanSchemaCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drops existing working and restore data stores if they exist
        /// and creates a new working data store.
        /// </summary>
        private void DropDataStoreAndPrepareNew(IDataStoreController dataStoreController)
        {
            var sw = Stopwatch.StartNew();

            this.logger.LogInformation("dropping existing data stores");

            var backtierConfig = this.appConfigService.AppConfig.Backtier;

            // Drop the existing databases
            using (var connection = new NpgsqlConnection(Services.Utils.GetConnectionString(backtierConfig, backtierConfig.DatabaseManage)))
            {
                connection.Open();

                // Drop the existing database
                using (var cmd = new NpgsqlCommand())
                {
                    this.logger.LogDebug("Drop the data store");

                    dataStoreController.DropDataStoreConnections(backtierConfig.Database, connection);

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {backtierConfig.Database};";

                    cmd.ExecuteNonQuery();
                }

                // Drop the existing restore database
                using (var cmd = new NpgsqlCommand())
                {
                    this.logger.LogDebug("Drop the restore data store");

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {backtierConfig.DatabaseRestore};";

                    cmd.ExecuteNonQuery();
                }

                // Create a new database
                using (var cmd = new NpgsqlCommand())
                {
                    this.logger.LogDebug("Create the data store");
                    cmd.Connection = connection;

                    cmd.CommandText = $"CREATE DATABASE {backtierConfig.Database} WITH OWNER = {backtierConfig.UserName} TEMPLATE = {backtierConfig.DatabaseManage} ENCODING = 'UTF8';";

                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }

            this.logger.LogInformation("dropping existing data stores completed in {sw} [ms]", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Create revision history for SiteDirectory and retrieve first person Id
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="personId">
        /// First person Id <see cref="Person" />
        /// </param>
        private void CreateRevisionHistoryForSiteDirectory(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, IPersonService personService,  ref Guid personId)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                // Create a revision history for SiteDirectory's entries
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // Get first person Id (so that actor isnt guid.empty), it is hard to determine who it should be.
                var person = personService.GetShallow(transaction, TopContainer, null, new RequestSecurityContext { ContainerReadAllowed = true }).OfType<Person>().FirstOrDefault();
                personId = person != null ? person.Iid : Guid.Empty;

                // Save revision history for SiteDirectory's entries
                revisionService.SaveRevisions(transaction, TopContainer, personId, EngineeringModelSetupSideEffect.FirstRevision);

                transaction.Commit();
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create revision history for each EngineeringModel in the database.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="revisionNumber">
        /// The revision number we want to create revision records for
        /// </param>
        /// <param name="personId">
        /// First person Id <see cref="Guid" />
        /// </param>
        /// <param name="engineeringModelIid">
        /// Engineering model Id <see cref="Guid" />
        /// </param>
        private void CreateRevisionHistoryForEngineeringModel(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, Guid personId, int revisionNumber, Guid engineeringModelIid)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                var partition = requestUtils.GetEngineeringModelPartitionString(engineeringModelIid);

                // Save revision history for EngineeringModel's entries
                revisionService.SaveRevisions(transaction, partition, personId, revisionNumber);

                // commit revision history
                transaction.Commit();
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create revision history for each entry in the database.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        private void CreateRevisionHistoryForEachEntry(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, IPersonService personService, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                // Create a revision history for SiteDirectory's entries
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // Get first person Id (so that actor isnt guid.empty), it is hard to determine who it should be.
                var actor = personService.GetShallow(transaction, TopContainer, null, new RequestSecurityContext {ContainerReadAllowed = true}).OfType<Person>().FirstOrDefault();
                var actorId = actor != null ? actor.Iid : Guid.Empty;

                // Save revision history for SiteDirectory's entries
                revisionService.SaveRevisions(transaction, TopContainer, actorId, EngineeringModelSetupSideEffect.FirstRevision);

                var siteDirectory = siteDirectoryService.Get(
                    transaction,
                    TopContainer,
                    null,
                    new RequestSecurityContext { ContainerReadAllowed = true }).OfType<SiteDirectory>().ToList();

                var engineeringModelSetups = engineeringModelSetupService.GetShallow(
                        transaction,
                        TopContainer,
                        siteDirectory[0].Model,
                        new RequestSecurityContext { ContainerReadAllowed = true }).OfType<EngineeringModelSetup>()
                    .ToList();

                // commit revision history
                transaction.Commit();

                // Get all EngineeringModelSetups and create a revision history for each EngineeringModel
                foreach (var engineeringModelSetup in engineeringModelSetups)
                {
                    transaction = transactionManager.SetupTransaction(ref connection, null);
                    transactionManager.SetFullAccessState(true);

                    var partition =
                        requestUtils.GetEngineeringModelPartitionString(engineeringModelSetup.EngineeringModelIid);

                    // Save revision history for EngineeringModel's entries
                    revisionService.SaveRevisions(transaction, partition, actorId, EngineeringModelSetupSideEffect.FirstRevision);

                    // commit revision history
                    transaction.Commit();
                }
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                this.logger.LogError(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create missing participant permissions for all <see cref="ParticipantRole"/>s
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateMissingParticipantPermissions(NpgsqlTransaction transaction, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDefaultPermissionProvider defaultPermissionProvider)
        {
            var participantRoles = participantRoleService.GetShallow(
                transaction,
                TopContainer,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantRole>();

            // Find and create all missing permissions for each Participant
            foreach (var participantRole in participantRoles)
            {
                this.FindAndCreateMissingParticipantPermissions(participantRole, transaction, participantPermissionService, defaultPermissionProvider);
            }
        }

        /// <summary>
        /// Find and create missing <see cref="ParticipantPermission"/>
        /// </summary>
        /// <param name="participantRole">
        /// The <see cref="ParticipantRole"/> to find and create <see cref="ParticipantPermission"/>s for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingParticipantPermissions(ParticipantRole participantRole, NpgsqlTransaction transaction, IParticipantPermissionService participantPermissionService,  IDefaultPermissionProvider defaultPermissionProvider)
        {
            var participantPermissions = participantPermissionService.GetShallow(
                transaction,
                TopContainer,
                participantRole.ParticipantPermission,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantPermission>().ToList();

            foreach (var classKind in Enum.GetValues(typeof(CDP4Common.CommonData.ClassKind)).Cast<CDP4Common.CommonData.ClassKind>())
            {
                var defaultPermission = defaultPermissionProvider.GetDefaultParticipantPermission(classKind);

                if (defaultPermission == CDP4Common.CommonData.ParticipantAccessRightKind.NONE)
                {
                    var participantPermission = participantPermissions.Find(x => x.ObjectClass == classKind);

                    if (participantPermission == null)
                    {
                        this.logger.LogDebug("Create ParticipantPermission for class {classKind} for ParticipantRole {participantRole}", classKind, participantRole.Iid);

                        var permission = new ParticipantPermission(Guid.NewGuid(), 0)
                        {
                            ObjectClass = classKind,
                            AccessRight = defaultPermission
                        };

                        participantRole.ParticipantPermission.Add(permission.Iid);
                        participantPermissionService.CreateConcept(transaction, TopContainer, permission, participantRole);
                    }
                }
            }
        }

        /// <summary>
        /// Create missing person permissions.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateMissingPersonPermissions(NpgsqlTransaction transaction, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider)
        {
            var personRoles = personRoleService.GetShallow(
                transaction,
                TopContainer,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonRole>();

            // Find and create all missing permissions for each Person
            foreach (var personRole in personRoles)
            {
                this.FindAndCreateMissingPersonPermissions(personPermissionService, defaultPermissionProvider, personRole, transaction);
            }
        }

        /// <summary>
        /// Find and create missing <see cref="PersonPermission"/>s
        /// </summary>
        /// <param name="personRole">
        /// The <see cref="PersonRole"/> to find and create <see cref="PersonPermission"/> for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingPersonPermissions(IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, PersonRole personRole, NpgsqlTransaction transaction)
        {
            var personPermissions = personPermissionService.GetShallow(
                transaction,
                TopContainer,
                personRole.PersonPermission,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonPermission>().ToList();

            foreach (var classKind in Enum.GetValues(typeof(CDP4Common.CommonData.ClassKind)).Cast<CDP4Common.CommonData.ClassKind>())
            {
                var defaultPermission = defaultPermissionProvider.GetDefaultPersonPermission(classKind);

                if (defaultPermission == CDP4Common.CommonData.PersonAccessRightKind.NONE)
                {
                    var personPermission = personPermissions.Find(x => x.ObjectClass == classKind);

                    if (personPermission == null)
                    {
                        this.logger.LogDebug("Create PersonPermission for class {classKind} for PersonRole {personRole}", classKind, personRole.Iid);

                        var permission = new PersonPermission(Guid.NewGuid(), 0)
                        {
                            ObjectClass = classKind,
                            AccessRight = defaultPermission
                        };

                        personRole.PersonPermission.Add(permission.Iid);
                        personPermissionService.CreateConcept(transaction, TopContainer, permission, personRole);
                    }
                }
            }
        }
    }
}
