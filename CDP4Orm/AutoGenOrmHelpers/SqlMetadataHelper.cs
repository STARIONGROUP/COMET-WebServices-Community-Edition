// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlMetadataHelper.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
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
//    Affero General Public License for more details.
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

    using CDP4Orm.Enumeration;

    /// <summary>
    /// The <see cref="SqlMetadataHelper" /> 
    /// </summary>
    public static class SqlMetadataHelper
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> that stores every columns for each table present in the SiteDirectory Schema 
        /// </summary>
        private static readonly Dictionary<string, string> SiteDirectoryColumnMapping = new()
        {
            { "Alias", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ArrayParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ArrayParameterType_Dimension", "\"ArrayParameterType\", \"Dimension\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "BinaryRelationshipRule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"RelationshipCategory\", \"SourceCategory\", \"TargetCategory\"" },
            { "BooleanParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Category", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Category_PermissibleClass", "\"Category\", \"PermissibleClass\", \"ValidFrom\", \"ValidTo\"" },
            { "Category_SuperCategory", "\"Category\", \"SuperCategory\", \"ValidFrom\", \"ValidTo\"" },
            { "Citation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\"" },
            { "CompoundParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Constant", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\"" },
            { "Constant_Category", "\"Constant\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "ConversionBasedUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferenceUnit\"" },
            { "CyclicRatioScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DateParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DateTimeParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DecompositionRule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ContainingCategory\"" },
            { "DecompositionRule_ContainedCategory", "\"DecompositionRule\", \"ContainedCategory\", \"ValidFrom\", \"ValidTo\"" },
            { "DefinedThing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Definition", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Definition_Example", "\"Definition\", \"Example\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "Definition_Note", "\"Definition\", \"Note\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "DependentParameterTypeAssignment", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"MeasurementScale\", \"ParameterType\", \"Sequence\"" },
            { "DerivedQuantityKind", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DerivedUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DiscussionItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\"" },
            { "DomainOfExpertise", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "DomainOfExpertise_Category", "\"DomainOfExpertise\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "DomainOfExpertiseGroup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "DomainOfExpertiseGroup_Domain", "\"DomainOfExpertiseGroup\", \"Domain\", \"ValidFrom\", \"ValidTo\"" },
            { "EmailAddress", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "EngineeringModelSetup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOrganizationalParticipant\"" },
            { "EngineeringModelSetup_ActiveDomain", "\"EngineeringModelSetup\", \"ActiveDomain\", \"ValidFrom\", \"ValidTo\"" },
            { "EnumerationParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "EnumerationValueDefinition", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\"" },
            { "FileType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "FileType_Category", "\"FileType\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "GenericAnnotation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Glossary", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Glossary_Category", "\"Glossary\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "HyperLink", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "IndependentParameterTypeAssignment", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"MeasurementScale\", \"ParameterType\", \"Sequence\"" },
            { "IntervalScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "IterationSetup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SourceIterationSetup\"" },
            { "LinearConversionUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "LogarithmicScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferenceQuantityKind\"" },
            { "LogEntryChangelogItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "LogEntryChangelogItem_AffectedReferenceIid", "\"LogEntryChangelogItem\", \"AffectedReferenceIid\", \"ValidFrom\", \"ValidTo\"" },
            { "MappingToReferenceScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DependentScaleValue\", \"ReferenceScaleValue\"" },
            { "MeasurementScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Unit\"" },
            { "MeasurementUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ModelReferenceDataLibrary", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "MultiRelationshipRule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"RelationshipCategory\"" },
            { "MultiRelationshipRule_RelatedCategory", "\"MultiRelationshipRule\", \"RelatedCategory\", \"ValidFrom\", \"ValidTo\"" },
            { "NaturalLanguage", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "OrdinalScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Organization", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "OrganizationalParticipant", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Organization\"" },
            { "ParameterizedCategoryRule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Category\"" },
            { "ParameterizedCategoryRule_ParameterType", "\"ParameterizedCategoryRule\", \"ParameterType\", \"ValidFrom\", \"ValidTo\"" },
            { "ParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ParameterType_Category", "\"ParameterType\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "ParameterTypeComponent", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\", \"Sequence\"" },
            { "Participant", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Person\", \"Role\", \"SelectedDomain\"" },
            { "Participant_Domain", "\"Participant\", \"Domain\", \"ValidFrom\", \"ValidTo\"" },
            { "ParticipantPermission", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ParticipantRole", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Person", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultDomain\", \"DefaultEmailAddress\", \"DefaultTelephoneNumber\", \"Organization\", \"Role\"" },
            { "PersonPermission", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "PersonRole", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "PrefixedUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Prefix\"" },
            { "QuantityKind", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"DefaultScale\"" },
            { "QuantityKind_PossibleScale", "\"QuantityKind\", \"PossibleScale\", \"ValidFrom\", \"ValidTo\"" },
            { "QuantityKindFactor", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"QuantityKind\", \"Sequence\"" },
            { "RatioScale", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ReferenceDataLibrary", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"RequiredRdl\"" },
            { "ReferenceDataLibrary_BaseQuantityKind", "\"ReferenceDataLibrary\", \"BaseQuantityKind\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "ReferenceDataLibrary_BaseUnit", "\"ReferenceDataLibrary\", \"BaseUnit\", \"ValidFrom\", \"ValidTo\"" },
            { "ReferencerRule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencingCategory\"" },
            { "ReferencerRule_ReferencedCategory", "\"ReferencerRule\", \"ReferencedCategory\", \"ValidFrom\", \"ValidTo\"" },
            { "ReferenceSource", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"PublishedIn\", \"Publisher\"" },
            { "ReferenceSource_Category", "\"ReferenceSource\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Rule", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "SampledFunctionParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ScalarParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ScaleReferenceQuantityValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Scale\"" },
            { "ScaleValueDefinition", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "SimpleQuantityKind", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "SimpleUnit", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "SiteDirectory", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"DefaultParticipantRole\", \"DefaultPersonRole\"" },
            { "SiteDirectoryDataAnnotation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"PrimaryAnnotatedThing\"" },
            { "SiteDirectoryDataDiscussionItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\"" },
            { "SiteDirectoryThingReference", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "SiteLogEntry", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\"" },
            { "SiteLogEntry_AffectedDomainIid", "\"SiteLogEntry\", \"AffectedDomainIid\", \"ValidFrom\", \"ValidTo\"" },
            { "SiteLogEntry_AffectedItemIid", "\"SiteLogEntry\", \"AffectedItemIid\", \"ValidFrom\", \"ValidTo\"" },
            { "SiteLogEntry_Category", "\"SiteLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "SiteReferenceDataLibrary", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "SpecializedQuantityKind", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"General\"" },
            { "TelephoneNumber", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "TelephoneNumber_VcardType", "\"TelephoneNumber\", \"VcardType\", \"ValidFrom\", \"ValidTo\"" },
            { "Term", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "TextParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedDomain", "\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedPerson", "\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\"" },
            { "ThingReference", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\"" },
            { "TimeOfDayParameterType", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "TopContainer", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "UnitFactor", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Unit\", \"Sequence\"" },
            { "UnitPrefix", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "UserPreference", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
        };

        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> that stores every columns for each table present in the EngineeringModel Schema 
        /// </summary>
        private static readonly Dictionary<string, string> EngineeringModelColumnMapping = new()
        {
            { "ActionItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Actionee\"" },
            { "Approval", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\"" },
            { "BinaryNote", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FileType\"" },
            { "Book", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\"" },
            { "Book_Category", "\"Book\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "ChangeProposal", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeRequest\"" },
            { "ChangeRequest", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "CommonFileStore", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ContractChangeNotice", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ChangeProposal\"" },
            { "ContractDeviation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DiscussionItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReplyTo\"" },
            { "EngineeringModel", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"EngineeringModelSetup\"" },
            { "EngineeringModelDataAnnotation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Author\", \"PrimaryAnnotatedThing\"" },
            { "EngineeringModelDataDiscussionItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\"" },
            { "EngineeringModelDataNote", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "File", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\"" },
            { "File_Category", "\"File\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "FileRevision", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\"" },
            { "FileRevision_FileType", "\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "FileStore", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\"" },
            { "Folder", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\"" },
            { "GenericAnnotation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Iteration", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\"" },
            { "LogEntryChangelogItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "LogEntryChangelogItem_AffectedReferenceIid", "\"LogEntryChangelogItem\", \"AffectedReferenceIid\", \"ValidFrom\", \"ValidTo\"" },
            { "ModellingAnnotationItem", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\"" },
            { "ModellingAnnotationItem_Category", "\"ModellingAnnotationItem\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "ModellingAnnotationItem_SourceAnnotation", "\"ModellingAnnotationItem\", \"SourceAnnotation\", \"ValidFrom\", \"ValidTo\"" },
            { "ModellingThingReference", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ModelLogEntry", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\"" },
            { "ModelLogEntry_AffectedDomainIid", "\"ModelLogEntry\", \"AffectedDomainIid\", \"ValidFrom\", \"ValidTo\"" },
            { "ModelLogEntry_AffectedItemIid", "\"ModelLogEntry\", \"AffectedItemIid\", \"ValidFrom\", \"ValidTo\"" },
            { "ModelLogEntry_Category", "\"ModelLogEntry\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Note", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\"" },
            { "Note_Category", "\"Note\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Page", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\"" },
            { "Page_Category", "\"Page\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "RequestForDeviation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "RequestForWaiver", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ReviewItemDiscrepancy", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Section", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\", \"Sequence\"" },
            { "Section_Category", "\"Section\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Solution", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Author\", \"Owner\"" },
            { "TextualNote", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedDomain", "\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedPerson", "\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\"" },
            { "ThingReference", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ReferencedThing\"" },
            { "TopContainer", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
        };

        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> that stores every columns for each table present in the Iteration Schema 
        /// </summary>
        private static readonly Dictionary<string, string> IterationColumnMapping = new()
        {
            { "ActualFiniteState", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ActualFiniteState_PossibleState", "\"ActualFiniteState\", \"PossibleState\", \"ValidFrom\", \"ValidTo\"" },
            { "ActualFiniteStateList", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\"" },
            { "ActualFiniteStateList_ExcludeOption", "\"ActualFiniteStateList\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\"" },
            { "ActualFiniteStateList_PossibleFiniteStateList", "\"ActualFiniteStateList\", \"PossibleFiniteStateList\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "Alias", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "AndExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "AndExpression_Term", "\"AndExpression\", \"Term\", \"ValidFrom\", \"ValidTo\"" },
            { "BinaryRelationship", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\"" },
            { "BooleanExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Bounds", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "BuiltInRuleVerification", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Citation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Source\"" },
            { "Color", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "DefinedThing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Definition", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Definition_Example", "\"Definition\", \"Example\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "Definition_Note", "\"Definition\", \"Note\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "DiagramCanvas", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "DiagramEdge", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Source\", \"Target\"" },
            { "DiagramElementContainer", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DiagramElementThing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DepictedThing\", \"SharedStyle\"" },
            { "DiagrammingStyle", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"FillColor\", \"FontColor\", \"StrokeColor\"" },
            { "DiagramObject", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DiagramShape", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DiagramThingBase", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "DomainFileStore", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ElementBase", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\"" },
            { "ElementBase_Category", "\"ElementBase\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "ElementDefinition", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ElementDefinition_OrganizationalParticipant", "\"ElementDefinition\", \"OrganizationalParticipant\", \"ValidFrom\", \"ValidTo\"" },
            { "ElementDefinition_ReferencedElement", "\"ElementDefinition\", \"ReferencedElement\", \"ValidFrom\", \"ValidTo\"" },
            { "ElementUsage", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ElementDefinition\"" },
            { "ElementUsage_ExcludeOption", "\"ElementUsage\", \"ExcludeOption\", \"ValidFrom\", \"ValidTo\"" },
            { "ExclusiveOrExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ExclusiveOrExpression_Term", "\"ExclusiveOrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\"" },
            { "ExternalIdentifierMap", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ExternalFormat\", \"Owner\"" },
            { "File", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"LockedBy\", \"Owner\"" },
            { "File_Category", "\"File\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "FileRevision", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\"" },
            { "FileRevision_FileType", "\"FileRevision\", \"FileType\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "FileStore", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\"" },
            { "Folder", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingFolder\", \"Creator\", \"Owner\"" },
            { "Goal", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Goal_Category", "\"Goal\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "HyperLink", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "IdCorrespondence", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "MultiRelationship", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "MultiRelationship_RelatedThing", "\"MultiRelationship\", \"RelatedThing\", \"ValidFrom\", \"ValidTo\"" },
            { "NestedElement", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RootElement\"" },
            { "NestedElement_ElementUsage", "\"NestedElement\", \"ElementUsage\", \"ValidFrom\", \"ValidTo\", \"Sequence\"" },
            { "NestedParameter", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ActualState\", \"AssociatedParameter\", \"Owner\"" },
            { "NotExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Term\"" },
            { "Option", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\"" },
            { "Option_Category", "\"Option\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "OrExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "OrExpression_Term", "\"OrExpression\", \"Term\", \"ValidFrom\", \"ValidTo\"" },
            { "OwnedStyle", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Parameter", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"RequestedBy\"" },
            { "ParameterBase", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Group\", \"Owner\", \"ParameterType\", \"Scale\", \"StateDependence\"" },
            { "ParameterGroup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ContainingGroup\"" },
            { "ParameterOrOverrideBase", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "ParameterOverride", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Parameter\"" },
            { "ParameterOverrideValueSet", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterValueSet\"" },
            { "ParameterSubscription", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ParameterSubscriptionValueSet", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"SubscribedValueSet\"" },
            { "ParameterValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\"" },
            { "ParameterValueSet", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ParameterValueSetBase", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ActualOption\", \"ActualState\"" },
            { "ParametricConstraint", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"TopExpression\", \"Sequence\"" },
            { "Point", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\"" },
            { "PossibleFiniteState", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\"" },
            { "PossibleFiniteStateList", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"DefaultState\", \"Owner\"" },
            { "PossibleFiniteStateList_Category", "\"PossibleFiniteStateList\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Publication", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Publication_Domain", "\"Publication\", \"Domain\", \"ValidFrom\", \"ValidTo\"" },
            { "Publication_PublishedParameter", "\"Publication\", \"PublishedParameter\", \"ValidFrom\", \"ValidTo\"" },
            { "RelationalExpression", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"ParameterType\", \"Scale\"" },
            { "Relationship", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\"" },
            { "Relationship_Category", "\"Relationship\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "RelationshipParameterValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Requirement", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Group\"" },
            { "Requirement_Category", "\"Requirement\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "RequirementsContainer", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\"" },
            { "RequirementsContainer_Category", "\"RequirementsContainer\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "RequirementsContainerParameterValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "RequirementsGroup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "RequirementsSpecification", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "RuleVerification", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Sequence\"" },
            { "RuleVerificationList", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"Owner\"" },
            { "RuleViolation", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "RuleViolation_ViolatingThing", "\"RuleViolation\", \"ViolatingThing\", \"ValidFrom\", \"ValidTo\"" },
            { "SharedStyle", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "SimpleParameterizableThing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Owner\"" },
            { "SimpleParameterValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"ParameterType\", \"Scale\"" },
            { "Stakeholder", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "Stakeholder_Category", "\"Stakeholder\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "Stakeholder_StakeholderValue", "\"Stakeholder\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeholderValue", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "StakeholderValue_Category", "\"StakeholderValue\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMap", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "StakeHolderValueMap_Category", "\"StakeHolderValueMap\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMap_Goal", "\"StakeHolderValueMap\", \"Goal\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMap_Requirement", "\"StakeHolderValueMap\", \"Requirement\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMap_StakeholderValue", "\"StakeHolderValueMap\", \"StakeholderValue\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMap_ValueGroup", "\"StakeHolderValueMap\", \"ValueGroup\", \"ValidFrom\", \"ValidTo\"" },
            { "StakeHolderValueMapSettings", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\", \"GoalToValueGroupRelationship\", \"StakeholderValueToRequirementRelationship\", \"ValueGroupToStakeholderValueRelationship\"" },
            { "Thing", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedDomain", "\"Thing\", \"ExcludedDomain\", \"ValidFrom\", \"ValidTo\"" },
            { "Thing_ExcludedPerson", "\"Thing\", \"ExcludedPerson\", \"ValidFrom\", \"ValidTo\"" },
            { "UserRuleVerification", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Rule\"" },
            { "ValueGroup", "\"Iid\", \"ValueTypeDictionary\", \"ValidFrom\", \"ValidTo\", \"Container\"" },
            { "ValueGroup_Category", "\"ValueGroup\", \"Category\", \"ValidFrom\", \"ValidTo\"" },
        };

        /// <summary>
        /// Get all columns present inside the SQL Table, based on a <see cref="SchemaKind" />
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="schema">The <see cref="SchemaKind" /></param>
        /// <returns>All columns name</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="schema" /> is not recognized</exception>
        /// <exception cref="InvalidDataException">The <paramref name="tableName" /> is not found</exception>
        public static string GetTableColumns(string tableName, SchemaKind schema)
        {
            var dictionary = GetDictionaryBySchema(schema);

            if (!dictionary.TryGetValue(tableName, out var columns))
            {
                throw new InvalidDataException($"{tableName} not supported for Schema {schema}");
            }

            return columns;
        }

        /// <summary>
        /// Get all table present inside a Schema
        /// </summary>
        /// <param name="schema">The <see cref="SchemaKind" /></param>
        /// <returns>All tables</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="schema" /> is not recognized</exception>
        public static IReadOnlyCollection<string> GetTables(SchemaKind schema)
        {
            var dictionary = GetDictionaryBySchema(schema);
            return dictionary.Keys;
        }

        /// <summary>
        /// Get the correct <see cref="Dictionary{TKey, TValue}" /> for a <see cref="SchemaKind" />
        /// </summary>
        /// <param name="schema">The <see cref="SchemaKind" /></param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}" /></returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="schema" /> is not recognized</exception>
        private static Dictionary<string, string> GetDictionaryBySchema(SchemaKind schema)
        {
            return schema switch
            {
                SchemaKind.SiteDirectory => SiteDirectoryColumnMapping,
                SchemaKind.EngineeringModel => EngineeringModelColumnMapping,
                SchemaKind.Iteration => IterationColumnMapping,
                _ => throw new ArgumentOutOfRangeException($"Unknown SchemaKind {schema}")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
