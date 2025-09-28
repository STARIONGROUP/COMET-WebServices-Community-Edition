// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExchangeFileWriterTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CometServer.Tests.Services.JsonExchangeFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Authentication;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Authorization;
    using CometServer.Services;
    using CometServer.Services.Protocol;

    using ICSharpCode.SharpZipLib.Zip;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class JsonExchangeFileWriterTestFixture
    {
        private readonly Mock<ICredentialsService> credentialsService = new();
        private readonly Mock<IObfuscationService> obfuscationService = new();
        private readonly Mock<ISiteDirectoryService> siteDirectoryService = new();
        private readonly Mock<IEngineeringModelService> engineeringModelService = new();
        private readonly Mock<IIterationService> iterationService = new();
        private readonly Mock<ISiteReferenceDataLibraryService> siteReferenceDataLibraryService = new();
        private readonly Mock<IModelReferenceDataLibraryService> modelReferenceDataLibraryService = new();
        private readonly Mock<IZipArchiveWriter> zipArchiveWriter = new();

        private Credentials credentials;
        private TestRequestUtils requestUtils;
        private JsonExchangeFileWriter writer;

        [SetUp]
        public void SetUp()
        {
            var authenticationPerson = new AuthenticationPerson(Guid.NewGuid(), 0)
            {
                UserName = "export.user"
            };

            this.credentials = new Credentials
            {
                Person = authenticationPerson
            };

            this.requestUtils = new TestRequestUtils
            {
                QueryParameters = new QueryParameters
                {
                    IncludeFileData = true
                }
            };

            this.credentialsService.Reset();
            this.obfuscationService.Reset();
            this.siteDirectoryService.Reset();
            this.engineeringModelService.Reset();
            this.iterationService.Reset();
            this.siteReferenceDataLibraryService.Reset();
            this.modelReferenceDataLibraryService.Reset();
            this.zipArchiveWriter.Reset();

            this.credentialsService.Setup(x => x.Credentials).Returns(this.credentials);
            this.credentialsService.Setup(x => x.ResolveParticipantCredentialsAsync(It.IsAny<Npgsql.NpgsqlTransaction>()))
                .Returns(Task.CompletedTask);

            this.writer = new JsonExchangeFileWriter
            {
                CredentialsService = this.credentialsService.Object,
                ObfuscationService = this.obfuscationService.Object,
                RequestUtils = this.requestUtils,
                SiteDirectoryService = this.siteDirectoryService.Object,
                EngineeringModelService = this.engineeringModelService.Object,
                IterationService = this.iterationService.Object,
                SiteReferenceDataLibraryService = this.siteReferenceDataLibraryService.Object,
                ModelReferenceDataLibraryService = this.modelReferenceDataLibraryService.Object,
                ZipArchiveWriter = this.zipArchiveWriter.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.credentialsService.Reset();
            this.obfuscationService.Reset();
            this.siteDirectoryService.Reset();
            this.engineeringModelService.Reset();
            this.iterationService.Reset();
            this.siteReferenceDataLibraryService.Reset();
            this.modelReferenceDataLibraryService.Reset();
            this.zipArchiveWriter.Reset();
        }

        [Test]
        public void VerifyCreateAsyncThrowsWhenInitiatingPersonCannotBeFound()
        {
            var exportDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0)
            {
                RequiredRdl = { Guid.NewGuid() }
            };

            var siteDirectory = new SiteDirectory
            {
                Iid = Guid.NewGuid(),
                Model = { engineeringModelSetup.Iid }
            };

            var modelReferenceDataLibrary = new ModelReferenceDataLibrary(Guid.NewGuid(), 0);

            this.siteDirectoryService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        "SiteDirectory",
                        null,
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    siteDirectory,
                    engineeringModelSetup,
                    modelReferenceDataLibrary
                });

            var transaction = default(Npgsql.NpgsqlTransaction);

            var exception = Assert.ThrowsAsync<ThingNotFoundException>(
                () => this.writer.CreateAsync(transaction, exportDirectory, new[] { engineeringModelSetup }, new Version(1, 0)));

            Assert.That(exception?.Message, Does.Contain(this.credentials.Person.UserName));

            this.zipArchiveWriter.Verify(
                x => x.WriteHeaderToZipFile(It.IsAny<ExchangeFileHeader>(), It.IsAny<ZipOutputStream>()),
                Times.Never);

            if (Directory.Exists(exportDirectory))
            {
                Directory.Delete(exportDirectory, true);
            }
        }

        [Test]
        public async Task VerifyCreateAsyncWritesCompleteArchiveAsync()
        {
            var exportDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(exportDirectory);

            var emailIdentifier = Guid.NewGuid();
            var organizationIdentifier = Guid.NewGuid();
            var personRoleIdentifier = Guid.NewGuid();
            var participantRoleIdentifier = Guid.NewGuid();
            var participantIdentifier = Guid.NewGuid();
            var domainIdentifier = Guid.NewGuid();
            var iterationSetupIdentifier = Guid.NewGuid();
            var iterationIdentifier = Guid.NewGuid();
            var engineeringModelIdentifier = Guid.NewGuid();
            var siteReferenceDataLibraryIdentifier = Guid.NewGuid();

            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0)
            {
                EngineeringModelIid = engineeringModelIdentifier,
                RequiredRdl = { Guid.NewGuid() },
                IterationSetup = { iterationSetupIdentifier },
                ActiveDomain = { domainIdentifier }
            };

            var emailAddress = new EmailAddress(emailIdentifier, 0)
            {
                Value = "export.user@example.com"
            };

            var organization = new Organization(organizationIdentifier, 0)
            {
                Name = "Export Org"
            };

            var personRole = new PersonRole(personRoleIdentifier, 0);

            var participantRole = new ParticipantRole(participantRoleIdentifier, 0);

            var participant = new Participant(participantIdentifier, 0)
            {
                Person = this.credentials.Person.Iid,
                Role = participantRoleIdentifier,
                EngineeringModelSetup = engineeringModelSetup.Iid
            };

            var domain = new DomainOfExpertise(domainIdentifier, 0);

            var person = new Person(this.credentials.Person.Iid, 0)
            {
                ShortName = this.credentials.Person.UserName,
                DefaultEmailAddress = emailIdentifier,
                Organization = organizationIdentifier,
                Role = personRoleIdentifier,
                DefaultDomain = domainIdentifier,
                OrganizationalUnit = "Orbital"
            };

            var siteDirectory = new SiteDirectory
            {
                Iid = Guid.NewGuid(),
                Person = { person.Iid },
                Organization = { organizationIdentifier },
                PersonRole = { personRoleIdentifier },
                ParticipantRole = { participantRoleIdentifier },
                Participant = { participantIdentifier },
                Domain = { domainIdentifier },
                Model = { engineeringModelSetup.Iid },
                SiteReferenceDataLibrary = { siteReferenceDataLibraryIdentifier }
            };

            var iterationSetup = new IterationSetup(iterationSetupIdentifier, 0)
            {
                IterationIid = iterationIdentifier
            };

            var engineeringModel = new EngineeringModel(engineeringModelIdentifier, 0);

            var engineeringModelFileRevision = new FileRevision(Guid.NewGuid(), 0);
            var iterationFileRevision = new FileRevision(Guid.NewGuid(), 0);

            var iteration = new Iteration(iterationIdentifier, 0)
            {
                IterationSetup = iterationSetupIdentifier
            };

            var modelReferenceDataLibrary = new ModelReferenceDataLibrary(engineeringModelSetup.RequiredRdl.Single(), 0)
            {
                RequiredRdl = siteReferenceDataLibraryIdentifier
            };

            var siteReferenceDataLibrary = new SiteReferenceDataLibrary(siteReferenceDataLibraryIdentifier, 0);

            var modelRdlThing = new DomainOfExpertise(Guid.NewGuid(), 0);
            var siteRdlThing = new Organization(Guid.NewGuid(), 0);

            this.siteDirectoryService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        "SiteDirectory",
                        null,
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    siteDirectory,
                    engineeringModelSetup,
                    iterationSetup,
                    person,
                    participant,
                    participantRole,
                    personRole,
                    organization,
                    emailAddress,
                    domain,
                    siteReferenceDataLibrary,
                    modelReferenceDataLibrary
                });

            this.modelReferenceDataLibraryService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        "SiteDirectory",
                        It.Is<IEnumerable<Guid>>(ids => ids.Single() == modelReferenceDataLibrary.Iid),
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    modelReferenceDataLibrary,
                    modelRdlThing
                });

            this.siteReferenceDataLibraryService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        "SiteDirectory",
                        It.Is<IEnumerable<Guid>>(ids => ids.Single() == siteReferenceDataLibrary.Iid),
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    siteReferenceDataLibrary,
                    siteRdlThing
                });

            this.engineeringModelService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        It.Is<string>(partition => partition.StartsWith("EngineeringModel_")),
                        It.Is<IEnumerable<Guid>>(ids => ids.Single() == engineeringModelIdentifier),
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    engineeringModel,
                    engineeringModelFileRevision
                });

            this.iterationService.Setup(
                    x => x.GetDeepAsync(
                        It.IsAny<Npgsql.NpgsqlTransaction>(),
                        It.Is<string>(partition => partition.StartsWith("EngineeringModel_")),
                        It.Is<IEnumerable<Guid>>(ids => ids.Single() == iterationIdentifier),
                        It.IsAny<CometServer.Services.Authorization.ISecurityContext>()))
                .ReturnsAsync(new List<Thing>
                {
                    iteration,
                    iterationFileRevision
                });

            ExchangeFileHeader capturedHeader = null;
            IEnumerable<FileRevision> capturedFileRevisions = null;
            IEnumerable<Thing> capturedModelRdlContents = null;
            IEnumerable<Thing> capturedSiteRdlContents = null;

            this.zipArchiveWriter
                .Setup(x => x.WriteHeaderToZipFile(It.IsAny<ExchangeFileHeader>(), It.IsAny<ZipOutputStream>()))
                .Callback<ExchangeFileHeader, ZipOutputStream>((header, _) => capturedHeader = header);

            this.zipArchiveWriter
                .Setup(x => x.WriteFileRevisionsToZipFile(
                    engineeringModelSetup,
                    It.IsAny<IEnumerable<FileRevision>>(),
                    It.IsAny<ZipOutputStream>()))
                .Callback<EngineeringModelSetup, IEnumerable<FileRevision>, ZipOutputStream>((_, revisions, _) => capturedFileRevisions = revisions);

            this.zipArchiveWriter
                .Setup(x => x.WriteModelReferenceDataLibraryToZipFile(
                    modelReferenceDataLibrary,
                    It.IsAny<IEnumerable<Thing>>(),
                    It.IsAny<ZipOutputStream>()))
                .Callback<ModelReferenceDataLibrary, IEnumerable<Thing>, ZipOutputStream>((_, contents, _) => capturedModelRdlContents = contents);

            this.zipArchiveWriter
                .Setup(x => x.WriteSiteReferenceDataLibraryToZipFile(
                    siteReferenceDataLibrary,
                    It.IsAny<IEnumerable<Thing>>(),
                    It.IsAny<ZipOutputStream>()))
                .Callback<SiteReferenceDataLibrary, IEnumerable<Thing>, ZipOutputStream>((_, contents, _) => capturedSiteRdlContents = contents);

            var resultPath = await this.writer.CreateAsync(null, exportDirectory, new[] { engineeringModelSetup }, new Version(1, 0));

            Assert.That(File.Exists(resultPath), Is.True);
            Assert.That(capturedHeader, Is.Not.Null);
            Assert.That(capturedHeader.CreatorPerson.Email, Is.EqualTo(emailAddress.Value));
            Assert.That(capturedHeader.CreatorOrganization, Is.Not.Null);
            Assert.That(capturedHeader.CreatorOrganization.Name, Is.EqualTo(organization.Name));
            Assert.That(capturedHeader.CreatorOrganization.Unit, Is.EqualTo(person.OrganizationalUnit));
            Assert.That(capturedHeader.Remark, Does.Contain("COMET"));

            Assert.That(capturedModelRdlContents, Is.Not.Null);
            Assert.That(capturedModelRdlContents.Single(), Is.EqualTo(modelRdlThing));

            Assert.That(capturedSiteRdlContents, Is.Not.Null);
            Assert.That(capturedSiteRdlContents.Single(), Is.EqualTo(siteRdlThing));

            Assert.That(capturedFileRevisions, Is.Not.Null);
            Assert.That(capturedFileRevisions, Does.Contain(engineeringModelFileRevision));
            Assert.That(capturedFileRevisions, Does.Contain(iterationFileRevision));

            Assert.That(this.requestUtils.OverrideAssignments.Count, Is.EqualTo(4));
            Assert.That(this.requestUtils.OverrideAssignments.Count(x => x != null), Is.EqualTo(2));
            Assert.That(this.requestUtils.OverrideAssignments.OfType<QueryParameters>().All(x => x.IncludeReferenceData), Is.True);
            Assert.That(this.requestUtils.OverrideQueryParameters, Is.Null);

            this.credentialsService.Verify(x => x.ResolveParticipantCredentialsAsync(It.IsAny<Npgsql.NpgsqlTransaction>()), Times.Once);
            this.obfuscationService.Verify(
                x => x.ObfuscateResponse(It.IsAny<List<Thing>>(), It.IsAny<Credentials>()),
                Times.Never);

            File.Delete(resultPath);
            Directory.Delete(exportDirectory, true);
        }

        private class TestRequestUtils : IRequestUtils
        {
            private IQueryParameters overrideQueryParameters;

            public List<Thing> Cache { get; set; } = new ();

            public IQueryParameters QueryParameters { get; set; }

            public List<IQueryParameters> OverrideAssignments { get; } = new ();

            public IQueryParameters OverrideQueryParameters
            {
                get => this.overrideQueryParameters;
                set
                {
                    this.overrideQueryParameters = value;
                    this.OverrideAssignments.Add(value);
                }
            }

            public string GetEngineeringModelPartitionString(Guid engineeringModelIid)
            {
                return $"EngineeringModel_{engineeringModelIid.ToString().Replace("-", "_")}";
            }

            public string GetIterationPartitionString(Guid engineeringModelIid)
            {
                return $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";
            }
        }
    }
}
