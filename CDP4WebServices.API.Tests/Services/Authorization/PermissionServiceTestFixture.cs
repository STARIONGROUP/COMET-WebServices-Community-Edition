// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CDP4Authentication;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Definition = CDP4Common.DTO.Definition;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Test fixture for the <see cref="PermissionService"/> class
    /// </summary>
    [TestFixture]
    public class PermissionServiceTestFixture
    {
        /// <summary>
        /// The EngineeringModel partition.
        /// </summary>
        private const string SiteDirectoryPartition = "SiteDirectory";

        /// <summary>
        /// The EngineeringModel partition.
        /// </summary>
        private const string EngineeringModelPartition = "EngineeringModel";

        /// <summary>
        /// The Iteration partition.
        /// </summary>
        private const string IterationPartition = "Iteration";

        private PermissionService permissionService;
        private Mock<IAccessRightKindService> accessRightKindService;
        private Mock<IResolveService> resolveService;
        private Mock<ParticipantDao> participantDao;

        private AuthenticationPerson authenticationPerson;
        private static EngineeringModel engineeringModel = new(Guid.NewGuid(), 0);
        private static ParameterType parameterType = new TextParameterType(Guid.NewGuid(), 0);
        private static Iteration iteration = new(Guid.NewGuid(), 0);
        private static Requirement requirement = new(Guid.NewGuid(), 0);
        private static Definition definition = new(Guid.NewGuid(), 0);
        private static Definition definition2 = new(Guid.NewGuid(), 0);
        private static RequirementsSpecification requirementsSpecification = new(Guid.NewGuid(), 0);
        private static DomainOfExpertise domain = new(Guid.NewGuid(), 0);
        private static SiteDirectory siteDirectory = new(Guid.NewGuid(), 0);
        private Participant participant;

        private Thing addContainerThingToCache = null;

        [SetUp]
        public void TestSetup()
        {
            this.authenticationPerson = new AuthenticationPerson(Guid.NewGuid(), 0)
            {
                UserName = "TestRunner"
            };

            this.permissionService = new PermissionService();

            this.permissionService.Credentials = new Credentials
            {
                Person = this.authenticationPerson,
                EngineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0)
            };

            this.resolveService = new Mock<IResolveService>();

            this.resolveService.Setup(x => x.ResolveItems(null, It.IsAny<string>(), It.IsAny<Dictionary<DtoInfo, DtoResolveHelper>>()))
                .Callback<NpgsqlTransaction, string, Dictionary<DtoInfo, DtoResolveHelper>>
                ((npgsqlTransaction, partition, operationThingContainerCache) =>
                {
                    if (this.addContainerThingToCache != null)
                    {
                        operationThingContainerCache.Add(new ContainerInfo(this.addContainerThingToCache.ClassKind.ToString(), this.addContainerThingToCache.Iid), new DtoResolveHelper(this.addContainerThingToCache));
                    }
                });

            this.permissionService.ResolveService = this.resolveService.Object;

            this.accessRightKindService = new Mock<IAccessRightKindService>();

            this.permissionService.AccessRightKindService = this.accessRightKindService.Object;

            this.participant = new Participant(Guid.NewGuid(), 0)
            {
                Domain = new List<Guid> { domain.Iid },
                Person = this.authenticationPerson.Iid
            };

            this.permissionService.Credentials.EngineeringModelSetup.Participant.Add(this.participant.Iid);

            this.participantDao = new Mock<ParticipantDao>();

            this.participantDao.Setup(
                    x =>
                        x.Read(null, It.IsAny<string>(), null, false))
                .Returns(new List<Participant>() { this.participant });

            this.permissionService.ParticipantDao = this.participantDao.Object;

            engineeringModel.Iteration.Add(iteration.Iid);
            requirement.Definition.Add(definition.Iid);
            parameterType.Definition.Add(definition2.Iid);
            siteDirectory.Domain.Add(domain.Iid);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void VerifySameAsContainerPermissionAutorization(Thing containerThing, Thing thing, string partition)
        {
            //-------------------------------------------------------------
            // Setup
            //-------------------------------------------------------------
            this.addContainerThingToCache = containerThing;
            engineeringModel.Iteration.Add(iteration.Iid);

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryPersonAccessRightKind(It.IsAny<Credentials>(), thing.ClassKind.ToString()))
                .Returns(PersonAccessRightKind.SAME_AS_CONTAINER);

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), thing.ClassKind.ToString()))
                .Returns(ParticipantAccessRightKind.SAME_AS_CONTAINER);

            var securityRequestContext = new RequestSecurityContext
            {
                ContainerReadAllowed = true, ContainerWriteAllowed = true
            };

            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // container modify is allowed
            //-------------------------------------------------------------
            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), containerThing.ClassKind.ToString()))
                .Returns(ParticipantAccessRightKind.MODIFY);

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryPersonAccessRightKind(It.IsAny<Credentials>(), containerThing.ClassKind.ToString()))
                .Returns(PersonAccessRightKind.MODIFY);

            Assert.IsTrue(
                this.permissionService.CanWrite(
                    null,
                    thing,
                    thing.ClassKind.ToString(),
                    partition,
                    ServiceBase.UpdateOperation,
                    securityRequestContext
                )
            );

            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // container modify is NOT allowed
            //-------------------------------------------------------------
            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), containerThing.ClassKind.ToString()))
                .Returns(ParticipantAccessRightKind.READ);

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryPersonAccessRightKind(It.IsAny<Credentials>(), containerThing.ClassKind.ToString()))
                .Returns(PersonAccessRightKind.READ);

            Assert.IsFalse(
                this.permissionService.CanWrite(
                    null,
                    thing,
                    thing.ClassKind.ToString(),
                    partition,
                    ServiceBase.UpdateOperation,
                    securityRequestContext
                )
            );

            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // Create operation does not check container, but returns
            // RequestSecurityContext setting
            //-------------------------------------------------------------
            Assert.IsTrue(
                this.permissionService.CanWrite(
                    null,
                    thing,
                    thing.ClassKind.ToString(),
                    partition,
                    ServiceBase.CreateOperation,
                    securityRequestContext
                )
            );

            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // container thing not found returns RequestSecurityContext setting
            //-------------------------------------------------------------
            this.addContainerThingToCache = null;

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), containerThing.ClassKind.ToString()))
                .Returns(ParticipantAccessRightKind.MODIFY);

            Assert.IsFalse(
                this.permissionService.CanWrite(
                    null,
                    thing,
                    thing.ClassKind.ToString(),
                    partition,
                    ServiceBase.UpdateOperation,
                    securityRequestContext
                )
            );

            //-------------------------------------------------------------
        }

        /// <summary>
        /// Different Cases we want to check access rights for
        /// </summary>
        /// <returns>an <see cref="IEnumerable"/> of type <see cref="object[]"/>
        /// containing the <see cref="PermissionServiceTestFixture.VerifySameAsContainerPermissionAutorization"/> method's parameters.</returns>
        public static IEnumerable TestCases()
        {
            yield return new object[] { requirement, definition, IterationPartition };
            yield return new object[] { engineeringModel, iteration, EngineeringModelPartition };
            yield return new object[] { parameterType, definition2, SiteDirectoryPartition };
            yield return new object[] { iteration, requirementsSpecification, IterationPartition };
            yield return new object[] { siteDirectory, domain, SiteDirectoryPartition };
        }
    }
}
