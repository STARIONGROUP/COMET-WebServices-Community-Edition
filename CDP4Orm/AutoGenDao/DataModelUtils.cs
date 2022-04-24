// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
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
                "SampledFunctionParameterType.NumberOfValues",
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
                            "DependentParameterTypeAssignment",
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
                            "IndependentParameterTypeAssignment",
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
                            "OrganizationalParticipant",
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
                            "SampledFunctionParameterType",
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
