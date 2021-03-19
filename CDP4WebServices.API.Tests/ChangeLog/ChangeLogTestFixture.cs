// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLogTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
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

namespace CDP4WebServices.API.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;
    using CDP4Common.Types;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.ChangeLog;
    using CDP4WebServices.API.Services.Operations;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using IServiceProvider = CDP4WebServices.API.Services.IServiceProvider;
    using LogEntryChangelogItem = CDP4Common.DTO.LogEntryChangelogItem;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Suite of tests for the <see cref="ChangeLogTestFixture"/>
    /// </summary>
    [TestFixture]
    public class ChangeLogTestFixture
    {
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IOperationProcessor> operationProcessor;
        private Mock<IRequestUtils> requestUtils;
        private Mock<IResolveService> resolveService;
        private Mock<IOptionService> optionService;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<IActualFiniteStateService> actualFiniteStateService;
        private Mock<IPossibleFiniteStateService> possibleFiniteStateService;
        private Mock<IParameterTypeService> parameterTypeService;
        private Mock<IParameterService> parameterService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<IMetaInfoProvider> metaInfoProvider;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IElementDefinitionService> elementDefinitionService;
        private Mock<IElementUsageService> elementUsageService;
        private Mock<IIterationService> iterationService;
        private Mock<IIterationSetupService> iterationSetupService;
        private Mock<IParameterOrOverrideBaseService> parameterOrOverrideBaseService;
        private Mock<IParameterValueSetService> parameterValueSetService;
        private Mock<IParameterValueSetBaseService> parameterValueSetBaseService;
        private Mock<IDomainOfExpertiseService> domainOfExpertiseService;
        private IDataModelUtils dataModelUtils;
        private string partition;
        private Guid actor;
        private ModelLogEntry existingModelLogEntry;
        private ElementDefinition elementDefinition_1;
        private EngineeringModel engineeringModel;
        private Iteration iteration;
        private ChangeLogService changeLogService;
        private RequirementsSpecification requirementsSpecification;
        private ParameterType parameterType;
        private Parameter parameter;
        private DomainOfExpertise domain_ElementDefinition;
        private DomainOfExpertise domain_Parameter;
        private DomainOfExpertise domain_ElementUsage;
        private DomainOfExpertise domain_ParameterOverride;
        private DomainOfExpertise domain_ParameterSubscription;
        private DomainOfExpertise domain_ParameterOverrideSubscription;

        private Category category_ElementDefinition;
        private Category category_ParameterType;
        private Category category_ElementUsage;

        private ParameterValueSet parameterValueSet_1;
        private ActualFiniteState actualFiniteState;
        private Option option;
        private PossibleFiniteState possibleFiniteState;
        private ElementUsage elementUsage_1;
        private IterationSetup iterationSetup;
        private ParameterOverrideValueSet parameterOverrideValueSet_1;
        private ParameterOverride parameterOverride;
        private ParameterSubscription parameterSubscription;
        private ParameterSubscriptionValueSet parameterSubscriptionValueSet_1;
        private ParameterSubscriptionValueSet parameterOverrideSubscriptionValueSet_1;
        private ParameterSubscription parameterOverrideSubscription;

        [SetUp]
        public void Setup()
        {
            this.serviceProvider = new Mock<IServiceProvider>();
            this.operationProcessor = new Mock<IOperationProcessor>();
            this.requestUtils = new Mock<IRequestUtils>();
            this.resolveService = new Mock<IResolveService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.optionService = new Mock<IOptionService>();
            this.actualFiniteStateService = new Mock<IActualFiniteStateService>();
            this.possibleFiniteStateService = new Mock<IPossibleFiniteStateService>();
            this.parameterTypeService = new Mock<IParameterTypeService>();
            this.parameterService = new Mock<IParameterService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterValueSetService = new Mock<IParameterValueSetService>();
            this.elementDefinitionService = new Mock<IElementDefinitionService>();
            this.elementUsageService = new Mock<IElementUsageService>();
            this.iterationService = new Mock<IIterationService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.parameterOrOverrideBaseService = new Mock<IParameterOrOverrideBaseService>();
            this.parameterValueSetBaseService = new Mock<IParameterValueSetBaseService>();
            this.domainOfExpertiseService = new Mock<IDomainOfExpertiseService>();

            this.metaInfoProvider = new Mock<IMetaInfoProvider>();
            this.dataModelUtils = new DataModelUtils();

            this.changeLogService = new ChangeLogService()
            {
                ServiceProvider = this.serviceProvider.Object,
                OperationProcessor = this.operationProcessor.Object,
                RequestUtils = this.requestUtils.Object,
                OptionService = this.optionService.Object,
                ActualFiniteStateService = this.actualFiniteStateService.Object,
                PossibleFiniteStateService = this.possibleFiniteStateService.Object,
                ParameterTypeService = this.parameterTypeService.Object,
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object,
                ParameterSubscriptionService = this.parameterSubscriptionService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                ParameterOrOverrideBaseService = this.parameterOrOverrideBaseService.Object,
                ParameterValueSetService = this.parameterValueSetService.Object,
                ParameterValueSetBaseService = this.parameterValueSetBaseService.Object,
                ResolveService = this.resolveService.Object,
                TransactionManager = this.transactionManager.Object,
                DataModelUtils = this.dataModelUtils,
            };

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing>());

            this.requirementsSpecification = new RequirementsSpecification(Guid.NewGuid(), 0);

            this.domain_ElementDefinition = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 1",
                ShortName = "D1"
            };

            this.domain_Parameter = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 2",
                ShortName = "D2"
            };

            this.domain_ElementUsage = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 3",
                ShortName = "D3"
            };

            this.domain_ParameterOverride = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 4",
                ShortName = "D4"
            };

            this.domain_ParameterSubscription = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 5",
                ShortName = "D5"
            };

            this.domain_ParameterOverrideSubscription = new DomainOfExpertise(Guid.NewGuid(), 0)
            {
                Name = "Domain 6",
                ShortName = "D6"
            };

            this.category_ElementDefinition = new Category(Guid.NewGuid(), 0)
                {
                    Name= "Category 1",
                    ShortName = "C1"
                };

            this.category_ParameterType = new Category(Guid.NewGuid(), 0)
            {
                Name= "Category 2",
                ShortName = "C2"
            };

            this.category_ElementUsage = new Category(Guid.NewGuid(), 0)
            {
                Name= "Category 3",
                ShortName = "C3"
            };

            this.option = new Option(Guid.NewGuid(), 0)
            {
                Name = "Option",
                ShortName = "Option"
            };

            this.possibleFiniteState = new PossibleFiniteState(Guid.NewGuid(), 0)
            {
                Name = "State 1",
                ShortName = "S1"
            };

            this.actualFiniteState = new ActualFiniteState(Guid.NewGuid(), 0);
            this.actualFiniteState.PossibleState.Add(this.possibleFiniteState.Iid);

            this.partition = "EngineeringModel_partition";
            this.actor = Guid.NewGuid();

            this.elementDefinition_1 = new ElementDefinition(Guid.NewGuid(), 0)
            {
                Name = "Element 1",
                ShortName = "E1",
                Owner = this.domain_ElementDefinition.Iid
            };

            this.elementDefinition_1.Category.Add(this.category_ElementDefinition.Iid);

            this.elementUsage_1 = new ElementUsage(Guid.NewGuid(), 0)
            {
                Name = "Usage 1",
                ShortName = "U1",
                Owner = this.domain_ElementUsage.Iid
            };

            this.elementUsage_1.Category.Add(this.category_ElementUsage.Iid);

            this.elementDefinition_1.ContainedElement.Add(this.elementUsage_1.Iid);
            
            this.engineeringModel = new EngineeringModel(Guid.NewGuid(), 0);

            this.iteration = new Iteration(Guid.NewGuid(), 0);

            this.iterationSetup = new IterationSetup(Guid.NewGuid(), 0)
            {
                IterationNumber = 1,
                IterationIid = this.iteration.Iid
            };

            this.iteration.IterationSetup = this.iterationSetup.Iid;
            this.iteration.Element.Add(this.elementDefinition_1.Iid);

            this.existingModelLogEntry = new ModelLogEntry(Guid.NewGuid(), 0)
            {
                Level = LogLevelKind.USER,
                Author = this.actor,
                LanguageCode = "en-GB",
                Content = "User generated content"
            };

            this.parameterType = new SimpleQuantityKind(Guid.NewGuid(), 0)
            {
                Name = "Quantity",
                ShortName = "Q"
            };

            this.parameterType.Category.Add(this.category_ParameterType.Iid);

            this.parameterValueSet_1 = new ParameterValueSet(Guid.NewGuid(), 0)
            {
                ActualOption = this.option.Iid,
                ActualState = this.actualFiniteState.Iid,
            };

            this.parameter = new Parameter(Guid.NewGuid(), 0)
            {
                ParameterType = this.parameterType.Iid,
                Owner = this.domain_Parameter.Iid,
            };

            this.parameter.ValueSet.Add(this.parameterValueSet_1.Iid);

            this.parameterSubscriptionValueSet_1 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 0)
            {
                SubscribedValueSet = this.parameterValueSet_1.Iid
            };

            this.parameterSubscription = new ParameterSubscription(Guid.NewGuid(), 0)
            {
                Owner = this.domain_ParameterSubscription.Iid,
            };

            this.parameter.ParameterSubscription.Add(this.parameterSubscription.Iid);

            this.parameterSubscription.ValueSet.Add(this.parameterSubscriptionValueSet_1.Iid);

            this.elementDefinition_1.Parameter.Add(this.parameter.Iid);

            this.parameterOverrideValueSet_1 = new ParameterOverrideValueSet(Guid.NewGuid(), 0)
            {
                ParameterValueSet = this.parameterValueSet_1.Iid
            };

            this.parameterOverride = new ParameterOverride(Guid.NewGuid(), 0)
            {
                Parameter = this.parameter.Iid,
                Owner = this.domain_ParameterOverride.Iid
            };

            this.parameterOverride.ValueSet.Add(this.parameterOverrideValueSet_1.Iid);

            this.parameterOverrideSubscriptionValueSet_1 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 0)
            {
                SubscribedValueSet = this.parameterOverrideValueSet_1.Iid
            };

            this.parameterOverrideSubscription = new ParameterSubscription(Guid.NewGuid(), 0)
            {
                Owner = this.domain_ParameterOverrideSubscription.Iid,
            };

            this.parameterOverrideSubscription.ValueSet.Add(this.parameterOverrideSubscriptionValueSet_1.Iid);

            this.parameterOverride.ParameterSubscription.Add(this.parameterOverrideSubscription.Iid);

            this.elementUsage_1.ParameterOverride.Add(this.parameterOverride.Iid);

            this.metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.EngineeringModel.ToString())).Returns(new EngineeringModelMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ModelLogEntry.ToString())).Returns(new ModelLogEntryMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo(this.parameter)).Returns(new ParameterMetaInfo());

            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ElementDefinition.ToString())).Returns(this.elementDefinitionService.Object);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.Iteration.ToString())).Returns(this.iterationService.Object);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ElementUsage.ToString())).Returns(this.elementUsageService.Object);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ParameterOverride.ToString())).Returns(this.parameterOverrideService.Object);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.ParameterOrOverrideBase.ToString())).Returns(this.parameterOrOverrideBaseService.Object);
            this.serviceProvider.Setup(x => x.MapToPersitableService(ClassKind.DomainOfExpertise.ToString())).Returns(this.domainOfExpertiseService.Object);

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(false);

            this.requestUtils.Setup(x => x.MetaInfoProvider).Returns(this.metaInfoProvider.Object);

            this.elementDefinitionService.Setup(
                    x =>
                        x.GetShallow(
                            null, 
                            It.IsAny<string>(), 
                            It.IsAny<IEnumerable<Guid>>(), 
                            It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.elementDefinition_1 });

            this.elementUsageService.Setup(
                    x =>
                        x.GetShallow(null, 
                            It.IsAny<string>(), 
                            It.IsAny<IEnumerable<Guid>>(), 
                            It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.elementUsage_1 });

            this.iterationService.Setup(
                    x =>
                        x.GetShallow(null, 
                            It.IsAny<string>(), 
                            It.IsAny<IEnumerable<Guid>>(), 
                            It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.iteration });
            
            this.optionService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.option.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.option });

            this.actualFiniteStateService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.actualFiniteState.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.actualFiniteState });

            this.possibleFiniteStateService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        this.actualFiniteState.PossibleState,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.possibleFiniteState });

            this.parameterService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameter });

            this.parameterService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameter.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameter });

            this.parameterTypeService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameterType.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameterType });

            this.parameterOrOverrideBaseService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { (ParameterOrOverrideBase) this.parameter, this.parameterOverride });

            this.parameterValueSetBaseService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameterValueSet_1.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { (ParameterValueSetBase) this.parameterValueSet_1 });

            this.parameterValueSetBaseService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameterOverrideValueSet_1.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { (ParameterValueSetBase) this.parameterOverrideValueSet_1 });

            this.domainOfExpertiseService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.domain_ElementDefinition.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.domain_ElementDefinition });

            this.domainOfExpertiseService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.domain_Parameter.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.domain_Parameter });

            this.elementDefinitionService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.elementDefinition_1 });

            this.elementUsageService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.elementUsage_1 });

            this.iterationSetupService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.iteration.IterationSetup },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.iterationSetup });

            this.parameterValueSetService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameterOverrideValueSet_1.ParameterValueSet },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameterValueSet_1 });

            this.parameterOverrideService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameterOverride });

            this.parameterOverrideService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        new[] { this.parameterOverride.Iid },
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameterOverride });

            this.parameterSubscriptionService.Setup(
                    x => x.GetShallow(
                        null,
                        It.IsAny<string>(),
                        null,
                        It.IsAny<ISecurityContext>()))
                .Returns(new[] { this.parameterSubscription, this.parameterOverrideSubscription });
        }

        [Test]
        public void VerifyAppendModelChangeLogDataOnlyWorksOnEngineeringModelChanges()
        {
            var postOperation = new CdpPostOperation();

            var iterationClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.iteration.Iid },
                { nameof(Thing.ClassKind), ClassKind.Iteration.ToString() },
                { nameof(Iteration.Element), new[] { this.elementDefinition_1.Iid }.ToList() }
            };

            postOperation.Update.Add(iterationClasslessDto);
            postOperation.Create.Add(this.elementDefinition_1);

            var things = new Thing[] { this.iteration, this.elementDefinition_1 };

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsFalse(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Never);

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Update.Add(engineeringModelClasslessDto);
            postOperation.Create.Add(this.existingModelLogEntry);

            things = new Thing[] { this.iteration, this.elementDefinition_1, this.engineeringModel, this.existingModelLogEntry };

            result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Once);
        }

        [Test]
        public void VerifyAppendModelChangeLogDataAddsModelLogEntryWhenNeeded()
        {
            var postOperation = new CdpPostOperation();

            var iterationClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.iteration.Iid },
                { nameof(Thing.ClassKind), ClassKind.Iteration.ToString() },
                { nameof(Iteration.Element), new[] { this.elementDefinition_1.Iid }.ToList() }
            };

            postOperation.Update.Add(iterationClasslessDto);
            postOperation.Create.Add(this.elementDefinition_1);

            var things = new Thing[] { this.iteration, this.elementDefinition_1, this.engineeringModel };

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatUnsupportedThingsAreNotAddedToTheModelLogEntry()
        {
            var postOperation = new CdpPostOperation();

            var iterationClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.iteration.Iid },
                { nameof(Thing.ClassKind), ClassKind.Iteration.ToString() },
                { nameof(Iteration.RequirementsSpecification), new[] { this.requirementsSpecification.Iid }.ToList() },
            };

            postOperation.Update.Add(iterationClasslessDto);
            postOperation.Create.Add(this.requirementsSpecification);

            var things = new Thing[] { this.iteration, this.requirementsSpecification, this.engineeringModel };

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsFalse(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(0));

            things = new Thing[] { this.iteration, this.requirementsSpecification, this.elementDefinition_1, this.engineeringModel };
            postOperation.Create.Add(this.elementDefinition_1);
            postOperation.Create.Add(this.existingModelLogEntry);

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Update.Add(engineeringModelClasslessDto);

            result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForNewParameter()
        {
            var postOperation = new CdpPostOperation();

            var elementDefinitionClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.elementDefinition_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementDefinition.ToString() },
                { nameof(ElementDefinition.Parameter), new[] { this.parameter.Iid }.ToList() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.parameter);
            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(elementDefinitionClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameter, this.elementDefinition_1, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(2, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var parameterCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameter.Iid);

            var expectedElementDefinitionAffectedReferenceItems = new[]
            {
                this.iteration.Iid,
                this.category_ElementDefinition.Iid,
                this.domain_ElementDefinition.Iid
            };

            var elementDefinitionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.elementDefinition_1.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.iteration.Iid,
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid
            };

            CollectionAssert.AreEquivalent(expectedParameterAffectedReferenceItems, parameterCreatedLogEntries.AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedElementDefinitionAffectedReferenceItems, elementDefinitionCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForDeletedParameter()
        {
            var postOperation = new CdpPostOperation();

            var parameterClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameter.Iid },
                { nameof(Thing.ClassKind), ClassKind.Parameter.ToString() }
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Delete.Add(parameterClasslessDto);
            postOperation.Create.Add(this.existingModelLogEntry);
            postOperation.Update.Add(engineeringModelClasslessDto);

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing>
            {
                this.parameter,
                this.elementDefinition_1,
            });

            var things = new Thing[] { this.elementDefinition_1, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameterSubscription.Iid,
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ElementDefinition.Iid,
            };

            var parameterCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameter.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.parameterSubscription.Iid,
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid
            };

            CollectionAssert.AreEquivalent(expectedParameterAffectedReferenceItems, parameterCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameter()
        {
            var postOperation = new CdpPostOperation();

            var parameterClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameter.Iid },
                { nameof(Thing.ClassKind), ClassKind.Parameter.ToString() },
                { nameof(Parameter.Owner), this.domain_ElementDefinition.Iid }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.iteration, this.parameter, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing> {this.parameter});

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameterSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.parameterSubscription.Iid,
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameterValueSet()
        {
            var postOperation = new CdpPostOperation();

            var parameterValueSetClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterValueSet_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterValueSet.ToString() },
                { nameof(ParameterValueSet.Manual), new ValueArray<string>(new[] { "100" }) }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Update.Add(engineeringModelClasslessDto);

            postOperation.Create.Add(this.existingModelLogEntry);
            postOperation.Update.Add(parameterValueSetClasslessDto);

            var things = new Thing[] { this.parameterValueSet_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid, 
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid, 
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.parameterValueSet_1.Iid,
                this.parameterSubscription.Iid,
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForNewElementUsage()
        {
            var postOperation = new CdpPostOperation();

            var elementDefinitionClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.elementDefinition_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementDefinition.ToString() },
                { nameof(ElementDefinition.ContainedElement), new[] { this.elementUsage_1.Iid }.ToList() }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Update.Add(engineeringModelClasslessDto);

            postOperation.Update.Add(elementDefinitionClasslessDto);
            postOperation.Create.Add(this.elementUsage_1);
            postOperation.Create.Add(this.existingModelLogEntry);

            var things = new Thing[] { this.elementUsage_1, this.elementDefinition_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Once);

            Assert.AreEqual(2, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedUsageAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid
            };

            var usageCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.elementUsage_1.Iid);

            var expectedElementDefinitionAffectedReferenceItems = new[]
            {
                this.iteration.Iid,
                this.category_ElementDefinition.Iid,
                this.domain_ElementDefinition.Iid
            };

            var elementDefinitionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.elementDefinition_1.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.iteration.Iid,
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
            };

            CollectionAssert.AreEquivalent(expectedUsageAffectedReferenceItems, usageCreatedLogEntries.AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedElementDefinitionAffectedReferenceItems, elementDefinitionCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForDeletedElementUsage()
        {
            var postOperation = new CdpPostOperation();

            var elementUsageClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.elementUsage_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementUsage.ToString() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Update.Add(engineeringModelClasslessDto);

            postOperation.Delete.Add(elementUsageClasslessDto);
            postOperation.Create.Add(this.existingModelLogEntry);

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing>
            {
                this.elementUsage_1,
                this.elementDefinition_1,
            });

            var things = new Thing[] { this.elementDefinition_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Once);

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedUsageAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_ElementDefinition.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
            };

            var usageCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.elementUsage_1.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
            };

            CollectionAssert.AreEquivalent(expectedUsageAffectedReferenceItems, usageCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedElementUsage()
        {
            var postOperation = new CdpPostOperation();

            var elementUsageClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.elementUsage_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementUsage.ToString() },
                { nameof(ElementUsage.Name), $"{this.elementUsage_1.Name} was changed" }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(elementUsageClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.elementUsage_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForNewParameterOverride()
        {
            var postOperation = new CdpPostOperation();

            var elementUsageClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.elementUsage_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementUsage.ToString() },
                { nameof(ElementUsage.ParameterOverride), new[] { this.parameterOverride.Iid }.ToList() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.parameterOverride);
            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(elementUsageClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterOverride, this.elementUsage_1, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(2, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterOverrideAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_Parameter.Iid,
            };

            var parameterOverrideCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterOverride.Iid);

            var expectedElementUsageAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.category_ElementUsage.Iid,
                this.category_ElementDefinition.Iid,
            };

            var elementDefinitionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.elementUsage_1.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_Parameter.Iid,
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.parameterOverride.Iid,
            };

            CollectionAssert.AreEquivalent(expectedParameterOverrideAffectedReferenceItems, parameterOverrideCreatedLogEntries.AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedElementUsageAffectedReferenceItems, elementDefinitionCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

                [Test]
        public void VerifyThatResultsAreAsExpectedForDeletedParameterOverride()
        {
            var postOperation = new CdpPostOperation();

            var parameterOverrideClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterOverride.Iid },
                { nameof(Thing.ClassKind), ClassKind.ElementUsage.ToString() }
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.parameterOverride);
            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Delete.Add(parameterOverrideClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing>
            {
                this.parameterOverride,
                this.elementUsage_1,
            });

            var things = new Thing[] { this.elementUsage_1, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterOverrideAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid
            };

            var parameterOverrideCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterOverride.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameterType.Iid,
                this.parameter.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
            };

            CollectionAssert.AreEquivalent(expectedParameterOverrideAffectedReferenceItems, parameterOverrideCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameterOverride()
        {
            var postOperation = new CdpPostOperation();

            var parameterOverrideClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterOverride.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterOverride.ToString() },
                { nameof(Parameter.Owner), this.domain_ElementDefinition.Iid }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterOverrideClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterOverride, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameterOverrideValueSet()
        {
            var postOperation = new CdpPostOperation();

            var parameterOverrideValueSetClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterOverrideValueSet_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterOverrideValueSet.ToString() },
                { nameof(ParameterValueSet.Manual), new ValueArray<string>(new[] { "100" }) }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);
            postOperation.Update.Add(engineeringModelClasslessDto);
            postOperation.Update.Add(parameterOverrideValueSetClasslessDto);

            var things = new Thing[] { this.parameterOverrideValueSet_1, this.engineeringModel, this.existingModelLogEntry };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.parameterOverrideValueSet_1.Iid
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForNewParameterSubscription_SubscriptionIsOnParameter()
        {
            var postOperation = new CdpPostOperation();

            this.parameter.ParameterSubscription.Add(this.parameterSubscription.Iid);

            var parameterClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameter.Iid },
                { nameof(Thing.ClassKind), ClassKind.Parameter.ToString() },
                { nameof(Parameter.ParameterSubscription), new[] { this.parameterSubscription.Iid }.ToList() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.parameterSubscription);
            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterSubscription, this.parameter, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(2, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterSubscriptionAffectedReferenceItems = new[]
            {
                this.parameter.Iid,
                this.parameterType.Iid,
                this.elementDefinition_1.Iid,
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid
            };

            var parameterSubscriptionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterSubscription.Iid);

            var expectedParameterAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_Parameter.Iid,
                this.domain_ElementDefinition.Iid,
            };

            var parameterCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameter.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.parameterSubscription.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.elementDefinition_1.Iid,
            };

            CollectionAssert.AreEquivalent(expectedParameterSubscriptionAffectedReferenceItems, parameterSubscriptionCreatedLogEntries.AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedParameterAffectedReferenceItems, parameterCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForDeletedParameterSubscription()
        {
            var postOperation = new CdpPostOperation();

            this.parameter.ParameterSubscription.Add(this.parameterSubscription.Iid);

            var parameterSubscriptionClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterSubscription.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterSubscription.ToString() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Delete.Add(parameterSubscriptionClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            this.operationProcessor.Setup(x => x.OperationOriginalThingCache).Returns(new List<Thing>
            {
                this.parameterSubscription,
                this.parameter,
            });

            var things = new Thing[] { this.parameter, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterSubscriptionAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid,
                this.domain_ElementDefinition.Iid,
            };

            var parameterSubscriptionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterSubscription.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.category_ParameterType.Iid,
                this.category_ElementDefinition.Iid,
                this.parameterSubscription.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.elementDefinition_1.Iid,
            };

            CollectionAssert.AreEquivalent(expectedParameterSubscriptionAffectedReferenceItems, parameterSubscriptionCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameterSubscriptionValueSet_SubscriptionIsOnParameter()
        {
            var postOperation = new CdpPostOperation();

            this.parameter.ParameterSubscription.Add(this.parameterSubscription.Iid);

            var parameterSubscriptionValueSetClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterSubscriptionValueSet_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterSubscriptionValueSet.ToString() },
                { nameof(ParameterSubscriptionValueSet.Manual), new ValueArray<string>(new[] { "100" }) }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterSubscriptionValueSetClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterSubscriptionValueSet_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterSubscription.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.parameterSubscriptionValueSet_1.Iid
            };

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForNewParameterSubscription_SubscriptionIsOnParameterOverride()
        {
            var postOperation = new CdpPostOperation();

            this.parameterOverride.ParameterSubscription.Add(this.parameterOverrideSubscription.Iid);

            var parameterClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterOverride.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterOverride.ToString() },
                { nameof(Parameter.ParameterSubscription), new[] { this.parameterOverrideSubscription.Iid }.ToList() },
            };

            var engineeringModelClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.parameterOverrideSubscription);
            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterOverrideSubscription, this.parameterOverride, this.engineeringModel };

            var createdLogEntries = new List<LogEntryChangelogItem>();

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries.AddRange(operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray());
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(2, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedParameterOverrideSubscriptionAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.category_ElementUsage.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_ParameterOverrideSubscription.Iid
            };

            var parameterOverrideSubscriptionCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterOverrideSubscription.Iid);

            var expectedParameterOverrideAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.category_ElementUsage.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid
            };

            var parameterOverrideCreatedLogEntries = createdLogEntries.Single(x => x.AffectedItemIid == this.parameterOverride.Iid);

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_ParameterOverrideSubscription.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.category_ElementUsage.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ParameterType.Iid,
                this.parameterOverrideSubscription.Iid,
            };

            CollectionAssert.AreEquivalent(expectedParameterOverrideSubscriptionAffectedReferenceItems, parameterOverrideSubscriptionCreatedLogEntries.AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedParameterOverrideAffectedReferenceItems, parameterOverrideCreatedLogEntries.AffectedReferenceIid);

            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }

        [Test]
        public void VerifyThatResultsAreAsExpectedForUpdatedParameterSubscriptionValueSet_SubscriptionIsOnParameterOverride()
        {
            var postOperation = new CdpPostOperation();

            this.parameterOverride.ParameterSubscription.Add(this.parameterOverrideSubscription.Iid);

            var parameterOverrideSubscriptionValueSetClasslessDto = new ClasslessDTO()
            {
                { nameof(Thing.Iid), this.parameterOverrideSubscriptionValueSet_1.Iid },
                { nameof(Thing.ClassKind), ClassKind.ParameterSubscriptionValueSet.ToString() },
                { nameof(ParameterSubscriptionValueSet.Manual), new ValueArray<string>(new[] { "100" }) }
            };

            var engineeringModelClasslessDto = new ClasslessDTO
            {
                { nameof(Thing.Iid), this.engineeringModel.Iid },
                { nameof(Thing.ClassKind), ClassKind.EngineeringModel.ToString() },
                { nameof(EngineeringModel.LogEntry), new[] { this.existingModelLogEntry.Iid }.ToList() }
            };

            postOperation.Create.Add(this.existingModelLogEntry);

            postOperation.Update.Add(parameterOverrideSubscriptionValueSetClasslessDto);
            postOperation.Update.Add(engineeringModelClasslessDto);

            var things = new Thing[] { this.parameterOverrideSubscriptionValueSet_1, this.engineeringModel };

            LogEntryChangelogItem[] createdLogEntries = { };

            this.operationProcessor.Setup(
                    x =>
                        x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null))
                .Callback<CdpPostOperation, NpgsqlTransaction, string, Dictionary<string, Stream>>(
                    (operation, transaction, partition, files)
                        =>
                    {
                        createdLogEntries = operation.Create.Where(x => x.ClassKind == ClassKind.LogEntryChangelogItem).Cast<LogEntryChangelogItem>().ToArray();
                    });

            var result = this.changeLogService.TryAppendModelChangeLogData(null, this.partition, this.actor, 0, postOperation, things);

            Assert.IsTrue(result);

            this.operationProcessor.Verify(
                x => x.Process(It.IsAny<CdpPostOperation>(), null, It.IsAny<string>(), null), Times.Exactly(1));

            Assert.AreEqual(1, this.existingModelLogEntry.LogEntryChangelogItem.Count);

            var expectedAffectedReferenceItems = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_ParameterOverrideSubscription.Iid
            };

            var expectedAffectedDomains = new[]
            {
                this.domain_ElementDefinition.Iid,
                this.domain_ElementUsage.Iid,
                this.domain_Parameter.Iid,
                this.domain_ParameterOverride.Iid,
                this.domain_ParameterOverrideSubscription.Iid
            };

            var expectedAffectedItemIds = new[]
            {
                this.elementDefinition_1.Iid,
                this.elementUsage_1.Iid,
                this.parameter.Iid,
                this.parameterType.Iid,
                this.parameterOverride.Iid,
                this.parameterOverrideSubscription.Iid,
                this.category_ElementDefinition.Iid,
                this.category_ElementUsage.Iid,
                this.category_ParameterType.Iid,
                this.parameterOverrideSubscriptionValueSet_1.Iid
            };

            CollectionAssert.AreEquivalent(expectedAffectedReferenceItems, createdLogEntries.Single(x => x.Iid == this.existingModelLogEntry.LogEntryChangelogItem.First()).AffectedReferenceIid);
            CollectionAssert.AreEquivalent(expectedAffectedDomains, this.existingModelLogEntry.AffectedDomainIid);
            CollectionAssert.AreEquivalent(expectedAffectedItemIds, this.existingModelLogEntry.AffectedItemIid);
        }
    }
}
