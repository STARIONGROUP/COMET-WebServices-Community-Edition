// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationProcessorTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;
    using CDP4Common.MetaInfo;
    using CDP4Common.Types;

    using CDP4Orm.Dao;

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

        private readonly IOperationSideEffectProcessor operationSideEffectProcessor = new OperationSideEffectProcessor(new List<IOperationSideEffect>());

        private readonly SimpleQuantityKindMetaInfo simpleQuantityKindMetaInfo = new SimpleQuantityKindMetaInfo();

        private readonly QuantityKindMetaInfo quantityKindMetaInfo = new QuantityKindMetaInfo();
        private readonly ThingMetaInfo thingMetaInfo = new ThingMetaInfo();

        private readonly EngineeringModelMetaInfo engineeringModelMetaInfo = new EngineeringModelMetaInfo();

        private readonly Dictionary<string, Stream> fileStore = new Dictionary<string, Stream>();

        private Mock<IMetaInfoProvider> mockedMetaInfoProvider;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<CometServer.Services.IServiceProvider> serviceProvider;
        private Mock<IResolveService> resolveService;
        private Mock<IPermissionService> permissionService;

        private OperationProcessor operationProcessor;

        private CdpPostOperation operation;

        [SetUp]
        public void TestSetup()
        {
            this.mockedMetaInfoProvider = new Mock<IMetaInfoProvider>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.requestUtils.MetaInfoProvider = this.mockedMetaInfoProvider.Object;
            this.operationProcessor = new OperationProcessor();
            this.operationProcessor.RequestUtils = this.requestUtils;
            this.operationSideEffectProcessor.RequestUtils = this.requestUtils;
            this.operationProcessor.OperationSideEffectProcessor = this.operationSideEffectProcessor;
            
            this.serviceProvider = new Mock<CometServer.Services.IServiceProvider>();
            this.resolveService = new Mock<IResolveService>();

            var copyservice = new CopySourceService();
            copyservice.TransactionManager = this.transactionManager.Object;
            copyservice.ServiceProvider = this.serviceProvider.Object;

            this.operationProcessor.CopySourceService = copyservice;
            this.operationProcessor.ServiceProvider = this.serviceProvider.Object;
            this.operationProcessor.ResolveService = this.resolveService.Object;

            this.permissionService = new Mock<IPermissionService>();
            this.permissionService.Setup(x => x.CanRead(It.IsAny<string>(), It.IsAny<ISecurityContext>(), It.IsAny<string>())).Returns(true);
            this.permissionService.Setup(x => x.CanRead(It.IsAny<NpgsqlTransaction>(), It.IsAny<Thing>(), It.IsAny<string>())).Returns(true);
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
                () => this.operationProcessor.ValidateDeleteOperations(postOperation, null, ""));

            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithoutClassKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => this.operationProcessor.ValidateDeleteOperations(postOperation, null, ""));

            var completeDeleteObject = new ClasslessDTO() { { IidKey, Guid.NewGuid() }, { ClasskindKey, SimpleQuantityKindTypeString } };
            postOperation.Delete.Clear();
            postOperation.Delete.Add(completeDeleteObject);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateDeleteOperations(postOperation, null, ""));
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
                () => this.operationProcessor.ValidateDeleteOperations(postOperation, null, ""));

            var deleteObjectWithListProperty = new ClasslessDTO()
                                                    {
                                                        { IidKey, Guid.NewGuid() },
                                                        { ClasskindKey, SimpleQuantityKindTypeString },
                                                        { "PossibleScale", new[] { Guid.NewGuid() } }
                                                    };
            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithListProperty);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateDeleteOperations(postOperation, null, ""));
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

            var newAlias = new Alias(Guid.NewGuid(), 0) { Content = "testContent", LanguageCode = "en-GB"};

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
                () => this.operationProcessor.ValidateUpdateOperations(postOperation));

            postOperation.Update.Clear();
            postOperation.Update.Add(updateObjectWithoutClassKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => this.operationProcessor.ValidateUpdateOperations(postOperation));

            var completeUpdateObject = new ClasslessDTO() { { IidKey, Guid.NewGuid() }, { ClasskindKey, Guid.NewGuid() } };
            postOperation.Update.Clear();
            postOperation.Update.Add(completeUpdateObject);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateUpdateOperations(postOperation));
        }

        private List<Thing> copySourceDtos;

        [Test]
        public void VerifyCopyElementDefWorks()
        {
            var modelSetupService = new Mock<IEngineeringModelSetupService>();
            var iterationService = new Mock<IIterationService>();
            var defaultArrayService = new Mock<IDefaultValueArrayFactory>();
            defaultArrayService.Setup(x => x.CreateDefaultValueArray(It.IsAny<Guid>())).Returns(new ValueArray<string>(new [] {"-"}));
            var modelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            modelSetupService.Setup(x => x.GetEngineeringModelSetup(It.IsAny<NpgsqlTransaction>(), It.IsAny<Guid>())).Returns(modelSetup);

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
                Manual = new ValueArray<string>(new[] {"true"}),
                Computed = new ValueArray<string>(new[] {"-"}),
                Reference = new ValueArray<string>(new[] {"-"}),
                Published = new ValueArray<string>(new[] {"-"}),
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

            var override2 = new ParameterOverride(Guid.NewGuid(), 1);
            override2.Parameter = parameter2.Iid;
            var ovs = new ParameterOverrideValueSet(Guid.NewGuid(), 1) {ParameterValueSet = pvs2.Iid};
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
            customOperationSideEffectProcessor.Setup(x => x.BeforeCreate(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>())).
                Returns(true);

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
            valueSetService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>((a, b, c, d) =>
                {
                    var list = new List<ParameterValueSet>();
                    foreach (var guid in c)
                    {
                        var vs = new ParameterValueSet(guid, 1)
                        {
                            Manual = new ValueArray<string>(new [] { "-" }),
                            Computed = new ValueArray<string>(new [] { "-" }),
                            Reference = new ValueArray<string>(new [] { "-" }),
                            Published = new ValueArray<string>(new [] { "-" })
                        };

                        list.Add(vs);
                    }
                    
                    return list;
                });  

            var overrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            overrideValueSetService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .Returns<NpgsqlTransaction, string, IEnumerable<Guid>, ISecurityContext>((a, b, c, d) =>
                {
                    var list = new List<ParameterOverrideValueSet>();
                    foreach (var guid in c)
                    {
                        var vs = new ParameterOverrideValueSet(guid, 1)
                        {
                            Manual = new ValueArray<string>(new[] { "-" }),
                            Computed = new ValueArray<string>(new[] { "-" }),
                            Reference = new ValueArray<string>(new[] { "-" }),
                            Published = new ValueArray<string>(new[] { "-" })
                        };

                        list.Add(vs);
                    }

                    return list;
                });

            var paramDao = new TestParameterDao();
            iterationService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(c => c.Contains(sourceIteration.Iid)), It.IsAny<ISecurityContext>())).Returns(new[] { sourceIteration });
            iterationService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(c => c.Contains(targetIteration.Iid)), It.IsAny<ISecurityContext>())).Returns(new[] { targetIteration });

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
            this.operationProcessor.Process(postOperation, null, $"Iteration_{targetIteration.Iid.ToString().Replace("-", "_")}", null);

            Assert.AreEqual(2, edDao.WrittenThingCount);
            Assert.AreEqual(2, paramDao.WrittenThingCount);
            Assert.AreEqual(1, paramOverrideDao.WrittenThingCount);
            usageDao.Verify(x => x.Write(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ElementUsage>(), It.IsAny<Thing>()), Times.Once);
        }
    }

    public class TestSourceService : ServiceBase, IPersistService, IModelReferenceDataLibraryService, IParameterValueSetService
    {
        private IReadOnlyList<Thing> dtos;
        private string type;

        private readonly List<Thing> writtenThings = new List<Thing>();

        public TestSourceService(IReadOnlyList<Thing> dtos, string type)
        {
            this.dtos = dtos;
            this.type = type;
        }

        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext)
        {
            var queriedIds = ids?.ToList();
            if (queriedIds == null)
            {
                return this.dtos.Where(x => x.ClassKind.ToString() == this.type);
            }

            return this.dtos.Where(x => x.ClassKind.ToString() == this.type && queriedIds.Contains(x.Iid));
        }

        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext)
        {
            if (this.type != ClassKind.ElementDefinition.ToString())
            {
                throw new NotImplementedException();
            }

            var eds = this.GetShallow(transaction, partition, ids, securityContext).OfType<ElementDefinition>().ToList();
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

        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            return true;
        }

        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            return true;
        }

        public bool ReorderContainment(NpgsqlTransaction transaction, string partition, OrderedItem orderedItem)
        {
            return true;
        }

        public bool CreateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            this.writtenThings.Add(thing);
            return true;
        }

        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return true;
        }

        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            return true;
        }

        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return true;
        }

        public IEnumerable<ReferenceDataLibrary> QueryReferenceDataLibrary(NpgsqlTransaction transaction, Iteration iteration)
        {
            return this.dtos.OfType<ReferenceDataLibrary>();
        }
    }

    public class TestParameterDao : IParameterDao
    {
        private List<Thing> writtenThings = new List<Thing>();
        public int WrittenThingCount => this.writtenThings.Count;

        public IEnumerable<Parameter> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            var queriedIds = ids?.ToList();
            if (queriedIds == null)
            {
                return this.writtenThings.OfType<Parameter>();
            }

            return this.writtenThings.OfType<Parameter>().Where(x => queriedIds.Contains(x.Iid));
        }

        public bool Write(NpgsqlTransaction transaction, string partition, Parameter parameter, Thing container = null)
        {
            parameter.ValueSet.Clear();
            parameter.ValueSet.Add(Guid.NewGuid());
            this.writtenThings.Add(parameter);
            return true;
        }

        public bool Update(NpgsqlTransaction transaction, string partition, Parameter parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class TestParameterOverrideDao : IParameterOverrideDao
    {
        private List<Thing> writtenThings = new List<Thing>();
        public int WrittenThingCount => this.writtenThings.Count;

        public IEnumerable<ParameterOverride> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            var queriedIds = ids?.ToList();
            if (queriedIds == null)
            {
                return this.writtenThings.OfType<ParameterOverride>();
            }

            return this.writtenThings.OfType<ParameterOverride>().Where(x => queriedIds.Contains(x.Iid));
        }

        public bool Write(NpgsqlTransaction transaction, string partition, ParameterOverride parameter, Thing container = null)
        {
            this.writtenThings.Add(parameter);
            parameter.ValueSet.Clear();
            parameter.ValueSet.Add(Guid.NewGuid());
            return true;
        }

        public bool Update(NpgsqlTransaction transaction, string partition, ParameterOverride parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class TestElementDefinitionDao : IElementDefinitionDao
    {
        private List<Thing> writtenThings = new List<Thing>();

        public int WrittenThingCount => this.writtenThings.Count;

        public IEnumerable<ElementDefinition> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            var queriedIds = ids?.ToList();
            if (queriedIds == null)
            {
                return this.writtenThings.OfType<ElementDefinition>();
            }

            return this.writtenThings.OfType<ElementDefinition>().Where(x => queriedIds.Contains(x.Iid));
        }

        public bool Write(NpgsqlTransaction transaction, string partition, ElementDefinition ed, Thing container = null)
        {
            this.writtenThings.Add(ed);
            return true;
        }

        public bool Update(NpgsqlTransaction transaction, string partition, ElementDefinition parameter, Thing container = null)
        {
            throw new NotImplementedException();
        }

        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, OrderedItem orderUpdate)
        {
            throw new NotImplementedException();
        }

        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotImplementedException();
        }
    }
}
