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
------------------------------------------- Class tables -----------------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Alias" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Alias_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Alias_ValidFrom" ON "SiteDirectory"."Alias" ("ValidFrom");
CREATE INDEX "Idx_Alias_ValidTo" ON "SiteDirectory"."Alias" ("ValidTo");
ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_analyze_threshold = 2500);

-- ArrayParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ArrayParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ArrayParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ArrayParameterType_ValidFrom" ON "SiteDirectory"."ArrayParameterType" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_ValidTo" ON "SiteDirectory"."ArrayParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BinaryRelationshipRule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BinaryRelationshipRule_ValidFrom" ON "SiteDirectory"."BinaryRelationshipRule" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationshipRule_ValidTo" ON "SiteDirectory"."BinaryRelationshipRule" ("ValidTo");
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_analyze_threshold = 2500);

-- BooleanParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."BooleanParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BooleanParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BooleanParameterType_ValidFrom" ON "SiteDirectory"."BooleanParameterType" ("ValidFrom");
CREATE INDEX "Idx_BooleanParameterType_ValidTo" ON "SiteDirectory"."BooleanParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_analyze_threshold = 2500);

-- Category class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Category" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Category_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Category_ValidFrom" ON "SiteDirectory"."Category" ("ValidFrom");
CREATE INDEX "Idx_Category_ValidTo" ON "SiteDirectory"."Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_analyze_threshold = 2500);

-- Citation class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Citation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Citation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Citation_ValidFrom" ON "SiteDirectory"."Citation" ("ValidFrom");
CREATE INDEX "Idx_Citation_ValidTo" ON "SiteDirectory"."Citation" ("ValidTo");
ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_analyze_threshold = 2500);

-- CompoundParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."CompoundParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "CompoundParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_CompoundParameterType_ValidFrom" ON "SiteDirectory"."CompoundParameterType" ("ValidFrom");
CREATE INDEX "Idx_CompoundParameterType_ValidTo" ON "SiteDirectory"."CompoundParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_analyze_threshold = 2500);

-- Constant class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Constant" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Constant_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Constant_ValidFrom" ON "SiteDirectory"."Constant" ("ValidFrom");
CREATE INDEX "Idx_Constant_ValidTo" ON "SiteDirectory"."Constant" ("ValidTo");
ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_analyze_threshold = 2500);

-- ConversionBasedUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ConversionBasedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ConversionBasedUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ConversionBasedUnit_ValidFrom" ON "SiteDirectory"."ConversionBasedUnit" ("ValidFrom");
CREATE INDEX "Idx_ConversionBasedUnit_ValidTo" ON "SiteDirectory"."ConversionBasedUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_analyze_threshold = 2500);

-- CyclicRatioScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."CyclicRatioScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "CyclicRatioScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_CyclicRatioScale_ValidFrom" ON "SiteDirectory"."CyclicRatioScale" ("ValidFrom");
CREATE INDEX "Idx_CyclicRatioScale_ValidTo" ON "SiteDirectory"."CyclicRatioScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_analyze_threshold = 2500);

-- DateParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DateParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DateParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DateParameterType_ValidFrom" ON "SiteDirectory"."DateParameterType" ("ValidFrom");
CREATE INDEX "Idx_DateParameterType_ValidTo" ON "SiteDirectory"."DateParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_analyze_threshold = 2500);

-- DateTimeParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DateTimeParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DateTimeParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DateTimeParameterType_ValidFrom" ON "SiteDirectory"."DateTimeParameterType" ("ValidFrom");
CREATE INDEX "Idx_DateTimeParameterType_ValidTo" ON "SiteDirectory"."DateTimeParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_analyze_threshold = 2500);

-- DecompositionRule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DecompositionRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DecompositionRule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DecompositionRule_ValidFrom" ON "SiteDirectory"."DecompositionRule" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ValidTo" ON "SiteDirectory"."DecompositionRule" ("ValidTo");
ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DefinedThing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DefinedThing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DefinedThing_ValidFrom" ON "SiteDirectory"."DefinedThing" ("ValidFrom");
CREATE INDEX "Idx_DefinedThing_ValidTo" ON "SiteDirectory"."DefinedThing" ("ValidTo");
ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_analyze_threshold = 2500);

-- Definition class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Definition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Definition_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Definition_ValidFrom" ON "SiteDirectory"."Definition" ("ValidFrom");
CREATE INDEX "Idx_Definition_ValidTo" ON "SiteDirectory"."Definition" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DerivedQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DerivedQuantityKind_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DerivedQuantityKind_ValidFrom" ON "SiteDirectory"."DerivedQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_DerivedQuantityKind_ValidTo" ON "SiteDirectory"."DerivedQuantityKind" ("ValidTo");
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_analyze_threshold = 2500);

-- DerivedUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DerivedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DerivedUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DerivedUnit_ValidFrom" ON "SiteDirectory"."DerivedUnit" ("ValidFrom");
CREATE INDEX "Idx_DerivedUnit_ValidTo" ON "SiteDirectory"."DerivedUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."DiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiscussionItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiscussionItem_ValidFrom" ON "SiteDirectory"."DiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItem_ValidTo" ON "SiteDirectory"."DiscussionItem" ("ValidTo");
ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertise class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DomainOfExpertise" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DomainOfExpertise_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DomainOfExpertise_ValidFrom" ON "SiteDirectory"."DomainOfExpertise" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_ValidTo" ON "SiteDirectory"."DomainOfExpertise" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DomainOfExpertiseGroup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DomainOfExpertiseGroup_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."EmailAddress" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EmailAddress_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EmailAddress_ValidFrom" ON "SiteDirectory"."EmailAddress" ("ValidFrom");
CREATE INDEX "Idx_EmailAddress_ValidTo" ON "SiteDirectory"."EmailAddress" ("ValidTo");
ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelSetup class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."EngineeringModelSetup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModelSetup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EngineeringModelSetup_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ValidTo" ON "SiteDirectory"."EngineeringModelSetup" ("ValidTo");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."EnumerationParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EnumerationParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EnumerationParameterType_ValidFrom" ON "SiteDirectory"."EnumerationParameterType" ("ValidFrom");
CREATE INDEX "Idx_EnumerationParameterType_ValidTo" ON "SiteDirectory"."EnumerationParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationValueDefinition class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EnumerationValueDefinition_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EnumerationValueDefinition_ValidFrom" ON "SiteDirectory"."EnumerationValueDefinition" ("ValidFrom");
CREATE INDEX "Idx_EnumerationValueDefinition_ValidTo" ON "SiteDirectory"."EnumerationValueDefinition" ("ValidTo");
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_analyze_threshold = 2500);

-- FileType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."FileType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_FileType_ValidFrom" ON "SiteDirectory"."FileType" ("ValidFrom");
CREATE INDEX "Idx_FileType_ValidTo" ON "SiteDirectory"."FileType" ("ValidTo");
ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."GenericAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "GenericAnnotation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_GenericAnnotation_ValidFrom" ON "SiteDirectory"."GenericAnnotation" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotation_ValidTo" ON "SiteDirectory"."GenericAnnotation" ("ValidTo");
ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_analyze_threshold = 2500);

-- Glossary class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Glossary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Glossary_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Glossary_ValidFrom" ON "SiteDirectory"."Glossary" ("ValidFrom");
CREATE INDEX "Idx_Glossary_ValidTo" ON "SiteDirectory"."Glossary" ("ValidTo");
ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."HyperLink" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "HyperLink_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_HyperLink_ValidFrom" ON "SiteDirectory"."HyperLink" ("ValidFrom");
CREATE INDEX "Idx_HyperLink_ValidTo" ON "SiteDirectory"."HyperLink" ("ValidTo");
ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_analyze_threshold = 2500);

-- IntervalScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."IntervalScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "IntervalScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_IntervalScale_ValidFrom" ON "SiteDirectory"."IntervalScale" ("ValidFrom");
CREATE INDEX "Idx_IntervalScale_ValidTo" ON "SiteDirectory"."IntervalScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_analyze_threshold = 2500);

-- IterationSetup class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."IterationSetup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "IterationSetup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_IterationSetup_ValidFrom" ON "SiteDirectory"."IterationSetup" ("ValidFrom");
CREATE INDEX "Idx_IterationSetup_ValidTo" ON "SiteDirectory"."IterationSetup" ("ValidTo");
ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_analyze_threshold = 2500);

-- LinearConversionUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."LinearConversionUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "LinearConversionUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_LinearConversionUnit_ValidFrom" ON "SiteDirectory"."LinearConversionUnit" ("ValidFrom");
CREATE INDEX "Idx_LinearConversionUnit_ValidTo" ON "SiteDirectory"."LinearConversionUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_analyze_threshold = 2500);

-- LogarithmicScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."LogarithmicScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "LogarithmicScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_LogarithmicScale_ValidFrom" ON "SiteDirectory"."LogarithmicScale" ("ValidFrom");
CREATE INDEX "Idx_LogarithmicScale_ValidTo" ON "SiteDirectory"."LogarithmicScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_analyze_threshold = 2500);

-- MappingToReferenceScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."MappingToReferenceScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MappingToReferenceScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_MappingToReferenceScale_ValidFrom" ON "SiteDirectory"."MappingToReferenceScale" ("ValidFrom");
CREATE INDEX "Idx_MappingToReferenceScale_ValidTo" ON "SiteDirectory"."MappingToReferenceScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."MeasurementScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MeasurementScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_MeasurementScale_ValidFrom" ON "SiteDirectory"."MeasurementScale" ("ValidFrom");
CREATE INDEX "Idx_MeasurementScale_ValidTo" ON "SiteDirectory"."MeasurementScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."MeasurementUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MeasurementUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_MeasurementUnit_ValidFrom" ON "SiteDirectory"."MeasurementUnit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementUnit_ValidTo" ON "SiteDirectory"."MeasurementUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_analyze_threshold = 2500);

-- ModelReferenceDataLibrary class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModelReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ModelReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."ModelReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_ModelReferenceDataLibrary_ValidTo" ON "SiteDirectory"."ModelReferenceDataLibrary" ("ValidTo");
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationshipRule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."MultiRelationshipRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MultiRelationshipRule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_MultiRelationshipRule_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_ValidTo" ON "SiteDirectory"."MultiRelationshipRule" ("ValidTo");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_analyze_threshold = 2500);

-- NaturalLanguage class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."NaturalLanguage" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "NaturalLanguage_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_NaturalLanguage_ValidFrom" ON "SiteDirectory"."NaturalLanguage" ("ValidFrom");
CREATE INDEX "Idx_NaturalLanguage_ValidTo" ON "SiteDirectory"."NaturalLanguage" ("ValidTo");
ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_analyze_threshold = 2500);

-- OrdinalScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."OrdinalScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "OrdinalScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_OrdinalScale_ValidFrom" ON "SiteDirectory"."OrdinalScale" ("ValidFrom");
CREATE INDEX "Idx_OrdinalScale_ValidTo" ON "SiteDirectory"."OrdinalScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_analyze_threshold = 2500);

-- Organization class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Organization" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Organization_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Organization_ValidFrom" ON "SiteDirectory"."Organization" ("ValidFrom");
CREATE INDEX "Idx_Organization_ValidTo" ON "SiteDirectory"."Organization" ("ValidTo");
ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_analyze_threshold = 2500);

-- ParameterizedCategoryRule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterizedCategoryRule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterizedCategoryRule_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterType_ValidFrom" ON "SiteDirectory"."ParameterType" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_ValidTo" ON "SiteDirectory"."ParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_analyze_threshold = 2500);

-- ParameterTypeComponent class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ParameterTypeComponent" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterTypeComponent_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterTypeComponent_ValidFrom" ON "SiteDirectory"."ParameterTypeComponent" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeComponent_ValidTo" ON "SiteDirectory"."ParameterTypeComponent" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_analyze_threshold = 2500);

-- Participant class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Participant" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Participant_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Participant_ValidFrom" ON "SiteDirectory"."Participant" ("ValidFrom");
CREATE INDEX "Idx_Participant_ValidTo" ON "SiteDirectory"."Participant" ("ValidTo");
ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantPermission class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ParticipantPermission" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParticipantPermission_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParticipantPermission_ValidFrom" ON "SiteDirectory"."ParticipantPermission" ("ValidFrom");
CREATE INDEX "Idx_ParticipantPermission_ValidTo" ON "SiteDirectory"."ParticipantPermission" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantRole class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ParticipantRole" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParticipantRole_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParticipantRole_ValidFrom" ON "SiteDirectory"."ParticipantRole" ("ValidFrom");
CREATE INDEX "Idx_ParticipantRole_ValidTo" ON "SiteDirectory"."ParticipantRole" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_analyze_threshold = 2500);

-- Person class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Person" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Person_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Person_ValidFrom" ON "SiteDirectory"."Person" ("ValidFrom");
CREATE INDEX "Idx_Person_ValidTo" ON "SiteDirectory"."Person" ("ValidTo");
ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_analyze_threshold = 2500);

-- PersonPermission class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."PersonPermission" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PersonPermission_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_PersonPermission_ValidFrom" ON "SiteDirectory"."PersonPermission" ("ValidFrom");
CREATE INDEX "Idx_PersonPermission_ValidTo" ON "SiteDirectory"."PersonPermission" ("ValidTo");
ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_analyze_threshold = 2500);

-- PersonRole class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."PersonRole" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PersonRole_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_PersonRole_ValidFrom" ON "SiteDirectory"."PersonRole" ("ValidFrom");
CREATE INDEX "Idx_PersonRole_ValidTo" ON "SiteDirectory"."PersonRole" ("ValidTo");
ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_analyze_threshold = 2500);

-- PrefixedUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."PrefixedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PrefixedUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_PrefixedUnit_ValidFrom" ON "SiteDirectory"."PrefixedUnit" ("ValidFrom");
CREATE INDEX "Idx_PrefixedUnit_ValidTo" ON "SiteDirectory"."PrefixedUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKind class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."QuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "QuantityKind_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_QuantityKind_ValidFrom" ON "SiteDirectory"."QuantityKind" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_ValidTo" ON "SiteDirectory"."QuantityKind" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKindFactor class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."QuantityKindFactor" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "QuantityKindFactor_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_QuantityKindFactor_ValidFrom" ON "SiteDirectory"."QuantityKindFactor" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindFactor_ValidTo" ON "SiteDirectory"."QuantityKindFactor" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_analyze_threshold = 2500);

-- RatioScale class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."RatioScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RatioScale_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RatioScale_ValidFrom" ON "SiteDirectory"."RatioScale" ("ValidFrom");
CREATE INDEX "Idx_RatioScale_ValidTo" ON "SiteDirectory"."RatioScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);

-- ReferencerRule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ReferencerRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferencerRule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ReferencerRule_ValidFrom" ON "SiteDirectory"."ReferencerRule" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ValidTo" ON "SiteDirectory"."ReferencerRule" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ReferenceSource" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferenceSource_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ReferenceSource_ValidFrom" ON "SiteDirectory"."ReferenceSource" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_ValidTo" ON "SiteDirectory"."ReferenceSource" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_analyze_threshold = 2500);

-- Rule class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Rule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Rule_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Rule_ValidFrom" ON "SiteDirectory"."Rule" ("ValidFrom");
CREATE INDEX "Idx_Rule_ValidTo" ON "SiteDirectory"."Rule" ("ValidTo");
ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_analyze_threshold = 2500);

-- ScalarParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ScalarParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ScalarParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ScalarParameterType_ValidFrom" ON "SiteDirectory"."ScalarParameterType" ("ValidFrom");
CREATE INDEX "Idx_ScalarParameterType_ValidTo" ON "SiteDirectory"."ScalarParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_analyze_threshold = 2500);

-- ScaleReferenceQuantityValue class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ScaleReferenceQuantityValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ScaleReferenceQuantityValue_ValidFrom" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("ValidFrom");
CREATE INDEX "Idx_ScaleReferenceQuantityValue_ValidTo" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_analyze_threshold = 2500);

-- ScaleValueDefinition class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."ScaleValueDefinition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ScaleValueDefinition_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ScaleValueDefinition_ValidFrom" ON "SiteDirectory"."ScaleValueDefinition" ("ValidFrom");
CREATE INDEX "Idx_ScaleValueDefinition_ValidTo" ON "SiteDirectory"."ScaleValueDefinition" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_analyze_threshold = 2500);

-- SimpleQuantityKind class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SimpleQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SimpleQuantityKind_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SimpleQuantityKind_ValidFrom" ON "SiteDirectory"."SimpleQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_SimpleQuantityKind_ValidTo" ON "SiteDirectory"."SimpleQuantityKind" ("ValidTo");
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_analyze_threshold = 2500);

-- SimpleUnit class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SimpleUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SimpleUnit_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SimpleUnit_ValidFrom" ON "SiteDirectory"."SimpleUnit" ("ValidFrom");
CREATE INDEX "Idx_SimpleUnit_ValidTo" ON "SiteDirectory"."SimpleUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectory class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SiteDirectory" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteDirectory_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteDirectory_ValidFrom" ON "SiteDirectory"."SiteDirectory" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectory_ValidTo" ON "SiteDirectory"."SiteDirectory" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataAnnotation class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteDirectoryDataAnnotation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_ValidTo" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataDiscussionItem class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteDirectoryDataDiscussionItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_ValidTo" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryThingReference class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteDirectoryThingReference_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteDirectoryThingReference_ValidFrom" ON "SiteDirectory"."SiteDirectoryThingReference" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryThingReference_ValidTo" ON "SiteDirectory"."SiteDirectoryThingReference" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SiteLogEntry" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteLogEntry_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteLogEntry_ValidFrom" ON "SiteDirectory"."SiteLogEntry" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_ValidTo" ON "SiteDirectory"."SiteLogEntry" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SiteReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."SiteReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_SiteReferenceDataLibrary_ValidTo" ON "SiteDirectory"."SiteReferenceDataLibrary" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);

-- SpecializedQuantityKind class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SpecializedQuantityKind_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SpecializedQuantityKind_ValidFrom" ON "SiteDirectory"."SpecializedQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_SpecializedQuantityKind_ValidTo" ON "SiteDirectory"."SpecializedQuantityKind" ("ValidTo");
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_analyze_threshold = 2500);

-- TelephoneNumber class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."TelephoneNumber" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TelephoneNumber_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TelephoneNumber_ValidFrom" ON "SiteDirectory"."TelephoneNumber" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_ValidTo" ON "SiteDirectory"."TelephoneNumber" ("ValidTo");
ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_analyze_threshold = 2500);

-- Term class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Term" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Term_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Term_ValidFrom" ON "SiteDirectory"."Term" ("ValidFrom");
CREATE INDEX "Idx_Term_ValidTo" ON "SiteDirectory"."Term" ("ValidTo");
ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_analyze_threshold = 2500);

-- TextParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."TextParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TextParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TextParameterType_ValidFrom" ON "SiteDirectory"."TextParameterType" ("ValidFrom");
CREATE INDEX "Idx_TextParameterType_ValidTo" ON "SiteDirectory"."TextParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_analyze_threshold = 2500);

-- Thing class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."Thing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Thing_ValidFrom" ON "SiteDirectory"."Thing" ("ValidFrom");
CREATE INDEX "Idx_Thing_ValidTo" ON "SiteDirectory"."Thing" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference class - table definition [version 1.1.0]
CREATE TABLE "SiteDirectory"."ThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ThingReference_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ThingReference_ValidFrom" ON "SiteDirectory"."ThingReference" ("ValidFrom");
CREATE INDEX "Idx_ThingReference_ValidTo" ON "SiteDirectory"."ThingReference" ("ValidTo");
ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_analyze_threshold = 2500);

-- TimeOfDayParameterType class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TimeOfDayParameterType_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TimeOfDayParameterType_ValidFrom" ON "SiteDirectory"."TimeOfDayParameterType" ("ValidFrom");
CREATE INDEX "Idx_TimeOfDayParameterType_ValidTo" ON "SiteDirectory"."TimeOfDayParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."TopContainer" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TopContainer_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TopContainer_ValidFrom" ON "SiteDirectory"."TopContainer" ("ValidFrom");
CREATE INDEX "Idx_TopContainer_ValidTo" ON "SiteDirectory"."TopContainer" ("ValidTo");
ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_analyze_threshold = 2500);

-- UnitFactor class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."UnitFactor" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "UnitFactor_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_UnitFactor_ValidFrom" ON "SiteDirectory"."UnitFactor" ("ValidFrom");
CREATE INDEX "Idx_UnitFactor_ValidTo" ON "SiteDirectory"."UnitFactor" ("ValidTo");
ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_analyze_threshold = 2500);

-- UnitPrefix class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."UnitPrefix" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "UnitPrefix_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_UnitPrefix_ValidFrom" ON "SiteDirectory"."UnitPrefix" ("ValidFrom");
CREATE INDEX "Idx_UnitPrefix_ValidTo" ON "SiteDirectory"."UnitPrefix" ("ValidTo");
ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_analyze_threshold = 2500);

-- UserPreference class - table definition [version 1.0.0]
CREATE TABLE "SiteDirectory"."UserPreference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "UserPreference_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_UserPreference_ValidFrom" ON "SiteDirectory"."UserPreference" ("ValidFrom");
CREATE INDEX "Idx_UserPreference_ValidTo" ON "SiteDirectory"."UserPreference" ("ValidTo");
ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
----------------------------------- Derives Relationships ----------------------------------------------
--------------------------------------------------------------------------------------------------------

-- Class Alias derives from class Thing
ALTER TABLE "SiteDirectory"."Alias" ADD CONSTRAINT "AliasDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ArrayParameterType derives from class CompoundParameterType
ALTER TABLE "SiteDirectory"."ArrayParameterType" ADD CONSTRAINT "ArrayParameterTypeDerivesFromCompoundParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."CompoundParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BinaryRelationshipRule derives from class Rule
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BooleanParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."BooleanParameterType" ADD CONSTRAINT "BooleanParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Category derives from class DefinedThing
ALTER TABLE "SiteDirectory"."Category" ADD CONSTRAINT "CategoryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Citation derives from class Thing
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "CitationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class CompoundParameterType derives from class ParameterType
ALTER TABLE "SiteDirectory"."CompoundParameterType" ADD CONSTRAINT "CompoundParameterTypeDerivesFromParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Constant derives from class DefinedThing
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "ConstantDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ConversionBasedUnit derives from class MeasurementUnit
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD CONSTRAINT "ConversionBasedUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class CyclicRatioScale derives from class RatioScale
ALTER TABLE "SiteDirectory"."CyclicRatioScale" ADD CONSTRAINT "CyclicRatioScaleDerivesFromRatioScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."RatioScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DateParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."DateParameterType" ADD CONSTRAINT "DateParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DateTimeParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."DateTimeParameterType" ADD CONSTRAINT "DateTimeParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DecompositionRule derives from class Rule
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD CONSTRAINT "DecompositionRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DefinedThing derives from class Thing
ALTER TABLE "SiteDirectory"."DefinedThing" ADD CONSTRAINT "DefinedThingDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Definition derives from class Thing
ALTER TABLE "SiteDirectory"."Definition" ADD CONSTRAINT "DefinitionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DerivedQuantityKind derives from class QuantityKind
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" ADD CONSTRAINT "DerivedQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DerivedUnit derives from class MeasurementUnit
ALTER TABLE "SiteDirectory"."DerivedUnit" ADD CONSTRAINT "DerivedUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiscussionItem derives from class GenericAnnotation
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD CONSTRAINT "DiscussionItemDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DomainOfExpertise derives from class DefinedThing
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD CONSTRAINT "DomainOfExpertiseDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DomainOfExpertiseGroup derives from class DefinedThing
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD CONSTRAINT "DomainOfExpertiseGroupDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EmailAddress derives from class Thing
ALTER TABLE "SiteDirectory"."EmailAddress" ADD CONSTRAINT "EmailAddressDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EngineeringModelSetup derives from class DefinedThing
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD CONSTRAINT "EngineeringModelSetupDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EnumerationParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."EnumerationParameterType" ADD CONSTRAINT "EnumerationParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EnumerationValueDefinition derives from class DefinedThing
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD CONSTRAINT "EnumerationValueDefinitionDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class FileType derives from class DefinedThing
ALTER TABLE "SiteDirectory"."FileType" ADD CONSTRAINT "FileTypeDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class GenericAnnotation derives from class Thing
ALTER TABLE "SiteDirectory"."GenericAnnotation" ADD CONSTRAINT "GenericAnnotationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Glossary derives from class DefinedThing
ALTER TABLE "SiteDirectory"."Glossary" ADD CONSTRAINT "GlossaryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class HyperLink derives from class Thing
ALTER TABLE "SiteDirectory"."HyperLink" ADD CONSTRAINT "HyperLinkDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class IntervalScale derives from class MeasurementScale
ALTER TABLE "SiteDirectory"."IntervalScale" ADD CONSTRAINT "IntervalScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class IterationSetup derives from class Thing
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetupDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class LinearConversionUnit derives from class ConversionBasedUnit
ALTER TABLE "SiteDirectory"."LinearConversionUnit" ADD CONSTRAINT "LinearConversionUnitDerivesFromConversionBasedUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ConversionBasedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class LogarithmicScale derives from class MeasurementScale
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD CONSTRAINT "LogarithmicScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class MappingToReferenceScale derives from class Thing
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScaleDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class MeasurementScale derives from class DefinedThing
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScaleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class MeasurementUnit derives from class DefinedThing
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD CONSTRAINT "MeasurementUnitDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ModelReferenceDataLibrary derives from class ReferenceDataLibrary
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD CONSTRAINT "ModelReferenceDataLibraryDerivesFromReferenceDataLibrary" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class MultiRelationshipRule derives from class Rule
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD CONSTRAINT "MultiRelationshipRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class NaturalLanguage derives from class Thing
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD CONSTRAINT "NaturalLanguageDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class OrdinalScale derives from class MeasurementScale
ALTER TABLE "SiteDirectory"."OrdinalScale" ADD CONSTRAINT "OrdinalScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Organization derives from class Thing
ALTER TABLE "SiteDirectory"."Organization" ADD CONSTRAINT "OrganizationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterizedCategoryRule derives from class Rule
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD CONSTRAINT "ParameterizedCategoryRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterType derives from class DefinedThing
ALTER TABLE "SiteDirectory"."ParameterType" ADD CONSTRAINT "ParameterTypeDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterTypeComponent derives from class Thing
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Participant derives from class Thing
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "ParticipantDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParticipantPermission derives from class Thing
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD CONSTRAINT "ParticipantPermissionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParticipantRole derives from class DefinedThing
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD CONSTRAINT "ParticipantRoleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Person derives from class Thing
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "PersonDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class PersonPermission derives from class Thing
ALTER TABLE "SiteDirectory"."PersonPermission" ADD CONSTRAINT "PersonPermissionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class PersonRole derives from class DefinedThing
ALTER TABLE "SiteDirectory"."PersonRole" ADD CONSTRAINT "PersonRoleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class PrefixedUnit derives from class ConversionBasedUnit
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD CONSTRAINT "PrefixedUnitDerivesFromConversionBasedUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ConversionBasedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class QuantityKind derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."QuantityKind" ADD CONSTRAINT "QuantityKindDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class QuantityKindFactor derives from class Thing
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactorDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RatioScale derives from class MeasurementScale
ALTER TABLE "SiteDirectory"."RatioScale" ADD CONSTRAINT "RatioScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ReferenceDataLibrary derives from class DefinedThing
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD CONSTRAINT "ReferenceDataLibraryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ReferencerRule derives from class Rule
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD CONSTRAINT "ReferencerRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ReferenceSource derives from class DefinedThing
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSourceDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Rule derives from class DefinedThing
ALTER TABLE "SiteDirectory"."Rule" ADD CONSTRAINT "RuleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ScalarParameterType derives from class ParameterType
ALTER TABLE "SiteDirectory"."ScalarParameterType" ADD CONSTRAINT "ScalarParameterTypeDerivesFromParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ScaleReferenceQuantityValue derives from class Thing
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValueDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ScaleValueDefinition derives from class DefinedThing
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD CONSTRAINT "ScaleValueDefinitionDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SimpleQuantityKind derives from class QuantityKind
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" ADD CONSTRAINT "SimpleQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SimpleUnit derives from class MeasurementUnit
ALTER TABLE "SiteDirectory"."SimpleUnit" ADD CONSTRAINT "SimpleUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteDirectory derives from class TopContainer
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectoryDerivesFromTopContainer" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."TopContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteDirectoryDataAnnotation derives from class GenericAnnotation
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotationDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteDirectoryDataDiscussionItem derives from class DiscussionItem
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItemDerivesFromDiscussionItem" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteDirectoryThingReference derives from class ThingReference
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD CONSTRAINT "SiteDirectoryThingReferenceDerivesFromThingReference" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ThingReference" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteLogEntry derives from class Thing
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntryDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SiteReferenceDataLibrary derives from class ReferenceDataLibrary
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD CONSTRAINT "SiteReferenceDataLibraryDerivesFromReferenceDataLibrary" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SpecializedQuantityKind derives from class QuantityKind
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD CONSTRAINT "SpecializedQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TelephoneNumber derives from class Thing
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD CONSTRAINT "TelephoneNumberDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Term derives from class DefinedThing
ALTER TABLE "SiteDirectory"."Term" ADD CONSTRAINT "TermDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TextParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."TextParameterType" ADD CONSTRAINT "TextParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Thing
-- The Thing class has no superclass

-- Class ThingReference derives from class Thing
ALTER TABLE "SiteDirectory"."ThingReference" ADD CONSTRAINT "ThingReferenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TimeOfDayParameterType derives from class ScalarParameterType
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" ADD CONSTRAINT "TimeOfDayParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TopContainer derives from class Thing
ALTER TABLE "SiteDirectory"."TopContainer" ADD CONSTRAINT "TopContainerDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class UnitFactor derives from class Thing
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactorDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class UnitPrefix derives from class DefinedThing
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD CONSTRAINT "UnitPrefixDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class UserPreference derives from class Thing
ALTER TABLE "SiteDirectory"."UserPreference" ADD CONSTRAINT "UserPreferenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

--------------------------------------------------------------------------------------------------------
--------------------------------- Containment Relationships --------------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias containment
-- The Alias class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Alias" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Alias" ADD CONSTRAINT "Alias_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Alias_Container" ON "SiteDirectory"."Alias" ("Container");

-- ArrayParameterType containment
-- The ArrayParameterType class is not directly contained

-- BinaryRelationshipRule containment
-- The BinaryRelationshipRule class is not directly contained

-- BooleanParameterType containment
-- The BooleanParameterType class is not directly contained

-- Category containment
-- The Category class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Category" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Category" ADD CONSTRAINT "Category_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Category_Container" ON "SiteDirectory"."Category" ("Container");

-- Citation containment
-- The Citation class is contained (composite) by the Definition class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Citation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "Citation_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Citation_Container" ON "SiteDirectory"."Citation" ("Container");

-- CompoundParameterType containment
-- The CompoundParameterType class is not directly contained

-- Constant containment
-- The Constant class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Constant_Container" ON "SiteDirectory"."Constant" ("Container");

-- ConversionBasedUnit containment
-- The ConversionBasedUnit class is not directly contained

-- CyclicRatioScale containment
-- The CyclicRatioScale class is not directly contained

-- DateParameterType containment
-- The DateParameterType class is not directly contained

-- DateTimeParameterType containment
-- The DateTimeParameterType class is not directly contained

-- DecompositionRule containment
-- The DecompositionRule class is not directly contained

-- DefinedThing containment
-- The DefinedThing class is not directly contained

-- Definition containment
-- The Definition class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Definition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Definition" ADD CONSTRAINT "Definition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Definition_Container" ON "SiteDirectory"."Definition" ("Container");

-- DerivedQuantityKind containment
-- The DerivedQuantityKind class is not directly contained

-- DerivedUnit containment
-- The DerivedUnit class is not directly contained

-- DiscussionItem containment
-- The DiscussionItem class is not directly contained

-- DomainOfExpertise containment
-- The DomainOfExpertise class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD CONSTRAINT "DomainOfExpertise_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_DomainOfExpertise_Container" ON "SiteDirectory"."DomainOfExpertise" ("Container");

-- DomainOfExpertiseGroup containment
-- The DomainOfExpertiseGroup class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD CONSTRAINT "DomainOfExpertiseGroup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_DomainOfExpertiseGroup_Container" ON "SiteDirectory"."DomainOfExpertiseGroup" ("Container");

-- EmailAddress containment
-- The EmailAddress class is contained (composite) by the Person class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."EmailAddress" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EmailAddress" ADD CONSTRAINT "EmailAddress_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_EmailAddress_Container" ON "SiteDirectory"."EmailAddress" ("Container");

-- EngineeringModelSetup containment
-- The EngineeringModelSetup class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD CONSTRAINT "EngineeringModelSetup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_EngineeringModelSetup_Container" ON "SiteDirectory"."EngineeringModelSetup" ("Container");

-- EnumerationParameterType containment
-- The EnumerationParameterType class is not directly contained

-- EnumerationValueDefinition containment
-- The EnumerationValueDefinition class is contained (composite) by the EnumerationParameterType class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD CONSTRAINT "EnumerationValueDefinition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EnumerationParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_EnumerationValueDefinition_Container" ON "SiteDirectory"."EnumerationValueDefinition" ("Container");

-- FileType containment
-- The FileType class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."FileType" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."FileType" ADD CONSTRAINT "FileType_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_FileType_Container" ON "SiteDirectory"."FileType" ("Container");

-- GenericAnnotation containment
-- The GenericAnnotation class is not directly contained

-- Glossary containment
-- The Glossary class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Glossary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Glossary" ADD CONSTRAINT "Glossary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Glossary_Container" ON "SiteDirectory"."Glossary" ("Container");

-- HyperLink containment
-- The HyperLink class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."HyperLink" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."HyperLink" ADD CONSTRAINT "HyperLink_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_HyperLink_Container" ON "SiteDirectory"."HyperLink" ("Container");

-- IntervalScale containment
-- The IntervalScale class is not directly contained

-- IterationSetup containment
-- The IterationSetup class is contained (composite) by the EngineeringModelSetup class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."IterationSetup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_IterationSetup_Container" ON "SiteDirectory"."IterationSetup" ("Container");

-- LinearConversionUnit containment
-- The LinearConversionUnit class is not directly contained

-- LogarithmicScale containment
-- The LogarithmicScale class is not directly contained

-- MappingToReferenceScale containment
-- The MappingToReferenceScale class is contained (composite) by the MeasurementScale class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_MappingToReferenceScale_Container" ON "SiteDirectory"."MappingToReferenceScale" ("Container");

-- MeasurementScale containment
-- The MeasurementScale class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScale_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_MeasurementScale_Container" ON "SiteDirectory"."MeasurementScale" ("Container");

-- MeasurementUnit containment
-- The MeasurementUnit class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD CONSTRAINT "MeasurementUnit_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_MeasurementUnit_Container" ON "SiteDirectory"."MeasurementUnit" ("Container");

-- ModelReferenceDataLibrary containment
-- The ModelReferenceDataLibrary class is contained (composite) by the EngineeringModelSetup class: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD CONSTRAINT "ModelReferenceDataLibrary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ModelReferenceDataLibrary_Container" ON "SiteDirectory"."ModelReferenceDataLibrary" ("Container");

-- MultiRelationshipRule containment
-- The MultiRelationshipRule class is not directly contained

-- NaturalLanguage containment
-- The NaturalLanguage class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD CONSTRAINT "NaturalLanguage_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_NaturalLanguage_Container" ON "SiteDirectory"."NaturalLanguage" ("Container");

-- OrdinalScale containment
-- The OrdinalScale class is not directly contained

-- Organization containment
-- The Organization class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Organization" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Organization" ADD CONSTRAINT "Organization_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Organization_Container" ON "SiteDirectory"."Organization" ("Container");

-- ParameterizedCategoryRule containment
-- The ParameterizedCategoryRule class is not directly contained

-- ParameterType containment
-- The ParameterType class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParameterType" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterType" ADD CONSTRAINT "ParameterType_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterType_Container" ON "SiteDirectory"."ParameterType" ("Container");

-- ParameterTypeComponent containment
-- The ParameterTypeComponent class is contained (composite) by the CompoundParameterType class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."CompoundParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterTypeComponent_Container" ON "SiteDirectory"."ParameterTypeComponent" ("Container");

-- Participant containment
-- The Participant class is contained (composite) by the EngineeringModelSetup class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Participant_Container" ON "SiteDirectory"."Participant" ("Container");

-- ParticipantPermission containment
-- The ParticipantPermission class is contained (composite) by the ParticipantRole class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD CONSTRAINT "ParticipantPermission_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParticipantPermission_Container" ON "SiteDirectory"."ParticipantPermission" ("Container");

-- ParticipantRole containment
-- The ParticipantRole class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD CONSTRAINT "ParticipantRole_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParticipantRole_Container" ON "SiteDirectory"."ParticipantRole" ("Container");

-- Person containment
-- The Person class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Person_Container" ON "SiteDirectory"."Person" ("Container");

-- PersonPermission containment
-- The PersonPermission class is contained (composite) by the PersonRole class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."PersonPermission" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PersonPermission" ADD CONSTRAINT "PersonPermission_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_PersonPermission_Container" ON "SiteDirectory"."PersonPermission" ("Container");

-- PersonRole containment
-- The PersonRole class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."PersonRole" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PersonRole" ADD CONSTRAINT "PersonRole_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_PersonRole_Container" ON "SiteDirectory"."PersonRole" ("Container");

-- PrefixedUnit containment
-- The PrefixedUnit class is not directly contained

-- QuantityKind containment
-- The QuantityKind class is not directly contained

-- QuantityKindFactor containment
-- The QuantityKindFactor class is contained (composite) by the DerivedQuantityKind class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactor_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DerivedQuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_QuantityKindFactor_Container" ON "SiteDirectory"."QuantityKindFactor" ("Container");

-- RatioScale containment
-- The RatioScale class is not directly contained

-- ReferenceDataLibrary containment
-- The ReferenceDataLibrary class is not directly contained

-- ReferencerRule containment
-- The ReferencerRule class is not directly contained

-- ReferenceSource containment
-- The ReferenceSource class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ReferenceSource_Container" ON "SiteDirectory"."ReferenceSource" ("Container");

-- Rule containment
-- The Rule class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Rule" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Rule" ADD CONSTRAINT "Rule_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Rule_Container" ON "SiteDirectory"."Rule" ("Container");

-- ScalarParameterType containment
-- The ScalarParameterType class is not directly contained

-- ScaleReferenceQuantityValue containment
-- The ScaleReferenceQuantityValue class is contained (composite) by the LogarithmicScale class: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."LogarithmicScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ScaleReferenceQuantityValue_Container" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("Container");

-- ScaleValueDefinition containment
-- The ScaleValueDefinition class is contained (composite) by the MeasurementScale class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD CONSTRAINT "ScaleValueDefinition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ScaleValueDefinition_Container" ON "SiteDirectory"."ScaleValueDefinition" ("Container");

-- SimpleQuantityKind containment
-- The SimpleQuantityKind class is not directly contained

-- SimpleUnit containment
-- The SimpleUnit class is not directly contained

-- SiteDirectory containment
-- The SiteDirectory class is not directly contained

-- SiteDirectoryDataAnnotation containment
-- The SiteDirectoryDataAnnotation class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_Container" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("Container");

-- SiteDirectoryDataDiscussionItem containment
-- The SiteDirectoryDataDiscussionItem class is contained (composite) by the SiteDirectoryDataAnnotation class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectoryDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_Container" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("Container");

-- SiteDirectoryThingReference containment
-- The SiteDirectoryThingReference class is contained (composite) by the SiteDirectoryDataAnnotation class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD CONSTRAINT "SiteDirectoryThingReference_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectoryDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SiteDirectoryThingReference_Container" ON "SiteDirectory"."SiteDirectoryThingReference" ("Container");

-- SiteLogEntry containment
-- The SiteLogEntry class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntry_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SiteLogEntry_Container" ON "SiteDirectory"."SiteLogEntry" ("Container");

-- SiteReferenceDataLibrary containment
-- The SiteReferenceDataLibrary class is contained (composite) by the SiteDirectory class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD CONSTRAINT "SiteReferenceDataLibrary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SiteReferenceDataLibrary_Container" ON "SiteDirectory"."SiteReferenceDataLibrary" ("Container");

-- SpecializedQuantityKind containment
-- The SpecializedQuantityKind class is not directly contained

-- TelephoneNumber containment
-- The TelephoneNumber class is contained (composite) by the Person class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD CONSTRAINT "TelephoneNumber_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_TelephoneNumber_Container" ON "SiteDirectory"."TelephoneNumber" ("Container");

-- Term containment
-- The Term class is contained (composite) by the Glossary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Term" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Term" ADD CONSTRAINT "Term_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Glossary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Term_Container" ON "SiteDirectory"."Term" ("Container");

-- TextParameterType containment
-- The TextParameterType class is not directly contained

-- Thing containment
-- The Thing class is not directly contained

-- ThingReference containment
-- The ThingReference class is not directly contained

-- TimeOfDayParameterType containment
-- The TimeOfDayParameterType class is not directly contained

-- TopContainer containment
-- The TopContainer class is not directly contained

-- UnitFactor containment
-- The UnitFactor class is contained (composite) by the DerivedUnit class: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactor_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DerivedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_UnitFactor_Container" ON "SiteDirectory"."UnitFactor" ("Container");

-- UnitPrefix containment
-- The UnitPrefix class is contained (composite) by the ReferenceDataLibrary class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD CONSTRAINT "UnitPrefix_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_UnitPrefix_Container" ON "SiteDirectory"."UnitPrefix" ("Container");

-- UserPreference containment
-- The UserPreference class is contained (composite) by the Person class: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."UserPreference" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UserPreference" ADD CONSTRAINT "UserPreference_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_UserPreference_Container" ON "SiteDirectory"."UserPreference" ("Container");

--------------------------------------------------------------------------------------------------------
----------------------------------- Class Reference Or Ordered Properties ------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Reference properties

-- ArrayParameterType - Reference properties
-- ArrayParameterType.Dimension is an ordered collection property of type Int: [1..*]
CREATE TABLE "SiteDirectory"."ArrayParameterType_Dimension" (
  "ArrayParameterType" uuid NOT NULL,
  "Dimension" integer NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "ArrayParameterType_Dimension_FK_Source" FOREIGN KEY ("ArrayParameterType") REFERENCES "SiteDirectory"."ArrayParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_ArrayParameterType_Dimension_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Dimension" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_Dimension_ValidTo" ON "SiteDirectory"."ArrayParameterType_Dimension" ("ValidTo");
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule - Reference properties
-- BinaryRelationshipRule.RelationshipCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "RelationshipCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_RelationshipCategory" FOREIGN KEY ("RelationshipCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BinaryRelationshipRule
-- BinaryRelationshipRule.SourceCategory is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "SourceCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_SourceCategory" FOREIGN KEY ("SourceCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BinaryRelationshipRule
-- BinaryRelationshipRule.TargetCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "TargetCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_TargetCategory" FOREIGN KEY ("TargetCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BooleanParameterType - Reference properties

-- Category - Reference properties
-- Category.PermissibleClass is a collection property of type ClassKind: [1..*]
CREATE TABLE "SiteDirectory"."Category_PermissibleClass" (
  "Category" uuid NOT NULL,
  "PermissibleClass" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Category_PermissibleClass_PK" PRIMARY KEY("Category", "PermissibleClass"),
  CONSTRAINT "Category_PermissibleClass_FK_Source" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_Category_PermissibleClass_ValidFrom" ON "SiteDirectory"."Category_PermissibleClass" ("ValidFrom");
CREATE INDEX "Idx_Category_PermissibleClass_ValidTo" ON "SiteDirectory"."Category_PermissibleClass" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_analyze_threshold = 2500);

-- Category
-- Category.SuperCategory is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Category_SuperCategory" (
  "Category" uuid NOT NULL,
  "SuperCategory" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Category_SuperCategory_PK" PRIMARY KEY("Category", "SuperCategory"),
  CONSTRAINT "Category_SuperCategory_FK_Source" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Category_SuperCategory_FK_Target" FOREIGN KEY ("SuperCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Category_SuperCategory_ValidFrom" ON "SiteDirectory"."Category_SuperCategory" ("ValidFrom");
CREATE INDEX "Idx_Category_SuperCategory_ValidTo" ON "SiteDirectory"."Category_SuperCategory" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Reference properties
-- Citation.Source is an association to ReferenceSource: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Citation" ADD COLUMN "Source" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "Citation_FK_Source" FOREIGN KEY ("Source") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- CompoundParameterType - Reference properties

-- Constant - Reference properties
-- Constant.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Constant_Category" (
  "Constant" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Constant_Category_PK" PRIMARY KEY("Constant", "Category"),
  CONSTRAINT "Constant_Category_FK_Source" FOREIGN KEY ("Constant") REFERENCES "SiteDirectory"."Constant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Constant_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Constant_Category_ValidFrom" ON "SiteDirectory"."Constant_Category" ("ValidFrom");
CREATE INDEX "Idx_Constant_Category_ValidTo" ON "SiteDirectory"."Constant_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_analyze_threshold = 2500);

-- Constant
-- Constant.ParameterType is an association to ParameterType: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Constant
-- Constant.Scale is an optional association to MeasurementScale: [0..1]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "Scale" uuid;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ConversionBasedUnit - Reference properties
-- ConversionBasedUnit.ReferenceUnit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD COLUMN "ReferenceUnit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD CONSTRAINT "ConversionBasedUnit_FK_ReferenceUnit" FOREIGN KEY ("ReferenceUnit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- CyclicRatioScale - Reference properties

-- DateParameterType - Reference properties

-- DateTimeParameterType - Reference properties

-- DecompositionRule - Reference properties
-- DecompositionRule.ContainedCategory is a collection property (many to many) of class Category: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" (
  "DecompositionRule" uuid NOT NULL,
  "ContainedCategory" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DecompositionRule_ContainedCategory_PK" PRIMARY KEY("DecompositionRule", "ContainedCategory"),
  CONSTRAINT "DecompositionRule_ContainedCategory_FK_Source" FOREIGN KEY ("DecompositionRule") REFERENCES "SiteDirectory"."DecompositionRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DecompositionRule_ContainedCategory_FK_Target" FOREIGN KEY ("ContainedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_DecompositionRule_ContainedCategory_ValidFrom" ON "SiteDirectory"."DecompositionRule_ContainedCategory" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ContainedCategory_ValidTo" ON "SiteDirectory"."DecompositionRule_ContainedCategory" ("ValidTo");
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_analyze_threshold = 2500);

-- DecompositionRule
-- DecompositionRule.ContainingCategory is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD COLUMN "ContainingCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD CONSTRAINT "DecompositionRule_FK_ContainingCategory" FOREIGN KEY ("ContainingCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- DefinedThing - Reference properties

-- Definition - Reference properties
-- Definition.Example is an ordered collection property of type String: [0..*]
CREATE TABLE "SiteDirectory"."Definition_Example" (
  "Definition" uuid NOT NULL,
  "Example" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Example_PK" PRIMARY KEY("Definition", "Example"),
  CONSTRAINT "Definition_Example_FK_Source" FOREIGN KEY ("Definition") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_Definition_Example_ValidFrom" ON "SiteDirectory"."Definition_Example" ("ValidFrom");
CREATE INDEX "Idx_Definition_Example_ValidTo" ON "SiteDirectory"."Definition_Example" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_analyze_threshold = 2500);

-- Definition
-- Definition.Note is an ordered collection property of type String: [0..*]
CREATE TABLE "SiteDirectory"."Definition_Note" (
  "Definition" uuid NOT NULL,
  "Note" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Note_PK" PRIMARY KEY("Definition", "Note"),
  CONSTRAINT "Definition_Note_FK_Source" FOREIGN KEY ("Definition") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_Definition_Note_ValidFrom" ON "SiteDirectory"."Definition_Note" ("ValidFrom");
CREATE INDEX "Idx_Definition_Note_ValidTo" ON "SiteDirectory"."Definition_Note" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind - Reference properties

-- DerivedUnit - Reference properties

-- DiscussionItem - Reference properties
-- DiscussionItem.ReplyTo is an optional association to DiscussionItem: [0..1]
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD COLUMN "ReplyTo" uuid;
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD CONSTRAINT "DiscussionItem_FK_ReplyTo" FOREIGN KEY ("ReplyTo") REFERENCES "SiteDirectory"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DomainOfExpertise - Reference properties
-- DomainOfExpertise.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Category" (
  "DomainOfExpertise" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DomainOfExpertise_Category_PK" PRIMARY KEY("DomainOfExpertise", "Category"),
  CONSTRAINT "DomainOfExpertise_Category_FK_Source" FOREIGN KEY ("DomainOfExpertise") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DomainOfExpertise_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_DomainOfExpertise_Category_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Category" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_Category_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup - Reference properties
-- DomainOfExpertiseGroup.Domain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" (
  "DomainOfExpertiseGroup" uuid NOT NULL,
  "Domain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DomainOfExpertiseGroup_Domain_PK" PRIMARY KEY("DomainOfExpertiseGroup", "Domain"),
  CONSTRAINT "DomainOfExpertiseGroup_Domain_FK_Source" FOREIGN KEY ("DomainOfExpertiseGroup") REFERENCES "SiteDirectory"."DomainOfExpertiseGroup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DomainOfExpertiseGroup_Domain_FK_Target" FOREIGN KEY ("Domain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_DomainOfExpertiseGroup_Domain_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_Domain_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress - Reference properties

-- EngineeringModelSetup - Reference properties
-- EngineeringModelSetup.ActiveDomain is a collection property (many to many) of class DomainOfExpertise: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" (
  "EngineeringModelSetup" uuid NOT NULL,
  "ActiveDomain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_PK" PRIMARY KEY("EngineeringModelSetup", "ActiveDomain"),
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_FK_Source" FOREIGN KEY ("EngineeringModelSetup") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_FK_Target" FOREIGN KEY ("ActiveDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomain_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomain_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain" ("ValidTo");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType - Reference properties

-- EnumerationValueDefinition - Reference properties
-- EnumerationValueDefinition is an ordered collection
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD COLUMN "Sequence" bigint NOT NULL;

-- FileType - Reference properties
-- FileType.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."FileType_Category" (
  "FileType" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileType_Category_PK" PRIMARY KEY("FileType", "Category"),
  CONSTRAINT "FileType_Category_FK_Source" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "FileType_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_FileType_Category_ValidFrom" ON "SiteDirectory"."FileType_Category" ("ValidFrom");
CREATE INDEX "Idx_FileType_Category_ValidTo" ON "SiteDirectory"."FileType_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Reference properties

-- Glossary - Reference properties
-- Glossary.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Glossary_Category" (
  "Glossary" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Glossary_Category_PK" PRIMARY KEY("Glossary", "Category"),
  CONSTRAINT "Glossary_Category_FK_Source" FOREIGN KEY ("Glossary") REFERENCES "SiteDirectory"."Glossary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Glossary_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Glossary_Category_ValidFrom" ON "SiteDirectory"."Glossary_Category" ("ValidFrom");
CREATE INDEX "Idx_Glossary_Category_ValidTo" ON "SiteDirectory"."Glossary_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Reference properties

-- IntervalScale - Reference properties

-- IterationSetup - Reference properties
-- IterationSetup.SourceIterationSetup is an optional association to IterationSetup: [0..1]
ALTER TABLE "SiteDirectory"."IterationSetup" ADD COLUMN "SourceIterationSetup" uuid;
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetup_FK_SourceIterationSetup" FOREIGN KEY ("SourceIterationSetup") REFERENCES "SiteDirectory"."IterationSetup" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- LinearConversionUnit - Reference properties

-- LogarithmicScale - Reference properties
-- LogarithmicScale.ReferenceQuantityKind is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD COLUMN "ReferenceQuantityKind" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD CONSTRAINT "LogarithmicScale_FK_ReferenceQuantityKind" FOREIGN KEY ("ReferenceQuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- MappingToReferenceScale - Reference properties
-- MappingToReferenceScale.DependentScaleValue is an association to ScaleValueDefinition: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "DependentScaleValue" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_DependentScaleValue" FOREIGN KEY ("DependentScaleValue") REFERENCES "SiteDirectory"."ScaleValueDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- MappingToReferenceScale
-- MappingToReferenceScale.ReferenceScaleValue is an association to ScaleValueDefinition: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "ReferenceScaleValue" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_ReferenceScaleValue" FOREIGN KEY ("ReferenceScaleValue") REFERENCES "SiteDirectory"."ScaleValueDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- MeasurementScale - Reference properties
-- MeasurementScale.Unit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD COLUMN "Unit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScale_FK_Unit" FOREIGN KEY ("Unit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- MeasurementUnit - Reference properties

-- ModelReferenceDataLibrary - Reference properties

-- MultiRelationshipRule - Reference properties
-- MultiRelationshipRule.RelatedCategory is a collection property (many to many) of class Category: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" (
  "MultiRelationshipRule" uuid NOT NULL,
  "RelatedCategory" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_PK" PRIMARY KEY("MultiRelationshipRule", "RelatedCategory"),
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_FK_Source" FOREIGN KEY ("MultiRelationshipRule") REFERENCES "SiteDirectory"."MultiRelationshipRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_FK_Target" FOREIGN KEY ("RelatedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategory_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategory_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory" ("ValidTo");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationshipRule
-- MultiRelationshipRule.RelationshipCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD COLUMN "RelationshipCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD CONSTRAINT "MultiRelationshipRule_FK_RelationshipCategory" FOREIGN KEY ("RelationshipCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- NaturalLanguage - Reference properties

-- OrdinalScale - Reference properties

-- Organization - Reference properties

-- ParameterizedCategoryRule - Reference properties
-- ParameterizedCategoryRule.Category is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD COLUMN "Category" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD CONSTRAINT "ParameterizedCategoryRule_FK_Category" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterizedCategoryRule
-- ParameterizedCategoryRule.ParameterType is a collection property (many to many) of class ParameterType: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" (
  "ParameterizedCategoryRule" uuid NOT NULL,
  "ParameterType" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_PK" PRIMARY KEY("ParameterizedCategoryRule", "ParameterType"),
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_FK_Source" FOREIGN KEY ("ParameterizedCategoryRule") REFERENCES "SiteDirectory"."ParameterizedCategoryRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_FK_Target" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterType_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterType_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType - Reference properties
-- ParameterType.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterType_Category" (
  "ParameterType" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterType_Category_PK" PRIMARY KEY("ParameterType", "Category"),
  CONSTRAINT "ParameterType_Category_FK_Source" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ParameterType_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ParameterType_Category_ValidFrom" ON "SiteDirectory"."ParameterType_Category" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_Category_ValidTo" ON "SiteDirectory"."ParameterType_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_analyze_threshold = 2500);

-- ParameterTypeComponent - Reference properties
-- ParameterTypeComponent.ParameterType is an association to ParameterType: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterTypeComponent
-- ParameterTypeComponent.Scale is an optional association to MeasurementScale: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Scale" uuid;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterTypeComponent is an ordered collection
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Sequence" bigint NOT NULL;

-- Participant - Reference properties
-- Participant.Domain is a collection property (many to many) of class DomainOfExpertise: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."Participant_Domain" (
  "Participant" uuid NOT NULL,
  "Domain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Participant_Domain_PK" PRIMARY KEY("Participant", "Domain"),
  CONSTRAINT "Participant_Domain_FK_Source" FOREIGN KEY ("Participant") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Participant_Domain_FK_Target" FOREIGN KEY ("Domain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Participant_Domain_ValidFrom" ON "SiteDirectory"."Participant_Domain" ("ValidFrom");
CREATE INDEX "Idx_Participant_Domain_ValidTo" ON "SiteDirectory"."Participant_Domain" ("ValidTo");
ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_analyze_threshold = 2500);

-- Participant
-- Participant.Person is an association to Person: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Person" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Person" FOREIGN KEY ("Person") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Participant
-- Participant.Role is an association to ParticipantRole: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Role" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Role" FOREIGN KEY ("Role") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Participant
-- Participant.SelectedDomain is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "SelectedDomain" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_SelectedDomain" FOREIGN KEY ("SelectedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParticipantPermission - Reference properties

-- ParticipantRole - Reference properties

-- Person - Reference properties
-- Person.DefaultDomain is an optional association to DomainOfExpertise: [0..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultDomain" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultDomain" FOREIGN KEY ("DefaultDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Person
-- Person.DefaultEmailAddress is an optional association to EmailAddress: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultEmailAddress" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultEmailAddress" FOREIGN KEY ("DefaultEmailAddress") REFERENCES "SiteDirectory"."EmailAddress" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Person
-- Person.DefaultTelephoneNumber is an optional association to TelephoneNumber: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultTelephoneNumber" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultTelephoneNumber" FOREIGN KEY ("DefaultTelephoneNumber") REFERENCES "SiteDirectory"."TelephoneNumber" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Person
-- Person.Organization is an optional association to Organization: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Organization" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Organization" FOREIGN KEY ("Organization") REFERENCES "SiteDirectory"."Organization" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Person
-- Person.Role is an optional association to PersonRole: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Role" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Role" FOREIGN KEY ("Role") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- PersonPermission - Reference properties

-- PersonRole - Reference properties

-- PrefixedUnit - Reference properties
-- PrefixedUnit.Prefix is an association to UnitPrefix: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD COLUMN "Prefix" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD CONSTRAINT "PrefixedUnit_FK_Prefix" FOREIGN KEY ("Prefix") REFERENCES "SiteDirectory"."UnitPrefix" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- QuantityKind - Reference properties
-- QuantityKind.DefaultScale is an association to MeasurementScale: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."QuantityKind" ADD COLUMN "DefaultScale" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKind" ADD CONSTRAINT "QuantityKind_FK_DefaultScale" FOREIGN KEY ("DefaultScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- QuantityKind
-- QuantityKind.PossibleScale is a collection property (many to many) of class MeasurementScale: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."QuantityKind_PossibleScale" (
  "QuantityKind" uuid NOT NULL,
  "PossibleScale" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "QuantityKind_PossibleScale_PK" PRIMARY KEY("QuantityKind", "PossibleScale"),
  CONSTRAINT "QuantityKind_PossibleScale_FK_Source" FOREIGN KEY ("QuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "QuantityKind_PossibleScale_FK_Target" FOREIGN KEY ("PossibleScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_QuantityKind_PossibleScale_ValidFrom" ON "SiteDirectory"."QuantityKind_PossibleScale" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_PossibleScale_ValidTo" ON "SiteDirectory"."QuantityKind_PossibleScale" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKindFactor - Reference properties
-- QuantityKindFactor.QuantityKind is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "QuantityKind" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactor_FK_QuantityKind" FOREIGN KEY ("QuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- QuantityKindFactor is an ordered collection
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "Sequence" bigint NOT NULL;

-- RatioScale - Reference properties

-- ReferenceDataLibrary - Reference properties
-- ReferenceDataLibrary.BaseQuantityKind is an ordered collection property (many to many) of class QuantityKind: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" (
  "ReferenceDataLibrary" uuid NOT NULL,
  "BaseQuantityKind" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_PK" PRIMARY KEY("ReferenceDataLibrary", "BaseQuantityKind"),
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_FK_Source" FOREIGN KEY ("ReferenceDataLibrary") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_FK_Target" FOREIGN KEY ("BaseQuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKind_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKind_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary
-- ReferenceDataLibrary.BaseUnit is a collection property (many to many) of class MeasurementUnit: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" (
  "ReferenceDataLibrary" uuid NOT NULL,
  "BaseUnit" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_PK" PRIMARY KEY("ReferenceDataLibrary", "BaseUnit"),
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_FK_Source" FOREIGN KEY ("ReferenceDataLibrary") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_FK_Target" FOREIGN KEY ("BaseUnit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary
-- ReferenceDataLibrary.RequiredRdl is an optional association to SiteReferenceDataLibrary: [0..1]-[1..*]
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD COLUMN "RequiredRdl" uuid;
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD CONSTRAINT "ReferenceDataLibrary_FK_RequiredRdl" FOREIGN KEY ("RequiredRdl") REFERENCES "SiteDirectory"."SiteReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ReferencerRule - Reference properties
-- ReferencerRule.ReferencedCategory is a collection property (many to many) of class Category: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" (
  "ReferencerRule" uuid NOT NULL,
  "ReferencedCategory" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferencerRule_ReferencedCategory_PK" PRIMARY KEY("ReferencerRule", "ReferencedCategory"),
  CONSTRAINT "ReferencerRule_ReferencedCategory_FK_Source" FOREIGN KEY ("ReferencerRule") REFERENCES "SiteDirectory"."ReferencerRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferencerRule_ReferencedCategory_FK_Target" FOREIGN KEY ("ReferencedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ReferencerRule_ReferencedCategory_ValidFrom" ON "SiteDirectory"."ReferencerRule_ReferencedCategory" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ReferencedCategory_ValidTo" ON "SiteDirectory"."ReferencerRule_ReferencedCategory" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_analyze_threshold = 2500);

-- ReferencerRule
-- ReferencerRule.ReferencingCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD COLUMN "ReferencingCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD CONSTRAINT "ReferencerRule_FK_ReferencingCategory" FOREIGN KEY ("ReferencingCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ReferenceSource - Reference properties
-- ReferenceSource.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ReferenceSource_Category" (
  "ReferenceSource" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReferenceSource_Category_PK" PRIMARY KEY("ReferenceSource", "Category"),
  CONSTRAINT "ReferenceSource_Category_FK_Source" FOREIGN KEY ("ReferenceSource") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceSource_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ReferenceSource_Category_ValidFrom" ON "SiteDirectory"."ReferenceSource_Category" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_Category_ValidTo" ON "SiteDirectory"."ReferenceSource_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource
-- ReferenceSource.PublishedIn is an optional association to ReferenceSource: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "PublishedIn" uuid;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_PublishedIn" FOREIGN KEY ("PublishedIn") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ReferenceSource
-- ReferenceSource.Publisher is an optional association to Organization: [0..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "Publisher" uuid;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_Publisher" FOREIGN KEY ("Publisher") REFERENCES "SiteDirectory"."Organization" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Rule - Reference properties

-- ScalarParameterType - Reference properties

-- ScaleReferenceQuantityValue - Reference properties
-- ScaleReferenceQuantityValue.Scale is an association to MeasurementScale: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD COLUMN "Scale" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValue_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ScaleValueDefinition - Reference properties

-- SimpleQuantityKind - Reference properties

-- SimpleUnit - Reference properties

-- SiteDirectory - Reference properties
-- SiteDirectory.DefaultParticipantRole is an optional association to ParticipantRole: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD COLUMN "DefaultParticipantRole" uuid;
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectory_FK_DefaultParticipantRole" FOREIGN KEY ("DefaultParticipantRole") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- SiteDirectory
-- SiteDirectory.DefaultPersonRole is an optional association to PersonRole: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD COLUMN "DefaultPersonRole" uuid;
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectory_FK_DefaultPersonRole" FOREIGN KEY ("DefaultPersonRole") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- SiteDirectoryDataAnnotation - Reference properties
-- SiteDirectoryDataAnnotation.Author is an association to Person: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- SiteDirectoryDataAnnotation
-- SiteDirectoryDataAnnotation.PrimaryAnnotatedThing is an association to SiteDirectoryThingReference: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "PrimaryAnnotatedThing" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_PrimaryAnnotatedThing" FOREIGN KEY ("PrimaryAnnotatedThing") REFERENCES "SiteDirectory"."SiteDirectoryThingReference" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- SiteDirectoryDataDiscussionItem - Reference properties
-- SiteDirectoryDataDiscussionItem.Author is an association to Person: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItem_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- SiteDirectoryThingReference - Reference properties

-- SiteLogEntry - Reference properties
-- SiteLogEntry.AffectedItemIid is a collection property of type Guid: [0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" (
  "SiteLogEntry" uuid NOT NULL,
  "AffectedItemIid" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteLogEntry_AffectedItemIid_PK" PRIMARY KEY("SiteLogEntry", "AffectedItemIid"),
  CONSTRAINT "SiteLogEntry_AffectedItemIid_FK_Source" FOREIGN KEY ("SiteLogEntry") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIid_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIid_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry
-- SiteLogEntry.Author is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD COLUMN "Author" uuid;
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntry_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- SiteLogEntry
-- SiteLogEntry.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_Category" (
  "SiteLogEntry" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SiteLogEntry_Category_PK" PRIMARY KEY("SiteLogEntry", "Category"),
  CONSTRAINT "SiteLogEntry_Category_FK_Source" FOREIGN KEY ("SiteLogEntry") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "SiteLogEntry_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_SiteLogEntry_Category_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Category" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_Category_ValidTo" ON "SiteDirectory"."SiteLogEntry_Category" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary - Reference properties

-- SpecializedQuantityKind - Reference properties
-- SpecializedQuantityKind.General is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD COLUMN "General" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD CONSTRAINT "SpecializedQuantityKind_FK_General" FOREIGN KEY ("General") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- TelephoneNumber - Reference properties
-- TelephoneNumber.VcardType is a collection property of type VcardTelephoneNumberKind: [0..*]
CREATE TABLE "SiteDirectory"."TelephoneNumber_VcardType" (
  "TelephoneNumber" uuid NOT NULL,
  "VcardType" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TelephoneNumber_VcardType_PK" PRIMARY KEY("TelephoneNumber", "VcardType"),
  CONSTRAINT "TelephoneNumber_VcardType_FK_Source" FOREIGN KEY ("TelephoneNumber") REFERENCES "SiteDirectory"."TelephoneNumber" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_TelephoneNumber_VcardType_ValidFrom" ON "SiteDirectory"."TelephoneNumber_VcardType" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_VcardType_ValidTo" ON "SiteDirectory"."TelephoneNumber_VcardType" ("ValidTo");
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_analyze_threshold = 2500);

-- Term - Reference properties

-- TextParameterType - Reference properties

-- Thing - Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedDomain" (
  "Thing" uuid NOT NULL,
  "ExcludedDomain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedDomain_PK" PRIMARY KEY("Thing", "ExcludedDomain"),
  CONSTRAINT "Thing_ExcludedDomain_FK_Source" FOREIGN KEY ("Thing") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedDomain_FK_Target" FOREIGN KEY ("ExcludedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidFrom" ON "SiteDirectory"."Thing_ExcludedDomain" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidTo" ON "SiteDirectory"."Thing_ExcludedDomain" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedPerson" (
  "Thing" uuid NOT NULL,
  "ExcludedPerson" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedPerson_PK" PRIMARY KEY("Thing", "ExcludedPerson"),
  CONSTRAINT "Thing_ExcludedPerson_FK_Source" FOREIGN KEY ("Thing") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedPerson_FK_Target" FOREIGN KEY ("ExcludedPerson") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidFrom" ON "SiteDirectory"."Thing_ExcludedPerson" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidTo" ON "SiteDirectory"."Thing_ExcludedPerson" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Reference properties
-- ThingReference.ReferencedThing is an association to Thing: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."ThingReference" ADD COLUMN "ReferencedThing" uuid NOT NULL;

-- TimeOfDayParameterType - Reference properties

-- TopContainer - Reference properties

-- UnitFactor - Reference properties
-- UnitFactor.Unit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Unit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactor_FK_Unit" FOREIGN KEY ("Unit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- UnitFactor is an ordered collection
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Sequence" bigint NOT NULL;

-- UnitPrefix - Reference properties

-- UserPreference - Reference properties

--------------------------------------------------------------------------------------------------------
------------------------------------------- Revision-History Tables ------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - revision history table
CREATE TABLE "SiteDirectory"."Alias_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ArrayParameterType - revision history table
CREATE TABLE "SiteDirectory"."ArrayParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArrayParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule - revision history table
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationshipRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BooleanParameterType - revision history table
CREATE TABLE "SiteDirectory"."BooleanParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BooleanParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Category - revision history table
CREATE TABLE "SiteDirectory"."Category_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Category_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Citation - revision history table
CREATE TABLE "SiteDirectory"."Citation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_analyze_threshold = 2500);

-- CompoundParameterType - revision history table
CREATE TABLE "SiteDirectory"."CompoundParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CompoundParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Constant - revision history table
CREATE TABLE "SiteDirectory"."Constant_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Constant_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ConversionBasedUnit - revision history table
-- the ConversionBasedUnit class is abstract and therefore does not have a Revision table

-- CyclicRatioScale - revision history table
CREATE TABLE "SiteDirectory"."CyclicRatioScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CyclicRatioScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DateParameterType - revision history table
CREATE TABLE "SiteDirectory"."DateParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DateTimeParameterType - revision history table
CREATE TABLE "SiteDirectory"."DateTimeParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateTimeParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DecompositionRule - revision history table
CREATE TABLE "SiteDirectory"."DecompositionRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DecompositionRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - revision history table
-- the DefinedThing class is abstract and therefore does not have a Revision table

-- Definition - revision history table
CREATE TABLE "SiteDirectory"."Definition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind - revision history table
CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DerivedUnit - revision history table
CREATE TABLE "SiteDirectory"."DerivedUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem - revision history table
-- the DiscussionItem class is abstract and therefore does not have a Revision table

-- DomainOfExpertise - revision history table
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertise_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup - revision history table
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertiseGroup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress - revision history table
CREATE TABLE "SiteDirectory"."EmailAddress_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EmailAddress_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelSetup - revision history table
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelSetup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType - revision history table
CREATE TABLE "SiteDirectory"."EnumerationParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationValueDefinition - revision history table
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationValueDefinition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_analyze_threshold = 2500);

-- FileType - revision history table
CREATE TABLE "SiteDirectory"."FileType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - revision history table
-- the GenericAnnotation class is abstract and therefore does not have a Revision table

-- Glossary - revision history table
CREATE TABLE "SiteDirectory"."Glossary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Glossary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - revision history table
CREATE TABLE "SiteDirectory"."HyperLink_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_analyze_threshold = 2500);

-- IntervalScale - revision history table
CREATE TABLE "SiteDirectory"."IntervalScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IntervalScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- IterationSetup - revision history table
CREATE TABLE "SiteDirectory"."IterationSetup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IterationSetup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_analyze_threshold = 2500);

-- LinearConversionUnit - revision history table
CREATE TABLE "SiteDirectory"."LinearConversionUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LinearConversionUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_analyze_threshold = 2500);

-- LogarithmicScale - revision history table
CREATE TABLE "SiteDirectory"."LogarithmicScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogarithmicScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- MappingToReferenceScale - revision history table
CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MappingToReferenceScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementScale - revision history table
-- the MeasurementScale class is abstract and therefore does not have a Revision table

-- MeasurementUnit - revision history table
-- the MeasurementUnit class is abstract and therefore does not have a Revision table

-- ModelReferenceDataLibrary - revision history table
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelReferenceDataLibrary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationshipRule - revision history table
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationshipRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_analyze_threshold = 2500);

-- NaturalLanguage - revision history table
CREATE TABLE "SiteDirectory"."NaturalLanguage_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NaturalLanguage_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_analyze_threshold = 2500);

-- OrdinalScale - revision history table
CREATE TABLE "SiteDirectory"."OrdinalScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrdinalScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Organization - revision history table
CREATE TABLE "SiteDirectory"."Organization_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Organization_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterizedCategoryRule - revision history table
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterizedCategoryRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType - revision history table
-- the ParameterType class is abstract and therefore does not have a Revision table

-- ParameterTypeComponent - revision history table
CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterTypeComponent_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Participant - revision history table
CREATE TABLE "SiteDirectory"."Participant_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Participant_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantPermission - revision history table
CREATE TABLE "SiteDirectory"."ParticipantPermission_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantPermission_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantRole - revision history table
CREATE TABLE "SiteDirectory"."ParticipantRole_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantRole_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Person - revision history table
CREATE TABLE "SiteDirectory"."Person_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Person_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_analyze_threshold = 2500);

-- PersonPermission - revision history table
CREATE TABLE "SiteDirectory"."PersonPermission_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonPermission_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_analyze_threshold = 2500);

-- PersonRole - revision history table
CREATE TABLE "SiteDirectory"."PersonRole_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonRole_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_analyze_threshold = 2500);

-- PrefixedUnit - revision history table
CREATE TABLE "SiteDirectory"."PrefixedUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PrefixedUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKind - revision history table
-- the QuantityKind class is abstract and therefore does not have a Revision table

-- QuantityKindFactor - revision history table
CREATE TABLE "SiteDirectory"."QuantityKindFactor_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "QuantityKindFactor_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RatioScale - revision history table
CREATE TABLE "SiteDirectory"."RatioScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RatioScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary - revision history table
-- the ReferenceDataLibrary class is abstract and therefore does not have a Revision table

-- ReferencerRule - revision history table
CREATE TABLE "SiteDirectory"."ReferencerRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferencerRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource - revision history table
CREATE TABLE "SiteDirectory"."ReferenceSource_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferenceSource_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Rule - revision history table
-- the Rule class is abstract and therefore does not have a Revision table

-- ScalarParameterType - revision history table
-- the ScalarParameterType class is abstract and therefore does not have a Revision table

-- ScaleReferenceQuantityValue - revision history table
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleReferenceQuantityValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ScaleValueDefinition - revision history table
CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleValueDefinition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SimpleQuantityKind - revision history table
CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SimpleUnit - revision history table
CREATE TABLE "SiteDirectory"."SimpleUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectory - revision history table
CREATE TABLE "SiteDirectory"."SiteDirectory_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectory_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataAnnotation - revision history table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataAnnotation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataDiscussionItem - revision history table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataDiscussionItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryThingReference - revision history table
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryThingReference_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry - revision history table
CREATE TABLE "SiteDirectory"."SiteLogEntry_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteLogEntry_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary - revision history table
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteReferenceDataLibrary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SpecializedQuantityKind - revision history table
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SpecializedQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);

-- TelephoneNumber - revision history table
CREATE TABLE "SiteDirectory"."TelephoneNumber_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TelephoneNumber_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Term - revision history table
CREATE TABLE "SiteDirectory"."Term_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Term_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_analyze_threshold = 2500);

-- TextParameterType - revision history table
CREATE TABLE "SiteDirectory"."TextParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Thing - revision history table
-- the Thing class is abstract and therefore does not have a Revision table

-- ThingReference - revision history table
-- the ThingReference class is abstract and therefore does not have a Revision table

-- TimeOfDayParameterType - revision history table
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TimeOfDayParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer - revision history table
-- the TopContainer class is abstract and therefore does not have a Revision table

-- UnitFactor - revision history table
CREATE TABLE "SiteDirectory"."UnitFactor_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitFactor_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_analyze_threshold = 2500);

-- UnitPrefix - revision history table
CREATE TABLE "SiteDirectory"."UnitPrefix_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitPrefix_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_analyze_threshold = 2500);

-- UserPreference - revision history table
CREATE TABLE "SiteDirectory"."UserPreference_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserPreference_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------------------- Cache Tables -----------------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Cache table
CREATE TABLE "SiteDirectory"."Alias_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AliasCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ArrayParameterType - Cache table
CREATE TABLE "SiteDirectory"."ArrayParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArrayParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ArrayParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule - Cache table
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationshipRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BinaryRelationshipRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BooleanParameterType - Cache table
CREATE TABLE "SiteDirectory"."BooleanParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BooleanParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BooleanParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Category - Cache table
CREATE TABLE "SiteDirectory"."Category_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Category_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CategoryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Cache table
CREATE TABLE "SiteDirectory"."Citation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CitationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_analyze_threshold = 2500);

-- CompoundParameterType - Cache table
CREATE TABLE "SiteDirectory"."CompoundParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CompoundParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CompoundParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Constant - Cache table
CREATE TABLE "SiteDirectory"."Constant_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Constant_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ConstantCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ConversionBasedUnit - Cache table
-- the ConversionBasedUnit class is abstract and therefore does not have a Cache table

-- CyclicRatioScale - Cache table
CREATE TABLE "SiteDirectory"."CyclicRatioScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CyclicRatioScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CyclicRatioScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DateParameterType - Cache table
CREATE TABLE "SiteDirectory"."DateParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DateParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DateTimeParameterType - Cache table
CREATE TABLE "SiteDirectory"."DateTimeParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateTimeParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DateTimeParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DecompositionRule - Cache table
CREATE TABLE "SiteDirectory"."DecompositionRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DecompositionRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DecompositionRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - Cache table
-- the DefinedThing class is abstract and therefore does not have a Cache table

-- Definition - Cache table
CREATE TABLE "SiteDirectory"."Definition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind - Cache table
CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DerivedQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DerivedUnit - Cache table
CREATE TABLE "SiteDirectory"."DerivedUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DerivedUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem - Cache table
-- the DiscussionItem class is abstract and therefore does not have a Cache table

-- DomainOfExpertise - Cache table
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertise_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DomainOfExpertiseCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup - Cache table
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertiseGroup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DomainOfExpertiseGroupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress - Cache table
CREATE TABLE "SiteDirectory"."EmailAddress_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EmailAddress_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EmailAddressCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelSetup - Cache table
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelSetup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EngineeringModelSetupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType - Cache table
CREATE TABLE "SiteDirectory"."EnumerationParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EnumerationParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationValueDefinition - Cache table
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationValueDefinition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EnumerationValueDefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_analyze_threshold = 2500);

-- FileType - Cache table
CREATE TABLE "SiteDirectory"."FileType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Cache table
-- the GenericAnnotation class is abstract and therefore does not have a Cache table

-- Glossary - Cache table
CREATE TABLE "SiteDirectory"."Glossary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Glossary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "GlossaryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Cache table
CREATE TABLE "SiteDirectory"."HyperLink_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "HyperLinkCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_analyze_threshold = 2500);

-- IntervalScale - Cache table
CREATE TABLE "SiteDirectory"."IntervalScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IntervalScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IntervalScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- IterationSetup - Cache table
CREATE TABLE "SiteDirectory"."IterationSetup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IterationSetup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IterationSetupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- LinearConversionUnit - Cache table
CREATE TABLE "SiteDirectory"."LinearConversionUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LinearConversionUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LinearConversionUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_analyze_threshold = 2500);

-- LogarithmicScale - Cache table
CREATE TABLE "SiteDirectory"."LogarithmicScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogarithmicScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LogarithmicScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- MappingToReferenceScale - Cache table
CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MappingToReferenceScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "MappingToReferenceScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementScale - Cache table
-- the MeasurementScale class is abstract and therefore does not have a Cache table

-- MeasurementUnit - Cache table
-- the MeasurementUnit class is abstract and therefore does not have a Cache table

-- ModelReferenceDataLibrary - Cache table
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelReferenceDataLibrary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ModelReferenceDataLibraryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationshipRule - Cache table
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationshipRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "MultiRelationshipRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_analyze_threshold = 2500);

-- NaturalLanguage - Cache table
CREATE TABLE "SiteDirectory"."NaturalLanguage_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NaturalLanguage_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "NaturalLanguageCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_analyze_threshold = 2500);

-- OrdinalScale - Cache table
CREATE TABLE "SiteDirectory"."OrdinalScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrdinalScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrdinalScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Organization - Cache table
CREATE TABLE "SiteDirectory"."Organization_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Organization_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrganizationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterizedCategoryRule - Cache table
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterizedCategoryRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterizedCategoryRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType - Cache table
-- the ParameterType class is abstract and therefore does not have a Cache table

-- ParameterTypeComponent - Cache table
CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterTypeComponent_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterTypeComponentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Participant - Cache table
CREATE TABLE "SiteDirectory"."Participant_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Participant_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantPermission - Cache table
CREATE TABLE "SiteDirectory"."ParticipantPermission_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantPermission_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantPermissionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantRole - Cache table
CREATE TABLE "SiteDirectory"."ParticipantRole_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantRole_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantRoleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Person - Cache table
CREATE TABLE "SiteDirectory"."Person_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Person_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_analyze_threshold = 2500);

-- PersonPermission - Cache table
CREATE TABLE "SiteDirectory"."PersonPermission_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonPermission_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonPermissionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_analyze_threshold = 2500);

-- PersonRole - Cache table
CREATE TABLE "SiteDirectory"."PersonRole_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonRole_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonRoleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_analyze_threshold = 2500);

-- PrefixedUnit - Cache table
CREATE TABLE "SiteDirectory"."PrefixedUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PrefixedUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PrefixedUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKind - Cache table
-- the QuantityKind class is abstract and therefore does not have a Cache table

-- QuantityKindFactor - Cache table
CREATE TABLE "SiteDirectory"."QuantityKindFactor_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "QuantityKindFactor_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "QuantityKindFactorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RatioScale - Cache table
CREATE TABLE "SiteDirectory"."RatioScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RatioScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RatioScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary - Cache table
-- the ReferenceDataLibrary class is abstract and therefore does not have a Cache table

-- ReferencerRule - Cache table
CREATE TABLE "SiteDirectory"."ReferencerRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferencerRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ReferencerRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource - Cache table
CREATE TABLE "SiteDirectory"."ReferenceSource_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferenceSource_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ReferenceSourceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Rule - Cache table
-- the Rule class is abstract and therefore does not have a Cache table

-- ScalarParameterType - Cache table
-- the ScalarParameterType class is abstract and therefore does not have a Cache table

-- ScaleReferenceQuantityValue - Cache table
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleReferenceQuantityValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ScaleReferenceQuantityValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ScaleValueDefinition - Cache table
CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleValueDefinition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ScaleValueDefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SimpleQuantityKind - Cache table
CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SimpleQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SimpleUnit - Cache table
CREATE TABLE "SiteDirectory"."SimpleUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SimpleUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectory - Cache table
CREATE TABLE "SiteDirectory"."SiteDirectory_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectory_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataAnnotation - Cache table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataAnnotation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryDataAnnotationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataDiscussionItem - Cache table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataDiscussionItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryDataDiscussionItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryThingReference - Cache table
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryThingReference_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryThingReferenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry - Cache table
CREATE TABLE "SiteDirectory"."SiteLogEntry_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteLogEntry_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteLogEntryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary - Cache table
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteReferenceDataLibrary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteReferenceDataLibraryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SpecializedQuantityKind - Cache table
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SpecializedQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SpecializedQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);

-- TelephoneNumber - Cache table
CREATE TABLE "SiteDirectory"."TelephoneNumber_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TelephoneNumber_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TelephoneNumberCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Term - Cache table
CREATE TABLE "SiteDirectory"."Term_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Term_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TermCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_analyze_threshold = 2500);

-- TextParameterType - Cache table
CREATE TABLE "SiteDirectory"."TextParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TextParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Cache table
-- the Thing class is abstract and therefore does not have a Cache table

-- ThingReference - Cache table
-- the ThingReference class is abstract and therefore does not have a Cache table

-- TimeOfDayParameterType - Cache table
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TimeOfDayParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TimeOfDayParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer - Cache table
-- the TopContainer class is abstract and therefore does not have a Cache table

-- UnitFactor - Cache table
CREATE TABLE "SiteDirectory"."UnitFactor_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitFactor_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UnitFactorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_analyze_threshold = 2500);

-- UnitPrefix - Cache table
CREATE TABLE "SiteDirectory"."UnitPrefix_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitPrefix_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UnitPrefixCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_analyze_threshold = 2500);

-- UserPreference - Cache table
CREATE TABLE "SiteDirectory"."UserPreference_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserPreference_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UserPreferenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------------------- Audit Tables -----------------------------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Audit table
CREATE TABLE "SiteDirectory"."Alias_Audit" (LIKE "SiteDirectory"."Alias");
ALTER TABLE "SiteDirectory"."Alias_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AliasAudit_ValidFrom" ON "SiteDirectory"."Alias_Audit" ("ValidFrom");
CREATE INDEX "Idx_AliasAudit_ValidTo" ON "SiteDirectory"."Alias_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ArrayParameterType - Audit table
CREATE TABLE "SiteDirectory"."ArrayParameterType_Audit" (LIKE "SiteDirectory"."ArrayParameterType");
ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArrayParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterTypeAudit_ValidTo" ON "SiteDirectory"."ArrayParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule - Audit table
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" (LIKE "SiteDirectory"."BinaryRelationshipRule");
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BinaryRelationshipRuleAudit_ValidFrom" ON "SiteDirectory"."BinaryRelationshipRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationshipRuleAudit_ValidTo" ON "SiteDirectory"."BinaryRelationshipRule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BooleanParameterType - Audit table
CREATE TABLE "SiteDirectory"."BooleanParameterType_Audit" (LIKE "SiteDirectory"."BooleanParameterType");
ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BooleanParameterTypeAudit_ValidFrom" ON "SiteDirectory"."BooleanParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_BooleanParameterTypeAudit_ValidTo" ON "SiteDirectory"."BooleanParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Category - Audit table
CREATE TABLE "SiteDirectory"."Category_Audit" (LIKE "SiteDirectory"."Category");
ALTER TABLE "SiteDirectory"."Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CategoryAudit_ValidFrom" ON "SiteDirectory"."Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_CategoryAudit_ValidTo" ON "SiteDirectory"."Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Audit table
CREATE TABLE "SiteDirectory"."Citation_Audit" (LIKE "SiteDirectory"."Citation");
ALTER TABLE "SiteDirectory"."Citation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CitationAudit_ValidFrom" ON "SiteDirectory"."Citation_Audit" ("ValidFrom");
CREATE INDEX "Idx_CitationAudit_ValidTo" ON "SiteDirectory"."Citation_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- CompoundParameterType - Audit table
CREATE TABLE "SiteDirectory"."CompoundParameterType_Audit" (LIKE "SiteDirectory"."CompoundParameterType");
ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CompoundParameterTypeAudit_ValidFrom" ON "SiteDirectory"."CompoundParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_CompoundParameterTypeAudit_ValidTo" ON "SiteDirectory"."CompoundParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Constant - Audit table
CREATE TABLE "SiteDirectory"."Constant_Audit" (LIKE "SiteDirectory"."Constant");
ALTER TABLE "SiteDirectory"."Constant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ConstantAudit_ValidFrom" ON "SiteDirectory"."Constant_Audit" ("ValidFrom");
CREATE INDEX "Idx_ConstantAudit_ValidTo" ON "SiteDirectory"."Constant_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ConversionBasedUnit - Audit table
CREATE TABLE "SiteDirectory"."ConversionBasedUnit_Audit" (LIKE "SiteDirectory"."ConversionBasedUnit");
ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ConversionBasedUnitAudit_ValidFrom" ON "SiteDirectory"."ConversionBasedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_ConversionBasedUnitAudit_ValidTo" ON "SiteDirectory"."ConversionBasedUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- CyclicRatioScale - Audit table
CREATE TABLE "SiteDirectory"."CyclicRatioScale_Audit" (LIKE "SiteDirectory"."CyclicRatioScale");
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CyclicRatioScaleAudit_ValidFrom" ON "SiteDirectory"."CyclicRatioScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_CyclicRatioScaleAudit_ValidTo" ON "SiteDirectory"."CyclicRatioScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DateParameterType - Audit table
CREATE TABLE "SiteDirectory"."DateParameterType_Audit" (LIKE "SiteDirectory"."DateParameterType");
ALTER TABLE "SiteDirectory"."DateParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DateParameterTypeAudit_ValidFrom" ON "SiteDirectory"."DateParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_DateParameterTypeAudit_ValidTo" ON "SiteDirectory"."DateParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DateTimeParameterType - Audit table
CREATE TABLE "SiteDirectory"."DateTimeParameterType_Audit" (LIKE "SiteDirectory"."DateTimeParameterType");
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DateTimeParameterTypeAudit_ValidFrom" ON "SiteDirectory"."DateTimeParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_DateTimeParameterTypeAudit_ValidTo" ON "SiteDirectory"."DateTimeParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DecompositionRule - Audit table
CREATE TABLE "SiteDirectory"."DecompositionRule_Audit" (LIKE "SiteDirectory"."DecompositionRule");
ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DecompositionRuleAudit_ValidFrom" ON "SiteDirectory"."DecompositionRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRuleAudit_ValidTo" ON "SiteDirectory"."DecompositionRule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - Audit table
CREATE TABLE "SiteDirectory"."DefinedThing_Audit" (LIKE "SiteDirectory"."DefinedThing");
ALTER TABLE "SiteDirectory"."DefinedThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinedThingAudit_ValidFrom" ON "SiteDirectory"."DefinedThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinedThingAudit_ValidTo" ON "SiteDirectory"."DefinedThing_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Definition - Audit table
CREATE TABLE "SiteDirectory"."Definition_Audit" (LIKE "SiteDirectory"."Definition");
ALTER TABLE "SiteDirectory"."Definition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinitionAudit_ValidFrom" ON "SiteDirectory"."Definition_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinitionAudit_ValidTo" ON "SiteDirectory"."Definition_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind - Audit table
CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Audit" (LIKE "SiteDirectory"."DerivedQuantityKind");
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DerivedQuantityKindAudit_ValidFrom" ON "SiteDirectory"."DerivedQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_DerivedQuantityKindAudit_ValidTo" ON "SiteDirectory"."DerivedQuantityKind_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DerivedUnit - Audit table
CREATE TABLE "SiteDirectory"."DerivedUnit_Audit" (LIKE "SiteDirectory"."DerivedUnit");
ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DerivedUnitAudit_ValidFrom" ON "SiteDirectory"."DerivedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_DerivedUnitAudit_ValidTo" ON "SiteDirectory"."DerivedUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem - Audit table
CREATE TABLE "SiteDirectory"."DiscussionItem_Audit" (LIKE "SiteDirectory"."DiscussionItem");
ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiscussionItemAudit_ValidFrom" ON "SiteDirectory"."DiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItemAudit_ValidTo" ON "SiteDirectory"."DiscussionItem_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertise - Audit table
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Audit" (LIKE "SiteDirectory"."DomainOfExpertise");
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup - Audit table
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" (LIKE "SiteDirectory"."DomainOfExpertiseGroup");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseGroupAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroupAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress - Audit table
CREATE TABLE "SiteDirectory"."EmailAddress_Audit" (LIKE "SiteDirectory"."EmailAddress");
ALTER TABLE "SiteDirectory"."EmailAddress_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EmailAddressAudit_ValidFrom" ON "SiteDirectory"."EmailAddress_Audit" ("ValidFrom");
CREATE INDEX "Idx_EmailAddressAudit_ValidTo" ON "SiteDirectory"."EmailAddress_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelSetup - Audit table
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Audit" (LIKE "SiteDirectory"."EngineeringModelSetup");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelSetupAudit_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetupAudit_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType - Audit table
CREATE TABLE "SiteDirectory"."EnumerationParameterType_Audit" (LIKE "SiteDirectory"."EnumerationParameterType");
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EnumerationParameterTypeAudit_ValidFrom" ON "SiteDirectory"."EnumerationParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_EnumerationParameterTypeAudit_ValidTo" ON "SiteDirectory"."EnumerationParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationValueDefinition - Audit table
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" (LIKE "SiteDirectory"."EnumerationValueDefinition");
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EnumerationValueDefinitionAudit_ValidFrom" ON "SiteDirectory"."EnumerationValueDefinition_Audit" ("ValidFrom");
CREATE INDEX "Idx_EnumerationValueDefinitionAudit_ValidTo" ON "SiteDirectory"."EnumerationValueDefinition_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileType - Audit table
CREATE TABLE "SiteDirectory"."FileType_Audit" (LIKE "SiteDirectory"."FileType");
ALTER TABLE "SiteDirectory"."FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileTypeAudit_ValidFrom" ON "SiteDirectory"."FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileTypeAudit_ValidTo" ON "SiteDirectory"."FileType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Audit table
CREATE TABLE "SiteDirectory"."GenericAnnotation_Audit" (LIKE "SiteDirectory"."GenericAnnotation");
ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GenericAnnotationAudit_ValidFrom" ON "SiteDirectory"."GenericAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotationAudit_ValidTo" ON "SiteDirectory"."GenericAnnotation_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Glossary - Audit table
CREATE TABLE "SiteDirectory"."Glossary_Audit" (LIKE "SiteDirectory"."Glossary");
ALTER TABLE "SiteDirectory"."Glossary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GlossaryAudit_ValidFrom" ON "SiteDirectory"."Glossary_Audit" ("ValidFrom");
CREATE INDEX "Idx_GlossaryAudit_ValidTo" ON "SiteDirectory"."Glossary_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Audit table
CREATE TABLE "SiteDirectory"."HyperLink_Audit" (LIKE "SiteDirectory"."HyperLink");
ALTER TABLE "SiteDirectory"."HyperLink_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_HyperLinkAudit_ValidFrom" ON "SiteDirectory"."HyperLink_Audit" ("ValidFrom");
CREATE INDEX "Idx_HyperLinkAudit_ValidTo" ON "SiteDirectory"."HyperLink_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_analyze_threshold = 2500);

-- IntervalScale - Audit table
CREATE TABLE "SiteDirectory"."IntervalScale_Audit" (LIKE "SiteDirectory"."IntervalScale");
ALTER TABLE "SiteDirectory"."IntervalScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IntervalScaleAudit_ValidFrom" ON "SiteDirectory"."IntervalScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_IntervalScaleAudit_ValidTo" ON "SiteDirectory"."IntervalScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- IterationSetup - Audit table
CREATE TABLE "SiteDirectory"."IterationSetup_Audit" (LIKE "SiteDirectory"."IterationSetup");
ALTER TABLE "SiteDirectory"."IterationSetup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IterationSetupAudit_ValidFrom" ON "SiteDirectory"."IterationSetup_Audit" ("ValidFrom");
CREATE INDEX "Idx_IterationSetupAudit_ValidTo" ON "SiteDirectory"."IterationSetup_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- LinearConversionUnit - Audit table
CREATE TABLE "SiteDirectory"."LinearConversionUnit_Audit" (LIKE "SiteDirectory"."LinearConversionUnit");
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LinearConversionUnitAudit_ValidFrom" ON "SiteDirectory"."LinearConversionUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_LinearConversionUnitAudit_ValidTo" ON "SiteDirectory"."LinearConversionUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- LogarithmicScale - Audit table
CREATE TABLE "SiteDirectory"."LogarithmicScale_Audit" (LIKE "SiteDirectory"."LogarithmicScale");
ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogarithmicScaleAudit_ValidFrom" ON "SiteDirectory"."LogarithmicScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogarithmicScaleAudit_ValidTo" ON "SiteDirectory"."LogarithmicScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- MappingToReferenceScale - Audit table
CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Audit" (LIKE "SiteDirectory"."MappingToReferenceScale");
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MappingToReferenceScaleAudit_ValidFrom" ON "SiteDirectory"."MappingToReferenceScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_MappingToReferenceScaleAudit_ValidTo" ON "SiteDirectory"."MappingToReferenceScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementScale - Audit table
CREATE TABLE "SiteDirectory"."MeasurementScale_Audit" (LIKE "SiteDirectory"."MeasurementScale");
ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MeasurementScaleAudit_ValidFrom" ON "SiteDirectory"."MeasurementScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementScaleAudit_ValidTo" ON "SiteDirectory"."MeasurementScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- MeasurementUnit - Audit table
CREATE TABLE "SiteDirectory"."MeasurementUnit_Audit" (LIKE "SiteDirectory"."MeasurementUnit");
ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MeasurementUnitAudit_ValidFrom" ON "SiteDirectory"."MeasurementUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementUnitAudit_ValidTo" ON "SiteDirectory"."MeasurementUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModelReferenceDataLibrary - Audit table
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."ModelReferenceDataLibrary");
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."ModelReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."ModelReferenceDataLibrary_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationshipRule - Audit table
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Audit" (LIKE "SiteDirectory"."MultiRelationshipRule");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationshipRuleAudit_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRuleAudit_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NaturalLanguage - Audit table
CREATE TABLE "SiteDirectory"."NaturalLanguage_Audit" (LIKE "SiteDirectory"."NaturalLanguage");
ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NaturalLanguageAudit_ValidFrom" ON "SiteDirectory"."NaturalLanguage_Audit" ("ValidFrom");
CREATE INDEX "Idx_NaturalLanguageAudit_ValidTo" ON "SiteDirectory"."NaturalLanguage_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_analyze_threshold = 2500);

-- OrdinalScale - Audit table
CREATE TABLE "SiteDirectory"."OrdinalScale_Audit" (LIKE "SiteDirectory"."OrdinalScale");
ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrdinalScaleAudit_ValidFrom" ON "SiteDirectory"."OrdinalScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrdinalScaleAudit_ValidTo" ON "SiteDirectory"."OrdinalScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Organization - Audit table
CREATE TABLE "SiteDirectory"."Organization_Audit" (LIKE "SiteDirectory"."Organization");
ALTER TABLE "SiteDirectory"."Organization_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrganizationAudit_ValidFrom" ON "SiteDirectory"."Organization_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrganizationAudit_ValidTo" ON "SiteDirectory"."Organization_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterizedCategoryRule - Audit table
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" (LIKE "SiteDirectory"."ParameterizedCategoryRule");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterizedCategoryRuleAudit_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRuleAudit_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType - Audit table
CREATE TABLE "SiteDirectory"."ParameterType_Audit" (LIKE "SiteDirectory"."ParameterType");
ALTER TABLE "SiteDirectory"."ParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeAudit_ValidTo" ON "SiteDirectory"."ParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterTypeComponent - Audit table
CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Audit" (LIKE "SiteDirectory"."ParameterTypeComponent");
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterTypeComponentAudit_ValidFrom" ON "SiteDirectory"."ParameterTypeComponent_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeComponentAudit_ValidTo" ON "SiteDirectory"."ParameterTypeComponent_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Participant - Audit table
CREATE TABLE "SiteDirectory"."Participant_Audit" (LIKE "SiteDirectory"."Participant");
ALTER TABLE "SiteDirectory"."Participant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantAudit_ValidFrom" ON "SiteDirectory"."Participant_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantAudit_ValidTo" ON "SiteDirectory"."Participant_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantPermission - Audit table
CREATE TABLE "SiteDirectory"."ParticipantPermission_Audit" (LIKE "SiteDirectory"."ParticipantPermission");
ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantPermissionAudit_ValidFrom" ON "SiteDirectory"."ParticipantPermission_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantPermissionAudit_ValidTo" ON "SiteDirectory"."ParticipantPermission_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantRole - Audit table
CREATE TABLE "SiteDirectory"."ParticipantRole_Audit" (LIKE "SiteDirectory"."ParticipantRole");
ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantRoleAudit_ValidFrom" ON "SiteDirectory"."ParticipantRole_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantRoleAudit_ValidTo" ON "SiteDirectory"."ParticipantRole_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Person - Audit table
CREATE TABLE "SiteDirectory"."Person_Audit" (LIKE "SiteDirectory"."Person");
ALTER TABLE "SiteDirectory"."Person_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonAudit_ValidFrom" ON "SiteDirectory"."Person_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonAudit_ValidTo" ON "SiteDirectory"."Person_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_analyze_threshold = 2500);

-- PersonPermission - Audit table
CREATE TABLE "SiteDirectory"."PersonPermission_Audit" (LIKE "SiteDirectory"."PersonPermission");
ALTER TABLE "SiteDirectory"."PersonPermission_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonPermissionAudit_ValidFrom" ON "SiteDirectory"."PersonPermission_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonPermissionAudit_ValidTo" ON "SiteDirectory"."PersonPermission_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_analyze_threshold = 2500);

-- PersonRole - Audit table
CREATE TABLE "SiteDirectory"."PersonRole_Audit" (LIKE "SiteDirectory"."PersonRole");
ALTER TABLE "SiteDirectory"."PersonRole_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonRoleAudit_ValidFrom" ON "SiteDirectory"."PersonRole_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonRoleAudit_ValidTo" ON "SiteDirectory"."PersonRole_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_analyze_threshold = 2500);

-- PrefixedUnit - Audit table
CREATE TABLE "SiteDirectory"."PrefixedUnit_Audit" (LIKE "SiteDirectory"."PrefixedUnit");
ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PrefixedUnitAudit_ValidFrom" ON "SiteDirectory"."PrefixedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_PrefixedUnitAudit_ValidTo" ON "SiteDirectory"."PrefixedUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKind - Audit table
CREATE TABLE "SiteDirectory"."QuantityKind_Audit" (LIKE "SiteDirectory"."QuantityKind");
ALTER TABLE "SiteDirectory"."QuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKindAudit_ValidFrom" ON "SiteDirectory"."QuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindAudit_ValidTo" ON "SiteDirectory"."QuantityKind_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKindFactor - Audit table
CREATE TABLE "SiteDirectory"."QuantityKindFactor_Audit" (LIKE "SiteDirectory"."QuantityKindFactor");
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKindFactorAudit_ValidFrom" ON "SiteDirectory"."QuantityKindFactor_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindFactorAudit_ValidTo" ON "SiteDirectory"."QuantityKindFactor_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RatioScale - Audit table
CREATE TABLE "SiteDirectory"."RatioScale_Audit" (LIKE "SiteDirectory"."RatioScale");
ALTER TABLE "SiteDirectory"."RatioScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RatioScaleAudit_ValidFrom" ON "SiteDirectory"."RatioScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_RatioScaleAudit_ValidTo" ON "SiteDirectory"."RatioScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary - Audit table
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferencerRule - Audit table
CREATE TABLE "SiteDirectory"."ReferencerRule_Audit" (LIKE "SiteDirectory"."ReferencerRule");
ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferencerRuleAudit_ValidFrom" ON "SiteDirectory"."ReferencerRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRuleAudit_ValidTo" ON "SiteDirectory"."ReferencerRule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource - Audit table
CREATE TABLE "SiteDirectory"."ReferenceSource_Audit" (LIKE "SiteDirectory"."ReferenceSource");
ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceSourceAudit_ValidFrom" ON "SiteDirectory"."ReferenceSource_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSourceAudit_ValidTo" ON "SiteDirectory"."ReferenceSource_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Rule - Audit table
CREATE TABLE "SiteDirectory"."Rule_Audit" (LIKE "SiteDirectory"."Rule");
ALTER TABLE "SiteDirectory"."Rule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleAudit_ValidFrom" ON "SiteDirectory"."Rule_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleAudit_ValidTo" ON "SiteDirectory"."Rule_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ScalarParameterType - Audit table
CREATE TABLE "SiteDirectory"."ScalarParameterType_Audit" (LIKE "SiteDirectory"."ScalarParameterType");
ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScalarParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ScalarParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScalarParameterTypeAudit_ValidTo" ON "SiteDirectory"."ScalarParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ScaleReferenceQuantityValue - Audit table
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" (LIKE "SiteDirectory"."ScaleReferenceQuantityValue");
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScaleReferenceQuantityValueAudit_ValidFrom" ON "SiteDirectory"."ScaleReferenceQuantityValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScaleReferenceQuantityValueAudit_ValidTo" ON "SiteDirectory"."ScaleReferenceQuantityValue_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ScaleValueDefinition - Audit table
CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Audit" (LIKE "SiteDirectory"."ScaleValueDefinition");
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScaleValueDefinitionAudit_ValidFrom" ON "SiteDirectory"."ScaleValueDefinition_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScaleValueDefinitionAudit_ValidTo" ON "SiteDirectory"."ScaleValueDefinition_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SimpleQuantityKind - Audit table
CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Audit" (LIKE "SiteDirectory"."SimpleQuantityKind");
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleQuantityKindAudit_ValidFrom" ON "SiteDirectory"."SimpleQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleQuantityKindAudit_ValidTo" ON "SiteDirectory"."SimpleQuantityKind_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SimpleUnit - Audit table
CREATE TABLE "SiteDirectory"."SimpleUnit_Audit" (LIKE "SiteDirectory"."SimpleUnit");
ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleUnitAudit_ValidFrom" ON "SiteDirectory"."SimpleUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleUnitAudit_ValidTo" ON "SiteDirectory"."SimpleUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectory - Audit table
CREATE TABLE "SiteDirectory"."SiteDirectory_Audit" (LIKE "SiteDirectory"."SiteDirectory");
ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryAudit_ValidFrom" ON "SiteDirectory"."SiteDirectory_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryAudit_ValidTo" ON "SiteDirectory"."SiteDirectory_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataAnnotation - Audit table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" (LIKE "SiteDirectory"."SiteDirectoryDataAnnotation");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryDataAnnotationAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataAnnotationAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryDataDiscussionItem - Audit table
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" (LIKE "SiteDirectory"."SiteDirectoryDataDiscussionItem");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItemAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItemAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteDirectoryThingReference - Audit table
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" (LIKE "SiteDirectory"."SiteDirectoryThingReference");
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryThingReferenceAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryThingReferenceAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryThingReference_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry - Audit table
CREATE TABLE "SiteDirectory"."SiteLogEntry_Audit" (LIKE "SiteDirectory"."SiteLogEntry");
ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntryAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntryAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary - Audit table
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."SiteReferenceDataLibrary");
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."SiteReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."SiteReferenceDataLibrary_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SpecializedQuantityKind - Audit table
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" (LIKE "SiteDirectory"."SpecializedQuantityKind");
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SpecializedQuantityKindAudit_ValidFrom" ON "SiteDirectory"."SpecializedQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_SpecializedQuantityKindAudit_ValidTo" ON "SiteDirectory"."SpecializedQuantityKind_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TelephoneNumber - Audit table
CREATE TABLE "SiteDirectory"."TelephoneNumber_Audit" (LIKE "SiteDirectory"."TelephoneNumber");
ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TelephoneNumberAudit_ValidFrom" ON "SiteDirectory"."TelephoneNumber_Audit" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumberAudit_ValidTo" ON "SiteDirectory"."TelephoneNumber_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Term - Audit table
CREATE TABLE "SiteDirectory"."Term_Audit" (LIKE "SiteDirectory"."Term");
ALTER TABLE "SiteDirectory"."Term_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TermAudit_ValidFrom" ON "SiteDirectory"."Term_Audit" ("ValidFrom");
CREATE INDEX "Idx_TermAudit_ValidTo" ON "SiteDirectory"."Term_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TextParameterType - Audit table
CREATE TABLE "SiteDirectory"."TextParameterType_Audit" (LIKE "SiteDirectory"."TextParameterType");
ALTER TABLE "SiteDirectory"."TextParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TextParameterTypeAudit_ValidFrom" ON "SiteDirectory"."TextParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TextParameterTypeAudit_ValidTo" ON "SiteDirectory"."TextParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Audit table
CREATE TABLE "SiteDirectory"."Thing_Audit" (LIKE "SiteDirectory"."Thing");
ALTER TABLE "SiteDirectory"."Thing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingAudit_ValidFrom" ON "SiteDirectory"."Thing_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingAudit_ValidTo" ON "SiteDirectory"."Thing_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Audit table
CREATE TABLE "SiteDirectory"."ThingReference_Audit" (LIKE "SiteDirectory"."ThingReference");
ALTER TABLE "SiteDirectory"."ThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingReferenceAudit_ValidFrom" ON "SiteDirectory"."ThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingReferenceAudit_ValidTo" ON "SiteDirectory"."ThingReference_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TimeOfDayParameterType - Audit table
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" (LIKE "SiteDirectory"."TimeOfDayParameterType");
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TimeOfDayParameterTypeAudit_ValidFrom" ON "SiteDirectory"."TimeOfDayParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TimeOfDayParameterTypeAudit_ValidTo" ON "SiteDirectory"."TimeOfDayParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer - Audit table
CREATE TABLE "SiteDirectory"."TopContainer_Audit" (LIKE "SiteDirectory"."TopContainer");
ALTER TABLE "SiteDirectory"."TopContainer_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TopContainerAudit_ValidFrom" ON "SiteDirectory"."TopContainer_Audit" ("ValidFrom");
CREATE INDEX "Idx_TopContainerAudit_ValidTo" ON "SiteDirectory"."TopContainer_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_analyze_threshold = 2500);

-- UnitFactor - Audit table
CREATE TABLE "SiteDirectory"."UnitFactor_Audit" (LIKE "SiteDirectory"."UnitFactor");
ALTER TABLE "SiteDirectory"."UnitFactor_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UnitFactorAudit_ValidFrom" ON "SiteDirectory"."UnitFactor_Audit" ("ValidFrom");
CREATE INDEX "Idx_UnitFactorAudit_ValidTo" ON "SiteDirectory"."UnitFactor_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_analyze_threshold = 2500);

-- UnitPrefix - Audit table
CREATE TABLE "SiteDirectory"."UnitPrefix_Audit" (LIKE "SiteDirectory"."UnitPrefix");
ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UnitPrefixAudit_ValidFrom" ON "SiteDirectory"."UnitPrefix_Audit" ("ValidFrom");
CREATE INDEX "Idx_UnitPrefixAudit_ValidTo" ON "SiteDirectory"."UnitPrefix_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_analyze_threshold = 2500);

-- UserPreference - Audit table
CREATE TABLE "SiteDirectory"."UserPreference_Audit" (LIKE "SiteDirectory"."UserPreference");
ALTER TABLE "SiteDirectory"."UserPreference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UserPreferenceAudit_ValidFrom" ON "SiteDirectory"."UserPreference_Audit" ("ValidFrom");
CREATE INDEX "Idx_UserPreferenceAudit_ValidTo" ON "SiteDirectory"."UserPreference_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------------- Audit Reference Or Ordered Properties ----------------------------
--------------------------------------------------------------------------------------------------------

-- Alias - Audit table Reference properties

-- ArrayParameterType - Audit table Reference properties
-- ArrayParameterType.Dimension is an ordered collection property of type Int: [1..*]
CREATE TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" (LIKE "SiteDirectory"."ArrayParameterType_Dimension");
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArrayParameterType_DimensionAudit_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Dimension_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_DimensionAudit_ValidTo" ON "SiteDirectory"."ArrayParameterType_Dimension_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationshipRule - Audit table Reference properties

-- BooleanParameterType - Audit table Reference properties

-- Category - Audit table Reference properties
-- Category.PermissibleClass is a collection property of type ClassKind: [1..*]
CREATE TABLE "SiteDirectory"."Category_PermissibleClass_Audit" (LIKE "SiteDirectory"."Category_PermissibleClass");
ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Category_PermissibleClassAudit_ValidFrom" ON "SiteDirectory"."Category_PermissibleClass_Audit" ("ValidFrom");
CREATE INDEX "Idx_Category_PermissibleClassAudit_ValidTo" ON "SiteDirectory"."Category_PermissibleClass_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Category
-- Category.SuperCategory is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Category_SuperCategory_Audit" (LIKE "SiteDirectory"."Category_SuperCategory");
ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Category_SuperCategoryAudit_ValidFrom" ON "SiteDirectory"."Category_SuperCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_Category_SuperCategoryAudit_ValidTo" ON "SiteDirectory"."Category_SuperCategory_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Audit table Reference properties

-- CompoundParameterType - Audit table Reference properties

-- Constant - Audit table Reference properties
-- Constant.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Constant_Category_Audit" (LIKE "SiteDirectory"."Constant_Category");
ALTER TABLE "SiteDirectory"."Constant_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Constant_CategoryAudit_ValidFrom" ON "SiteDirectory"."Constant_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Constant_CategoryAudit_ValidTo" ON "SiteDirectory"."Constant_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ConversionBasedUnit - Audit table Reference properties

-- CyclicRatioScale - Audit table Reference properties

-- DateParameterType - Audit table Reference properties

-- DateTimeParameterType - Audit table Reference properties

-- DecompositionRule - Audit table Reference properties
-- DecompositionRule.ContainedCategory is a collection property (many to many) of class Category: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" (LIKE "SiteDirectory"."DecompositionRule_ContainedCategory");
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DecompositionRule_ContainedCategoryAudit_ValidFrom" ON "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ContainedCategoryAudit_ValidTo" ON "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - Audit table Reference properties

-- Definition - Audit table Reference properties
-- Definition.Example is an ordered collection property of type String: [0..*]
CREATE TABLE "SiteDirectory"."Definition_Example_Audit" (LIKE "SiteDirectory"."Definition_Example");
ALTER TABLE "SiteDirectory"."Definition_Example_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_ExampleAudit_ValidFrom" ON "SiteDirectory"."Definition_Example_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_ExampleAudit_ValidTo" ON "SiteDirectory"."Definition_Example_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Definition
-- Definition.Note is an ordered collection property of type String: [0..*]
CREATE TABLE "SiteDirectory"."Definition_Note_Audit" (LIKE "SiteDirectory"."Definition_Note");
ALTER TABLE "SiteDirectory"."Definition_Note_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_NoteAudit_ValidFrom" ON "SiteDirectory"."Definition_Note_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_NoteAudit_ValidTo" ON "SiteDirectory"."Definition_Note_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DerivedQuantityKind - Audit table Reference properties

-- DerivedUnit - Audit table Reference properties

-- DiscussionItem - Audit table Reference properties

-- DomainOfExpertise - Audit table Reference properties
-- DomainOfExpertise.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" (LIKE "SiteDirectory"."DomainOfExpertise_Category");
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertise_CategoryAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_CategoryAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DomainOfExpertiseGroup - Audit table Reference properties
-- DomainOfExpertiseGroup.Domain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" (LIKE "SiteDirectory"."DomainOfExpertiseGroup_Domain");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseGroup_DomainAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_DomainAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EmailAddress - Audit table Reference properties

-- EngineeringModelSetup - Audit table Reference properties
-- EngineeringModelSetup.ActiveDomain is a collection property (many to many) of class DomainOfExpertise: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" (LIKE "SiteDirectory"."EngineeringModelSetup_ActiveDomain");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomainAudit_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomainAudit_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EnumerationParameterType - Audit table Reference properties

-- EnumerationValueDefinition - Audit table Reference properties

-- FileType - Audit table Reference properties
-- FileType.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."FileType_Category_Audit" (LIKE "SiteDirectory"."FileType_Category");
ALTER TABLE "SiteDirectory"."FileType_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileType_CategoryAudit_ValidFrom" ON "SiteDirectory"."FileType_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileType_CategoryAudit_ValidTo" ON "SiteDirectory"."FileType_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Audit table Reference properties

-- Glossary - Audit table Reference properties
-- Glossary.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Glossary_Category_Audit" (LIKE "SiteDirectory"."Glossary_Category");
ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Glossary_CategoryAudit_ValidFrom" ON "SiteDirectory"."Glossary_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Glossary_CategoryAudit_ValidTo" ON "SiteDirectory"."Glossary_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Audit table Reference properties

-- IntervalScale - Audit table Reference properties

-- IterationSetup - Audit table Reference properties

-- LinearConversionUnit - Audit table Reference properties

-- LogarithmicScale - Audit table Reference properties

-- MappingToReferenceScale - Audit table Reference properties

-- MeasurementScale - Audit table Reference properties

-- MeasurementUnit - Audit table Reference properties

-- ModelReferenceDataLibrary - Audit table Reference properties

-- MultiRelationshipRule - Audit table Reference properties
-- MultiRelationshipRule.RelatedCategory is a collection property (many to many) of class Category: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" (LIKE "SiteDirectory"."MultiRelationshipRule_RelatedCategory");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategoryAudit_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategoryAudit_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NaturalLanguage - Audit table Reference properties

-- OrdinalScale - Audit table Reference properties

-- Organization - Audit table Reference properties

-- ParameterizedCategoryRule - Audit table Reference properties
-- ParameterizedCategoryRule.ParameterType is a collection property (many to many) of class ParameterType: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" (LIKE "SiteDirectory"."ParameterizedCategoryRule_ParameterType");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterTypeAudit_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterType - Audit table Reference properties
-- ParameterType.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterType_Category_Audit" (LIKE "SiteDirectory"."ParameterType_Category");
ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterType_CategoryAudit_ValidFrom" ON "SiteDirectory"."ParameterType_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_CategoryAudit_ValidTo" ON "SiteDirectory"."ParameterType_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterTypeComponent - Audit table Reference properties

-- Participant - Audit table Reference properties
-- Participant.Domain is a collection property (many to many) of class DomainOfExpertise: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."Participant_Domain_Audit" (LIKE "SiteDirectory"."Participant_Domain");
ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Participant_DomainAudit_ValidFrom" ON "SiteDirectory"."Participant_Domain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Participant_DomainAudit_ValidTo" ON "SiteDirectory"."Participant_Domain_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParticipantPermission - Audit table Reference properties

-- ParticipantRole - Audit table Reference properties

-- Person - Audit table Reference properties

-- PersonPermission - Audit table Reference properties

-- PersonRole - Audit table Reference properties

-- PrefixedUnit - Audit table Reference properties

-- QuantityKind - Audit table Reference properties
-- QuantityKind.PossibleScale is a collection property (many to many) of class MeasurementScale: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" (LIKE "SiteDirectory"."QuantityKind_PossibleScale");
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKind_PossibleScaleAudit_ValidFrom" ON "SiteDirectory"."QuantityKind_PossibleScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_PossibleScaleAudit_ValidTo" ON "SiteDirectory"."QuantityKind_PossibleScale_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_analyze_threshold = 2500);

-- QuantityKindFactor - Audit table Reference properties

-- RatioScale - Audit table Reference properties

-- ReferenceDataLibrary - Audit table Reference properties
-- ReferenceDataLibrary.BaseQuantityKind is an ordered collection property (many to many) of class QuantityKind: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKindAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKindAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceDataLibrary
-- ReferenceDataLibrary.BaseUnit is a collection property (many to many) of class MeasurementUnit: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary_BaseUnit");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnitAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnitAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferencerRule - Audit table Reference properties
-- ReferencerRule.ReferencedCategory is a collection property (many to many) of class Category: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" (LIKE "SiteDirectory"."ReferencerRule_ReferencedCategory");
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferencerRule_ReferencedCategoryAudit_ValidFrom" ON "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ReferencedCategoryAudit_ValidTo" ON "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReferenceSource - Audit table Reference properties
-- ReferenceSource.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ReferenceSource_Category_Audit" (LIKE "SiteDirectory"."ReferenceSource_Category");
ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceSource_CategoryAudit_ValidFrom" ON "SiteDirectory"."ReferenceSource_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_CategoryAudit_ValidTo" ON "SiteDirectory"."ReferenceSource_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Rule - Audit table Reference properties

-- ScalarParameterType - Audit table Reference properties

-- ScaleReferenceQuantityValue - Audit table Reference properties

-- ScaleValueDefinition - Audit table Reference properties

-- SimpleQuantityKind - Audit table Reference properties

-- SimpleUnit - Audit table Reference properties

-- SiteDirectory - Audit table Reference properties

-- SiteDirectoryDataAnnotation - Audit table Reference properties

-- SiteDirectoryDataDiscussionItem - Audit table Reference properties

-- SiteDirectoryThingReference - Audit table Reference properties

-- SiteLogEntry - Audit table Reference properties
-- SiteLogEntry.AffectedItemIid is a collection property of type Guid: [0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" (LIKE "SiteDirectory"."SiteLogEntry_AffectedItemIid");
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIidAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIidAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteLogEntry
-- SiteLogEntry.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" (LIKE "SiteDirectory"."SiteLogEntry_Category");
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntry_CategoryAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_CategoryAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_Category_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SiteReferenceDataLibrary - Audit table Reference properties

-- SpecializedQuantityKind - Audit table Reference properties

-- TelephoneNumber - Audit table Reference properties
-- TelephoneNumber.VcardType is a collection property of type VcardTelephoneNumberKind: [0..*]
CREATE TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" (LIKE "SiteDirectory"."TelephoneNumber_VcardType");
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TelephoneNumber_VcardTypeAudit_ValidFrom" ON "SiteDirectory"."TelephoneNumber_VcardType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_VcardTypeAudit_ValidTo" ON "SiteDirectory"."TelephoneNumber_VcardType_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Term - Audit table Reference properties

-- TextParameterType - Audit table Reference properties

-- Thing - Audit table Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" (LIKE "SiteDirectory"."Thing_ExcludedDomain");
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidFrom" ON "SiteDirectory"."Thing_ExcludedDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidTo" ON "SiteDirectory"."Thing_ExcludedDomain_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" (LIKE "SiteDirectory"."Thing_ExcludedPerson");
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidFrom" ON "SiteDirectory"."Thing_ExcludedPerson_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidTo" ON "SiteDirectory"."Thing_ExcludedPerson_Audit" ("ValidTo");
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Audit table Reference properties

-- TimeOfDayParameterType - Audit table Reference properties

-- TopContainer - Audit table Reference properties

-- UnitFactor - Audit table Reference properties

-- UnitPrefix - Audit table Reference properties

-- UserPreference - Audit table Reference properties
