// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
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

    /// <summary>
    /// Test fixture for the <see cref="PermissionService"/> class
    /// </summary>
    [TestFixture]
    public class PermissionServiceTestFixture
    {
        /// <summary>
        /// The EngineeringModel partition.
        /// </summary>
        private const string EngineeringModelPartition = "EngineeringModel";

        private PermissionService permissionService;
        private Mock<IAccessRightKindService> accessRightKindService;
        private Mock<IResolveService> resolveService;
        private Mock<ParticipantDao> participantDao;

        private AuthenticationPerson authenticationPerson;
        private Requirement requirement;
        private Definition definition;
        private DomainOfExpertise domain;
        private Participant participant;

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
                    operationThingContainerCache.Add(new ContainerInfo(nameof(Requirement), this.requirement.Iid), new DtoResolveHelper(this.requirement));
                });

            this.permissionService.ResolveService = this.resolveService.Object;

            this.accessRightKindService = new Mock<IAccessRightKindService>();

            this.permissionService.AccessRightKindService = this.accessRightKindService.Object;

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), "Definition"))
                .Returns(ParticipantAccessRightKind.SAME_AS_CONTAINER);

            this.domain = new DomainOfExpertise(Guid.NewGuid(), 0);

            this.participant = new Participant(Guid.NewGuid(), 0)
            {
                Domain = new List<Guid> { this.domain.Iid },
                Person = this.authenticationPerson.Iid
            };

            this.permissionService.Credentials.EngineeringModelSetup.Participant.Add(this.participant.Iid);

            this.participantDao = new Mock<ParticipantDao>();

            this.participantDao.Setup(
                    x =>
                        x.Read(null, It.IsAny<string>(), null, false))
                .Returns(new List<Participant>() { this.participant });

            this.permissionService.ParticipantDao = this.participantDao.Object;

            this.requirement = new Requirement(Guid.NewGuid(), 0);
            this.definition = new Definition(Guid.NewGuid(), 0);

            this.requirement.Definition.Add(this.requirement.Iid);
        }

        [Test]
        public void VerifySameAsContainerPermissionAutorization()
        {
            var partitionString = EngineeringModelPartition;

            var securityRequestContext = new RequestSecurityContext
            {
                ContainerReadAllowed = true, ContainerWriteAllowed = true
            };

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), "Requirement"))
                .Returns(ParticipantAccessRightKind.MODIFY);

            Assert.IsTrue(
                this.permissionService.CanWrite(
                    null,
                    this.definition,
                    "Definition",
                    partitionString,
                    ServiceBase.UpdateOperation,
                    securityRequestContext
                    )
                );

            this.accessRightKindService.Setup(
                    x =>
                        x.QueryParticipantAccessRightKind(It.IsAny<Credentials>(), "Requirement"))
                .Returns(ParticipantAccessRightKind.READ);

            Assert.IsFalse(
                this.permissionService.CanWrite(
                    null,
                    this.definition,
                    "Definition",
                    partitionString,
                    ServiceBase.UpdateOperation,
                    securityRequestContext
                )
            );
        }
    }
}
