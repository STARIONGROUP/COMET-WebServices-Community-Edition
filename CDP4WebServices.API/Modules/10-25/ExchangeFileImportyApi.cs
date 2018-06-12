// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileImportyApi.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CDP4WebService.Authentication;
    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.DataStore;
    using CDP4WebServices.API.Services.FileHandling;
    using CDP4WebServices.API.Services.Protocol;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses;
    using NLog;
    using Npgsql;

    using IServiceProvider = CDP4WebServices.API.Services.IServiceProvider;
    using Thing = CDP4Common.DTO.Thing;
    using TopContainer = CDP4Common.DTO.TopContainer;

    /// <summary>
    /// This is an API endpoint class to support the ECSS-E-TM-10-25-AnnexC exchange file format import
    /// </summary>
    public class ExchangeFileImportyApi : NancyModule
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
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the <see cref="IPersonResolver"/> that manages retrieving the <see cref="CDP4Common.DTO.Person"/> from the database. 
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets or sets the revision service.
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets or sets the cache service.
        /// </summary>
        public ICacheService CacheService { get; set; }

        /// <summary>
        /// Gets or sets the permission service.
        /// </summary>
        public IPermissionService PermissionService { get; set; }

        /// <summary>
        /// Gets or sets the SiteDirectory service.
        /// </summary>
        public ISiteDirectoryService SiteDirectoryService { get; set; }

        /// <summary>
        /// Gets or sets the EngineeringModelSetup service.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the file upload handler.
        /// </summary>
        public ILocalFileStorage LocalFileStorage { get; set; }

        /// <summary>
        /// Gets or sets the exchange file processor.
        /// </summary>
        public IExchangeFileProcessor ExchangeFileProcessor { get; set; }

        /// <summary>
        /// Gets or sets the header info provider.
        /// </summary>
        public IHeaderInfoProvider HeaderInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the request utils.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the data store controller.
        /// </summary>
        public IDataStoreController DataStoreController { get; set; }

        /// <summary>
        /// Gets or sets the web service authentication.
        /// </summary>
        public ICDP4WebServiceAuthentication WebServiceAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModel dao.
        /// </summary>
        public IEngineeringModelDao EngineeringModelDao { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelSetup dao.
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectory dao.
        /// </summary>
        public ISiteDirectoryDao SiteDirectoryDao { get; set; }

        /// <summary>
        /// Gets or sets the user validator
        /// </summary>
        public IUserValidator UserValidator { get; set; }

        /// <summary>
        /// Gets or sets the Person service.
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Gets or sets the PersonRole service.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }

        /// <summary>
        /// Gets or sets the PersonPermission service.
        /// </summary>
        public IPersonPermissionService PersonPermissionService { get; set; }

        /// <summary>
        /// Gets or sets the Participant service.
        /// </summary>
        public IParticipantService ParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the ParticipantRole service.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }

        /// <summary>
        /// Gets or sets the ParticipantPermission service.
        /// </summary>
        public IParticipantPermissionService ParticipantPermissionService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileImportyApi"/> class.
        /// </summary>
        public ExchangeFileImportyApi()
        {
            // Seed the data store from the provided (uploaded) exchange file
            this.Post["/Data/Exchange", true] = async (x, ct) => await this.SeedDataStore();

            // Restore the data store to the data snapshot created from the inital seed
            this.Post["/Data/Restore"] = x => this.RestoreDatastore();
        }

        /// <summary>
        /// Restore the data store.
        /// </summary>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        internal Response RestoreDatastore()
        {
            if (!AppConfig.Current.Backtier.IsDbRestoreEnabled)
            {
                Logger.Info(
                    "Data restore API invoked but it was disabled from configuration, cancel further processing...");
                var notAcceptableResponse = new Response().WithStatusCode(HttpStatusCode.NotAcceptable);
                this.HeaderInfoProvider.RegisterResponseHeaders(notAcceptableResponse);
                return notAcceptableResponse;
            }

            try
            {
                Logger.Info("Starting data store rollback");
                this.DataStoreController.RestoreDataStore();

                // reset the credential cache as the underlying datastore was reset
                this.WebServiceAuthentication.ResetCredentialCache();

                Logger.Info("Finished data store rollback");
                var response = new HtmlResponse();
                this.HeaderInfoProvider.RegisterResponseHeaders(response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occured during data store rollback");

                var errorResponse = new HtmlResponse(HttpStatusCode.InternalServerError);
                this.HeaderInfoProvider.RegisterResponseHeaders(errorResponse);
                return errorResponse;
            }
        }

        /// <summary>
        /// Asynchronously seed the data store.
        /// </summary>
        /// <returns>
        /// The <see cref="Task{Response}"/>.
        /// </returns>
        internal async Task<Response> SeedDataStore()
        {
            if (!AppConfig.Current.Backtier.IsDbSeedEnabled)
            {
                Logger.Info(
                    "Data store seed API invoked but it was disabled from configuration, cancel further processing...");
                var notAcceptableResponse = new Response().WithStatusCode(HttpStatusCode.NotAcceptable);
                this.HeaderInfoProvider.RegisterResponseHeaders(notAcceptableResponse);
                return notAcceptableResponse;
            }

            Logger.Info("Starting data store seeding");

            // bind the request to the specialized model
            var exchangeFileRequest = this.Bind<ExchangeFileUploadRequest>();

            // make sure there is only one file
            if (exchangeFileRequest.File == null)
            {
                var badRequestResponse = new Response().WithStatusCode(HttpStatusCode.BadRequest);
                this.HeaderInfoProvider.RegisterResponseHeaders(badRequestResponse);
                return badRequestResponse;
            }

            // stream the file to disk
            var filePath = await this.LocalFileStorage.StreamFileToDisk(exchangeFileRequest.File.Value);

            // drop existing data stores
            this.DropDataStoreAndPrepareNew();

            // handle exchange processing
            if (!this.InsertModelData(filePath, exchangeFileRequest.Password))
            {
                var errorResponse = new NotAcceptableResponse();
                this.HeaderInfoProvider.RegisterResponseHeaders(errorResponse);
                return errorResponse;
            }

            // Remove the exchange file after processing (saving space)
            try
            {
                this.LocalFileStorage.RemoveFileFromDisk(filePath);
            }
            catch (Exception ex)
            {
                // swallow exception but log it
                Logger.Error(ex, "Unable to remove file");
            }

            try
            {
                // Create a jsonb for each entry in the database
                this.CreateRevisionHistoryForEachEntry();

                // database was succesfully seeded 
                this.DataStoreController
                    .CloneDataStore(); // create a clone of the data store for future restore support

                // reset the credential cache as the underlying datastore was reset
                this.WebServiceAuthentication.ResetCredentialCache();

                var response = new Response().WithStatusCode(HttpStatusCode.OK);
                this.HeaderInfoProvider.RegisterResponseHeaders(response);

                Logger.Info("Finished the data store seed");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to dump the datastore");
                var errorResponse = new Response().WithStatusCode(HttpStatusCode.InternalServerError);
                this.HeaderInfoProvider.RegisterResponseHeaders(errorResponse);
                return errorResponse;
            }
        }

        /// <summary>
        /// Parse the url segments and return the data as serialized JSON
        /// </summary>
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
        /// True if successful
        /// </returns>
        internal bool InsertModelData(string fileName, string password = null, bool seed = true)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                var sw = new Stopwatch();
                if (seed)
                {
                    // clear database schemas if seeding
                    Logger.Info("Start clearing the current data store");
                    transaction = this.TransactionManager.SetupTransaction(ref connection, null);
                    this.TransactionManager.SetFullAccessState(true);
                    this.ClearDatabaseSchemas(transaction);
                    transaction.Commit();
                }

                sw.Start();
                Logger.Info("Start seeding the data");

                // use new transaction to for inserting the data
                transaction = this.TransactionManager.SetupTransaction(ref connection, null);
                this.TransactionManager.SetFullAccessState(true);

                // important, make sure to defer the constraints
                var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);

                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                this.TransactionManager.SetAuditLoggingState(transaction, false);

                // get sitedirectory data
                var items = this.ExchangeFileProcessor.ReadSiteDirectoryFromfile(fileName, password).ToList();

                // assign default password to all imported persons.
                foreach (var person in items.Where(i => i.GetType() == typeof(Person)).Cast<Person>())
                {
                    person.Password = AppConfig.Current.Defaults.PersonPassword;
                }

                var topContainer = items.SingleOrDefault(x => x.IsSameOrDerivedClass<TopContainer>()) as TopContainer;
                if (topContainer == null)
                {
                    Logger.Error("No Topcontainer item encountered");
                    throw new NoNullAllowedException("Topcontainer item needs to be present in the dataset");
                }

                this.RequestUtils.Cache = new List<Thing>(items);

                // setup Site Directory schema
                using (var siteDirCommand = new NpgsqlCommand())
                {
                    Logger.Info("Start Site Directory structure");
                    siteDirCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.SiteDirectoryDefinition.sql");

                    siteDirCommand.Connection = transaction.Connection;
                    siteDirCommand.Transaction = transaction;
                    siteDirCommand.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                var result = false;
                if (topContainer.GetType().Name == "SiteDirectory")
                {
                    // make sure single iterationsetups are set to unfrozen before persitence
                    this.FixSingleIterationSetups(items);
                    var siteDirectoryService =
                        this.ServiceProvider.MapToPersitableService<SiteDirectoryService>("SiteDirectory");

                    result = siteDirectoryService.Insert(transaction, "SiteDirectory", topContainer);
                }

                if (result)
                {
                    this.RequestUtils.QueryParameters = new QueryParameters();

                    // Add missing Person permissions
                    this.CreateMissingPersonPermissions(transaction);

                    var engineeringModelSetups =
                        items.Where(
                                x => x.ClassKind == ClassKind.EngineeringModelSetup
                                     && x.GetType() == typeof(EngineeringModelSetup)).Cast<EngineeringModelSetup>()
                            .ToList();
                    var engineeringModelService =
                        this.ServiceProvider.MapToPersitableService<EngineeringModelService>("EngineeringModel");
                    var iterationService = this.ServiceProvider.MapToPersitableService<IterationService>("Iteration");

                    foreach (var engineeringModelSetup in engineeringModelSetups)
                    {
                        // cleanup before handling TopContainer
                        this.RequestUtils.Cache.Clear();

                        // get referenced engineeringmodel data
                        var engineeringModelItems = this.ExchangeFileProcessor
                            .ReadEngineeringModelFromfile(fileName, password, engineeringModelSetup).ToList();

                        // should return one engineeringmodel topcontainer 
                        var engineeringModel = engineeringModelItems.Where(i => i.GetType() == typeof(EngineeringModel))
                            .Cast<EngineeringModel>().Single();
                        if (engineeringModel == null)
                        {
                            result = false;
                            break;
                        }

                        var dataPartition = CDP4Orm.Dao.Utils.GetEngineeringModelSchemaName(engineeringModel.Iid);
                        this.RequestUtils.Cache = new List<Thing>(engineeringModelItems);

                        if (!engineeringModelService.Insert(transaction, dataPartition, engineeringModel))
                        {
                            result = false;
                            break;
                        }

                        // Add missing Participant permissions
                        this.CreateMissingParticipantPermissions(transaction, engineeringModelSetup);

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(fileName, password);

                        var iterationSetups = items
                            .Where(
                                x => engineeringModelSetup.IterationSetup.Contains(x.Iid)
                                     && x.ClassKind == ClassKind.IterationSetup
                                     && x.GetType() == typeof(IterationSetup)).Cast<IterationSetup>().ToList();

                        // get current maximum iterationNumber and increase by one for the next value
                        int maxIterationNumber = iterationSetups.Select(x => x.IterationNumber).Max() + IterationNumberSequenceInitialization;

                        // reset the start number of iterationNumber sequence
                        this.EngineeringModelDao.ResetIterationNumberSequenceStartNumber(
                            transaction,
                            dataPartition,
                            maxIterationNumber);

                        var iterationInsertResult = true;
                        foreach (var iterationSetup in iterationSetups)
                        {
                            this.RequestUtils.Cache.Clear();
                            var iterationItems = this.ExchangeFileProcessor
                                .ReadModelIterationFromFile(fileName, password, iterationSetup).ToList();

                            // FixRevisionNumber(iterationItems);
                            this.RequestUtils.Cache = new List<Thing>(iterationItems);

                            // should return one iteration
                            var iteration =
                                iterationItems.SingleOrDefault(x => x.ClassKind == ClassKind.Iteration) as Iteration;
                            if (iteration == null || !iterationService.CreateConcept(
                                    transaction,
                                    dataPartition,
                                    iteration,
                                    engineeringModel))
                            {
                                iterationInsertResult = false;
                                break;
                            }

                            // bump the transaction time, so we can keep track of individual iterations
                            this.TransactionManager.UpdateTransactionStatementTime(transaction);
                        }

                        if (!iterationInsertResult)
                        {
                            result = false;
                            break;
                        }

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(fileName, password);
                    }
                }

                transaction.Commit();
                sw.Stop();
                Logger.Info("Finished seeding the data store in {0} [ms]", sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Error occured during data store seeding");

                return false;
            }
            finally
            {
                // clean log (will happen at end of request as well due to IOC lifetime
                this.TransactionManager.CommandLogger.ClearLog();

                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Persist the file binary data in the reference zip archive.
        /// </summary>
        /// <param name="fileName">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        private void PersistFileBinaryData(string fileName, string password = null)
        {
            var fileRevisions = this.RequestUtils.Cache.Where(i => i.GetType() == typeof(FileRevision))
                .Cast<FileRevision>().ToList();
            if (!fileRevisions.Any())
            {
                // nothing to do
                return;
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                this.ExchangeFileProcessor.StoreFileBinary(fileName, password, hash);
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
        private void FixSingleIterationSetups(List<Thing> items)
        {
            var engineeringModelSetups = items.Where(x => x.ClassKind == ClassKind.EngineeringModelSetup)
                .Where(i => i.GetType() == typeof(EngineeringModelSetup)).Cast<EngineeringModelSetup>().ToList();

            // unset the FrozenOn and SourceIterationSetup properties for single iteration setup in the siteDirectory.
            foreach (var iterationSetups in engineeringModelSetups.Select(
                engineeringModelSetup =>
                    items.Where(
                            x => engineeringModelSetup.IterationSetup.Contains(x.Iid)
                                 && x.ClassKind == ClassKind.IterationSetup && x.GetType() == typeof(IterationSetup))
                        .Cast<IterationSetup>().ToList()).Where(iterationSetups => iterationSetups.Count == 1))
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
        private void ClearDatabaseSchemas(NpgsqlTransaction transaction)
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
        private void DropDataStoreAndPrepareNew()
        {
            Logger.Info("start dropping existing data stores");

            // Drop the existing databases 
            using (var connection = new NpgsqlConnection(
                Services.Utils.GetConnectionString(AppConfig.Current.Backtier.DatabaseManage)))
            {
                connection.Open();

                // Drop the existing database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the data store");

                    this.DataStoreController.DropDataStoreConnections(AppConfig.Current.Backtier.Database, connection);

                    cmd.Connection = connection;
                    var commandDefinition = "DROP DATABASE IF EXISTS {0};";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/
                        AppConfig.Current.Backtier.Database);
                    cmd.ExecuteNonQuery();
                }

                // Drop the existing restore database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the restore data store");

                    cmd.Connection = connection;
                    var commandDefinition = "DROP DATABASE IF EXISTS {0};";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/
                        AppConfig.Current.Backtier.DatabaseRestore);
                    cmd.ExecuteNonQuery();
                }

                // Create a new database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Create the data store");
                    cmd.Connection = connection;
                    var commandDefinition = "CREATE DATABASE {0} WITH OWNER = {1} TEMPLATE = {2} ENCODING = 'UTF8';";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/
                        AppConfig.Current.Backtier.Database,
                        /*1*/
                        AppConfig.Current.Backtier.UserName,
                        /*2*/
                        AppConfig.Current.Backtier.DatabaseManage);
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Create revision history for each entry in the database.
        /// </summary>
        private void CreateRevisionHistoryForEachEntry()
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.RequestUtils.QueryParameters = new QueryParameters();

            try
            {
                // Create a revision history for SiteDirectory's entries
                transaction = this.TransactionManager.SetupTransaction(ref connection, null);
                this.TransactionManager.SetFullAccessState(true);

                // Save revision history for SiteDirectory's entries
                this.RevisionService.SaveRevisions(transaction, TopContainer, Guid.Empty, 0);

                var siteDirectory = this.SiteDirectoryService.Get(
                    transaction,
                    TopContainer,
                    null,
                    new RequestSecurityContext { ContainerReadAllowed = true }).OfType<SiteDirectory>().ToList();

                var engineeringModelSetups = this.EngineeringModelSetupService.GetShallow(
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
                    transaction = this.TransactionManager.SetupTransaction(ref connection, null);
                    this.TransactionManager.SetFullAccessState(true);

                    var partition =
                        this.RequestUtils.GetEngineeringModelPartitionString(engineeringModelSetup.EngineeringModelIid);

                    // Save revision history for EngineeringModel's entries
                    this.RevisionService.SaveRevisions(transaction, partition, Guid.Empty, 0);

                    // commit revision history
                    transaction.Commit();
                }
            }
            catch (NpgsqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Error occured during revision history creation");
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Create missing participant permissions.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The EngineeringModelSetup to create permissions for participants of it.
        /// </param>
        private void CreateMissingParticipantPermissions(
            NpgsqlTransaction transaction,
            EngineeringModelSetup engineeringModelSetup)
        {
            var participants = this.ParticipantService.GetShallow(
                transaction,
                TopContainer,
                engineeringModelSetup.Participant,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<Participant>().ToList();

            // Find and create all missing permissions for each Participant
            foreach (var participant in participants)
            {
                this.FindAndCreateMissingParticipantPermissions(participant, transaction);
            }
        }

        /// <summary>
        /// Find and create missing participant permissions.
        /// </summary>
        /// <param name="participant">
        /// The participant to find and create permissions for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingParticipantPermissions(Participant participant, NpgsqlTransaction transaction)
        {
            var participantRole = this.ParticipantRoleService.GetShallow(
                transaction,
                TopContainer,
                new List<Guid> { participant.Role },
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantRole>().ToList()[0];

            var participantPermissions = this.ParticipantPermissionService.GetShallow(
                transaction,
                TopContainer,
                participantRole.ParticipantPermission,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantPermission>().ToList();

            var defaultDefaultPermissionProvider = new CDP4Common.Helpers.DefaultPermissionProvider();

            // Iterate all available clasKinds to filter out classKinds a person misses permissions for
            foreach (var classKind in Enum.GetValues(typeof(ClassKind)).Cast<ClassKind>())
            {
                // Some classKinds might be the cause of an exception during retrieving a default permission (for instance NotThing)
                try
                {
                    var defaultPermission =
                        defaultDefaultPermissionProvider.GetDefaultParticipantPermission(classKind.ToString());

                    if (defaultPermission == ParticipantAccessRightKind.NONE)
                    {
                        var participantPermission = participantPermissions.Find(x => x.ObjectClass == classKind);

                        if (participantPermission == null)
                        {
                            var permission =
                                new ParticipantPermission(Guid.NewGuid(), 0)
                                    {
                                        ObjectClass = classKind,
                                        AccessRight = defaultPermission
                                    };

                            participantRole.ParticipantPermission.Add(permission.Iid);
                            this.ParticipantPermissionService.CreateConcept(
                                transaction,
                                TopContainer,
                                permission,
                                participantRole);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "There is no default permission for the given ClassKind: " + classKind);
                }
            }
        }

        /// <summary>
        /// Create missing person permissions.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateMissingPersonPermissions(NpgsqlTransaction transaction)
        {
            var siteDirectory = this.SiteDirectoryService.Get(
                transaction,
                TopContainer,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<SiteDirectory>().ToList()[0];

            var persons = this.PersonService.GetShallow(
                transaction,
                TopContainer,
                siteDirectory.Person,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<Person>().ToList();

            // Find and create all missing permissions for each Person
            foreach (var person in persons)
            {
                this.FindAndCreateMissingPersonPermissions(person, transaction);
            }
        }

        /// <summary>
        /// Find and create missing person permissions.
        /// </summary>
        /// <param name="person">
        /// The person to find and create permissions for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingPersonPermissions(Person person, NpgsqlTransaction transaction)
        {
            if (person.Role.HasValue)
            {
                var personRole = this.PersonRoleService.GetShallow(
                    transaction,
                    TopContainer,
                    new List<Guid> { person.Role.Value },
                    new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonRole>().ToList()[0];

                var personPermissions = this.PersonPermissionService.GetShallow(
                    transaction,
                    TopContainer,
                    personRole.PersonPermission,
                    new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonPermission>().ToList();

                var defaultDefaultPermissionProvider = new CDP4Common.Helpers.DefaultPermissionProvider();

                // Iterate all available clasKinds to filter out classKinds a person misses permissions for
                foreach (var classKind in Enum.GetValues(typeof(ClassKind)).Cast<ClassKind>())
                {
                    // Some classKinds might be the cause of an exception during retrieving a default permission (for instance NotThing)
                    try
                    {
                        var defaultPermission =
                            defaultDefaultPermissionProvider.GetDefaultPersonPermission(classKind.ToString());

                        if (defaultPermission == PersonAccessRightKind.NONE)
                        {
                            var personPermission = personPermissions.Find(x => x.ObjectClass == classKind);

                            if (personPermission == null)
                            {
                                var permission =
                                    new PersonPermission(Guid.NewGuid(), 0)
                                        {
                                            ObjectClass = classKind,
                                            AccessRight = defaultPermission
                                        };

                                personRole.PersonPermission.Add(permission.Iid);
                                this.PersonPermissionService.CreateConcept(
                                    transaction,
                                    TopContainer,
                                    permission,
                                    personRole);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "There is no default permission for the given ClassKind: " + classKind);
                    }
                }
            }
        }
    }
}