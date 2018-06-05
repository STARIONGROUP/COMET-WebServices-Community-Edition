// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelUtils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA S.A.
// </copyright>
// <summary>
//   This a utility class for Dao functionalities
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A utility class that supplies common functionalities to the Dao layer.
    /// </summary>
    public class DataModelUtils : IDataModelUtils
    {
        /// <summary>
        /// The derived properties per class.
        /// </summary>
        private readonly HashSet<string> derivedProperties = new HashSet<string>
            {
                "ActualFiniteState.Name",
                "ActualFiniteState.ShortName",
                "ActualFiniteState.Owner",
                "ActualFiniteStateList.Name",
                "ActualFiniteStateList.ShortName",
                "ArrayParameterType.NumberOfValues",
                "ArrayParameterType.HasSingleComponentType",
                "ArrayParameterType.Rank",
                "BooleanParameterType.NumberOfValues",
                "BuiltInRuleVerification.Owner",
                "CompoundParameterType.NumberOfValues",
                "DateParameterType.NumberOfValues",
                "DateTimeParameterType.NumberOfValues",
                "DerivedQuantityKind.NumberOfValues",
                "DerivedQuantityKind.AllPossibleScale",
                "DerivedQuantityKind.QuantityDimensionExponent",
                "DerivedQuantityKind.QuantityDimensionExpression",
                "EnumerationParameterType.NumberOfValues",
                "FileRevision.Path",
                "Folder.Path",
                "IdCorrespondence.Owner",
                "NestedElement.Owner",
                "NestedElement.Name",
                "NestedElement.ShortName",
                "NestedParameter.Path",
                "ParameterOverride.ParameterType",
                "ParameterOverride.IsOptionDependent",
                "ParameterOverride.Scale",
                "ParameterOverride.StateDependence",
                "ParameterOverride.Group",
                "ParameterOverrideValueSet.ActualValue",
                "ParameterOverrideValueSet.Owner",
                "ParameterOverrideValueSet.ActualState",
                "ParameterOverrideValueSet.ActualOption",
                "ParameterSubscription.ParameterType",
                "ParameterSubscription.Scale",
                "ParameterSubscription.StateDependence",
                "ParameterSubscription.IsOptionDependent",
                "ParameterSubscription.Group",
                "ParameterSubscriptionValueSet.Computed",
                "ParameterSubscriptionValueSet.Reference",
                "ParameterSubscriptionValueSet.ActualValue",
                "ParameterSubscriptionValueSet.ActualState",
                "ParameterSubscriptionValueSet.ActualOption",
                "ParameterSubscriptionValueSet.Owner",
                "ParameterType.NumberOfValues",
                "ParameterValueSet.ActualValue",
                "ParameterValueSet.Owner",
                "ParameterValueSetBase.ActualValue",
                "ParameterValueSetBase.Owner",
                "ParametricConstraint.Owner",
                "Person.Name",
                "PossibleFiniteState.Owner",
                "PrefixedUnit.ConversionFactor",
                "PrefixedUnit.Name",
                "PrefixedUnit.ShortName",
                "QuantityKind.NumberOfValues",
                "QuantityKind.AllPossibleScale",
                "QuantityKind.QuantityDimensionExponent",
                "QuantityKind.QuantityDimensionExpression",
                "RuleVerification.Owner",
                "ScalarParameterType.NumberOfValues",
                "SimpleParameterValue.Owner",
                "SimpleQuantityKind.NumberOfValues",
                "SimpleQuantityKind.AllPossibleScale",
                "SimpleQuantityKind.QuantityDimensionExponent",
                "SimpleQuantityKind.QuantityDimensionExpression",
                "SpecializedQuantityKind.NumberOfValues",
                "SpecializedQuantityKind.AllPossibleScale",
                "SpecializedQuantityKind.QuantityDimensionExponent",
                "SpecializedQuantityKind.QuantityDimensionExpression",
                "TextParameterType.NumberOfValues",
                "TimeOfDayParameterType.NumberOfValues",
                "UserRuleVerification.Owner",
                "UserRuleVerification.Name",
            };
 
        /// <summary>
        /// The type source partition mapping for each concrete class.
        /// </summary>
        private readonly Dictionary<string, IEnumerable<string>> typePartitionMap = new Dictionary<string, IEnumerable<string>>
            {
                {
                    "SiteDirectory", 
                    new[] 
                        {
                            "ArrayParameterType",
                            "BinaryRelationshipRule",
                            "BooleanParameterType",
                            "Category",
                            "CompoundParameterType",
                            "Constant",
                            "CyclicRatioScale",
                            "DateParameterType",
                            "DateTimeParameterType",
                            "DecompositionRule",
                            "DerivedQuantityKind",
                            "DerivedUnit",
                            "DomainOfExpertise",
                            "DomainOfExpertiseGroup",
                            "EmailAddress",
                            "EngineeringModelSetup",
                            "EnumerationParameterType",
                            "EnumerationValueDefinition",
                            "FileType",
                            "Glossary",
                            "IntervalScale",
                            "IterationSetup",
                            "LinearConversionUnit",
                            "LogarithmicScale",
                            "MappingToReferenceScale",
                            "ModelReferenceDataLibrary",
                            "MultiRelationshipRule",
                            "NaturalLanguage",
                            "OrdinalScale",
                            "Organization",
                            "ParameterizedCategoryRule",
                            "ParameterTypeComponent",
                            "Participant",
                            "ParticipantPermission",
                            "ParticipantRole",
                            "Person",
                            "PersonPermission",
                            "PersonRole",
                            "PrefixedUnit",
                            "QuantityKindFactor",
                            "RatioScale",
                            "ReferencerRule",
                            "ReferenceSource",
                            "ScaleReferenceQuantityValue",
                            "ScaleValueDefinition",
                            "SimpleQuantityKind",
                            "SimpleUnit",
                            "SiteDirectory",
                            "SiteDirectoryDataAnnotation",
                            "SiteDirectoryDataDiscussionItem",
                            "SiteDirectoryThingReference",
                            "SiteLogEntry",
                            "SiteReferenceDataLibrary",
                            "SpecializedQuantityKind",
                            "TelephoneNumber",
                            "Term",
                            "TextParameterType",
                            "TimeOfDayParameterType",
                            "UnitFactor",
                            "UnitPrefix",
                            "UserPreference"
                         }
                },
                {
                    "EngineeringModel", 
                    new[] 
                        {
                            "ActionItem",
                            "Approval",
                            "BinaryNote",
                            "Book",
                            "ChangeProposal",
                            "ChangeRequest",
                            "CommonFileStore",
                            "ContractChangeNotice",
                            "EngineeringModel",
                            "EngineeringModelDataDiscussionItem",
                            "EngineeringModelDataNote",
                            "File",
                            "FileRevision",
                            "Folder",
                            "Iteration",
                            "ModellingThingReference",
                            "ModelLogEntry",
                            "Page",
                            "RequestForDeviation",
                            "RequestForWaiver",
                            "ReviewItemDiscrepancy",
                            "Section",
                            "Solution",
                            "TextualNote"
                         }
                },
                {
                    "Iteration", 
                    new[] 
                        {
                            "ActualFiniteState",
                            "ActualFiniteStateList",
                            "AndExpression",
                            "BinaryRelationship",
                            "Bounds",
                            "BuiltInRuleVerification",
                            "Color",
                            "DiagramCanvas",
                            "DiagramEdge",
                            "DiagramObject",
                            "DomainFileStore",
                            "ElementDefinition",
                            "ElementUsage",
                            "ExclusiveOrExpression",
                            "ExternalIdentifierMap",
                            "File",
                            "FileRevision",
                            "Folder",
                            "Goal",
                            "IdCorrespondence",
                            "MultiRelationship",
                            "NestedElement",
                            "NestedParameter",
                            "NotExpression",
                            "Option",
                            "OrExpression",
                            "OwnedStyle",
                            "Parameter",
                            "ParameterGroup",
                            "ParameterOverride",
                            "ParameterOverrideValueSet",
                            "ParameterSubscription",
                            "ParameterSubscriptionValueSet",
                            "ParameterValueSet",
                            "ParametricConstraint",
                            "Point",
                            "PossibleFiniteState",
                            "PossibleFiniteStateList",
                            "Publication",
                            "RelationalExpression",
                            "RelationshipParameterValue",
                            "Requirement",
                            "RequirementsContainerParameterValue",
                            "RequirementsGroup",
                            "RequirementsSpecification",
                            "RuleVerificationList",
                            "RuleViolation",
                            "SharedStyle",
                            "SimpleParameterValue",
                            "Stakeholder",
                            "StakeholderValue",
                            "StakeHolderValueMap",
                            "StakeHolderValueMapSettings",
                            "UserRuleVerification",
                            "ValueGroup"
                         }
                }
            };

        /// <summary>
        /// Check if the property for the given className is derived.
        /// </summary>
        /// <param name="className">
        /// The class name.
        /// </param>
        /// <param name="property">
        /// The property to check.
        /// </param>
        /// <returns>
        /// True if property is derived.
        /// </returns>
        public bool IsDerived(string className, string property)
        {
            var key = string.Format("{0}.{1}", className, property);
            return this.derivedProperties.Contains(key);
        }

        /// <summary>
        /// Get the source partition for a passed in concrete type.
        /// </summary>
        /// <param name="typeName">
        /// The concrete type name.
        /// </param>
        /// <returns>
        /// The partition name for the passed in concrete type, otherwise null
        /// </returns>
        /// <remarks>
        /// Null is returned if there is no type map entry or there are multiple
        /// </remarks>
        public string GetSourcePartition(string typeName)
        {
            var partitionInfo = this.typePartitionMap.Where(kvp => kvp.Value.Contains(typeName)).Select(x => x.Key).ToList();

            return partitionInfo.Count() == 1 ? partitionInfo.Single() : null;
        }
    }
}
