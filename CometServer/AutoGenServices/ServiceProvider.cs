// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProvider.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// A service registry class that allows retrieval of service instances by type name.
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        /// <summary>
        /// The type to read service instance map.
        /// </summary>
        private Dictionary<string, IReadService> readServiceMap;

        /// <summary>
        /// The type to persistable service instance map.
        /// </summary>
        private Dictionary<string, IPersistService> persistableServiceMap;

        /// <summary>
        /// Gets or sets the actionItem service.
        /// </summary>
        public IActionItemService ActionItemService { get; set; }

        /// <summary>
        /// Gets or sets the actualFiniteState service.
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the actualFiniteStateList service.
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the alias service.
        /// </summary>
        public IAliasService AliasService { get; set; }

        /// <summary>
        /// Gets or sets the andExpression service.
        /// </summary>
        public IAndExpressionService AndExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the approval service.
        /// </summary>
        public IApprovalService ApprovalService { get; set; }

        /// <summary>
        /// Gets or sets the arrayParameterType service.
        /// </summary>
        public IArrayParameterTypeService ArrayParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the binaryNote service.
        /// </summary>
        public IBinaryNoteService BinaryNoteService { get; set; }

        /// <summary>
        /// Gets or sets the binaryRelationship service.
        /// </summary>
        public IBinaryRelationshipService BinaryRelationshipService { get; set; }

        /// <summary>
        /// Gets or sets the binaryRelationshipRule service.
        /// </summary>
        public IBinaryRelationshipRuleService BinaryRelationshipRuleService { get; set; }

        /// <summary>
        /// Gets or sets the book service.
        /// </summary>
        public IBookService BookService { get; set; }

        /// <summary>
        /// Gets or sets the booleanExpression service.
        /// </summary>
        public IBooleanExpressionService BooleanExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the booleanParameterType service.
        /// </summary>
        public IBooleanParameterTypeService BooleanParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the bounds service.
        /// </summary>
        public IBoundsService BoundsService { get; set; }

        /// <summary>
        /// Gets or sets the builtInRuleVerification service.
        /// </summary>
        public IBuiltInRuleVerificationService BuiltInRuleVerificationService { get; set; }

        /// <summary>
        /// Gets or sets the category service.
        /// </summary>
        public ICategoryService CategoryService { get; set; }

        /// <summary>
        /// Gets or sets the changeProposal service.
        /// </summary>
        public IChangeProposalService ChangeProposalService { get; set; }

        /// <summary>
        /// Gets or sets the changeRequest service.
        /// </summary>
        public IChangeRequestService ChangeRequestService { get; set; }

        /// <summary>
        /// Gets or sets the citation service.
        /// </summary>
        public ICitationService CitationService { get; set; }

        /// <summary>
        /// Gets or sets the color service.
        /// </summary>
        public IColorService ColorService { get; set; }

        /// <summary>
        /// Gets or sets the commonFileStore service.
        /// </summary>
        public ICommonFileStoreService CommonFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the compoundParameterType service.
        /// </summary>
        public ICompoundParameterTypeService CompoundParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the constant service.
        /// </summary>
        public IConstantService ConstantService { get; set; }

        /// <summary>
        /// Gets or sets the contractChangeNotice service.
        /// </summary>
        public IContractChangeNoticeService ContractChangeNoticeService { get; set; }

        /// <summary>
        /// Gets or sets the contractDeviation service.
        /// </summary>
        public IContractDeviationService ContractDeviationService { get; set; }

        /// <summary>
        /// Gets or sets the conversionBasedUnit service.
        /// </summary>
        public IConversionBasedUnitService ConversionBasedUnitService { get; set; }

        /// <summary>
        /// Gets or sets the cyclicRatioScale service.
        /// </summary>
        public ICyclicRatioScaleService CyclicRatioScaleService { get; set; }

        /// <summary>
        /// Gets or sets the dateParameterType service.
        /// </summary>
        public IDateParameterTypeService DateParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the dateTimeParameterType service.
        /// </summary>
        public IDateTimeParameterTypeService DateTimeParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the decompositionRule service.
        /// </summary>
        public IDecompositionRuleService DecompositionRuleService { get; set; }

        /// <summary>
        /// Gets or sets the definedThing service.
        /// </summary>
        public IDefinedThingService DefinedThingService { get; set; }

        /// <summary>
        /// Gets or sets the definition service.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the dependentParameterTypeAssignment service.
        /// </summary>
        public IDependentParameterTypeAssignmentService DependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the derivedQuantityKind service.
        /// </summary>
        public IDerivedQuantityKindService DerivedQuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the derivedUnit service.
        /// </summary>
        public IDerivedUnitService DerivedUnitService { get; set; }

        /// <summary>
        /// Gets or sets the diagramCanvas service.
        /// </summary>
        public IDiagramCanvasService DiagramCanvasService { get; set; }

        /// <summary>
        /// Gets or sets the diagramEdge service.
        /// </summary>
        public IDiagramEdgeService DiagramEdgeService { get; set; }

        /// <summary>
        /// Gets or sets the diagramElementContainer service.
        /// </summary>
        public IDiagramElementContainerService DiagramElementContainerService { get; set; }

        /// <summary>
        /// Gets or sets the diagramElementThing service.
        /// </summary>
        public IDiagramElementThingService DiagramElementThingService { get; set; }

        /// <summary>
        /// Gets or sets the diagrammingStyle service.
        /// </summary>
        public IDiagrammingStyleService DiagrammingStyleService { get; set; }

        /// <summary>
        /// Gets or sets the diagramObject service.
        /// </summary>
        public IDiagramObjectService DiagramObjectService { get; set; }

        /// <summary>
        /// Gets or sets the diagramShape service.
        /// </summary>
        public IDiagramShapeService DiagramShapeService { get; set; }

        /// <summary>
        /// Gets or sets the diagramThingBase service.
        /// </summary>
        public IDiagramThingBaseService DiagramThingBaseService { get; set; }

        /// <summary>
        /// Gets or sets the discussionItem service.
        /// </summary>
        public IDiscussionItemService DiscussionItemService { get; set; }

        /// <summary>
        /// Gets or sets the domainFileStore service.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the domainOfExpertise service.
        /// </summary>
        public IDomainOfExpertiseService DomainOfExpertiseService { get; set; }

        /// <summary>
        /// Gets or sets the domainOfExpertiseGroup service.
        /// </summary>
        public IDomainOfExpertiseGroupService DomainOfExpertiseGroupService { get; set; }

        /// <summary>
        /// Gets or sets the elementBase service.
        /// </summary>
        public IElementBaseService ElementBaseService { get; set; }

        /// <summary>
        /// Gets or sets the elementDefinition service.
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the elementUsage service.
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Gets or sets the emailAddress service.
        /// </summary>
        public IEmailAddressService EmailAddressService { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModel service.
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataAnnotation service.
        /// </summary>
        public IEngineeringModelDataAnnotationService EngineeringModelDataAnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataDiscussionItem service.
        /// </summary>
        public IEngineeringModelDataDiscussionItemService EngineeringModelDataDiscussionItemService { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataNote service.
        /// </summary>
        public IEngineeringModelDataNoteService EngineeringModelDataNoteService { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelSetup service.
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

        /// <summary>
        /// Gets or sets the enumerationParameterType service.
        /// </summary>
        public IEnumerationParameterTypeService EnumerationParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the enumerationValueDefinition service.
        /// </summary>
        public IEnumerationValueDefinitionService EnumerationValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the exclusiveOrExpression service.
        /// </summary>
        public IExclusiveOrExpressionService ExclusiveOrExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the externalIdentifierMap service.
        /// </summary>
        public IExternalIdentifierMapService ExternalIdentifierMapService { get; set; }

        /// <summary>
        /// Gets or sets the file service.
        /// </summary>
        public IFileService FileService { get; set; }

        /// <summary>
        /// Gets or sets the fileRevision service.
        /// </summary>
        public IFileRevisionService FileRevisionService { get; set; }

        /// <summary>
        /// Gets or sets the fileStore service.
        /// </summary>
        public IFileStoreService FileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the fileType service.
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }

        /// <summary>
        /// Gets or sets the folder service.
        /// </summary>
        public IFolderService FolderService { get; set; }

        /// <summary>
        /// Gets or sets the genericAnnotation service.
        /// </summary>
        public IGenericAnnotationService GenericAnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the glossary service.
        /// </summary>
        public IGlossaryService GlossaryService { get; set; }

        /// <summary>
        /// Gets or sets the goal service.
        /// </summary>
        public IGoalService GoalService { get; set; }

        /// <summary>
        /// Gets or sets the hyperLink service.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }

        /// <summary>
        /// Gets or sets the idCorrespondence service.
        /// </summary>
        public IIdCorrespondenceService IdCorrespondenceService { get; set; }

        /// <summary>
        /// Gets or sets the independentParameterTypeAssignment service.
        /// </summary>
        public IIndependentParameterTypeAssignmentService IndependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the intervalScale service.
        /// </summary>
        public IIntervalScaleService IntervalScaleService { get; set; }

        /// <summary>
        /// Gets or sets the iteration service.
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the iterationSetup service.
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Gets or sets the linearConversionUnit service.
        /// </summary>
        public ILinearConversionUnitService LinearConversionUnitService { get; set; }

        /// <summary>
        /// Gets or sets the logarithmicScale service.
        /// </summary>
        public ILogarithmicScaleService LogarithmicScaleService { get; set; }

        /// <summary>
        /// Gets or sets the logEntryChangelogItem service.
        /// </summary>
        public ILogEntryChangelogItemService LogEntryChangelogItemService { get; set; }

        /// <summary>
        /// Gets or sets the mappingToReferenceScale service.
        /// </summary>
        public IMappingToReferenceScaleService MappingToReferenceScaleService { get; set; }

        /// <summary>
        /// Gets or sets the measurementScale service.
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }

        /// <summary>
        /// Gets or sets the measurementUnit service.
        /// </summary>
        public IMeasurementUnitService MeasurementUnitService { get; set; }

        /// <summary>
        /// Gets or sets the modellingAnnotationItem service.
        /// </summary>
        public IModellingAnnotationItemService ModellingAnnotationItemService { get; set; }

        /// <summary>
        /// Gets or sets the modellingThingReference service.
        /// </summary>
        public IModellingThingReferenceService ModellingThingReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the modelLogEntry service.
        /// </summary>
        public IModelLogEntryService ModelLogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the modelReferenceDataLibrary service.
        /// </summary>
        public IModelReferenceDataLibraryService ModelReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the multiRelationship service.
        /// </summary>
        public IMultiRelationshipService MultiRelationshipService { get; set; }

        /// <summary>
        /// Gets or sets the multiRelationshipRule service.
        /// </summary>
        public IMultiRelationshipRuleService MultiRelationshipRuleService { get; set; }

        /// <summary>
        /// Gets or sets the naturalLanguage service.
        /// </summary>
        public INaturalLanguageService NaturalLanguageService { get; set; }

        /// <summary>
        /// Gets or sets the nestedElement service.
        /// </summary>
        public INestedElementService NestedElementService { get; set; }

        /// <summary>
        /// Gets or sets the nestedParameter service.
        /// </summary>
        public INestedParameterService NestedParameterService { get; set; }

        /// <summary>
        /// Gets or sets the note service.
        /// </summary>
        public INoteService NoteService { get; set; }

        /// <summary>
        /// Gets or sets the notExpression service.
        /// </summary>
        public INotExpressionService NotExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the option service.
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the ordinalScale service.
        /// </summary>
        public IOrdinalScaleService OrdinalScaleService { get; set; }

        /// <summary>
        /// Gets or sets the orExpression service.
        /// </summary>
        public IOrExpressionService OrExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the organization service.
        /// </summary>
        public IOrganizationService OrganizationService { get; set; }

        /// <summary>
        /// Gets or sets the organizationalParticipant service.
        /// </summary>
        public IOrganizationalParticipantService OrganizationalParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the ownedStyle service.
        /// </summary>
        public IOwnedStyleService OwnedStyleService { get; set; }

        /// <summary>
        /// Gets or sets the page service.
        /// </summary>
        public IPageService PageService { get; set; }

        /// <summary>
        /// Gets or sets the parameter service.
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the parameterBase service.
        /// </summary>
        public IParameterBaseService ParameterBaseService { get; set; }

        /// <summary>
        /// Gets or sets the parameterGroup service.
        /// </summary>
        public IParameterGroupService ParameterGroupService { get; set; }

        /// <summary>
        /// Gets or sets the parameterizedCategoryRule service.
        /// </summary>
        public IParameterizedCategoryRuleService ParameterizedCategoryRuleService { get; set; }

        /// <summary>
        /// Gets or sets the parameterOrOverrideBase service.
        /// </summary>
        public IParameterOrOverrideBaseService ParameterOrOverrideBaseService { get; set; }

        /// <summary>
        /// Gets or sets the parameterOverride service.
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the parameterOverrideValueSet service.
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the parameterSubscription service.
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the parameterSubscriptionValueSet service.
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the parameterType service.
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the parameterTypeComponent service.
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the parameterValue service.
        /// </summary>
        public IParameterValueService ParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the parameterValueSet service.
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the parameterValueSetBase service.
        /// </summary>
        public IParameterValueSetBaseService ParameterValueSetBaseService { get; set; }

        /// <summary>
        /// Gets or sets the parametricConstraint service.
        /// </summary>
        public IParametricConstraintService ParametricConstraintService { get; set; }

        /// <summary>
        /// Gets or sets the participant service.
        /// </summary>
        public IParticipantService ParticipantService { get; set; }

        /// <summary>
        /// Gets or sets the participantPermission service.
        /// </summary>
        public IParticipantPermissionService ParticipantPermissionService { get; set; }

        /// <summary>
        /// Gets or sets the participantRole service.
        /// </summary>
        public IParticipantRoleService ParticipantRoleService { get; set; }

        /// <summary>
        /// Gets or sets the person service.
        /// </summary>
        public IPersonService PersonService { get; set; }

        /// <summary>
        /// Gets or sets the personPermission service.
        /// </summary>
        public IPersonPermissionService PersonPermissionService { get; set; }

        /// <summary>
        /// Gets or sets the personRole service.
        /// </summary>
        public IPersonRoleService PersonRoleService { get; set; }

        /// <summary>
        /// Gets or sets the point service.
        /// </summary>
        public IPointService PointService { get; set; }

        /// <summary>
        /// Gets or sets the possibleFiniteState service.
        /// </summary>
        public IPossibleFiniteStateService PossibleFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the possibleFiniteStateList service.
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the prefixedUnit service.
        /// </summary>
        public IPrefixedUnitService PrefixedUnitService { get; set; }

        /// <summary>
        /// Gets or sets the publication service.
        /// </summary>
        public IPublicationService PublicationService { get; set; }

        /// <summary>
        /// Gets or sets the quantityKind service.
        /// </summary>
        public IQuantityKindService QuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the quantityKindFactor service.
        /// </summary>
        public IQuantityKindFactorService QuantityKindFactorService { get; set; }

        /// <summary>
        /// Gets or sets the ratioScale service.
        /// </summary>
        public IRatioScaleService RatioScaleService { get; set; }

        /// <summary>
        /// Gets or sets the referenceDataLibrary service.
        /// </summary>
        public IReferenceDataLibraryService ReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the referencerRule service.
        /// </summary>
        public IReferencerRuleService ReferencerRuleService { get; set; }

        /// <summary>
        /// Gets or sets the referenceSource service.
        /// </summary>
        public IReferenceSourceService ReferenceSourceService { get; set; }

        /// <summary>
        /// Gets or sets the relationalExpression service.
        /// </summary>
        public IRelationalExpressionService RelationalExpressionService { get; set; }

        /// <summary>
        /// Gets or sets the relationship service.
        /// </summary>
        public IRelationshipService RelationshipService { get; set; }

        /// <summary>
        /// Gets or sets the relationshipParameterValue service.
        /// </summary>
        public IRelationshipParameterValueService RelationshipParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the requestForDeviation service.
        /// </summary>
        public IRequestForDeviationService RequestForDeviationService { get; set; }

        /// <summary>
        /// Gets or sets the requestForWaiver service.
        /// </summary>
        public IRequestForWaiverService RequestForWaiverService { get; set; }

        /// <summary>
        /// Gets or sets the requirement service.
        /// </summary>
        public IRequirementService RequirementService { get; set; }

        /// <summary>
        /// Gets or sets the requirementsContainer service.
        /// </summary>
        public IRequirementsContainerService RequirementsContainerService { get; set; }

        /// <summary>
        /// Gets or sets the requirementsContainerParameterValue service.
        /// </summary>
        public IRequirementsContainerParameterValueService RequirementsContainerParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the requirementsGroup service.
        /// </summary>
        public IRequirementsGroupService RequirementsGroupService { get; set; }

        /// <summary>
        /// Gets or sets the requirementsSpecification service.
        /// </summary>
        public IRequirementsSpecificationService RequirementsSpecificationService { get; set; }

        /// <summary>
        /// Gets or sets the reviewItemDiscrepancy service.
        /// </summary>
        public IReviewItemDiscrepancyService ReviewItemDiscrepancyService { get; set; }

        /// <summary>
        /// Gets or sets the rule service.
        /// </summary>
        public IRuleService RuleService { get; set; }

        /// <summary>
        /// Gets or sets the ruleVerification service.
        /// </summary>
        public IRuleVerificationService RuleVerificationService { get; set; }

        /// <summary>
        /// Gets or sets the ruleVerificationList service.
        /// </summary>
        public IRuleVerificationListService RuleVerificationListService { get; set; }

        /// <summary>
        /// Gets or sets the ruleViolation service.
        /// </summary>
        public IRuleViolationService RuleViolationService { get; set; }

        /// <summary>
        /// Gets or sets the sampledFunctionParameterType service.
        /// </summary>
        public ISampledFunctionParameterTypeService SampledFunctionParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the scalarParameterType service.
        /// </summary>
        public IScalarParameterTypeService ScalarParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the scaleReferenceQuantityValue service.
        /// </summary>
        public IScaleReferenceQuantityValueService ScaleReferenceQuantityValueService { get; set; }

        /// <summary>
        /// Gets or sets the scaleValueDefinition service.
        /// </summary>
        public IScaleValueDefinitionService ScaleValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the section service.
        /// </summary>
        public ISectionService SectionService { get; set; }

        /// <summary>
        /// Gets or sets the sharedStyle service.
        /// </summary>
        public ISharedStyleService SharedStyleService { get; set; }

        /// <summary>
        /// Gets or sets the simpleParameterizableThing service.
        /// </summary>
        public ISimpleParameterizableThingService SimpleParameterizableThingService { get; set; }

        /// <summary>
        /// Gets or sets the simpleParameterValue service.
        /// </summary>
        public ISimpleParameterValueService SimpleParameterValueService { get; set; }

        /// <summary>
        /// Gets or sets the simpleQuantityKind service.
        /// </summary>
        public ISimpleQuantityKindService SimpleQuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the simpleUnit service.
        /// </summary>
        public ISimpleUnitService SimpleUnitService { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectory service.
        /// </summary>
        public ISiteDirectoryService SiteDirectoryService { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryDataAnnotation service.
        /// </summary>
        public ISiteDirectoryDataAnnotationService SiteDirectoryDataAnnotationService { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryDataDiscussionItem service.
        /// </summary>
        public ISiteDirectoryDataDiscussionItemService SiteDirectoryDataDiscussionItemService { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryThingReference service.
        /// </summary>
        public ISiteDirectoryThingReferenceService SiteDirectoryThingReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the siteLogEntry service.
        /// </summary>
        public ISiteLogEntryService SiteLogEntryService { get; set; }

        /// <summary>
        /// Gets or sets the siteReferenceDataLibrary service.
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the solution service.
        /// </summary>
        public ISolutionService SolutionService { get; set; }

        /// <summary>
        /// Gets or sets the specializedQuantityKind service.
        /// </summary>
        public ISpecializedQuantityKindService SpecializedQuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the stakeholder service.
        /// </summary>
        public IStakeholderService StakeholderService { get; set; }

        /// <summary>
        /// Gets or sets the stakeholderValue service.
        /// </summary>
        public IStakeholderValueService StakeholderValueService { get; set; }

        /// <summary>
        /// Gets or sets the stakeHolderValueMap service.
        /// </summary>
        public IStakeHolderValueMapService StakeHolderValueMapService { get; set; }

        /// <summary>
        /// Gets or sets the stakeHolderValueMapSettings service.
        /// </summary>
        public IStakeHolderValueMapSettingsService StakeHolderValueMapSettingsService { get; set; }

        /// <summary>
        /// Gets or sets the telephoneNumber service.
        /// </summary>
        public ITelephoneNumberService TelephoneNumberService { get; set; }

        /// <summary>
        /// Gets or sets the term service.
        /// </summary>
        public ITermService TermService { get; set; }

        /// <summary>
        /// Gets or sets the textParameterType service.
        /// </summary>
        public ITextParameterTypeService TextParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the textualNote service.
        /// </summary>
        public ITextualNoteService TextualNoteService { get; set; }

        /// <summary>
        /// Gets or sets the thing service.
        /// </summary>
        public IThingService ThingService { get; set; }

        /// <summary>
        /// Gets or sets the thingReference service.
        /// </summary>
        public IThingReferenceService ThingReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the timeOfDayParameterType service.
        /// </summary>
        public ITimeOfDayParameterTypeService TimeOfDayParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the topContainer service.
        /// </summary>
        public ITopContainerService TopContainerService { get; set; }

        /// <summary>
        /// Gets or sets the unitFactor service.
        /// </summary>
        public IUnitFactorService UnitFactorService { get; set; }

        /// <summary>
        /// Gets or sets the unitPrefix service.
        /// </summary>
        public IUnitPrefixService UnitPrefixService { get; set; }

        /// <summary>
        /// Gets or sets the userPreference service.
        /// </summary>
        public IUserPreferenceService UserPreferenceService { get; set; }

        /// <summary>
        /// Gets or sets the userRuleVerification service.
        /// </summary>
        public IUserRuleVerificationService UserRuleVerificationService { get; set; }

        /// <summary>
        /// Gets or sets the valueGroup service.
        /// </summary>
        public IValueGroupService ValueGroupService { get; set; }

        /// <summary>
        /// Gets the read service map.
        /// </summary>
        private Dictionary<string, IReadService> ReadServiceMap
        {
            get
            {
                if (this.readServiceMap == null)
                {
                    this.readServiceMap = new Dictionary<string, IReadService> 
                        {
                            { "ActionItem", this.ActionItemService },
                            { "ActualFiniteState", this.ActualFiniteStateService },
                            { "ActualFiniteStateList", this.ActualFiniteStateListService },
                            { "Alias", this.AliasService },
                            { "AndExpression", this.AndExpressionService },
                            { "Approval", this.ApprovalService },
                            { "ArrayParameterType", this.ArrayParameterTypeService },
                            { "BinaryNote", this.BinaryNoteService },
                            { "BinaryRelationship", this.BinaryRelationshipService },
                            { "BinaryRelationshipRule", this.BinaryRelationshipRuleService },
                            { "Book", this.BookService },
                            { "BooleanExpression", this.BooleanExpressionService },
                            { "BooleanParameterType", this.BooleanParameterTypeService },
                            { "Bounds", this.BoundsService },
                            { "BuiltInRuleVerification", this.BuiltInRuleVerificationService },
                            { "Category", this.CategoryService },
                            { "ChangeProposal", this.ChangeProposalService },
                            { "ChangeRequest", this.ChangeRequestService },
                            { "Citation", this.CitationService },
                            { "Color", this.ColorService },
                            { "CommonFileStore", this.CommonFileStoreService },
                            { "CompoundParameterType", this.CompoundParameterTypeService },
                            { "Constant", this.ConstantService },
                            { "ContractChangeNotice", this.ContractChangeNoticeService },
                            { "ContractDeviation", this.ContractDeviationService },
                            { "ConversionBasedUnit", this.ConversionBasedUnitService },
                            { "CyclicRatioScale", this.CyclicRatioScaleService },
                            { "DateParameterType", this.DateParameterTypeService },
                            { "DateTimeParameterType", this.DateTimeParameterTypeService },
                            { "DecompositionRule", this.DecompositionRuleService },
                            { "DefinedThing", this.DefinedThingService },
                            { "Definition", this.DefinitionService },
                            { "DependentParameterTypeAssignment", this.DependentParameterTypeAssignmentService },
                            { "DerivedQuantityKind", this.DerivedQuantityKindService },
                            { "DerivedUnit", this.DerivedUnitService },
                            { "DiagramCanvas", this.DiagramCanvasService },
                            { "DiagramEdge", this.DiagramEdgeService },
                            { "DiagramElementContainer", this.DiagramElementContainerService },
                            { "DiagramElementThing", this.DiagramElementThingService },
                            { "DiagrammingStyle", this.DiagrammingStyleService },
                            { "DiagramObject", this.DiagramObjectService },
                            { "DiagramShape", this.DiagramShapeService },
                            { "DiagramThingBase", this.DiagramThingBaseService },
                            { "DiscussionItem", this.DiscussionItemService },
                            { "DomainFileStore", this.DomainFileStoreService },
                            { "DomainOfExpertise", this.DomainOfExpertiseService },
                            { "DomainOfExpertiseGroup", this.DomainOfExpertiseGroupService },
                            { "ElementBase", this.ElementBaseService },
                            { "ElementDefinition", this.ElementDefinitionService },
                            { "ElementUsage", this.ElementUsageService },
                            { "EmailAddress", this.EmailAddressService },
                            { "EngineeringModel", this.EngineeringModelService },
                            { "EngineeringModelDataAnnotation", this.EngineeringModelDataAnnotationService },
                            { "EngineeringModelDataDiscussionItem", this.EngineeringModelDataDiscussionItemService },
                            { "EngineeringModelDataNote", this.EngineeringModelDataNoteService },
                            { "EngineeringModelSetup", this.EngineeringModelSetupService },
                            { "EnumerationParameterType", this.EnumerationParameterTypeService },
                            { "EnumerationValueDefinition", this.EnumerationValueDefinitionService },
                            { "ExclusiveOrExpression", this.ExclusiveOrExpressionService },
                            { "ExternalIdentifierMap", this.ExternalIdentifierMapService },
                            { "File", this.FileService },
                            { "FileRevision", this.FileRevisionService },
                            { "FileStore", this.FileStoreService },
                            { "FileType", this.FileTypeService },
                            { "Folder", this.FolderService },
                            { "GenericAnnotation", this.GenericAnnotationService },
                            { "Glossary", this.GlossaryService },
                            { "Goal", this.GoalService },
                            { "HyperLink", this.HyperLinkService },
                            { "IdCorrespondence", this.IdCorrespondenceService },
                            { "IndependentParameterTypeAssignment", this.IndependentParameterTypeAssignmentService },
                            { "IntervalScale", this.IntervalScaleService },
                            { "Iteration", this.IterationService },
                            { "IterationSetup", this.IterationSetupService },
                            { "LinearConversionUnit", this.LinearConversionUnitService },
                            { "LogarithmicScale", this.LogarithmicScaleService },
                            { "LogEntryChangelogItem", this.LogEntryChangelogItemService },
                            { "MappingToReferenceScale", this.MappingToReferenceScaleService },
                            { "MeasurementScale", this.MeasurementScaleService },
                            { "MeasurementUnit", this.MeasurementUnitService },
                            { "ModellingAnnotationItem", this.ModellingAnnotationItemService },
                            { "ModellingThingReference", this.ModellingThingReferenceService },
                            { "ModelLogEntry", this.ModelLogEntryService },
                            { "ModelReferenceDataLibrary", this.ModelReferenceDataLibraryService },
                            { "MultiRelationship", this.MultiRelationshipService },
                            { "MultiRelationshipRule", this.MultiRelationshipRuleService },
                            { "NaturalLanguage", this.NaturalLanguageService },
                            { "NestedElement", this.NestedElementService },
                            { "NestedParameter", this.NestedParameterService },
                            { "Note", this.NoteService },
                            { "NotExpression", this.NotExpressionService },
                            { "Option", this.OptionService },
                            { "OrdinalScale", this.OrdinalScaleService },
                            { "OrExpression", this.OrExpressionService },
                            { "Organization", this.OrganizationService },
                            { "OrganizationalParticipant", this.OrganizationalParticipantService },
                            { "OwnedStyle", this.OwnedStyleService },
                            { "Page", this.PageService },
                            { "Parameter", this.ParameterService },
                            { "ParameterBase", this.ParameterBaseService },
                            { "ParameterGroup", this.ParameterGroupService },
                            { "ParameterizedCategoryRule", this.ParameterizedCategoryRuleService },
                            { "ParameterOrOverrideBase", this.ParameterOrOverrideBaseService },
                            { "ParameterOverride", this.ParameterOverrideService },
                            { "ParameterOverrideValueSet", this.ParameterOverrideValueSetService },
                            { "ParameterSubscription", this.ParameterSubscriptionService },
                            { "ParameterSubscriptionValueSet", this.ParameterSubscriptionValueSetService },
                            { "ParameterType", this.ParameterTypeService },
                            { "ParameterTypeComponent", this.ParameterTypeComponentService },
                            { "ParameterValue", this.ParameterValueService },
                            { "ParameterValueSet", this.ParameterValueSetService },
                            { "ParameterValueSetBase", this.ParameterValueSetBaseService },
                            { "ParametricConstraint", this.ParametricConstraintService },
                            { "Participant", this.ParticipantService },
                            { "ParticipantPermission", this.ParticipantPermissionService },
                            { "ParticipantRole", this.ParticipantRoleService },
                            { "Person", this.PersonService },
                            { "PersonPermission", this.PersonPermissionService },
                            { "PersonRole", this.PersonRoleService },
                            { "Point", this.PointService },
                            { "PossibleFiniteState", this.PossibleFiniteStateService },
                            { "PossibleFiniteStateList", this.PossibleFiniteStateListService },
                            { "PrefixedUnit", this.PrefixedUnitService },
                            { "Publication", this.PublicationService },
                            { "QuantityKind", this.QuantityKindService },
                            { "QuantityKindFactor", this.QuantityKindFactorService },
                            { "RatioScale", this.RatioScaleService },
                            { "ReferenceDataLibrary", this.ReferenceDataLibraryService },
                            { "ReferencerRule", this.ReferencerRuleService },
                            { "ReferenceSource", this.ReferenceSourceService },
                            { "RelationalExpression", this.RelationalExpressionService },
                            { "Relationship", this.RelationshipService },
                            { "RelationshipParameterValue", this.RelationshipParameterValueService },
                            { "RequestForDeviation", this.RequestForDeviationService },
                            { "RequestForWaiver", this.RequestForWaiverService },
                            { "Requirement", this.RequirementService },
                            { "RequirementsContainer", this.RequirementsContainerService },
                            { "RequirementsContainerParameterValue", this.RequirementsContainerParameterValueService },
                            { "RequirementsGroup", this.RequirementsGroupService },
                            { "RequirementsSpecification", this.RequirementsSpecificationService },
                            { "ReviewItemDiscrepancy", this.ReviewItemDiscrepancyService },
                            { "Rule", this.RuleService },
                            { "RuleVerification", this.RuleVerificationService },
                            { "RuleVerificationList", this.RuleVerificationListService },
                            { "RuleViolation", this.RuleViolationService },
                            { "SampledFunctionParameterType", this.SampledFunctionParameterTypeService },
                            { "ScalarParameterType", this.ScalarParameterTypeService },
                            { "ScaleReferenceQuantityValue", this.ScaleReferenceQuantityValueService },
                            { "ScaleValueDefinition", this.ScaleValueDefinitionService },
                            { "Section", this.SectionService },
                            { "SharedStyle", this.SharedStyleService },
                            { "SimpleParameterizableThing", this.SimpleParameterizableThingService },
                            { "SimpleParameterValue", this.SimpleParameterValueService },
                            { "SimpleQuantityKind", this.SimpleQuantityKindService },
                            { "SimpleUnit", this.SimpleUnitService },
                            { "SiteDirectory", this.SiteDirectoryService },
                            { "SiteDirectoryDataAnnotation", this.SiteDirectoryDataAnnotationService },
                            { "SiteDirectoryDataDiscussionItem", this.SiteDirectoryDataDiscussionItemService },
                            { "SiteDirectoryThingReference", this.SiteDirectoryThingReferenceService },
                            { "SiteLogEntry", this.SiteLogEntryService },
                            { "SiteReferenceDataLibrary", this.SiteReferenceDataLibraryService },
                            { "Solution", this.SolutionService },
                            { "SpecializedQuantityKind", this.SpecializedQuantityKindService },
                            { "Stakeholder", this.StakeholderService },
                            { "StakeholderValue", this.StakeholderValueService },
                            { "StakeHolderValueMap", this.StakeHolderValueMapService },
                            { "StakeHolderValueMapSettings", this.StakeHolderValueMapSettingsService },
                            { "TelephoneNumber", this.TelephoneNumberService },
                            { "Term", this.TermService },
                            { "TextParameterType", this.TextParameterTypeService },
                            { "TextualNote", this.TextualNoteService },
                            { "Thing", this.ThingService },
                            { "ThingReference", this.ThingReferenceService },
                            { "TimeOfDayParameterType", this.TimeOfDayParameterTypeService },
                            { "TopContainer", this.TopContainerService },
                            { "UnitFactor", this.UnitFactorService },
                            { "UnitPrefix", this.UnitPrefixService },
                            { "UserPreference", this.UserPreferenceService },
                            { "UserRuleVerification", this.UserRuleVerificationService },
                            { "ValueGroup", this.ValueGroupService },
                        };
                }

                return this.readServiceMap;
            }
        }

        /// <summary>
        /// Gets the persistable service map.
        /// </summary>
        private Dictionary<string, IPersistService> PersistableServiceMap
        {
            get
            {
                if (this.persistableServiceMap == null)
                {
                    this.persistableServiceMap = new Dictionary<string, IPersistService> 
                        {
                            { "ActionItem", this.ActionItemService },
                            { "ActualFiniteState", this.ActualFiniteStateService },
                            { "ActualFiniteStateList", this.ActualFiniteStateListService },
                            { "Alias", this.AliasService },
                            { "AndExpression", this.AndExpressionService },
                            { "Approval", this.ApprovalService },
                            { "ArrayParameterType", this.ArrayParameterTypeService },
                            { "BinaryNote", this.BinaryNoteService },
                            { "BinaryRelationship", this.BinaryRelationshipService },
                            { "BinaryRelationshipRule", this.BinaryRelationshipRuleService },
                            { "Book", this.BookService },
                            { "BooleanExpression", this.BooleanExpressionService },
                            { "BooleanParameterType", this.BooleanParameterTypeService },
                            { "Bounds", this.BoundsService },
                            { "BuiltInRuleVerification", this.BuiltInRuleVerificationService },
                            { "Category", this.CategoryService },
                            { "ChangeProposal", this.ChangeProposalService },
                            { "ChangeRequest", this.ChangeRequestService },
                            { "Citation", this.CitationService },
                            { "Color", this.ColorService },
                            { "CommonFileStore", this.CommonFileStoreService },
                            { "CompoundParameterType", this.CompoundParameterTypeService },
                            { "Constant", this.ConstantService },
                            { "ContractChangeNotice", this.ContractChangeNoticeService },
                            { "ContractDeviation", this.ContractDeviationService },
                            { "ConversionBasedUnit", this.ConversionBasedUnitService },
                            { "CyclicRatioScale", this.CyclicRatioScaleService },
                            { "DateParameterType", this.DateParameterTypeService },
                            { "DateTimeParameterType", this.DateTimeParameterTypeService },
                            { "DecompositionRule", this.DecompositionRuleService },
                            { "DefinedThing", this.DefinedThingService },
                            { "Definition", this.DefinitionService },
                            { "DependentParameterTypeAssignment", this.DependentParameterTypeAssignmentService },
                            { "DerivedQuantityKind", this.DerivedQuantityKindService },
                            { "DerivedUnit", this.DerivedUnitService },
                            { "DiagramCanvas", this.DiagramCanvasService },
                            { "DiagramEdge", this.DiagramEdgeService },
                            { "DiagramElementContainer", this.DiagramElementContainerService },
                            { "DiagramElementThing", this.DiagramElementThingService },
                            { "DiagrammingStyle", this.DiagrammingStyleService },
                            { "DiagramObject", this.DiagramObjectService },
                            { "DiagramShape", this.DiagramShapeService },
                            { "DiagramThingBase", this.DiagramThingBaseService },
                            { "DiscussionItem", this.DiscussionItemService },
                            { "DomainFileStore", this.DomainFileStoreService },
                            { "DomainOfExpertise", this.DomainOfExpertiseService },
                            { "DomainOfExpertiseGroup", this.DomainOfExpertiseGroupService },
                            { "ElementBase", this.ElementBaseService },
                            { "ElementDefinition", this.ElementDefinitionService },
                            { "ElementUsage", this.ElementUsageService },
                            { "EmailAddress", this.EmailAddressService },
                            { "EngineeringModel", this.EngineeringModelService },
                            { "EngineeringModelDataAnnotation", this.EngineeringModelDataAnnotationService },
                            { "EngineeringModelDataDiscussionItem", this.EngineeringModelDataDiscussionItemService },
                            { "EngineeringModelDataNote", this.EngineeringModelDataNoteService },
                            { "EngineeringModelSetup", this.EngineeringModelSetupService },
                            { "EnumerationParameterType", this.EnumerationParameterTypeService },
                            { "EnumerationValueDefinition", this.EnumerationValueDefinitionService },
                            { "ExclusiveOrExpression", this.ExclusiveOrExpressionService },
                            { "ExternalIdentifierMap", this.ExternalIdentifierMapService },
                            { "File", this.FileService },
                            { "FileRevision", this.FileRevisionService },
                            { "FileStore", this.FileStoreService },
                            { "FileType", this.FileTypeService },
                            { "Folder", this.FolderService },
                            { "GenericAnnotation", this.GenericAnnotationService },
                            { "Glossary", this.GlossaryService },
                            { "Goal", this.GoalService },
                            { "HyperLink", this.HyperLinkService },
                            { "IdCorrespondence", this.IdCorrespondenceService },
                            { "IndependentParameterTypeAssignment", this.IndependentParameterTypeAssignmentService },
                            { "IntervalScale", this.IntervalScaleService },
                            { "Iteration", this.IterationService },
                            { "IterationSetup", this.IterationSetupService },
                            { "LinearConversionUnit", this.LinearConversionUnitService },
                            { "LogarithmicScale", this.LogarithmicScaleService },
                            { "LogEntryChangelogItem", this.LogEntryChangelogItemService },
                            { "MappingToReferenceScale", this.MappingToReferenceScaleService },
                            { "MeasurementScale", this.MeasurementScaleService },
                            { "MeasurementUnit", this.MeasurementUnitService },
                            { "ModellingAnnotationItem", this.ModellingAnnotationItemService },
                            { "ModellingThingReference", this.ModellingThingReferenceService },
                            { "ModelLogEntry", this.ModelLogEntryService },
                            { "ModelReferenceDataLibrary", this.ModelReferenceDataLibraryService },
                            { "MultiRelationship", this.MultiRelationshipService },
                            { "MultiRelationshipRule", this.MultiRelationshipRuleService },
                            { "NaturalLanguage", this.NaturalLanguageService },
                            { "NestedElement", this.NestedElementService },
                            { "NestedParameter", this.NestedParameterService },
                            { "Note", this.NoteService },
                            { "NotExpression", this.NotExpressionService },
                            { "Option", this.OptionService },
                            { "OrdinalScale", this.OrdinalScaleService },
                            { "OrExpression", this.OrExpressionService },
                            { "Organization", this.OrganizationService },
                            { "OrganizationalParticipant", this.OrganizationalParticipantService },
                            { "OwnedStyle", this.OwnedStyleService },
                            { "Page", this.PageService },
                            { "Parameter", this.ParameterService },
                            { "ParameterBase", this.ParameterBaseService },
                            { "ParameterGroup", this.ParameterGroupService },
                            { "ParameterizedCategoryRule", this.ParameterizedCategoryRuleService },
                            { "ParameterOrOverrideBase", this.ParameterOrOverrideBaseService },
                            { "ParameterOverride", this.ParameterOverrideService },
                            { "ParameterOverrideValueSet", this.ParameterOverrideValueSetService },
                            { "ParameterSubscription", this.ParameterSubscriptionService },
                            { "ParameterSubscriptionValueSet", this.ParameterSubscriptionValueSetService },
                            { "ParameterType", this.ParameterTypeService },
                            { "ParameterTypeComponent", this.ParameterTypeComponentService },
                            { "ParameterValue", this.ParameterValueService },
                            { "ParameterValueSet", this.ParameterValueSetService },
                            { "ParameterValueSetBase", this.ParameterValueSetBaseService },
                            { "ParametricConstraint", this.ParametricConstraintService },
                            { "Participant", this.ParticipantService },
                            { "ParticipantPermission", this.ParticipantPermissionService },
                            { "ParticipantRole", this.ParticipantRoleService },
                            { "Person", this.PersonService },
                            { "PersonPermission", this.PersonPermissionService },
                            { "PersonRole", this.PersonRoleService },
                            { "Point", this.PointService },
                            { "PossibleFiniteState", this.PossibleFiniteStateService },
                            { "PossibleFiniteStateList", this.PossibleFiniteStateListService },
                            { "PrefixedUnit", this.PrefixedUnitService },
                            { "Publication", this.PublicationService },
                            { "QuantityKind", this.QuantityKindService },
                            { "QuantityKindFactor", this.QuantityKindFactorService },
                            { "RatioScale", this.RatioScaleService },
                            { "ReferenceDataLibrary", this.ReferenceDataLibraryService },
                            { "ReferencerRule", this.ReferencerRuleService },
                            { "ReferenceSource", this.ReferenceSourceService },
                            { "RelationalExpression", this.RelationalExpressionService },
                            { "Relationship", this.RelationshipService },
                            { "RelationshipParameterValue", this.RelationshipParameterValueService },
                            { "RequestForDeviation", this.RequestForDeviationService },
                            { "RequestForWaiver", this.RequestForWaiverService },
                            { "Requirement", this.RequirementService },
                            { "RequirementsContainer", this.RequirementsContainerService },
                            { "RequirementsContainerParameterValue", this.RequirementsContainerParameterValueService },
                            { "RequirementsGroup", this.RequirementsGroupService },
                            { "RequirementsSpecification", this.RequirementsSpecificationService },
                            { "ReviewItemDiscrepancy", this.ReviewItemDiscrepancyService },
                            { "Rule", this.RuleService },
                            { "RuleVerification", this.RuleVerificationService },
                            { "RuleVerificationList", this.RuleVerificationListService },
                            { "RuleViolation", this.RuleViolationService },
                            { "SampledFunctionParameterType", this.SampledFunctionParameterTypeService },
                            { "ScalarParameterType", this.ScalarParameterTypeService },
                            { "ScaleReferenceQuantityValue", this.ScaleReferenceQuantityValueService },
                            { "ScaleValueDefinition", this.ScaleValueDefinitionService },
                            { "Section", this.SectionService },
                            { "SharedStyle", this.SharedStyleService },
                            { "SimpleParameterizableThing", this.SimpleParameterizableThingService },
                            { "SimpleParameterValue", this.SimpleParameterValueService },
                            { "SimpleQuantityKind", this.SimpleQuantityKindService },
                            { "SimpleUnit", this.SimpleUnitService },
                            { "SiteDirectory", this.SiteDirectoryService },
                            { "SiteDirectoryDataAnnotation", this.SiteDirectoryDataAnnotationService },
                            { "SiteDirectoryDataDiscussionItem", this.SiteDirectoryDataDiscussionItemService },
                            { "SiteDirectoryThingReference", this.SiteDirectoryThingReferenceService },
                            { "SiteLogEntry", this.SiteLogEntryService },
                            { "SiteReferenceDataLibrary", this.SiteReferenceDataLibraryService },
                            { "Solution", this.SolutionService },
                            { "SpecializedQuantityKind", this.SpecializedQuantityKindService },
                            { "Stakeholder", this.StakeholderService },
                            { "StakeholderValue", this.StakeholderValueService },
                            { "StakeHolderValueMap", this.StakeHolderValueMapService },
                            { "StakeHolderValueMapSettings", this.StakeHolderValueMapSettingsService },
                            { "TelephoneNumber", this.TelephoneNumberService },
                            { "Term", this.TermService },
                            { "TextParameterType", this.TextParameterTypeService },
                            { "TextualNote", this.TextualNoteService },
                            { "Thing", this.ThingService },
                            { "ThingReference", this.ThingReferenceService },
                            { "TimeOfDayParameterType", this.TimeOfDayParameterTypeService },
                            { "TopContainer", this.TopContainerService },
                            { "UnitFactor", this.UnitFactorService },
                            { "UnitPrefix", this.UnitPrefixService },
                            { "UserPreference", this.UserPreferenceService },
                            { "UserRuleVerification", this.UserRuleVerificationService },
                            { "ValueGroup", this.ValueGroupService },
                        };
                }

                return this.persistableServiceMap;
            }
        }

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public IReadService MapToReadService(string typeName)
        {
            if (!this.ReadServiceMap.ContainsKey(typeName))
            {
                throw new TypeLoadException(string.Format("Type not supported {0}", typeName));
            }

            return this.ReadServiceMap[typeName];
        }

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public TService MapToReadService<TService>(string typeName) where TService : IReadService
        {
            return (TService)this.MapToReadService(typeName);
        }

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public IReadService MapToReadService(Thing thing)
        {
            var typeName = thing.GetType().Name;
            return this.MapToReadService(typeName);
        }

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A read service instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public TService MapToReadService<TService>(Thing thing) where TService : IReadService
        {
            return (TService)this.MapToReadService(thing);
        }

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public IPersistService MapToPersitableService(string typeName)
        {
            if (!this.PersistableServiceMap.ContainsKey(typeName))
            {
                throw new TypeLoadException(string.Format("Type not supported {0}", typeName));
            }

            return this.PersistableServiceMap[typeName];
        }

        /// <summary>
        /// Returns a service instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public TService MapToPersitableService<TService>(string typeName) where TService : IPersistService
        {
            return (TService)this.MapToPersitableService(typeName);
        }

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public IPersistService MapToPersitableService(Thing thing)
        {
            var typeName = thing.GetType().Name;
            return this.MapToPersitableService(typeName);
        }

        /// <summary>
        /// Returns a service instance based on the passed in instance.
        /// </summary>
        /// <param name="thing">
        /// The thing instance for which to retrieve a service.
        /// </param>
        /// <returns>
        /// A service instance with provides persistence functionalities.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported.
        /// </exception>
        public TService MapToPersitableService<TService>(Thing thing) where TService : IPersistService
        {
            return (TService)this.MapToPersitableService(thing);
        }
    }
}
