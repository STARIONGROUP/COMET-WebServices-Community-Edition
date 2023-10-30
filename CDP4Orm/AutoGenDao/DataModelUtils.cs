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
                { "ActualFiniteState.Name" },
                { "ActualFiniteState.Owner" },
                { "ActualFiniteState.ShortName" },
                { "ActualFiniteStateList.Name" },
                { "ActualFiniteStateList.ShortName" },
                { "ArrayParameterType.HasSingleComponentType" },
                { "ArrayParameterType.NumberOfValues" },
                { "ArrayParameterType.Rank" },
                { "BooleanParameterType.NumberOfValues" },
                { "BuiltInRuleVerification.Owner" },
                { "CompoundParameterType.NumberOfValues" },
                { "DateParameterType.NumberOfValues" },
                { "DateTimeParameterType.NumberOfValues" },
                { "DerivedQuantityKind.AllPossibleScale" },
                { "DerivedQuantityKind.NumberOfValues" },
                { "DerivedQuantityKind.QuantityDimensionExponent" },
                { "DerivedQuantityKind.QuantityDimensionExpression" },
                { "EnumerationParameterType.NumberOfValues" },
                { "FileRevision.Path" },
                { "Folder.Path" },
                { "IdCorrespondence.Owner" },
                { "NestedElement.Name" },
                { "NestedElement.Owner" },
                { "NestedElement.ShortName" },
                { "NestedParameter.Path" },
                { "ParameterOverride.Group" },
                { "ParameterOverride.IsOptionDependent" },
                { "ParameterOverride.ParameterType" },
                { "ParameterOverride.Scale" },
                { "ParameterOverride.StateDependence" },
                { "ParameterOverrideValueSet.ActualOption" },
                { "ParameterOverrideValueSet.ActualState" },
                { "ParameterOverrideValueSet.ActualValue" },
                { "ParameterOverrideValueSet.Owner" },
                { "ParameterSubscription.Group" },
                { "ParameterSubscription.IsOptionDependent" },
                { "ParameterSubscription.ParameterType" },
                { "ParameterSubscription.Scale" },
                { "ParameterSubscription.StateDependence" },
                { "ParameterSubscriptionValueSet.ActualOption" },
                { "ParameterSubscriptionValueSet.ActualState" },
                { "ParameterSubscriptionValueSet.ActualValue" },
                { "ParameterSubscriptionValueSet.Computed" },
                { "ParameterSubscriptionValueSet.Owner" },
                { "ParameterSubscriptionValueSet.Reference" },
                { "ParameterType.NumberOfValues" },
                { "ParameterValueSet.ActualValue" },
                { "ParameterValueSet.Owner" },
                { "ParameterValueSetBase.ActualValue" },
                { "ParameterValueSetBase.Owner" },
                { "ParametricConstraint.Owner" },
                { "Person.Name" },
                { "PossibleFiniteState.Owner" },
                { "PrefixedUnit.ConversionFactor" },
                { "PrefixedUnit.Name" },
                { "PrefixedUnit.ShortName" },
                { "QuantityKind.AllPossibleScale" },
                { "QuantityKind.NumberOfValues" },
                { "QuantityKind.QuantityDimensionExponent" },
                { "QuantityKind.QuantityDimensionExpression" },
                { "RuleVerification.Owner" },
                { "SampledFunctionParameterType.NumberOfValues" },
                { "ScalarParameterType.NumberOfValues" },
                { "SimpleParameterValue.Owner" },
                { "SimpleQuantityKind.AllPossibleScale" },
                { "SimpleQuantityKind.NumberOfValues" },
                { "SimpleQuantityKind.QuantityDimensionExponent" },
                { "SimpleQuantityKind.QuantityDimensionExpression" },
                { "SpecializedQuantityKind.AllPossibleScale" },
                { "SpecializedQuantityKind.NumberOfValues" },
                { "SpecializedQuantityKind.QuantityDimensionExponent" },
                { "SpecializedQuantityKind.QuantityDimensionExpression" },
                { "TextParameterType.NumberOfValues" },
                { "TimeOfDayParameterType.NumberOfValues" },
                { "UserRuleVerification.Name" },
                { "UserRuleVerification.Owner" },
            };

        /// <summary>
        /// The type source partition mapping for each concrete class.
        /// </summary>
        private readonly Dictionary<string, IEnumerable<string>> typePartitionMap = new Dictionary<string, IEnumerable<string>>
            {

                {
                    "EngineeringModel",
                    new []
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
                        "LogEntryChangelogItem",
                        "ModellingAnnotationItem",
                        "ModellingThingReference",
                        "ModelLogEntry",
                        "Note",
                        "Page",
                        "RequestForDeviation",
                        "RequestForWaiver",
                        "ReviewItemDiscrepancy",
                        "Section",
                        "Solution",
                        "TextualNote",
                        }
                },

                {
                    "Iteration",
                    new []
                        {
                        "ActualFiniteState",
                        "ActualFiniteStateList",
                        "Alias",
                        "AndExpression",
                        "BinaryRelationship",
                        "BooleanExpression",
                        "Bounds",
                        "BuiltInRuleVerification",
                        "Citation",
                        "Color",
                        "Definition",
                        "DiagramCanvas",
                        "DiagramEdge",
                        "DiagramElementThing",
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
                        "HyperLink",
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
                        "Relationship",
                        "RelationshipParameterValue",
                        "Requirement",
                        "RequirementsContainerParameterValue",
                        "RequirementsGroup",
                        "RequirementsSpecification",
                        "RuleVerification",
                        "RuleVerificationList",
                        "RuleViolation",
                        "SharedStyle",
                        "SimpleParameterValue",
                        "Stakeholder",
                        "StakeholderValue",
                        "StakeHolderValueMap",
                        "StakeHolderValueMapSettings",
                        "UserRuleVerification",
                        "ValueGroup",
                        }
                },

                {
                    "SiteDirectory",
                    new []
                        {
                        "Alias",
                        "ArrayParameterType",
                        "BinaryRelationshipRule",
                        "BooleanParameterType",
                        "Category",
                        "Citation",
                        "CompoundParameterType",
                        "Constant",
                        "CyclicRatioScale",
                        "DateParameterType",
                        "DateTimeParameterType",
                        "DecompositionRule",
                        "Definition",
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
                        "HyperLink",
                        "IndependentParameterTypeAssignment",
                        "IntervalScale",
                        "IterationSetup",
                        "LinearConversionUnit",
                        "LogarithmicScale",
                        "LogEntryChangelogItem",
                        "MappingToReferenceScale",
                        "MeasurementScale",
                        "MeasurementUnit",
                        "ModelReferenceDataLibrary",
                        "MultiRelationshipRule",
                        "NaturalLanguage",
                        "OrdinalScale",
                        "Organization",
                        "OrganizationalParticipant",
                        "ParameterizedCategoryRule",
                        "ParameterType",
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
                        "Rule",
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
                        "UserPreference",
                        }
                },
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
        /// A collection of possible partitions as string
        /// </remarks>
        public IList<string> GetSourcePartition(string typeName)
        {
            var partitionInfo = this.typePartitionMap.Where(kvp => kvp.Value.Contains(typeName)).Select(x => x.Key).ToList();

            return partitionInfo;
        }
    }
}
