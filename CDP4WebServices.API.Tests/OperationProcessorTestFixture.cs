// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationProcessorTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This the Service operation processor test class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;
    using CDP4Common.MetaInfo;
    using CDP4Common.Types;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Operations;
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using CDP4WebServices.API.Services.Protocol;

    using Moq;

    using NUnit.Framework;

    using Alias = CDP4Common.DTO.Alias;

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

        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };

        private readonly IOperationSideEffectProcessor operationSideEffectProcessor = new OperationSideEffectProcessor(new List<IOperationSideEffect>());

        private readonly SimpleQuantityKindMetaInfo simpleQuantityKindMetaInfo = new SimpleQuantityKindMetaInfo();

        private readonly EngineeringModelMetaInfo engineeringModelMetaInfo = new EngineeringModelMetaInfo();

        private readonly Dictionary<string, Stream> fileStore = new Dictionary<string, Stream>();

        private Mock<IMetaInfoProvider> mockedMetaInfoProvider;

        private OperationProcessor operationProcessor;

        private CdpPostOperation operation;

        [SetUp]
        public void TestSetup()
        {
            this.mockedMetaInfoProvider = new Mock<IMetaInfoProvider>();
            this.requestUtils.MetaInfoProvider = this.mockedMetaInfoProvider.Object;
            this.operationProcessor = new OperationProcessor();
            this.operationProcessor.RequestUtils = this.requestUtils;
            this.operationProcessor.OperationSideEffectProcessor = this.operationSideEffectProcessor;
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
                () => this.operationProcessor.ValidateDeleteOperations(postOperation));

            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithoutClassKind);

            Assert.Throws(
                typeof(InvalidOperationException),
                () => this.operationProcessor.ValidateDeleteOperations(postOperation));

            var completeDeleteObject = new ClasslessDTO() { { IidKey, Guid.NewGuid() }, { ClasskindKey, SimpleQuantityKindTypeString } };
            postOperation.Delete.Clear();
            postOperation.Delete.Add(completeDeleteObject);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateDeleteOperations(postOperation));
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
                () => this.operationProcessor.ValidateDeleteOperations(postOperation));

            var deleteObjectWithListProperty = new ClasslessDTO()
                                                    {
                                                        { IidKey, Guid.NewGuid() },
                                                        { ClasskindKey, SimpleQuantityKindTypeString },
                                                        { "PossibleScale", new[] { Guid.NewGuid() } }
                                                    };
            postOperation.Delete.Clear();
            postOperation.Delete.Add(deleteObjectWithListProperty);

            Assert.DoesNotThrow(() => this.operationProcessor.ValidateDeleteOperations(postOperation));
        }

        [Test]
        public void VerifyCreateInvalidThingValidation()
        {
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.IsAny<SimpleQuantityKind>())).Returns(this.simpleQuantityKindMetaInfo);
            this.mockedMetaInfoProvider.Setup(x => x.GetMetaInfo(It.Is<string>(y => y == SimpleQuantityKindTypeString))).Returns(this.simpleQuantityKindMetaInfo);

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
    }
}
