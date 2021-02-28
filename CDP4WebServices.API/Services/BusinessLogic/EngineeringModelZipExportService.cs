// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelZipExportService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CDP4Common.CommonData;
    using CDP4Common.Comparers;
    using CDP4Common.DTO;
    using CDP4Common.Extensions;

    using CDP4JsonSerializer;

    using CometServer.Helpers;
    using CometServer.Services.Authentication;
    using CometServer.Services.Authorization;
    using CometServer.Services.Protocol;

    using Ionic.Zip;

    using NLog;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The purpose of the <see cref="EngineeringModelZipExportService"/> is to provide file functions for exporting an engineering model 
    /// in a zipped archive.
    /// </summary>
    public class EngineeringModelZipExportService : IEngineeringModelZipExportService
    {
        /// <summary>
        /// The site RDL zip location.
        /// </summary>
        private const string SiteRdlZipLocation = "SiteReferenceDataLibraries";

        /// <summary>
        /// The model RDL zip location.
        /// </summary>
        private const string ModelRdlZipLocation = "ModelReferenceDataLibraries";

        /// <summary>
        /// The engineering model zip location.
        /// </summary>
        private const string EngineeringModelZipLocation = "EngineeringModels";

        /// <summary>
        /// The iteration zip location.
        /// </summary>
        private const string IterationZipLocation = "Iterations";

        /// <summary>
        /// The FileRevisions zip location.
        /// </summary>
        private const string FileRevisionZipLocation = "FileRevisions";

        /// <summary>
        /// A remark to be included in the exchange header file.
        /// </summary>
        private const string ExchangeHeaderRemark = "This is an ECSS-E-TM-10-25 exchange file";

        /// <summary>
        /// The copyright text to be included in the exchange header file.
        /// </summary>
        private const string ExchangeHeaderCopyright = "Copyright 2017 © RHEA System S.A.";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the CDP4 request context.
        /// </summary>
        public ICdp4RequestContext Cdp4Context { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager for this request.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

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
        /// Gets or sets the site reference data library service.
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the model reference data library service.
        /// </summary>
        public IModelReferenceDataLibraryService ModelReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the person service.
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        public ICdp4JsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// Gets or sets the Utils instance for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the permission service.
        /// </summary>
        public IPermissionService PermissionService { get; set; }

        /// <summary>
        /// Gets or sets the person resolver service.
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets or sets the email address service.
        /// </summary>
        public IEmailAddressService EmailAddressService { get; set; }

        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }

        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public ISiteDirectoryService SiteDirectoryService { get; set; }

        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public IDomainOfExpertiseService DomainOfExpertiseService { get; set; }

        /// <summary>
        /// Gets or sets the file binary service.
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Create a zip export file with EngineeringModels from supplied EngineeringModelSetupGuids and paths to files.
        /// </summary>
        /// <param name="engineeringModelSetupGuids">
        /// The provided list of <see cref="Guid"/> of EngineeringModelSetups to write to the zip file
        /// </param>
        /// <param name="files">
        /// The path to the files that need to be uploaded. If <paramref name="files"/> is null, then no files are to be uploaded
        /// </param>
        /// <returns>
        /// The <see cref="string"/> path of the created archive.
        /// </returns>
        public string CreateZipExportFile(
            IEnumerable<Guid> engineeringModelSetupGuids,
            IEnumerable<string> files = null)
        {
            // Initialize the json serializer
            this.JsonSerializer.Initialize(
                this.RequestUtils.MetaInfoProvider,
                this.RequestUtils.GetRequestDataModelVersion);

            var siteReferenceDataLibraries = new HashSet<SiteReferenceDataLibrary>(new DtoThingIidComparer());
            var credentials = this.Cdp4Context.Context.CurrentUser as Credentials;
            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };

            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

            try
            {
                var siteDirectoryPartition = "SiteDirectory";

                SiteDirectory siteDirectory = this.SiteDirectoryService
                    .GetShallow(transaction, siteDirectoryPartition, null, authorizedContext).OfType<SiteDirectory>()
                    .ToList()[0];

                var engineeringModelSetupThings = this.EngineeringModelSetupService.GetDeep(
                    transaction,
                    siteDirectoryPartition,
                    engineeringModelSetupGuids,
                    authorizedContext).ToList();

                // Get required EngineeringModelSetups
                var engineeringModelSetups = engineeringModelSetupThings
                    .Where(x => x.ClassKind == ClassKind.EngineeringModelSetup).OfType<EngineeringModelSetup>()
                    .ToList();

                // Get all DomainOfExpertises that are contained by the EngineeringModelSetups
                var domainsOfExpertises = new HashSet<DomainOfExpertise>(new DtoThingIidComparer());
                foreach (var engineeringModelSetup in engineeringModelSetups)
                {
                    var activeDomains = this.DomainOfExpertiseService
                        .GetShallow(
                            transaction,
                            siteDirectoryPartition,
                            engineeringModelSetup.ActiveDomain,
                            authorizedContext).OfType<DomainOfExpertise>().ToList();

                    activeDomains.ForEach(x => domainsOfExpertises.Add(x));
                }

                // Get all ModelReferenceDataLibraris that are contained by the EngineeringModelSetups
                var modelReferenceDataLibraries = engineeringModelSetupThings
                    .Where(x => x.ClassKind == ClassKind.ModelReferenceDataLibrary).OfType<ModelReferenceDataLibrary>()
                    .ToList();

                // Get all Persons that are contained by the EngineeringModelSetups
                var siteDirectoryPersons = this.PersonService
                    .GetShallow(transaction, siteDirectoryPartition, null, authorizedContext).OfType<Person>().ToList();
                var participants = engineeringModelSetupThings.Where(x => x.ClassKind == ClassKind.Participant)
                    .OfType<Participant>().ToList();
                var persons = this.GetPersonsFromParticipants(siteDirectoryPersons, participants);

                // Get all unique SiteReferenceDataLibraries that are referenced by ModelReferenceDataLibraries
                this.GetSiteReferenceDataLibraries(
                    modelReferenceDataLibraries,
                    siteReferenceDataLibraries,
                    transaction,
                    authorizedContext,
                    siteDirectoryPartition);

                var prunedSiteDirectoryDtos = this.CreateSiteDirectoryAndPrunedContainedThingDtos(
                    siteDirectory,
                    siteReferenceDataLibraries,
                    domainsOfExpertises,
                    persons,
                    engineeringModelSetups,
                    transaction,
                    authorizedContext,
                    siteDirectoryPartition);

                var activePersonGuidList = new List<Guid> { this.Cdp4Context.AuthenticatedCredentials.Person.Iid };

                var activePerson = this.PersonService
                    .GetShallow(transaction, siteDirectoryPartition, activePersonGuidList, authorizedContext)
                    .OfType<Person>().ToList()[0];

                var exchangeFileHeader = this.CreateExchangeFileHeader(
                    activePerson,
                    transaction,
                    authorizedContext,
                    siteDirectoryPartition);

                // Specify a random name for an archive.
                string path = Guid.NewGuid() + ".zip";

                try
                {
                    using (var zipFile = new ZipFile())
                    {
                        this.WriteHeaderToZipFile(exchangeFileHeader, zipFile, path);

                        this.WriteSiteDirectoryToZipFile(prunedSiteDirectoryDtos, zipFile, path);

                        this.WriteSiteReferenceDataLibraryToZipFile(
                            siteReferenceDataLibraries,
                            zipFile,
                            path,
                            transaction,
                            authorizedContext,
                            siteDirectoryPartition);

                        this.WriteModelReferenceDataLibraryToZipFile(
                            modelReferenceDataLibraries,
                            zipFile,
                            path,
                            transaction,
                            authorizedContext,
                            siteDirectoryPartition);

                        this.WriteModelsWithIterationsToZipFile(
                            engineeringModelSetups,
                            zipFile,
                            path,
                            transaction,
                            authorizedContext);
                    }

                    Logger.Info("Successfully exported selected models to {0}.", path);
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to export the open session to {0}. Error: {1}", path, ex.Message);

                    System.IO.File.Delete(path);
                    path = null;
                }

                transaction.Commit();

                return path;
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error("Failed to export the open session. Error: {0}", ex.Message);

                return string.Empty;
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
        /// Gets the <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> for each supplied <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/>
        /// </summary>
        /// <param name="modelReferenceDataLibraries">
        /// The <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/> to get <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> for
        /// </param>
        /// <param name="siteReferenceDataLibraries">
        /// The <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> set where to store found <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void GetSiteReferenceDataLibraries(
            List<ModelReferenceDataLibrary> modelReferenceDataLibraries,
            HashSet<SiteReferenceDataLibrary> siteReferenceDataLibraries,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            foreach (var modelReferenceDataLibrary in modelReferenceDataLibraries)
            {
                var requiredRdlGuid = modelReferenceDataLibrary.RequiredRdl;
                if (requiredRdlGuid != null)
                {
                    this.GetSiteRdlChain(
                        (Guid)requiredRdlGuid,
                        siteReferenceDataLibraries,
                        transaction,
                        authorizedContext,
                        partition);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> chain for each supplied <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/> guid
        /// </summary>
        /// <param name="requiredRdlGuid">
        /// The <see cref="Guid"/> of the <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> to get the chain for
        /// </param>
        /// <param name="siteReferenceDataLibraries">
        /// The <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/> set where to store found <see cref="CDP4Common.DTO.SiteReferenceDataLibrary"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void GetSiteRdlChain(
            Guid requiredRdlGuid,
            HashSet<SiteReferenceDataLibrary> siteReferenceDataLibraries,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            var requiredRdlGuidList = new List<Guid>();
            requiredRdlGuidList.Add(requiredRdlGuid);
            var requiredRdl = this.SiteReferenceDataLibraryService
                .GetShallow(transaction, partition, requiredRdlGuidList, authorizedContext)
                .OfType<SiteReferenceDataLibrary>().ToList()[0];

            siteReferenceDataLibraries.Add(requiredRdl);

            var nextRequiredRdlGuid = requiredRdl.RequiredRdl;
            if (nextRequiredRdlGuid != null)
            {
                this.GetSiteRdlChain(
                    (Guid)nextRequiredRdlGuid,
                    siteReferenceDataLibraries,
                    transaction,
                    authorizedContext,
                    partition);
            }
        }

        /// <summary>
        /// Gets unique persons from participants.
        /// </summary>
        /// <param name="siteDirectoryPersons">
        /// The site directory persons.
        /// </param>
        /// <param name="participants">
        /// The participants from required <see cref="CDP4Common.DTO.EngineeringModelSetup"/>.
        /// </param>
        /// <returns>
        /// The set of unique persons that belong to the required <see cref="CDP4Common.DTO.EngineeringModelSetup"/>.
        /// </returns>
        private HashSet<Person> GetPersonsFromParticipants(
            IEnumerable<Person> siteDirectoryPersons,
            IEnumerable<Participant> participants)
        {
            var personSet = new HashSet<Person>();

            foreach (var participant in participants)
            {
                var person = siteDirectoryPersons.Where(x => x.Iid == participant.Person).ToList();
                if (person.Count > 0)
                {
                    personSet.Add(person[0]);
                }
            }

            return personSet;
        }

        /// <summary>
        /// Creates a  <see cref="IEnumerable{Thing}"/> that contains references to those objects that are to be included
        /// in the JSON file
        /// </summary>
        /// <param name="siteDirectory">
        /// The <see cref="SiteDirectory"/> object that is to be serialized to the JSON file
        /// </param>
        /// <param name="siteReferenceDataLibraries">
        /// The <see cref="SiteReferenceDataLibrary"/> instances that are to be included
        /// </param>
        /// <param name="domainOfExpertises">
        /// The <see cref="DomainOfExpertise"/> instances that are to be included
        /// </param>
        /// <param name="persons">
        /// /// The <see cref="Person"/> instances that are to be included
        /// </param>
        /// <param name="engineeringModelSetups">
        /// The engineering Model Setups.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The pruned <see cref="Thing"/> collection.
        /// </returns>
        private IEnumerable<Thing> CreateSiteDirectoryAndPrunedContainedThingDtos(
            SiteDirectory siteDirectory,
            HashSet<SiteReferenceDataLibrary> siteReferenceDataLibraries,
            HashSet<DomainOfExpertise> domainOfExpertises,
            HashSet<Person> persons,
            IEnumerable<EngineeringModelSetup> engineeringModelSetups,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            var dtos = new List<Thing>();

            var siteDirectoryItems = this.SiteDirectoryService.GetDeep(
                transaction,
                partition,
                new List<Guid> { siteDirectory.Iid },
                authorizedContext);

            SiteDirectory siteDirectoryDto = siteDirectory.DeepClone<SiteDirectory>();
            dtos.Add(siteDirectoryDto);

            // collect the SiteDirectory object graph, except for the graph branches that need to be pruned
            foreach (var siteDirectoryItem in siteDirectoryItems)
            {
                if (siteDirectoryItem.ClassKind != ClassKind.SiteDirectory
                    && siteDirectoryItem.ClassKind != ClassKind.SiteReferenceDataLibrary
                    && siteDirectoryItem.ClassKind != ClassKind.DomainOfExpertise
                    && siteDirectoryItem.ClassKind != ClassKind.Person
                    && siteDirectoryItem.ClassKind != ClassKind.EngineeringModelSetup)
                {
                    var containerClassNameList = this.GetContainerClassNameList(siteDirectoryItem);
                    var exclude = this.CheckClassNameForExclusion(containerClassNameList);

                    if (!exclude)
                    {
                        dtos.Add(siteDirectoryItem);
                    }
                }
            }

            // remove the domains-of-expertise that should not be included in the cloned SiteDirectory
            siteDirectoryDto.Domain.Clear();
            foreach (var domainOfExpertise in domainOfExpertises)
            {
                var domainOfExpertiseDtos = this.DomainOfExpertiseService.GetDeep(
                    transaction,
                    partition,
                    new List<Guid> { domainOfExpertise.Iid },
                    authorizedContext);
                dtos.AddRange(domainOfExpertiseDtos);

                siteDirectoryDto.Domain.Add(domainOfExpertise.Iid);
            }

            // remove the persons that should not be included in SiteDirectory DTO from the DTO
            siteDirectoryDto.Person.Clear();
            foreach (var person in persons)
            {
                var personDtos = this.PersonService.GetDeep(
                    transaction,
                    partition,
                    new List<Guid> { person.Iid },
                    authorizedContext);
                dtos.AddRange(personDtos);
                siteDirectoryDto.Person.Add(person.Iid);
            }

            // remove the EngineeringModelSetup instances and replace with only required ones
            siteDirectoryDto.Model.Clear();
            foreach (var engineeringModelSetup in engineeringModelSetups)
            {
                // retrieve the EngineeringModelSetup contained object graph, including a shallow copy of the ModelReferenceDataLibrary
                // the ModelReferenceDataLibrary contained objects are written to its respective JSON file
                var engineeringModelSetupDtos = this.EngineeringModelSetupService.GetDeep(
                    transaction,
                    partition,
                    new List<Guid> { engineeringModelSetup.Iid },
                    authorizedContext);
                dtos.AddRange(engineeringModelSetupDtos);

                siteDirectoryDto.Model.Add(engineeringModelSetup.Iid);
            }

            // remove the SiteReferenceDataLibrary instances and replace with only referenced ones
            siteDirectoryDto.SiteReferenceDataLibrary.Clear();
            foreach (var siteReferenceDataLibrary in siteReferenceDataLibraries)
            {
                dtos.Add(siteReferenceDataLibrary);

                siteDirectoryDto.SiteReferenceDataLibrary.Add(siteReferenceDataLibrary.Iid);
            }

            // Clean the list from duplicates (for instance Definition)
            return dtos.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Gets a list of all containers class name in the hierarchy.
        /// </summary>
        /// <param name="siteDirectoryItem">
        /// The site directory item.
        /// </param>
        /// <returns>
        /// The list of containers class name.
        /// </returns>
        private List<string> GetContainerClassNameList(Thing siteDirectoryItem)
        {
            var containerClassNameList = new List<string>();

            this.GetContainerClassName(siteDirectoryItem.ClassKind.ToString(), containerClassNameList);

            return containerClassNameList;
        }

        /// <summary>
        /// Gets a class name of the container of a given class name.
        /// </summary>
        /// <param name="siteDirectoryItemClassName">
        /// The site directory item.
        /// </param>
        /// <param name="containerClassNameList">
        /// The list where to add the found container class name.
        /// </param>
        private void GetContainerClassName(string siteDirectoryItemClassName, List<string> containerClassNameList)
        {
            var containerClassName =
                CDP4Common.Helpers.ContainerPropertyHelper.ContainerClassName(siteDirectoryItemClassName);

            // Check whether it is a top container
            if (containerClassName != siteDirectoryItemClassName)
            {
                containerClassNameList.Add(containerClassName);

                // There is no case in CDP4Common.Helpers.ContainerPropertyHelper to return DefinedThing for the supplied DefinedThing as it is done for the top containers
                if (containerClassName != "DefinedThing")
                {
                    this.GetContainerClassName(containerClassName, containerClassNameList);
                }
            }
        }

        /// <summary>
        /// Checks whether the thing should be excluded based on the container class name in a given list.
        /// </summary>
        /// <param name="containerClassNameList">
        /// The container class name list.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether to exclude the thing or not.
        /// </returns>
        private bool CheckClassNameForExclusion(List<string> containerClassNameList)
        {
            var isExclusion = containerClassNameList.Contains(ClassKind.SiteReferenceDataLibrary.ToString())
                              || containerClassNameList.Contains(ClassKind.DomainOfExpertise.ToString())
                              || containerClassNameList.Contains(ClassKind.Person.ToString())
                              || containerClassNameList.Contains(ClassKind.EngineeringModelSetup.ToString());

            return isExclusion;
        }

        /// <summary>
        /// Factory method that creates a <see cref="ExchangeFileHeader"/> based on the provided <see cref="person"/>
        /// </summary>
        /// <param name="person">
        /// The <see cref="Person"/> that is used to create the <see cref="ExchangeFileHeader"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ExchangeFileHeader"/>
        /// </returns>
        private ExchangeFileHeader CreateExchangeFileHeader(
            Person person,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            EmailAddress email = null;

            if (person.DefaultEmailAddress != null)
            {
                email = this.EmailAddressService.GetShallow(
                    transaction,
                    partition,
                    new List<Guid> { (Guid)person.DefaultEmailAddress },
                    authorizedContext).OfType<EmailAddress>().ToList()[0];
            }

            var exchangeFileInitiator =
                new ExchangeFileInitiator
                    {
                        Iid = person.Iid,
                        GivenName = person.GivenName,
                        Surname = person.Surname,
                        Email = email != null ? email.Value : string.Empty
                    };

            Organization organizationDto = null;

            if (person.Organization != null)
            {
                organizationDto = this.OrganizationService
                    .GetShallow(transaction, partition, new List<Guid> { (Guid)person.Organization }, authorizedContext)
                    .OfType<Organization>().ToList()[0];
            }

            var organization = organizationDto != null
                                   ? new OrganizationInfo
                                         {
                                             Iid = organizationDto.Iid,
                                             Name = organizationDto.Name,
                                             Site = null,
                                             Unit = !string.IsNullOrEmpty(person.OrganizationalUnit)
                                                        ? person.OrganizationalUnit
                                                        : null
                                         }
                                   : null;

            var exchangeFileHeader =
                new ExchangeFileHeader
                    {
                        DataModelVersion = "2.4.1",
                        Remark = ExchangeHeaderRemark,
                        Copyright = ExchangeHeaderCopyright,
                        Extensions = null,
                        CreatorPerson = exchangeFileInitiator,
                        CreatorOrganization = organization
                    };

            return exchangeFileHeader;
        }

        /// <summary>
        /// Write the header file to the zip export archive.
        /// </summary>
        /// <param name="echExchangeFileHeader">
        /// The <see cref="ExchangeFileHeader"/> that is to be written to the <see cref="zipFile"/>
        /// </param>
        /// <param name="zipFile">
        /// The zip archive instance to add the information to.
        /// </param>
        /// <param name="filePath">
        /// The path of the file.
        /// </param>
        private void WriteHeaderToZipFile(ExchangeFileHeader echExchangeFileHeader, ZipFile zipFile, string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.JsonSerializer.SerializeToStream(echExchangeFileHeader, memoryStream);
                using (var outputStream = new MemoryStream(memoryStream.ToArray()))
                {
                    var zipEntry = zipFile.AddEntry("Header.json", outputStream);
                    zipEntry.Comment = "The Header for this file based source";
                    zipFile.Save(filePath);
                }
            }
        }

        /// <summary>
        /// Writes the pruned <see cref="CDP4Common.SiteDirectoryData.SiteDirectory"/> to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="prunedSiteDirectoryContents">
        /// The <see cref="CDP4Common.SiteDirectoryData.SiteDirectory"/> that has been pruned of all unnecessary data
        /// </param>
        /// <param name="zipFile">
        /// The target <see cref="ZipFile"/>
        /// </param>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        private void WriteSiteDirectoryToZipFile(
            IEnumerable<Thing> prunedSiteDirectoryContents,
            ZipFile zipFile,
            string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.JsonSerializer.SerializeToStream(prunedSiteDirectoryContents, memoryStream);
                using (var outputStream = new MemoryStream(memoryStream.ToArray()))
                {
                    var zipEntry = zipFile.AddEntry("SiteDirectory.json", outputStream);
                    zipEntry.Comment = "The SiteDirectory for this file based source";
                    zipFile.Save(filePath);
                }
            }
        }

        /// <summary>
        /// Writes the <see cref="CDP4Common.SiteDirectoryData.SiteReferenceDataLibrary"/> to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="siteReferenceDataLibraries">
        /// The <see cref="CDP4Common.SiteDirectoryData.SiteReferenceDataLibrary"/> that are to be written to the <see cref="ZipFile"/>
        /// </param>
        /// <param name="zipFile">
        /// The target <see cref="ZipFile"/> that the <see cref="siteReferenceDataLibraries"/> are written to.
        /// </param>
        /// <param name="filePath">
        /// The file of the target <see cref="ZipFile"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void WriteSiteReferenceDataLibraryToZipFile(
            IEnumerable<SiteReferenceDataLibrary> siteReferenceDataLibraries,
            ZipFile zipFile,
            string filePath,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            // Force deep get for reference data libraries
            var queryParameters = new QueryParameters { IncludeReferenceData = true, ExtentDeep = true };
            this.RequestUtils.OverrideQueryParameters = queryParameters;

            foreach (var siteReferenceDataLibrary in siteReferenceDataLibraries)
            {
                var siteReferenceDataLibraryGuidList = new List<Guid>();
                siteReferenceDataLibraryGuidList.Add(siteReferenceDataLibrary.Iid);

                // Get all siteRdl objects excluding siteRdl itself
                var dtos = this.SiteReferenceDataLibraryService
                    .GetDeep(transaction, partition, siteReferenceDataLibraryGuidList, authorizedContext).ToList()
                    .Where(x => x.ClassKind != ClassKind.SiteReferenceDataLibrary);

                // Serialize and save dtos to the archive
                using (var memoryStream = new MemoryStream())
                {
                    this.JsonSerializer.SerializeToStream(dtos, memoryStream);
                    using (var outputStream = new MemoryStream(memoryStream.ToArray()))
                    {
                        var siteReferenceDataLibraryFilename = string.Format(
                            "{0}\\{1}.json",
                            SiteRdlZipLocation,
                            siteReferenceDataLibrary.Iid);
                        var zipEntry = zipFile.AddEntry(siteReferenceDataLibraryFilename, outputStream);
                        zipEntry.Comment = string.Format(
                            "The {0} SiteReferenceDataLibrary",
                            siteReferenceDataLibrary.ShortName);
                        zipFile.Save(filePath);
                    }
                }
            }

            // Set query parameters back
            this.RequestUtils.OverrideQueryParameters = null;
        }

        /// <summary>
        /// Writes the <see cref="CDP4Common.DTO.ModelReferenceDataLibrary"/> to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="modelReferenceDataLibraries">
        /// The <see cref="CDP4Common.SiteDirectoryData.ModelReferenceDataLibrary"/> that are to be written to the <see cref="ZipFile"/>
        /// </param>
        /// <param name="zipFile">
        /// The target <see cref="ZipFile"/> that the <see cref="modelReferenceDataLibraries"/> are written to.
        /// </param>
        /// <param name="filePath">
        /// The file of the target <see cref="ZipFile"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void WriteModelReferenceDataLibraryToZipFile(
            IEnumerable<ModelReferenceDataLibrary> modelReferenceDataLibraries,
            ZipFile zipFile,
            string filePath,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext,
            string partition)
        {
            // Force deep get for reference data libraries
            var queryParameters = new QueryParameters { IncludeReferenceData = true, ExtentDeep = true };
            this.RequestUtils.OverrideQueryParameters = queryParameters;

            foreach (var modelReferenceDataLibrary in modelReferenceDataLibraries)
            {
                // Get all modelRdl objects excluding modelRdl itself
                var dtos = this.ModelReferenceDataLibraryService
                    .GetDeep(
                        transaction,
                        partition,
                        new List<Guid> { modelReferenceDataLibrary.Iid },
                        authorizedContext).ToList().Where(x => x.ClassKind != ClassKind.ModelReferenceDataLibrary);

                // Serialize and save dtos to the archive
                using (var memoryStream = new MemoryStream())
                {
                    this.JsonSerializer.SerializeToStream(dtos, memoryStream);
                    using (var outputStream = new MemoryStream(memoryStream.ToArray()))
                    {
                        var modelReferenceDataLibraryFilename = string.Format(
                            "{0}\\{1}.json",
                            ModelRdlZipLocation,
                            modelReferenceDataLibrary.Iid);
                        var zipEntry = zipFile.AddEntry(modelReferenceDataLibraryFilename, outputStream);
                        zipEntry.Comment = string.Format(
                            "The {0} ModelReferenceDataLibrary",
                            modelReferenceDataLibrary.ShortName);
                        zipFile.Save(filePath);
                    }
                }
            }

            // Set query parameters back
            this.RequestUtils.OverrideQueryParameters = null;
        }

        /// <summary>
        /// Writes the <see cref="CDP4Common.DTO.EngineeringModel"/> and its <see cref="CDP4Common.DTO.Iteration"/> to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="engineeringModelSetups">
        /// The <see cref="CDP4Common.DTO.EngineeringModelSetup"/> which <see cref="CDP4Common.DTO.EngineeringModel"/> and its <see cref="CDP4Common.DTO.Iteration"/> are to be written to the <see cref="ZipFile"/>
        /// </param>
        /// <param name="zipFile">
        /// The target <see cref="ZipFile"/> that the <see cref="CDP4Common.DTO.EngineeringModel"/> and its <see cref="CDP4Common.DTO.Iteration"/> are written to.
        /// </param>
        /// <param name="filePath">
        /// The file of the target <see cref="ZipFile"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        private void WriteModelsWithIterationsToZipFile(
            IEnumerable<EngineeringModelSetup> engineeringModelSetups,
            ZipFile zipFile,
            string filePath,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext)
        {
            string pattern = "-";
            string replacement = "_";
            Regex rgx = new Regex(pattern);

            // Get the state of the current credentials and the participant flag 
            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            var currentParticipantFlag = credentials.IsParticipant;

            // As EngineeringModel data will be retrieved the participant flag needs to be set to true
            credentials.IsParticipant = true;

            foreach (var engineeringModelSetup in engineeringModelSetups)
            {
                var modelPartition = rgx.Replace(
                    "EngineeringModel_" + engineeringModelSetup.EngineeringModelIid,
                    replacement);

                // Get participant permissions as EngineeringModel data will be retrieved
                credentials.EngineeringModelSetup = engineeringModelSetup;
                this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
                this.PermissionService.Credentials = credentials;

                // Get all engineeringModel objects
                var engineeringModelDtos = this.EngineeringModelService.GetDeep(
                    transaction,
                    modelPartition,
                    new List<Guid> { engineeringModelSetup.EngineeringModelIid },
                    authorizedContext).ToList();

                if (engineeringModelDtos.Count == 0)
                {
                    throw new UnauthorizedAccessException($"Person {this.RequestUtils.Context.AuthenticatedCredentials.Person.UserName} is not authorized to access model {engineeringModelSetup.EngineeringModelIid}");
                }

                var fileRevisions = engineeringModelDtos.Where(x => x.ClassKind == ClassKind.FileRevision)
                    .OfType<FileRevision>().ToList();

                // Serialize and save dtos to the archive
                using (var engineeringModelMemoryStream = new MemoryStream())
                {
                    this.JsonSerializer.SerializeToStream(engineeringModelDtos, engineeringModelMemoryStream);

                    using (var outputStream = new MemoryStream(engineeringModelMemoryStream.ToArray()))
                    {
                        var engineeringModelFilename = string.Format(
                            @"{0}\{1}\{1}.json",
                            EngineeringModelZipLocation,
                            engineeringModelSetup.EngineeringModelIid);
                        var engineeringModelZipEntry = zipFile.AddEntry(engineeringModelFilename, outputStream);
                        engineeringModelZipEntry.Comment = string.Format(
                            "The {0} EngineeringModel",
                            engineeringModelSetup.ShortName);
                        zipFile.Save(filePath);
                    }
                }

                var engineeringModel =
                    (EngineeringModel)engineeringModelDtos.Single(x => x.ClassKind == ClassKind.EngineeringModel);
                var iterationIids = engineeringModel.Iteration;

                foreach (var iterationIid in iterationIids)
                {
                    // Set the iteration context for transaction
                    this.TransactionManager.SetIterationContext(transaction, modelPartition, iterationIid);

                    // Get all iteration objects
                    var iterationDtos = this.IterationService.GetDeep(
                        transaction,
                        modelPartition,
                        new List<Guid> { iterationIid },
                        authorizedContext).ToList();

                    fileRevisions.AddRange(
                        iterationDtos.Where(x => x.ClassKind == ClassKind.FileRevision).OfType<FileRevision>()
                            .ToList());

                    // Serialize and save dtos to the archive
                    using (var iterationMemoryStream = new MemoryStream())
                    {
                        this.JsonSerializer.SerializeToStream(iterationDtos, iterationMemoryStream);

                        using (var outputStream = new MemoryStream(iterationMemoryStream.ToArray()))
                        {
                            var iterationFilename = $@"{EngineeringModelZipLocation}\{engineeringModelSetup.EngineeringModelIid}\{IterationZipLocation}\{iterationIid}.json";
                            zipFile.AddEntry(iterationFilename, outputStream);
                            zipFile.Save(filePath);
                        }
                    }
                }

                // Add files to the archive
                foreach (var fileRevision in fileRevisions)
                {
                    var fileRevisionPath = $@"{EngineeringModelZipLocation}\{engineeringModelSetup.EngineeringModelIid}\{FileRevisionZipLocation}";

                    string storageFilePath;
                    this.FileBinaryService.TryGetFileStoragePath(fileRevision.ContentHash, out storageFilePath);

                    zipFile.AddFile(storageFilePath, fileRevisionPath);
                    zipFile.Save(filePath);
                }
            }

            // Set the state of the credentials and the participant flag before retrieving EngineeringModel data
            credentials.IsParticipant = currentParticipantFlag;
            this.PermissionService.Credentials = credentials;
        }
    }
}
