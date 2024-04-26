// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaoResolver.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace CDP4Orm.Helper
{
    using System;
    using System.IO;

    using CDP4Orm.Dao;

    /// <summary>
    /// The <see cref="DaoResolver" /> provides resolve capabilites to retrieve <see cref="IDao" /> based on the type name
    /// <remarks>This class is registered as InstancePerLifetimeScope</remarks>
    /// </summary>
    public class DaoResolver: IDaoResolver
    {
        /// <summary>
        /// Gets or sets the injected <see cref="IActionItemDao" />
        /// </summary>
        public IActionItemDao ActionItem { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IActualFiniteStateDao" />
        /// </summary>
        public IActualFiniteStateDao ActualFiniteState { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IActualFiniteStateListDao" />
        /// </summary>
        public IActualFiniteStateListDao ActualFiniteStateList { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IAliasDao" />
        /// </summary>
        public IAliasDao Alias { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IAndExpressionDao" />
        /// </summary>
        public IAndExpressionDao AndExpression { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IApprovalDao" />
        /// </summary>
        public IApprovalDao Approval { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IArrayParameterTypeDao" />
        /// </summary>
        public IArrayParameterTypeDao ArrayParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBinaryNoteDao" />
        /// </summary>
        public IBinaryNoteDao BinaryNote { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBinaryRelationshipDao" />
        /// </summary>
        public IBinaryRelationshipDao BinaryRelationship { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBinaryRelationshipRuleDao" />
        /// </summary>
        public IBinaryRelationshipRuleDao BinaryRelationshipRule { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBookDao" />
        /// </summary>
        public IBookDao Book { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBooleanParameterTypeDao" />
        /// </summary>
        public IBooleanParameterTypeDao BooleanParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBoundsDao" />
        /// </summary>
        public IBoundsDao Bounds { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IBuiltInRuleVerificationDao" />
        /// </summary>
        public IBuiltInRuleVerificationDao BuiltInRuleVerification { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ICategoryDao" />
        /// </summary>
        public ICategoryDao Category { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IChangeProposalDao" />
        /// </summary>
        public IChangeProposalDao ChangeProposal { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IChangeRequestDao" />
        /// </summary>
        public IChangeRequestDao ChangeRequest { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ICitationDao" />
        /// </summary>
        public ICitationDao Citation { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IColorDao" />
        /// </summary>
        public IColorDao Color { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ICommonFileStoreDao" />
        /// </summary>
        public ICommonFileStoreDao CommonFileStore { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ICompoundParameterTypeDao" />
        /// </summary>
        public ICompoundParameterTypeDao CompoundParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IConstantDao" />
        /// </summary>
        public IConstantDao Constant { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IContractChangeNoticeDao" />
        /// </summary>
        public IContractChangeNoticeDao ContractChangeNotice { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ICyclicRatioScaleDao" />
        /// </summary>
        public ICyclicRatioScaleDao CyclicRatioScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDateParameterTypeDao" />
        /// </summary>
        public IDateParameterTypeDao DateParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDateTimeParameterTypeDao" />
        /// </summary>
        public IDateTimeParameterTypeDao DateTimeParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDecompositionRuleDao" />
        /// </summary>
        public IDecompositionRuleDao DecompositionRule { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDefinitionDao" />
        /// </summary>
        public IDefinitionDao Definition { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDependentParameterTypeAssignmentDao" />
        /// </summary>
        public IDependentParameterTypeAssignmentDao DependentParameterTypeAssignment { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDerivedQuantityKindDao" />
        /// </summary>
        public IDerivedQuantityKindDao DerivedQuantityKind { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDerivedUnitDao" />
        /// </summary>
        public IDerivedUnitDao DerivedUnit { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDiagramCanvasDao" />
        /// </summary>
        public IDiagramCanvasDao DiagramCanvas { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDiagramEdgeDao" />
        /// </summary>
        public IDiagramEdgeDao DiagramEdge { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDiagramObjectDao" />
        /// </summary>
        public IDiagramObjectDao DiagramObject { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDomainFileStoreDao" />
        /// </summary>
        public IDomainFileStoreDao DomainFileStore { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDomainOfExpertiseDao" />
        /// </summary>
        public IDomainOfExpertiseDao DomainOfExpertise { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IDomainOfExpertiseGroupDao" />
        /// </summary>
        public IDomainOfExpertiseGroupDao DomainOfExpertiseGroup { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IElementDefinitionDao" />
        /// </summary>
        public IElementDefinitionDao ElementDefinition { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IElementUsageDao" />
        /// </summary>
        public IElementUsageDao ElementUsage { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEmailAddressDao" />
        /// </summary>
        public IEmailAddressDao EmailAddress { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelDao" />
        /// </summary>
        public IEngineeringModelDao EngineeringModel { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelDataDiscussionItemDao" />
        /// </summary>
        public IEngineeringModelDataDiscussionItemDao EngineeringModelDataDiscussionItem { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelDataNoteDao" />
        /// </summary>
        public IEngineeringModelDataNoteDao EngineeringModelDataNote { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelSetupDao" />
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetup { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEnumerationParameterTypeDao" />
        /// </summary>
        public IEnumerationParameterTypeDao EnumerationParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IEnumerationValueDefinitionDao" />
        /// </summary>
        public IEnumerationValueDefinitionDao EnumerationValueDefinition { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IExclusiveOrExpressionDao" />
        /// </summary>
        public IExclusiveOrExpressionDao ExclusiveOrExpression { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IExternalIdentifierMapDao" />
        /// </summary>
        public IExternalIdentifierMapDao ExternalIdentifierMap { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IFileDao" />
        /// </summary>
        public IFileDao File { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IFileRevisionDao" />
        /// </summary>
        public IFileRevisionDao FileRevision { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IFileTypeDao" />
        /// </summary>
        public IFileTypeDao FileType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IFolderDao" />
        /// </summary>
        public IFolderDao Folder { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IGlossaryDao" />
        /// </summary>
        public IGlossaryDao Glossary { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IGoalDao" />
        /// </summary>
        public IGoalDao Goal { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IHyperLinkDao" />
        /// </summary>
        public IHyperLinkDao HyperLink { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IIdCorrespondenceDao" />
        /// </summary>
        public IIdCorrespondenceDao IdCorrespondence { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IIndependentParameterTypeAssignmentDao" />
        /// </summary>
        public IIndependentParameterTypeAssignmentDao IndependentParameterTypeAssignment { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IIntervalScaleDao" />
        /// </summary>
        public IIntervalScaleDao IntervalScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IIterationDao" />
        /// </summary>
        public IIterationDao Iteration { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IIterationSetupDao" />
        /// </summary>
        public IIterationSetupDao IterationSetup { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ILinearConversionUnitDao" />
        /// </summary>
        public ILinearConversionUnitDao LinearConversionUnit { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ILogarithmicScaleDao" />
        /// </summary>
        public ILogarithmicScaleDao LogarithmicScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ILogEntryChangelogItemDao" />
        /// </summary>
        public ILogEntryChangelogItemDao LogEntryChangelogItem { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IMappingToReferenceScaleDao" />
        /// </summary>
        public IMappingToReferenceScaleDao MappingToReferenceScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IModellingThingReferenceDao" />
        /// </summary>
        public IModellingThingReferenceDao ModellingThingReference { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IModelLogEntryDao" />
        /// </summary>
        public IModelLogEntryDao ModelLogEntry { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IModelReferenceDataLibraryDao" />
        /// </summary>
        public IModelReferenceDataLibraryDao ModelReferenceDataLibrary { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IMultiRelationshipDao" />
        /// </summary>
        public IMultiRelationshipDao MultiRelationship { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IMultiRelationshipRuleDao" />
        /// </summary>
        public IMultiRelationshipRuleDao MultiRelationshipRule { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="INaturalLanguageDao" />
        /// </summary>
        public INaturalLanguageDao NaturalLanguage { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="INestedElementDao" />
        /// </summary>
        public INestedElementDao NestedElement { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="INestedParameterDao" />
        /// </summary>
        public INestedParameterDao NestedParameter { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="INotExpressionDao" />
        /// </summary>
        public INotExpressionDao NotExpression { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOptionDao" />
        /// </summary>
        public IOptionDao Option { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOrdinalScaleDao" />
        /// </summary>
        public IOrdinalScaleDao OrdinalScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOrExpressionDao" />
        /// </summary>
        public IOrExpressionDao OrExpression { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOrganizationDao" />
        /// </summary>
        public IOrganizationDao Organization { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOrganizationalParticipantDao" />
        /// </summary>
        public IOrganizationalParticipantDao OrganizationalParticipant { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IOwnedStyleDao" />
        /// </summary>
        public IOwnedStyleDao OwnedStyle { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPageDao" />
        /// </summary>
        public IPageDao Page { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterDao" />
        /// </summary>
        public IParameterDao Parameter { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterGroupDao" />
        /// </summary>
        public IParameterGroupDao ParameterGroup { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterizedCategoryRuleDao" />
        /// </summary>
        public IParameterizedCategoryRuleDao ParameterizedCategoryRule { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterOverrideDao" />
        /// </summary>
        public IParameterOverrideDao ParameterOverride { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterOverrideValueSetDao" />
        /// </summary>
        public IParameterOverrideValueSetDao ParameterOverrideValueSet { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterSubscriptionDao" />
        /// </summary>
        public IParameterSubscriptionDao ParameterSubscription { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterSubscriptionValueSetDao" />
        /// </summary>
        public IParameterSubscriptionValueSetDao ParameterSubscriptionValueSet { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterTypeComponentDao" />
        /// </summary>
        public IParameterTypeComponentDao ParameterTypeComponent { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterValueSetDao" />
        /// </summary>
        public IParameterValueSetDao ParameterValueSet { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParametricConstraintDao" />
        /// </summary>
        public IParametricConstraintDao ParametricConstraint { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParticipantDao" />
        /// </summary>
        public IParticipantDao Participant { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParticipantPermissionDao" />
        /// </summary>
        public IParticipantPermissionDao ParticipantPermission { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IParticipantRoleDao" />
        /// </summary>
        public IParticipantRoleDao ParticipantRole { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPersonDao" />
        /// </summary>
        public IPersonDao Person { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPersonPermissionDao" />
        /// </summary>
        public IPersonPermissionDao PersonPermission { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPersonRoleDao" />
        /// </summary>
        public IPersonRoleDao PersonRole { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPointDao" />
        /// </summary>
        public IPointDao Point { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPossibleFiniteStateDao" />
        /// </summary>
        public IPossibleFiniteStateDao PossibleFiniteState { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPossibleFiniteStateListDao" />
        /// </summary>
        public IPossibleFiniteStateListDao PossibleFiniteStateList { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPrefixedUnitDao" />
        /// </summary>
        public IPrefixedUnitDao PrefixedUnit { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IPublicationDao" />
        /// </summary>
        public IPublicationDao Publication { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IQuantityKindFactorDao" />
        /// </summary>
        public IQuantityKindFactorDao QuantityKindFactor { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRatioScaleDao" />
        /// </summary>
        public IRatioScaleDao RatioScale { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IReferencerRuleDao" />
        /// </summary>
        public IReferencerRuleDao ReferencerRule { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IReferenceSourceDao" />
        /// </summary>
        public IReferenceSourceDao ReferenceSource { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRelationalExpressionDao" />
        /// </summary>
        public IRelationalExpressionDao RelationalExpression { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRelationshipParameterValueDao" />
        /// </summary>
        public IRelationshipParameterValueDao RelationshipParameterValue { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequestForDeviationDao" />
        /// </summary>
        public IRequestForDeviationDao RequestForDeviation { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequestForWaiverDao" />
        /// </summary>
        public IRequestForWaiverDao RequestForWaiver { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequirementDao" />
        /// </summary>
        public IRequirementDao Requirement { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequirementsContainerParameterValueDao" />
        /// </summary>
        public IRequirementsContainerParameterValueDao RequirementsContainerParameterValue { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequirementsGroupDao" />
        /// </summary>
        public IRequirementsGroupDao RequirementsGroup { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRequirementsSpecificationDao" />
        /// </summary>
        public IRequirementsSpecificationDao RequirementsSpecification { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IReviewItemDiscrepancyDao" />
        /// </summary>
        public IReviewItemDiscrepancyDao ReviewItemDiscrepancy { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRuleVerificationListDao" />
        /// </summary>
        public IRuleVerificationListDao RuleVerificationList { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IRuleViolationDao" />
        /// </summary>
        public IRuleViolationDao RuleViolation { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISampledFunctionParameterTypeDao" />
        /// </summary>
        public ISampledFunctionParameterTypeDao SampledFunctionParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IScaleReferenceQuantityValueDao" />
        /// </summary>
        public IScaleReferenceQuantityValueDao ScaleReferenceQuantityValue { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IScaleValueDefinitionDao" />
        /// </summary>
        public IScaleValueDefinitionDao ScaleValueDefinition { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISectionDao" />
        /// </summary>
        public ISectionDao Section { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISharedStyleDao" />
        /// </summary>
        public ISharedStyleDao SharedStyle { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISimpleParameterValueDao" />
        /// </summary>
        public ISimpleParameterValueDao SimpleParameterValue { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISimpleQuantityKindDao" />
        /// </summary>
        public ISimpleQuantityKindDao SimpleQuantityKind { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISimpleUnitDao" />
        /// </summary>
        public ISimpleUnitDao SimpleUnit { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteDirectoryDao" />
        /// </summary>
        public ISiteDirectoryDao SiteDirectory { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteDirectoryDataAnnotationDao" />
        /// </summary>
        public ISiteDirectoryDataAnnotationDao SiteDirectoryDataAnnotation { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteDirectoryDataDiscussionItemDao" />
        /// </summary>
        public ISiteDirectoryDataDiscussionItemDao SiteDirectoryDataDiscussionItem { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteDirectoryThingReferenceDao" />
        /// </summary>
        public ISiteDirectoryThingReferenceDao SiteDirectoryThingReference { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteLogEntryDao" />
        /// </summary>
        public ISiteLogEntryDao SiteLogEntry { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISiteReferenceDataLibraryDao" />
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibrary { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISolutionDao" />
        /// </summary>
        public ISolutionDao Solution { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ISpecializedQuantityKindDao" />
        /// </summary>
        public ISpecializedQuantityKindDao SpecializedQuantityKind { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IStakeholderDao" />
        /// </summary>
        public IStakeholderDao Stakeholder { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IStakeholderValueDao" />
        /// </summary>
        public IStakeholderValueDao StakeholderValue { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IStakeHolderValueMapDao" />
        /// </summary>
        public IStakeHolderValueMapDao StakeHolderValueMap { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IStakeHolderValueMapSettingsDao" />
        /// </summary>
        public IStakeHolderValueMapSettingsDao StakeHolderValueMapSettings { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ITelephoneNumberDao" />
        /// </summary>
        public ITelephoneNumberDao TelephoneNumber { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ITermDao" />
        /// </summary>
        public ITermDao Term { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ITextParameterTypeDao" />
        /// </summary>
        public ITextParameterTypeDao TextParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ITextualNoteDao" />
        /// </summary>
        public ITextualNoteDao TextualNote { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="ITimeOfDayParameterTypeDao" />
        /// </summary>
        public ITimeOfDayParameterTypeDao TimeOfDayParameterType { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IUnitFactorDao" />
        /// </summary>
        public IUnitFactorDao UnitFactor { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IUnitPrefixDao" />
        /// </summary>
        public IUnitPrefixDao UnitPrefix { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IUserPreferenceDao" />
        /// </summary>
        public IUserPreferenceDao UserPreference { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IUserRuleVerificationDao" />
        /// </summary>
        public IUserRuleVerificationDao UserRuleVerification { get; set; } 

        /// <summary>
        /// Gets or sets the injected <see cref="IValueGroupDao" />
        /// </summary>
        public IValueGroupDao ValueGroup { get; set; } 

        /// <summary>
        /// Queries an <see cref="IDao" /> based on the <see cref="Type" /> name
        /// </summary>
        /// <param name="typeName">The <see cref="Type" /> name</param>
        /// <returns>The retrieved <see cref="IDao" /></returns>
        public IDao QueryDaoByTypeName(string typeName)
        {
            return typeName switch
            {
                "ActionItem" => this.ActionItem,
                "ActualFiniteState" => this.ActualFiniteState,
                "ActualFiniteStateList" => this.ActualFiniteStateList,
                "Alias" => this.Alias,
                "AndExpression" => this.AndExpression,
                "Approval" => this.Approval,
                "ArrayParameterType" => this.ArrayParameterType,
                "BinaryNote" => this.BinaryNote,
                "BinaryRelationship" => this.BinaryRelationship,
                "BinaryRelationshipRule" => this.BinaryRelationshipRule,
                "Book" => this.Book,
                "BooleanParameterType" => this.BooleanParameterType,
                "Bounds" => this.Bounds,
                "BuiltInRuleVerification" => this.BuiltInRuleVerification,
                "Category" => this.Category,
                "ChangeProposal" => this.ChangeProposal,
                "ChangeRequest" => this.ChangeRequest,
                "Citation" => this.Citation,
                "Color" => this.Color,
                "CommonFileStore" => this.CommonFileStore,
                "CompoundParameterType" => this.CompoundParameterType,
                "Constant" => this.Constant,
                "ContractChangeNotice" => this.ContractChangeNotice,
                "CyclicRatioScale" => this.CyclicRatioScale,
                "DateParameterType" => this.DateParameterType,
                "DateTimeParameterType" => this.DateTimeParameterType,
                "DecompositionRule" => this.DecompositionRule,
                "Definition" => this.Definition,
                "DependentParameterTypeAssignment" => this.DependentParameterTypeAssignment,
                "DerivedQuantityKind" => this.DerivedQuantityKind,
                "DerivedUnit" => this.DerivedUnit,
                "DiagramCanvas" => this.DiagramCanvas,
                "DiagramEdge" => this.DiagramEdge,
                "DiagramObject" => this.DiagramObject,
                "DomainFileStore" => this.DomainFileStore,
                "DomainOfExpertise" => this.DomainOfExpertise,
                "DomainOfExpertiseGroup" => this.DomainOfExpertiseGroup,
                "ElementDefinition" => this.ElementDefinition,
                "ElementUsage" => this.ElementUsage,
                "EmailAddress" => this.EmailAddress,
                "EngineeringModel" => this.EngineeringModel,
                "EngineeringModelDataDiscussionItem" => this.EngineeringModelDataDiscussionItem,
                "EngineeringModelDataNote" => this.EngineeringModelDataNote,
                "EngineeringModelSetup" => this.EngineeringModelSetup,
                "EnumerationParameterType" => this.EnumerationParameterType,
                "EnumerationValueDefinition" => this.EnumerationValueDefinition,
                "ExclusiveOrExpression" => this.ExclusiveOrExpression,
                "ExternalIdentifierMap" => this.ExternalIdentifierMap,
                "File" => this.File,
                "FileRevision" => this.FileRevision,
                "FileType" => this.FileType,
                "Folder" => this.Folder,
                "Glossary" => this.Glossary,
                "Goal" => this.Goal,
                "HyperLink" => this.HyperLink,
                "IdCorrespondence" => this.IdCorrespondence,
                "IndependentParameterTypeAssignment" => this.IndependentParameterTypeAssignment,
                "IntervalScale" => this.IntervalScale,
                "Iteration" => this.Iteration,
                "IterationSetup" => this.IterationSetup,
                "LinearConversionUnit" => this.LinearConversionUnit,
                "LogarithmicScale" => this.LogarithmicScale,
                "LogEntryChangelogItem" => this.LogEntryChangelogItem,
                "MappingToReferenceScale" => this.MappingToReferenceScale,
                "ModellingThingReference" => this.ModellingThingReference,
                "ModelLogEntry" => this.ModelLogEntry,
                "ModelReferenceDataLibrary" => this.ModelReferenceDataLibrary,
                "MultiRelationship" => this.MultiRelationship,
                "MultiRelationshipRule" => this.MultiRelationshipRule,
                "NaturalLanguage" => this.NaturalLanguage,
                "NestedElement" => this.NestedElement,
                "NestedParameter" => this.NestedParameter,
                "NotExpression" => this.NotExpression,
                "Option" => this.Option,
                "OrdinalScale" => this.OrdinalScale,
                "OrExpression" => this.OrExpression,
                "Organization" => this.Organization,
                "OrganizationalParticipant" => this.OrganizationalParticipant,
                "OwnedStyle" => this.OwnedStyle,
                "Page" => this.Page,
                "Parameter" => this.Parameter,
                "ParameterGroup" => this.ParameterGroup,
                "ParameterizedCategoryRule" => this.ParameterizedCategoryRule,
                "ParameterOverride" => this.ParameterOverride,
                "ParameterOverrideValueSet" => this.ParameterOverrideValueSet,
                "ParameterSubscription" => this.ParameterSubscription,
                "ParameterSubscriptionValueSet" => this.ParameterSubscriptionValueSet,
                "ParameterTypeComponent" => this.ParameterTypeComponent,
                "ParameterValueSet" => this.ParameterValueSet,
                "ParametricConstraint" => this.ParametricConstraint,
                "Participant" => this.Participant,
                "ParticipantPermission" => this.ParticipantPermission,
                "ParticipantRole" => this.ParticipantRole,
                "Person" => this.Person,
                "PersonPermission" => this.PersonPermission,
                "PersonRole" => this.PersonRole,
                "Point" => this.Point,
                "PossibleFiniteState" => this.PossibleFiniteState,
                "PossibleFiniteStateList" => this.PossibleFiniteStateList,
                "PrefixedUnit" => this.PrefixedUnit,
                "Publication" => this.Publication,
                "QuantityKindFactor" => this.QuantityKindFactor,
                "RatioScale" => this.RatioScale,
                "ReferencerRule" => this.ReferencerRule,
                "ReferenceSource" => this.ReferenceSource,
                "RelationalExpression" => this.RelationalExpression,
                "RelationshipParameterValue" => this.RelationshipParameterValue,
                "RequestForDeviation" => this.RequestForDeviation,
                "RequestForWaiver" => this.RequestForWaiver,
                "Requirement" => this.Requirement,
                "RequirementsContainerParameterValue" => this.RequirementsContainerParameterValue,
                "RequirementsGroup" => this.RequirementsGroup,
                "RequirementsSpecification" => this.RequirementsSpecification,
                "ReviewItemDiscrepancy" => this.ReviewItemDiscrepancy,
                "RuleVerificationList" => this.RuleVerificationList,
                "RuleViolation" => this.RuleViolation,
                "SampledFunctionParameterType" => this.SampledFunctionParameterType,
                "ScaleReferenceQuantityValue" => this.ScaleReferenceQuantityValue,
                "ScaleValueDefinition" => this.ScaleValueDefinition,
                "Section" => this.Section,
                "SharedStyle" => this.SharedStyle,
                "SimpleParameterValue" => this.SimpleParameterValue,
                "SimpleQuantityKind" => this.SimpleQuantityKind,
                "SimpleUnit" => this.SimpleUnit,
                "SiteDirectory" => this.SiteDirectory,
                "SiteDirectoryDataAnnotation" => this.SiteDirectoryDataAnnotation,
                "SiteDirectoryDataDiscussionItem" => this.SiteDirectoryDataDiscussionItem,
                "SiteDirectoryThingReference" => this.SiteDirectoryThingReference,
                "SiteLogEntry" => this.SiteLogEntry,
                "SiteReferenceDataLibrary" => this.SiteReferenceDataLibrary,
                "Solution" => this.Solution,
                "SpecializedQuantityKind" => this.SpecializedQuantityKind,
                "Stakeholder" => this.Stakeholder,
                "StakeholderValue" => this.StakeholderValue,
                "StakeHolderValueMap" => this.StakeHolderValueMap,
                "StakeHolderValueMapSettings" => this.StakeHolderValueMapSettings,
                "TelephoneNumber" => this.TelephoneNumber,
                "Term" => this.Term,
                "TextParameterType" => this.TextParameterType,
                "TextualNote" => this.TextualNote,
                "TimeOfDayParameterType" => this.TimeOfDayParameterType,
                "UnitFactor" => this.UnitFactor,
                "UnitPrefix" => this.UnitPrefix,
                "UserPreference" => this.UserPreference,
                "UserRuleVerification" => this.UserRuleVerification,
                "ValueGroup" => this.ValueGroup,
                _ => throw new InvalidDataException($"The provided {typeName} Dao could not be retrieved")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
