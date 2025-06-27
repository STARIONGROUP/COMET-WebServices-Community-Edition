﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationProcessorTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;
    using CDP4Common.MetaInfo;
    using CDP4Common.Types;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Exceptions;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Protocol;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Alias = CDP4Common.DTO.Alias;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using EngineeringModel = CDP4Common.DTO.EngineeringModel;
    using Iteration = CDP4Common.DTO.Iteration;
    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterOverride = CDP4Common.DTO.ParameterOverride;
    using ParameterOverrideValueSet = CDP4Common.DTO.ParameterOverrideValueSet;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Test fixture for the <see cref="OperationProcessor"/> class
    /// </summary>
    [TestFixture]
    public class OperationProcessorTestFixture
    {
        private const string ClasskindKey = "ClassKind";

        private const string IidKey = "Iid";

        private const string TestName = "testName";

        private const string TestShortName = "testShortName";

        private const string SimpleQuantityKindTypeString = "SimpleQuantityKind";

        private const string QuantityKindTypeString = "QuantityKind";

        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };

        private readonly OperationSideEffectProcessor operationSideEffectProcessor = new(new List<IOperationSideEffect>());

        private readonly SimpleQuantityKindMetaInfo simpleQuantityKindMetaInfo = new();

        private readonly QuantityKindMetaInfo quantityKindMetaInfo = new();
        private readonly ThingMetaInfo thingMetaInfo = new();

        private readonly EngineeringModelMetaInfo engineeringModelMetaInfo = new();

        private readonly Dictionary<string, Stream> fileStore = new();

        private Mock<IMetaInfoProvider> mockedMetaInfoProvider;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<CometServer.Services.IServiceProvider> serviceProvider;
        private Mock<IResolveService> resolveService;
        private Mock<IPermissionService> permissionService;

        private OperationProcessor operationProcessor;

        [SetUp]
        public void TestSetup()
        {
            this.mockedMetaInfoProvider = new Mock<IMetaInfoProvider>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.transactionManager.Setup(x => x.GetRawSessionInstantAsync(It.IsAny<NpgsqlTransaction>())).Returns(Task.FromResult(DateTime.MaxValue as object));
            this.operationSideEffectProcessor.RequestUtils = this.requestUtils;
            this.operationSideEffectProcessor.MetaInfoProvider = this.mockedMetaInfoProvider.Object;

            this.operationProcessor = new OperationProcessor
            {
                OperationSideEffectProcessor = this.operationSideEffectProcessor,
                MetaInfoProvider = this.mockedMetaInfoProvider.Object
            };

            this.serviceProvider = new Mock<CometServer.Services.IServiceProvider>();
            this.resolveService = new Mock<IResolveService>();

            var copyservice = new CopySourceService
            {
                TransactionManager = this.transactionManager.Object,
                ServiceProvider = this.serviceProvider.Object
            };

            this.operationProcessor.CopySourceService = copyservice;
            this.operationProcessor.ServiceProvider = this.serviceProvider.Object;
            this.operationProcessor.ResolveService = this.resolveService.Object;

            this.permissionService = new Mock<IPermissionService>();
            this.permissionService.Setup(x => x.CanRead(It.IsAny<string>(), It.IsAny<ISecurityContext>(), It.IsAny<string>())).Returns(true);
            this.permissionService.Setup(x => x.CanReadAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<Thing>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        }

        [Test]
        public void VerifyIncompleteDeleteOperationValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);

            var deleteObjectWithoutIid = new ClasslessDTO() { { ClasskindKey, SimpleQuantityKindTypeString } };

            var deleteObjectWithoutClassKind = new ClasslessDTO() { { IidKey, Guid.NewGuid() } };

            var postOperation = new CdpPostOperation();
            postOperation.Delete.Add(deleteObjectWithoutIid);

            Assert.Throws(
                typeof(InvalidOperationException),
                async () => await this.operationProcessor.ValidateDeleteOperationsAsync(postOperation, null, ""));

            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithoutClassKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                async () => await this.operationProcessor.ValidateDeleteOperationsAsync(postOperation, null, ""));

            var completeDeleteObject = new ClasslessDTO() { { IidKey, Guid.NewGuid() }, { ClasskindKey, SimpleQuantityKindTypeString } };
            postOperation.Delete.Clear();
            postOperation.Delete.Add(completeDeleteObject);

            Assert.DoesNotThrow(async () =>await this.operationProcessor.ValidateDeleteOperationsAsync(postOperation, null, ""));
        }

        [Test]
        public void VerifyScalarPropertiesOnDeleteOperationValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);

            var deleteObjectWithScalarPropertySet = new ClasslessDTO()
            {
                { IidKey, Guid.NewGuid() },
                { ClasskindKey, SimpleQuantityKindTypeString },
                { "Name", TestName }
            };

            var postOperation = new CdpPostOperation();
            postOperation.Delete.Add(deleteObjectWithScalarPropertySet);

            Assert.Throws(
                typeof(InvalidOperationException),
                async () => await this.operationProcessor.ValidateDeleteOperationsAsync(postOperation, null, ""));

            var deleteObjectWithListProperty = new ClasslessDTO()
            {
                { IidKey, Guid.NewGuid() },
                { ClasskindKey, SimpleQuantityKindTypeString },
                { "PossibleScale", new[] { Guid.NewGuid() } }
            };

            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithListProperty);

            Assert.DoesNotThrow(async() => await this.operationProcessor.ValidateDeleteOperationsAsync(postOperation, null, ""));
        }

        [Test]
        public void VerifyCreateInvalidThingValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<string>())).Returns(this.thingMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == QuantityKindTypeString))).Returns(this.quantityKindMetaInfo);

            var newSimpleQuantityKind = new SimpleQuantityKind(Guid.NewGuid(), 0)
            {
                Alias = new List<Guid>(),
                Definition = new List<Guid>(),
                HyperLink = new List<Guid>(),
                PossibleScale = new List<Guid>(),
                ShortName = TestShortName,
                Symbol = "testSymbol"
            };

            var postOperation = new CdpPostOperation();
            postOperation.Create.Add(newSimpleQuantityKind);

            Assert.Throws(
                typeof(Cdp4ModelValidationException),
                () => this.operationProcessor.ValidateCreateOperations(postOperation, new Dictionary<string, Stream>()));
        }

        [Test]
        public void VerifyTopContainerCreationNotAllowedValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<EngineeringModel>())).Returns(this.engineeringModelMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<string>())).Returns(this.thingMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == "EngineeringModel"))).Returns(this.engineeringModelMetaInfo);

            var newEngineeringModel = new EngineeringModel(Guid.NewGuid(), 0)
            {
                Book = new List<OrderedItem>(),
                CommonFileStore = new List<Guid>(),
                Iteration = new List<Guid> { Guid.NewGuid() },
                LogEntry = new List<Guid>()
            };

            var postOperation = new CdpPostOperation();
            postOperation.Create.Add(newEngineeringModel);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => this.operationProcessor.ValidateCreateOperations(postOperation, this.fileStore));
        }

        [Test]
        public void VerifyOrderedItemListValidationForCorrectValues()
        {
            var updatedItem = new CDP4Common.DTO.PossibleFiniteStateList();

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 1,
                    V = Guid.NewGuid()
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 2,
                    V = Guid.NewGuid()
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 3,
                    V = Guid.NewGuid()
                });

            var metaInfo = new PossibleFiniteStateListMetaInfo();

            Assert.DoesNotThrow(
                () => OperationProcessor.OrderedItemListValidation(
                    null,
                    updatedItem,
                    new List<string> { "PossibleState" },
                    metaInfo
                )
            );
        }

        [Test]
        public void VerifyOrderedItemListValidationForKeys()
        {
            var updatedItem = new CDP4Common.DTO.PossibleFiniteStateList();

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 0,
                    V = Guid.NewGuid()
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 0,
                    V = Guid.NewGuid()
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 1,
                    V = Guid.NewGuid()
                });

            var metaInfo = new PossibleFiniteStateListMetaInfo();

            var exception = Assert.Throws<BadRequestException>(
                () => OperationProcessor.OrderedItemListValidation(
                    null,
                    updatedItem,
                    new List<string> { "PossibleState" },
                    metaInfo
                )
            );

            Assert.That(exception.Message, Contains.Substring("contains duplicate keys"));
        }

        [Test]
        public void VerifyOrderedItemListValidationForValues()
        {
            var updatedItem = new CDP4Common.DTO.PossibleFiniteStateList();

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 1,
                    V = Guid.Parse("2cfad1a6-87a4-412e-bcaa-655782cb60cf")
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 2,
                    V = Guid.Parse("2cfad1a6-87a4-412e-bcaa-655782cb60cf")
                });

            updatedItem.PossibleState.Add(
                new OrderedItem
                {
                    K = 3,
                    V = Guid.NewGuid()
                });

            var metaInfo = new PossibleFiniteStateListMetaInfo();

            var exception = Assert.Throws<BadRequestException>(
                () => OperationProcessor.OrderedItemListValidation(
                    null,
                    updatedItem,
                    new List<string> { "PossibleState" },
                    metaInfo
                )
            );

            Assert.That(exception.Message, Contains.Substring("contains duplicate values"));
        }

        [Test]
        public void VerifyCreateWithoutContainerUpdateValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<string>())).Returns(this.thingMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);

            var newSimpleQuantityKind = new SimpleQuantityKind(Guid.NewGuid(), 0)
            {
                Alias = new List<Guid>(),
                Definition = new List<Guid>(),
                HyperLink = new List<Guid>(),
                PossibleScale = new List<Guid>(),
                Name = TestName,
                ShortName = TestShortName,
                Symbol = "testSymbol"
            };

            var postOperation = new CdpPostOperation();
            postOperation.Create.Add(newSimpleQuantityKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => this.operationProcessor.ValidateCreateOperations(postOperation, this.fileStore));
        }

        [Test]
        public void VerifyCreateWithContainerUpdateInUpdatesValidation()
        {
            var modelReferenceDataLibraryMetaInfo = new ModelReferenceDataLibraryMetaInfo();

            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<ModelReferenceDataLibrary>())).Returns(modelReferenceDataLibraryMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<string>())).Returns(this.thingMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == "ModelReferenceDataLibrary"))).Returns(modelReferenceDataLibraryMetaInfo);

            var newSimpleQuantityKind = new SimpleQuantityKind(Guid.NewGuid(), 0)
            {
                Alias = new List<Guid>(),
                Definition = new List<Guid>(),
                HyperLink = new List<Guid>(),
                PossibleScale = new List<Guid>(),
                Name = TestName,
                ShortName = TestShortName,
                Symbol = "testSymbol"
            };

            // simpleQuantityKind container update
            var modelReferenceDataLibrary = new ClasslessDTO()
            {
                { IidKey, Guid.NewGuid() },
                { ClasskindKey, ClassKind.ModelReferenceDataLibrary },
                { "ParameterType", new[] { newSimpleQuantityKind.Iid } }
            };

            var postOperation = new CdpPostOperation();
            postOperation.Create.Add(newSimpleQuantityKind);
            postOperation.Update.Add(modelReferenceDataLibrary);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateCreateOperations(postOperation, this.fileStore));
        }

        [Test]
        public void VerifyCreateWithContainerUpdateInCreateValidation()
        {
            var aliasMetaInfo = new AliasMetaInfo();
            var modelReferenceDataLibraryMetaInfo = new ModelReferenceDataLibraryMetaInfo();

            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<Alias>())).Returns(aliasMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<string>())).Returns(this.thingMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<ModelReferenceDataLibrary>())).Returns(modelReferenceDataLibraryMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == "Alias"))).Returns(aliasMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == "ModelReferenceDataLibrary"))).Returns(modelReferenceDataLibraryMetaInfo);

            var newAlias = new Alias(Guid.NewGuid(), 0) { Content = "testContent", LanguageCode = "en-GB" };

            // alias container create
            var newSimpleQuantityKind = new SimpleQuantityKind(Guid.NewGuid(), 0)
            {
                Alias = new List<Guid> { newAlias.Iid },
                Definition = new List<Guid>(),
                HyperLink = new List<Guid>(),
                PossibleScale = new List<Guid>(),
                Name = TestName,
                ShortName = TestShortName,
                Symbol = "testSymbol"
            };

            // simplequantitykind container update
            var modelReferenceDataLibrary = new ClasslessDTO()
            {
                { IidKey, Guid.NewGuid() },
                { ClasskindKey, ClassKind.ModelReferenceDataLibrary },
                { "ParameterType", new[] { newSimpleQuantityKind.Iid } }
            };

            var postOperation = new CdpPostOperation();
            postOperation.Create.Add(newAlias);
            postOperation.Create.Add(newSimpleQuantityKind);
            postOperation.Update.Add(modelReferenceDataLibrary);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateCreateOperations(postOperation, this.fileStore));
        }

        [Test]
        public void VerifyIncompleteUpdateOperationValidation()
        {
            var updateObjectWithoutIid = new ClasslessDTO() { { ClasskindKey, SimpleQuantityKindTypeString } };

            var updateObjectWithoutClassKind = new ClasslessDTO() { { IidKey, Guid.NewGuid() } };

            var postOperation = new CdpPostOperation();
            postOperation.Update.Add(updateObjectWithoutIid);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => OperationProcessor.ValidateUpdateOperations(postOperation));

            postOperation.Update.Clear();
            postOperation.Update.Add(updateObjectWithoutClassKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => OperationProcessor.ValidateUpdateOperations(postOperation));

            var completeUpdateObject = new ClasslessDTO() { { IidKey, Guid.NewGuid() }, { ClasskindKey, Guid.NewGuid() } };
            postOperation.Update.Clear();
            postOperation.Update.Add(completeUpdateObject);

            Assert.DoesNotThrow(() => OperationProcessor.ValidateUpdateOperations(postOperation));
        }

        private List<Thing> copySourceDtos;

        private static readonly string[] DefaultValueArray = new[] { "-" };

        [Test]
        public async Task VerifyCopyElementDefWorks()
        {
            var modelSetupService = new Mock<IEngineeringModelSetupService>();
            var iterationService = new Mock<IIterationService>();
            var defaultArrayService = new Mock<IDefaultValueArrayFactory>();
            defaultArrayService.Setup(x => x.CreateDefaultValueArray(It.IsAny<Guid>())).Returns(new ValueArray<string>(new[] { "-" }));
            var modelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            modelSetupService.Setup(x => x.GetEngineeringModelSetupFromDataBaseCache(It.IsAny<NpgsqlTransaction>(), It.IsAny<Guid>())).Returns(Task.FromResult(modelSetup));

            this.copySourceDtos = new List<Thing>();

            var boolParamTypeId = Guid.NewGuid();
            var mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), 1);
            mrdl.ParameterType.Add(boolParamTypeId);

            var sourceIteration = new Iteration(Guid.NewGuid(), 1);
            var sourceElementDef1 = new ElementDefinition(Guid.NewGuid(), 1);
            var sourceElementDef2 = new ElementDefinition(Guid.NewGuid(), 1);
            var sourceUsage1 = new ElementUsage(Guid.NewGuid(), 1) { ElementDefinition = sourceElementDef2.Iid };
            sourceElementDef1.ContainedElement.Add(sourceUsage1.Iid);
            sourceIteration.Element.Add(sourceElementDef1.Iid);
            sourceIteration.Element.Add(sourceElementDef2.Iid);

            var parameter1 = new Parameter(Guid.NewGuid(), 1) { ParameterType = boolParamTypeId };

            var pvs1 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(new[] { "true" }),
                Computed = new ValueArray<string>(new[] { "-" }),
                Reference = new ValueArray<string>(new[] { "-" }),
                Published = new ValueArray<string>(new[] { "-" }),
                ValueSwitch = ParameterSwitchKind.MANUAL
            };

            var parameter2 = new Parameter(Guid.NewGuid(), 1) { ParameterType = boolParamTypeId };

            var pvs2 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(new[] { "true" }),
                Computed = new ValueArray<string>(new[] { "-" }),
                Reference = new ValueArray<string>(new[] { "-" }),
                Published = new ValueArray<string>(new[] { "-" }),
                ValueSwitch = ParameterSwitchKind.MANUAL
            };

            parameter1.ValueSet.Add(pvs1.Iid);
            sourceElementDef1.Parameter.Add(parameter1.Iid);
            parameter2.ValueSet.Add(pvs2.Iid);
            sourceElementDef2.Parameter.Add(parameter2.Iid);

            var override2 = new ParameterOverride(Guid.NewGuid(), 1)
            {
                Parameter = parameter2.Iid
            };

            var ovs = new ParameterOverrideValueSet(Guid.NewGuid(), 1) { ParameterValueSet = pvs2.Iid };
            override2.ValueSet.Add(ovs.Iid);
            sourceUsage1.ParameterOverride.Add(override2.Iid);

            this.copySourceDtos.Add(sourceIteration);
            this.copySourceDtos.Add(sourceElementDef1);
            this.copySourceDtos.Add(sourceElementDef2);
            this.copySourceDtos.Add(sourceUsage1);
            this.copySourceDtos.Add(parameter1);
            this.copySourceDtos.Add(pvs1);
            this.copySourceDtos.Add(parameter2);
            this.copySourceDtos.Add(pvs2);
            this.copySourceDtos.Add(override2);
            this.copySourceDtos.Add(ovs);

            var targetIteration = new Iteration(Guid.NewGuid(), 1);

            this.serviceProvider.Setup(x => x.MapToReadService(It.IsAny<string>())).Returns<string>(x => new TestSourceService(this.copySourceDtos, x));

            this.serviceProvider.Setup(x => x.MapToReadService(It.Is<string>(t => t == ClassKind.ModelReferenceDataLibrary.ToString())))
                .Returns<string>(x => new TestSourceService(new List<Thing> { mrdl }, x));

            this.serviceProvider.Setup(x => x.MapToReadService(It.Is<string>(t => t == ClassKind.Iteration.ToString())))
                .Returns<string>(x => new TestSourceService(new List<Thing> { sourceIteration, targetIteration }, x));

            var customOperationSideEffectProcessor = new Mock<IOperationSideEffectProcessor>();
            customOperationSideEffectProcessor.Setup(x => x.BeforeCreateAsync(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(true));

            var paramSubscriptionService = new ParameterSubscriptionService
            {
                ParameterSubscriptionDao = new Mock<IParameterSubscriptionDao>().Object,
                PermissionService = this.permissionService.Object,
            };

            var parameterContextProvider = new OldParameterContextProvider
            {
                ParameterValueSetService = new TestSourceService(this.copySourceDtos, ClassKind.ParameterValueSet.ToString())
            };

            var paramGroupService = new ParameterGroupService
            {
                PermissionService = this.permissionService.Object,

                TransactionManager = this.transactionManager.Object,
                ParameterGroupDao = new Mock<IParameterGroupDao>().Object
            };

            var valueSetService = new Mock<IParameterValueSetService>();

            valueSetService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(async (a, b, c, d) =>
                {
                    var list = new List<ParameterValueSet>();

                    foreach (var guid in c)
                    {
                        var vs = new ParameterValueSet(guid, 1)
                        {
                            Manual = new ValueArray<string>(DefaultValueArray),
                            Computed = new ValueArray<string>(DefaultValueArray),
                            Reference = new ValueArray<string>(DefaultValueArray),
                            Published = new ValueArray<string>(DefaultValueArray)
                        };

                        list.Add(vs);
                    }

                    return list;
                });

            var overrideValueSetService = new Mock<IParameterOverrideValueSetService>();

            overrideValueSetService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>(async (a, b, c, d) =>
                {
                    var list = new List<ParameterOverrideValueSet>();

                    foreach (var guid in c)
                    {
                        var vs = new ParameterOverrideValueSet(guid, 1)
                        {
                            Manual = new ValueArray<string>(DefaultValueArray),
                            Computed = new ValueArray<string>(DefaultValueArray),
                            Reference = new ValueArray<string>(DefaultValueArray),
                            Published = new ValueArray<string>(DefaultValueArray)
                        };

                        list.Add(vs);
                    }

                    return list;
                });

            var paramDao = new TestParameterDao();
            iterationService.Setup(x => x.GetAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(c => c.Contains(sourceIteration.Iid)), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(new List<Thing> { sourceIteration }.AsEnumerable()));
            iterationService.Setup(x => x.GetAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(c => c.Contains(targetIteration.Iid)), It.IsAny<ISecurityContext>())).Returns(Task.FromResult(new List<Thing> { targetIteration }.AsEnumerable()));

            var paramService = new ParameterService
            {
                PermissionService = this.permissionService.Object,
                ParameterDao = paramDao,
                OperationSideEffectProcessor = customOperationSideEffectProcessor.Object,
                TransactionManager = this.transactionManager.Object,
                OldParameterContextProvider = parameterContextProvider,
                ParameterSubscriptionService = paramSubscriptionService,
                ValueSetService = valueSetService.Object,
                IterationService = iterationService.Object,
                DefaultValueArrayFactory = defaultArrayService.Object
            };

            var paramOverrideDao = new TestParameterOverrideDao();

            var paramOverrideService = new ParameterOverrideService
            {
                PermissionService = this.permissionService.Object,

                ParameterOverrideDao = paramOverrideDao,
                TransactionManager = this.transactionManager.Object,
                OperationSideEffectProcessor = customOperationSideEffectProcessor.Object,
                ParameterSubscriptionService = paramSubscriptionService,
                ValueSetService = overrideValueSetService.Object
            };

            var usageDao = new Mock<IElementUsageDao>();

            var usageService = new ElementUsageService
            {
                PermissionService = this.permissionService.Object,

                ParameterOverrideService = paramOverrideService,
                ElementUsageDao = usageDao.Object,
                TransactionManager = this.transactionManager.Object,
                OperationSideEffectProcessor = customOperationSideEffectProcessor.Object
            };

            var edDao = new TestElementDefinitionDao();

            var edService = new ElementDefinitionService
            {
                PermissionService = this.permissionService.Object,

                ElementDefinitionDao = edDao,
                ContainedElementService = usageService,
                ParameterService = paramService,
                TransactionManager = this.transactionManager.Object,
                OperationSideEffectProcessor = customOperationSideEffectProcessor.Object,
                ParameterGroupService = paramGroupService
            };

            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ElementDefinition.ToString())).Returns(edService);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ElementUsage.ToString())).Returns(usageService);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.Parameter.ToString())).Returns(paramService);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ParameterOverride.ToString())).Returns(paramOverrideService);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ParameterSubscription.ToString())).Returns(paramSubscriptionService);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ParameterGroup.ToString())).Returns(paramGroupService);

            var postOperation = new CdpPostOperation();

            var copyinfo = new CopyInfo
            {
                ActiveOwner = Guid.NewGuid(),
                Options = new CopyInfoOptions
                {
                    CopyKind = CopyKind.Deep,
                    KeepOwner = true,
                    KeepValues = true
                },
                Source = new CopySource
                {
                    IterationId = sourceIteration.Iid,
                    Thing = new CopyReference
                    {
                        Iid = sourceElementDef1.Iid,
                        ClassKind = ClassKind.ElementDefinition
                    },
                    TopContainer = new CopyReference
                    {
                        Iid = Guid.NewGuid(),
                        ClassKind = ClassKind.EngineeringModel
                    }
                },
                Target = new CopyTarget
                {
                    IterationId = targetIteration.Iid,
                    Container = new CopyReference
                    {
                        Iid = targetIteration.Iid,
                        ClassKind = ClassKind.Iteration
                    },
                    TopContainer = new CopyReference
                    {
                        Iid = Guid.NewGuid(),
                        ClassKind = ClassKind.EngineeringModel
                    }
                }
            };

            postOperation.Copy.Add(copyinfo);

            this.serviceProvider.Setup(x => x.MapToReadService(ClassKind.EngineeringModelSetup.ToString())).Returns(modelSetupService.Object);

            // targetIteration
            await this.operationProcessor.ProcessAsync(postOperation, null, $"Iteration_{targetIteration.Iid.ToString().Replace("-", "_")}", null);

            Assert.That(edDao.WrittenThingCount, Is.EqualTo(2));
            Assert.That(paramDao.WrittenThingCount, Is.EqualTo(2));
            Assert.That(paramOverrideDao.WrittenThingCount, Is.EqualTo(1));
            usageDao.Verify(x => x.WriteAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ElementUsage>(), It.IsAny<Thing>()), Times.Once);
        }
    }

    public class TestSourceService : ServiceBase, IModelReferenceDataLibraryService, IParameterValueSetService
    {
        private IReadOnlyList<Thing> dtos;
        private string type;

        private readonly List<Thing> writtenThings = new();

        public TestSourceService(IReadOnlyList<Thing> dtos, string type)
        {
            this.dtos = dtos;
            this.type = type;
        }

        public Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thing>> GetShallowAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext)
        {
            var queriedIds = ids?.ToList();

            if (queriedIds == null)
            {
                return Task.FromResult(this.dtos.Where(x => x.ClassKind.ToString() == this.type));
            }

            return Task.FromResult(this.dtos.Where(x => x.ClassKind.ToString() == this.type && queriedIds.Contains(x.Iid)));
        }

        public async Task<IEnumerable<Thing>> GetDeepAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext)
        {
            if (this.type != ClassKind.ElementDefinition.ToString())
            {
                throw new NotImplementedException();
            }

            var eds = (await this.GetShallowAsync(transaction, partition, ids, securityContext)).OfType<ElementDefinition>().ToList();
            var dtos = new List<Thing>(eds);

            foreach (var thing in eds)
            {
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.ElementUsage.ToString() && thing.ContainedElement.Contains(x.Iid)));
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.Parameter.ToString() && thing.Parameter.Contains(x.Iid)));
            }

            foreach (var parameter in dtos.OfType<Parameter>().ToArray())
            {
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.ParameterValueSet.ToString() && parameter.ValueSet.Contains(x.Iid)));
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.ParameterSubscription.ToString() && parameter.ParameterSubscription.Contains(x.Iid)));
            }

            foreach (var usage in dtos.OfType<ElementUsage>().ToArray())
            {
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.ParameterOverride.ToString() && usage.ParameterOverride.Contains(x.Iid)));
            }

            foreach (var poverride in dtos.OfType<ParameterOverride>().ToArray())
            {
                dtos.AddRange(this.dtos.Where(x => x.ClassKind.ToString() == ClassKind.ParameterOverrideValueSet.ToString() && poverride.ValueSet.Contains(x.Iid)));
            }

            return dtos;
        }

        public Task<bool> UpdateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ReorderContainmentAsync(NpgsqlTransaction transaction, string partition, OrderedItem orderedItem)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CreateConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            this.writtenThings.Add(thing);
            return Task.FromResult(true);
        }

        public Task<bool> UpsertConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            this.writtenThings.Add(thing);
            return Task.FromResult(true);
        }

        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RawDeleteConceptAsync(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ReferenceDataLibrary>> QueryReferenceDataLibraryAsync(NpgsqlTransaction transaction, Iteration iteration)
        {
            return Task.FromResult(this.dtos.OfType<ReferenceDataLibrary>());
        }
    }

    public class TestParameterDao : IParameterDao
    {
        private List<Thing> writtenThings = new();

        public int WrittenThingCount => this.writtenThings.Count;

        public Task<IEnumerable<Parameter>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var queriedIds = ids?.ToList();

            if (queriedIds == null)
            {
                return Task.FromResult(this.writtenThings.OfType<Parameter>());
            }

            return Task.FromResult(this.writtenThings.OfType<Parameter>().Where(x => queriedIds.Contains(x.Iid)));
        }

        public Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, Parameter parameter, Thing container = null)
        {
            parameter.ValueSet.Clear();
            parameter.ValueSet.Add(Guid.NewGuid());
            this.writtenThings.Add(parameter);
            return Task.FromResult(true);
        }

        public Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, Parameter parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, Parameter parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public string BuildReadQuery(string partition, DateTime? instant)
        {
            throw new NotImplementedException();
        }

        public string BuildJoinForActorProperty(string partition)
        {
            throw new NotImplementedException();
        }

        public string GetValueTypeSet()
        {
            throw new NotImplementedException();
        }
    }

    public class TestParameterOverrideDao : IParameterOverrideDao
    {
        private List<Thing> writtenThings = new();

        public int WrittenThingCount => this.writtenThings.Count;

        public Task<IEnumerable<ParameterOverride>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var queriedIds = ids?.ToList();

            if (queriedIds == null)
            {
                return Task.FromResult(this.writtenThings.OfType<ParameterOverride>());
            }

            return Task.FromResult(this.writtenThings.OfType<ParameterOverride>().Where(x => queriedIds.Contains(x.Iid)));
        }

        public Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, ParameterOverride parameter, Thing container = null)
        {
            this.writtenThings.Add(parameter);
            parameter.ValueSet.Clear();
            parameter.ValueSet.Add(Guid.NewGuid());
            return Task.FromResult(true);
        }

        public Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, ParameterOverride parameterOverride, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, ParameterOverride parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public string BuildReadQuery(string partition, DateTime? instant)
        {
            throw new NotImplementedException();
        }

        public string BuildJoinForActorProperty(string partition)
        {
            throw new NotImplementedException();
        }

        public string GetValueTypeSet()
        {
            throw new NotImplementedException();
        }
    }

    public class TestElementDefinitionDao : IElementDefinitionDao
    {
        private List<Thing> writtenThings = new();

        public int WrittenThingCount => this.writtenThings.Count;

        public Task<IEnumerable<ElementDefinition>> ReadAsync(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false, DateTime? instant = null)
        {
            var queriedIds = ids?.ToList();

            if (queriedIds == null)
            {
                return Task.FromResult(this.writtenThings.OfType<ElementDefinition>());
            }

            return Task.FromResult(this.writtenThings.OfType<ElementDefinition>().Where(x => queriedIds.Contains(x.Iid)));
        }

        public Task<bool> WriteAsync(NpgsqlTransaction transaction, string partition, ElementDefinition ed, Thing container = null)
        {
            this.writtenThings.Add(ed);
            return Task.FromResult(true);
        }

        public Task<bool> UpsertAsync(NpgsqlTransaction transaction, string partition, ElementDefinition elementDefinition, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(NpgsqlTransaction transaction, string partition, ElementDefinition parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReorderCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddToCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RawDeleteAsync(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFromCollectionPropertyAsync(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public string BuildReadQuery(string partition, DateTime? instant)
        {
            throw new NotImplementedException();
        }

        public string BuildJoinForActorProperty(string partition)
        {
            throw new NotImplementedException();
        }

        public string GetValueTypeSet()
        {
            throw new NotImplementedException();
        }
    }
}
