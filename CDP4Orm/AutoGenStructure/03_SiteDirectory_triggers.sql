--------------------------------------------------------------------------------------------------------
-- Copyright RHEA System S.A.                                                                         --
--                                                                                                    --
-- Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Théate Antoine      --
--                                                                                                    --
-- This file is part of COMET Server Community Edition. The COMET Server Community Edition is the     --
-- RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.                                        --
--                                                                                                    --
-- The COMET Server Community Edition is free software; you can redistribute it and/or modify it      --
-- the terms of the GNU Affero General Public License as published by the Free Software Foundation;   --
-- either version 3 of the License, or (at your option) any later version.                            --
--                                                                                                    --
-- The COMET Server Community Edition is distributed in the hope that it will be useful, but WITHOUT  --
-- ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     --
-- PURPOSE. See the GNU Affero General Public License for more details.                               --
--                                                                                                    --
-- You should have received a copy of the GNU Affero General Public License                           --
-- along with this program.  If not, see <http://www.gnu.org/licenses/>.                              --
--                                                                                                    --
-- This a generated file using version 1.1.27 of the COMET Master Model                               --
--------------------------------------------------------------------------------------------------------

--------------------------------------------------------------------------------------------------------
-------------------------- Classes - Revision Trigger (apply_revision) ---------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Class Revision Trigger
CREATE TRIGGER alias_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Alias"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ArrayParameterType - Class Revision Trigger

-- Attachment - Class Revision Trigger
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- BinaryRelationshipRule - Class Revision Trigger

-- BooleanParameterType - Class Revision Trigger

-- Category - Class Revision Trigger
CREATE TRIGGER category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Citation - Class Revision Trigger
CREATE TRIGGER citation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Citation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- CompoundParameterType - Class Revision Trigger

-- Constant - Class Revision Trigger
CREATE TRIGGER constant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Constant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ConversionBasedUnit - Class Revision Trigger

-- CyclicRatioScale - Class Revision Trigger

-- DateParameterType - Class Revision Trigger

-- DateTimeParameterType - Class Revision Trigger

-- DecompositionRule - Class Revision Trigger

-- DefinedThing - Class Revision Trigger

-- Definition - Class Revision Trigger
CREATE TRIGGER definition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- DependentParameterTypeAssignment - Class Revision Trigger
CREATE TRIGGER dependentparametertypeassignment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- DerivedQuantityKind - Class Revision Trigger

-- DerivedUnit - Class Revision Trigger

-- DiscussionItem - Class Revision Trigger

-- DomainOfExpertise - Class Revision Trigger
CREATE TRIGGER domainofexpertise_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- DomainOfExpertiseGroup - Class Revision Trigger
CREATE TRIGGER domainofexpertisegroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- EmailAddress - Class Revision Trigger
CREATE TRIGGER emailaddress_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- EngineeringModelSetup - Class Revision Trigger
CREATE TRIGGER engineeringmodelsetup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- EnumerationParameterType - Class Revision Trigger

-- EnumerationValueDefinition - Class Revision Trigger
CREATE TRIGGER enumerationvaluedefinition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- FileType - Class Revision Trigger
CREATE TRIGGER filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- GenericAnnotation - Class Revision Trigger

-- Glossary - Class Revision Trigger
CREATE TRIGGER glossary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Glossary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- HyperLink - Class Revision Trigger
CREATE TRIGGER hyperlink_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."HyperLink"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- IndependentParameterTypeAssignment - Class Revision Trigger
CREATE TRIGGER independentparametertypeassignment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- IntervalScale - Class Revision Trigger

-- IterationSetup - Class Revision Trigger
CREATE TRIGGER iterationsetup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- LinearConversionUnit - Class Revision Trigger

-- LogarithmicScale - Class Revision Trigger

-- LogEntryChangelogItem - Class Revision Trigger
CREATE TRIGGER logentrychangelogitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- MappingToReferenceScale - Class Revision Trigger
CREATE TRIGGER mappingtoreferencescale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- MeasurementScale - Class Revision Trigger
CREATE TRIGGER measurementscale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- MeasurementUnit - Class Revision Trigger
CREATE TRIGGER measurementunit_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ModelReferenceDataLibrary - Class Revision Trigger
CREATE TRIGGER modelreferencedatalibrary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- MultiRelationshipRule - Class Revision Trigger

-- NaturalLanguage - Class Revision Trigger
CREATE TRIGGER naturallanguage_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- OrdinalScale - Class Revision Trigger

-- Organization - Class Revision Trigger
CREATE TRIGGER organization_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Organization"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- OrganizationalParticipant - Class Revision Trigger
CREATE TRIGGER organizationalparticipant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ParameterizedCategoryRule - Class Revision Trigger

-- ParameterType - Class Revision Trigger
CREATE TRIGGER parametertype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ParameterTypeComponent - Class Revision Trigger
CREATE TRIGGER parametertypecomponent_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Participant - Class Revision Trigger
CREATE TRIGGER participant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Participant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ParticipantPermission - Class Revision Trigger
CREATE TRIGGER participantpermission_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ParticipantRole - Class Revision Trigger
CREATE TRIGGER participantrole_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Person - Class Revision Trigger
CREATE TRIGGER person_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Person"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- PersonPermission - Class Revision Trigger
CREATE TRIGGER personpermission_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- PersonRole - Class Revision Trigger
CREATE TRIGGER personrole_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."PersonRole"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- PrefixedUnit - Class Revision Trigger

-- QuantityKind - Class Revision Trigger

-- QuantityKindFactor - Class Revision Trigger
CREATE TRIGGER quantitykindfactor_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- RatioScale - Class Revision Trigger

-- ReferenceDataLibrary - Class Revision Trigger

-- ReferencerRule - Class Revision Trigger

-- ReferenceSource - Class Revision Trigger
CREATE TRIGGER referencesource_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Rule - Class Revision Trigger
CREATE TRIGGER rule_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Rule"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SampledFunctionParameterType - Class Revision Trigger

-- ScalarParameterType - Class Revision Trigger

-- ScaleReferenceQuantityValue - Class Revision Trigger
CREATE TRIGGER scalereferencequantityvalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- ScaleValueDefinition - Class Revision Trigger
CREATE TRIGGER scalevaluedefinition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SimpleQuantityKind - Class Revision Trigger

-- SimpleUnit - Class Revision Trigger

-- SiteDirectory - Class Revision Trigger

-- SiteDirectoryDataAnnotation - Class Revision Trigger
CREATE TRIGGER sitedirectorydataannotation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SiteDirectoryDataDiscussionItem - Class Revision Trigger
CREATE TRIGGER sitedirectorydatadiscussionitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SiteDirectoryThingReference - Class Revision Trigger
CREATE TRIGGER sitedirectorythingreference_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SiteLogEntry - Class Revision Trigger
CREATE TRIGGER sitelogentry_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SiteReferenceDataLibrary - Class Revision Trigger
CREATE TRIGGER sitereferencedatalibrary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- SpecializedQuantityKind - Class Revision Trigger

-- TelephoneNumber - Class Revision Trigger
CREATE TRIGGER telephonenumber_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Term - Class Revision Trigger
CREATE TRIGGER term_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Term"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- TextParameterType - Class Revision Trigger

-- Thing - Class Revision Trigger
CREATE TRIGGER thing_apply_revision
  BEFORE INSERT 
  ON "SiteDirectory"."Thing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Iid', 'SiteDirectory', 'SiteDirectory');

-- ThingReference - Class Revision Trigger

-- TimeOfDayParameterType - Class Revision Trigger

-- TopContainer - Class Revision Trigger

-- UnitFactor - Class Revision Trigger
CREATE TRIGGER unitfactor_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- UnitPrefix - Class Revision Trigger
CREATE TRIGGER unitprefix_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- UserPreference - Class Revision Trigger
CREATE TRIGGER userpreference_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UserPreference"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

--------------------------------------------------------------------------------------------------------
-------------------------- Reference Properties - Revision Trigger (apply_revision) --------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Reference Property Revision Trigger

-- ArrayParameterType - Reference Property Revision Trigger
-- ArrayParameterType.Dimension
CREATE TRIGGER arrayparametertype_dimension_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ArrayParameterType', 'SiteDirectory');

-- Attachment - Reference Property Revision Trigger
-- Attachment.FileType
CREATE TRIGGER attachment_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Attachment', 'SiteDirectory');

-- BinaryRelationshipRule - Reference Property Revision Trigger

-- BooleanParameterType - Reference Property Revision Trigger

-- Category - Reference Property Revision Trigger
-- Category.PermissibleClass
CREATE TRIGGER category_permissibleclass_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Category', 'SiteDirectory');

-- Category.SuperCategory
CREATE TRIGGER category_supercategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Category', 'SiteDirectory');

-- Citation - Reference Property Revision Trigger

-- CompoundParameterType - Reference Property Revision Trigger

-- Constant - Reference Property Revision Trigger
-- Constant.Category
CREATE TRIGGER constant_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Constant', 'SiteDirectory');

-- ConversionBasedUnit - Reference Property Revision Trigger

-- CyclicRatioScale - Reference Property Revision Trigger

-- DateParameterType - Reference Property Revision Trigger

-- DateTimeParameterType - Reference Property Revision Trigger

-- DecompositionRule - Reference Property Revision Trigger
-- DecompositionRule.ContainedCategory
CREATE TRIGGER decompositionrule_containedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DecompositionRule', 'SiteDirectory');

-- DefinedThing - Reference Property Revision Trigger

-- Definition - Reference Property Revision Trigger
-- Definition.Example
CREATE TRIGGER definition_example_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'SiteDirectory');

-- Definition.Note
CREATE TRIGGER definition_note_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'SiteDirectory');

-- DependentParameterTypeAssignment - Reference Property Revision Trigger

-- DerivedQuantityKind - Reference Property Revision Trigger

-- DerivedUnit - Reference Property Revision Trigger

-- DiscussionItem - Reference Property Revision Trigger

-- DomainOfExpertise - Reference Property Revision Trigger
-- DomainOfExpertise.Category
CREATE TRIGGER domainofexpertise_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DomainOfExpertise', 'SiteDirectory');

-- DomainOfExpertiseGroup - Reference Property Revision Trigger
-- DomainOfExpertiseGroup.Domain
CREATE TRIGGER domainofexpertisegroup_domain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DomainOfExpertiseGroup', 'SiteDirectory');

-- EmailAddress - Reference Property Revision Trigger

-- EngineeringModelSetup - Reference Property Revision Trigger
-- EngineeringModelSetup.ActiveDomain
CREATE TRIGGER engineeringmodelsetup_activedomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('EngineeringModelSetup', 'SiteDirectory');

-- EnumerationParameterType - Reference Property Revision Trigger

-- EnumerationValueDefinition - Reference Property Revision Trigger

-- FileType - Reference Property Revision Trigger
-- FileType.Category
CREATE TRIGGER filetype_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('FileType', 'SiteDirectory');

-- GenericAnnotation - Reference Property Revision Trigger

-- Glossary - Reference Property Revision Trigger
-- Glossary.Category
CREATE TRIGGER glossary_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Glossary', 'SiteDirectory');

-- HyperLink - Reference Property Revision Trigger

-- IndependentParameterTypeAssignment - Reference Property Revision Trigger

-- IntervalScale - Reference Property Revision Trigger

-- IterationSetup - Reference Property Revision Trigger

-- LinearConversionUnit - Reference Property Revision Trigger

-- LogarithmicScale - Reference Property Revision Trigger

-- LogEntryChangelogItem - Reference Property Revision Trigger
-- LogEntryChangelogItem.AffectedReferenceIid
CREATE TRIGGER logentrychangelogitem_affectedreferenceiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('LogEntryChangelogItem', 'SiteDirectory');

-- MappingToReferenceScale - Reference Property Revision Trigger

-- MeasurementScale - Reference Property Revision Trigger

-- MeasurementUnit - Reference Property Revision Trigger

-- ModelReferenceDataLibrary - Reference Property Revision Trigger

-- MultiRelationshipRule - Reference Property Revision Trigger
-- MultiRelationshipRule.RelatedCategory
CREATE TRIGGER multirelationshiprule_relatedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('MultiRelationshipRule', 'SiteDirectory');

-- NaturalLanguage - Reference Property Revision Trigger

-- OrdinalScale - Reference Property Revision Trigger

-- Organization - Reference Property Revision Trigger

-- OrganizationalParticipant - Reference Property Revision Trigger

-- ParameterizedCategoryRule - Reference Property Revision Trigger
-- ParameterizedCategoryRule.ParameterType
CREATE TRIGGER parameterizedcategoryrule_parametertype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ParameterizedCategoryRule', 'SiteDirectory');

-- ParameterType - Reference Property Revision Trigger
-- ParameterType.Category
CREATE TRIGGER parametertype_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ParameterType', 'SiteDirectory');

-- ParameterTypeComponent - Reference Property Revision Trigger

-- Participant - Reference Property Revision Trigger
-- Participant.Domain
CREATE TRIGGER participant_domain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Participant', 'SiteDirectory');

-- ParticipantPermission - Reference Property Revision Trigger

-- ParticipantRole - Reference Property Revision Trigger

-- Person - Reference Property Revision Trigger

-- PersonPermission - Reference Property Revision Trigger

-- PersonRole - Reference Property Revision Trigger

-- PrefixedUnit - Reference Property Revision Trigger

-- QuantityKind - Reference Property Revision Trigger
-- QuantityKind.PossibleScale
CREATE TRIGGER quantitykind_possiblescale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('QuantityKind', 'SiteDirectory');

-- QuantityKindFactor - Reference Property Revision Trigger

-- RatioScale - Reference Property Revision Trigger

-- ReferenceDataLibrary - Reference Property Revision Trigger
-- ReferenceDataLibrary.BaseQuantityKind
CREATE TRIGGER referencedatalibrary_basequantitykind_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceDataLibrary', 'SiteDirectory');

-- ReferenceDataLibrary.BaseUnit
CREATE TRIGGER referencedatalibrary_baseunit_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceDataLibrary', 'SiteDirectory');

-- ReferencerRule - Reference Property Revision Trigger
-- ReferencerRule.ReferencedCategory
CREATE TRIGGER referencerrule_referencedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferencerRule', 'SiteDirectory');

-- ReferenceSource - Reference Property Revision Trigger
-- ReferenceSource.Category
CREATE TRIGGER referencesource_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceSource', 'SiteDirectory');

-- Rule - Reference Property Revision Trigger

-- SampledFunctionParameterType - Reference Property Revision Trigger

-- ScalarParameterType - Reference Property Revision Trigger

-- ScaleReferenceQuantityValue - Reference Property Revision Trigger

-- ScaleValueDefinition - Reference Property Revision Trigger

-- SimpleQuantityKind - Reference Property Revision Trigger

-- SimpleUnit - Reference Property Revision Trigger

-- SiteDirectory - Reference Property Revision Trigger

-- SiteDirectoryDataAnnotation - Reference Property Revision Trigger

-- SiteDirectoryDataDiscussionItem - Reference Property Revision Trigger

-- SiteDirectoryThingReference - Reference Property Revision Trigger

-- SiteLogEntry - Reference Property Revision Trigger
-- SiteLogEntry.AffectedDomainIid
CREATE TRIGGER sitelogentry_affecteddomainiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');

-- SiteLogEntry.AffectedItemIid
CREATE TRIGGER sitelogentry_affecteditemiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');

-- SiteLogEntry.Category
CREATE TRIGGER sitelogentry_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');

-- SiteReferenceDataLibrary - Reference Property Revision Trigger

-- SpecializedQuantityKind - Reference Property Revision Trigger

-- TelephoneNumber - Reference Property Revision Trigger
-- TelephoneNumber.VcardType
CREATE TRIGGER telephonenumber_vcardtype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('TelephoneNumber', 'SiteDirectory');

-- Term - Reference Property Revision Trigger

-- TextParameterType - Reference Property Revision Trigger

-- Thing - Reference Property Revision Trigger
-- Thing.ExcludedDomain
CREATE TRIGGER thing_excludeddomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'SiteDirectory');

-- Thing.ExcludedPerson
CREATE TRIGGER thing_excludedperson_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'SiteDirectory');

-- ThingReference - Reference Property Revision Trigger

-- TimeOfDayParameterType - Reference Property Revision Trigger

-- TopContainer - Reference Property Revision Trigger

-- UnitFactor - Reference Property Revision Trigger

-- UnitPrefix - Reference Property Revision Trigger

-- UserPreference - Reference Property Revision Trigger

--------------------------------------------------------------------------------------------------------
--------------------------------------------- Class Audit triggers -------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Class Audit Triggers
CREATE TRIGGER Alias_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Alias_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ArrayParameterType - Class Audit Triggers
CREATE TRIGGER ArrayParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ArrayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArrayParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ArrayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Attachment - Class Audit Triggers
CREATE TRIGGER Attachment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BinaryRelationshipRule - Class Audit Triggers
CREATE TRIGGER BinaryRelationshipRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."BinaryRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BinaryRelationshipRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."BinaryRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BooleanParameterType - Class Audit Triggers
CREATE TRIGGER BooleanParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."BooleanParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BooleanParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."BooleanParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Category - Class Audit Triggers
CREATE TRIGGER Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Citation - Class Audit Triggers
CREATE TRIGGER Citation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Citation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- CompoundParameterType - Class Audit Triggers
CREATE TRIGGER CompoundParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."CompoundParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER CompoundParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."CompoundParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Constant - Class Audit Triggers
CREATE TRIGGER Constant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Constant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Constant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Constant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ConversionBasedUnit - Class Audit Triggers
CREATE TRIGGER ConversionBasedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ConversionBasedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ConversionBasedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ConversionBasedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- CyclicRatioScale - Class Audit Triggers
CREATE TRIGGER CyclicRatioScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."CyclicRatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER CyclicRatioScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."CyclicRatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DateParameterType - Class Audit Triggers
CREATE TRIGGER DateParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DateParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DateParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DateParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DateTimeParameterType - Class Audit Triggers
CREATE TRIGGER DateTimeParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DateTimeParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DateTimeParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DateTimeParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DecompositionRule - Class Audit Triggers
CREATE TRIGGER DecompositionRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DecompositionRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DecompositionRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DecompositionRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DefinedThing - Class Audit Triggers
CREATE TRIGGER DefinedThing_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DefinedThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Definition - Class Audit Triggers
CREATE TRIGGER Definition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DependentParameterTypeAssignment - Class Audit Triggers
CREATE TRIGGER DependentParameterTypeAssignment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DependentParameterTypeAssignment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DerivedQuantityKind - Class Audit Triggers
CREATE TRIGGER DerivedQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DerivedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DerivedQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DerivedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DerivedUnit - Class Audit Triggers
CREATE TRIGGER DerivedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DerivedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DerivedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DerivedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiscussionItem - Class Audit Triggers
CREATE TRIGGER DiscussionItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DomainOfExpertise - Class Audit Triggers
CREATE TRIGGER DomainOfExpertise_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertise_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DomainOfExpertiseGroup - Class Audit Triggers
CREATE TRIGGER DomainOfExpertiseGroup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertiseGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EmailAddress - Class Audit Triggers
CREATE TRIGGER EmailAddress_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EmailAddress_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EngineeringModelSetup - Class Audit Triggers
CREATE TRIGGER EngineeringModelSetup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelSetup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EnumerationParameterType - Class Audit Triggers
CREATE TRIGGER EnumerationParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EnumerationParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EnumerationParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EnumerationParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EnumerationValueDefinition - Class Audit Triggers
CREATE TRIGGER EnumerationValueDefinition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EnumerationValueDefinition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileType - Class Audit Triggers
CREATE TRIGGER FileType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- GenericAnnotation - Class Audit Triggers
CREATE TRIGGER GenericAnnotation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER GenericAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Glossary - Class Audit Triggers
CREATE TRIGGER Glossary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Glossary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Glossary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Glossary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- HyperLink - Class Audit Triggers
CREATE TRIGGER HyperLink_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER HyperLink_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- IndependentParameterTypeAssignment - Class Audit Triggers
CREATE TRIGGER IndependentParameterTypeAssignment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IndependentParameterTypeAssignment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- IntervalScale - Class Audit Triggers
CREATE TRIGGER IntervalScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IntervalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IntervalScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IntervalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- IterationSetup - Class Audit Triggers
CREATE TRIGGER IterationSetup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IterationSetup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- LinearConversionUnit - Class Audit Triggers
CREATE TRIGGER LinearConversionUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LinearConversionUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LinearConversionUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LinearConversionUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- LogarithmicScale - Class Audit Triggers
CREATE TRIGGER LogarithmicScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogarithmicScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogarithmicScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogarithmicScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- LogEntryChangelogItem - Class Audit Triggers
CREATE TRIGGER LogEntryChangelogItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MappingToReferenceScale - Class Audit Triggers
CREATE TRIGGER MappingToReferenceScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MappingToReferenceScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MeasurementScale - Class Audit Triggers
CREATE TRIGGER MeasurementScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MeasurementScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MeasurementUnit - Class Audit Triggers
CREATE TRIGGER MeasurementUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MeasurementUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModelReferenceDataLibrary - Class Audit Triggers
CREATE TRIGGER ModelReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MultiRelationshipRule - Class Audit Triggers
CREATE TRIGGER MultiRelationshipRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MultiRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationshipRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MultiRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NaturalLanguage - Class Audit Triggers
CREATE TRIGGER NaturalLanguage_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NaturalLanguage_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OrdinalScale - Class Audit Triggers
CREATE TRIGGER OrdinalScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."OrdinalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrdinalScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."OrdinalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Organization - Class Audit Triggers
CREATE TRIGGER Organization_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Organization"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Organization_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Organization"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OrganizationalParticipant - Class Audit Triggers
CREATE TRIGGER OrganizationalParticipant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrganizationalParticipant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterizedCategoryRule - Class Audit Triggers
CREATE TRIGGER ParameterizedCategoryRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterizedCategoryRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterizedCategoryRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterizedCategoryRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterType - Class Audit Triggers
CREATE TRIGGER ParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterTypeComponent - Class Audit Triggers
CREATE TRIGGER ParameterTypeComponent_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterTypeComponent_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Participant - Class Audit Triggers
CREATE TRIGGER Participant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Participant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Participant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Participant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParticipantPermission - Class Audit Triggers
CREATE TRIGGER ParticipantPermission_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParticipantPermission_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParticipantRole - Class Audit Triggers
CREATE TRIGGER ParticipantRole_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParticipantRole_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Person - Class Audit Triggers
CREATE TRIGGER Person_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Person"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Person_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Person"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- PersonPermission - Class Audit Triggers
CREATE TRIGGER PersonPermission_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PersonPermission_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- PersonRole - Class Audit Triggers
CREATE TRIGGER PersonRole_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PersonRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PersonRole_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PersonRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- PrefixedUnit - Class Audit Triggers
CREATE TRIGGER PrefixedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PrefixedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PrefixedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PrefixedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- QuantityKind - Class Audit Triggers
CREATE TRIGGER QuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- QuantityKindFactor - Class Audit Triggers
CREATE TRIGGER QuantityKindFactor_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKindFactor_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RatioScale - Class Audit Triggers
CREATE TRIGGER RatioScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."RatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RatioScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."RatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferenceDataLibrary - Class Audit Triggers
CREATE TRIGGER ReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferencerRule - Class Audit Triggers
CREATE TRIGGER ReferencerRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferencerRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferencerRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferencerRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferenceSource - Class Audit Triggers
CREATE TRIGGER ReferenceSource_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceSource_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Rule - Class Audit Triggers
CREATE TRIGGER Rule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Rule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Rule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Rule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SampledFunctionParameterType - Class Audit Triggers
CREATE TRIGGER SampledFunctionParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SampledFunctionParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SampledFunctionParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SampledFunctionParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ScalarParameterType - Class Audit Triggers
CREATE TRIGGER ScalarParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScalarParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScalarParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScalarParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ScaleReferenceQuantityValue - Class Audit Triggers
CREATE TRIGGER ScaleReferenceQuantityValue_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScaleReferenceQuantityValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ScaleValueDefinition - Class Audit Triggers
CREATE TRIGGER ScaleValueDefinition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScaleValueDefinition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SimpleQuantityKind - Class Audit Triggers
CREATE TRIGGER SimpleQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SimpleQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SimpleQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SimpleUnit - Class Audit Triggers
CREATE TRIGGER SimpleUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SimpleUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SimpleUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteDirectory - Class Audit Triggers
CREATE TRIGGER SiteDirectory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteDirectoryDataAnnotation - Class Audit Triggers
CREATE TRIGGER SiteDirectoryDataAnnotation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryDataAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteDirectoryDataDiscussionItem - Class Audit Triggers
CREATE TRIGGER SiteDirectoryDataDiscussionItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryDataDiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteDirectoryThingReference - Class Audit Triggers
CREATE TRIGGER SiteDirectoryThingReference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteLogEntry - Class Audit Triggers
CREATE TRIGGER SiteLogEntry_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteReferenceDataLibrary - Class Audit Triggers
CREATE TRIGGER SiteReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SpecializedQuantityKind - Class Audit Triggers
CREATE TRIGGER SpecializedQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SpecializedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SpecializedQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SpecializedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TelephoneNumber - Class Audit Triggers
CREATE TRIGGER TelephoneNumber_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TelephoneNumber_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Term - Class Audit Triggers
CREATE TRIGGER Term_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Term_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TextParameterType - Class Audit Triggers
CREATE TRIGGER TextParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TextParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TextParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TextParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing - Class Audit Triggers
CREATE TRIGGER Thing_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ThingReference - Class Audit Triggers
CREATE TRIGGER ThingReference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TimeOfDayParameterType - Class Audit Triggers
CREATE TRIGGER TimeOfDayParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TimeOfDayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TimeOfDayParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TimeOfDayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TopContainer - Class Audit Triggers
CREATE TRIGGER TopContainer_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TopContainer_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- UnitFactor - Class Audit Triggers
CREATE TRIGGER UnitFactor_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UnitFactor_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- UnitPrefix - Class Audit Triggers
CREATE TRIGGER UnitPrefix_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UnitPrefix_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- UserPreference - Class Audit Triggers
CREATE TRIGGER UserPreference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UserPreference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UserPreference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UserPreference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

--------------------------------------------------------------------------------------------------------
-------------------------------- Reference Property Audit triggers -------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Reference Property Audit Triggers

-- ArrayParameterType - Reference Property Audit Triggers
-- ArrayParameterType.Dimension
CREATE TRIGGER ArrayParameterType_Dimension_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArrayParameterType_Dimension_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Attachment - Reference Property Audit Triggers
-- Attachment.FileType
CREATE TRIGGER Attachment_FileType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BinaryRelationshipRule - Reference Property Audit Triggers

-- BooleanParameterType - Reference Property Audit Triggers

-- Category - Reference Property Audit Triggers
-- Category.PermissibleClass
CREATE TRIGGER Category_PermissibleClass_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_PermissibleClass_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Category.SuperCategory
CREATE TRIGGER Category_SuperCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_SuperCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Citation - Reference Property Audit Triggers

-- CompoundParameterType - Reference Property Audit Triggers

-- Constant - Reference Property Audit Triggers
-- Constant.Category
CREATE TRIGGER Constant_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Constant_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ConversionBasedUnit - Reference Property Audit Triggers

-- CyclicRatioScale - Reference Property Audit Triggers

-- DateParameterType - Reference Property Audit Triggers

-- DateTimeParameterType - Reference Property Audit Triggers

-- DecompositionRule - Reference Property Audit Triggers
-- DecompositionRule.ContainedCategory
CREATE TRIGGER DecompositionRule_ContainedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DecompositionRule_ContainedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DefinedThing - Reference Property Audit Triggers

-- Definition - Reference Property Audit Triggers
-- Definition.Example
CREATE TRIGGER Definition_Example_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Example_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Definition.Note
CREATE TRIGGER Definition_Note_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Note_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DependentParameterTypeAssignment - Reference Property Audit Triggers

-- DerivedQuantityKind - Reference Property Audit Triggers

-- DerivedUnit - Reference Property Audit Triggers

-- DiscussionItem - Reference Property Audit Triggers

-- DomainOfExpertise - Reference Property Audit Triggers
-- DomainOfExpertise.Category
CREATE TRIGGER DomainOfExpertise_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertise_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DomainOfExpertiseGroup - Reference Property Audit Triggers
-- DomainOfExpertiseGroup.Domain
CREATE TRIGGER DomainOfExpertiseGroup_Domain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertiseGroup_Domain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EmailAddress - Reference Property Audit Triggers

-- EngineeringModelSetup - Reference Property Audit Triggers
-- EngineeringModelSetup.ActiveDomain
CREATE TRIGGER EngineeringModelSetup_ActiveDomain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelSetup_ActiveDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EnumerationParameterType - Reference Property Audit Triggers

-- EnumerationValueDefinition - Reference Property Audit Triggers

-- FileType - Reference Property Audit Triggers
-- FileType.Category
CREATE TRIGGER FileType_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileType_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- GenericAnnotation - Reference Property Audit Triggers

-- Glossary - Reference Property Audit Triggers
-- Glossary.Category
CREATE TRIGGER Glossary_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Glossary_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- HyperLink - Reference Property Audit Triggers

-- IndependentParameterTypeAssignment - Reference Property Audit Triggers

-- IntervalScale - Reference Property Audit Triggers

-- IterationSetup - Reference Property Audit Triggers

-- LinearConversionUnit - Reference Property Audit Triggers

-- LogarithmicScale - Reference Property Audit Triggers

-- LogEntryChangelogItem - Reference Property Audit Triggers
-- LogEntryChangelogItem.AffectedReferenceIid
CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MappingToReferenceScale - Reference Property Audit Triggers

-- MeasurementScale - Reference Property Audit Triggers

-- MeasurementUnit - Reference Property Audit Triggers

-- ModelReferenceDataLibrary - Reference Property Audit Triggers

-- MultiRelationshipRule - Reference Property Audit Triggers
-- MultiRelationshipRule.RelatedCategory
CREATE TRIGGER MultiRelationshipRule_RelatedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationshipRule_RelatedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NaturalLanguage - Reference Property Audit Triggers

-- OrdinalScale - Reference Property Audit Triggers

-- Organization - Reference Property Audit Triggers

-- OrganizationalParticipant - Reference Property Audit Triggers

-- ParameterizedCategoryRule - Reference Property Audit Triggers
-- ParameterizedCategoryRule.ParameterType
CREATE TRIGGER ParameterizedCategoryRule_ParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterizedCategoryRule_ParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterType - Reference Property Audit Triggers
-- ParameterType.Category
CREATE TRIGGER ParameterType_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterType_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterTypeComponent - Reference Property Audit Triggers

-- Participant - Reference Property Audit Triggers
-- Participant.Domain
CREATE TRIGGER Participant_Domain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Participant_Domain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParticipantPermission - Reference Property Audit Triggers

-- ParticipantRole - Reference Property Audit Triggers

-- Person - Reference Property Audit Triggers

-- PersonPermission - Reference Property Audit Triggers

-- PersonRole - Reference Property Audit Triggers

-- PrefixedUnit - Reference Property Audit Triggers

-- QuantityKind - Reference Property Audit Triggers
-- QuantityKind.PossibleScale
CREATE TRIGGER QuantityKind_PossibleScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKind_PossibleScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- QuantityKindFactor - Reference Property Audit Triggers

-- RatioScale - Reference Property Audit Triggers

-- ReferenceDataLibrary - Reference Property Audit Triggers
-- ReferenceDataLibrary.BaseQuantityKind
CREATE TRIGGER ReferenceDataLibrary_BaseQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_BaseQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferenceDataLibrary.BaseUnit
CREATE TRIGGER ReferenceDataLibrary_BaseUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_BaseUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferencerRule - Reference Property Audit Triggers
-- ReferencerRule.ReferencedCategory
CREATE TRIGGER ReferencerRule_ReferencedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferencerRule_ReferencedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReferenceSource - Reference Property Audit Triggers
-- ReferenceSource.Category
CREATE TRIGGER ReferenceSource_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceSource_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Rule - Reference Property Audit Triggers

-- SampledFunctionParameterType - Reference Property Audit Triggers

-- ScalarParameterType - Reference Property Audit Triggers

-- ScaleReferenceQuantityValue - Reference Property Audit Triggers

-- ScaleValueDefinition - Reference Property Audit Triggers

-- SimpleQuantityKind - Reference Property Audit Triggers

-- SimpleUnit - Reference Property Audit Triggers

-- SiteDirectory - Reference Property Audit Triggers

-- SiteDirectoryDataAnnotation - Reference Property Audit Triggers

-- SiteDirectoryDataDiscussionItem - Reference Property Audit Triggers

-- SiteDirectoryThingReference - Reference Property Audit Triggers

-- SiteLogEntry - Reference Property Audit Triggers
-- SiteLogEntry.AffectedDomainIid
CREATE TRIGGER SiteLogEntry_AffectedDomainIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_AffectedDomainIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteLogEntry.AffectedItemIid
CREATE TRIGGER SiteLogEntry_AffectedItemIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_AffectedItemIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteLogEntry.Category
CREATE TRIGGER SiteLogEntry_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SiteReferenceDataLibrary - Reference Property Audit Triggers

-- SpecializedQuantityKind - Reference Property Audit Triggers

-- TelephoneNumber - Reference Property Audit Triggers
-- TelephoneNumber.VcardType
CREATE TRIGGER TelephoneNumber_VcardType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TelephoneNumber_VcardType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Term - Reference Property Audit Triggers

-- TextParameterType - Reference Property Audit Triggers

-- Thing - Reference Property Audit Triggers
-- Thing.ExcludedDomain
CREATE TRIGGER Thing_ExcludedDomain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing.ExcludedPerson
CREATE TRIGGER Thing_ExcludedPerson_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedPerson_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ThingReference - Reference Property Audit Triggers

-- TimeOfDayParameterType - Reference Property Audit Triggers

-- TopContainer - Reference Property Audit Triggers

-- UnitFactor - Reference Property Audit Triggers

-- UnitPrefix - Reference Property Audit Triggers

-- UserPreference - Reference Property Audit Triggers
