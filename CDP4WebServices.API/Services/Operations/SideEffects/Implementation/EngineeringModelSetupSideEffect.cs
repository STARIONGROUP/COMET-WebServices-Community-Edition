// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Authentication;
    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;

    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Services.Authorization;
    using NLog;
    using Npgsql;

    using EngineeringModelSetup = CDP4Common.DTO.EngineeringModelSetup;
    using IterationSetup = CDP4Common.DTO.IterationSetup;

    /// <summary>
    /// The purpose of the EngineeringModelSetup SideEffect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public class EngineeringModelSetupSideEffect : OperationSideEffect<EngineeringModelSetup>
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the engineering model service.
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// Gets or sets the engineering model setup service.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the iteration service.
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Gets or sets the revision service.
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Participant"/> service.
        /// </summary>
        public IParticipantService ParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup service.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the Option service.
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModelCreatorManager"/> (injected)
        /// </summary>
        public IModelCreatorManager ModelCreatorManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPersonResolver"/> (injected)
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation
        {
            get
            {
                return new[] { "ActiveDomain", "IterationSetup", "Participant", "RequiredRdl" };
            }
        }

        /// <summary>
        /// Execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void BeforeCreate(EngineeringModelSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.TransactionManager.SetFullAccessState(true);

            // validate that the SiteDirectory container has the Default Participant Role set
            var siteDir = (SiteDirectory)container;
            if (!siteDir.DefaultParticipantRole.HasValue)
            {
                var errorMessage = "The Default Participant Role must be set on the Site Directory";
                Logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (!thing.SourceEngineeringModelSetupIid.HasValue && !thing.ActiveDomain.Any())
            {
                // new engineeringmodelsetup without active domain, set the user's default domain as the active domain
                var defaultDomain = this.RequestUtils.Context.AuthenticatedCredentials.Person.DefaultDomain;
                if (defaultDomain != null)
                {
                    var domainId = defaultDomain.Value;
                    thing.ActiveDomain.Add(domainId);
                }
            }

            if (thing.SourceEngineeringModelSetupIid.HasValue)
            {
                if (thing.RequiredRdl.Any())
                {
                    throw new InvalidOperationException("The RequiredRdl cannot be specified if the EngineeringModel is created based on an existing EngineeringModel.");
                }

                this.ModelCreatorManager.CreateEngineeringModelSetupFromSource(thing.SourceEngineeringModelSetupIid.Value, thing, transaction, securityContext);
            }

            if (thing.EngineeringModelIid == Guid.Empty)
            {
                thing.EngineeringModelIid = Guid.NewGuid();
            }

            this.TransactionManager.SetFullAccessState(false);

            // TODO set default participant(s) iids (which are not contained) for engineeringmodelsetup (task T2818 CDP4WEBSERVICES)
        }

        /// <summary>
        /// Execute additional logic after a successful EngineeringModelSetup create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="EngineeringModelSetup"/> instance that has been created.
        /// </param>
        /// <param name="container">
        /// The container (<see cref="SiteDirectory"/>) instance of the <see cref="EngineeringModelSetup"/> that has been created.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterCreate(EngineeringModelSetup thing, Thing container, EngineeringModelSetup originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // reset the cache
            this.RequestUtils.Cache.Clear();

            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            var actor = credentials.Person.Iid;

            // at this point the engineering model schema has been created (handled in EngineeringModelSetupDao)
            if (thing.SourceEngineeringModelSetupIid.HasValue)
            {
                this.CreateCopyEngineeringModel(thing, container, transaction, partition, securityContext);

                // Create revisions for created EngineeringModel
                this.RevisionService.SaveRevisions(transaction, this.RequestUtils.GetEngineeringModelPartitionString(thing.EngineeringModelIid), actor, 0);

                return;
            }

            this.CreateDefaultEngineeringModel(thing, container, transaction, partition, securityContext);

            // Create revisions for created EngineeringModel
            this.RevisionService.SaveRevisions(transaction, this.RequestUtils.GetEngineeringModelPartitionString(thing.EngineeringModelIid), actor, 0);
        }

        /// <summary>
        /// Creates a new <see cref="EngineeringModel"/> from a source
        /// </summary>
        /// <param name="thing">The <see cref="EngineeringModelSetup"/></param>
        /// <param name="container">The container</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The security context</param>
        private void CreateCopyEngineeringModel(EngineeringModelSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // copy data from the source engineering-model
            this.ModelCreatorManager.CopyEngineeringModelData(thing, transaction, securityContext);
        }

        /// <summary>
        /// Create a new <see cref="EngineeringModel"/> from scratch
        /// </summary>
        /// <param name="thing">The <see cref="EngineeringModelSetup"/></param>
        /// <param name="container">The container</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The security context</param>
        private void CreateDefaultEngineeringModel(EngineeringModelSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // No need to create a model RDL for the created EngineeringModelSetup since is handled in the client. It happens in the same transaction as the creation of the EngineeringModelSetup itself
            var firstIterationSetup = this.CreateIterationSetup(thing, transaction, partition);

            this.CreateParticipant(thing, (SiteDirectory)container, transaction, partition);

            // The EngineeringModel schema (for the new EngineeringModelSetup) is already created from the DAO at this point, get its partition name
            var newEngineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(thing.EngineeringModelIid);

            // Create the engineering model in the newEngineeringModelPartition
            var engineeringModel = new EngineeringModel(thing.EngineeringModelIid, 1) { EngineeringModelSetup = thing.Iid };
            if (!this.EngineeringModelService.CreateConcept(transaction, newEngineeringModelPartition, engineeringModel, container))
            {
                var errorMessage = string.Format("There was a problem creating the new EngineeringModel: {0} from EngineeringModelSetup: {1}", engineeringModel.Iid, thing.Iid);
                Logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Create the first iteration in the newEngineeringModelPartition
            var firstIteration = new Iteration(firstIterationSetup.IterationIid, 1) { IterationSetup = firstIterationSetup.Iid };
            if (!this.IterationService.CreateConcept(transaction, newEngineeringModelPartition, firstIteration, engineeringModel))
            {
                var errorMessage = string.Format("There was a problem creating the new Iteration: {0} contained by EngineeringModel: {1}", firstIteration.Iid, engineeringModel.Iid);
                Logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // switch to iteration partition:
            var newIterationPartition = newEngineeringModelPartition.Replace(Utils.EngineeringModelPartition, Utils.IterationSubPartition);

            // Create a Domain FileStore for the current domain in the first iteration
            var newDomainFileStore = new DomainFileStore(Guid.NewGuid(), 1)
            {
                Owner = thing.ActiveDomain.Single(),
                CreatedOn = DateTime.UtcNow
            };

            if (!this.DomainFileStoreService.CreateConcept(transaction, newIterationPartition, newDomainFileStore, firstIteration))
            {
                var errorMessage = string.Format("There was a problem creating the new DomainFileStore: {0} contained by Iteration: {1}", newDomainFileStore.Iid, firstIteration.Iid);
                Logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            this.CreateDefaultOption(firstIteration, transaction, newIterationPartition);
        }
        
        /// <summary>
        /// Create the first <see cref="Option"/> in the <see cref="Iteration"/>
        /// </summary>
        /// <param name="container">
        /// The container <see cref="Iteration"/> of the <see cref="Option"/> that is to be created
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private void CreateDefaultOption(Iteration container, NpgsqlTransaction transaction, string partition)
        {
            var newOption = new Option(Guid.NewGuid(), 1)
            {
                Name = "Option 1",
                ShortName = "option_1"
            };
            if (!this.OptionService.CreateConcept(transaction, partition, newOption, container))
            {
                var errorMessage = string.Format("There was a problem creating the new Option: {0} contained by Iteration: {1}", newOption.Iid, container.Iid);
                Logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Create the first <see cref="IterationSetup"/> that is to be contained by the new <see cref="EngineeringModelSetup"/>
        /// </summary>
        /// <param name="thing">
        /// The <see cref="EngineeringModelSetup"/> instance that has been created.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private IterationSetup CreateIterationSetup(EngineeringModelSetup thing, NpgsqlTransaction transaction, string partition)
        {
            // create iteration setup in sitedirectory (= partition)
            var newIterationIid = Guid.NewGuid();
            var iterationSetup = new IterationSetup(Guid.NewGuid(), 1)
            {
                IterationNumber = 1,
                IterationIid = newIterationIid,
                Description = "Iteration 1"
            };

            if (!this.IterationSetupService.CreateConcept(transaction, partition, iterationSetup, thing))
            {
                throw new InvalidOperationException(string.Format("There was a problem creating the new IterationSetup: {0} contained by EngineeringModelSetup: {1}", iterationSetup.Iid, thing.Iid));
            }

            return iterationSetup;
        }

        /// <summary>
        /// Create a <see cref="Participant"/> in the new <see cref="EngineeringModelSetup"/> for the current <see cref="Person"/>
        /// </summary>
        /// The <see cref="EngineeringModelSetup"/> instance that has been created.
        /// <param name="thing">
        /// The <see cref="EngineeringModelSetup"/> instance that has been created.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private void CreateParticipant(EngineeringModelSetup thing, SiteDirectory container, NpgsqlTransaction transaction, string partition)
        {
            if (!container.DefaultParticipantRole.HasValue)
            {
                throw new InvalidOperationException("The Default Participant Role must be set on the Site Directory");
            }

            // use the default participant role as specified in the SiteDirectort container object
            var defaultRole = container.DefaultParticipantRole.Value;

            var currentPerson = this.RequestUtils.Context.AuthenticatedCredentials.Person;
            var defaultDomain = currentPerson.DefaultDomain;
            var participant = new Participant(Guid.NewGuid(), 1) { IsActive = true, Person = currentPerson.Iid, Role = defaultRole };
            if (defaultDomain != null)
            {
                var domainId = defaultDomain.Value;
                participant.Domain.Add(domainId);
                participant.SelectedDomain = domainId;
            }

            if (!this.ParticipantService.CreateConcept(transaction, partition, participant, thing))
            {
                throw new InvalidOperationException(
                    string.Format("There was a problem creating the new Participant: {0} contained by EngineeringModelSetup: {1}", participant.Iid, thing.Iid));
            }

            thing.Participant.Add(participant.Iid);

            if (!this.EngineeringModelSetupService.UpdateConcept(transaction, partition, thing, container))
            {
                throw new InvalidOperationException(
                    string.Format("There was a problem adding the new Participant: {0} to the EngineeringModelSetup: {1}", participant.Iid, thing.Iid));
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw Update Info.
        /// </param>
        public override void BeforeUpdate(EngineeringModelSetup thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            // TODO: if the EngineeringModelSetup has multiple iterations or is not in PREPARATION Phase and change on ActiveDomain should not be allowed (task T2818 CDP4WEBSERVICES)
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterUpdate(EngineeringModelSetup thing, Thing container, EngineeringModelSetup originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var newEngineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(thing.EngineeringModelIid);
            var newIterationPartition = newEngineeringModelPartition.Replace(
                Utils.EngineeringModelPartition,
                Utils.IterationSubPartition);
            var iterationSetups = this.IterationSetupService.GetShallow(transaction, partition, thing.IterationSetup, securityContext).Cast<IterationSetup>();
            var iterations = this.IterationService.GetShallow(transaction, newEngineeringModelPartition, iterationSetups.Select(i => i.IterationIid), securityContext).Cast<Iteration>();

            // determine which domains have been added and removed
            var removedDomains = originalThing.ActiveDomain.Except(thing.ActiveDomain).ToList();
            var addedDomains = thing.ActiveDomain.Except(originalThing.ActiveDomain).ToList();

            foreach (var iteration in iterations)
            {
                var domainFileStores = this.DomainFileStoreService.GetShallow(transaction, newIterationPartition, iteration.DomainFileStore, securityContext).Cast<DomainFileStore>().ToList();

                // if domain was removed, remove the DomainFileStore as well
                foreach (var removedDomain in removedDomains)
                {
                    // see if one already exists
                    var domainFileStore = domainFileStores.SingleOrDefault(dfs => dfs.Owner.Equals(removedDomain));

                    if (domainFileStore == null)
                    {
                        // indicates that the model is in a broken state (no DomainFileStore for the given DomainOfExpertise). Be forgiving and allow the code to return to a steady state.
                        continue;
                    }

                    if (!this.DomainFileStoreService.DeleteConcept(transaction, newIterationPartition, domainFileStore, iteration))
                    {
                        throw new InvalidOperationException(
                            string.Format("There was a problem deleting the DomainFileStore: {0} contained by Iteration: {1}", domainFileStore.Iid, iteration.Iid));
                    }
                }

                // create domain file store for every added domain
                foreach (var addedDomain in addedDomains)
                {
                    // see if one already exists
                    if (domainFileStores.Any(dfs => dfs.Owner.Equals(addedDomain)))
                    {
                        // indicates the model is in a broken state (a DomainFileStore was not cleaned up previously). Be forgiving and allow the domain to relink to it.
                        continue;
                    }

                    var newDomainFileStore = new DomainFileStore(Guid.NewGuid(), 1)
                    {
                        Owner = addedDomain,
                        CreatedOn = DateTime.UtcNow
                    };

                    if (!this.DomainFileStoreService.CreateConcept(transaction, newIterationPartition, newDomainFileStore, iteration))
                    {
                        throw new InvalidOperationException(
                            string.Format("There was a problem creating the new DomainFileStore: {0} contained by Iteration: {1}", newDomainFileStore.Iid, iteration.Iid));
                    }
                }
            }
        }

        /// <summary>
        /// Execute additional logic after a successful EngineeringModelSetup delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="EngineeringModelSetup"/> instance that has been removed.
        /// </param>
        /// <param name="container">
        /// The container (<see cref="SiteDirectory"/>) instance of the <see cref="EngineeringModelSetup"/> that has been removed.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted from.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void AfterDelete(EngineeringModelSetup thing, Thing container, EngineeringModelSetup originalThing, NpgsqlTransaction transaction,  string partition, ISecurityContext securityContext)
        {
            var newEngineeringModelPartition = this.RequestUtils.GetEngineeringModelPartitionString(thing.EngineeringModelIid);

            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            credentials.EngineeringModelSetup = thing;
            this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
            this.PermissionService.Credentials = credentials;

            var engineeringModel = this.EngineeringModelService.GetShallow(transaction, newEngineeringModelPartition, new[] { thing.EngineeringModelIid }, securityContext).SingleOrDefault();
            if (engineeringModel == null)
            {
                return;
            }

            // Delete the EngineeringModel associated to the EngineeringModelSetup that has been deleted
            if (!this.EngineeringModelService.DeleteConcept(transaction, newEngineeringModelPartition, engineeringModel))
            {
                throw new InvalidOperationException(
                    string.Format("There was a problem deleting the EngineeringModel: {0} for EngineeringModelSetup: {1}", engineeringModel.Iid, thing.Iid));
            }
        }
    }
}

