// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaoResolver.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Collections.Generic;
    using System.IO;

    using CDP4Orm.Dao;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The <see cref="DaoResolver" /> provides resolve capabilites to retrieve <see cref="IDao" /> based on the type name
    /// </summary>
    public class DaoResolver: IDaoResolver
    {
        /// <summary>
        /// Gets the <see cref="Dictionary{TKey,TValue}" /> that can provides
        /// </summary>
        private readonly Dictionary<string, Func<IDao>> existingDaos;

        /// <summary>
        /// Initializes a new <see cref="DaoResolver" />
        /// </summary>
        /// <param name="serviceProvider">The injected <see cref="IServiceProvider" /></param>
        public DaoResolver(IServiceProvider serviceProvider)
        {
            this.existingDaos = new Dictionary<string, Func<IDao>>
            {
                { "ActionItem", serviceProvider.GetService<IActionItemDao> },
                { "ActualFiniteState", serviceProvider.GetService<IActualFiniteStateDao> },
                { "ActualFiniteStateList", serviceProvider.GetService<IActualFiniteStateListDao> },
                { "Alias", serviceProvider.GetService<IAliasDao> },
                { "AndExpression", serviceProvider.GetService<IAndExpressionDao> },
                { "Approval", serviceProvider.GetService<IApprovalDao> },
                { "ArrayParameterType", serviceProvider.GetService<IArrayParameterTypeDao> },
                { "BinaryNote", serviceProvider.GetService<IBinaryNoteDao> },
                { "BinaryRelationship", serviceProvider.GetService<IBinaryRelationshipDao> },
                { "BinaryRelationshipRule", serviceProvider.GetService<IBinaryRelationshipRuleDao> },
                { "Book", serviceProvider.GetService<IBookDao> },
                { "BooleanParameterType", serviceProvider.GetService<IBooleanParameterTypeDao> },
                { "Bounds", serviceProvider.GetService<IBoundsDao> },
                { "BuiltInRuleVerification", serviceProvider.GetService<IBuiltInRuleVerificationDao> },
                { "Category", serviceProvider.GetService<ICategoryDao> },
                { "ChangeProposal", serviceProvider.GetService<IChangeProposalDao> },
                { "ChangeRequest", serviceProvider.GetService<IChangeRequestDao> },
                { "Citation", serviceProvider.GetService<ICitationDao> },
                { "Color", serviceProvider.GetService<IColorDao> },
                { "CommonFileStore", serviceProvider.GetService<ICommonFileStoreDao> },
                { "CompoundParameterType", serviceProvider.GetService<ICompoundParameterTypeDao> },
                { "Constant", serviceProvider.GetService<IConstantDao> },
                { "ContractChangeNotice", serviceProvider.GetService<IContractChangeNoticeDao> },
                { "CyclicRatioScale", serviceProvider.GetService<ICyclicRatioScaleDao> },
                { "DateParameterType", serviceProvider.GetService<IDateParameterTypeDao> },
                { "DateTimeParameterType", serviceProvider.GetService<IDateTimeParameterTypeDao> },
                { "DecompositionRule", serviceProvider.GetService<IDecompositionRuleDao> },
                { "Definition", serviceProvider.GetService<IDefinitionDao> },
                { "DependentParameterTypeAssignment", serviceProvider.GetService<IDependentParameterTypeAssignmentDao> },
                { "DerivedQuantityKind", serviceProvider.GetService<IDerivedQuantityKindDao> },
                { "DerivedUnit", serviceProvider.GetService<IDerivedUnitDao> },
                { "DiagramCanvas", serviceProvider.GetService<IDiagramCanvasDao> },
                { "DiagramEdge", serviceProvider.GetService<IDiagramEdgeDao> },
                { "DiagramObject", serviceProvider.GetService<IDiagramObjectDao> },
                { "DomainFileStore", serviceProvider.GetService<IDomainFileStoreDao> },
                { "DomainOfExpertise", serviceProvider.GetService<IDomainOfExpertiseDao> },
                { "DomainOfExpertiseGroup", serviceProvider.GetService<IDomainOfExpertiseGroupDao> },
                { "ElementDefinition", serviceProvider.GetService<IElementDefinitionDao> },
                { "ElementUsage", serviceProvider.GetService<IElementUsageDao> },
                { "EmailAddress", serviceProvider.GetService<IEmailAddressDao> },
                { "EngineeringModel", serviceProvider.GetService<IEngineeringModelDao> },
                { "EngineeringModelDataDiscussionItem", serviceProvider.GetService<IEngineeringModelDataDiscussionItemDao> },
                { "EngineeringModelDataNote", serviceProvider.GetService<IEngineeringModelDataNoteDao> },
                { "EngineeringModelSetup", serviceProvider.GetService<IEngineeringModelSetupDao> },
                { "EnumerationParameterType", serviceProvider.GetService<IEnumerationParameterTypeDao> },
                { "EnumerationValueDefinition", serviceProvider.GetService<IEnumerationValueDefinitionDao> },
                { "ExclusiveOrExpression", serviceProvider.GetService<IExclusiveOrExpressionDao> },
                { "ExternalIdentifierMap", serviceProvider.GetService<IExternalIdentifierMapDao> },
                { "File", serviceProvider.GetService<IFileDao> },
                { "FileRevision", serviceProvider.GetService<IFileRevisionDao> },
                { "FileType", serviceProvider.GetService<IFileTypeDao> },
                { "Folder", serviceProvider.GetService<IFolderDao> },
                { "Glossary", serviceProvider.GetService<IGlossaryDao> },
                { "Goal", serviceProvider.GetService<IGoalDao> },
                { "HyperLink", serviceProvider.GetService<IHyperLinkDao> },
                { "IdCorrespondence", serviceProvider.GetService<IIdCorrespondenceDao> },
                { "IndependentParameterTypeAssignment", serviceProvider.GetService<IIndependentParameterTypeAssignmentDao> },
                { "IntervalScale", serviceProvider.GetService<IIntervalScaleDao> },
                { "Iteration", serviceProvider.GetService<IIterationDao> },
                { "IterationSetup", serviceProvider.GetService<IIterationSetupDao> },
                { "LinearConversionUnit", serviceProvider.GetService<ILinearConversionUnitDao> },
                { "LogarithmicScale", serviceProvider.GetService<ILogarithmicScaleDao> },
                { "LogEntryChangelogItem", serviceProvider.GetService<ILogEntryChangelogItemDao> },
                { "MappingToReferenceScale", serviceProvider.GetService<IMappingToReferenceScaleDao> },
                { "ModellingThingReference", serviceProvider.GetService<IModellingThingReferenceDao> },
                { "ModelLogEntry", serviceProvider.GetService<IModelLogEntryDao> },
                { "ModelReferenceDataLibrary", serviceProvider.GetService<IModelReferenceDataLibraryDao> },
                { "MultiRelationship", serviceProvider.GetService<IMultiRelationshipDao> },
                { "MultiRelationshipRule", serviceProvider.GetService<IMultiRelationshipRuleDao> },
                { "NaturalLanguage", serviceProvider.GetService<INaturalLanguageDao> },
                { "NestedElement", serviceProvider.GetService<INestedElementDao> },
                { "NestedParameter", serviceProvider.GetService<INestedParameterDao> },
                { "NotExpression", serviceProvider.GetService<INotExpressionDao> },
                { "Option", serviceProvider.GetService<IOptionDao> },
                { "OrdinalScale", serviceProvider.GetService<IOrdinalScaleDao> },
                { "OrExpression", serviceProvider.GetService<IOrExpressionDao> },
                { "Organization", serviceProvider.GetService<IOrganizationDao> },
                { "OrganizationalParticipant", serviceProvider.GetService<IOrganizationalParticipantDao> },
                { "OwnedStyle", serviceProvider.GetService<IOwnedStyleDao> },
                { "Page", serviceProvider.GetService<IPageDao> },
                { "Parameter", serviceProvider.GetService<IParameterDao> },
                { "ParameterGroup", serviceProvider.GetService<IParameterGroupDao> },
                { "ParameterizedCategoryRule", serviceProvider.GetService<IParameterizedCategoryRuleDao> },
                { "ParameterOverride", serviceProvider.GetService<IParameterOverrideDao> },
                { "ParameterOverrideValueSet", serviceProvider.GetService<IParameterOverrideValueSetDao> },
                { "ParameterSubscription", serviceProvider.GetService<IParameterSubscriptionDao> },
                { "ParameterSubscriptionValueSet", serviceProvider.GetService<IParameterSubscriptionValueSetDao> },
                { "ParameterTypeComponent", serviceProvider.GetService<IParameterTypeComponentDao> },
                { "ParameterValueSet", serviceProvider.GetService<IParameterValueSetDao> },
                { "ParametricConstraint", serviceProvider.GetService<IParametricConstraintDao> },
                { "Participant", serviceProvider.GetService<IParticipantDao> },
                { "ParticipantPermission", serviceProvider.GetService<IParticipantPermissionDao> },
                { "ParticipantRole", serviceProvider.GetService<IParticipantRoleDao> },
                { "Person", serviceProvider.GetService<IPersonDao> },
                { "PersonPermission", serviceProvider.GetService<IPersonPermissionDao> },
                { "PersonRole", serviceProvider.GetService<IPersonRoleDao> },
                { "Point", serviceProvider.GetService<IPointDao> },
                { "PossibleFiniteState", serviceProvider.GetService<IPossibleFiniteStateDao> },
                { "PossibleFiniteStateList", serviceProvider.GetService<IPossibleFiniteStateListDao> },
                { "PrefixedUnit", serviceProvider.GetService<IPrefixedUnitDao> },
                { "Publication", serviceProvider.GetService<IPublicationDao> },
                { "QuantityKindFactor", serviceProvider.GetService<IQuantityKindFactorDao> },
                { "RatioScale", serviceProvider.GetService<IRatioScaleDao> },
                { "ReferencerRule", serviceProvider.GetService<IReferencerRuleDao> },
                { "ReferenceSource", serviceProvider.GetService<IReferenceSourceDao> },
                { "RelationalExpression", serviceProvider.GetService<IRelationalExpressionDao> },
                { "RelationshipParameterValue", serviceProvider.GetService<IRelationshipParameterValueDao> },
                { "RequestForDeviation", serviceProvider.GetService<IRequestForDeviationDao> },
                { "RequestForWaiver", serviceProvider.GetService<IRequestForWaiverDao> },
                { "Requirement", serviceProvider.GetService<IRequirementDao> },
                { "RequirementsContainerParameterValue", serviceProvider.GetService<IRequirementsContainerParameterValueDao> },
                { "RequirementsGroup", serviceProvider.GetService<IRequirementsGroupDao> },
                { "RequirementsSpecification", serviceProvider.GetService<IRequirementsSpecificationDao> },
                { "ReviewItemDiscrepancy", serviceProvider.GetService<IReviewItemDiscrepancyDao> },
                { "RuleVerificationList", serviceProvider.GetService<IRuleVerificationListDao> },
                { "RuleViolation", serviceProvider.GetService<IRuleViolationDao> },
                { "SampledFunctionParameterType", serviceProvider.GetService<ISampledFunctionParameterTypeDao> },
                { "ScaleReferenceQuantityValue", serviceProvider.GetService<IScaleReferenceQuantityValueDao> },
                { "ScaleValueDefinition", serviceProvider.GetService<IScaleValueDefinitionDao> },
                { "Section", serviceProvider.GetService<ISectionDao> },
                { "SharedStyle", serviceProvider.GetService<ISharedStyleDao> },
                { "SimpleParameterValue", serviceProvider.GetService<ISimpleParameterValueDao> },
                { "SimpleQuantityKind", serviceProvider.GetService<ISimpleQuantityKindDao> },
                { "SimpleUnit", serviceProvider.GetService<ISimpleUnitDao> },
                { "SiteDirectory", serviceProvider.GetService<ISiteDirectoryDao> },
                { "SiteDirectoryDataAnnotation", serviceProvider.GetService<ISiteDirectoryDataAnnotationDao> },
                { "SiteDirectoryDataDiscussionItem", serviceProvider.GetService<ISiteDirectoryDataDiscussionItemDao> },
                { "SiteDirectoryThingReference", serviceProvider.GetService<ISiteDirectoryThingReferenceDao> },
                { "SiteLogEntry", serviceProvider.GetService<ISiteLogEntryDao> },
                { "SiteReferenceDataLibrary", serviceProvider.GetService<ISiteReferenceDataLibraryDao> },
                { "Solution", serviceProvider.GetService<ISolutionDao> },
                { "SpecializedQuantityKind", serviceProvider.GetService<ISpecializedQuantityKindDao> },
                { "Stakeholder", serviceProvider.GetService<IStakeholderDao> },
                { "StakeholderValue", serviceProvider.GetService<IStakeholderValueDao> },
                { "StakeHolderValueMap", serviceProvider.GetService<IStakeHolderValueMapDao> },
                { "StakeHolderValueMapSettings", serviceProvider.GetService<IStakeHolderValueMapSettingsDao> },
                { "TelephoneNumber", serviceProvider.GetService<ITelephoneNumberDao> },
                { "Term", serviceProvider.GetService<ITermDao> },
                { "TextParameterType", serviceProvider.GetService<ITextParameterTypeDao> },
                { "TextualNote", serviceProvider.GetService<ITextualNoteDao> },
                { "TimeOfDayParameterType", serviceProvider.GetService<ITimeOfDayParameterTypeDao> },
                { "UnitFactor", serviceProvider.GetService<IUnitFactorDao> },
                { "UnitPrefix", serviceProvider.GetService<IUnitPrefixDao> },
                { "UserPreference", serviceProvider.GetService<IUserPreferenceDao> },
                { "UserRuleVerification", serviceProvider.GetService<IUserRuleVerificationDao> },
                { "ValueGroup", serviceProvider.GetService<IValueGroupDao> },
            };
        }

        /// <summary>
        /// Queries an <see cref="IDao" /> based on the <see cref="Type" /> name
        /// </summary>
        /// <param name="typeName">The <see cref="Type" /> name</param>
        /// <returns>The retrieved <see cref="IDao" /></returns>
        public IDao QueryDaoByTypeName(string typeName)
        {
            if (!this.existingDaos.TryGetValue(typeName, out var dao))
            {
                throw new InvalidDataException($"The provided {typeName} Dao could not be retrieved");
            }

            return dao.Invoke();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
