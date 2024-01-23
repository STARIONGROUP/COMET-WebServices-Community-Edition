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
------------------------------- EngineeringModel Class tables ------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ActionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ActionItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ActionItem_ValidFrom" ON "EngineeringModel_REPLACE"."ActionItem" ("ValidFrom");
CREATE INDEX "Idx_ActionItem_ValidTo" ON "EngineeringModel_REPLACE"."ActionItem" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" SET (autovacuum_analyze_threshold = 2500);

-- Approval class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Approval" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Approval_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Approval_ValidFrom" ON "EngineeringModel_REPLACE"."Approval" ("ValidFrom");
CREATE INDEX "Idx_Approval_ValidTo" ON "EngineeringModel_REPLACE"."Approval" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Approval" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Approval" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval" SET (autovacuum_analyze_threshold = 2500);

-- BinaryNote class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."BinaryNote" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BinaryNote_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BinaryNote_ValidFrom" ON "EngineeringModel_REPLACE"."BinaryNote" ("ValidFrom");
CREATE INDEX "Idx_BinaryNote_ValidTo" ON "EngineeringModel_REPLACE"."BinaryNote" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" SET (autovacuum_analyze_threshold = 2500);

-- Book class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Book" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Book_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Book_ValidFrom" ON "EngineeringModel_REPLACE"."Book" ("ValidFrom");
CREATE INDEX "Idx_Book_ValidTo" ON "EngineeringModel_REPLACE"."Book" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Book" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book" SET (autovacuum_analyze_threshold = 2500);

-- ChangeProposal class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ChangeProposal" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ChangeProposal_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ChangeProposal_ValidFrom" ON "EngineeringModel_REPLACE"."ChangeProposal" ("ValidFrom");
CREATE INDEX "Idx_ChangeProposal_ValidTo" ON "EngineeringModel_REPLACE"."ChangeProposal" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" SET (autovacuum_analyze_threshold = 2500);

-- ChangeRequest class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ChangeRequest" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ChangeRequest_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ChangeRequest_ValidFrom" ON "EngineeringModel_REPLACE"."ChangeRequest" ("ValidFrom");
CREATE INDEX "Idx_ChangeRequest_ValidTo" ON "EngineeringModel_REPLACE"."ChangeRequest" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest" SET (autovacuum_analyze_threshold = 2500);

-- CommonFileStore class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."CommonFileStore" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "CommonFileStore_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_CommonFileStore_ValidFrom" ON "EngineeringModel_REPLACE"."CommonFileStore" ("ValidFrom");
CREATE INDEX "Idx_CommonFileStore_ValidTo" ON "EngineeringModel_REPLACE"."CommonFileStore" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" SET (autovacuum_analyze_threshold = 2500);

-- ContractChangeNotice class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ContractChangeNotice_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ContractChangeNotice_ValidFrom" ON "EngineeringModel_REPLACE"."ContractChangeNotice" ("ValidFrom");
CREATE INDEX "Idx_ContractChangeNotice_ValidTo" ON "EngineeringModel_REPLACE"."ContractChangeNotice" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" SET (autovacuum_analyze_threshold = 2500);

-- ContractDeviation class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ContractDeviation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ContractDeviation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ContractDeviation_ValidFrom" ON "EngineeringModel_REPLACE"."ContractDeviation" ("ValidFrom");
CREATE INDEX "Idx_ContractDeviation_ValidTo" ON "EngineeringModel_REPLACE"."ContractDeviation" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."DiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiscussionItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiscussionItem_ValidFrom" ON "EngineeringModel_REPLACE"."DiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItem_ValidTo" ON "EngineeringModel_REPLACE"."DiscussionItem" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModel class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModel" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModel_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EngineeringModel_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModel" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModel_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModel" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataAnnotation class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModelDataAnnotation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EngineeringModelDataAnnotation_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataAnnotation_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataDiscussionItem class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModelDataDiscussionItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EngineeringModelDataDiscussionItem_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataDiscussionItem_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataNote class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "EngineeringModelDataNote_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_EngineeringModelDataNote_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataNote" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataNote_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataNote" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" SET (autovacuum_analyze_threshold = 2500);

-- File class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."File" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "File_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_File_ValidFrom" ON "EngineeringModel_REPLACE"."File" ("ValidFrom");
CREATE INDEX "Idx_File_ValidTo" ON "EngineeringModel_REPLACE"."File" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."File" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileRevision_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_FileRevision_ValidFrom" ON "EngineeringModel_REPLACE"."FileRevision" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_ValidTo" ON "EngineeringModel_REPLACE"."FileRevision" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" SET (autovacuum_analyze_threshold = 2500);

-- FileStore class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."FileStore" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileStore_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_FileStore_ValidFrom" ON "EngineeringModel_REPLACE"."FileStore" ("ValidFrom");
CREATE INDEX "Idx_FileStore_ValidTo" ON "EngineeringModel_REPLACE"."FileStore" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" SET (autovacuum_analyze_threshold = 2500);

-- Folder class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."Folder" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Folder_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Folder_ValidFrom" ON "EngineeringModel_REPLACE"."Folder" ("ValidFrom");
CREATE INDEX "Idx_Folder_ValidTo" ON "EngineeringModel_REPLACE"."Folder" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Folder" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Folder" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."GenericAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "GenericAnnotation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_GenericAnnotation_ValidFrom" ON "EngineeringModel_REPLACE"."GenericAnnotation" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotation_ValidTo" ON "EngineeringModel_REPLACE"."GenericAnnotation" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation" SET (autovacuum_analyze_threshold = 2500);

-- Iteration class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."Iteration" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Iteration_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Iteration_ValidFrom" ON "EngineeringModel_REPLACE"."Iteration" ("ValidFrom");
CREATE INDEX "Idx_Iteration_ValidTo" ON "EngineeringModel_REPLACE"."Iteration" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModellingAnnotationItem_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ModellingAnnotationItem_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItem_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" SET (autovacuum_analyze_threshold = 2500);

-- ModellingThingReference class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModellingThingReference_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ModellingThingReference_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingThingReference" ("ValidFrom");
CREATE INDEX "Idx_ModellingThingReference_ValidTo" ON "EngineeringModel_REPLACE"."ModellingThingReference" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModelLogEntry_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ModelLogEntry_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" SET (autovacuum_analyze_threshold = 2500);

-- Note class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Note" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Note_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Note_ValidFrom" ON "EngineeringModel_REPLACE"."Note" ("ValidFrom");
CREATE INDEX "Idx_Note_ValidTo" ON "EngineeringModel_REPLACE"."Note" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Note" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Note" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note" SET (autovacuum_analyze_threshold = 2500);

-- Page class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Page" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Page_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Page_ValidFrom" ON "EngineeringModel_REPLACE"."Page" ("ValidFrom");
CREATE INDEX "Idx_Page_ValidTo" ON "EngineeringModel_REPLACE"."Page" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Page" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page" SET (autovacuum_analyze_threshold = 2500);

-- RequestForDeviation class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."RequestForDeviation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequestForDeviation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequestForDeviation_ValidFrom" ON "EngineeringModel_REPLACE"."RequestForDeviation" ("ValidFrom");
CREATE INDEX "Idx_RequestForDeviation_ValidTo" ON "EngineeringModel_REPLACE"."RequestForDeviation" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation" SET (autovacuum_analyze_threshold = 2500);

-- RequestForWaiver class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."RequestForWaiver" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequestForWaiver_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequestForWaiver_ValidFrom" ON "EngineeringModel_REPLACE"."RequestForWaiver" ("ValidFrom");
CREATE INDEX "Idx_RequestForWaiver_ValidTo" ON "EngineeringModel_REPLACE"."RequestForWaiver" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver" SET (autovacuum_analyze_threshold = 2500);

-- ReviewItemDiscrepancy class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ReviewItemDiscrepancy_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ReviewItemDiscrepancy_ValidFrom" ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" ("ValidFrom");
CREATE INDEX "Idx_ReviewItemDiscrepancy_ValidTo" ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" SET (autovacuum_analyze_threshold = 2500);

-- Section class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Section" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Section_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Section_ValidFrom" ON "EngineeringModel_REPLACE"."Section" ("ValidFrom");
CREATE INDEX "Idx_Section_ValidTo" ON "EngineeringModel_REPLACE"."Section" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Section" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section" SET (autovacuum_analyze_threshold = 2500);

-- Solution class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."Solution" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Solution_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Solution_ValidFrom" ON "EngineeringModel_REPLACE"."Solution" ("ValidFrom");
CREATE INDEX "Idx_Solution_ValidTo" ON "EngineeringModel_REPLACE"."Solution" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Solution" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Solution" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution" SET (autovacuum_analyze_threshold = 2500);

-- TextualNote class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."TextualNote" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TextualNote_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TextualNote_ValidFrom" ON "EngineeringModel_REPLACE"."TextualNote" ("ValidFrom");
CREATE INDEX "Idx_TextualNote_ValidTo" ON "EngineeringModel_REPLACE"."TextualNote" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote" SET (autovacuum_analyze_threshold = 2500);

-- Thing class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."Thing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Thing_ValidFrom" ON "EngineeringModel_REPLACE"."Thing" ("ValidFrom");
CREATE INDEX "Idx_Thing_ValidTo" ON "EngineeringModel_REPLACE"."Thing" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference class - table definition [version 1.1.0]
CREATE TABLE "EngineeringModel_REPLACE"."ThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ThingReference_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ThingReference_ValidFrom" ON "EngineeringModel_REPLACE"."ThingReference" ("ValidFrom");
CREATE INDEX "Idx_ThingReference_ValidTo" ON "EngineeringModel_REPLACE"."ThingReference" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer class - table definition [version 1.0.0]
CREATE TABLE "EngineeringModel_REPLACE"."TopContainer" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "TopContainer_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_TopContainer_ValidFrom" ON "EngineeringModel_REPLACE"."TopContainer" ("ValidFrom");
CREATE INDEX "Idx_TopContainer_ValidTo" ON "EngineeringModel_REPLACE"."TopContainer" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- EngineeringModel Derives Relationships ---------------------------------
--------------------------------------------------------------------------------------------------------

-- Class ActionItem derives from class ModellingAnnotationItem
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" ADD CONSTRAINT "ActionItemDerivesFromModellingAnnotationItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Approval derives from class GenericAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD CONSTRAINT "ApprovalDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BinaryNote derives from class Note
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" ADD CONSTRAINT "BinaryNoteDerivesFromNote" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Note" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Book derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD CONSTRAINT "BookDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ChangeProposal derives from class ModellingAnnotationItem
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" ADD CONSTRAINT "ChangeProposalDerivesFromModellingAnnotationItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ChangeRequest derives from class ContractDeviation
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest" ADD CONSTRAINT "ChangeRequestDerivesFromContractDeviation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ContractDeviation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class CommonFileStore derives from class FileStore
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" ADD CONSTRAINT "CommonFileStoreDerivesFromFileStore" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ContractChangeNotice derives from class ModellingAnnotationItem
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" ADD CONSTRAINT "ContractChangeNoticeDerivesFromModellingAnnotationItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ContractDeviation derives from class ModellingAnnotationItem
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation" ADD CONSTRAINT "ContractDeviationDerivesFromModellingAnnotationItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiscussionItem derives from class GenericAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" ADD CONSTRAINT "DiscussionItemDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EngineeringModel derives from class TopContainer
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" ADD CONSTRAINT "EngineeringModelDerivesFromTopContainer" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."TopContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EngineeringModelDataAnnotation derives from class GenericAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ADD CONSTRAINT "EngineeringModelDataAnnotationDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EngineeringModelDataDiscussionItem derives from class DiscussionItem
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ADD CONSTRAINT "EngineeringModelDataDiscussionItemDerivesFromDiscussionItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class EngineeringModelDataNote derives from class EngineeringModelDataAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" ADD CONSTRAINT "EngineeringModelDataNoteDerivesFromEngineeringModelDataAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class File derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD CONSTRAINT "FileDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class FileRevision derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevisionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class FileStore derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" ADD CONSTRAINT "FileStoreDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Folder derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD CONSTRAINT "FolderDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class GenericAnnotation derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation" ADD CONSTRAINT "GenericAnnotationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Iteration derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD CONSTRAINT "IterationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ModellingAnnotationItem derives from class EngineeringModelDataAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" ADD CONSTRAINT "ModellingAnnotationItemDerivesFromEngineeringModelDataAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ModellingThingReference derives from class ThingReference
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" ADD CONSTRAINT "ModellingThingReferenceDerivesFromThingReference" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ThingReference" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ModelLogEntry derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" ADD CONSTRAINT "ModelLogEntryDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Note derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD CONSTRAINT "NoteDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Page derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD CONSTRAINT "PageDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequestForDeviation derives from class ContractDeviation
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation" ADD CONSTRAINT "RequestForDeviationDerivesFromContractDeviation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ContractDeviation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequestForWaiver derives from class ContractDeviation
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver" ADD CONSTRAINT "RequestForWaiverDerivesFromContractDeviation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ContractDeviation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ReviewItemDiscrepancy derives from class ModellingAnnotationItem
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" ADD CONSTRAINT "ReviewItemDiscrepancyDerivesFromModellingAnnotationItem" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Section derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD CONSTRAINT "SectionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Solution derives from class GenericAnnotation
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD CONSTRAINT "SolutionDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TextualNote derives from class Note
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote" ADD CONSTRAINT "TextualNoteDerivesFromNote" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Note" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Thing
-- The Thing class has no superclass

-- Class ThingReference derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" ADD CONSTRAINT "ThingReferenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class TopContainer derives from class Thing
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer" ADD CONSTRAINT "TopContainerDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

--------------------------------------------------------------------------------------------------------
------------------------------- EngineeringModel Containment Relationships -----------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem containment
-- The ActionItem class is not directly contained

-- Approval containment
-- The Approval class is contained (composite) by the ModellingAnnotationItem class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD CONSTRAINT "Approval_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Approval_Container" ON "EngineeringModel_REPLACE"."Approval" ("Container");

-- BinaryNote containment
-- The BinaryNote class is not directly contained

-- Book containment
-- The Book class is contained (composite) by the EngineeringModel class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD CONSTRAINT "Book_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Book_Container" ON "EngineeringModel_REPLACE"."Book" ("Container");

-- ChangeProposal containment
-- The ChangeProposal class is not directly contained

-- ChangeRequest containment
-- The ChangeRequest class is not directly contained

-- CommonFileStore containment
-- The CommonFileStore class is contained (composite) by the EngineeringModel class: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore" ADD CONSTRAINT "CommonFileStore_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_CommonFileStore_Container" ON "EngineeringModel_REPLACE"."CommonFileStore" ("Container");

-- ContractChangeNotice containment
-- The ContractChangeNotice class is not directly contained

-- ContractDeviation containment
-- The ContractDeviation class is not directly contained

-- DiscussionItem containment
-- The DiscussionItem class is not directly contained

-- EngineeringModel containment
-- The EngineeringModel class is not directly contained

-- EngineeringModelDataAnnotation containment
-- The EngineeringModelDataAnnotation class is not directly contained

-- EngineeringModelDataDiscussionItem containment
-- The EngineeringModelDataDiscussionItem class is contained (composite) by the EngineeringModelDataAnnotation class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ADD CONSTRAINT "EngineeringModelDataDiscussionItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_EngineeringModelDataDiscussionItem_Container" ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ("Container");

-- EngineeringModelDataNote containment
-- The EngineeringModelDataNote class is contained (composite) by the EngineeringModel class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote" ADD CONSTRAINT "EngineeringModelDataNote_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_EngineeringModelDataNote_Container" ON "EngineeringModel_REPLACE"."EngineeringModelDataNote" ("Container");

-- File containment
-- The File class is contained (composite) by the FileStore class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD CONSTRAINT "File_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_File_Container" ON "EngineeringModel_REPLACE"."File" ("Container");

-- FileRevision containment
-- The FileRevision class is contained (composite) by the File class: [1..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."File" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_FileRevision_Container" ON "EngineeringModel_REPLACE"."FileRevision" ("Container");

-- FileStore containment
-- The FileStore class is not directly contained

-- Folder containment
-- The Folder class is contained (composite) by the FileStore class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Folder_Container" ON "EngineeringModel_REPLACE"."Folder" ("Container");

-- GenericAnnotation containment
-- The GenericAnnotation class is not directly contained

-- Iteration containment
-- The Iteration class is contained (composite) by the EngineeringModel class: [1..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD CONSTRAINT "Iteration_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Iteration_Container" ON "EngineeringModel_REPLACE"."Iteration" ("Container");

-- ModellingAnnotationItem containment
-- The ModellingAnnotationItem class is contained (composite) by the EngineeringModel class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" ADD CONSTRAINT "ModellingAnnotationItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ModellingAnnotationItem_Container" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Container");

-- ModellingThingReference containment
-- The ModellingThingReference class is contained (composite) by the EngineeringModelDataAnnotation class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference" ADD CONSTRAINT "ModellingThingReference_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ModellingThingReference_Container" ON "EngineeringModel_REPLACE"."ModellingThingReference" ("Container");

-- ModelLogEntry containment
-- The ModelLogEntry class is contained (composite) by the EngineeringModel class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" ADD CONSTRAINT "ModelLogEntry_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."EngineeringModel" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ModelLogEntry_Container" ON "EngineeringModel_REPLACE"."ModelLogEntry" ("Container");

-- Note containment
-- The Note class is contained (composite) by the Page class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD CONSTRAINT "Note_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Page" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Note_Container" ON "EngineeringModel_REPLACE"."Note" ("Container");

-- Page containment
-- The Page class is contained (composite) by the Section class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD CONSTRAINT "Page_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Section" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Page_Container" ON "EngineeringModel_REPLACE"."Page" ("Container");

-- RequestForDeviation containment
-- The RequestForDeviation class is not directly contained

-- RequestForWaiver containment
-- The RequestForWaiver class is not directly contained

-- ReviewItemDiscrepancy containment
-- The ReviewItemDiscrepancy class is not directly contained

-- Section containment
-- The Section class is contained (composite) by the Book class: [0..*]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD CONSTRAINT "Section_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Book" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Section_Container" ON "EngineeringModel_REPLACE"."Section" ("Container");

-- Solution containment
-- The Solution class is contained (composite) by the ReviewItemDiscrepancy class: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD CONSTRAINT "Solution_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."ReviewItemDiscrepancy" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Solution_Container" ON "EngineeringModel_REPLACE"."Solution" ("Container");

-- TextualNote containment
-- The TextualNote class is not directly contained

-- Thing containment
-- The Thing class is not directly contained

-- ThingReference containment
-- The ThingReference class is not directly contained

-- TopContainer containment
-- The TopContainer class is not directly contained

--------------------------------------------------------------------------------------------------------
------------------------- EngineeringModel Class Reference or Ordered Properties -----------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Reference properties
-- ActionItem.Actionee is an association to Participant: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" ADD COLUMN "Actionee" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem" ADD CONSTRAINT "ActionItem_FK_Actionee" FOREIGN KEY ("Actionee") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Approval - Reference properties
-- Approval.Author is an association to Participant: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD CONSTRAINT "Approval_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Approval
-- Approval.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Approval" ADD CONSTRAINT "Approval_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BinaryNote - Reference properties
-- BinaryNote.FileType is an association to FileType: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" ADD COLUMN "FileType" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote" ADD CONSTRAINT "BinaryNote_FK_FileType" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Book - Reference properties
-- Book.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Book_Category" (
  "Book" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Book_Category_PK" PRIMARY KEY("Book", "Category"),
  CONSTRAINT "Book_Category_FK_Source" FOREIGN KEY ("Book") REFERENCES "EngineeringModel_REPLACE"."Book" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Book_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Book_Category_ValidFrom" ON "EngineeringModel_REPLACE"."Book_Category" ("ValidFrom");
CREATE INDEX "Idx_Book_Category_ValidTo" ON "EngineeringModel_REPLACE"."Book_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category" SET (autovacuum_analyze_threshold = 2500);

-- Book
-- Book.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD CONSTRAINT "Book_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Book is an ordered collection
ALTER TABLE "EngineeringModel_REPLACE"."Book" ADD COLUMN "Sequence" bigint NOT NULL;

-- ChangeProposal - Reference properties
-- ChangeProposal.ChangeRequest is an association to ChangeRequest: [1..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" ADD COLUMN "ChangeRequest" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal" ADD CONSTRAINT "ChangeProposal_FK_ChangeRequest" FOREIGN KEY ("ChangeRequest") REFERENCES "EngineeringModel_REPLACE"."ChangeRequest" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ChangeRequest - Reference properties

-- CommonFileStore - Reference properties

-- ContractChangeNotice - Reference properties
-- ContractChangeNotice.ChangeProposal is an association to ChangeProposal: [1..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" ADD COLUMN "ChangeProposal" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice" ADD CONSTRAINT "ContractChangeNotice_FK_ChangeProposal" FOREIGN KEY ("ChangeProposal") REFERENCES "EngineeringModel_REPLACE"."ChangeProposal" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ContractDeviation - Reference properties

-- DiscussionItem - Reference properties
-- DiscussionItem.ReplyTo is an optional association to DiscussionItem: [0..1]
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" ADD COLUMN "ReplyTo" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem" ADD CONSTRAINT "DiscussionItem_FK_ReplyTo" FOREIGN KEY ("ReplyTo") REFERENCES "EngineeringModel_REPLACE"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- EngineeringModel - Reference properties
-- EngineeringModel.EngineeringModelSetup is an association to EngineeringModelSetup: [1..1]-[0..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel" ADD COLUMN "EngineeringModelSetup" uuid NOT NULL;

-- EngineeringModelDataAnnotation - Reference properties
-- EngineeringModelDataAnnotation.Author is an association to Participant: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ADD CONSTRAINT "EngineeringModelDataAnnotation_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- EngineeringModelDataAnnotation
-- EngineeringModelDataAnnotation.PrimaryAnnotatedThing is an optional association to ModellingThingReference: [0..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ADD COLUMN "PrimaryAnnotatedThing" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation" ADD CONSTRAINT "EngineeringModelDataAnnotation_FK_PrimaryAnnotatedThing" FOREIGN KEY ("PrimaryAnnotatedThing") REFERENCES "EngineeringModel_REPLACE"."ModellingThingReference" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- EngineeringModelDataDiscussionItem - Reference properties
-- EngineeringModelDataDiscussionItem.Author is an association to Participant: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem" ADD CONSTRAINT "EngineeringModelDataDiscussionItem_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- EngineeringModelDataNote - Reference properties

-- File - Reference properties
-- File.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."File_Category" (
  "File" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "File_Category_PK" PRIMARY KEY("File", "Category"),
  CONSTRAINT "File_Category_FK_Source" FOREIGN KEY ("File") REFERENCES "EngineeringModel_REPLACE"."File" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "File_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_File_Category_ValidFrom" ON "EngineeringModel_REPLACE"."File_Category" ("ValidFrom");
CREATE INDEX "Idx_File_Category_ValidTo" ON "EngineeringModel_REPLACE"."File_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."File_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category" SET (autovacuum_analyze_threshold = 2500);

-- File
-- File.LockedBy is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD COLUMN "LockedBy" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD CONSTRAINT "File_FK_LockedBy" FOREIGN KEY ("LockedBy") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- File
-- File.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."File" ADD CONSTRAINT "File_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- FileRevision - Reference properties
-- FileRevision.ContainingFolder is an optional association to Folder: [0..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD COLUMN "ContainingFolder" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_ContainingFolder" FOREIGN KEY ("ContainingFolder") REFERENCES "EngineeringModel_REPLACE"."Folder" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- FileRevision
-- FileRevision.Creator is an association to Participant: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD COLUMN "Creator" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_Creator" FOREIGN KEY ("Creator") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- FileRevision
-- FileRevision.FileType is an ordered collection property (many to many) of class FileType: [1..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision_FileType" (
  "FileRevision" uuid NOT NULL,
  "FileType" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "FileRevision_FileType_PK" PRIMARY KEY("FileRevision", "FileType"),
  CONSTRAINT "FileRevision_FileType_FK_Source" FOREIGN KEY ("FileRevision") REFERENCES "EngineeringModel_REPLACE"."FileRevision" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "FileRevision_FileType_FK_Target" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_FileRevision_FileType_ValidFrom" ON "EngineeringModel_REPLACE"."FileRevision_FileType" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_FileType_ValidTo" ON "EngineeringModel_REPLACE"."FileRevision_FileType" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Reference properties
-- FileStore.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."FileStore" ADD CONSTRAINT "FileStore_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Folder - Reference properties
-- Folder.ContainingFolder is an optional association to Folder: [0..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD COLUMN "ContainingFolder" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_ContainingFolder" FOREIGN KEY ("ContainingFolder") REFERENCES "EngineeringModel_REPLACE"."Folder" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Folder
-- Folder.Creator is an association to Participant: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD COLUMN "Creator" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Creator" FOREIGN KEY ("Creator") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Folder
-- Folder.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- GenericAnnotation - Reference properties

-- Iteration - Reference properties
-- Iteration.DefaultOption is an optional association to Option: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD COLUMN "DefaultOption" uuid;

-- Iteration
-- Iteration.IterationSetup is an association to IterationSetup: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD COLUMN "IterationSetup" uuid NOT NULL;

-- Iteration
-- Iteration.TopElement is an optional association to ElementDefinition: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Iteration" ADD COLUMN "TopElement" uuid;

-- ModellingAnnotationItem - Reference properties
-- ModellingAnnotationItem.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" (
  "ModellingAnnotationItem" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModellingAnnotationItem_Category_PK" PRIMARY KEY("ModellingAnnotationItem", "Category"),
  CONSTRAINT "ModellingAnnotationItem_Category_FK_Source" FOREIGN KEY ("ModellingAnnotationItem") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ModellingAnnotationItem_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ModellingAnnotationItem_Category_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItem_Category_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem
-- ModellingAnnotationItem.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem" ADD CONSTRAINT "ModellingAnnotationItem_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ModellingAnnotationItem
-- ModellingAnnotationItem.SourceAnnotation is a collection property (many to many) of class ModellingAnnotationItem: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" (
  "ModellingAnnotationItem" uuid NOT NULL,
  "SourceAnnotation" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModellingAnnotationItem_SourceAnnotation_PK" PRIMARY KEY("ModellingAnnotationItem", "SourceAnnotation"),
  CONSTRAINT "ModellingAnnotationItem_SourceAnnotation_FK_Source" FOREIGN KEY ("ModellingAnnotationItem") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ModellingAnnotationItem_SourceAnnotation_FK_Target" FOREIGN KEY ("SourceAnnotation") REFERENCES "EngineeringModel_REPLACE"."ModellingAnnotationItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ModellingAnnotationItem_SourceAnnotation_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItem_SourceAnnotation_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation" SET (autovacuum_analyze_threshold = 2500);

-- ModellingThingReference - Reference properties

-- ModelLogEntry - Reference properties
-- ModelLogEntry.AffectedItemIid is a collection property of type Guid: [0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" (
  "ModelLogEntry" uuid NOT NULL,
  "AffectedItemIid" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModelLogEntry_AffectedItemIid_PK" PRIMARY KEY("ModelLogEntry", "AffectedItemIid"),
  CONSTRAINT "ModelLogEntry_AffectedItemIid_FK_Source" FOREIGN KEY ("ModelLogEntry") REFERENCES "EngineeringModel_REPLACE"."ModelLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_ModelLogEntry_AffectedItemIid_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_AffectedItemIid_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry
-- ModelLogEntry.Author is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" ADD COLUMN "Author" uuid;
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry" ADD CONSTRAINT "ModelLogEntry_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ModelLogEntry
-- ModelLogEntry.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category" (
  "ModelLogEntry" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ModelLogEntry_Category_PK" PRIMARY KEY("ModelLogEntry", "Category"),
  CONSTRAINT "ModelLogEntry_Category_FK_Source" FOREIGN KEY ("ModelLogEntry") REFERENCES "EngineeringModel_REPLACE"."ModelLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ModelLogEntry_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ModelLogEntry_Category_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry_Category" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_Category_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category" SET (autovacuum_analyze_threshold = 2500);

-- Note - Reference properties
-- Note.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Note_Category" (
  "Note" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Note_Category_PK" PRIMARY KEY("Note", "Category"),
  CONSTRAINT "Note_Category_FK_Source" FOREIGN KEY ("Note") REFERENCES "EngineeringModel_REPLACE"."Note" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Note_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Note_Category_ValidFrom" ON "EngineeringModel_REPLACE"."Note_Category" ("ValidFrom");
CREATE INDEX "Idx_Note_Category_ValidTo" ON "EngineeringModel_REPLACE"."Note_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category" SET (autovacuum_analyze_threshold = 2500);

-- Note
-- Note.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD CONSTRAINT "Note_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Note is an ordered collection
ALTER TABLE "EngineeringModel_REPLACE"."Note" ADD COLUMN "Sequence" bigint NOT NULL;

-- Page - Reference properties
-- Page.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Page_Category" (
  "Page" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Page_Category_PK" PRIMARY KEY("Page", "Category"),
  CONSTRAINT "Page_Category_FK_Source" FOREIGN KEY ("Page") REFERENCES "EngineeringModel_REPLACE"."Page" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Page_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Page_Category_ValidFrom" ON "EngineeringModel_REPLACE"."Page_Category" ("ValidFrom");
CREATE INDEX "Idx_Page_Category_ValidTo" ON "EngineeringModel_REPLACE"."Page_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category" SET (autovacuum_analyze_threshold = 2500);

-- Page
-- Page.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD CONSTRAINT "Page_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Page is an ordered collection
ALTER TABLE "EngineeringModel_REPLACE"."Page" ADD COLUMN "Sequence" bigint NOT NULL;

-- RequestForDeviation - Reference properties

-- RequestForWaiver - Reference properties

-- ReviewItemDiscrepancy - Reference properties

-- Section - Reference properties
-- Section.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Section_Category" (
  "Section" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Section_Category_PK" PRIMARY KEY("Section", "Category"),
  CONSTRAINT "Section_Category_FK_Source" FOREIGN KEY ("Section") REFERENCES "EngineeringModel_REPLACE"."Section" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Section_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Section_Category_ValidFrom" ON "EngineeringModel_REPLACE"."Section_Category" ("ValidFrom");
CREATE INDEX "Idx_Section_Category_ValidTo" ON "EngineeringModel_REPLACE"."Section_Category" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category" SET (autovacuum_analyze_threshold = 2500);

-- Section
-- Section.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD CONSTRAINT "Section_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Section is an ordered collection
ALTER TABLE "EngineeringModel_REPLACE"."Section" ADD COLUMN "Sequence" bigint NOT NULL;

-- Solution - Reference properties
-- Solution.Author is an association to Participant: [1..1]
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD CONSTRAINT "Solution_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Solution
-- Solution.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "EngineeringModel_REPLACE"."Solution" ADD CONSTRAINT "Solution_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- TextualNote - Reference properties

-- Thing - Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain" (
  "Thing" uuid NOT NULL,
  "ExcludedDomain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedDomain_PK" PRIMARY KEY("Thing", "ExcludedDomain"),
  CONSTRAINT "Thing_ExcludedDomain_FK_Source" FOREIGN KEY ("Thing") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedDomain_FK_Target" FOREIGN KEY ("ExcludedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidFrom" ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidTo" ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson" (
  "Thing" uuid NOT NULL,
  "ExcludedPerson" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedPerson_PK" PRIMARY KEY("Thing", "ExcludedPerson"),
  CONSTRAINT "Thing_ExcludedPerson_FK_Source" FOREIGN KEY ("Thing") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedPerson_FK_Target" FOREIGN KEY ("ExcludedPerson") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidFrom" ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidTo" ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Reference properties
-- ThingReference.ReferencedThing is an association to Thing: [1..1]-[1..1]
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference" ADD COLUMN "ReferencedThing" uuid NOT NULL;

-- TopContainer - Reference properties

--------------------------------------------------------------------------------------------------------
------------------------------- EngineeringModel Revision-History tables -------------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ActionItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActionItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Approval - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Approval_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Approval_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BinaryNote - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."BinaryNote_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryNote_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Book - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Book_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Book_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ChangeProposal - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeProposal_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ChangeProposal_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ChangeRequest - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeRequest_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ChangeRequest_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Revision" SET (autovacuum_analyze_threshold = 2500);

-- CommonFileStore - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."CommonFileStore_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CommonFileStore_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ContractChangeNotice - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ContractChangeNotice_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ContractDeviation - revision history table
-- The ContractDeviation class is abstract and therefore does not have a Revision table

-- DiscussionItem - revision history table
-- The DiscussionItem class is abstract and therefore does not have a Revision table

-- EngineeringModel - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModel_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModel_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataAnnotation - revision history table
-- The EngineeringModelDataAnnotation class is abstract and therefore does not have a Revision table

-- EngineeringModelDataDiscussionItem - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelDataDiscussionItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Revision" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataNote - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelDataNote_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Revision" SET (autovacuum_analyze_threshold = 2500);

-- File - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."File_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "File_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."File_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Revision" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileRevision_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Revision" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - revision history table
-- The FileStore class is abstract and therefore does not have a Revision table

-- Folder - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Folder_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Folder_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Revision" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - revision history table
-- The GenericAnnotation class is abstract and therefore does not have a Revision table

-- Iteration - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Iteration_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Iteration_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem - revision history table
-- The ModellingAnnotationItem class is abstract and therefore does not have a Revision table

-- ModellingThingReference - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModellingThingReference_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelLogEntry_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Note - revision history table
-- The Note class is abstract and therefore does not have a Revision table

-- Page - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Page_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Page_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RequestForDeviation - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequestForDeviation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RequestForWaiver - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequestForWaiver_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ReviewItemDiscrepancy - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReviewItemDiscrepancy_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Section - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Section_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Section_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Solution - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."Solution_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Solution_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Revision" SET (autovacuum_analyze_threshold = 2500);

-- TextualNote - revision history table
CREATE TABLE "EngineeringModel_REPLACE"."TextualNote_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextualNote_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Thing - revision history table
-- The Thing class is abstract and therefore does not have a Revision table

-- ThingReference - revision history table
-- The ThingReference class is abstract and therefore does not have a Revision table

-- TopContainer - revision history table
-- The TopContainer class is abstract and therefore does not have a Revision table

--------------------------------------------------------------------------------------------------------
------------------------------- EngineeringModel Cache tables ------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ActionItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActionItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ActionItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Approval - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Approval_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Approval_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ApprovalCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BinaryNote - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."BinaryNote_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryNote_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BinaryNoteCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Book - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Book_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Book_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BookCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ChangeProposal - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeProposal_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ChangeProposal_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ChangeProposalCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ChangeRequest - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeRequest_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ChangeRequest_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ChangeRequestCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Cache" SET (autovacuum_analyze_threshold = 2500);

-- CommonFileStore - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."CommonFileStore_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CommonFileStore_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CommonFileStoreCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ContractChangeNotice - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ContractChangeNotice_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ContractChangeNoticeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ContractDeviation - Cache table
-- The ContractDeviation class is abstract and therefore does not have a Cache table

-- DiscussionItem - Cache table
-- The DiscussionItem class is abstract and therefore does not have a Cache table

-- EngineeringModel - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModel_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModel_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EngineeringModelCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataAnnotation - Cache table
-- The EngineeringModelDataAnnotation class is abstract and therefore does not have a Cache table

-- EngineeringModelDataDiscussionItem - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelDataDiscussionItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EngineeringModelDataDiscussionItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Cache" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataNote - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelDataNote_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EngineeringModelDataNoteCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Cache" SET (autovacuum_analyze_threshold = 2500);

-- File - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."File_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "File_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."File_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Cache" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileRevision_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileRevisionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Cache" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Cache table
-- The FileStore class is abstract and therefore does not have a Cache table

-- Folder - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Folder_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Folder_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FolderCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Cache" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Cache table
-- The GenericAnnotation class is abstract and therefore does not have a Cache table

-- Iteration - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Iteration_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Iteration_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IterationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem - Cache table
-- The ModellingAnnotationItem class is abstract and therefore does not have a Cache table

-- ModellingThingReference - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModellingThingReference_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ModellingThingReferenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelLogEntry_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ModelLogEntryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Note - Cache table
-- The Note class is abstract and therefore does not have a Cache table

-- Page - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Page_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Page_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PageCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RequestForDeviation - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequestForDeviation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequestForDeviationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RequestForWaiver - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequestForWaiver_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequestForWaiverCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ReviewItemDiscrepancy - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReviewItemDiscrepancy_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ReviewItemDiscrepancyCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Section - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Section_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Section_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SectionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Solution - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."Solution_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Solution_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SolutionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Cache" SET (autovacuum_analyze_threshold = 2500);

-- TextualNote - Cache table
CREATE TABLE "EngineeringModel_REPLACE"."TextualNote_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextualNote_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TextualNoteCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "EngineeringModel_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Cache table
-- The Thing class is abstract and therefore does not have a Cache table

-- ThingReference - Cache table
-- The ThingReference class is abstract and therefore does not have a Cache table

-- TopContainer - Cache table
-- The TopContainer class is abstract and therefore does not have a Cache table

--------------------------------------------------------------------------------------------------------
------------------------------- EngineeringModel Audit class -------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" (LIKE "EngineeringModel_REPLACE"."ActionItem");
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActionItemAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ActionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActionItemAudit_ValidTo" ON "EngineeringModel_REPLACE"."ActionItem_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ActionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Approval - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Approval_Audit" (LIKE "EngineeringModel_REPLACE"."Approval");
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ApprovalAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Approval_Audit" ("ValidFrom");
CREATE INDEX "Idx_ApprovalAudit_ValidTo" ON "EngineeringModel_REPLACE"."Approval_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Approval_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BinaryNote - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" (LIKE "EngineeringModel_REPLACE"."BinaryNote");
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BinaryNoteAudit_ValidFrom" ON "EngineeringModel_REPLACE"."BinaryNote_Audit" ("ValidFrom");
CREATE INDEX "Idx_BinaryNoteAudit_ValidTo" ON "EngineeringModel_REPLACE"."BinaryNote_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."BinaryNote_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Book - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Book_Audit" (LIKE "EngineeringModel_REPLACE"."Book");
ALTER TABLE "EngineeringModel_REPLACE"."Book_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BookAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Book_Audit" ("ValidFrom");
CREATE INDEX "Idx_BookAudit_ValidTo" ON "EngineeringModel_REPLACE"."Book_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Book_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ChangeProposal - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" (LIKE "EngineeringModel_REPLACE"."ChangeProposal");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ChangeProposalAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ChangeProposal_Audit" ("ValidFrom");
CREATE INDEX "Idx_ChangeProposalAudit_ValidTo" ON "EngineeringModel_REPLACE"."ChangeProposal_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeProposal_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ChangeRequest - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" (LIKE "EngineeringModel_REPLACE"."ChangeRequest");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ChangeRequestAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ChangeRequest_Audit" ("ValidFrom");
CREATE INDEX "Idx_ChangeRequestAudit_ValidTo" ON "EngineeringModel_REPLACE"."ChangeRequest_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ChangeRequest_Audit" SET (autovacuum_analyze_threshold = 2500);

-- CommonFileStore - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" (LIKE "EngineeringModel_REPLACE"."CommonFileStore");
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CommonFileStoreAudit_ValidFrom" ON "EngineeringModel_REPLACE"."CommonFileStore_Audit" ("ValidFrom");
CREATE INDEX "Idx_CommonFileStoreAudit_ValidTo" ON "EngineeringModel_REPLACE"."CommonFileStore_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."CommonFileStore_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ContractChangeNotice - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" (LIKE "EngineeringModel_REPLACE"."ContractChangeNotice");
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ContractChangeNoticeAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" ("ValidFrom");
CREATE INDEX "Idx_ContractChangeNoticeAudit_ValidTo" ON "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractChangeNotice_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ContractDeviation - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" (LIKE "EngineeringModel_REPLACE"."ContractDeviation");
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ContractDeviationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ContractDeviation_Audit" ("ValidFrom");
CREATE INDEX "Idx_ContractDeviationAudit_ValidTo" ON "EngineeringModel_REPLACE"."ContractDeviation_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ContractDeviation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiscussionItem - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" (LIKE "EngineeringModel_REPLACE"."DiscussionItem");
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiscussionItemAudit_ValidFrom" ON "EngineeringModel_REPLACE"."DiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItemAudit_ValidTo" ON "EngineeringModel_REPLACE"."DiscussionItem_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."DiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModel - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" (LIKE "EngineeringModel_REPLACE"."EngineeringModel");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelAudit_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModel_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelAudit_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModel_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModel_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataAnnotation - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" (LIKE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelDataAnnotationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataAnnotationAudit_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataDiscussionItem - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" (LIKE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelDataDiscussionItemAudit_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataDiscussionItemAudit_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModelDataNote - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" (LIKE "EngineeringModel_REPLACE"."EngineeringModelDataNote");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelDataNoteAudit_ValidFrom" ON "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelDataNoteAudit_ValidTo" ON "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."EngineeringModelDataNote_Audit" SET (autovacuum_analyze_threshold = 2500);

-- File - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."File_Audit" (LIKE "EngineeringModel_REPLACE"."File");
ALTER TABLE "EngineeringModel_REPLACE"."File_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileAudit_ValidFrom" ON "EngineeringModel_REPLACE"."File_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileAudit_ValidTo" ON "EngineeringModel_REPLACE"."File_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."File_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" (LIKE "EngineeringModel_REPLACE"."FileRevision");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileRevisionAudit_ValidFrom" ON "EngineeringModel_REPLACE"."FileRevision_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileRevisionAudit_ValidTo" ON "EngineeringModel_REPLACE"."FileRevision_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."FileStore_Audit" (LIKE "EngineeringModel_REPLACE"."FileStore");
ALTER TABLE "EngineeringModel_REPLACE"."FileStore_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileStoreAudit_ValidFrom" ON "EngineeringModel_REPLACE"."FileStore_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileStoreAudit_ValidTo" ON "EngineeringModel_REPLACE"."FileStore_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileStore_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileStore_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Folder - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Folder_Audit" (LIKE "EngineeringModel_REPLACE"."Folder");
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FolderAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Folder_Audit" ("ValidFrom");
CREATE INDEX "Idx_FolderAudit_ValidTo" ON "EngineeringModel_REPLACE"."Folder_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Folder_Audit" SET (autovacuum_analyze_threshold = 2500);

-- GenericAnnotation - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" (LIKE "EngineeringModel_REPLACE"."GenericAnnotation");
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GenericAnnotationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."GenericAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotationAudit_ValidTo" ON "EngineeringModel_REPLACE"."GenericAnnotation_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."GenericAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Iteration - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Iteration_Audit" (LIKE "EngineeringModel_REPLACE"."Iteration");
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IterationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Iteration_Audit" ("ValidFrom");
CREATE INDEX "Idx_IterationAudit_ValidTo" ON "EngineeringModel_REPLACE"."Iteration_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Iteration_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" (LIKE "EngineeringModel_REPLACE"."ModellingAnnotationItem");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModellingAnnotationItemAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItemAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModellingThingReference - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" (LIKE "EngineeringModel_REPLACE"."ModellingThingReference");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModellingThingReferenceAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModellingThingReferenceAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModellingThingReference_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" (LIKE "EngineeringModel_REPLACE"."ModelLogEntry");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelLogEntryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntryAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Note - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Note_Audit" (LIKE "EngineeringModel_REPLACE"."Note");
ALTER TABLE "EngineeringModel_REPLACE"."Note_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NoteAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Note_Audit" ("ValidFrom");
CREATE INDEX "Idx_NoteAudit_ValidTo" ON "EngineeringModel_REPLACE"."Note_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Note_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Page - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Page_Audit" (LIKE "EngineeringModel_REPLACE"."Page");
ALTER TABLE "EngineeringModel_REPLACE"."Page_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PageAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Page_Audit" ("ValidFrom");
CREATE INDEX "Idx_PageAudit_ValidTo" ON "EngineeringModel_REPLACE"."Page_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Page_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequestForDeviation - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" (LIKE "EngineeringModel_REPLACE"."RequestForDeviation");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequestForDeviationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."RequestForDeviation_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequestForDeviationAudit_ValidTo" ON "EngineeringModel_REPLACE"."RequestForDeviation_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForDeviation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequestForWaiver - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" (LIKE "EngineeringModel_REPLACE"."RequestForWaiver");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequestForWaiverAudit_ValidFrom" ON "EngineeringModel_REPLACE"."RequestForWaiver_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequestForWaiverAudit_ValidTo" ON "EngineeringModel_REPLACE"."RequestForWaiver_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RequestForWaiver_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ReviewItemDiscrepancy - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" (LIKE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy");
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReviewItemDiscrepancyAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReviewItemDiscrepancyAudit_ValidTo" ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ReviewItemDiscrepancy_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Section - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Section_Audit" (LIKE "EngineeringModel_REPLACE"."Section");
ALTER TABLE "EngineeringModel_REPLACE"."Section_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SectionAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Section_Audit" ("ValidFrom");
CREATE INDEX "Idx_SectionAudit_ValidTo" ON "EngineeringModel_REPLACE"."Section_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Section_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Solution - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Solution_Audit" (LIKE "EngineeringModel_REPLACE"."Solution");
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SolutionAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Solution_Audit" ("ValidFrom");
CREATE INDEX "Idx_SolutionAudit_ValidTo" ON "EngineeringModel_REPLACE"."Solution_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Solution_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TextualNote - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" (LIKE "EngineeringModel_REPLACE"."TextualNote");
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TextualNoteAudit_ValidFrom" ON "EngineeringModel_REPLACE"."TextualNote_Audit" ("ValidFrom");
CREATE INDEX "Idx_TextualNoteAudit_ValidTo" ON "EngineeringModel_REPLACE"."TextualNote_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TextualNote_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."Thing_Audit" (LIKE "EngineeringModel_REPLACE"."Thing");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Thing_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingAudit_ValidTo" ON "EngineeringModel_REPLACE"."Thing_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" (LIKE "EngineeringModel_REPLACE"."ThingReference");
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingReferenceAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingReferenceAudit_ValidTo" ON "EngineeringModel_REPLACE"."ThingReference_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

-- TopContainer - Audit table
CREATE TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" (LIKE "EngineeringModel_REPLACE"."TopContainer");
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TopContainerAudit_ValidFrom" ON "EngineeringModel_REPLACE"."TopContainer_Audit" ("ValidFrom");
CREATE INDEX "Idx_TopContainerAudit_ValidTo" ON "EngineeringModel_REPLACE"."TopContainer_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."TopContainer_Audit" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------ EngineeringModel Audit Reference Or Ordered Properties ------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Audit table Reference properties

-- Approval - Audit table Reference properties

-- BinaryNote - Audit table Reference properties

-- Book - Audit table Reference properties
-- Book.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" (LIKE "EngineeringModel_REPLACE"."Book_Category");
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Book_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Book_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Book_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."Book_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Book_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ChangeProposal - Audit table Reference properties

-- ChangeRequest - Audit table Reference properties

-- CommonFileStore - Audit table Reference properties

-- ContractChangeNotice - Audit table Reference properties

-- ContractDeviation - Audit table Reference properties

-- DiscussionItem - Audit table Reference properties

-- EngineeringModel - Audit table Reference properties

-- EngineeringModelDataAnnotation - Audit table Reference properties

-- EngineeringModelDataDiscussionItem - Audit table Reference properties

-- EngineeringModelDataNote - Audit table Reference properties

-- File - Audit table Reference properties
-- File.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."File_Category_Audit" (LIKE "EngineeringModel_REPLACE"."File_Category");
ALTER TABLE "EngineeringModel_REPLACE"."File_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_File_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."File_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_File_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."File_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."File_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."File_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Audit table Reference properties
-- FileRevision.FileType is an ordered collection property (many to many) of class FileType: [1..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" (LIKE "EngineeringModel_REPLACE"."FileRevision_FileType");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileRevision_FileTypeAudit_ValidFrom" ON "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_FileTypeAudit_ValidTo" ON "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Audit table Reference properties

-- Folder - Audit table Reference properties

-- GenericAnnotation - Audit table Reference properties

-- Iteration - Audit table Reference properties

-- ModellingAnnotationItem - Audit table Reference properties
-- ModellingAnnotationItem.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" (LIKE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModellingAnnotationItem_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItem_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModellingAnnotationItem
-- ModellingAnnotationItem.SourceAnnotation is a collection property (many to many) of class ModellingAnnotationItem: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" (LIKE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModellingAnnotationItem_SourceAnnotationAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModellingAnnotationItem_SourceAnnotationAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModellingThingReference - Audit table Reference properties

-- ModelLogEntry - Audit table Reference properties
-- ModelLogEntry.AffectedItemIid is a collection property of type Guid: [0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" (LIKE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelLogEntry_AffectedItemIidAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_AffectedItemIidAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ModelLogEntry
-- ModelLogEntry.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" (LIKE "EngineeringModel_REPLACE"."ModelLogEntry_Category");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelLogEntry_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."ModelLogEntry_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Note - Audit table Reference properties
-- Note.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" (LIKE "EngineeringModel_REPLACE"."Note_Category");
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Note_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Note_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Note_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."Note_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Note_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Page - Audit table Reference properties
-- Page.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" (LIKE "EngineeringModel_REPLACE"."Page_Category");
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Page_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Page_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Page_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."Page_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Page_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequestForDeviation - Audit table Reference properties

-- RequestForWaiver - Audit table Reference properties

-- ReviewItemDiscrepancy - Audit table Reference properties

-- Section - Audit table Reference properties
-- Section.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" (LIKE "EngineeringModel_REPLACE"."Section_Category");
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Section_CategoryAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Section_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Section_CategoryAudit_ValidTo" ON "EngineeringModel_REPLACE"."Section_Category_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Section_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Solution - Audit table Reference properties

-- TextualNote - Audit table Reference properties

-- Thing - Audit table Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" (LIKE "EngineeringModel_REPLACE"."Thing_ExcludedDomain");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidTo" ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" (LIKE "EngineeringModel_REPLACE"."Thing_ExcludedPerson");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidFrom" ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidTo" ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" ("ValidTo");
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ThingReference - Audit table Reference properties

-- TopContainer - Audit table Reference properties

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Class tables -------------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ActualFiniteState_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ActualFiniteState_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteState" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteState_ValidTo" ON "Iteration_REPLACE"."ActualFiniteState" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ActualFiniteStateList_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ActualFiniteStateList_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateList_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" SET (autovacuum_analyze_threshold = 2500);

-- Alias class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Alias" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Alias_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Alias_ValidFrom" ON "Iteration_REPLACE"."Alias" ("ValidFrom");
CREATE INDEX "Idx_Alias_ValidTo" ON "Iteration_REPLACE"."Alias" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Alias" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Alias" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias" SET (autovacuum_analyze_threshold = 2500);

-- AndExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."AndExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "AndExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_AndExpression_ValidFrom" ON "Iteration_REPLACE"."AndExpression" ("ValidFrom");
CREATE INDEX "Idx_AndExpression_ValidTo" ON "Iteration_REPLACE"."AndExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."AndExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."BinaryRelationship" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BinaryRelationship_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BinaryRelationship_ValidFrom" ON "Iteration_REPLACE"."BinaryRelationship" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationship_ValidTo" ON "Iteration_REPLACE"."BinaryRelationship" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" SET (autovacuum_analyze_threshold = 2500);

-- BooleanExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."BooleanExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BooleanExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BooleanExpression_ValidFrom" ON "Iteration_REPLACE"."BooleanExpression" ("ValidFrom");
CREATE INDEX "Idx_BooleanExpression_ValidTo" ON "Iteration_REPLACE"."BooleanExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" SET (autovacuum_analyze_threshold = 2500);

-- Bounds class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."Bounds" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Bounds_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Bounds_ValidFrom" ON "Iteration_REPLACE"."Bounds" ("ValidFrom");
CREATE INDEX "Idx_Bounds_ValidTo" ON "Iteration_REPLACE"."Bounds" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Bounds" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Bounds" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds" SET (autovacuum_analyze_threshold = 2500);

-- BuiltInRuleVerification class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."BuiltInRuleVerification" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "BuiltInRuleVerification_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_BuiltInRuleVerification_ValidFrom" ON "Iteration_REPLACE"."BuiltInRuleVerification" ("ValidFrom");
CREATE INDEX "Idx_BuiltInRuleVerification_ValidTo" ON "Iteration_REPLACE"."BuiltInRuleVerification" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification" SET (autovacuum_analyze_threshold = 2500);

-- Citation class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Citation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Citation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Citation_ValidFrom" ON "Iteration_REPLACE"."Citation" ("ValidFrom");
CREATE INDEX "Idx_Citation_ValidTo" ON "Iteration_REPLACE"."Citation" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Citation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Citation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation" SET (autovacuum_analyze_threshold = 2500);

-- Color class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Color" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Color_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Color_ValidFrom" ON "Iteration_REPLACE"."Color" ("ValidFrom");
CREATE INDEX "Idx_Color_ValidTo" ON "Iteration_REPLACE"."Color" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Color" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Color" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."DefinedThing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DefinedThing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DefinedThing_ValidFrom" ON "Iteration_REPLACE"."DefinedThing" ("ValidFrom");
CREATE INDEX "Idx_DefinedThing_ValidTo" ON "Iteration_REPLACE"."DefinedThing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DefinedThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DefinedThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DefinedThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DefinedThing" SET (autovacuum_analyze_threshold = 2500);

-- Definition class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Definition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Definition_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Definition_ValidFrom" ON "Iteration_REPLACE"."Definition" ("ValidFrom");
CREATE INDEX "Idx_Definition_ValidTo" ON "Iteration_REPLACE"."Definition" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramCanvas" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramCanvas_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramCanvas_ValidFrom" ON "Iteration_REPLACE"."DiagramCanvas" ("ValidFrom");
CREATE INDEX "Idx_DiagramCanvas_ValidTo" ON "Iteration_REPLACE"."DiagramCanvas" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" SET (autovacuum_analyze_threshold = 2500);

-- DiagramEdge class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramEdge" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramEdge_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramEdge_ValidFrom" ON "Iteration_REPLACE"."DiagramEdge" ("ValidFrom");
CREATE INDEX "Idx_DiagramEdge_ValidTo" ON "Iteration_REPLACE"."DiagramEdge" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementContainer class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramElementContainer" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramElementContainer_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramElementContainer_ValidFrom" ON "Iteration_REPLACE"."DiagramElementContainer" ("ValidFrom");
CREATE INDEX "Idx_DiagramElementContainer_ValidTo" ON "Iteration_REPLACE"."DiagramElementContainer" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementThing class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramElementThing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramElementThing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramElementThing_ValidFrom" ON "Iteration_REPLACE"."DiagramElementThing" ("ValidFrom");
CREATE INDEX "Idx_DiagramElementThing_ValidTo" ON "Iteration_REPLACE"."DiagramElementThing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" SET (autovacuum_analyze_threshold = 2500);

-- DiagrammingStyle class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagrammingStyle" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagrammingStyle_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagrammingStyle_ValidFrom" ON "Iteration_REPLACE"."DiagrammingStyle" ("ValidFrom");
CREATE INDEX "Idx_DiagrammingStyle_ValidTo" ON "Iteration_REPLACE"."DiagrammingStyle" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" SET (autovacuum_analyze_threshold = 2500);

-- DiagramObject class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramObject" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramObject_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramObject_ValidFrom" ON "Iteration_REPLACE"."DiagramObject" ("ValidFrom");
CREATE INDEX "Idx_DiagramObject_ValidTo" ON "Iteration_REPLACE"."DiagramObject" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramObject" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramObject" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject" SET (autovacuum_analyze_threshold = 2500);

-- DiagramShape class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramShape" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramShape_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramShape_ValidFrom" ON "Iteration_REPLACE"."DiagramShape" ("ValidFrom");
CREATE INDEX "Idx_DiagramShape_ValidTo" ON "Iteration_REPLACE"."DiagramShape" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramShape" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramShape" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramShape" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramShape" SET (autovacuum_analyze_threshold = 2500);

-- DiagramThingBase class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."DiagramThingBase" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DiagramThingBase_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DiagramThingBase_ValidFrom" ON "Iteration_REPLACE"."DiagramThingBase" ("ValidFrom");
CREATE INDEX "Idx_DiagramThingBase_ValidTo" ON "Iteration_REPLACE"."DiagramThingBase" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase" SET (autovacuum_analyze_threshold = 2500);

-- DomainFileStore class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."DomainFileStore" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "DomainFileStore_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_DomainFileStore_ValidFrom" ON "Iteration_REPLACE"."DomainFileStore" ("ValidFrom");
CREATE INDEX "Idx_DomainFileStore_ValidTo" ON "Iteration_REPLACE"."DomainFileStore" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" SET (autovacuum_analyze_threshold = 2500);

-- ElementBase class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ElementBase" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementBase_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ElementBase_ValidFrom" ON "Iteration_REPLACE"."ElementBase" ("ValidFrom");
CREATE INDEX "Idx_ElementBase_ValidTo" ON "Iteration_REPLACE"."ElementBase" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementBase" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementBase" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase" SET (autovacuum_analyze_threshold = 2500);

-- ElementDefinition class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ElementDefinition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementDefinition_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ElementDefinition_ValidFrom" ON "Iteration_REPLACE"."ElementDefinition" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_ValidTo" ON "Iteration_REPLACE"."ElementDefinition" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ElementUsage" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementUsage_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ElementUsage_ValidFrom" ON "Iteration_REPLACE"."ElementUsage" ("ValidFrom");
CREATE INDEX "Idx_ElementUsage_ValidTo" ON "Iteration_REPLACE"."ElementUsage" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementUsage" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ExclusiveOrExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ExclusiveOrExpression_ValidFrom" ON "Iteration_REPLACE"."ExclusiveOrExpression" ("ValidFrom");
CREATE INDEX "Idx_ExclusiveOrExpression_ValidTo" ON "Iteration_REPLACE"."ExclusiveOrExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ExternalIdentifierMap" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ExternalIdentifierMap_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ExternalIdentifierMap_ValidFrom" ON "Iteration_REPLACE"."ExternalIdentifierMap" ("ValidFrom");
CREATE INDEX "Idx_ExternalIdentifierMap_ValidTo" ON "Iteration_REPLACE"."ExternalIdentifierMap" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" SET (autovacuum_analyze_threshold = 2500);

-- File class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."File" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "File_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_File_ValidFrom" ON "Iteration_REPLACE"."File" ("ValidFrom");
CREATE INDEX "Idx_File_ValidTo" ON "Iteration_REPLACE"."File" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."File" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."FileRevision" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileRevision_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_FileRevision_ValidFrom" ON "Iteration_REPLACE"."FileRevision" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_ValidTo" ON "Iteration_REPLACE"."FileRevision" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileRevision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision" SET (autovacuum_analyze_threshold = 2500);

-- FileStore class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."FileStore" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "FileStore_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_FileStore_ValidFrom" ON "Iteration_REPLACE"."FileStore" ("ValidFrom");
CREATE INDEX "Idx_FileStore_ValidTo" ON "Iteration_REPLACE"."FileStore" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileStore" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileStore" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileStore" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileStore" SET (autovacuum_analyze_threshold = 2500);

-- Folder class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Folder" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Folder_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Folder_ValidFrom" ON "Iteration_REPLACE"."Folder" ("ValidFrom");
CREATE INDEX "Idx_Folder_ValidTo" ON "Iteration_REPLACE"."Folder" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Folder" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Folder" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder" SET (autovacuum_analyze_threshold = 2500);

-- Goal class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."Goal" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Goal_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Goal_ValidFrom" ON "Iteration_REPLACE"."Goal" ("ValidFrom");
CREATE INDEX "Idx_Goal_ValidTo" ON "Iteration_REPLACE"."Goal" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Goal" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."HyperLink" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "HyperLink_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_HyperLink_ValidFrom" ON "Iteration_REPLACE"."HyperLink" ("ValidFrom");
CREATE INDEX "Idx_HyperLink_ValidTo" ON "Iteration_REPLACE"."HyperLink" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."HyperLink" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."HyperLink" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink" SET (autovacuum_analyze_threshold = 2500);

-- IdCorrespondence class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."IdCorrespondence" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "IdCorrespondence_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_IdCorrespondence_ValidFrom" ON "Iteration_REPLACE"."IdCorrespondence" ("ValidFrom");
CREATE INDEX "Idx_IdCorrespondence_ValidTo" ON "Iteration_REPLACE"."IdCorrespondence" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationship class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."MultiRelationship" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MultiRelationship_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_MultiRelationship_ValidFrom" ON "Iteration_REPLACE"."MultiRelationship" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationship_ValidTo" ON "Iteration_REPLACE"."MultiRelationship" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."NestedElement" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "NestedElement_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_NestedElement_ValidFrom" ON "Iteration_REPLACE"."NestedElement" ("ValidFrom");
CREATE INDEX "Idx_NestedElement_ValidTo" ON "Iteration_REPLACE"."NestedElement" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedElement" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement" SET (autovacuum_analyze_threshold = 2500);

-- NestedParameter class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."NestedParameter" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "NestedParameter_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_NestedParameter_ValidFrom" ON "Iteration_REPLACE"."NestedParameter" ("ValidFrom");
CREATE INDEX "Idx_NestedParameter_ValidTo" ON "Iteration_REPLACE"."NestedParameter" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedParameter" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedParameter" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter" SET (autovacuum_analyze_threshold = 2500);

-- NotExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."NotExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "NotExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_NotExpression_ValidFrom" ON "Iteration_REPLACE"."NotExpression" ("ValidFrom");
CREATE INDEX "Idx_NotExpression_ValidTo" ON "Iteration_REPLACE"."NotExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NotExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NotExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression" SET (autovacuum_analyze_threshold = 2500);

-- Option class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Option" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Option_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Option_ValidFrom" ON "Iteration_REPLACE"."Option" ("ValidFrom");
CREATE INDEX "Idx_Option_ValidTo" ON "Iteration_REPLACE"."Option" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Option" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option" SET (autovacuum_analyze_threshold = 2500);

-- OrExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."OrExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "OrExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_OrExpression_ValidFrom" ON "Iteration_REPLACE"."OrExpression" ("ValidFrom");
CREATE INDEX "Idx_OrExpression_ValidTo" ON "Iteration_REPLACE"."OrExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OrExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."OwnedStyle" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "OwnedStyle_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_OwnedStyle_ValidFrom" ON "Iteration_REPLACE"."OwnedStyle" ("ValidFrom");
CREATE INDEX "Idx_OwnedStyle_ValidTo" ON "Iteration_REPLACE"."OwnedStyle" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" SET (autovacuum_analyze_threshold = 2500);

-- Parameter class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Parameter" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Parameter_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Parameter_ValidFrom" ON "Iteration_REPLACE"."Parameter" ("ValidFrom");
CREATE INDEX "Idx_Parameter_ValidTo" ON "Iteration_REPLACE"."Parameter" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Parameter" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Parameter" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter" SET (autovacuum_analyze_threshold = 2500);

-- ParameterBase class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterBase" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterBase_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterBase_ValidFrom" ON "Iteration_REPLACE"."ParameterBase" ("ValidFrom");
CREATE INDEX "Idx_ParameterBase_ValidTo" ON "Iteration_REPLACE"."ParameterBase" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterBase" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterBase" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterBase" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterBase" SET (autovacuum_analyze_threshold = 2500);

-- ParameterGroup class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterGroup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterGroup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterGroup_ValidFrom" ON "Iteration_REPLACE"."ParameterGroup" ("ValidFrom");
CREATE INDEX "Idx_ParameterGroup_ValidTo" ON "Iteration_REPLACE"."ParameterGroup" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOrOverrideBase class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterOrOverrideBase_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterOrOverrideBase_ValidFrom" ON "Iteration_REPLACE"."ParameterOrOverrideBase" ("ValidFrom");
CREATE INDEX "Idx_ParameterOrOverrideBase_ValidTo" ON "Iteration_REPLACE"."ParameterOrOverrideBase" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverride class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterOverride" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterOverride_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterOverride_ValidFrom" ON "Iteration_REPLACE"."ParameterOverride" ("ValidFrom");
CREATE INDEX "Idx_ParameterOverride_ValidTo" ON "Iteration_REPLACE"."ParameterOverride" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverrideValueSet class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterOverrideValueSet_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterOverrideValueSet_ValidFrom" ON "Iteration_REPLACE"."ParameterOverrideValueSet" ("ValidFrom");
CREATE INDEX "Idx_ParameterOverrideValueSet_ValidTo" ON "Iteration_REPLACE"."ParameterOverrideValueSet" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscription class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterSubscription" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterSubscription_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterSubscription_ValidFrom" ON "Iteration_REPLACE"."ParameterSubscription" ("ValidFrom");
CREATE INDEX "Idx_ParameterSubscription_ValidTo" ON "Iteration_REPLACE"."ParameterSubscription" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscriptionValueSet class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterSubscriptionValueSet_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterSubscriptionValueSet_ValidFrom" ON "Iteration_REPLACE"."ParameterSubscriptionValueSet" ("ValidFrom");
CREATE INDEX "Idx_ParameterSubscriptionValueSet_ValidTo" ON "Iteration_REPLACE"."ParameterSubscriptionValueSet" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValue class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."ParameterValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterValue_ValidFrom" ON "Iteration_REPLACE"."ParameterValue" ("ValidFrom");
CREATE INDEX "Idx_ParameterValue_ValidTo" ON "Iteration_REPLACE"."ParameterValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValue" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSet class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterValueSet" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterValueSet_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterValueSet_ValidFrom" ON "Iteration_REPLACE"."ParameterValueSet" ("ValidFrom");
CREATE INDEX "Idx_ParameterValueSet_ValidTo" ON "Iteration_REPLACE"."ParameterValueSet" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSetBase class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParameterValueSetBase" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParameterValueSetBase_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParameterValueSetBase_ValidFrom" ON "Iteration_REPLACE"."ParameterValueSetBase" ("ValidFrom");
CREATE INDEX "Idx_ParameterValueSetBase_ValidTo" ON "Iteration_REPLACE"."ParameterValueSetBase" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" SET (autovacuum_analyze_threshold = 2500);

-- ParametricConstraint class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."ParametricConstraint" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ParametricConstraint_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ParametricConstraint_ValidFrom" ON "Iteration_REPLACE"."ParametricConstraint" ("ValidFrom");
CREATE INDEX "Idx_ParametricConstraint_ValidTo" ON "Iteration_REPLACE"."ParametricConstraint" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" SET (autovacuum_analyze_threshold = 2500);

-- Point class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."Point" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Point_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Point_ValidFrom" ON "Iteration_REPLACE"."Point" ("ValidFrom");
CREATE INDEX "Idx_Point_ValidTo" ON "Iteration_REPLACE"."Point" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Point" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Point" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteState class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteState" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PossibleFiniteState_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_PossibleFiniteState_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteState" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteState_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteState" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteStateList class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PossibleFiniteStateList_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_PossibleFiniteStateList_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteStateList" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteStateList_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteStateList" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" SET (autovacuum_analyze_threshold = 2500);

-- Publication class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Publication" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Publication_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Publication_ValidFrom" ON "Iteration_REPLACE"."Publication" ("ValidFrom");
CREATE INDEX "Idx_Publication_ValidTo" ON "Iteration_REPLACE"."Publication" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RelationalExpression" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RelationalExpression_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RelationalExpression_ValidFrom" ON "Iteration_REPLACE"."RelationalExpression" ("ValidFrom");
CREATE INDEX "Idx_RelationalExpression_ValidTo" ON "Iteration_REPLACE"."RelationalExpression" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" SET (autovacuum_analyze_threshold = 2500);

-- Relationship class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Relationship" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Relationship_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Relationship_ValidFrom" ON "Iteration_REPLACE"."Relationship" ("ValidFrom");
CREATE INDEX "Idx_Relationship_ValidTo" ON "Iteration_REPLACE"."Relationship" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Relationship" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Relationship" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship" SET (autovacuum_analyze_threshold = 2500);

-- RelationshipParameterValue class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."RelationshipParameterValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RelationshipParameterValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RelationshipParameterValue_ValidFrom" ON "Iteration_REPLACE"."RelationshipParameterValue" ("ValidFrom");
CREATE INDEX "Idx_RelationshipParameterValue_ValidTo" ON "Iteration_REPLACE"."RelationshipParameterValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" SET (autovacuum_analyze_threshold = 2500);

-- Requirement class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Requirement" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Requirement_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Requirement_ValidFrom" ON "Iteration_REPLACE"."Requirement" ("ValidFrom");
CREATE INDEX "Idx_Requirement_ValidTo" ON "Iteration_REPLACE"."Requirement" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Requirement" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RequirementsContainer" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequirementsContainer_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequirementsContainer_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainer" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainer_ValidTo" ON "Iteration_REPLACE"."RequirementsContainer" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainerParameterValue class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequirementsContainerParameterValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequirementsContainerParameterValue_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainerParameterValue" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainerParameterValue_ValidTo" ON "Iteration_REPLACE"."RequirementsContainerParameterValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsGroup class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RequirementsGroup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequirementsGroup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequirementsGroup_ValidFrom" ON "Iteration_REPLACE"."RequirementsGroup" ("ValidFrom");
CREATE INDEX "Idx_RequirementsGroup_ValidTo" ON "Iteration_REPLACE"."RequirementsGroup" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsSpecification class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RequirementsSpecification" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequirementsSpecification_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RequirementsSpecification_ValidFrom" ON "Iteration_REPLACE"."RequirementsSpecification" ("ValidFrom");
CREATE INDEX "Idx_RequirementsSpecification_ValidTo" ON "Iteration_REPLACE"."RequirementsSpecification" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerification class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RuleVerification" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RuleVerification_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RuleVerification_ValidFrom" ON "Iteration_REPLACE"."RuleVerification" ("ValidFrom");
CREATE INDEX "Idx_RuleVerification_ValidTo" ON "Iteration_REPLACE"."RuleVerification" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleVerification" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerification" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerification" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerification" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerificationList class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RuleVerificationList" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RuleVerificationList_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RuleVerificationList_ValidFrom" ON "Iteration_REPLACE"."RuleVerificationList" ("ValidFrom");
CREATE INDEX "Idx_RuleVerificationList_ValidTo" ON "Iteration_REPLACE"."RuleVerificationList" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" SET (autovacuum_analyze_threshold = 2500);

-- RuleViolation class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."RuleViolation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RuleViolation_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_RuleViolation_ValidFrom" ON "Iteration_REPLACE"."RuleViolation" ("ValidFrom");
CREATE INDEX "Idx_RuleViolation_ValidTo" ON "Iteration_REPLACE"."RuleViolation" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleViolation" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."SharedStyle" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SharedStyle_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SharedStyle_ValidFrom" ON "Iteration_REPLACE"."SharedStyle" ("ValidFrom");
CREATE INDEX "Idx_SharedStyle_ValidTo" ON "Iteration_REPLACE"."SharedStyle" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SharedStyle" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SharedStyle" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterizableThing class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."SimpleParameterizableThing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SimpleParameterizableThing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SimpleParameterizableThing_ValidFrom" ON "Iteration_REPLACE"."SimpleParameterizableThing" ("ValidFrom");
CREATE INDEX "Idx_SimpleParameterizableThing_ValidTo" ON "Iteration_REPLACE"."SimpleParameterizableThing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterValue class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."SimpleParameterValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "SimpleParameterValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_SimpleParameterValue_ValidFrom" ON "Iteration_REPLACE"."SimpleParameterValue" ("ValidFrom");
CREATE INDEX "Idx_SimpleParameterValue_ValidTo" ON "Iteration_REPLACE"."SimpleParameterValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."Stakeholder" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Stakeholder_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Stakeholder_ValidFrom" ON "Iteration_REPLACE"."Stakeholder" ("ValidFrom");
CREATE INDEX "Idx_Stakeholder_ValidTo" ON "Iteration_REPLACE"."Stakeholder" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."StakeholderValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeholderValue_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_StakeholderValue_ValidFrom" ON "Iteration_REPLACE"."StakeholderValue" ("ValidFrom");
CREATE INDEX "Idx_StakeholderValue_ValidTo" ON "Iteration_REPLACE"."StakeholderValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_StakeHolderValueMap_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMapSettings_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_StakeHolderValueMapSettings_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMapSettings" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMapSettings_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMapSettings" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" SET (autovacuum_analyze_threshold = 2500);

-- Thing class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."Thing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_Thing_ValidFrom" ON "Iteration_REPLACE"."Thing" ("ValidFrom");
CREATE INDEX "Idx_Thing_ValidTo" ON "Iteration_REPLACE"."Thing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing" SET (autovacuum_analyze_threshold = 2500);

-- UserRuleVerification class - table definition [version 1.0.0]
CREATE TABLE "Iteration_REPLACE"."UserRuleVerification" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "UserRuleVerification_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_UserRuleVerification_ValidFrom" ON "Iteration_REPLACE"."UserRuleVerification" ("ValidFrom");
CREATE INDEX "Idx_UserRuleVerification_ValidTo" ON "Iteration_REPLACE"."UserRuleVerification" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" SET (autovacuum_analyze_threshold = 2500);

-- ValueGroup class - table definition [version 1.1.0]
CREATE TABLE "Iteration_REPLACE"."ValueGroup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ValueGroup_PK" PRIMARY KEY ("Iid")
);
CREATE INDEX "Idx_ValueGroup_ValidFrom" ON "Iteration_REPLACE"."ValueGroup" ("ValidFrom");
CREATE INDEX "Idx_ValueGroup_ValidTo" ON "Iteration_REPLACE"."ValueGroup" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ValueGroup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Derives Relationships ----------------------------------------
--------------------------------------------------------------------------------------------------------

-- Class ActualFiniteState derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" ADD CONSTRAINT "ActualFiniteStateDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ActualFiniteStateList derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" ADD CONSTRAINT "ActualFiniteStateListDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Alias derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Alias" ADD CONSTRAINT "AliasDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class AndExpression derives from class BooleanExpression
ALTER TABLE "Iteration_REPLACE"."AndExpression" ADD CONSTRAINT "AndExpressionDerivesFromBooleanExpression" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BinaryRelationship derives from class Relationship
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" ADD CONSTRAINT "BinaryRelationshipDerivesFromRelationship" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Relationship" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BooleanExpression derives from class Thing
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" ADD CONSTRAINT "BooleanExpressionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Bounds derives from class DiagramThingBase
ALTER TABLE "Iteration_REPLACE"."Bounds" ADD CONSTRAINT "BoundsDerivesFromDiagramThingBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramThingBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class BuiltInRuleVerification derives from class RuleVerification
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification" ADD CONSTRAINT "BuiltInRuleVerificationDerivesFromRuleVerification" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."RuleVerification" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Citation derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Citation" ADD CONSTRAINT "CitationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Color derives from class DiagramThingBase
ALTER TABLE "Iteration_REPLACE"."Color" ADD CONSTRAINT "ColorDerivesFromDiagramThingBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramThingBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DefinedThing derives from class Thing
ALTER TABLE "Iteration_REPLACE"."DefinedThing" ADD CONSTRAINT "DefinedThingDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Definition derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Definition" ADD CONSTRAINT "DefinitionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramCanvas derives from class DiagramElementContainer
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" ADD CONSTRAINT "DiagramCanvasDerivesFromDiagramElementContainer" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramElementContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramEdge derives from class DiagramElementThing
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" ADD CONSTRAINT "DiagramEdgeDerivesFromDiagramElementThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramElementThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramElementContainer derives from class DiagramThingBase
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer" ADD CONSTRAINT "DiagramElementContainerDerivesFromDiagramThingBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramThingBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramElementThing derives from class DiagramElementContainer
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD CONSTRAINT "DiagramElementThingDerivesFromDiagramElementContainer" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramElementContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagrammingStyle derives from class DiagramThingBase
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD CONSTRAINT "DiagrammingStyleDerivesFromDiagramThingBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramThingBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramObject derives from class DiagramShape
ALTER TABLE "Iteration_REPLACE"."DiagramObject" ADD CONSTRAINT "DiagramObjectDerivesFromDiagramShape" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramShape" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramShape derives from class DiagramElementThing
ALTER TABLE "Iteration_REPLACE"."DiagramShape" ADD CONSTRAINT "DiagramShapeDerivesFromDiagramElementThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramElementThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramThingBase derives from class Thing
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase" ADD CONSTRAINT "DiagramThingBaseDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DomainFileStore derives from class FileStore
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" ADD CONSTRAINT "DomainFileStoreDerivesFromFileStore" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ElementBase derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."ElementBase" ADD CONSTRAINT "ElementBaseDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ElementDefinition derives from class ElementBase
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" ADD CONSTRAINT "ElementDefinitionDerivesFromElementBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ElementBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ElementUsage derives from class ElementBase
ALTER TABLE "Iteration_REPLACE"."ElementUsage" ADD CONSTRAINT "ElementUsageDerivesFromElementBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ElementBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ExclusiveOrExpression derives from class BooleanExpression
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression" ADD CONSTRAINT "ExclusiveOrExpressionDerivesFromBooleanExpression" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ExternalIdentifierMap derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD CONSTRAINT "ExternalIdentifierMapDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class File derives from class Thing
ALTER TABLE "Iteration_REPLACE"."File" ADD CONSTRAINT "FileDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class FileRevision derives from class Thing
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevisionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class FileStore derives from class Thing
ALTER TABLE "Iteration_REPLACE"."FileStore" ADD CONSTRAINT "FileStoreDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Folder derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Folder" ADD CONSTRAINT "FolderDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Goal derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."Goal" ADD CONSTRAINT "GoalDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class HyperLink derives from class Thing
ALTER TABLE "Iteration_REPLACE"."HyperLink" ADD CONSTRAINT "HyperLinkDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class IdCorrespondence derives from class Thing
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" ADD CONSTRAINT "IdCorrespondenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class MultiRelationship derives from class Relationship
ALTER TABLE "Iteration_REPLACE"."MultiRelationship" ADD CONSTRAINT "MultiRelationshipDerivesFromRelationship" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Relationship" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class NestedElement derives from class Thing
ALTER TABLE "Iteration_REPLACE"."NestedElement" ADD CONSTRAINT "NestedElementDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class NestedParameter derives from class Thing
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD CONSTRAINT "NestedParameterDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class NotExpression derives from class BooleanExpression
ALTER TABLE "Iteration_REPLACE"."NotExpression" ADD CONSTRAINT "NotExpressionDerivesFromBooleanExpression" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Option derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."Option" ADD CONSTRAINT "OptionDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class OrExpression derives from class BooleanExpression
ALTER TABLE "Iteration_REPLACE"."OrExpression" ADD CONSTRAINT "OrExpressionDerivesFromBooleanExpression" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class OwnedStyle derives from class DiagrammingStyle
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" ADD CONSTRAINT "OwnedStyleDerivesFromDiagrammingStyle" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagrammingStyle" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Parameter derives from class ParameterOrOverrideBase
ALTER TABLE "Iteration_REPLACE"."Parameter" ADD CONSTRAINT "ParameterDerivesFromParameterOrOverrideBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterOrOverrideBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterBase derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBaseDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterGroup derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" ADD CONSTRAINT "ParameterGroupDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterOrOverrideBase derives from class ParameterBase
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase" ADD CONSTRAINT "ParameterOrOverrideBaseDerivesFromParameterBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterOverride derives from class ParameterOrOverrideBase
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" ADD CONSTRAINT "ParameterOverrideDerivesFromParameterOrOverrideBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterOrOverrideBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterOverrideValueSet derives from class ParameterValueSetBase
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" ADD CONSTRAINT "ParameterOverrideValueSetDerivesFromParameterValueSetBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterValueSetBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterSubscription derives from class ParameterBase
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" ADD CONSTRAINT "ParameterSubscriptionDerivesFromParameterBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterSubscriptionValueSet derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" ADD CONSTRAINT "ParameterSubscriptionValueSetDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterValue derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParameterValue" ADD CONSTRAINT "ParameterValueDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterValueSet derives from class ParameterValueSetBase
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" ADD CONSTRAINT "ParameterValueSetDerivesFromParameterValueSetBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterValueSetBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParameterValueSetBase derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" ADD CONSTRAINT "ParameterValueSetBaseDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ParametricConstraint derives from class Thing
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD CONSTRAINT "ParametricConstraintDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Point derives from class DiagramThingBase
ALTER TABLE "Iteration_REPLACE"."Point" ADD CONSTRAINT "PointDerivesFromDiagramThingBase" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagramThingBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class PossibleFiniteState derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" ADD CONSTRAINT "PossibleFiniteStateDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class PossibleFiniteStateList derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD CONSTRAINT "PossibleFiniteStateListDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Publication derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Publication" ADD CONSTRAINT "PublicationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RelationalExpression derives from class BooleanExpression
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" ADD CONSTRAINT "RelationalExpressionDerivesFromBooleanExpression" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Relationship derives from class Thing
ALTER TABLE "Iteration_REPLACE"."Relationship" ADD CONSTRAINT "RelationshipDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RelationshipParameterValue derives from class ParameterValue
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" ADD CONSTRAINT "RelationshipParameterValueDerivesFromParameterValue" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterValue" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Requirement derives from class SimpleParameterizableThing
ALTER TABLE "Iteration_REPLACE"."Requirement" ADD CONSTRAINT "RequirementDerivesFromSimpleParameterizableThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."SimpleParameterizableThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequirementsContainer derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" ADD CONSTRAINT "RequirementsContainerDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequirementsContainerParameterValue derives from class ParameterValue
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" ADD CONSTRAINT "RequirementsContainerParameterValueDerivesFromParameterValue" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."ParameterValue" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequirementsGroup derives from class RequirementsContainer
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" ADD CONSTRAINT "RequirementsGroupDerivesFromRequirementsContainer" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."RequirementsContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RequirementsSpecification derives from class RequirementsContainer
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" ADD CONSTRAINT "RequirementsSpecificationDerivesFromRequirementsContainer" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."RequirementsContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RuleVerification derives from class Thing
ALTER TABLE "Iteration_REPLACE"."RuleVerification" ADD CONSTRAINT "RuleVerificationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RuleVerificationList derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" ADD CONSTRAINT "RuleVerificationListDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class RuleViolation derives from class Thing
ALTER TABLE "Iteration_REPLACE"."RuleViolation" ADD CONSTRAINT "RuleViolationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SharedStyle derives from class DiagrammingStyle
ALTER TABLE "Iteration_REPLACE"."SharedStyle" ADD CONSTRAINT "SharedStyleDerivesFromDiagrammingStyle" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DiagrammingStyle" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SimpleParameterizableThing derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" ADD CONSTRAINT "SimpleParameterizableThingDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class SimpleParameterValue derives from class Thing
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD CONSTRAINT "SimpleParameterValueDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class Stakeholder derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."Stakeholder" ADD CONSTRAINT "StakeholderDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class StakeholderValue derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" ADD CONSTRAINT "StakeholderValueDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class StakeHolderValueMap derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" ADD CONSTRAINT "StakeHolderValueMapDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class StakeHolderValueMapSettings derives from class Thing
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD CONSTRAINT "StakeHolderValueMapSettingsDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Thing
-- The Thing class has no superclass

-- Class UserRuleVerification derives from class RuleVerification
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" ADD CONSTRAINT "UserRuleVerificationDerivesFromRuleVerification" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."RuleVerification" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ValueGroup derives from class DefinedThing
ALTER TABLE "Iteration_REPLACE"."ValueGroup" ADD CONSTRAINT "ValueGroupDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Containment Relationships ------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState containment
-- The ActualFiniteState class is contained (composite) by the ActualFiniteStateList class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState" ADD CONSTRAINT "ActualFiniteState_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ActualFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ActualFiniteState_Container" ON "Iteration_REPLACE"."ActualFiniteState" ("Container");

-- ActualFiniteStateList containment
-- The ActualFiniteStateList class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" ADD CONSTRAINT "ActualFiniteStateList_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ActualFiniteStateList_Container" ON "Iteration_REPLACE"."ActualFiniteStateList" ("Container");

-- Alias containment
-- The Alias class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Alias" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Alias" ADD CONSTRAINT "Alias_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Alias_Container" ON "Iteration_REPLACE"."Alias" ("Container");

-- AndExpression containment
-- The AndExpression class is not directly contained

-- BinaryRelationship containment
-- The BinaryRelationship class is not directly contained

-- BooleanExpression containment
-- The BooleanExpression class is contained (composite) by the ParametricConstraint class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."BooleanExpression" ADD CONSTRAINT "BooleanExpression_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ParametricConstraint" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_BooleanExpression_Container" ON "Iteration_REPLACE"."BooleanExpression" ("Container");

-- Bounds containment
-- The Bounds class is contained (composite) by the DiagramElementContainer class: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Bounds" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Bounds" ADD CONSTRAINT "Bounds_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DiagramElementContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Bounds_Container" ON "Iteration_REPLACE"."Bounds" ("Container");

-- BuiltInRuleVerification containment
-- The BuiltInRuleVerification class is not directly contained

-- Citation containment
-- The Citation class is contained (composite) by the Definition class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Citation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Citation" ADD CONSTRAINT "Citation_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Citation_Container" ON "Iteration_REPLACE"."Citation" ("Container");

-- Color containment
-- The Color class is contained (composite) by the DiagrammingStyle class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Color" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Color" ADD CONSTRAINT "Color_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DiagrammingStyle" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Color_Container" ON "Iteration_REPLACE"."Color" ("Container");

-- DefinedThing containment
-- The DefinedThing class is not directly contained

-- Definition containment
-- The Definition class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Definition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Definition" ADD CONSTRAINT "Definition_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Definition_Container" ON "Iteration_REPLACE"."Definition" ("Container");

-- DiagramCanvas containment
-- The DiagramCanvas class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas" ADD CONSTRAINT "DiagramCanvas_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_DiagramCanvas_Container" ON "Iteration_REPLACE"."DiagramCanvas" ("Container");

-- DiagramEdge containment
-- The DiagramEdge class is not directly contained

-- DiagramElementContainer containment
-- The DiagramElementContainer class is not directly contained

-- DiagramElementThing containment
-- The DiagramElementThing class is contained (composite) by the DiagramElementContainer class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD CONSTRAINT "DiagramElementThing_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DiagramElementContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_DiagramElementThing_Container" ON "Iteration_REPLACE"."DiagramElementThing" ("Container");

-- DiagrammingStyle containment
-- The DiagrammingStyle class is not directly contained

-- DiagramObject containment
-- The DiagramObject class is not directly contained

-- DiagramShape containment
-- The DiagramShape class is not directly contained

-- DiagramThingBase containment
-- The DiagramThingBase class is not directly contained

-- DomainFileStore containment
-- The DomainFileStore class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."DomainFileStore" ADD CONSTRAINT "DomainFileStore_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_DomainFileStore_Container" ON "Iteration_REPLACE"."DomainFileStore" ("Container");

-- ElementBase containment
-- The ElementBase class is not directly contained

-- ElementDefinition containment
-- The ElementDefinition class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ElementDefinition" ADD CONSTRAINT "ElementDefinition_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ElementDefinition_Container" ON "Iteration_REPLACE"."ElementDefinition" ("Container");

-- ElementUsage containment
-- The ElementUsage class is contained (composite) by the ElementDefinition class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ElementUsage" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ElementUsage" ADD CONSTRAINT "ElementUsage_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ElementUsage_Container" ON "Iteration_REPLACE"."ElementUsage" ("Container");

-- ExclusiveOrExpression containment
-- The ExclusiveOrExpression class is not directly contained

-- ExternalIdentifierMap containment
-- The ExternalIdentifierMap class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD CONSTRAINT "ExternalIdentifierMap_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ExternalIdentifierMap_Container" ON "Iteration_REPLACE"."ExternalIdentifierMap" ("Container");

-- File containment
-- The File class is contained (composite) by the FileStore class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."File" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."File" ADD CONSTRAINT "File_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_File_Container" ON "Iteration_REPLACE"."File" ("Container");

-- FileRevision containment
-- The FileRevision class is contained (composite) by the File class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."File" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_FileRevision_Container" ON "Iteration_REPLACE"."FileRevision" ("Container");

-- FileStore containment
-- The FileStore class is not directly contained

-- Folder containment
-- The Folder class is contained (composite) by the FileStore class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Folder" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."FileStore" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Folder_Container" ON "Iteration_REPLACE"."Folder" ("Container");

-- Goal containment
-- The Goal class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Goal" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Goal" ADD CONSTRAINT "Goal_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Goal_Container" ON "Iteration_REPLACE"."Goal" ("Container");

-- HyperLink containment
-- The HyperLink class is contained (composite) by the DefinedThing class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."HyperLink" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."HyperLink" ADD CONSTRAINT "HyperLink_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_HyperLink_Container" ON "Iteration_REPLACE"."HyperLink" ("Container");

-- IdCorrespondence containment
-- The IdCorrespondence class is contained (composite) by the ExternalIdentifierMap class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence" ADD CONSTRAINT "IdCorrespondence_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ExternalIdentifierMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_IdCorrespondence_Container" ON "Iteration_REPLACE"."IdCorrespondence" ("Container");

-- MultiRelationship containment
-- The MultiRelationship class is not directly contained

-- NestedElement containment
-- The NestedElement class is contained (composite) by the Option class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."NestedElement" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NestedElement" ADD CONSTRAINT "NestedElement_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."Option" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_NestedElement_Container" ON "Iteration_REPLACE"."NestedElement" ("Container");

-- NestedParameter containment
-- The NestedParameter class is contained (composite) by the NestedElement class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD CONSTRAINT "NestedParameter_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."NestedElement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_NestedParameter_Container" ON "Iteration_REPLACE"."NestedParameter" ("Container");

-- NotExpression containment
-- The NotExpression class is not directly contained

-- Option containment
-- The Option class is contained (composite) by the Iteration class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Option" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Option" ADD CONSTRAINT "Option_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Option_Container" ON "Iteration_REPLACE"."Option" ("Container");

-- OrExpression containment
-- The OrExpression class is not directly contained

-- OwnedStyle containment
-- The OwnedStyle class is contained (composite) by the DiagramElementThing class: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."OwnedStyle" ADD CONSTRAINT "OwnedStyle_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DiagramElementThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_OwnedStyle_Container" ON "Iteration_REPLACE"."OwnedStyle" ("Container");

-- Parameter containment
-- The Parameter class is contained (composite) by the ElementDefinition class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Parameter" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Parameter" ADD CONSTRAINT "Parameter_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Parameter_Container" ON "Iteration_REPLACE"."Parameter" ("Container");

-- ParameterBase containment
-- The ParameterBase class is not directly contained

-- ParameterGroup containment
-- The ParameterGroup class is contained (composite) by the ElementDefinition class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" ADD CONSTRAINT "ParameterGroup_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterGroup_Container" ON "Iteration_REPLACE"."ParameterGroup" ("Container");

-- ParameterOrOverrideBase containment
-- The ParameterOrOverrideBase class is not directly contained

-- ParameterOverride containment
-- The ParameterOverride class is contained (composite) by the ElementUsage class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" ADD CONSTRAINT "ParameterOverride_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ElementUsage" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterOverride_Container" ON "Iteration_REPLACE"."ParameterOverride" ("Container");

-- ParameterOverrideValueSet containment
-- The ParameterOverrideValueSet class is contained (composite) by the ParameterOverride class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" ADD CONSTRAINT "ParameterOverrideValueSet_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ParameterOverride" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterOverrideValueSet_Container" ON "Iteration_REPLACE"."ParameterOverrideValueSet" ("Container");

-- ParameterSubscription containment
-- The ParameterSubscription class is contained (composite) by the ParameterOrOverrideBase class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription" ADD CONSTRAINT "ParameterSubscription_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ParameterOrOverrideBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterSubscription_Container" ON "Iteration_REPLACE"."ParameterSubscription" ("Container");

-- ParameterSubscriptionValueSet containment
-- The ParameterSubscriptionValueSet class is contained (composite) by the ParameterSubscription class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" ADD CONSTRAINT "ParameterSubscriptionValueSet_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."ParameterSubscription" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterSubscriptionValueSet_Container" ON "Iteration_REPLACE"."ParameterSubscriptionValueSet" ("Container");

-- ParameterValue containment
-- The ParameterValue class is not directly contained

-- ParameterValueSet containment
-- The ParameterValueSet class is contained (composite) by the Parameter class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet" ADD CONSTRAINT "ParameterValueSet_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."Parameter" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParameterValueSet_Container" ON "Iteration_REPLACE"."ParameterValueSet" ("Container");

-- ParameterValueSetBase containment
-- The ParameterValueSetBase class is not directly contained

-- ParametricConstraint containment
-- The ParametricConstraint class is contained (composite) by the Requirement class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD CONSTRAINT "ParametricConstraint_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."Requirement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ParametricConstraint_Container" ON "Iteration_REPLACE"."ParametricConstraint" ("Container");

-- Point containment
-- The Point class is contained (composite) by the DiagramEdge class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Point" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Point" ADD CONSTRAINT "Point_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."DiagramEdge" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Point_Container" ON "Iteration_REPLACE"."Point" ("Container");

-- PossibleFiniteState containment
-- The PossibleFiniteState class is contained (composite) by the PossibleFiniteStateList class: [1..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" ADD CONSTRAINT "PossibleFiniteState_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."PossibleFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_PossibleFiniteState_Container" ON "Iteration_REPLACE"."PossibleFiniteState" ("Container");

-- PossibleFiniteStateList containment
-- The PossibleFiniteStateList class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD CONSTRAINT "PossibleFiniteStateList_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_PossibleFiniteStateList_Container" ON "Iteration_REPLACE"."PossibleFiniteStateList" ("Container");

-- Publication containment
-- The Publication class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Publication" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Publication" ADD CONSTRAINT "Publication_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Publication_Container" ON "Iteration_REPLACE"."Publication" ("Container");

-- RelationalExpression containment
-- The RelationalExpression class is not directly contained

-- Relationship containment
-- The Relationship class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Relationship" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Relationship" ADD CONSTRAINT "Relationship_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Relationship_Container" ON "Iteration_REPLACE"."Relationship" ("Container");

-- RelationshipParameterValue containment
-- The RelationshipParameterValue class is contained (composite) by the Relationship class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue" ADD CONSTRAINT "RelationshipParameterValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."Relationship" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RelationshipParameterValue_Container" ON "Iteration_REPLACE"."RelationshipParameterValue" ("Container");

-- Requirement containment
-- The Requirement class is contained (composite) by the RequirementsSpecification class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Requirement" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Requirement" ADD CONSTRAINT "Requirement_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."RequirementsSpecification" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Requirement_Container" ON "Iteration_REPLACE"."Requirement" ("Container");

-- RequirementsContainer containment
-- The RequirementsContainer class is not directly contained

-- RequirementsContainerParameterValue containment
-- The RequirementsContainerParameterValue class is contained (composite) by the RequirementsContainer class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue" ADD CONSTRAINT "RequirementsContainerParameterValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."RequirementsContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RequirementsContainerParameterValue_Container" ON "Iteration_REPLACE"."RequirementsContainerParameterValue" ("Container");

-- RequirementsGroup containment
-- The RequirementsGroup class is contained (composite) by the RequirementsContainer class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup" ADD CONSTRAINT "RequirementsGroup_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."RequirementsContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RequirementsGroup_Container" ON "Iteration_REPLACE"."RequirementsGroup" ("Container");

-- RequirementsSpecification containment
-- The RequirementsSpecification class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification" ADD CONSTRAINT "RequirementsSpecification_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RequirementsSpecification_Container" ON "Iteration_REPLACE"."RequirementsSpecification" ("Container");

-- RuleVerification containment
-- The RuleVerification class is contained (composite) by the RuleVerificationList class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RuleVerification" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RuleVerification" ADD CONSTRAINT "RuleVerification_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."RuleVerificationList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RuleVerification_Container" ON "Iteration_REPLACE"."RuleVerification" ("Container");

-- RuleVerificationList containment
-- The RuleVerificationList class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" ADD CONSTRAINT "RuleVerificationList_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RuleVerificationList_Container" ON "Iteration_REPLACE"."RuleVerificationList" ("Container");

-- RuleViolation containment
-- The RuleViolation class is contained (composite) by the RuleVerification class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."RuleViolation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RuleViolation" ADD CONSTRAINT "RuleViolation_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."RuleVerification" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_RuleViolation_Container" ON "Iteration_REPLACE"."RuleViolation" ("Container");

-- SharedStyle containment
-- The SharedStyle class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."SharedStyle" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."SharedStyle" ADD CONSTRAINT "SharedStyle_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SharedStyle_Container" ON "Iteration_REPLACE"."SharedStyle" ("Container");

-- SimpleParameterizableThing containment
-- The SimpleParameterizableThing class is not directly contained

-- SimpleParameterValue containment
-- The SimpleParameterValue class is contained (composite) by the SimpleParameterizableThing class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD CONSTRAINT "SimpleParameterValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."SimpleParameterizableThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_SimpleParameterValue_Container" ON "Iteration_REPLACE"."SimpleParameterValue" ("Container");

-- Stakeholder containment
-- The Stakeholder class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."Stakeholder" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Stakeholder" ADD CONSTRAINT "Stakeholder_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_Stakeholder_Container" ON "Iteration_REPLACE"."Stakeholder" ("Container");

-- StakeholderValue containment
-- The StakeholderValue class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."StakeholderValue" ADD CONSTRAINT "StakeholderValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_StakeholderValue_Container" ON "Iteration_REPLACE"."StakeholderValue" ("Container");

-- StakeHolderValueMap containment
-- The StakeHolderValueMap class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap" ADD CONSTRAINT "StakeHolderValueMap_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_StakeHolderValueMap_Container" ON "Iteration_REPLACE"."StakeHolderValueMap" ("Container");

-- StakeHolderValueMapSettings containment
-- The StakeHolderValueMapSettings class is contained (composite) by the StakeHolderValueMap class: [1..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD CONSTRAINT "StakeHolderValueMapSettings_FK_Container" FOREIGN KEY ("Container") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_StakeHolderValueMapSettings_Container" ON "Iteration_REPLACE"."StakeHolderValueMapSettings" ("Container");

-- Thing containment
-- The Thing class is not directly contained

-- UserRuleVerification containment
-- The UserRuleVerification class is not directly contained

-- ValueGroup containment
-- The ValueGroup class is contained (composite) by the Iteration class: [0..*]-[1..1]
ALTER TABLE "Iteration_REPLACE"."ValueGroup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ValueGroup" ADD CONSTRAINT "ValueGroup_FK_Container" FOREIGN KEY ("Container") REFERENCES "EngineeringModel_REPLACE"."Iteration" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
CREATE INDEX "Idx_ValueGroup_Container" ON "Iteration_REPLACE"."ValueGroup" ("Container");

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Class Reference or Ordered Properties ------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Reference properties
-- ActualFiniteState.PossibleState is a collection property (many to many) of class PossibleFiniteState: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState" (
  "ActualFiniteState" uuid NOT NULL,
  "PossibleState" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ActualFiniteState_PossibleState_PK" PRIMARY KEY("ActualFiniteState", "PossibleState"),
  CONSTRAINT "ActualFiniteState_PossibleState_FK_Source" FOREIGN KEY ("ActualFiniteState") REFERENCES "Iteration_REPLACE"."ActualFiniteState" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ActualFiniteState_PossibleState_FK_Target" FOREIGN KEY ("PossibleState") REFERENCES "Iteration_REPLACE"."PossibleFiniteState" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ActualFiniteState_PossibleState_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteState_PossibleState" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteState_PossibleState_ValidTo" ON "Iteration_REPLACE"."ActualFiniteState_PossibleState" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList - Reference properties
-- ActualFiniteStateList.ExcludeOption is a collection property (many to many) of class Option: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" (
  "ActualFiniteStateList" uuid NOT NULL,
  "ExcludeOption" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ActualFiniteStateList_ExcludeOption_PK" PRIMARY KEY("ActualFiniteStateList", "ExcludeOption"),
  CONSTRAINT "ActualFiniteStateList_ExcludeOption_FK_Source" FOREIGN KEY ("ActualFiniteStateList") REFERENCES "Iteration_REPLACE"."ActualFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ActualFiniteStateList_ExcludeOption_FK_Target" FOREIGN KEY ("ExcludeOption") REFERENCES "Iteration_REPLACE"."Option" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ActualFiniteStateList_ExcludeOption_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateList_ExcludeOption_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList
-- ActualFiniteStateList.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList" ADD CONSTRAINT "ActualFiniteStateList_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ActualFiniteStateList
-- ActualFiniteStateList.PossibleFiniteStateList is an ordered collection property (many to many) of class PossibleFiniteStateList: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" (
  "ActualFiniteStateList" uuid NOT NULL,
  "PossibleFiniteStateList" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "ActualFiniteStateList_PossibleFiniteStateList_PK" PRIMARY KEY("ActualFiniteStateList", "PossibleFiniteStateList"),
  CONSTRAINT "ActualFiniteStateList_PossibleFiniteStateList_FK_Source" FOREIGN KEY ("ActualFiniteStateList") REFERENCES "Iteration_REPLACE"."ActualFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ActualFiniteStateList_PossibleFiniteStateList_FK_Target" FOREIGN KEY ("PossibleFiniteStateList") REFERENCES "Iteration_REPLACE"."PossibleFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ActualFiniteStateList_PossibleFiniteStateList_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateList_PossibleFiniteStateList_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList" SET (autovacuum_analyze_threshold = 2500);

-- Alias - Reference properties

-- AndExpression - Reference properties
-- AndExpression.Term is a collection property (many to many) of class BooleanExpression: [2..*]-[0..1]
CREATE TABLE "Iteration_REPLACE"."AndExpression_Term" (
  "AndExpression" uuid NOT NULL,
  "Term" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "AndExpression_Term_PK" PRIMARY KEY("AndExpression", "Term"),
  CONSTRAINT "AndExpression_Term_FK_Source" FOREIGN KEY ("AndExpression") REFERENCES "Iteration_REPLACE"."AndExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "AndExpression_Term_FK_Target" FOREIGN KEY ("Term") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_AndExpression_Term_ValidFrom" ON "Iteration_REPLACE"."AndExpression_Term" ("ValidFrom");
CREATE INDEX "Idx_AndExpression_Term_ValidTo" ON "Iteration_REPLACE"."AndExpression_Term" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship - Reference properties
-- BinaryRelationship.Source is an association to Thing: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" ADD COLUMN "Source" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" ADD CONSTRAINT "BinaryRelationship_FK_Source" FOREIGN KEY ("Source") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BinaryRelationship
-- BinaryRelationship.Target is an association to Thing: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" ADD COLUMN "Target" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship" ADD CONSTRAINT "BinaryRelationship_FK_Target" FOREIGN KEY ("Target") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- BooleanExpression - Reference properties

-- Bounds - Reference properties

-- BuiltInRuleVerification - Reference properties

-- Citation - Reference properties
-- Citation.Source is an association to ReferenceSource: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Citation" ADD COLUMN "Source" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Citation" ADD CONSTRAINT "Citation_FK_Source" FOREIGN KEY ("Source") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Color - Reference properties

-- DefinedThing - Reference properties

-- Definition - Reference properties
-- Definition.Example is an ordered collection property of type String: [0..*]
CREATE TABLE "Iteration_REPLACE"."Definition_Example" (
  "Definition" uuid NOT NULL,
  "Example" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Example_PK" PRIMARY KEY("Definition", "Example"),
  CONSTRAINT "Definition_Example_FK_Source" FOREIGN KEY ("Definition") REFERENCES "Iteration_REPLACE"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_Definition_Example_ValidFrom" ON "Iteration_REPLACE"."Definition_Example" ("ValidFrom");
CREATE INDEX "Idx_Definition_Example_ValidTo" ON "Iteration_REPLACE"."Definition_Example" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition_Example" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Example" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Example" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Example" SET (autovacuum_analyze_threshold = 2500);

-- Definition
-- Definition.Note is an ordered collection property of type String: [0..*]
CREATE TABLE "Iteration_REPLACE"."Definition_Note" (
  "Definition" uuid NOT NULL,
  "Note" text NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Note_PK" PRIMARY KEY("Definition", "Note"),
  CONSTRAINT "Definition_Note_FK_Source" FOREIGN KEY ("Definition") REFERENCES "Iteration_REPLACE"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_Definition_Note_ValidFrom" ON "Iteration_REPLACE"."Definition_Note" ("ValidFrom");
CREATE INDEX "Idx_Definition_Note_ValidTo" ON "Iteration_REPLACE"."Definition_Note" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition_Note" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Note" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Note" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Note" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas - Reference properties

-- DiagramEdge - Reference properties
-- DiagramEdge.Source is an association to DiagramElementThing: [1..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" ADD COLUMN "Source" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" ADD CONSTRAINT "DiagramEdge_FK_Source" FOREIGN KEY ("Source") REFERENCES "Iteration_REPLACE"."DiagramElementThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- DiagramEdge
-- DiagramEdge.Target is an association to DiagramElementThing: [1..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" ADD COLUMN "Target" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."DiagramEdge" ADD CONSTRAINT "DiagramEdge_FK_Target" FOREIGN KEY ("Target") REFERENCES "Iteration_REPLACE"."DiagramElementThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- DiagramElementContainer - Reference properties

-- DiagramElementThing - Reference properties
-- DiagramElementThing.DepictedThing is an optional association to Thing: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD COLUMN "DepictedThing" uuid;
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD CONSTRAINT "DiagramElementThing_FK_DepictedThing" FOREIGN KEY ("DepictedThing") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DiagramElementThing
-- DiagramElementThing.SharedStyle is an optional association to SharedStyle: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD COLUMN "SharedStyle" uuid;
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing" ADD CONSTRAINT "DiagramElementThing_FK_SharedStyle" FOREIGN KEY ("SharedStyle") REFERENCES "Iteration_REPLACE"."SharedStyle" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DiagrammingStyle - Reference properties
-- DiagrammingStyle.FillColor is an optional association to Color: [0..1]
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD COLUMN "FillColor" uuid;
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD CONSTRAINT "DiagrammingStyle_FK_FillColor" FOREIGN KEY ("FillColor") REFERENCES "Iteration_REPLACE"."Color" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DiagrammingStyle
-- DiagrammingStyle.FontColor is an optional association to Color: [0..1]
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD COLUMN "FontColor" uuid;
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD CONSTRAINT "DiagrammingStyle_FK_FontColor" FOREIGN KEY ("FontColor") REFERENCES "Iteration_REPLACE"."Color" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DiagrammingStyle
-- DiagrammingStyle.StrokeColor is an optional association to Color: [0..1]
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD COLUMN "StrokeColor" uuid;
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle" ADD CONSTRAINT "DiagrammingStyle_FK_StrokeColor" FOREIGN KEY ("StrokeColor") REFERENCES "Iteration_REPLACE"."Color" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- DiagramObject - Reference properties

-- DiagramShape - Reference properties

-- DiagramThingBase - Reference properties

-- DomainFileStore - Reference properties

-- ElementBase - Reference properties
-- ElementBase.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementBase_Category" (
  "ElementBase" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementBase_Category_PK" PRIMARY KEY("ElementBase", "Category"),
  CONSTRAINT "ElementBase_Category_FK_Source" FOREIGN KEY ("ElementBase") REFERENCES "Iteration_REPLACE"."ElementBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ElementBase_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ElementBase_Category_ValidFrom" ON "Iteration_REPLACE"."ElementBase_Category" ("ValidFrom");
CREATE INDEX "Idx_ElementBase_Category_ValidTo" ON "Iteration_REPLACE"."ElementBase_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category" SET (autovacuum_analyze_threshold = 2500);

-- ElementBase
-- ElementBase.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ElementBase" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ElementBase" ADD CONSTRAINT "ElementBase_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ElementDefinition - Reference properties
-- ElementDefinition.ReferencedElement is a collection property (many to many) of class NestedElement: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement" (
  "ElementDefinition" uuid NOT NULL,
  "ReferencedElement" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementDefinition_ReferencedElement_PK" PRIMARY KEY("ElementDefinition", "ReferencedElement"),
  CONSTRAINT "ElementDefinition_ReferencedElement_FK_Source" FOREIGN KEY ("ElementDefinition") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ElementDefinition_ReferencedElement_FK_Target" FOREIGN KEY ("ReferencedElement") REFERENCES "Iteration_REPLACE"."NestedElement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ElementDefinition_ReferencedElement_ValidFrom" ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_ReferencedElement_ValidTo" ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage - Reference properties
-- ElementUsage.ElementDefinition is an association to ElementDefinition: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ElementUsage" ADD COLUMN "ElementDefinition" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ElementUsage" ADD CONSTRAINT "ElementUsage_FK_ElementDefinition" FOREIGN KEY ("ElementDefinition") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ElementUsage
-- ElementUsage.ExcludeOption is a collection property (many to many) of class Option: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption" (
  "ElementUsage" uuid NOT NULL,
  "ExcludeOption" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementUsage_ExcludeOption_PK" PRIMARY KEY("ElementUsage", "ExcludeOption"),
  CONSTRAINT "ElementUsage_ExcludeOption_FK_Source" FOREIGN KEY ("ElementUsage") REFERENCES "Iteration_REPLACE"."ElementUsage" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ElementUsage_ExcludeOption_FK_Target" FOREIGN KEY ("ExcludeOption") REFERENCES "Iteration_REPLACE"."Option" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ElementUsage_ExcludeOption_ValidFrom" ON "Iteration_REPLACE"."ElementUsage_ExcludeOption" ("ValidFrom");
CREATE INDEX "Idx_ElementUsage_ExcludeOption_ValidTo" ON "Iteration_REPLACE"."ElementUsage_ExcludeOption" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression - Reference properties
-- ExclusiveOrExpression.Term is a collection property (many to many) of class BooleanExpression: [2..2]-[0..1]
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term" (
  "ExclusiveOrExpression" uuid NOT NULL,
  "Term" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ExclusiveOrExpression_Term_PK" PRIMARY KEY("ExclusiveOrExpression", "Term"),
  CONSTRAINT "ExclusiveOrExpression_Term_FK_Source" FOREIGN KEY ("ExclusiveOrExpression") REFERENCES "Iteration_REPLACE"."ExclusiveOrExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ExclusiveOrExpression_Term_FK_Target" FOREIGN KEY ("Term") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ExclusiveOrExpression_Term_ValidFrom" ON "Iteration_REPLACE"."ExclusiveOrExpression_Term" ("ValidFrom");
CREATE INDEX "Idx_ExclusiveOrExpression_Term_ValidTo" ON "Iteration_REPLACE"."ExclusiveOrExpression_Term" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap - Reference properties
-- ExternalIdentifierMap.ExternalFormat is an optional association to ReferenceSource: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD COLUMN "ExternalFormat" uuid;
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD CONSTRAINT "ExternalIdentifierMap_FK_ExternalFormat" FOREIGN KEY ("ExternalFormat") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ExternalIdentifierMap
-- ExternalIdentifierMap.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap" ADD CONSTRAINT "ExternalIdentifierMap_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- File - Reference properties
-- File.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."File_Category" (
  "File" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "File_Category_PK" PRIMARY KEY("File", "Category"),
  CONSTRAINT "File_Category_FK_Source" FOREIGN KEY ("File") REFERENCES "Iteration_REPLACE"."File" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "File_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_File_Category_ValidFrom" ON "Iteration_REPLACE"."File_Category" ("ValidFrom");
CREATE INDEX "Idx_File_Category_ValidTo" ON "Iteration_REPLACE"."File_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."File_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Category" SET (autovacuum_analyze_threshold = 2500);

-- File
-- File.LockedBy is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."File" ADD COLUMN "LockedBy" uuid;
ALTER TABLE "Iteration_REPLACE"."File" ADD CONSTRAINT "File_FK_LockedBy" FOREIGN KEY ("LockedBy") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- File
-- File.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."File" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."File" ADD CONSTRAINT "File_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- FileRevision - Reference properties
-- FileRevision.ContainingFolder is an optional association to Folder: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD COLUMN "ContainingFolder" uuid;
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_ContainingFolder" FOREIGN KEY ("ContainingFolder") REFERENCES "Iteration_REPLACE"."Folder" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- FileRevision
-- FileRevision.Creator is an association to Participant: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD COLUMN "Creator" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."FileRevision" ADD CONSTRAINT "FileRevision_FK_Creator" FOREIGN KEY ("Creator") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- FileRevision
-- FileRevision.FileType is an ordered collection property (many to many) of class FileType: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."FileRevision_FileType" (
  "FileRevision" uuid NOT NULL,
  "FileType" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "FileRevision_FileType_PK" PRIMARY KEY("FileRevision", "FileType"),
  CONSTRAINT "FileRevision_FileType_FK_Source" FOREIGN KEY ("FileRevision") REFERENCES "Iteration_REPLACE"."FileRevision" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "FileRevision_FileType_FK_Target" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_FileRevision_FileType_ValidFrom" ON "Iteration_REPLACE"."FileRevision_FileType" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_FileType_ValidTo" ON "Iteration_REPLACE"."FileRevision_FileType" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Reference properties
-- FileStore.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."FileStore" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."FileStore" ADD CONSTRAINT "FileStore_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Folder - Reference properties
-- Folder.ContainingFolder is an optional association to Folder: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Folder" ADD COLUMN "ContainingFolder" uuid;
ALTER TABLE "Iteration_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_ContainingFolder" FOREIGN KEY ("ContainingFolder") REFERENCES "Iteration_REPLACE"."Folder" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Folder
-- Folder.Creator is an association to Participant: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Folder" ADD COLUMN "Creator" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Creator" FOREIGN KEY ("Creator") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Folder
-- Folder.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Folder" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Folder" ADD CONSTRAINT "Folder_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Goal - Reference properties
-- Goal.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Goal_Category" (
  "Goal" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Goal_Category_PK" PRIMARY KEY("Goal", "Category"),
  CONSTRAINT "Goal_Category_FK_Source" FOREIGN KEY ("Goal") REFERENCES "Iteration_REPLACE"."Goal" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Goal_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Goal_Category_ValidFrom" ON "Iteration_REPLACE"."Goal_Category" ("ValidFrom");
CREATE INDEX "Idx_Goal_Category_ValidTo" ON "Iteration_REPLACE"."Goal_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Goal_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Category" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Reference properties

-- IdCorrespondence - Reference properties

-- MultiRelationship - Reference properties
-- MultiRelationship.RelatedThing is a collection property (many to many) of class Thing: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing" (
  "MultiRelationship" uuid NOT NULL,
  "RelatedThing" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "MultiRelationship_RelatedThing_PK" PRIMARY KEY("MultiRelationship", "RelatedThing"),
  CONSTRAINT "MultiRelationship_RelatedThing_FK_Source" FOREIGN KEY ("MultiRelationship") REFERENCES "Iteration_REPLACE"."MultiRelationship" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "MultiRelationship_RelatedThing_FK_Target" FOREIGN KEY ("RelatedThing") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_MultiRelationship_RelatedThing_ValidFrom" ON "Iteration_REPLACE"."MultiRelationship_RelatedThing" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationship_RelatedThing_ValidTo" ON "Iteration_REPLACE"."MultiRelationship_RelatedThing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement - Reference properties
-- NestedElement.ElementUsage is an ordered collection property (many to many) of class ElementUsage: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."NestedElement_ElementUsage" (
  "NestedElement" uuid NOT NULL,
  "ElementUsage" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  "Sequence" bigint NOT NULL,
  CONSTRAINT "NestedElement_ElementUsage_PK" PRIMARY KEY("NestedElement", "ElementUsage"),
  CONSTRAINT "NestedElement_ElementUsage_FK_Source" FOREIGN KEY ("NestedElement") REFERENCES "Iteration_REPLACE"."NestedElement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "NestedElement_ElementUsage_FK_Target" FOREIGN KEY ("ElementUsage") REFERENCES "Iteration_REPLACE"."ElementUsage" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_NestedElement_ElementUsage_ValidFrom" ON "Iteration_REPLACE"."NestedElement_ElementUsage" ("ValidFrom");
CREATE INDEX "Idx_NestedElement_ElementUsage_ValidTo" ON "Iteration_REPLACE"."NestedElement_ElementUsage" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement
-- NestedElement.RootElement is an association to ElementDefinition: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."NestedElement" ADD COLUMN "RootElement" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NestedElement" ADD CONSTRAINT "NestedElement_FK_RootElement" FOREIGN KEY ("RootElement") REFERENCES "Iteration_REPLACE"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- NestedParameter - Reference properties
-- NestedParameter.ActualState is an optional association to ActualFiniteState: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD COLUMN "ActualState" uuid;
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD CONSTRAINT "NestedParameter_FK_ActualState" FOREIGN KEY ("ActualState") REFERENCES "Iteration_REPLACE"."ActualFiniteState" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- NestedParameter
-- NestedParameter.AssociatedParameter is an association to ParameterBase: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD COLUMN "AssociatedParameter" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD CONSTRAINT "NestedParameter_FK_AssociatedParameter" FOREIGN KEY ("AssociatedParameter") REFERENCES "Iteration_REPLACE"."ParameterBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- NestedParameter
-- NestedParameter.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NestedParameter" ADD CONSTRAINT "NestedParameter_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- NotExpression - Reference properties
-- NotExpression.Term is an association to BooleanExpression: [1..1]-[0..1]
ALTER TABLE "Iteration_REPLACE"."NotExpression" ADD COLUMN "Term" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."NotExpression" ADD CONSTRAINT "NotExpression_FK_Term" FOREIGN KEY ("Term") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Option - Reference properties
-- Option.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Option_Category" (
  "Option" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Option_Category_PK" PRIMARY KEY("Option", "Category"),
  CONSTRAINT "Option_Category_FK_Source" FOREIGN KEY ("Option") REFERENCES "Iteration_REPLACE"."Option" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Option_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Option_Category_ValidFrom" ON "Iteration_REPLACE"."Option_Category" ("ValidFrom");
CREATE INDEX "Idx_Option_Category_ValidTo" ON "Iteration_REPLACE"."Option_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Option_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Category" SET (autovacuum_analyze_threshold = 2500);

-- Option is an ordered collection
ALTER TABLE "Iteration_REPLACE"."Option" ADD COLUMN "Sequence" bigint NOT NULL;

-- OrExpression - Reference properties
-- OrExpression.Term is a collection property (many to many) of class BooleanExpression: [2..*]-[0..1]
CREATE TABLE "Iteration_REPLACE"."OrExpression_Term" (
  "OrExpression" uuid NOT NULL,
  "Term" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "OrExpression_Term_PK" PRIMARY KEY("OrExpression", "Term"),
  CONSTRAINT "OrExpression_Term_FK_Source" FOREIGN KEY ("OrExpression") REFERENCES "Iteration_REPLACE"."OrExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "OrExpression_Term_FK_Target" FOREIGN KEY ("Term") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_OrExpression_Term_ValidFrom" ON "Iteration_REPLACE"."OrExpression_Term" ("ValidFrom");
CREATE INDEX "Idx_OrExpression_Term_ValidTo" ON "Iteration_REPLACE"."OrExpression_Term" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle - Reference properties

-- Parameter - Reference properties
-- Parameter.RequestedBy is an optional association to DomainOfExpertise: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Parameter" ADD COLUMN "RequestedBy" uuid;
ALTER TABLE "Iteration_REPLACE"."Parameter" ADD CONSTRAINT "Parameter_FK_RequestedBy" FOREIGN KEY ("RequestedBy") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterBase - Reference properties
-- ParameterBase.Group is an optional association to ParameterGroup derived by subclass: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD COLUMN "Group" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBase_FK_Group" FOREIGN KEY ("Group") REFERENCES "Iteration_REPLACE"."ParameterGroup" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterBase
-- ParameterBase.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBase_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterBase
-- ParameterBase.ParameterType is an association to ParameterType derived by subclass: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD COLUMN "ParameterType" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBase_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterBase
-- ParameterBase.Scale is an optional association to MeasurementScale derived by subclass: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD COLUMN "Scale" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBase_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterBase
-- ParameterBase.StateDependence is an optional association to ActualFiniteStateList derived by subclass: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD COLUMN "StateDependence" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterBase" ADD CONSTRAINT "ParameterBase_FK_StateDependence" FOREIGN KEY ("StateDependence") REFERENCES "Iteration_REPLACE"."ActualFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterGroup - Reference properties
-- ParameterGroup.ContainingGroup is an optional association to ParameterGroup: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" ADD COLUMN "ContainingGroup" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterGroup" ADD CONSTRAINT "ParameterGroup_FK_ContainingGroup" FOREIGN KEY ("ContainingGroup") REFERENCES "Iteration_REPLACE"."ParameterGroup" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterOrOverrideBase - Reference properties

-- ParameterOverride - Reference properties
-- ParameterOverride.Parameter is an association to Parameter: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" ADD COLUMN "Parameter" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterOverride" ADD CONSTRAINT "ParameterOverride_FK_Parameter" FOREIGN KEY ("Parameter") REFERENCES "Iteration_REPLACE"."Parameter" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterOverrideValueSet - Reference properties
-- ParameterOverrideValueSet.ParameterValueSet is an association to ParameterValueSet: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" ADD COLUMN "ParameterValueSet" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet" ADD CONSTRAINT "ParameterOverrideValueSet_FK_ParameterValueSet" FOREIGN KEY ("ParameterValueSet") REFERENCES "Iteration_REPLACE"."ParameterValueSet" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterSubscription - Reference properties

-- ParameterSubscriptionValueSet - Reference properties
-- ParameterSubscriptionValueSet.SubscribedValueSet is an association to ParameterValueSetBase: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" ADD COLUMN "SubscribedValueSet" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet" ADD CONSTRAINT "ParameterSubscriptionValueSet_FK_SubscribedValueSet" FOREIGN KEY ("SubscribedValueSet") REFERENCES "Iteration_REPLACE"."ParameterValueSetBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterValue - Reference properties
-- ParameterValue.ParameterType is an association to ParameterType: [1..1]
ALTER TABLE "Iteration_REPLACE"."ParameterValue" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."ParameterValue" ADD CONSTRAINT "ParameterValue_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ParameterValue
-- ParameterValue.Scale is an optional association to MeasurementScale: [0..1]
ALTER TABLE "Iteration_REPLACE"."ParameterValue" ADD COLUMN "Scale" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterValue" ADD CONSTRAINT "ParameterValue_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterValueSet - Reference properties

-- ParameterValueSetBase - Reference properties
-- ParameterValueSetBase.ActualOption is an optional association to Option derived by subclass: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" ADD COLUMN "ActualOption" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" ADD CONSTRAINT "ParameterValueSetBase_FK_ActualOption" FOREIGN KEY ("ActualOption") REFERENCES "Iteration_REPLACE"."Option" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParameterValueSetBase
-- ParameterValueSetBase.ActualState is an optional association to ActualFiniteState derived by subclass: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" ADD COLUMN "ActualState" uuid;
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase" ADD CONSTRAINT "ParameterValueSetBase_FK_ActualState" FOREIGN KEY ("ActualState") REFERENCES "Iteration_REPLACE"."ActualFiniteState" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParametricConstraint - Reference properties
-- ParametricConstraint.TopExpression is an optional association to BooleanExpression: [0..1]-[0..1]
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD COLUMN "TopExpression" uuid;
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD CONSTRAINT "ParametricConstraint_FK_TopExpression" FOREIGN KEY ("TopExpression") REFERENCES "Iteration_REPLACE"."BooleanExpression" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- ParametricConstraint is an ordered collection
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint" ADD COLUMN "Sequence" bigint NOT NULL;

-- Point - Reference properties
-- Point is an ordered collection
ALTER TABLE "Iteration_REPLACE"."Point" ADD COLUMN "Sequence" bigint NOT NULL;

-- PossibleFiniteState - Reference properties
-- PossibleFiniteState is an ordered collection
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState" ADD COLUMN "Sequence" bigint NOT NULL;

-- PossibleFiniteStateList - Reference properties
-- PossibleFiniteStateList.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category" (
  "PossibleFiniteStateList" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "PossibleFiniteStateList_Category_PK" PRIMARY KEY("PossibleFiniteStateList", "Category"),
  CONSTRAINT "PossibleFiniteStateList_Category_FK_Source" FOREIGN KEY ("PossibleFiniteStateList") REFERENCES "Iteration_REPLACE"."PossibleFiniteStateList" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "PossibleFiniteStateList_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_PossibleFiniteStateList_Category_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteStateList_Category" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteStateList_Category_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteStateList_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteStateList
-- PossibleFiniteStateList.DefaultState is an optional association to PossibleFiniteState: [0..1]-[1..1]
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD COLUMN "DefaultState" uuid;
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD CONSTRAINT "PossibleFiniteStateList_FK_DefaultState" FOREIGN KEY ("DefaultState") REFERENCES "Iteration_REPLACE"."PossibleFiniteState" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- PossibleFiniteStateList
-- PossibleFiniteStateList.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList" ADD CONSTRAINT "PossibleFiniteStateList_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Publication - Reference properties
-- Publication.Domain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Publication_Domain" (
  "Publication" uuid NOT NULL,
  "Domain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Publication_Domain_PK" PRIMARY KEY("Publication", "Domain"),
  CONSTRAINT "Publication_Domain_FK_Source" FOREIGN KEY ("Publication") REFERENCES "Iteration_REPLACE"."Publication" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Publication_Domain_FK_Target" FOREIGN KEY ("Domain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Publication_Domain_ValidFrom" ON "Iteration_REPLACE"."Publication_Domain" ("ValidFrom");
CREATE INDEX "Idx_Publication_Domain_ValidTo" ON "Iteration_REPLACE"."Publication_Domain" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication_Domain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain" SET (autovacuum_analyze_threshold = 2500);

-- Publication
-- Publication.PublishedParameter is a collection property (many to many) of class ParameterOrOverrideBase: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Publication_PublishedParameter" (
  "Publication" uuid NOT NULL,
  "PublishedParameter" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Publication_PublishedParameter_PK" PRIMARY KEY("Publication", "PublishedParameter"),
  CONSTRAINT "Publication_PublishedParameter_FK_Source" FOREIGN KEY ("Publication") REFERENCES "Iteration_REPLACE"."Publication" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Publication_PublishedParameter_FK_Target" FOREIGN KEY ("PublishedParameter") REFERENCES "Iteration_REPLACE"."ParameterOrOverrideBase" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Publication_PublishedParameter_ValidFrom" ON "Iteration_REPLACE"."Publication_PublishedParameter" ("ValidFrom");
CREATE INDEX "Idx_Publication_PublishedParameter_ValidTo" ON "Iteration_REPLACE"."Publication_PublishedParameter" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression - Reference properties
-- RelationalExpression.ParameterType is an association to ParameterType: [1..1]
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" ADD CONSTRAINT "RelationalExpression_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- RelationalExpression
-- RelationalExpression.Scale is an optional association to MeasurementScale: [0..1]
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" ADD COLUMN "Scale" uuid;
ALTER TABLE "Iteration_REPLACE"."RelationalExpression" ADD CONSTRAINT "RelationalExpression_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Relationship - Reference properties
-- Relationship.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Relationship_Category" (
  "Relationship" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Relationship_Category_PK" PRIMARY KEY("Relationship", "Category"),
  CONSTRAINT "Relationship_Category_FK_Source" FOREIGN KEY ("Relationship") REFERENCES "Iteration_REPLACE"."Relationship" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Relationship_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Relationship_Category_ValidFrom" ON "Iteration_REPLACE"."Relationship_Category" ("ValidFrom");
CREATE INDEX "Idx_Relationship_Category_ValidTo" ON "Iteration_REPLACE"."Relationship_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Relationship_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category" SET (autovacuum_analyze_threshold = 2500);

-- Relationship
-- Relationship.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Relationship" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."Relationship" ADD CONSTRAINT "Relationship_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- RelationshipParameterValue - Reference properties

-- Requirement - Reference properties
-- Requirement.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Requirement_Category" (
  "Requirement" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Requirement_Category_PK" PRIMARY KEY("Requirement", "Category"),
  CONSTRAINT "Requirement_Category_FK_Source" FOREIGN KEY ("Requirement") REFERENCES "Iteration_REPLACE"."Requirement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Requirement_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Requirement_Category_ValidFrom" ON "Iteration_REPLACE"."Requirement_Category" ("ValidFrom");
CREATE INDEX "Idx_Requirement_Category_ValidTo" ON "Iteration_REPLACE"."Requirement_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Requirement_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category" SET (autovacuum_analyze_threshold = 2500);

-- Requirement
-- Requirement.Group is an optional association to RequirementsGroup: [0..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."Requirement" ADD COLUMN "Group" uuid;
ALTER TABLE "Iteration_REPLACE"."Requirement" ADD CONSTRAINT "Requirement_FK_Group" FOREIGN KEY ("Group") REFERENCES "Iteration_REPLACE"."RequirementsGroup" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- RequirementsContainer - Reference properties
-- RequirementsContainer.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."RequirementsContainer_Category" (
  "RequirementsContainer" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RequirementsContainer_Category_PK" PRIMARY KEY("RequirementsContainer", "Category"),
  CONSTRAINT "RequirementsContainer_Category_FK_Source" FOREIGN KEY ("RequirementsContainer") REFERENCES "Iteration_REPLACE"."RequirementsContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "RequirementsContainer_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_RequirementsContainer_Category_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainer_Category" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainer_Category_ValidTo" ON "Iteration_REPLACE"."RequirementsContainer_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer
-- RequirementsContainer.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer" ADD CONSTRAINT "RequirementsContainer_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- RequirementsContainerParameterValue - Reference properties

-- RequirementsGroup - Reference properties

-- RequirementsSpecification - Reference properties

-- RuleVerification - Reference properties
-- RuleVerification is an ordered collection
ALTER TABLE "Iteration_REPLACE"."RuleVerification" ADD COLUMN "Sequence" bigint NOT NULL;

-- RuleVerificationList - Reference properties
-- RuleVerificationList.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList" ADD CONSTRAINT "RuleVerificationList_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- RuleViolation - Reference properties
-- RuleViolation.ViolatingThing is a collection property of type Guid: [0..*]
CREATE TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing" (
  "RuleViolation" uuid NOT NULL,
  "ViolatingThing" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "RuleViolation_ViolatingThing_PK" PRIMARY KEY("RuleViolation", "ViolatingThing"),
  CONSTRAINT "RuleViolation_ViolatingThing_FK_Source" FOREIGN KEY ("RuleViolation") REFERENCES "Iteration_REPLACE"."RuleViolation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
CREATE INDEX "Idx_RuleViolation_ViolatingThing_ValidFrom" ON "Iteration_REPLACE"."RuleViolation_ViolatingThing" ("ValidFrom");
CREATE INDEX "Idx_RuleViolation_ViolatingThing_ValidTo" ON "Iteration_REPLACE"."RuleViolation_ViolatingThing" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle - Reference properties

-- SimpleParameterizableThing - Reference properties
-- SimpleParameterizableThing.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing" ADD CONSTRAINT "SimpleParameterizableThing_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- SimpleParameterValue - Reference properties
-- SimpleParameterValue.ParameterType is an association to ParameterType: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD CONSTRAINT "SimpleParameterValue_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- SimpleParameterValue
-- SimpleParameterValue.Scale is an optional association to MeasurementScale: [0..1]
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD COLUMN "Scale" uuid;
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue" ADD CONSTRAINT "SimpleParameterValue_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Stakeholder - Reference properties
-- Stakeholder.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Stakeholder_Category" (
  "Stakeholder" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Stakeholder_Category_PK" PRIMARY KEY("Stakeholder", "Category"),
  CONSTRAINT "Stakeholder_Category_FK_Source" FOREIGN KEY ("Stakeholder") REFERENCES "Iteration_REPLACE"."Stakeholder" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Stakeholder_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Stakeholder_Category_ValidFrom" ON "Iteration_REPLACE"."Stakeholder_Category" ("ValidFrom");
CREATE INDEX "Idx_Stakeholder_Category_ValidTo" ON "Iteration_REPLACE"."Stakeholder_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder
-- Stakeholder.StakeholderValue is a collection property (many to many) of class StakeholderValue: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue" (
  "Stakeholder" uuid NOT NULL,
  "StakeholderValue" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Stakeholder_StakeholderValue_PK" PRIMARY KEY("Stakeholder", "StakeholderValue"),
  CONSTRAINT "Stakeholder_StakeholderValue_FK_Source" FOREIGN KEY ("Stakeholder") REFERENCES "Iteration_REPLACE"."Stakeholder" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Stakeholder_StakeholderValue_FK_Target" FOREIGN KEY ("StakeholderValue") REFERENCES "Iteration_REPLACE"."StakeholderValue" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Stakeholder_StakeholderValue_ValidFrom" ON "Iteration_REPLACE"."Stakeholder_StakeholderValue" ("ValidFrom");
CREATE INDEX "Idx_Stakeholder_StakeholderValue_ValidTo" ON "Iteration_REPLACE"."Stakeholder_StakeholderValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue - Reference properties
-- StakeholderValue.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."StakeholderValue_Category" (
  "StakeholderValue" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeholderValue_Category_PK" PRIMARY KEY("StakeholderValue", "Category"),
  CONSTRAINT "StakeholderValue_Category_FK_Source" FOREIGN KEY ("StakeholderValue") REFERENCES "Iteration_REPLACE"."StakeholderValue" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeholderValue_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeholderValue_Category_ValidFrom" ON "Iteration_REPLACE"."StakeholderValue_Category" ("ValidFrom");
CREATE INDEX "Idx_StakeholderValue_Category_ValidTo" ON "Iteration_REPLACE"."StakeholderValue_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap - Reference properties
-- StakeHolderValueMap.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category" (
  "StakeHolderValueMap" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_Category_PK" PRIMARY KEY("StakeHolderValueMap", "Category"),
  CONSTRAINT "StakeHolderValueMap_Category_FK_Source" FOREIGN KEY ("StakeHolderValueMap") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeHolderValueMap_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeHolderValueMap_Category_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Category" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_Category_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.Goal is a collection property (many to many) of class Goal: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal" (
  "StakeHolderValueMap" uuid NOT NULL,
  "Goal" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_Goal_PK" PRIMARY KEY("StakeHolderValueMap", "Goal"),
  CONSTRAINT "StakeHolderValueMap_Goal_FK_Source" FOREIGN KEY ("StakeHolderValueMap") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeHolderValueMap_Goal_FK_Target" FOREIGN KEY ("Goal") REFERENCES "Iteration_REPLACE"."Goal" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeHolderValueMap_Goal_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Goal" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_Goal_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Goal" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.Requirement is a collection property (many to many) of class Requirement: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement" (
  "StakeHolderValueMap" uuid NOT NULL,
  "Requirement" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_Requirement_PK" PRIMARY KEY("StakeHolderValueMap", "Requirement"),
  CONSTRAINT "StakeHolderValueMap_Requirement_FK_Source" FOREIGN KEY ("StakeHolderValueMap") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeHolderValueMap_Requirement_FK_Target" FOREIGN KEY ("Requirement") REFERENCES "Iteration_REPLACE"."Requirement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeHolderValueMap_Requirement_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_Requirement_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.StakeholderValue is a collection property (many to many) of class StakeholderValue: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" (
  "StakeHolderValueMap" uuid NOT NULL,
  "StakeholderValue" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_StakeholderValue_PK" PRIMARY KEY("StakeHolderValueMap", "StakeholderValue"),
  CONSTRAINT "StakeHolderValueMap_StakeholderValue_FK_Source" FOREIGN KEY ("StakeHolderValueMap") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeHolderValueMap_StakeholderValue_FK_Target" FOREIGN KEY ("StakeholderValue") REFERENCES "Iteration_REPLACE"."StakeholderValue" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeHolderValueMap_StakeholderValue_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_StakeholderValue_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.ValueGroup is a collection property (many to many) of class ValueGroup: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" (
  "StakeHolderValueMap" uuid NOT NULL,
  "ValueGroup" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "StakeHolderValueMap_ValueGroup_PK" PRIMARY KEY("StakeHolderValueMap", "ValueGroup"),
  CONSTRAINT "StakeHolderValueMap_ValueGroup_FK_Source" FOREIGN KEY ("StakeHolderValueMap") REFERENCES "Iteration_REPLACE"."StakeHolderValueMap" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "StakeHolderValueMap_ValueGroup_FK_Target" FOREIGN KEY ("ValueGroup") REFERENCES "Iteration_REPLACE"."ValueGroup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_StakeHolderValueMap_ValueGroup_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_ValueGroup_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings - Reference properties
-- StakeHolderValueMapSettings.GoalToValueGroupRelationship is an optional association to BinaryRelationshipRule: [0..1]
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD COLUMN "GoalToValueGroupRelationship" uuid;
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD CONSTRAINT "StakeHolderValueMapSettings_FK_GoalToValueGroupRelationship" FOREIGN KEY ("GoalToValueGroupRelationship") REFERENCES "SiteDirectory"."BinaryRelationshipRule" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- StakeHolderValueMapSettings
-- StakeHolderValueMapSettings.StakeholderValueToRequirementRelationship is an optional association to BinaryRelationshipRule: [0..1]
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD COLUMN "StakeholderValueToRequirementRelationship" uuid;
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD CONSTRAINT "StakeHolderValueMapSettings_FK_StakeholderValueToRequirementRelationship" FOREIGN KEY ("StakeholderValueToRequirementRelationship") REFERENCES "SiteDirectory"."BinaryRelationshipRule" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- StakeHolderValueMapSettings
-- StakeHolderValueMapSettings.ValueGroupToStakeholderValueRelationship is an optional association to BinaryRelationshipRule: [0..1]
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD COLUMN "ValueGroupToStakeholderValueRelationship" uuid;
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings" ADD CONSTRAINT "StakeHolderValueMapSettings_FK_ValueGroupToStakeholderValueRelationship" FOREIGN KEY ("ValueGroupToStakeholderValueRelationship") REFERENCES "SiteDirectory"."BinaryRelationshipRule" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- Thing - Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Thing_ExcludedDomain" (
  "Thing" uuid NOT NULL,
  "ExcludedDomain" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedDomain_PK" PRIMARY KEY("Thing", "ExcludedDomain"),
  CONSTRAINT "Thing_ExcludedDomain_FK_Source" FOREIGN KEY ("Thing") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedDomain_FK_Target" FOREIGN KEY ("ExcludedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidFrom" ON "Iteration_REPLACE"."Thing_ExcludedDomain" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidTo" ON "Iteration_REPLACE"."Thing_ExcludedDomain" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Thing_ExcludedPerson" (
  "Thing" uuid NOT NULL,
  "ExcludedPerson" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "Thing_ExcludedPerson_PK" PRIMARY KEY("Thing", "ExcludedPerson"),
  CONSTRAINT "Thing_ExcludedPerson_FK_Source" FOREIGN KEY ("Thing") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedPerson_FK_Target" FOREIGN KEY ("ExcludedPerson") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidFrom" ON "Iteration_REPLACE"."Thing_ExcludedPerson" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidTo" ON "Iteration_REPLACE"."Thing_ExcludedPerson" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson" SET (autovacuum_analyze_threshold = 2500);

-- UserRuleVerification - Reference properties
-- UserRuleVerification.Rule is an association to Rule: [1..1]-[0..*]
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" ADD COLUMN "Rule" uuid NOT NULL;
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification" ADD CONSTRAINT "UserRuleVerification_FK_Rule" FOREIGN KEY ("Rule") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- ValueGroup - Reference properties
-- ValueGroup.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ValueGroup_Category" (
  "ValueGroup" uuid NOT NULL,
  "Category" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ValueGroup_Category_PK" PRIMARY KEY("ValueGroup", "Category"),
  CONSTRAINT "ValueGroup_Category_FK_Source" FOREIGN KEY ("ValueGroup") REFERENCES "Iteration_REPLACE"."ValueGroup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ValueGroup_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);
CREATE INDEX "Idx_ValueGroup_Category_ValidFrom" ON "Iteration_REPLACE"."ValueGroup_Category" ("ValidFrom");
CREATE INDEX "Idx_ValueGroup_Category_ValidTo" ON "Iteration_REPLACE"."ValueGroup_Category" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Revision-History tables --------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - revision history table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActualFiniteState_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList - revision history table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActualFiniteStateList_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Alias - revision history table
CREATE TABLE "Iteration_REPLACE"."Alias_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Alias_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Alias_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Revision" SET (autovacuum_analyze_threshold = 2500);

-- AndExpression - revision history table
CREATE TABLE "Iteration_REPLACE"."AndExpression_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "AndExpression_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship - revision history table
CREATE TABLE "Iteration_REPLACE"."BinaryRelationship_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationship_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BooleanExpression - revision history table
-- The BooleanExpression class is abstract and therefore does not have a Revision table

-- Bounds - revision history table
CREATE TABLE "Iteration_REPLACE"."Bounds_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Bounds_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Bounds_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Bounds_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Revision" SET (autovacuum_analyze_threshold = 2500);

-- BuiltInRuleVerification - revision history table
CREATE TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BuiltInRuleVerification_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Citation - revision history table
CREATE TABLE "Iteration_REPLACE"."Citation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Citation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Citation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Color - revision history table
CREATE TABLE "Iteration_REPLACE"."Color_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Color_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Color_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Color_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - revision history table
-- The DefinedThing class is abstract and therefore does not have a Revision table

-- Definition - revision history table
CREATE TABLE "Iteration_REPLACE"."Definition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Definition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas - revision history table
CREATE TABLE "Iteration_REPLACE"."DiagramCanvas_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramCanvas_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DiagramEdge - revision history table
CREATE TABLE "Iteration_REPLACE"."DiagramEdge_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramEdge_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementContainer - revision history table
-- The DiagramElementContainer class is abstract and therefore does not have a Revision table

-- DiagramElementThing - revision history table
-- The DiagramElementThing class is abstract and therefore does not have a Revision table

-- DiagrammingStyle - revision history table
-- The DiagrammingStyle class is abstract and therefore does not have a Revision table

-- DiagramObject - revision history table
CREATE TABLE "Iteration_REPLACE"."DiagramObject_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramObject_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Revision" SET (autovacuum_analyze_threshold = 2500);

-- DiagramShape - revision history table
-- The DiagramShape class is abstract and therefore does not have a Revision table

-- DiagramThingBase - revision history table
-- The DiagramThingBase class is abstract and therefore does not have a Revision table

-- DomainFileStore - revision history table
CREATE TABLE "Iteration_REPLACE"."DomainFileStore_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainFileStore_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ElementBase - revision history table
-- The ElementBase class is abstract and therefore does not have a Revision table

-- ElementDefinition - revision history table
CREATE TABLE "Iteration_REPLACE"."ElementDefinition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ElementDefinition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage - revision history table
CREATE TABLE "Iteration_REPLACE"."ElementUsage_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ElementUsage_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression - revision history table
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ExclusiveOrExpression_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap - revision history table
CREATE TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ExternalIdentifierMap_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Revision" SET (autovacuum_analyze_threshold = 2500);

-- File - revision history table
CREATE TABLE "Iteration_REPLACE"."File_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "File_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."File_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Revision" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - revision history table
CREATE TABLE "Iteration_REPLACE"."FileRevision_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileRevision_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Revision" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - revision history table
-- The FileStore class is abstract and therefore does not have a Revision table

-- Folder - revision history table
CREATE TABLE "Iteration_REPLACE"."Folder_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Folder_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Folder_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Folder_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Goal - revision history table
CREATE TABLE "Iteration_REPLACE"."Goal_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Goal_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Goal_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Revision" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - revision history table
CREATE TABLE "Iteration_REPLACE"."HyperLink_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Revision" SET (autovacuum_analyze_threshold = 2500);

-- IdCorrespondence - revision history table
CREATE TABLE "Iteration_REPLACE"."IdCorrespondence_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IdCorrespondence_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Revision" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationship - revision history table
CREATE TABLE "Iteration_REPLACE"."MultiRelationship_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationship_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Revision" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement - revision history table
CREATE TABLE "Iteration_REPLACE"."NestedElement_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NestedElement_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Revision" SET (autovacuum_analyze_threshold = 2500);

-- NestedParameter - revision history table
CREATE TABLE "Iteration_REPLACE"."NestedParameter_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NestedParameter_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Revision" SET (autovacuum_analyze_threshold = 2500);

-- NotExpression - revision history table
CREATE TABLE "Iteration_REPLACE"."NotExpression_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NotExpression_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Option - revision history table
CREATE TABLE "Iteration_REPLACE"."Option_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Option_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Option_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Revision" SET (autovacuum_analyze_threshold = 2500);

-- OrExpression - revision history table
CREATE TABLE "Iteration_REPLACE"."OrExpression_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrExpression_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Revision" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle - revision history table
CREATE TABLE "Iteration_REPLACE"."OwnedStyle_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OwnedStyle_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Parameter - revision history table
CREATE TABLE "Iteration_REPLACE"."Parameter_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Parameter_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Parameter_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Parameter_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterBase - revision history table
-- The ParameterBase class is abstract and therefore does not have a Revision table

-- ParameterGroup - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterGroup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterGroup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOrOverrideBase - revision history table
-- The ParameterOrOverrideBase class is abstract and therefore does not have a Revision table

-- ParameterOverride - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterOverride_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterOverride_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverrideValueSet - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterOverrideValueSet_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscription - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscription_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterSubscription_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscriptionValueSet - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterSubscriptionValueSet_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValue - revision history table
-- The ParameterValue class is abstract and therefore does not have a Revision table

-- ParameterValueSet - revision history table
CREATE TABLE "Iteration_REPLACE"."ParameterValueSet_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterValueSet_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSetBase - revision history table
-- The ParameterValueSetBase class is abstract and therefore does not have a Revision table

-- ParametricConstraint - revision history table
CREATE TABLE "Iteration_REPLACE"."ParametricConstraint_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParametricConstraint_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Point - revision history table
CREATE TABLE "Iteration_REPLACE"."Point_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Point_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Point_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Point_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Revision" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteState - revision history table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteState_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PossibleFiniteState_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Revision" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteStateList - revision history table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PossibleFiniteStateList_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Publication - revision history table
CREATE TABLE "Iteration_REPLACE"."Publication_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Publication_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Publication_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression - revision history table
CREATE TABLE "Iteration_REPLACE"."RelationalExpression_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RelationalExpression_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Relationship - revision history table
-- The Relationship class is abstract and therefore does not have a Revision table

-- RelationshipParameterValue - revision history table
CREATE TABLE "Iteration_REPLACE"."RelationshipParameterValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RelationshipParameterValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Requirement - revision history table
CREATE TABLE "Iteration_REPLACE"."Requirement_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Requirement_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Requirement_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer - revision history table
-- The RequirementsContainer class is abstract and therefore does not have a Revision table

-- RequirementsContainerParameterValue - revision history table
CREATE TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsContainerParameterValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsGroup - revision history table
CREATE TABLE "Iteration_REPLACE"."RequirementsGroup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsGroup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsSpecification - revision history table
CREATE TABLE "Iteration_REPLACE"."RequirementsSpecification_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsSpecification_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerification - revision history table
-- The RuleVerification class is abstract and therefore does not have a Revision table

-- RuleVerificationList - revision history table
CREATE TABLE "Iteration_REPLACE"."RuleVerificationList_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RuleVerificationList_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Revision" SET (autovacuum_analyze_threshold = 2500);

-- RuleViolation - revision history table
CREATE TABLE "Iteration_REPLACE"."RuleViolation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RuleViolation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle - revision history table
CREATE TABLE "Iteration_REPLACE"."SharedStyle_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SharedStyle_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Revision" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterizableThing - revision history table
-- The SimpleParameterizableThing class is abstract and therefore does not have a Revision table

-- SimpleParameterValue - revision history table
CREATE TABLE "Iteration_REPLACE"."SimpleParameterValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleParameterValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder - revision history table
CREATE TABLE "Iteration_REPLACE"."Stakeholder_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Stakeholder_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Revision" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue - revision history table
CREATE TABLE "Iteration_REPLACE"."StakeholderValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeholderValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Revision" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap - revision history table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeHolderValueMap_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Revision" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings - revision history table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeHolderValueMapSettings_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Revision" SET (autovacuum_analyze_threshold = 2500);

-- Thing - revision history table
-- The Thing class is abstract and therefore does not have a Revision table

-- UserRuleVerification - revision history table
CREATE TABLE "Iteration_REPLACE"."UserRuleVerification_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserRuleVerification_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Revision" SET (autovacuum_analyze_threshold = 2500);

-- ValueGroup - revision history table
CREATE TABLE "Iteration_REPLACE"."ValueGroup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ValueGroup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Revision" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Revision" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Cache tables -------------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Cache table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActualFiniteState_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ActualFiniteStateCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList - Cache table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ActualFiniteStateList_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ActualFiniteStateListCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Alias - Cache table
CREATE TABLE "Iteration_REPLACE"."Alias_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AliasCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Alias_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Alias_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Cache" SET (autovacuum_analyze_threshold = 2500);

-- AndExpression - Cache table
CREATE TABLE "Iteration_REPLACE"."AndExpression_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "AndExpression_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AndExpressionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship - Cache table
CREATE TABLE "Iteration_REPLACE"."BinaryRelationship_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationship_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BinaryRelationshipCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BooleanExpression - Cache table
-- The BooleanExpression class is abstract and therefore does not have a Cache table

-- Bounds - Cache table
CREATE TABLE "Iteration_REPLACE"."Bounds_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Bounds_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BoundsCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Bounds_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Bounds_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Cache" SET (autovacuum_analyze_threshold = 2500);

-- BuiltInRuleVerification - Cache table
CREATE TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BuiltInRuleVerification_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BuiltInRuleVerificationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Cache table
CREATE TABLE "Iteration_REPLACE"."Citation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CitationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Citation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Citation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Color - Cache table
CREATE TABLE "Iteration_REPLACE"."Color_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Color_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ColorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Color_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Color_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - Cache table
-- The DefinedThing class is abstract and therefore does not have a Cache table

-- Definition - Cache table
CREATE TABLE "Iteration_REPLACE"."Definition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Definition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas - Cache table
CREATE TABLE "Iteration_REPLACE"."DiagramCanvas_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramCanvas_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DiagramCanvasCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DiagramEdge - Cache table
CREATE TABLE "Iteration_REPLACE"."DiagramEdge_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramEdge_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DiagramEdgeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementContainer - Cache table
-- The DiagramElementContainer class is abstract and therefore does not have a Cache table

-- DiagramElementThing - Cache table
-- The DiagramElementThing class is abstract and therefore does not have a Cache table

-- DiagrammingStyle - Cache table
-- The DiagrammingStyle class is abstract and therefore does not have a Cache table

-- DiagramObject - Cache table
CREATE TABLE "Iteration_REPLACE"."DiagramObject_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramObject_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DiagramObjectCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Cache" SET (autovacuum_analyze_threshold = 2500);

-- DiagramShape - Cache table
-- The DiagramShape class is abstract and therefore does not have a Cache table

-- DiagramThingBase - Cache table
-- The DiagramThingBase class is abstract and therefore does not have a Cache table

-- DomainFileStore - Cache table
CREATE TABLE "Iteration_REPLACE"."DomainFileStore_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainFileStore_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DomainFileStoreCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ElementBase - Cache table
-- The ElementBase class is abstract and therefore does not have a Cache table

-- ElementDefinition - Cache table
CREATE TABLE "Iteration_REPLACE"."ElementDefinition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ElementDefinition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ElementDefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage - Cache table
CREATE TABLE "Iteration_REPLACE"."ElementUsage_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ElementUsage_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ElementUsageCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression - Cache table
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ExclusiveOrExpression_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ExclusiveOrExpressionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap - Cache table
CREATE TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ExternalIdentifierMap_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ExternalIdentifierMapCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Cache" SET (autovacuum_analyze_threshold = 2500);

-- File - Cache table
CREATE TABLE "Iteration_REPLACE"."File_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "File_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."File_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Cache" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Cache table
CREATE TABLE "Iteration_REPLACE"."FileRevision_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileRevision_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileRevisionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Cache" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Cache table
-- The FileStore class is abstract and therefore does not have a Cache table

-- Folder - Cache table
CREATE TABLE "Iteration_REPLACE"."Folder_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Folder_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FolderCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Folder_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Folder_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Goal - Cache table
CREATE TABLE "Iteration_REPLACE"."Goal_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Goal_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "GoalCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Goal_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Cache" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Cache table
CREATE TABLE "Iteration_REPLACE"."HyperLink_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "HyperLinkCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Cache" SET (autovacuum_analyze_threshold = 2500);

-- IdCorrespondence - Cache table
CREATE TABLE "Iteration_REPLACE"."IdCorrespondence_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IdCorrespondence_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IdCorrespondenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Cache" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationship - Cache table
CREATE TABLE "Iteration_REPLACE"."MultiRelationship_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationship_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "MultiRelationshipCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Cache" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement - Cache table
CREATE TABLE "Iteration_REPLACE"."NestedElement_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NestedElement_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "NestedElementCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Cache" SET (autovacuum_analyze_threshold = 2500);

-- NestedParameter - Cache table
CREATE TABLE "Iteration_REPLACE"."NestedParameter_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NestedParameter_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "NestedParameterCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Cache" SET (autovacuum_analyze_threshold = 2500);

-- NotExpression - Cache table
CREATE TABLE "Iteration_REPLACE"."NotExpression_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NotExpression_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "NotExpressionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Option - Cache table
CREATE TABLE "Iteration_REPLACE"."Option_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Option_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OptionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Option_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Cache" SET (autovacuum_analyze_threshold = 2500);

-- OrExpression - Cache table
CREATE TABLE "Iteration_REPLACE"."OrExpression_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrExpression_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrExpressionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Cache" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle - Cache table
CREATE TABLE "Iteration_REPLACE"."OwnedStyle_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OwnedStyle_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OwnedStyleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Parameter - Cache table
CREATE TABLE "Iteration_REPLACE"."Parameter_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Parameter_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Parameter_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Parameter_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterBase - Cache table
-- The ParameterBase class is abstract and therefore does not have a Cache table

-- ParameterGroup - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterGroup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterGroup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterGroupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOrOverrideBase - Cache table
-- The ParameterOrOverrideBase class is abstract and therefore does not have a Cache table

-- ParameterOverride - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterOverride_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterOverride_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterOverrideCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverrideValueSet - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterOverrideValueSet_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterOverrideValueSetCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscription - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscription_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterSubscription_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterSubscriptionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscriptionValueSet - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterSubscriptionValueSet_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterSubscriptionValueSetCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValue - Cache table
-- The ParameterValue class is abstract and therefore does not have a Cache table

-- ParameterValueSet - Cache table
CREATE TABLE "Iteration_REPLACE"."ParameterValueSet_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterValueSet_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterValueSetCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSetBase - Cache table
-- The ParameterValueSetBase class is abstract and therefore does not have a Cache table

-- ParametricConstraint - Cache table
CREATE TABLE "Iteration_REPLACE"."ParametricConstraint_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParametricConstraint_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParametricConstraintCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Point - Cache table
CREATE TABLE "Iteration_REPLACE"."Point_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Point_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PointCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Point_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Point_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Cache" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteState - Cache table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteState_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PossibleFiniteState_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PossibleFiniteStateCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Cache" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteStateList - Cache table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PossibleFiniteStateList_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PossibleFiniteStateListCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Publication - Cache table
CREATE TABLE "Iteration_REPLACE"."Publication_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Publication_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PublicationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Publication_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression - Cache table
CREATE TABLE "Iteration_REPLACE"."RelationalExpression_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RelationalExpression_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RelationalExpressionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Relationship - Cache table
-- The Relationship class is abstract and therefore does not have a Cache table

-- RelationshipParameterValue - Cache table
CREATE TABLE "Iteration_REPLACE"."RelationshipParameterValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RelationshipParameterValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RelationshipParameterValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Requirement - Cache table
CREATE TABLE "Iteration_REPLACE"."Requirement_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Requirement_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequirementCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Requirement_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer - Cache table
-- The RequirementsContainer class is abstract and therefore does not have a Cache table

-- RequirementsContainerParameterValue - Cache table
CREATE TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsContainerParameterValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequirementsContainerParameterValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsGroup - Cache table
CREATE TABLE "Iteration_REPLACE"."RequirementsGroup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsGroup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequirementsGroupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsSpecification - Cache table
CREATE TABLE "Iteration_REPLACE"."RequirementsSpecification_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RequirementsSpecification_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RequirementsSpecificationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerification - Cache table
-- The RuleVerification class is abstract and therefore does not have a Cache table

-- RuleVerificationList - Cache table
CREATE TABLE "Iteration_REPLACE"."RuleVerificationList_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RuleVerificationList_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RuleVerificationListCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Cache" SET (autovacuum_analyze_threshold = 2500);

-- RuleViolation - Cache table
CREATE TABLE "Iteration_REPLACE"."RuleViolation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RuleViolation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RuleViolationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle - Cache table
CREATE TABLE "Iteration_REPLACE"."SharedStyle_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SharedStyle_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SharedStyleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Cache" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterizableThing - Cache table
-- The SimpleParameterizableThing class is abstract and therefore does not have a Cache table

-- SimpleParameterValue - Cache table
CREATE TABLE "Iteration_REPLACE"."SimpleParameterValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleParameterValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SimpleParameterValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder - Cache table
CREATE TABLE "Iteration_REPLACE"."Stakeholder_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Stakeholder_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "StakeholderCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Cache" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue - Cache table
CREATE TABLE "Iteration_REPLACE"."StakeholderValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeholderValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "StakeholderValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Cache" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap - Cache table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeHolderValueMap_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "StakeHolderValueMapCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Cache" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings - Cache table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "StakeHolderValueMapSettings_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "StakeHolderValueMapSettingsCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Cache table
-- The Thing class is abstract and therefore does not have a Cache table

-- UserRuleVerification - Cache table
CREATE TABLE "Iteration_REPLACE"."UserRuleVerification_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserRuleVerification_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UserRuleVerificationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Cache" SET (autovacuum_analyze_threshold = 2500);

-- ValueGroup - Cache table
CREATE TABLE "Iteration_REPLACE"."ValueGroup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ValueGroup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ValueGroupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "Iteration_REPLACE"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Cache" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Cache" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Audit class --------------------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Audit table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" (LIKE "Iteration_REPLACE"."ActualFiniteState");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActualFiniteStateAudit_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteState_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateAudit_ValidTo" ON "Iteration_REPLACE"."ActualFiniteState_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList - Audit table
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" (LIKE "Iteration_REPLACE"."ActualFiniteStateList");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActualFiniteStateListAudit_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateListAudit_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Alias - Audit table
CREATE TABLE "Iteration_REPLACE"."Alias_Audit" (LIKE "Iteration_REPLACE"."Alias");
ALTER TABLE "Iteration_REPLACE"."Alias_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AliasAudit_ValidFrom" ON "Iteration_REPLACE"."Alias_Audit" ("ValidFrom");
CREATE INDEX "Idx_AliasAudit_ValidTo" ON "Iteration_REPLACE"."Alias_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Alias_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Alias_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Alias_Audit" SET (autovacuum_analyze_threshold = 2500);

-- AndExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."AndExpression_Audit" (LIKE "Iteration_REPLACE"."AndExpression");
ALTER TABLE "Iteration_REPLACE"."AndExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AndExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."AndExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_AndExpressionAudit_ValidTo" ON "Iteration_REPLACE"."AndExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."AndExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship - Audit table
CREATE TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" (LIKE "Iteration_REPLACE"."BinaryRelationship");
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BinaryRelationshipAudit_ValidFrom" ON "Iteration_REPLACE"."BinaryRelationship_Audit" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationshipAudit_ValidTo" ON "Iteration_REPLACE"."BinaryRelationship_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BinaryRelationship_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BooleanExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."BooleanExpression_Audit" (LIKE "Iteration_REPLACE"."BooleanExpression");
ALTER TABLE "Iteration_REPLACE"."BooleanExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BooleanExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."BooleanExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_BooleanExpressionAudit_ValidTo" ON "Iteration_REPLACE"."BooleanExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BooleanExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BooleanExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Bounds - Audit table
CREATE TABLE "Iteration_REPLACE"."Bounds_Audit" (LIKE "Iteration_REPLACE"."Bounds");
ALTER TABLE "Iteration_REPLACE"."Bounds_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BoundsAudit_ValidFrom" ON "Iteration_REPLACE"."Bounds_Audit" ("ValidFrom");
CREATE INDEX "Idx_BoundsAudit_ValidTo" ON "Iteration_REPLACE"."Bounds_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Bounds_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Bounds_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Bounds_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BuiltInRuleVerification - Audit table
CREATE TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" (LIKE "Iteration_REPLACE"."BuiltInRuleVerification");
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BuiltInRuleVerificationAudit_ValidFrom" ON "Iteration_REPLACE"."BuiltInRuleVerification_Audit" ("ValidFrom");
CREATE INDEX "Idx_BuiltInRuleVerificationAudit_ValidTo" ON "Iteration_REPLACE"."BuiltInRuleVerification_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."BuiltInRuleVerification_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Citation - Audit table
CREATE TABLE "Iteration_REPLACE"."Citation_Audit" (LIKE "Iteration_REPLACE"."Citation");
ALTER TABLE "Iteration_REPLACE"."Citation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CitationAudit_ValidFrom" ON "Iteration_REPLACE"."Citation_Audit" ("ValidFrom");
CREATE INDEX "Idx_CitationAudit_ValidTo" ON "Iteration_REPLACE"."Citation_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Citation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Citation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Citation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Color - Audit table
CREATE TABLE "Iteration_REPLACE"."Color_Audit" (LIKE "Iteration_REPLACE"."Color");
ALTER TABLE "Iteration_REPLACE"."Color_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ColorAudit_ValidFrom" ON "Iteration_REPLACE"."Color_Audit" ("ValidFrom");
CREATE INDEX "Idx_ColorAudit_ValidTo" ON "Iteration_REPLACE"."Color_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Color_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Color_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Color_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DefinedThing - Audit table
CREATE TABLE "Iteration_REPLACE"."DefinedThing_Audit" (LIKE "Iteration_REPLACE"."DefinedThing");
ALTER TABLE "Iteration_REPLACE"."DefinedThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinedThingAudit_ValidFrom" ON "Iteration_REPLACE"."DefinedThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinedThingAudit_ValidTo" ON "Iteration_REPLACE"."DefinedThing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DefinedThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DefinedThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DefinedThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DefinedThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Definition - Audit table
CREATE TABLE "Iteration_REPLACE"."Definition_Audit" (LIKE "Iteration_REPLACE"."Definition");
ALTER TABLE "Iteration_REPLACE"."Definition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinitionAudit_ValidFrom" ON "Iteration_REPLACE"."Definition_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinitionAudit_ValidTo" ON "Iteration_REPLACE"."Definition_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" (LIKE "Iteration_REPLACE"."DiagramCanvas");
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramCanvasAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramCanvas_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramCanvasAudit_ValidTo" ON "Iteration_REPLACE"."DiagramCanvas_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramCanvas_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramEdge - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramEdge_Audit" (LIKE "Iteration_REPLACE"."DiagramEdge");
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramEdgeAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramEdge_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramEdgeAudit_ValidTo" ON "Iteration_REPLACE"."DiagramEdge_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramEdge_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementContainer - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" (LIKE "Iteration_REPLACE"."DiagramElementContainer");
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramElementContainerAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramElementContainer_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramElementContainerAudit_ValidTo" ON "Iteration_REPLACE"."DiagramElementContainer_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementContainer_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramElementThing - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" (LIKE "Iteration_REPLACE"."DiagramElementThing");
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramElementThingAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramElementThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramElementThingAudit_ValidTo" ON "Iteration_REPLACE"."DiagramElementThing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramElementThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagrammingStyle - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" (LIKE "Iteration_REPLACE"."DiagrammingStyle");
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagrammingStyleAudit_ValidFrom" ON "Iteration_REPLACE"."DiagrammingStyle_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagrammingStyleAudit_ValidTo" ON "Iteration_REPLACE"."DiagrammingStyle_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagrammingStyle_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramObject - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramObject_Audit" (LIKE "Iteration_REPLACE"."DiagramObject");
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramObjectAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramObject_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramObjectAudit_ValidTo" ON "Iteration_REPLACE"."DiagramObject_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramObject_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramShape - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramShape_Audit" (LIKE "Iteration_REPLACE"."DiagramShape");
ALTER TABLE "Iteration_REPLACE"."DiagramShape_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramShapeAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramShape_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramShapeAudit_ValidTo" ON "Iteration_REPLACE"."DiagramShape_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramShape_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramShape_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramShape_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramShape_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramThingBase - Audit table
CREATE TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" (LIKE "Iteration_REPLACE"."DiagramThingBase");
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramThingBaseAudit_ValidFrom" ON "Iteration_REPLACE"."DiagramThingBase_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramThingBaseAudit_ValidTo" ON "Iteration_REPLACE"."DiagramThingBase_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DiagramThingBase_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DomainFileStore - Audit table
CREATE TABLE "Iteration_REPLACE"."DomainFileStore_Audit" (LIKE "Iteration_REPLACE"."DomainFileStore");
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainFileStoreAudit_ValidFrom" ON "Iteration_REPLACE"."DomainFileStore_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainFileStoreAudit_ValidTo" ON "Iteration_REPLACE"."DomainFileStore_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."DomainFileStore_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ElementBase - Audit table
CREATE TABLE "Iteration_REPLACE"."ElementBase_Audit" (LIKE "Iteration_REPLACE"."ElementBase");
ALTER TABLE "Iteration_REPLACE"."ElementBase_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementBaseAudit_ValidFrom" ON "Iteration_REPLACE"."ElementBase_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementBaseAudit_ValidTo" ON "Iteration_REPLACE"."ElementBase_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementBase_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ElementDefinition - Audit table
CREATE TABLE "Iteration_REPLACE"."ElementDefinition_Audit" (LIKE "Iteration_REPLACE"."ElementDefinition");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementDefinitionAudit_ValidFrom" ON "Iteration_REPLACE"."ElementDefinition_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinitionAudit_ValidTo" ON "Iteration_REPLACE"."ElementDefinition_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage - Audit table
CREATE TABLE "Iteration_REPLACE"."ElementUsage_Audit" (LIKE "Iteration_REPLACE"."ElementUsage");
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementUsageAudit_ValidFrom" ON "Iteration_REPLACE"."ElementUsage_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementUsageAudit_ValidTo" ON "Iteration_REPLACE"."ElementUsage_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" (LIKE "Iteration_REPLACE"."ExclusiveOrExpression");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ExclusiveOrExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."ExclusiveOrExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_ExclusiveOrExpressionAudit_ValidTo" ON "Iteration_REPLACE"."ExclusiveOrExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap - Audit table
CREATE TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" (LIKE "Iteration_REPLACE"."ExternalIdentifierMap");
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ExternalIdentifierMapAudit_ValidFrom" ON "Iteration_REPLACE"."ExternalIdentifierMap_Audit" ("ValidFrom");
CREATE INDEX "Idx_ExternalIdentifierMapAudit_ValidTo" ON "Iteration_REPLACE"."ExternalIdentifierMap_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExternalIdentifierMap_Audit" SET (autovacuum_analyze_threshold = 2500);

-- File - Audit table
CREATE TABLE "Iteration_REPLACE"."File_Audit" (LIKE "Iteration_REPLACE"."File");
ALTER TABLE "Iteration_REPLACE"."File_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileAudit_ValidFrom" ON "Iteration_REPLACE"."File_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileAudit_ValidTo" ON "Iteration_REPLACE"."File_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."File_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Audit table
CREATE TABLE "Iteration_REPLACE"."FileRevision_Audit" (LIKE "Iteration_REPLACE"."FileRevision");
ALTER TABLE "Iteration_REPLACE"."FileRevision_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileRevisionAudit_ValidFrom" ON "Iteration_REPLACE"."FileRevision_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileRevisionAudit_ValidTo" ON "Iteration_REPLACE"."FileRevision_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileRevision_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Audit table
CREATE TABLE "Iteration_REPLACE"."FileStore_Audit" (LIKE "Iteration_REPLACE"."FileStore");
ALTER TABLE "Iteration_REPLACE"."FileStore_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileStoreAudit_ValidFrom" ON "Iteration_REPLACE"."FileStore_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileStoreAudit_ValidTo" ON "Iteration_REPLACE"."FileStore_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileStore_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileStore_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileStore_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileStore_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Folder - Audit table
CREATE TABLE "Iteration_REPLACE"."Folder_Audit" (LIKE "Iteration_REPLACE"."Folder");
ALTER TABLE "Iteration_REPLACE"."Folder_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FolderAudit_ValidFrom" ON "Iteration_REPLACE"."Folder_Audit" ("ValidFrom");
CREATE INDEX "Idx_FolderAudit_ValidTo" ON "Iteration_REPLACE"."Folder_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Folder_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Folder_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Folder_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Goal - Audit table
CREATE TABLE "Iteration_REPLACE"."Goal_Audit" (LIKE "Iteration_REPLACE"."Goal");
ALTER TABLE "Iteration_REPLACE"."Goal_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GoalAudit_ValidFrom" ON "Iteration_REPLACE"."Goal_Audit" ("ValidFrom");
CREATE INDEX "Idx_GoalAudit_ValidTo" ON "Iteration_REPLACE"."Goal_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Goal_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Audit" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Audit table
CREATE TABLE "Iteration_REPLACE"."HyperLink_Audit" (LIKE "Iteration_REPLACE"."HyperLink");
ALTER TABLE "Iteration_REPLACE"."HyperLink_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_HyperLinkAudit_ValidFrom" ON "Iteration_REPLACE"."HyperLink_Audit" ("ValidFrom");
CREATE INDEX "Idx_HyperLinkAudit_ValidTo" ON "Iteration_REPLACE"."HyperLink_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."HyperLink_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."HyperLink_Audit" SET (autovacuum_analyze_threshold = 2500);

-- IdCorrespondence - Audit table
CREATE TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" (LIKE "Iteration_REPLACE"."IdCorrespondence");
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IdCorrespondenceAudit_ValidFrom" ON "Iteration_REPLACE"."IdCorrespondence_Audit" ("ValidFrom");
CREATE INDEX "Idx_IdCorrespondenceAudit_ValidTo" ON "Iteration_REPLACE"."IdCorrespondence_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."IdCorrespondence_Audit" SET (autovacuum_analyze_threshold = 2500);

-- MultiRelationship - Audit table
CREATE TABLE "Iteration_REPLACE"."MultiRelationship_Audit" (LIKE "Iteration_REPLACE"."MultiRelationship");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationshipAudit_ValidFrom" ON "Iteration_REPLACE"."MultiRelationship_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipAudit_ValidTo" ON "Iteration_REPLACE"."MultiRelationship_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement - Audit table
CREATE TABLE "Iteration_REPLACE"."NestedElement_Audit" (LIKE "Iteration_REPLACE"."NestedElement");
ALTER TABLE "Iteration_REPLACE"."NestedElement_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NestedElementAudit_ValidFrom" ON "Iteration_REPLACE"."NestedElement_Audit" ("ValidFrom");
CREATE INDEX "Idx_NestedElementAudit_ValidTo" ON "Iteration_REPLACE"."NestedElement_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedElement_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NestedParameter - Audit table
CREATE TABLE "Iteration_REPLACE"."NestedParameter_Audit" (LIKE "Iteration_REPLACE"."NestedParameter");
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NestedParameterAudit_ValidFrom" ON "Iteration_REPLACE"."NestedParameter_Audit" ("ValidFrom");
CREATE INDEX "Idx_NestedParameterAudit_ValidTo" ON "Iteration_REPLACE"."NestedParameter_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedParameter_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NotExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."NotExpression_Audit" (LIKE "Iteration_REPLACE"."NotExpression");
ALTER TABLE "Iteration_REPLACE"."NotExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NotExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."NotExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_NotExpressionAudit_ValidTo" ON "Iteration_REPLACE"."NotExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NotExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NotExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Option - Audit table
CREATE TABLE "Iteration_REPLACE"."Option_Audit" (LIKE "Iteration_REPLACE"."Option");
ALTER TABLE "Iteration_REPLACE"."Option_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OptionAudit_ValidFrom" ON "Iteration_REPLACE"."Option_Audit" ("ValidFrom");
CREATE INDEX "Idx_OptionAudit_ValidTo" ON "Iteration_REPLACE"."Option_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Option_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Audit" SET (autovacuum_analyze_threshold = 2500);

-- OrExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."OrExpression_Audit" (LIKE "Iteration_REPLACE"."OrExpression");
ALTER TABLE "Iteration_REPLACE"."OrExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."OrExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrExpressionAudit_ValidTo" ON "Iteration_REPLACE"."OrExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OrExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle - Audit table
CREATE TABLE "Iteration_REPLACE"."OwnedStyle_Audit" (LIKE "Iteration_REPLACE"."OwnedStyle");
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OwnedStyleAudit_ValidFrom" ON "Iteration_REPLACE"."OwnedStyle_Audit" ("ValidFrom");
CREATE INDEX "Idx_OwnedStyleAudit_ValidTo" ON "Iteration_REPLACE"."OwnedStyle_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OwnedStyle_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Parameter - Audit table
CREATE TABLE "Iteration_REPLACE"."Parameter_Audit" (LIKE "Iteration_REPLACE"."Parameter");
ALTER TABLE "Iteration_REPLACE"."Parameter_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterAudit_ValidFrom" ON "Iteration_REPLACE"."Parameter_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterAudit_ValidTo" ON "Iteration_REPLACE"."Parameter_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Parameter_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Parameter_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Parameter_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterBase - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterBase_Audit" (LIKE "Iteration_REPLACE"."ParameterBase");
ALTER TABLE "Iteration_REPLACE"."ParameterBase_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterBaseAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterBase_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterBaseAudit_ValidTo" ON "Iteration_REPLACE"."ParameterBase_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterBase_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterBase_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterBase_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterBase_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterGroup - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterGroup_Audit" (LIKE "Iteration_REPLACE"."ParameterGroup");
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterGroupAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterGroupAudit_ValidTo" ON "Iteration_REPLACE"."ParameterGroup_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOrOverrideBase - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" (LIKE "Iteration_REPLACE"."ParameterOrOverrideBase");
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterOrOverrideBaseAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterOrOverrideBaseAudit_ValidTo" ON "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOrOverrideBase_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverride - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterOverride_Audit" (LIKE "Iteration_REPLACE"."ParameterOverride");
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterOverrideAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterOverride_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterOverrideAudit_ValidTo" ON "Iteration_REPLACE"."ParameterOverride_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverride_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterOverrideValueSet - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" (LIKE "Iteration_REPLACE"."ParameterOverrideValueSet");
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterOverrideValueSetAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterOverrideValueSetAudit_ValidTo" ON "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterOverrideValueSet_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscription - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" (LIKE "Iteration_REPLACE"."ParameterSubscription");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterSubscriptionAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterSubscription_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterSubscriptionAudit_ValidTo" ON "Iteration_REPLACE"."ParameterSubscription_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscription_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterSubscriptionValueSet - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" (LIKE "Iteration_REPLACE"."ParameterSubscriptionValueSet");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterSubscriptionValueSetAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterSubscriptionValueSetAudit_ValidTo" ON "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterSubscriptionValueSet_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValue - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterValue_Audit" (LIKE "Iteration_REPLACE"."ParameterValue");
ALTER TABLE "Iteration_REPLACE"."ParameterValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterValueAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterValueAudit_ValidTo" ON "Iteration_REPLACE"."ParameterValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSet - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" (LIKE "Iteration_REPLACE"."ParameterValueSet");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterValueSetAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterValueSet_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterValueSetAudit_ValidTo" ON "Iteration_REPLACE"."ParameterValueSet_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSet_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParameterValueSetBase - Audit table
CREATE TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" (LIKE "Iteration_REPLACE"."ParameterValueSetBase");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterValueSetBaseAudit_ValidFrom" ON "Iteration_REPLACE"."ParameterValueSetBase_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterValueSetBaseAudit_ValidTo" ON "Iteration_REPLACE"."ParameterValueSetBase_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParameterValueSetBase_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ParametricConstraint - Audit table
CREATE TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" (LIKE "Iteration_REPLACE"."ParametricConstraint");
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParametricConstraintAudit_ValidFrom" ON "Iteration_REPLACE"."ParametricConstraint_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParametricConstraintAudit_ValidTo" ON "Iteration_REPLACE"."ParametricConstraint_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ParametricConstraint_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Point - Audit table
CREATE TABLE "Iteration_REPLACE"."Point_Audit" (LIKE "Iteration_REPLACE"."Point");
ALTER TABLE "Iteration_REPLACE"."Point_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PointAudit_ValidFrom" ON "Iteration_REPLACE"."Point_Audit" ("ValidFrom");
CREATE INDEX "Idx_PointAudit_ValidTo" ON "Iteration_REPLACE"."Point_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Point_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Point_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Point_Audit" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteState - Audit table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" (LIKE "Iteration_REPLACE"."PossibleFiniteState");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PossibleFiniteStateAudit_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteState_Audit" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteStateAudit_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteState_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteState_Audit" SET (autovacuum_analyze_threshold = 2500);

-- PossibleFiniteStateList - Audit table
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" (LIKE "Iteration_REPLACE"."PossibleFiniteStateList");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PossibleFiniteStateListAudit_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteStateList_Audit" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteStateListAudit_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteStateList_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Publication - Audit table
CREATE TABLE "Iteration_REPLACE"."Publication_Audit" (LIKE "Iteration_REPLACE"."Publication");
ALTER TABLE "Iteration_REPLACE"."Publication_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PublicationAudit_ValidFrom" ON "Iteration_REPLACE"."Publication_Audit" ("ValidFrom");
CREATE INDEX "Idx_PublicationAudit_ValidTo" ON "Iteration_REPLACE"."Publication_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression - Audit table
CREATE TABLE "Iteration_REPLACE"."RelationalExpression_Audit" (LIKE "Iteration_REPLACE"."RelationalExpression");
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RelationalExpressionAudit_ValidFrom" ON "Iteration_REPLACE"."RelationalExpression_Audit" ("ValidFrom");
CREATE INDEX "Idx_RelationalExpressionAudit_ValidTo" ON "Iteration_REPLACE"."RelationalExpression_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationalExpression_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Relationship - Audit table
CREATE TABLE "Iteration_REPLACE"."Relationship_Audit" (LIKE "Iteration_REPLACE"."Relationship");
ALTER TABLE "Iteration_REPLACE"."Relationship_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RelationshipAudit_ValidFrom" ON "Iteration_REPLACE"."Relationship_Audit" ("ValidFrom");
CREATE INDEX "Idx_RelationshipAudit_ValidTo" ON "Iteration_REPLACE"."Relationship_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Relationship_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Relationship_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RelationshipParameterValue - Audit table
CREATE TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" (LIKE "Iteration_REPLACE"."RelationshipParameterValue");
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RelationshipParameterValueAudit_ValidFrom" ON "Iteration_REPLACE"."RelationshipParameterValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_RelationshipParameterValueAudit_ValidTo" ON "Iteration_REPLACE"."RelationshipParameterValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RelationshipParameterValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Requirement - Audit table
CREATE TABLE "Iteration_REPLACE"."Requirement_Audit" (LIKE "Iteration_REPLACE"."Requirement");
ALTER TABLE "Iteration_REPLACE"."Requirement_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementAudit_ValidFrom" ON "Iteration_REPLACE"."Requirement_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementAudit_ValidTo" ON "Iteration_REPLACE"."Requirement_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Requirement_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer - Audit table
CREATE TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" (LIKE "Iteration_REPLACE"."RequirementsContainer");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementsContainerAudit_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainer_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainerAudit_ValidTo" ON "Iteration_REPLACE"."RequirementsContainer_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainerParameterValue - Audit table
CREATE TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" (LIKE "Iteration_REPLACE"."RequirementsContainerParameterValue");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementsContainerParameterValueAudit_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainerParameterValueAudit_ValidTo" ON "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainerParameterValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsGroup - Audit table
CREATE TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" (LIKE "Iteration_REPLACE"."RequirementsGroup");
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementsGroupAudit_ValidFrom" ON "Iteration_REPLACE"."RequirementsGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementsGroupAudit_ValidTo" ON "Iteration_REPLACE"."RequirementsGroup_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsSpecification - Audit table
CREATE TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" (LIKE "Iteration_REPLACE"."RequirementsSpecification");
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementsSpecificationAudit_ValidFrom" ON "Iteration_REPLACE"."RequirementsSpecification_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementsSpecificationAudit_ValidTo" ON "Iteration_REPLACE"."RequirementsSpecification_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsSpecification_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerification - Audit table
CREATE TABLE "Iteration_REPLACE"."RuleVerification_Audit" (LIKE "Iteration_REPLACE"."RuleVerification");
ALTER TABLE "Iteration_REPLACE"."RuleVerification_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleVerificationAudit_ValidFrom" ON "Iteration_REPLACE"."RuleVerification_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleVerificationAudit_ValidTo" ON "Iteration_REPLACE"."RuleVerification_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleVerification_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerification_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerification_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerification_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RuleVerificationList - Audit table
CREATE TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" (LIKE "Iteration_REPLACE"."RuleVerificationList");
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleVerificationListAudit_ValidFrom" ON "Iteration_REPLACE"."RuleVerificationList_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleVerificationListAudit_ValidTo" ON "Iteration_REPLACE"."RuleVerificationList_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleVerificationList_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RuleViolation - Audit table
CREATE TABLE "Iteration_REPLACE"."RuleViolation_Audit" (LIKE "Iteration_REPLACE"."RuleViolation");
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleViolationAudit_ValidFrom" ON "Iteration_REPLACE"."RuleViolation_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleViolationAudit_ValidTo" ON "Iteration_REPLACE"."RuleViolation_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle - Audit table
CREATE TABLE "Iteration_REPLACE"."SharedStyle_Audit" (LIKE "Iteration_REPLACE"."SharedStyle");
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SharedStyleAudit_ValidFrom" ON "Iteration_REPLACE"."SharedStyle_Audit" ("ValidFrom");
CREATE INDEX "Idx_SharedStyleAudit_ValidTo" ON "Iteration_REPLACE"."SharedStyle_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SharedStyle_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterizableThing - Audit table
CREATE TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" (LIKE "Iteration_REPLACE"."SimpleParameterizableThing");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleParameterizableThingAudit_ValidFrom" ON "Iteration_REPLACE"."SimpleParameterizableThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleParameterizableThingAudit_ValidTo" ON "Iteration_REPLACE"."SimpleParameterizableThing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterizableThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SimpleParameterValue - Audit table
CREATE TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" (LIKE "Iteration_REPLACE"."SimpleParameterValue");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleParameterValueAudit_ValidFrom" ON "Iteration_REPLACE"."SimpleParameterValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleParameterValueAudit_ValidTo" ON "Iteration_REPLACE"."SimpleParameterValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."SimpleParameterValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder - Audit table
CREATE TABLE "Iteration_REPLACE"."Stakeholder_Audit" (LIKE "Iteration_REPLACE"."Stakeholder");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeholderAudit_ValidFrom" ON "Iteration_REPLACE"."Stakeholder_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeholderAudit_ValidTo" ON "Iteration_REPLACE"."Stakeholder_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue - Audit table
CREATE TABLE "Iteration_REPLACE"."StakeholderValue_Audit" (LIKE "Iteration_REPLACE"."StakeholderValue");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeholderValueAudit_ValidFrom" ON "Iteration_REPLACE"."StakeholderValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeholderValueAudit_ValidTo" ON "Iteration_REPLACE"."StakeholderValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap - Audit table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMapAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMapAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings - Audit table
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMapSettings");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMapSettingsAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMapSettingsAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMapSettings_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing - Audit table
CREATE TABLE "Iteration_REPLACE"."Thing_Audit" (LIKE "Iteration_REPLACE"."Thing");
ALTER TABLE "Iteration_REPLACE"."Thing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingAudit_ValidFrom" ON "Iteration_REPLACE"."Thing_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingAudit_ValidTo" ON "Iteration_REPLACE"."Thing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- UserRuleVerification - Audit table
CREATE TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" (LIKE "Iteration_REPLACE"."UserRuleVerification");
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UserRuleVerificationAudit_ValidFrom" ON "Iteration_REPLACE"."UserRuleVerification_Audit" ("ValidFrom");
CREATE INDEX "Idx_UserRuleVerificationAudit_ValidTo" ON "Iteration_REPLACE"."UserRuleVerification_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."UserRuleVerification_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ValueGroup - Audit table
CREATE TABLE "Iteration_REPLACE"."ValueGroup_Audit" (LIKE "Iteration_REPLACE"."ValueGroup");
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ValueGroupAudit_ValidFrom" ON "Iteration_REPLACE"."ValueGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_ValueGroupAudit_ValidTo" ON "Iteration_REPLACE"."ValueGroup_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
------------------------------- Iteration Audit Reference Or Ordered Properties ------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Audit table Reference properties
-- ActualFiniteState.PossibleState is a collection property (many to many) of class PossibleFiniteState: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" (LIKE "Iteration_REPLACE"."ActualFiniteState_PossibleState");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActualFiniteState_PossibleStateAudit_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteState_PossibleStateAudit_ValidTo" ON "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteState_PossibleState_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList - Audit table Reference properties
-- ActualFiniteStateList.ExcludeOption is a collection property (many to many) of class Option: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" (LIKE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActualFiniteStateList_ExcludeOptionAudit_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateList_ExcludeOptionAudit_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ActualFiniteStateList
-- ActualFiniteStateList.PossibleFiniteStateList is an ordered collection property (many to many) of class PossibleFiniteStateList: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" (LIKE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ActualFiniteStateList_PossibleFiniteStateListAudit_ValidFrom" ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" ("ValidFrom");
CREATE INDEX "Idx_ActualFiniteStateList_PossibleFiniteStateListAudit_ValidTo" ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Alias - Audit table Reference properties

-- AndExpression - Audit table Reference properties
-- AndExpression.Term is a collection property (many to many) of class BooleanExpression: [2..*]-[0..1]
CREATE TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" (LIKE "Iteration_REPLACE"."AndExpression_Term");
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AndExpression_TermAudit_ValidFrom" ON "Iteration_REPLACE"."AndExpression_Term_Audit" ("ValidFrom");
CREATE INDEX "Idx_AndExpression_TermAudit_ValidTo" ON "Iteration_REPLACE"."AndExpression_Term_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."AndExpression_Term_Audit" SET (autovacuum_analyze_threshold = 2500);

-- BinaryRelationship - Audit table Reference properties

-- BooleanExpression - Audit table Reference properties

-- Bounds - Audit table Reference properties

-- BuiltInRuleVerification - Audit table Reference properties

-- Citation - Audit table Reference properties

-- Color - Audit table Reference properties

-- DefinedThing - Audit table Reference properties

-- Definition - Audit table Reference properties
-- Definition.Example is an ordered collection property of type String: [0..*]
CREATE TABLE "Iteration_REPLACE"."Definition_Example_Audit" (LIKE "Iteration_REPLACE"."Definition_Example");
ALTER TABLE "Iteration_REPLACE"."Definition_Example_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_ExampleAudit_ValidFrom" ON "Iteration_REPLACE"."Definition_Example_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_ExampleAudit_ValidTo" ON "Iteration_REPLACE"."Definition_Example_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition_Example_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Example_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Example_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Example_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Definition
-- Definition.Note is an ordered collection property of type String: [0..*]
CREATE TABLE "Iteration_REPLACE"."Definition_Note_Audit" (LIKE "Iteration_REPLACE"."Definition_Note");
ALTER TABLE "Iteration_REPLACE"."Definition_Note_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_NoteAudit_ValidFrom" ON "Iteration_REPLACE"."Definition_Note_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_NoteAudit_ValidTo" ON "Iteration_REPLACE"."Definition_Note_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Definition_Note_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Note_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Definition_Note_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Definition_Note_Audit" SET (autovacuum_analyze_threshold = 2500);

-- DiagramCanvas - Audit table Reference properties

-- DiagramEdge - Audit table Reference properties

-- DiagramElementContainer - Audit table Reference properties

-- DiagramElementThing - Audit table Reference properties

-- DiagrammingStyle - Audit table Reference properties

-- DiagramObject - Audit table Reference properties

-- DiagramShape - Audit table Reference properties

-- DiagramThingBase - Audit table Reference properties

-- DomainFileStore - Audit table Reference properties

-- ElementBase - Audit table Reference properties
-- ElementBase.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" (LIKE "Iteration_REPLACE"."ElementBase_Category");
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementBase_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."ElementBase_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementBase_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."ElementBase_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementBase_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ElementDefinition - Audit table Reference properties
-- ElementDefinition.ReferencedElement is a collection property (many to many) of class NestedElement: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" (LIKE "Iteration_REPLACE"."ElementDefinition_ReferencedElement");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementDefinition_ReferencedElementAudit_ValidFrom" ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_ReferencedElementAudit_ValidTo" ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementDefinition_ReferencedElement_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ElementUsage - Audit table Reference properties
-- ElementUsage.ExcludeOption is a collection property (many to many) of class Option: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" (LIKE "Iteration_REPLACE"."ElementUsage_ExcludeOption");
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementUsage_ExcludeOptionAudit_ValidFrom" ON "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementUsage_ExcludeOptionAudit_ValidTo" ON "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ElementUsage_ExcludeOption_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ExclusiveOrExpression - Audit table Reference properties
-- ExclusiveOrExpression.Term is a collection property (many to many) of class BooleanExpression: [2..2]-[0..1]
CREATE TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" (LIKE "Iteration_REPLACE"."ExclusiveOrExpression_Term");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ExclusiveOrExpression_TermAudit_ValidFrom" ON "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" ("ValidFrom");
CREATE INDEX "Idx_ExclusiveOrExpression_TermAudit_ValidTo" ON "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ExclusiveOrExpression_Term_Audit" SET (autovacuum_analyze_threshold = 2500);

-- ExternalIdentifierMap - Audit table Reference properties

-- File - Audit table Reference properties
-- File.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."File_Category_Audit" (LIKE "Iteration_REPLACE"."File_Category");
ALTER TABLE "Iteration_REPLACE"."File_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_File_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."File_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_File_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."File_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."File_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."File_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."File_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileRevision - Audit table Reference properties
-- FileRevision.FileType is an ordered collection property (many to many) of class FileType: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" (LIKE "Iteration_REPLACE"."FileRevision_FileType");
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileRevision_FileTypeAudit_ValidFrom" ON "Iteration_REPLACE"."FileRevision_FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileRevision_FileTypeAudit_ValidTo" ON "Iteration_REPLACE"."FileRevision_FileType_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."FileRevision_FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

-- FileStore - Audit table Reference properties

-- Folder - Audit table Reference properties

-- Goal - Audit table Reference properties
-- Goal.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Goal_Category_Audit" (LIKE "Iteration_REPLACE"."Goal_Category");
ALTER TABLE "Iteration_REPLACE"."Goal_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Goal_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."Goal_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Goal_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."Goal_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Goal_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Goal_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Goal_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- HyperLink - Audit table Reference properties

-- IdCorrespondence - Audit table Reference properties

-- MultiRelationship - Audit table Reference properties
-- MultiRelationship.RelatedThing is a collection property (many to many) of class Thing: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" (LIKE "Iteration_REPLACE"."MultiRelationship_RelatedThing");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationship_RelatedThingAudit_ValidFrom" ON "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationship_RelatedThingAudit_ValidTo" ON "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."MultiRelationship_RelatedThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NestedElement - Audit table Reference properties
-- NestedElement.ElementUsage is an ordered collection property (many to many) of class ElementUsage: [1..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" (LIKE "Iteration_REPLACE"."NestedElement_ElementUsage");
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NestedElement_ElementUsageAudit_ValidFrom" ON "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" ("ValidFrom");
CREATE INDEX "Idx_NestedElement_ElementUsageAudit_ValidTo" ON "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."NestedElement_ElementUsage_Audit" SET (autovacuum_analyze_threshold = 2500);

-- NestedParameter - Audit table Reference properties

-- NotExpression - Audit table Reference properties

-- Option - Audit table Reference properties
-- Option.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Option_Category_Audit" (LIKE "Iteration_REPLACE"."Option_Category");
ALTER TABLE "Iteration_REPLACE"."Option_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Option_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."Option_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Option_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."Option_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Option_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Option_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Option_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- OrExpression - Audit table Reference properties
-- OrExpression.Term is a collection property (many to many) of class BooleanExpression: [2..*]-[0..1]
CREATE TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" (LIKE "Iteration_REPLACE"."OrExpression_Term");
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrExpression_TermAudit_ValidFrom" ON "Iteration_REPLACE"."OrExpression_Term_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrExpression_TermAudit_ValidTo" ON "Iteration_REPLACE"."OrExpression_Term_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."OrExpression_Term_Audit" SET (autovacuum_analyze_threshold = 2500);

-- OwnedStyle - Audit table Reference properties

-- Parameter - Audit table Reference properties

-- ParameterBase - Audit table Reference properties

-- ParameterGroup - Audit table Reference properties

-- ParameterOrOverrideBase - Audit table Reference properties

-- ParameterOverride - Audit table Reference properties

-- ParameterOverrideValueSet - Audit table Reference properties

-- ParameterSubscription - Audit table Reference properties

-- ParameterSubscriptionValueSet - Audit table Reference properties

-- ParameterValue - Audit table Reference properties

-- ParameterValueSet - Audit table Reference properties

-- ParameterValueSetBase - Audit table Reference properties

-- ParametricConstraint - Audit table Reference properties

-- Point - Audit table Reference properties

-- PossibleFiniteState - Audit table Reference properties

-- PossibleFiniteStateList - Audit table Reference properties
-- PossibleFiniteStateList.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" (LIKE "Iteration_REPLACE"."PossibleFiniteStateList_Category");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PossibleFiniteStateList_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_PossibleFiniteStateList_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."PossibleFiniteStateList_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Publication - Audit table Reference properties
-- Publication.Domain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Publication_Domain_Audit" (LIKE "Iteration_REPLACE"."Publication_Domain");
ALTER TABLE "Iteration_REPLACE"."Publication_Domain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Publication_DomainAudit_ValidFrom" ON "Iteration_REPLACE"."Publication_Domain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Publication_DomainAudit_ValidTo" ON "Iteration_REPLACE"."Publication_Domain_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication_Domain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_Domain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Publication
-- Publication.PublishedParameter is a collection property (many to many) of class ParameterOrOverrideBase: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" (LIKE "Iteration_REPLACE"."Publication_PublishedParameter");
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Publication_PublishedParameterAudit_ValidFrom" ON "Iteration_REPLACE"."Publication_PublishedParameter_Audit" ("ValidFrom");
CREATE INDEX "Idx_Publication_PublishedParameterAudit_ValidTo" ON "Iteration_REPLACE"."Publication_PublishedParameter_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Publication_PublishedParameter_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RelationalExpression - Audit table Reference properties

-- Relationship - Audit table Reference properties
-- Relationship.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Relationship_Category_Audit" (LIKE "Iteration_REPLACE"."Relationship_Category");
ALTER TABLE "Iteration_REPLACE"."Relationship_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Relationship_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."Relationship_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Relationship_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."Relationship_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Relationship_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Relationship_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RelationshipParameterValue - Audit table Reference properties

-- Requirement - Audit table Reference properties
-- Requirement.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Requirement_Category_Audit" (LIKE "Iteration_REPLACE"."Requirement_Category");
ALTER TABLE "Iteration_REPLACE"."Requirement_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Requirement_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."Requirement_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Requirement_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."Requirement_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Requirement_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Requirement_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainer - Audit table Reference properties
-- RequirementsContainer.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" (LIKE "Iteration_REPLACE"."RequirementsContainer_Category");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RequirementsContainer_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."RequirementsContainer_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_RequirementsContainer_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."RequirementsContainer_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RequirementsContainer_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- RequirementsContainerParameterValue - Audit table Reference properties

-- RequirementsGroup - Audit table Reference properties

-- RequirementsSpecification - Audit table Reference properties

-- RuleVerification - Audit table Reference properties

-- RuleVerificationList - Audit table Reference properties

-- RuleViolation - Audit table Reference properties
-- RuleViolation.ViolatingThing is a collection property of type Guid: [0..*]
CREATE TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" (LIKE "Iteration_REPLACE"."RuleViolation_ViolatingThing");
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleViolation_ViolatingThingAudit_ValidFrom" ON "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleViolation_ViolatingThingAudit_ValidTo" ON "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."RuleViolation_ViolatingThing_Audit" SET (autovacuum_analyze_threshold = 2500);

-- SharedStyle - Audit table Reference properties

-- SimpleParameterizableThing - Audit table Reference properties

-- SimpleParameterValue - Audit table Reference properties

-- Stakeholder - Audit table Reference properties
-- Stakeholder.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" (LIKE "Iteration_REPLACE"."Stakeholder_Category");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Stakeholder_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."Stakeholder_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Stakeholder_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."Stakeholder_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Stakeholder
-- Stakeholder.StakeholderValue is a collection property (many to many) of class StakeholderValue: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" (LIKE "Iteration_REPLACE"."Stakeholder_StakeholderValue");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Stakeholder_StakeholderValueAudit_ValidFrom" ON "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_Stakeholder_StakeholderValueAudit_ValidTo" ON "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Stakeholder_StakeholderValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeholderValue - Audit table Reference properties
-- StakeholderValue.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" (LIKE "Iteration_REPLACE"."StakeholderValue_Category");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeholderValue_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."StakeholderValue_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeholderValue_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."StakeholderValue_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeholderValue_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap - Audit table Reference properties
-- StakeHolderValueMap.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap_Category");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMap_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.Goal is a collection property (many to many) of class Goal: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap_Goal");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMap_GoalAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_GoalAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Goal_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.Requirement is a collection property (many to many) of class Requirement: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap_Requirement");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMap_RequirementAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_RequirementAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_Requirement_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.StakeholderValue is a collection property (many to many) of class StakeholderValue: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMap_StakeholderValueAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_StakeholderValueAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMap
-- StakeHolderValueMap.ValueGroup is a collection property (many to many) of class ValueGroup: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" (LIKE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_StakeHolderValueMap_ValueGroupAudit_ValidFrom" ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_StakeHolderValueMap_ValueGroupAudit_ValidTo" ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

-- StakeHolderValueMapSettings - Audit table Reference properties

-- Thing - Audit table Reference properties
-- Thing.ExcludedDomain is a collection property (many to many) of class DomainOfExpertise: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" (LIKE "Iteration_REPLACE"."Thing_ExcludedDomain");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidFrom" ON "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidTo" ON "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

-- Thing
-- Thing.ExcludedPerson is a collection property (many to many) of class Person: [0..*]-[1..1]
CREATE TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" (LIKE "Iteration_REPLACE"."Thing_ExcludedPerson");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidFrom" ON "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidTo" ON "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_threshold = 2500);

-- UserRuleVerification - Audit table Reference properties

-- ValueGroup - Audit table Reference properties
-- ValueGroup.Category is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" (LIKE "Iteration_REPLACE"."ValueGroup_Category");
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ValueGroup_CategoryAudit_ValidFrom" ON "Iteration_REPLACE"."ValueGroup_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ValueGroup_CategoryAudit_ValidTo" ON "Iteration_REPLACE"."ValueGroup_Category_Audit" ("ValidTo");
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "Iteration_REPLACE"."ValueGroup_Category_Audit" SET (autovacuum_analyze_threshold = 2500);
