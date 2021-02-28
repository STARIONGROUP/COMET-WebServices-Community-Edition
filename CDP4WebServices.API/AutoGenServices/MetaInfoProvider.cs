// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaInfoProvider.cs" company="RHEA System S.A.">
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
    using CDP4Common.MetaInfo;

    /// <summary>
    /// A meta info registry class that allows retrieval of meta info instances by type name.
    /// </summary>
    public class MetaInfoProvider : MetaInfoProviderBase, IMetaInfoProvider
    {
        /// <summary>
        /// The type to meta info instance map.
        /// </summary>
        private Dictionary<string, IMetaInfo> metaInfoMap;

        /// <summary>
        /// Gets or sets the actionItem meta info.
        /// </summary>
        public IActionItemMetaInfo ActionItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the actualFiniteState meta info.
        /// </summary>
        public IActualFiniteStateMetaInfo ActualFiniteStateMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the actualFiniteStateList meta info.
        /// </summary>
        public IActualFiniteStateListMetaInfo ActualFiniteStateListMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the alias meta info.
        /// </summary>
        public IAliasMetaInfo AliasMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the andExpression meta info.
        /// </summary>
        public IAndExpressionMetaInfo AndExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the approval meta info.
        /// </summary>
        public IApprovalMetaInfo ApprovalMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the arrayParameterType meta info.
        /// </summary>
        public IArrayParameterTypeMetaInfo ArrayParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the binaryNote meta info.
        /// </summary>
        public IBinaryNoteMetaInfo BinaryNoteMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the binaryRelationship meta info.
        /// </summary>
        public IBinaryRelationshipMetaInfo BinaryRelationshipMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the binaryRelationshipRule meta info.
        /// </summary>
        public IBinaryRelationshipRuleMetaInfo BinaryRelationshipRuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the book meta info.
        /// </summary>
        public IBookMetaInfo BookMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the booleanExpression meta info.
        /// </summary>
        public IBooleanExpressionMetaInfo BooleanExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the booleanParameterType meta info.
        /// </summary>
        public IBooleanParameterTypeMetaInfo BooleanParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the bounds meta info.
        /// </summary>
        public IBoundsMetaInfo BoundsMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the builtInRuleVerification meta info.
        /// </summary>
        public IBuiltInRuleVerificationMetaInfo BuiltInRuleVerificationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the category meta info.
        /// </summary>
        public ICategoryMetaInfo CategoryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the changeProposal meta info.
        /// </summary>
        public IChangeProposalMetaInfo ChangeProposalMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the changeRequest meta info.
        /// </summary>
        public IChangeRequestMetaInfo ChangeRequestMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the citation meta info.
        /// </summary>
        public ICitationMetaInfo CitationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the color meta info.
        /// </summary>
        public IColorMetaInfo ColorMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the commonFileStore meta info.
        /// </summary>
        public ICommonFileStoreMetaInfo CommonFileStoreMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the compoundParameterType meta info.
        /// </summary>
        public ICompoundParameterTypeMetaInfo CompoundParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the constant meta info.
        /// </summary>
        public IConstantMetaInfo ConstantMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the contractChangeNotice meta info.
        /// </summary>
        public IContractChangeNoticeMetaInfo ContractChangeNoticeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the contractDeviation meta info.
        /// </summary>
        public IContractDeviationMetaInfo ContractDeviationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the conversionBasedUnit meta info.
        /// </summary>
        public IConversionBasedUnitMetaInfo ConversionBasedUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the cyclicRatioScale meta info.
        /// </summary>
        public ICyclicRatioScaleMetaInfo CyclicRatioScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the dateParameterType meta info.
        /// </summary>
        public IDateParameterTypeMetaInfo DateParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the dateTimeParameterType meta info.
        /// </summary>
        public IDateTimeParameterTypeMetaInfo DateTimeParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the decompositionRule meta info.
        /// </summary>
        public IDecompositionRuleMetaInfo DecompositionRuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the definedThing meta info.
        /// </summary>
        public IDefinedThingMetaInfo DefinedThingMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the definition meta info.
        /// </summary>
        public IDefinitionMetaInfo DefinitionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the dependentParameterTypeAssignment meta info.
        /// </summary>
        public IDependentParameterTypeAssignmentMetaInfo DependentParameterTypeAssignmentMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the derivedQuantityKind meta info.
        /// </summary>
        public IDerivedQuantityKindMetaInfo DerivedQuantityKindMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the derivedUnit meta info.
        /// </summary>
        public IDerivedUnitMetaInfo DerivedUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramCanvas meta info.
        /// </summary>
        public IDiagramCanvasMetaInfo DiagramCanvasMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramEdge meta info.
        /// </summary>
        public IDiagramEdgeMetaInfo DiagramEdgeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramElementContainer meta info.
        /// </summary>
        public IDiagramElementContainerMetaInfo DiagramElementContainerMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramElementThing meta info.
        /// </summary>
        public IDiagramElementThingMetaInfo DiagramElementThingMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagrammingStyle meta info.
        /// </summary>
        public IDiagrammingStyleMetaInfo DiagrammingStyleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramObject meta info.
        /// </summary>
        public IDiagramObjectMetaInfo DiagramObjectMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramShape meta info.
        /// </summary>
        public IDiagramShapeMetaInfo DiagramShapeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the diagramThingBase meta info.
        /// </summary>
        public IDiagramThingBaseMetaInfo DiagramThingBaseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the discussionItem meta info.
        /// </summary>
        public IDiscussionItemMetaInfo DiscussionItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the domainFileStore meta info.
        /// </summary>
        public IDomainFileStoreMetaInfo DomainFileStoreMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the domainOfExpertise meta info.
        /// </summary>
        public IDomainOfExpertiseMetaInfo DomainOfExpertiseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the domainOfExpertiseGroup meta info.
        /// </summary>
        public IDomainOfExpertiseGroupMetaInfo DomainOfExpertiseGroupMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the elementBase meta info.
        /// </summary>
        public IElementBaseMetaInfo ElementBaseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the elementDefinition meta info.
        /// </summary>
        public IElementDefinitionMetaInfo ElementDefinitionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the elementUsage meta info.
        /// </summary>
        public IElementUsageMetaInfo ElementUsageMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the emailAddress meta info.
        /// </summary>
        public IEmailAddressMetaInfo EmailAddressMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModel meta info.
        /// </summary>
        public IEngineeringModelMetaInfo EngineeringModelMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataAnnotation meta info.
        /// </summary>
        public IEngineeringModelDataAnnotationMetaInfo EngineeringModelDataAnnotationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataDiscussionItem meta info.
        /// </summary>
        public IEngineeringModelDataDiscussionItemMetaInfo EngineeringModelDataDiscussionItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelDataNote meta info.
        /// </summary>
        public IEngineeringModelDataNoteMetaInfo EngineeringModelDataNoteMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the engineeringModelSetup meta info.
        /// </summary>
        public IEngineeringModelSetupMetaInfo EngineeringModelSetupMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the enumerationParameterType meta info.
        /// </summary>
        public IEnumerationParameterTypeMetaInfo EnumerationParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the enumerationValueDefinition meta info.
        /// </summary>
        public IEnumerationValueDefinitionMetaInfo EnumerationValueDefinitionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the exclusiveOrExpression meta info.
        /// </summary>
        public IExclusiveOrExpressionMetaInfo ExclusiveOrExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the externalIdentifierMap meta info.
        /// </summary>
        public IExternalIdentifierMapMetaInfo ExternalIdentifierMapMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the file meta info.
        /// </summary>
        public IFileMetaInfo FileMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the fileRevision meta info.
        /// </summary>
        public IFileRevisionMetaInfo FileRevisionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the fileStore meta info.
        /// </summary>
        public IFileStoreMetaInfo FileStoreMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the fileType meta info.
        /// </summary>
        public IFileTypeMetaInfo FileTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the folder meta info.
        /// </summary>
        public IFolderMetaInfo FolderMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the genericAnnotation meta info.
        /// </summary>
        public IGenericAnnotationMetaInfo GenericAnnotationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the glossary meta info.
        /// </summary>
        public IGlossaryMetaInfo GlossaryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the goal meta info.
        /// </summary>
        public IGoalMetaInfo GoalMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the hyperLink meta info.
        /// </summary>
        public IHyperLinkMetaInfo HyperLinkMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the idCorrespondence meta info.
        /// </summary>
        public IIdCorrespondenceMetaInfo IdCorrespondenceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the independentParameterTypeAssignment meta info.
        /// </summary>
        public IIndependentParameterTypeAssignmentMetaInfo IndependentParameterTypeAssignmentMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the intervalScale meta info.
        /// </summary>
        public IIntervalScaleMetaInfo IntervalScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the iteration meta info.
        /// </summary>
        public IIterationMetaInfo IterationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the iterationSetup meta info.
        /// </summary>
        public IIterationSetupMetaInfo IterationSetupMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the linearConversionUnit meta info.
        /// </summary>
        public ILinearConversionUnitMetaInfo LinearConversionUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the logarithmicScale meta info.
        /// </summary>
        public ILogarithmicScaleMetaInfo LogarithmicScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the logEntryChangelogItem meta info.
        /// </summary>
        public ILogEntryChangelogItemMetaInfo LogEntryChangelogItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the mappingToReferenceScale meta info.
        /// </summary>
        public IMappingToReferenceScaleMetaInfo MappingToReferenceScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the measurementScale meta info.
        /// </summary>
        public IMeasurementScaleMetaInfo MeasurementScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the measurementUnit meta info.
        /// </summary>
        public IMeasurementUnitMetaInfo MeasurementUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the modellingAnnotationItem meta info.
        /// </summary>
        public IModellingAnnotationItemMetaInfo ModellingAnnotationItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the modellingThingReference meta info.
        /// </summary>
        public IModellingThingReferenceMetaInfo ModellingThingReferenceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the modelLogEntry meta info.
        /// </summary>
        public IModelLogEntryMetaInfo ModelLogEntryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the modelReferenceDataLibrary meta info.
        /// </summary>
        public IModelReferenceDataLibraryMetaInfo ModelReferenceDataLibraryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the multiRelationship meta info.
        /// </summary>
        public IMultiRelationshipMetaInfo MultiRelationshipMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the multiRelationshipRule meta info.
        /// </summary>
        public IMultiRelationshipRuleMetaInfo MultiRelationshipRuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the naturalLanguage meta info.
        /// </summary>
        public INaturalLanguageMetaInfo NaturalLanguageMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the nestedElement meta info.
        /// </summary>
        public INestedElementMetaInfo NestedElementMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the nestedParameter meta info.
        /// </summary>
        public INestedParameterMetaInfo NestedParameterMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the note meta info.
        /// </summary>
        public INoteMetaInfo NoteMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the notExpression meta info.
        /// </summary>
        public INotExpressionMetaInfo NotExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the option meta info.
        /// </summary>
        public IOptionMetaInfo OptionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ordinalScale meta info.
        /// </summary>
        public IOrdinalScaleMetaInfo OrdinalScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the orExpression meta info.
        /// </summary>
        public IOrExpressionMetaInfo OrExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the organization meta info.
        /// </summary>
        public IOrganizationMetaInfo OrganizationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the organizationalParticipant meta info.
        /// </summary>
        public IOrganizationalParticipantMetaInfo OrganizationalParticipantMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ownedStyle meta info.
        /// </summary>
        public IOwnedStyleMetaInfo OwnedStyleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the page meta info.
        /// </summary>
        public IPageMetaInfo PageMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameter meta info.
        /// </summary>
        public IParameterMetaInfo ParameterMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterBase meta info.
        /// </summary>
        public IParameterBaseMetaInfo ParameterBaseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterGroup meta info.
        /// </summary>
        public IParameterGroupMetaInfo ParameterGroupMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterizedCategoryRule meta info.
        /// </summary>
        public IParameterizedCategoryRuleMetaInfo ParameterizedCategoryRuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterOrOverrideBase meta info.
        /// </summary>
        public IParameterOrOverrideBaseMetaInfo ParameterOrOverrideBaseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterOverride meta info.
        /// </summary>
        public IParameterOverrideMetaInfo ParameterOverrideMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterOverrideValueSet meta info.
        /// </summary>
        public IParameterOverrideValueSetMetaInfo ParameterOverrideValueSetMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterSubscription meta info.
        /// </summary>
        public IParameterSubscriptionMetaInfo ParameterSubscriptionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterSubscriptionValueSet meta info.
        /// </summary>
        public IParameterSubscriptionValueSetMetaInfo ParameterSubscriptionValueSetMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterType meta info.
        /// </summary>
        public IParameterTypeMetaInfo ParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterTypeComponent meta info.
        /// </summary>
        public IParameterTypeComponentMetaInfo ParameterTypeComponentMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterValue meta info.
        /// </summary>
        public IParameterValueMetaInfo ParameterValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterValueSet meta info.
        /// </summary>
        public IParameterValueSetMetaInfo ParameterValueSetMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parameterValueSetBase meta info.
        /// </summary>
        public IParameterValueSetBaseMetaInfo ParameterValueSetBaseMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the parametricConstraint meta info.
        /// </summary>
        public IParametricConstraintMetaInfo ParametricConstraintMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the participant meta info.
        /// </summary>
        public IParticipantMetaInfo ParticipantMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the participantPermission meta info.
        /// </summary>
        public IParticipantPermissionMetaInfo ParticipantPermissionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the participantRole meta info.
        /// </summary>
        public IParticipantRoleMetaInfo ParticipantRoleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the person meta info.
        /// </summary>
        public IPersonMetaInfo PersonMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the personPermission meta info.
        /// </summary>
        public IPersonPermissionMetaInfo PersonPermissionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the personRole meta info.
        /// </summary>
        public IPersonRoleMetaInfo PersonRoleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the point meta info.
        /// </summary>
        public IPointMetaInfo PointMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the possibleFiniteState meta info.
        /// </summary>
        public IPossibleFiniteStateMetaInfo PossibleFiniteStateMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the possibleFiniteStateList meta info.
        /// </summary>
        public IPossibleFiniteStateListMetaInfo PossibleFiniteStateListMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the prefixedUnit meta info.
        /// </summary>
        public IPrefixedUnitMetaInfo PrefixedUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the publication meta info.
        /// </summary>
        public IPublicationMetaInfo PublicationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the quantityKind meta info.
        /// </summary>
        public IQuantityKindMetaInfo QuantityKindMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the quantityKindFactor meta info.
        /// </summary>
        public IQuantityKindFactorMetaInfo QuantityKindFactorMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ratioScale meta info.
        /// </summary>
        public IRatioScaleMetaInfo RatioScaleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the referenceDataLibrary meta info.
        /// </summary>
        public IReferenceDataLibraryMetaInfo ReferenceDataLibraryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the referencerRule meta info.
        /// </summary>
        public IReferencerRuleMetaInfo ReferencerRuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the referenceSource meta info.
        /// </summary>
        public IReferenceSourceMetaInfo ReferenceSourceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the relationalExpression meta info.
        /// </summary>
        public IRelationalExpressionMetaInfo RelationalExpressionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the relationship meta info.
        /// </summary>
        public IRelationshipMetaInfo RelationshipMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the relationshipParameterValue meta info.
        /// </summary>
        public IRelationshipParameterValueMetaInfo RelationshipParameterValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requestForDeviation meta info.
        /// </summary>
        public IRequestForDeviationMetaInfo RequestForDeviationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requestForWaiver meta info.
        /// </summary>
        public IRequestForWaiverMetaInfo RequestForWaiverMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requirement meta info.
        /// </summary>
        public IRequirementMetaInfo RequirementMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requirementsContainer meta info.
        /// </summary>
        public IRequirementsContainerMetaInfo RequirementsContainerMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requirementsContainerParameterValue meta info.
        /// </summary>
        public IRequirementsContainerParameterValueMetaInfo RequirementsContainerParameterValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requirementsGroup meta info.
        /// </summary>
        public IRequirementsGroupMetaInfo RequirementsGroupMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the requirementsSpecification meta info.
        /// </summary>
        public IRequirementsSpecificationMetaInfo RequirementsSpecificationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the reviewItemDiscrepancy meta info.
        /// </summary>
        public IReviewItemDiscrepancyMetaInfo ReviewItemDiscrepancyMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the rule meta info.
        /// </summary>
        public IRuleMetaInfo RuleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ruleVerification meta info.
        /// </summary>
        public IRuleVerificationMetaInfo RuleVerificationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ruleVerificationList meta info.
        /// </summary>
        public IRuleVerificationListMetaInfo RuleVerificationListMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the ruleViolation meta info.
        /// </summary>
        public IRuleViolationMetaInfo RuleViolationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the sampledFunctionParameterType meta info.
        /// </summary>
        public ISampledFunctionParameterTypeMetaInfo SampledFunctionParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the scalarParameterType meta info.
        /// </summary>
        public IScalarParameterTypeMetaInfo ScalarParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the scaleReferenceQuantityValue meta info.
        /// </summary>
        public IScaleReferenceQuantityValueMetaInfo ScaleReferenceQuantityValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the scaleValueDefinition meta info.
        /// </summary>
        public IScaleValueDefinitionMetaInfo ScaleValueDefinitionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the section meta info.
        /// </summary>
        public ISectionMetaInfo SectionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the sharedStyle meta info.
        /// </summary>
        public ISharedStyleMetaInfo SharedStyleMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the simpleParameterizableThing meta info.
        /// </summary>
        public ISimpleParameterizableThingMetaInfo SimpleParameterizableThingMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the simpleParameterValue meta info.
        /// </summary>
        public ISimpleParameterValueMetaInfo SimpleParameterValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the simpleQuantityKind meta info.
        /// </summary>
        public ISimpleQuantityKindMetaInfo SimpleQuantityKindMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the simpleUnit meta info.
        /// </summary>
        public ISimpleUnitMetaInfo SimpleUnitMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectory meta info.
        /// </summary>
        public ISiteDirectoryMetaInfo SiteDirectoryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryDataAnnotation meta info.
        /// </summary>
        public ISiteDirectoryDataAnnotationMetaInfo SiteDirectoryDataAnnotationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryDataDiscussionItem meta info.
        /// </summary>
        public ISiteDirectoryDataDiscussionItemMetaInfo SiteDirectoryDataDiscussionItemMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteDirectoryThingReference meta info.
        /// </summary>
        public ISiteDirectoryThingReferenceMetaInfo SiteDirectoryThingReferenceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteLogEntry meta info.
        /// </summary>
        public ISiteLogEntryMetaInfo SiteLogEntryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the siteReferenceDataLibrary meta info.
        /// </summary>
        public ISiteReferenceDataLibraryMetaInfo SiteReferenceDataLibraryMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the solution meta info.
        /// </summary>
        public ISolutionMetaInfo SolutionMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the specializedQuantityKind meta info.
        /// </summary>
        public ISpecializedQuantityKindMetaInfo SpecializedQuantityKindMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the stakeholder meta info.
        /// </summary>
        public IStakeholderMetaInfo StakeholderMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the stakeholderValue meta info.
        /// </summary>
        public IStakeholderValueMetaInfo StakeholderValueMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the stakeHolderValueMap meta info.
        /// </summary>
        public IStakeHolderValueMapMetaInfo StakeHolderValueMapMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the stakeHolderValueMapSettings meta info.
        /// </summary>
        public IStakeHolderValueMapSettingsMetaInfo StakeHolderValueMapSettingsMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the telephoneNumber meta info.
        /// </summary>
        public ITelephoneNumberMetaInfo TelephoneNumberMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the term meta info.
        /// </summary>
        public ITermMetaInfo TermMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the textParameterType meta info.
        /// </summary>
        public ITextParameterTypeMetaInfo TextParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the textualNote meta info.
        /// </summary>
        public ITextualNoteMetaInfo TextualNoteMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the thing meta info.
        /// </summary>
        public IThingMetaInfo ThingMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the thingReference meta info.
        /// </summary>
        public IThingReferenceMetaInfo ThingReferenceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the timeOfDayParameterType meta info.
        /// </summary>
        public ITimeOfDayParameterTypeMetaInfo TimeOfDayParameterTypeMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the topContainer meta info.
        /// </summary>
        public ITopContainerMetaInfo TopContainerMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the unitFactor meta info.
        /// </summary>
        public IUnitFactorMetaInfo UnitFactorMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the unitPrefix meta info.
        /// </summary>
        public IUnitPrefixMetaInfo UnitPrefixMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the userPreference meta info.
        /// </summary>
        public IUserPreferenceMetaInfo UserPreferenceMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the userRuleVerification meta info.
        /// </summary>
        public IUserRuleVerificationMetaInfo UserRuleVerificationMetaInfo { get; set; }

        /// <summary>
        /// Gets or sets the valueGroup meta info.
        /// </summary>
        public IValueGroupMetaInfo ValueGroupMetaInfo { get; set; }
   
        /// <summary>
        /// Gets the meta info map.
        /// </summary>
        private Dictionary<string, IMetaInfo> MetaInfoMap
        {
            get
            {
                if (this.metaInfoMap == null)
                {
                    this.metaInfoMap = new Dictionary<string, IMetaInfo> 
                        {
                            { "ActionItem", this.ActionItemMetaInfo },
                            { "ActualFiniteState", this.ActualFiniteStateMetaInfo },
                            { "ActualFiniteStateList", this.ActualFiniteStateListMetaInfo },
                            { "Alias", this.AliasMetaInfo },
                            { "AndExpression", this.AndExpressionMetaInfo },
                            { "Approval", this.ApprovalMetaInfo },
                            { "ArrayParameterType", this.ArrayParameterTypeMetaInfo },
                            { "BinaryNote", this.BinaryNoteMetaInfo },
                            { "BinaryRelationship", this.BinaryRelationshipMetaInfo },
                            { "BinaryRelationshipRule", this.BinaryRelationshipRuleMetaInfo },
                            { "Book", this.BookMetaInfo },
                            { "BooleanExpression", this.BooleanExpressionMetaInfo },
                            { "BooleanParameterType", this.BooleanParameterTypeMetaInfo },
                            { "Bounds", this.BoundsMetaInfo },
                            { "BuiltInRuleVerification", this.BuiltInRuleVerificationMetaInfo },
                            { "Category", this.CategoryMetaInfo },
                            { "ChangeProposal", this.ChangeProposalMetaInfo },
                            { "ChangeRequest", this.ChangeRequestMetaInfo },
                            { "Citation", this.CitationMetaInfo },
                            { "Color", this.ColorMetaInfo },
                            { "CommonFileStore", this.CommonFileStoreMetaInfo },
                            { "CompoundParameterType", this.CompoundParameterTypeMetaInfo },
                            { "Constant", this.ConstantMetaInfo },
                            { "ContractChangeNotice", this.ContractChangeNoticeMetaInfo },
                            { "ContractDeviation", this.ContractDeviationMetaInfo },
                            { "ConversionBasedUnit", this.ConversionBasedUnitMetaInfo },
                            { "CyclicRatioScale", this.CyclicRatioScaleMetaInfo },
                            { "DateParameterType", this.DateParameterTypeMetaInfo },
                            { "DateTimeParameterType", this.DateTimeParameterTypeMetaInfo },
                            { "DecompositionRule", this.DecompositionRuleMetaInfo },
                            { "DefinedThing", this.DefinedThingMetaInfo },
                            { "Definition", this.DefinitionMetaInfo },
                            { "DependentParameterTypeAssignment", this.DependentParameterTypeAssignmentMetaInfo },
                            { "DerivedQuantityKind", this.DerivedQuantityKindMetaInfo },
                            { "DerivedUnit", this.DerivedUnitMetaInfo },
                            { "DiagramCanvas", this.DiagramCanvasMetaInfo },
                            { "DiagramEdge", this.DiagramEdgeMetaInfo },
                            { "DiagramElementContainer", this.DiagramElementContainerMetaInfo },
                            { "DiagramElementThing", this.DiagramElementThingMetaInfo },
                            { "DiagrammingStyle", this.DiagrammingStyleMetaInfo },
                            { "DiagramObject", this.DiagramObjectMetaInfo },
                            { "DiagramShape", this.DiagramShapeMetaInfo },
                            { "DiagramThingBase", this.DiagramThingBaseMetaInfo },
                            { "DiscussionItem", this.DiscussionItemMetaInfo },
                            { "DomainFileStore", this.DomainFileStoreMetaInfo },
                            { "DomainOfExpertise", this.DomainOfExpertiseMetaInfo },
                            { "DomainOfExpertiseGroup", this.DomainOfExpertiseGroupMetaInfo },
                            { "ElementBase", this.ElementBaseMetaInfo },
                            { "ElementDefinition", this.ElementDefinitionMetaInfo },
                            { "ElementUsage", this.ElementUsageMetaInfo },
                            { "EmailAddress", this.EmailAddressMetaInfo },
                            { "EngineeringModel", this.EngineeringModelMetaInfo },
                            { "EngineeringModelDataAnnotation", this.EngineeringModelDataAnnotationMetaInfo },
                            { "EngineeringModelDataDiscussionItem", this.EngineeringModelDataDiscussionItemMetaInfo },
                            { "EngineeringModelDataNote", this.EngineeringModelDataNoteMetaInfo },
                            { "EngineeringModelSetup", this.EngineeringModelSetupMetaInfo },
                            { "EnumerationParameterType", this.EnumerationParameterTypeMetaInfo },
                            { "EnumerationValueDefinition", this.EnumerationValueDefinitionMetaInfo },
                            { "ExclusiveOrExpression", this.ExclusiveOrExpressionMetaInfo },
                            { "ExternalIdentifierMap", this.ExternalIdentifierMapMetaInfo },
                            { "File", this.FileMetaInfo },
                            { "FileRevision", this.FileRevisionMetaInfo },
                            { "FileStore", this.FileStoreMetaInfo },
                            { "FileType", this.FileTypeMetaInfo },
                            { "Folder", this.FolderMetaInfo },
                            { "GenericAnnotation", this.GenericAnnotationMetaInfo },
                            { "Glossary", this.GlossaryMetaInfo },
                            { "Goal", this.GoalMetaInfo },
                            { "HyperLink", this.HyperLinkMetaInfo },
                            { "IdCorrespondence", this.IdCorrespondenceMetaInfo },
                            { "IndependentParameterTypeAssignment", this.IndependentParameterTypeAssignmentMetaInfo },
                            { "IntervalScale", this.IntervalScaleMetaInfo },
                            { "Iteration", this.IterationMetaInfo },
                            { "IterationSetup", this.IterationSetupMetaInfo },
                            { "LinearConversionUnit", this.LinearConversionUnitMetaInfo },
                            { "LogarithmicScale", this.LogarithmicScaleMetaInfo },
                            { "LogEntryChangelogItem", this.LogEntryChangelogItemMetaInfo },
                            { "MappingToReferenceScale", this.MappingToReferenceScaleMetaInfo },
                            { "MeasurementScale", this.MeasurementScaleMetaInfo },
                            { "MeasurementUnit", this.MeasurementUnitMetaInfo },
                            { "ModellingAnnotationItem", this.ModellingAnnotationItemMetaInfo },
                            { "ModellingThingReference", this.ModellingThingReferenceMetaInfo },
                            { "ModelLogEntry", this.ModelLogEntryMetaInfo },
                            { "ModelReferenceDataLibrary", this.ModelReferenceDataLibraryMetaInfo },
                            { "MultiRelationship", this.MultiRelationshipMetaInfo },
                            { "MultiRelationshipRule", this.MultiRelationshipRuleMetaInfo },
                            { "NaturalLanguage", this.NaturalLanguageMetaInfo },
                            { "NestedElement", this.NestedElementMetaInfo },
                            { "NestedParameter", this.NestedParameterMetaInfo },
                            { "Note", this.NoteMetaInfo },
                            { "NotExpression", this.NotExpressionMetaInfo },
                            { "Option", this.OptionMetaInfo },
                            { "OrdinalScale", this.OrdinalScaleMetaInfo },
                            { "OrExpression", this.OrExpressionMetaInfo },
                            { "Organization", this.OrganizationMetaInfo },
                            { "OrganizationalParticipant", this.OrganizationalParticipantMetaInfo },
                            { "OwnedStyle", this.OwnedStyleMetaInfo },
                            { "Page", this.PageMetaInfo },
                            { "Parameter", this.ParameterMetaInfo },
                            { "ParameterBase", this.ParameterBaseMetaInfo },
                            { "ParameterGroup", this.ParameterGroupMetaInfo },
                            { "ParameterizedCategoryRule", this.ParameterizedCategoryRuleMetaInfo },
                            { "ParameterOrOverrideBase", this.ParameterOrOverrideBaseMetaInfo },
                            { "ParameterOverride", this.ParameterOverrideMetaInfo },
                            { "ParameterOverrideValueSet", this.ParameterOverrideValueSetMetaInfo },
                            { "ParameterSubscription", this.ParameterSubscriptionMetaInfo },
                            { "ParameterSubscriptionValueSet", this.ParameterSubscriptionValueSetMetaInfo },
                            { "ParameterType", this.ParameterTypeMetaInfo },
                            { "ParameterTypeComponent", this.ParameterTypeComponentMetaInfo },
                            { "ParameterValue", this.ParameterValueMetaInfo },
                            { "ParameterValueSet", this.ParameterValueSetMetaInfo },
                            { "ParameterValueSetBase", this.ParameterValueSetBaseMetaInfo },
                            { "ParametricConstraint", this.ParametricConstraintMetaInfo },
                            { "Participant", this.ParticipantMetaInfo },
                            { "ParticipantPermission", this.ParticipantPermissionMetaInfo },
                            { "ParticipantRole", this.ParticipantRoleMetaInfo },
                            { "Person", this.PersonMetaInfo },
                            { "PersonPermission", this.PersonPermissionMetaInfo },
                            { "PersonRole", this.PersonRoleMetaInfo },
                            { "Point", this.PointMetaInfo },
                            { "PossibleFiniteState", this.PossibleFiniteStateMetaInfo },
                            { "PossibleFiniteStateList", this.PossibleFiniteStateListMetaInfo },
                            { "PrefixedUnit", this.PrefixedUnitMetaInfo },
                            { "Publication", this.PublicationMetaInfo },
                            { "QuantityKind", this.QuantityKindMetaInfo },
                            { "QuantityKindFactor", this.QuantityKindFactorMetaInfo },
                            { "RatioScale", this.RatioScaleMetaInfo },
                            { "ReferenceDataLibrary", this.ReferenceDataLibraryMetaInfo },
                            { "ReferencerRule", this.ReferencerRuleMetaInfo },
                            { "ReferenceSource", this.ReferenceSourceMetaInfo },
                            { "RelationalExpression", this.RelationalExpressionMetaInfo },
                            { "Relationship", this.RelationshipMetaInfo },
                            { "RelationshipParameterValue", this.RelationshipParameterValueMetaInfo },
                            { "RequestForDeviation", this.RequestForDeviationMetaInfo },
                            { "RequestForWaiver", this.RequestForWaiverMetaInfo },
                            { "Requirement", this.RequirementMetaInfo },
                            { "RequirementsContainer", this.RequirementsContainerMetaInfo },
                            { "RequirementsContainerParameterValue", this.RequirementsContainerParameterValueMetaInfo },
                            { "RequirementsGroup", this.RequirementsGroupMetaInfo },
                            { "RequirementsSpecification", this.RequirementsSpecificationMetaInfo },
                            { "ReviewItemDiscrepancy", this.ReviewItemDiscrepancyMetaInfo },
                            { "Rule", this.RuleMetaInfo },
                            { "RuleVerification", this.RuleVerificationMetaInfo },
                            { "RuleVerificationList", this.RuleVerificationListMetaInfo },
                            { "RuleViolation", this.RuleViolationMetaInfo },
                            { "SampledFunctionParameterType", this.SampledFunctionParameterTypeMetaInfo },
                            { "ScalarParameterType", this.ScalarParameterTypeMetaInfo },
                            { "ScaleReferenceQuantityValue", this.ScaleReferenceQuantityValueMetaInfo },
                            { "ScaleValueDefinition", this.ScaleValueDefinitionMetaInfo },
                            { "Section", this.SectionMetaInfo },
                            { "SharedStyle", this.SharedStyleMetaInfo },
                            { "SimpleParameterizableThing", this.SimpleParameterizableThingMetaInfo },
                            { "SimpleParameterValue", this.SimpleParameterValueMetaInfo },
                            { "SimpleQuantityKind", this.SimpleQuantityKindMetaInfo },
                            { "SimpleUnit", this.SimpleUnitMetaInfo },
                            { "SiteDirectory", this.SiteDirectoryMetaInfo },
                            { "SiteDirectoryDataAnnotation", this.SiteDirectoryDataAnnotationMetaInfo },
                            { "SiteDirectoryDataDiscussionItem", this.SiteDirectoryDataDiscussionItemMetaInfo },
                            { "SiteDirectoryThingReference", this.SiteDirectoryThingReferenceMetaInfo },
                            { "SiteLogEntry", this.SiteLogEntryMetaInfo },
                            { "SiteReferenceDataLibrary", this.SiteReferenceDataLibraryMetaInfo },
                            { "Solution", this.SolutionMetaInfo },
                            { "SpecializedQuantityKind", this.SpecializedQuantityKindMetaInfo },
                            { "Stakeholder", this.StakeholderMetaInfo },
                            { "StakeholderValue", this.StakeholderValueMetaInfo },
                            { "StakeHolderValueMap", this.StakeHolderValueMapMetaInfo },
                            { "StakeHolderValueMapSettings", this.StakeHolderValueMapSettingsMetaInfo },
                            { "TelephoneNumber", this.TelephoneNumberMetaInfo },
                            { "Term", this.TermMetaInfo },
                            { "TextParameterType", this.TextParameterTypeMetaInfo },
                            { "TextualNote", this.TextualNoteMetaInfo },
                            { "Thing", this.ThingMetaInfo },
                            { "ThingReference", this.ThingReferenceMetaInfo },
                            { "TimeOfDayParameterType", this.TimeOfDayParameterTypeMetaInfo },
                            { "TopContainer", this.TopContainerMetaInfo },
                            { "UnitFactor", this.UnitFactorMetaInfo },
                            { "UnitPrefix", this.UnitPrefixMetaInfo },
                            { "UserPreference", this.UserPreferenceMetaInfo },
                            { "UserRuleVerification", this.UserRuleVerificationMetaInfo },
                            { "ValueGroup", this.ValueGroupMetaInfo },
                        };
                }

                return this.metaInfoMap;
            }
        }
 
        /// <summary>
        /// Returns a meta info instance based on the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// A concrete meta info instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported
        /// </exception>
        public IMetaInfo GetMetaInfo(string typeName)
        {
            if (!this.MetaInfoMap.ContainsKey(typeName))
            {
                throw new TypeLoadException(string.Format("Type not supported {0}", typeName));
            }

            return this.MetaInfoMap[typeName];
        }
 
        /// <summary>
        /// Returns a meta info instance based on the passed in <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance.
        /// </param>
        /// <returns>
        /// A concrete meta info instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported
        /// </exception>
        public IMetaInfo GetMetaInfo(Thing thing)
        {
            var typeName = thing.GetType().Name;
            return this.GetMetaInfo(typeName);
        }
   
        /// <summary>
        /// Get the base type name of the supplied type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The base type or the same type name if class is the inheritance root.
        /// </returns>
        public string BaseType(string typeName)
        {
            var metaInfo = this.GetMetaInfo(typeName);
            return metaInfo.BaseType;
        }
 
        /// <summary>
        /// Get the class version for the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The version string.
        /// </returns>
        public string GetClassVersion(string typeName)
        {
            var metaInfo = this.GetMetaInfo(typeName);
            return metaInfo.ClassVersion ?? this.DefaultModelVersion;
        }

        /// <summary>
        /// Get the property version for the passed in type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The version string.
        /// </returns>
        public string GetPropertyVersion(string typeName, string propertyName)
        {
            var metaInfo = this.GetMetaInfo(typeName);
            return metaInfo.GetPropertyVersion(propertyName) ?? this.DefaultModelVersion;
        }
    }
}
