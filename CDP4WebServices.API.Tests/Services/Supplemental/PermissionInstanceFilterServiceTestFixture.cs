// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionInstanceFilterServiceTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Supplemental;

    using Moq;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Suite of tests for the <see cref="PermissionInstanceFilterService"/>
    /// </summary>
    [TestFixture]
    public class PermissionInstanceFilterServiceTestFixture
    {
        [Test]
        public void VerifyThatRoleAndPermissionAreFilteredCorrectly()
        {
            var requestUtil = new Mock<IRequestUtils>();
            var metaInfoProvider = new Mock<IMetaInfoProvider>();
            requestUtil.Setup(x => x.GetRequestDataModelVersion).Returns(new Version(1, 0));
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ActionItem.ToString()))
                .Returns(new ActionItemMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.SiteDirectory.ToString()))
                .Returns(new SiteDirectoryMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ActualFiniteState.ToString()))
                .Returns(new ActualFiniteStateMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.DiagramCanvas.ToString()))
                .Returns(new DiagramCanvasMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.EngineeringModel.ToString()))
                .Returns(new EngineeringModelMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ElementDefinition.ToString()))
                .Returns(new ElementDefinitionMetaInfo());
            requestUtil.Setup(x => x.MetaInfoProvider).Returns(metaInfoProvider.Object);
            var personRole = new PersonRole(Guid.NewGuid(), 0);
            var participantRole = new ParticipantRole(Guid.NewGuid(), 0);

            var personPermission1 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission1.ObjectClass = ClassKind.ActionItem;
            var personPermission2 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission2.ObjectClass = ClassKind.SiteDirectory;
            var personPermission3 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission3.ObjectClass = ClassKind.ActualFiniteState;

            var participantPermission1 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission1.ObjectClass = ClassKind.DiagramCanvas;
            var participantPermission2 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission2.ObjectClass = ClassKind.EngineeringModel;
            var participantPermission3 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission3.ObjectClass = ClassKind.ElementDefinition;

            personRole.PersonPermission.Add(personPermission3.Iid);
            personRole.PersonPermission.Add(personPermission2.Iid);
            personRole.PersonPermission.Add(personPermission1.Iid);

            participantRole.ParticipantPermission.Add(participantPermission1.Iid);
            participantRole.ParticipantPermission.Add(participantPermission2.Iid);
            participantRole.ParticipantPermission.Add(participantPermission3.Iid);

            var input = new List<Thing>
                            {
                                personRole,
                                participantRole,
                                personPermission1,
                                personPermission2,
                                personPermission3,
                                participantPermission1,
                                participantPermission2,
                                participantPermission3
                            };

            var result = new PermissionInstanceFilterService().FilterOutPermissions(
                input,
                requestUtil.Object,
                requestUtil.Object.GetRequestDataModelVersion).ToArray();
            Assert.IsFalse(result.Contains(personPermission1));
            Assert.IsFalse(result.Contains(participantPermission1));
            Assert.AreEqual(personRole.PersonPermission.Count, 2);
            Assert.AreEqual(participantRole.ParticipantPermission.Count, 2);
        }
    }
}
