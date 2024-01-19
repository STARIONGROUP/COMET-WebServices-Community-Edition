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
--------------------- EngineeringModel Classes - revision trigger (apply_revision) ---------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Class Revision Trigger

-- Approval - Class Revision Trigger
CREATE TRIGGER approval_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Approval"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- BinaryNote - Class Revision Trigger

-- Book - Class Revision Trigger
CREATE TRIGGER book_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Book"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ChangeProposal - Class Revision Trigger

-- ChangeRequest - Class Revision Trigger

-- CommonFileStore - Class Revision Trigger
CREATE TRIGGER commonfilestore_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."CommonFileStore"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ContractChangeNotice - Class Revision Trigger

-- ContractDeviation - Class Revision Trigger

-- DiscussionItem - Class Revision Trigger

-- EngineeringModel - Class Revision Trigger

-- EngineeringModelDataAnnotation - Class Revision Trigger

-- EngineeringModelDataDiscussionItem - Class Revision Trigger
CREATE TRIGGER engineeringmodeldatadiscussionitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- EngineeringModelDataNote - Class Revision Trigger
CREATE TRIGGER engineeringmodeldatanote_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."EngineeringModelDataNote"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- File - Class Revision Trigger
CREATE TRIGGER file_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."File"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- FileRevision - Class Revision Trigger
CREATE TRIGGER filerevision_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."FileRevision"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- FileStore - Class Revision Trigger

-- Folder - Class Revision Trigger
CREATE TRIGGER folder_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Folder"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- GenericAnnotation - Class Revision Trigger

-- Iteration - Class Revision Trigger
CREATE TRIGGER iteration_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Iteration"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- LogEntryChangelogItem - Class Revision Trigger
CREATE TRIGGER logentrychangelogitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."LogEntryChangelogItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ModellingAnnotationItem - Class Revision Trigger
CREATE TRIGGER modellingannotationitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModellingAnnotationItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ModellingThingReference - Class Revision Trigger
CREATE TRIGGER modellingthingreference_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModellingThingReference"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ModelLogEntry - Class Revision Trigger
CREATE TRIGGER modellogentry_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModelLogEntry"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- Note - Class Revision Trigger
CREATE TRIGGER note_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Note"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- Page - Class Revision Trigger
CREATE TRIGGER page_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Page"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- RequestForDeviation - Class Revision Trigger

-- RequestForWaiver - Class Revision Trigger

-- ReviewItemDiscrepancy - Class Revision Trigger

-- Section - Class Revision Trigger
CREATE TRIGGER section_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Section"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- Solution - Class Revision Trigger
CREATE TRIGGER solution_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Solution"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- TextualNote - Class Revision Trigger

-- Thing - Class Revision Trigger
CREATE TRIGGER thing_apply_revision
  BEFORE INSERT 
  ON "EngineeringModel_REPLACE"."Thing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Iid', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ThingReference - Class Revision Trigger

-- TopContainer - Class Revision Trigger

--------------------------------------------------------------------------------------------------------
-------------- EngineeringModel Reference Properties - Revision Trigger (apply_revision) ---------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Reference Property Revision Trigger

-- Approval - Reference Property Revision Trigger

-- BinaryNote - Reference Property Revision Trigger

-- Book - Reference Property Revision Trigger
-- Book.Category
CREATE TRIGGER book_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Book_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Book', 'EngineeringModel_REPLACE');

-- ChangeProposal - Reference Property Revision Trigger

-- ChangeRequest - Reference Property Revision Trigger

-- CommonFileStore - Reference Property Revision Trigger

-- ContractChangeNotice - Reference Property Revision Trigger

-- ContractDeviation - Reference Property Revision Trigger

-- DiscussionItem - Reference Property Revision Trigger

-- EngineeringModel - Reference Property Revision Trigger

-- EngineeringModelDataAnnotation - Reference Property Revision Trigger

-- EngineeringModelDataDiscussionItem - Reference Property Revision Trigger

-- EngineeringModelDataNote - Reference Property Revision Trigger

-- File - Reference Property Revision Trigger
-- File.Category
CREATE TRIGGER file_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."File_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('File', 'EngineeringModel_REPLACE');

-- FileRevision - Reference Property Revision Trigger
-- FileRevision.FileType
CREATE TRIGGER filerevision_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."FileRevision_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('FileRevision', 'EngineeringModel_REPLACE');

-- FileStore - Reference Property Revision Trigger

-- Folder - Reference Property Revision Trigger

-- GenericAnnotation - Reference Property Revision Trigger

-- Iteration - Reference Property Revision Trigger

-- LogEntryChangelogItem - Reference Property Revision Trigger
-- LogEntryChangelogItem.AffectedReferenceIid
CREATE TRIGGER logentrychangelogitem_affectedreferenceiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('LogEntryChangelogItem', 'EngineeringModel_REPLACE');

-- ModellingAnnotationItem - Reference Property Revision Trigger
-- ModellingAnnotationItem.Category
CREATE TRIGGER modellingannotationitem_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModellingAnnotationItem', 'EngineeringModel_REPLACE');

-- ModellingAnnotationItem.SourceAnnotation
CREATE TRIGGER modellingannotationitem_sourceannotation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModellingAnnotationItem', 'EngineeringModel_REPLACE');

-- ModellingThingReference - Reference Property Revision Trigger

-- ModelLogEntry - Reference Property Revision Trigger
-- ModelLogEntry.AffectedDomainIid
CREATE TRIGGER modellogentry_affecteddomainiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModelLogEntry', 'EngineeringModel_REPLACE');

-- ModelLogEntry.AffectedItemIid
CREATE TRIGGER modellogentry_affecteditemiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModelLogEntry', 'EngineeringModel_REPLACE');

-- ModelLogEntry.Category
CREATE TRIGGER modellogentry_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."ModelLogEntry_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModelLogEntry', 'EngineeringModel_REPLACE');

-- Note - Reference Property Revision Trigger
-- Note.Category
CREATE TRIGGER note_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Note_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Note', 'EngineeringModel_REPLACE');

-- Page - Reference Property Revision Trigger
-- Page.Category
CREATE TRIGGER page_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Page_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Page', 'EngineeringModel_REPLACE');

-- RequestForDeviation - Reference Property Revision Trigger

-- RequestForWaiver - Reference Property Revision Trigger

-- ReviewItemDiscrepancy - Reference Property Revision Trigger

-- Section - Reference Property Revision Trigger
-- Section.Category
CREATE TRIGGER section_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Section_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Section', 'EngineeringModel_REPLACE');

-- Solution - Reference Property Revision Trigger

-- TextualNote - Reference Property Revision Trigger

-- Thing - Reference Property Revision Trigger
-- Thing.ExcludedDomain
CREATE TRIGGER thing_excludeddomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'EngineeringModel_REPLACE');

-- Thing.ExcludedPerson
CREATE TRIGGER thing_excludedperson_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'EngineeringModel_REPLACE');

-- ThingReference - Reference Property Revision Trigger

-- TopContainer - Reference Property Revision Trigger

--------------------------------------------------------------------------------------------------------
-------------------------------- EngineeringModel Class Audit triggers ---------------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Class Audit Triggers
CREATE TRIGGER ActionItem_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ActionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ActionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Approval - Class Audit Triggers
CREATE TRIGGER Approval_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Approval"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Approval_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Approval"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BinaryNote - Class Audit Triggers
CREATE TRIGGER BinaryNote_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."BinaryNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BinaryNote_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."BinaryNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Book - Class Audit Triggers
CREATE TRIGGER Book_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Book"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Book_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Book"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ChangeProposal - Class Audit Triggers
CREATE TRIGGER ChangeProposal_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ChangeProposal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ChangeProposal_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ChangeProposal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ChangeRequest - Class Audit Triggers
CREATE TRIGGER ChangeRequest_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ChangeRequest"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ChangeRequest_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ChangeRequest"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- CommonFileStore - Class Audit Triggers
CREATE TRIGGER CommonFileStore_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."CommonFileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER CommonFileStore_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."CommonFileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ContractChangeNotice - Class Audit Triggers
CREATE TRIGGER ContractChangeNotice_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ContractChangeNotice"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ContractChangeNotice_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ContractChangeNotice"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ContractDeviation - Class Audit Triggers
CREATE TRIGGER ContractDeviation_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ContractDeviation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ContractDeviation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ContractDeviation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiscussionItem - Class Audit Triggers
CREATE TRIGGER DiscussionItem_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EngineeringModel - Class Audit Triggers
CREATE TRIGGER EngineeringModel_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."EngineeringModel"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModel_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."EngineeringModel"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EngineeringModelDataAnnotation - Class Audit Triggers
CREATE TRIGGER EngineeringModelDataAnnotation_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelDataAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."EngineeringModelDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EngineeringModelDataDiscussionItem - Class Audit Triggers
CREATE TRIGGER EngineeringModelDataDiscussionItem_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelDataDiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."EngineeringModelDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- EngineeringModelDataNote - Class Audit Triggers
CREATE TRIGGER EngineeringModelDataNote_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."EngineeringModelDataNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelDataNote_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."EngineeringModelDataNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- File - Class Audit Triggers
CREATE TRIGGER File_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."File"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER File_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."File"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileRevision - Class Audit Triggers
CREATE TRIGGER FileRevision_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."FileRevision"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileRevision_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."FileRevision"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileStore - Class Audit Triggers
CREATE TRIGGER FileStore_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."FileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileStore_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."FileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Folder - Class Audit Triggers
CREATE TRIGGER Folder_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Folder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Folder_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Folder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- GenericAnnotation - Class Audit Triggers
CREATE TRIGGER GenericAnnotation_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER GenericAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Iteration - Class Audit Triggers
CREATE TRIGGER Iteration_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Iteration"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Iteration_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Iteration"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- LogEntryChangelogItem - Class Audit Triggers
CREATE TRIGGER LogEntryChangelogItem_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModellingAnnotationItem - Class Audit Triggers
CREATE TRIGGER ModellingAnnotationItem_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModellingAnnotationItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModellingThingReference - Class Audit Triggers
CREATE TRIGGER ModellingThingReference_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModellingThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModellingThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModellingThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModelLogEntry - Class Audit Triggers
CREATE TRIGGER ModelLogEntry_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModelLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelLogEntry_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModelLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Note - Class Audit Triggers
CREATE TRIGGER Note_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Note_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Page - Class Audit Triggers
CREATE TRIGGER Page_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Page"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Page_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Page"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequestForDeviation - Class Audit Triggers
CREATE TRIGGER RequestForDeviation_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."RequestForDeviation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequestForDeviation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."RequestForDeviation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequestForWaiver - Class Audit Triggers
CREATE TRIGGER RequestForWaiver_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."RequestForWaiver"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequestForWaiver_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."RequestForWaiver"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ReviewItemDiscrepancy - Class Audit Triggers
CREATE TRIGGER ReviewItemDiscrepancy_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReviewItemDiscrepancy_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ReviewItemDiscrepancy"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Section - Class Audit Triggers
CREATE TRIGGER Section_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Section"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Section_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Section"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Solution - Class Audit Triggers
CREATE TRIGGER Solution_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Solution"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Solution_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Solution"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TextualNote - Class Audit Triggers
CREATE TRIGGER TextualNote_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."TextualNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TextualNote_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."TextualNote"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing - Class Audit Triggers
CREATE TRIGGER Thing_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ThingReference - Class Audit Triggers
CREATE TRIGGER ThingReference_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- TopContainer - Class Audit Triggers
CREATE TRIGGER TopContainer_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TopContainer_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

--------------------------------------------------------------------------------------------------------
------------------------- EngineeringModel  Reference Property Audit triggers --------------------------
--------------------------------------------------------------------------------------------------------

-- ActionItem - Reference Property Audit Triggers

-- Approval - Reference Property Audit Triggers

-- BinaryNote - Reference Property Audit Triggers

-- Book - Reference Property Audit Triggers
-- Book.Category
CREATE TRIGGER Book_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Book_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Book_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Book_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ChangeProposal - Reference Property Audit Triggers

-- ChangeRequest - Reference Property Audit Triggers

-- CommonFileStore - Reference Property Audit Triggers

-- ContractChangeNotice - Reference Property Audit Triggers

-- ContractDeviation - Reference Property Audit Triggers

-- DiscussionItem - Reference Property Audit Triggers

-- EngineeringModel - Reference Property Audit Triggers

-- EngineeringModelDataAnnotation - Reference Property Audit Triggers

-- EngineeringModelDataDiscussionItem - Reference Property Audit Triggers

-- EngineeringModelDataNote - Reference Property Audit Triggers

-- File - Reference Property Audit Triggers
-- File.Category
CREATE TRIGGER File_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."File_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER File_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."File_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileRevision - Reference Property Audit Triggers
-- FileRevision.FileType
CREATE TRIGGER FileRevision_FileType_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."FileRevision_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileRevision_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."FileRevision_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileStore - Reference Property Audit Triggers

-- Folder - Reference Property Audit Triggers

-- GenericAnnotation - Reference Property Audit Triggers

-- Iteration - Reference Property Audit Triggers

-- LogEntryChangelogItem - Reference Property Audit Triggers
-- LogEntryChangelogItem.AffectedReferenceIid
CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModellingAnnotationItem - Reference Property Audit Triggers
-- ModellingAnnotationItem.Category
CREATE TRIGGER ModellingAnnotationItem_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModellingAnnotationItem_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModellingAnnotationItem.SourceAnnotation
CREATE TRIGGER ModellingAnnotationItem_SourceAnnotation_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModellingAnnotationItem_SourceAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModellingAnnotationItem_SourceAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModellingThingReference - Reference Property Audit Triggers

-- ModelLogEntry - Reference Property Audit Triggers
-- ModelLogEntry.AffectedDomainIid
CREATE TRIGGER ModelLogEntry_AffectedDomainIid_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelLogEntry_AffectedDomainIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModelLogEntry.AffectedItemIid
CREATE TRIGGER ModelLogEntry_AffectedItemIid_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelLogEntry_AffectedItemIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModelLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ModelLogEntry.Category
CREATE TRIGGER ModelLogEntry_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."ModelLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelLogEntry_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."ModelLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Note - Reference Property Audit Triggers
-- Note.Category
CREATE TRIGGER Note_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Note_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Note_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Note_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Page - Reference Property Audit Triggers
-- Page.Category
CREATE TRIGGER Page_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Page_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Page_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Page_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequestForDeviation - Reference Property Audit Triggers

-- RequestForWaiver - Reference Property Audit Triggers

-- ReviewItemDiscrepancy - Reference Property Audit Triggers

-- Section - Reference Property Audit Triggers
-- Section.Category
CREATE TRIGGER Section_Category_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Section_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Section_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Section_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Solution - Reference Property Audit Triggers

-- TextualNote - Reference Property Audit Triggers

-- Thing - Reference Property Audit Triggers
-- Thing.ExcludedDomain
CREATE TRIGGER Thing_ExcludedDomain_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing.ExcludedPerson
CREATE TRIGGER Thing_ExcludedPerson_audit_prepare
  BEFORE UPDATE ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedPerson_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "EngineeringModel_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ThingReference - Reference Property Audit Triggers

-- TopContainer - Reference Property Audit Triggers

--------------------------------------------------------------------------------------------------------
------------------------ Iteration Classes - revision trigger (apply_revision) -------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Class Revision Trigger
CREATE TRIGGER actualfinitestate_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ActualFiniteState"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ActualFiniteStateList - Class Revision Trigger
CREATE TRIGGER actualfinitestatelist_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ActualFiniteStateList"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- Alias - Class Revision Trigger
CREATE TRIGGER alias_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Alias"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- AndExpression - Class Revision Trigger

-- ArchitectureDiagram - Class Revision Trigger

-- ArchitectureElement - Class Revision Trigger

-- Attachment - Class Revision Trigger
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Behavior - Class Revision Trigger
CREATE TRIGGER behavior_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Behavior"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- BehavioralParameter - Class Revision Trigger
CREATE TRIGGER behavioralparameter_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."BehavioralParameter"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- BinaryRelationship - Class Revision Trigger

-- BooleanExpression - Class Revision Trigger
CREATE TRIGGER booleanexpression_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."BooleanExpression"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Bounds - Class Revision Trigger
CREATE TRIGGER bounds_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Bounds"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- BuiltInRuleVerification - Class Revision Trigger

-- Citation - Class Revision Trigger
CREATE TRIGGER citation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Citation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Color - Class Revision Trigger
CREATE TRIGGER color_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Color"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- DefinedThing - Class Revision Trigger

-- Definition - Class Revision Trigger
CREATE TRIGGER definition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Definition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- DiagramCanvas - Class Revision Trigger
CREATE TRIGGER diagramcanvas_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."DiagramCanvas"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- DiagramEdge - Class Revision Trigger

-- DiagramElementContainer - Class Revision Trigger

-- DiagramElementThing - Class Revision Trigger
CREATE TRIGGER diagramelementthing_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."DiagramElementThing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- DiagramFrame - Class Revision Trigger

-- DiagrammingStyle - Class Revision Trigger

-- DiagramObject - Class Revision Trigger

-- DiagramPort - Class Revision Trigger

-- DiagramShape - Class Revision Trigger

-- DiagramThingBase - Class Revision Trigger

-- DomainFileStore - Class Revision Trigger
CREATE TRIGGER domainfilestore_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."DomainFileStore"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ElementBase - Class Revision Trigger

-- ElementDefinition - Class Revision Trigger
CREATE TRIGGER elementdefinition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementDefinition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- ElementUsage - Class Revision Trigger
CREATE TRIGGER elementusage_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementUsage"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ExclusiveOrExpression - Class Revision Trigger

-- ExternalIdentifierMap - Class Revision Trigger
CREATE TRIGGER externalidentifiermap_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ExternalIdentifierMap"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- File - Class Revision Trigger
CREATE TRIGGER file_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."File"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- FileRevision - Class Revision Trigger
CREATE TRIGGER filerevision_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."FileRevision"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- FileStore - Class Revision Trigger

-- Folder - Class Revision Trigger
CREATE TRIGGER folder_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Folder"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Goal - Class Revision Trigger
CREATE TRIGGER goal_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Goal"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- HyperLink - Class Revision Trigger
CREATE TRIGGER hyperlink_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."HyperLink"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- IdCorrespondence - Class Revision Trigger
CREATE TRIGGER idcorrespondence_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."IdCorrespondence"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- MultiRelationship - Class Revision Trigger

-- NestedElement - Class Revision Trigger
CREATE TRIGGER nestedelement_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."NestedElement"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- NestedParameter - Class Revision Trigger
CREATE TRIGGER nestedparameter_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."NestedParameter"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- NotExpression - Class Revision Trigger

-- Option - Class Revision Trigger
CREATE TRIGGER option_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Option"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- OrExpression - Class Revision Trigger

-- OwnedStyle - Class Revision Trigger
CREATE TRIGGER ownedstyle_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."OwnedStyle"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Parameter - Class Revision Trigger
CREATE TRIGGER parameter_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Parameter"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterBase - Class Revision Trigger

-- ParameterGroup - Class Revision Trigger
CREATE TRIGGER parametergroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterOrOverrideBase - Class Revision Trigger

-- ParameterOverride - Class Revision Trigger
CREATE TRIGGER parameteroverride_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterOverride"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterOverrideValueSet - Class Revision Trigger
CREATE TRIGGER parameteroverridevalueset_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterOverrideValueSet"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterSubscription - Class Revision Trigger
CREATE TRIGGER parametersubscription_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterSubscription"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterSubscriptionValueSet - Class Revision Trigger
CREATE TRIGGER parametersubscriptionvalueset_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterSubscriptionValueSet"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterValue - Class Revision Trigger

-- ParameterValueSet - Class Revision Trigger
CREATE TRIGGER parametervalueset_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParameterValueSet"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- ParameterValueSetBase - Class Revision Trigger

-- ParametricConstraint - Class Revision Trigger
CREATE TRIGGER parametricconstraint_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ParametricConstraint"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Point - Class Revision Trigger
CREATE TRIGGER point_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Point"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- PossibleFiniteState - Class Revision Trigger
CREATE TRIGGER possiblefinitestate_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."PossibleFiniteState"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- PossibleFiniteStateList - Class Revision Trigger
CREATE TRIGGER possiblefinitestatelist_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."PossibleFiniteStateList"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- Publication - Class Revision Trigger
CREATE TRIGGER publication_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Publication"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- RelationalExpression - Class Revision Trigger

-- Relationship - Class Revision Trigger
CREATE TRIGGER relationship_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Relationship"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- RelationshipParameterValue - Class Revision Trigger
CREATE TRIGGER relationshipparametervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RelationshipParameterValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Requirement - Class Revision Trigger
CREATE TRIGGER requirement_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Requirement"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- RequirementsContainer - Class Revision Trigger

-- RequirementsContainerParameterValue - Class Revision Trigger
CREATE TRIGGER requirementscontainerparametervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RequirementsContainerParameterValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- RequirementsGroup - Class Revision Trigger
CREATE TRIGGER requirementsgroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RequirementsGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- RequirementsSpecification - Class Revision Trigger
CREATE TRIGGER requirementsspecification_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RequirementsSpecification"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- RuleVerification - Class Revision Trigger
CREATE TRIGGER ruleverification_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RuleVerification"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- RuleVerificationList - Class Revision Trigger
CREATE TRIGGER ruleverificationlist_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RuleVerificationList"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- RuleViolation - Class Revision Trigger
CREATE TRIGGER ruleviolation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RuleViolation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- SharedStyle - Class Revision Trigger
CREATE TRIGGER sharedstyle_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."SharedStyle"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- SimpleParameterizableThing - Class Revision Trigger

-- SimpleParameterValue - Class Revision Trigger
CREATE TRIGGER simpleparametervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."SimpleParameterValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Stakeholder - Class Revision Trigger
CREATE TRIGGER stakeholder_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Stakeholder"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- StakeholderValue - Class Revision Trigger
CREATE TRIGGER stakeholdervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeholderValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap - Class Revision Trigger
CREATE TRIGGER stakeholdervaluemap_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

-- StakeHolderValueMapSettings - Class Revision Trigger
CREATE TRIGGER stakeholdervaluemapsettings_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMapSettings"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- Thing - Class Revision Trigger
CREATE TRIGGER thing_apply_revision
  BEFORE INSERT 
  ON "Iteration_REPLACE"."Thing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Iid', 'EngineeringModel_REPLACE', 'Iteration_REPLACE');

-- UserRuleVerification - Class Revision Trigger

-- ValueGroup - Class Revision Trigger
CREATE TRIGGER valuegroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ValueGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_REPLACE', 'EngineeringModel_REPLACE');

--------------------------------------------------------------------------------------------------------
------------------ Iteration Reference Properties - Revision Trigger (apply_revision) ------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Reference Property Revision Trigger
-- ActualFiniteState.PossibleState
CREATE TRIGGER actualfinitestate_possiblestate_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ActualFiniteState_PossibleState"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ActualFiniteState', 'EngineeringModel_REPLACE');

-- ActualFiniteStateList - Reference Property Revision Trigger
-- ActualFiniteStateList.ExcludeOption
CREATE TRIGGER actualfinitestatelist_excludeoption_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ActualFiniteStateList', 'EngineeringModel_REPLACE');

-- ActualFiniteStateList.PossibleFiniteStateList
CREATE TRIGGER actualfinitestatelist_possiblefinitestatelist_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ActualFiniteStateList', 'EngineeringModel_REPLACE');

-- Alias - Reference Property Revision Trigger

-- AndExpression - Reference Property Revision Trigger
-- AndExpression.Term
CREATE TRIGGER andexpression_term_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."AndExpression_Term"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('AndExpression', 'EngineeringModel_REPLACE');

-- ArchitectureDiagram - Reference Property Revision Trigger

-- ArchitectureElement - Reference Property Revision Trigger

-- Attachment - Reference Property Revision Trigger
-- Attachment.FileType
CREATE TRIGGER attachment_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Attachment_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Attachment', 'EngineeringModel_REPLACE');

-- Behavior - Reference Property Revision Trigger

-- BehavioralParameter - Reference Property Revision Trigger

-- BinaryRelationship - Reference Property Revision Trigger

-- BooleanExpression - Reference Property Revision Trigger

-- Bounds - Reference Property Revision Trigger

-- BuiltInRuleVerification - Reference Property Revision Trigger

-- Citation - Reference Property Revision Trigger

-- Color - Reference Property Revision Trigger

-- DefinedThing - Reference Property Revision Trigger

-- Definition - Reference Property Revision Trigger
-- Definition.Example
CREATE TRIGGER definition_example_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Definition_Example"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'EngineeringModel_REPLACE');

-- Definition.Note
CREATE TRIGGER definition_note_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Definition_Note"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'EngineeringModel_REPLACE');

-- DiagramCanvas - Reference Property Revision Trigger

-- DiagramEdge - Reference Property Revision Trigger

-- DiagramElementContainer - Reference Property Revision Trigger

-- DiagramElementThing - Reference Property Revision Trigger

-- DiagramFrame - Reference Property Revision Trigger

-- DiagrammingStyle - Reference Property Revision Trigger

-- DiagramObject - Reference Property Revision Trigger

-- DiagramPort - Reference Property Revision Trigger

-- DiagramShape - Reference Property Revision Trigger

-- DiagramThingBase - Reference Property Revision Trigger

-- DomainFileStore - Reference Property Revision Trigger

-- ElementBase - Reference Property Revision Trigger
-- ElementBase.Category
CREATE TRIGGER elementbase_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementBase_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ElementBase', 'EngineeringModel_REPLACE');

-- ElementDefinition - Reference Property Revision Trigger
-- ElementDefinition.OrganizationalParticipant
CREATE TRIGGER elementdefinition_organizationalparticipant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ElementDefinition', 'EngineeringModel_REPLACE');

-- ElementDefinition.ReferencedElement
CREATE TRIGGER elementdefinition_referencedelement_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ElementDefinition', 'EngineeringModel_REPLACE');

-- ElementUsage - Reference Property Revision Trigger
-- ElementUsage.ExcludeOption
CREATE TRIGGER elementusage_excludeoption_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ElementUsage_ExcludeOption"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ElementUsage', 'EngineeringModel_REPLACE');

-- ExclusiveOrExpression - Reference Property Revision Trigger
-- ExclusiveOrExpression.Term
CREATE TRIGGER exclusiveorexpression_term_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ExclusiveOrExpression_Term"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ExclusiveOrExpression', 'EngineeringModel_REPLACE');

-- ExternalIdentifierMap - Reference Property Revision Trigger

-- File - Reference Property Revision Trigger
-- File.Category
CREATE TRIGGER file_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."File_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('File', 'EngineeringModel_REPLACE');

-- FileRevision - Reference Property Revision Trigger
-- FileRevision.FileType
CREATE TRIGGER filerevision_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."FileRevision_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('FileRevision', 'EngineeringModel_REPLACE');

-- FileStore - Reference Property Revision Trigger

-- Folder - Reference Property Revision Trigger

-- Goal - Reference Property Revision Trigger
-- Goal.Category
CREATE TRIGGER goal_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Goal_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Goal', 'EngineeringModel_REPLACE');

-- HyperLink - Reference Property Revision Trigger

-- IdCorrespondence - Reference Property Revision Trigger

-- MultiRelationship - Reference Property Revision Trigger
-- MultiRelationship.RelatedThing
CREATE TRIGGER multirelationship_relatedthing_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."MultiRelationship_RelatedThing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('MultiRelationship', 'EngineeringModel_REPLACE');

-- NestedElement - Reference Property Revision Trigger
-- NestedElement.ElementUsage
CREATE TRIGGER nestedelement_elementusage_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."NestedElement_ElementUsage"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('NestedElement', 'EngineeringModel_REPLACE');

-- NestedParameter - Reference Property Revision Trigger

-- NotExpression - Reference Property Revision Trigger

-- Option - Reference Property Revision Trigger
-- Option.Category
CREATE TRIGGER option_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Option_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Option', 'EngineeringModel_REPLACE');

-- OrExpression - Reference Property Revision Trigger
-- OrExpression.Term
CREATE TRIGGER orexpression_term_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."OrExpression_Term"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('OrExpression', 'EngineeringModel_REPLACE');

-- OwnedStyle - Reference Property Revision Trigger

-- Parameter - Reference Property Revision Trigger

-- ParameterBase - Reference Property Revision Trigger

-- ParameterGroup - Reference Property Revision Trigger

-- ParameterOrOverrideBase - Reference Property Revision Trigger

-- ParameterOverride - Reference Property Revision Trigger

-- ParameterOverrideValueSet - Reference Property Revision Trigger

-- ParameterSubscription - Reference Property Revision Trigger

-- ParameterSubscriptionValueSet - Reference Property Revision Trigger

-- ParameterValue - Reference Property Revision Trigger

-- ParameterValueSet - Reference Property Revision Trigger

-- ParameterValueSetBase - Reference Property Revision Trigger

-- ParametricConstraint - Reference Property Revision Trigger

-- Point - Reference Property Revision Trigger

-- PossibleFiniteState - Reference Property Revision Trigger

-- PossibleFiniteStateList - Reference Property Revision Trigger
-- PossibleFiniteStateList.Category
CREATE TRIGGER possiblefinitestatelist_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."PossibleFiniteStateList_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('PossibleFiniteStateList', 'EngineeringModel_REPLACE');

-- Publication - Reference Property Revision Trigger
-- Publication.Domain
CREATE TRIGGER publication_domain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Publication_Domain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Publication', 'EngineeringModel_REPLACE');

-- Publication.PublishedParameter
CREATE TRIGGER publication_publishedparameter_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Publication_PublishedParameter"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Publication', 'EngineeringModel_REPLACE');

-- RelationalExpression - Reference Property Revision Trigger

-- Relationship - Reference Property Revision Trigger
-- Relationship.Category
CREATE TRIGGER relationship_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Relationship_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Relationship', 'EngineeringModel_REPLACE');

-- RelationshipParameterValue - Reference Property Revision Trigger

-- Requirement - Reference Property Revision Trigger
-- Requirement.Category
CREATE TRIGGER requirement_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Requirement_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Requirement', 'EngineeringModel_REPLACE');

-- RequirementsContainer - Reference Property Revision Trigger
-- RequirementsContainer.Category
CREATE TRIGGER requirementscontainer_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RequirementsContainer_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('RequirementsContainer', 'EngineeringModel_REPLACE');

-- RequirementsContainerParameterValue - Reference Property Revision Trigger

-- RequirementsGroup - Reference Property Revision Trigger

-- RequirementsSpecification - Reference Property Revision Trigger

-- RuleVerification - Reference Property Revision Trigger

-- RuleVerificationList - Reference Property Revision Trigger

-- RuleViolation - Reference Property Revision Trigger
-- RuleViolation.ViolatingThing
CREATE TRIGGER ruleviolation_violatingthing_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."RuleViolation_ViolatingThing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('RuleViolation', 'EngineeringModel_REPLACE');

-- SharedStyle - Reference Property Revision Trigger

-- SimpleParameterizableThing - Reference Property Revision Trigger

-- SimpleParameterValue - Reference Property Revision Trigger

-- Stakeholder - Reference Property Revision Trigger
-- Stakeholder.Category
CREATE TRIGGER stakeholder_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Stakeholder_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Stakeholder', 'EngineeringModel_REPLACE');

-- Stakeholder.StakeholderValue
CREATE TRIGGER stakeholder_stakeholdervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Stakeholder_StakeholderValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Stakeholder', 'EngineeringModel_REPLACE');

-- StakeholderValue - Reference Property Revision Trigger
-- StakeholderValue.Category
CREATE TRIGGER stakeholdervalue_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeholderValue_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeholderValue', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap - Reference Property Revision Trigger
-- StakeHolderValueMap.Category
CREATE TRIGGER stakeholdervaluemap_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeHolderValueMap', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap.Goal
CREATE TRIGGER stakeholdervaluemap_goal_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap_Goal"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeHolderValueMap', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap.Requirement
CREATE TRIGGER stakeholdervaluemap_requirement_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeHolderValueMap', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap.StakeholderValue
CREATE TRIGGER stakeholdervaluemap_stakeholdervalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeHolderValueMap', 'EngineeringModel_REPLACE');

-- StakeHolderValueMap.ValueGroup
CREATE TRIGGER stakeholdervaluemap_valuegroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('StakeHolderValueMap', 'EngineeringModel_REPLACE');

-- StakeHolderValueMapSettings - Reference Property Revision Trigger

-- Thing - Reference Property Revision Trigger
-- Thing.ExcludedDomain
CREATE TRIGGER thing_excludeddomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'EngineeringModel_REPLACE');

-- Thing.ExcludedPerson
CREATE TRIGGER thing_excludedperson_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'EngineeringModel_REPLACE');

-- UserRuleVerification - Reference Property Revision Trigger

-- ValueGroup - Reference Property Revision Trigger
-- ValueGroup.Category
CREATE TRIGGER valuegroup_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "Iteration_REPLACE"."ValueGroup_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ValueGroup', 'EngineeringModel_REPLACE');

--------------------------------------------------------------------------------------------------------
------------------------------------ Iteration Class Audit triggers ------------------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Class Audit Triggers
CREATE TRIGGER ActualFiniteState_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ActualFiniteState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActualFiniteState_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ActualFiniteState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ActualFiniteStateList - Class Audit Triggers
CREATE TRIGGER ActualFiniteStateList_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ActualFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActualFiniteStateList_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ActualFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Alias - Class Audit Triggers
CREATE TRIGGER Alias_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Alias_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- AndExpression - Class Audit Triggers
CREATE TRIGGER AndExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."AndExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER AndExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."AndExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ArchitectureDiagram - Class Audit Triggers
CREATE TRIGGER ArchitectureDiagram_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ArchitectureDiagram"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArchitectureDiagram_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ArchitectureDiagram"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ArchitectureElement - Class Audit Triggers
CREATE TRIGGER ArchitectureElement_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ArchitectureElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArchitectureElement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ArchitectureElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Attachment - Class Audit Triggers
CREATE TRIGGER Attachment_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Behavior - Class Audit Triggers
CREATE TRIGGER Behavior_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Behavior"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Behavior_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Behavior"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BehavioralParameter - Class Audit Triggers
CREATE TRIGGER BehavioralParameter_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."BehavioralParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BehavioralParameter_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."BehavioralParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BinaryRelationship - Class Audit Triggers
CREATE TRIGGER BinaryRelationship_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."BinaryRelationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BinaryRelationship_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."BinaryRelationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BooleanExpression - Class Audit Triggers
CREATE TRIGGER BooleanExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."BooleanExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BooleanExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."BooleanExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Bounds - Class Audit Triggers
CREATE TRIGGER Bounds_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Bounds"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Bounds_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Bounds"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- BuiltInRuleVerification - Class Audit Triggers
CREATE TRIGGER BuiltInRuleVerification_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."BuiltInRuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BuiltInRuleVerification_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."BuiltInRuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Citation - Class Audit Triggers
CREATE TRIGGER Citation_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Citation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Color - Class Audit Triggers
CREATE TRIGGER Color_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Color"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Color_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Color"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DefinedThing - Class Audit Triggers
CREATE TRIGGER DefinedThing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DefinedThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Definition - Class Audit Triggers
CREATE TRIGGER Definition_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramCanvas - Class Audit Triggers
CREATE TRIGGER DiagramCanvas_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramCanvas"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramCanvas_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramCanvas"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramEdge - Class Audit Triggers
CREATE TRIGGER DiagramEdge_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramEdge"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramEdge_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramEdge"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramElementContainer - Class Audit Triggers
CREATE TRIGGER DiagramElementContainer_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramElementContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramElementContainer_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramElementContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramElementThing - Class Audit Triggers
CREATE TRIGGER DiagramElementThing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramElementThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramElementThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramElementThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramFrame - Class Audit Triggers
CREATE TRIGGER DiagramFrame_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramFrame"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramFrame_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramFrame"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagrammingStyle - Class Audit Triggers
CREATE TRIGGER DiagrammingStyle_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagrammingStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagrammingStyle_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagrammingStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramObject - Class Audit Triggers
CREATE TRIGGER DiagramObject_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramObject"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramObject_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramObject"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramPort - Class Audit Triggers
CREATE TRIGGER DiagramPort_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramPort"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramPort_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramPort"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramShape - Class Audit Triggers
CREATE TRIGGER DiagramShape_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramShape"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramShape_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramShape"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramThingBase - Class Audit Triggers
CREATE TRIGGER DiagramThingBase_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DiagramThingBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramThingBase_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DiagramThingBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DomainFileStore - Class Audit Triggers
CREATE TRIGGER DomainFileStore_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."DomainFileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainFileStore_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."DomainFileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementBase - Class Audit Triggers
CREATE TRIGGER ElementBase_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementBase_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementDefinition - Class Audit Triggers
CREATE TRIGGER ElementDefinition_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementDefinition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementUsage - Class Audit Triggers
CREATE TRIGGER ElementUsage_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementUsage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementUsage_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementUsage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ExclusiveOrExpression - Class Audit Triggers
CREATE TRIGGER ExclusiveOrExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ExclusiveOrExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ExclusiveOrExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ExclusiveOrExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ExternalIdentifierMap - Class Audit Triggers
CREATE TRIGGER ExternalIdentifierMap_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ExternalIdentifierMap"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ExternalIdentifierMap_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ExternalIdentifierMap"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- File - Class Audit Triggers
CREATE TRIGGER File_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."File"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER File_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."File"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileRevision - Class Audit Triggers
CREATE TRIGGER FileRevision_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."FileRevision"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileRevision_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."FileRevision"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileStore - Class Audit Triggers
CREATE TRIGGER FileStore_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."FileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileStore_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."FileStore"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Folder - Class Audit Triggers
CREATE TRIGGER Folder_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Folder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Folder_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Folder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Goal - Class Audit Triggers
CREATE TRIGGER Goal_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Goal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Goal_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Goal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- HyperLink - Class Audit Triggers
CREATE TRIGGER HyperLink_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER HyperLink_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- IdCorrespondence - Class Audit Triggers
CREATE TRIGGER IdCorrespondence_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."IdCorrespondence"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IdCorrespondence_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."IdCorrespondence"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- MultiRelationship - Class Audit Triggers
CREATE TRIGGER MultiRelationship_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."MultiRelationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationship_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."MultiRelationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NestedElement - Class Audit Triggers
CREATE TRIGGER NestedElement_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."NestedElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NestedElement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."NestedElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NestedParameter - Class Audit Triggers
CREATE TRIGGER NestedParameter_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."NestedParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NestedParameter_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."NestedParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NotExpression - Class Audit Triggers
CREATE TRIGGER NotExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."NotExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NotExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."NotExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Option - Class Audit Triggers
CREATE TRIGGER Option_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Option"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Option_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Option"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OrExpression - Class Audit Triggers
CREATE TRIGGER OrExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."OrExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."OrExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OwnedStyle - Class Audit Triggers
CREATE TRIGGER OwnedStyle_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."OwnedStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OwnedStyle_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."OwnedStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Parameter - Class Audit Triggers
CREATE TRIGGER Parameter_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Parameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Parameter_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Parameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterBase - Class Audit Triggers
CREATE TRIGGER ParameterBase_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterBase_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterGroup - Class Audit Triggers
CREATE TRIGGER ParameterGroup_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterOrOverrideBase - Class Audit Triggers
CREATE TRIGGER ParameterOrOverrideBase_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterOrOverrideBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterOrOverrideBase_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterOrOverrideBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterOverride - Class Audit Triggers
CREATE TRIGGER ParameterOverride_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterOverride"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterOverride_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterOverride"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterOverrideValueSet - Class Audit Triggers
CREATE TRIGGER ParameterOverrideValueSet_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterOverrideValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterOverrideValueSet_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterOverrideValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterSubscription - Class Audit Triggers
CREATE TRIGGER ParameterSubscription_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterSubscription"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterSubscription_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterSubscription"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterSubscriptionValueSet - Class Audit Triggers
CREATE TRIGGER ParameterSubscriptionValueSet_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterSubscriptionValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterSubscriptionValueSet_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterSubscriptionValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterValue - Class Audit Triggers
CREATE TRIGGER ParameterValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterValueSet - Class Audit Triggers
CREATE TRIGGER ParameterValueSet_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterValueSet_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterValueSet"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParameterValueSetBase - Class Audit Triggers
CREATE TRIGGER ParameterValueSetBase_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParameterValueSetBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterValueSetBase_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParameterValueSetBase"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ParametricConstraint - Class Audit Triggers
CREATE TRIGGER ParametricConstraint_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ParametricConstraint"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParametricConstraint_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ParametricConstraint"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Point - Class Audit Triggers
CREATE TRIGGER Point_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Point"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Point_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Point"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- PossibleFiniteState - Class Audit Triggers
CREATE TRIGGER PossibleFiniteState_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."PossibleFiniteState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PossibleFiniteState_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."PossibleFiniteState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- PossibleFiniteStateList - Class Audit Triggers
CREATE TRIGGER PossibleFiniteStateList_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."PossibleFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PossibleFiniteStateList_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."PossibleFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Publication - Class Audit Triggers
CREATE TRIGGER Publication_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Publication"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Publication_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Publication"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RelationalExpression - Class Audit Triggers
CREATE TRIGGER RelationalExpression_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RelationalExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RelationalExpression_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RelationalExpression"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Relationship - Class Audit Triggers
CREATE TRIGGER Relationship_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Relationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Relationship_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Relationship"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RelationshipParameterValue - Class Audit Triggers
CREATE TRIGGER RelationshipParameterValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RelationshipParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RelationshipParameterValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RelationshipParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Requirement - Class Audit Triggers
CREATE TRIGGER Requirement_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Requirement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Requirement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Requirement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsContainer - Class Audit Triggers
CREATE TRIGGER RequirementsContainer_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RequirementsContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequirementsContainer_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RequirementsContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsContainerParameterValue - Class Audit Triggers
CREATE TRIGGER RequirementsContainerParameterValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RequirementsContainerParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequirementsContainerParameterValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RequirementsContainerParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsGroup - Class Audit Triggers
CREATE TRIGGER RequirementsGroup_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RequirementsGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequirementsGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RequirementsGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsSpecification - Class Audit Triggers
CREATE TRIGGER RequirementsSpecification_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RequirementsSpecification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequirementsSpecification_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RequirementsSpecification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RuleVerification - Class Audit Triggers
CREATE TRIGGER RuleVerification_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RuleVerification_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RuleVerificationList - Class Audit Triggers
CREATE TRIGGER RuleVerificationList_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RuleVerificationList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RuleVerificationList_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RuleVerificationList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RuleViolation - Class Audit Triggers
CREATE TRIGGER RuleViolation_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RuleViolation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RuleViolation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RuleViolation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SharedStyle - Class Audit Triggers
CREATE TRIGGER SharedStyle_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."SharedStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SharedStyle_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."SharedStyle"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SimpleParameterizableThing - Class Audit Triggers
CREATE TRIGGER SimpleParameterizableThing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."SimpleParameterizableThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleParameterizableThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."SimpleParameterizableThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SimpleParameterValue - Class Audit Triggers
CREATE TRIGGER SimpleParameterValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."SimpleParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleParameterValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."SimpleParameterValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Stakeholder - Class Audit Triggers
CREATE TRIGGER Stakeholder_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Stakeholder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Stakeholder_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Stakeholder"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeholderValue - Class Audit Triggers
CREATE TRIGGER StakeholderValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeholderValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap - Class Audit Triggers
CREATE TRIGGER StakeHolderValueMap_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMapSettings - Class Audit Triggers
CREATE TRIGGER StakeHolderValueMapSettings_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMapSettings"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMapSettings_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMapSettings"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing - Class Audit Triggers
CREATE TRIGGER Thing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- UserRuleVerification - Class Audit Triggers
CREATE TRIGGER UserRuleVerification_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."UserRuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UserRuleVerification_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."UserRuleVerification"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ValueGroup - Class Audit Triggers
CREATE TRIGGER ValueGroup_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ValueGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ValueGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ValueGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

--------------------------------------------------------------------------------------------------------
----------------------------- Iteration  Reference Property Audit triggers -----------------------------
--------------------------------------------------------------------------------------------------------

-- ActualFiniteState - Reference Property Audit Triggers
-- ActualFiniteState.PossibleState
CREATE TRIGGER ActualFiniteState_PossibleState_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ActualFiniteState_PossibleState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActualFiniteState_PossibleState_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ActualFiniteState_PossibleState"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ActualFiniteStateList - Reference Property Audit Triggers
-- ActualFiniteStateList.ExcludeOption
CREATE TRIGGER ActualFiniteStateList_ExcludeOption_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActualFiniteStateList_ExcludeOption_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ActualFiniteStateList_ExcludeOption"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ActualFiniteStateList.PossibleFiniteStateList
CREATE TRIGGER ActualFiniteStateList_PossibleFiniteStateList_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ActualFiniteStateList_PossibleFiniteStateList_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ActualFiniteStateList_PossibleFiniteStateList"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Alias - Reference Property Audit Triggers

-- AndExpression - Reference Property Audit Triggers
-- AndExpression.Term
CREATE TRIGGER AndExpression_Term_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."AndExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER AndExpression_Term_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."AndExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ArchitectureDiagram - Reference Property Audit Triggers

-- ArchitectureElement - Reference Property Audit Triggers

-- Attachment - Reference Property Audit Triggers
-- Attachment.FileType
CREATE TRIGGER Attachment_FileType_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Behavior - Reference Property Audit Triggers

-- BehavioralParameter - Reference Property Audit Triggers

-- BinaryRelationship - Reference Property Audit Triggers

-- BooleanExpression - Reference Property Audit Triggers

-- Bounds - Reference Property Audit Triggers

-- BuiltInRuleVerification - Reference Property Audit Triggers

-- Citation - Reference Property Audit Triggers

-- Color - Reference Property Audit Triggers

-- DefinedThing - Reference Property Audit Triggers

-- Definition - Reference Property Audit Triggers
-- Definition.Example
CREATE TRIGGER Definition_Example_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Example_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Definition.Note
CREATE TRIGGER Definition_Note_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Note_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- DiagramCanvas - Reference Property Audit Triggers

-- DiagramEdge - Reference Property Audit Triggers

-- DiagramElementContainer - Reference Property Audit Triggers

-- DiagramElementThing - Reference Property Audit Triggers

-- DiagramFrame - Reference Property Audit Triggers

-- DiagrammingStyle - Reference Property Audit Triggers

-- DiagramObject - Reference Property Audit Triggers

-- DiagramPort - Reference Property Audit Triggers

-- DiagramShape - Reference Property Audit Triggers

-- DiagramThingBase - Reference Property Audit Triggers

-- DomainFileStore - Reference Property Audit Triggers

-- ElementBase - Reference Property Audit Triggers
-- ElementBase.Category
CREATE TRIGGER ElementBase_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementBase_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementBase_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementBase_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementDefinition - Reference Property Audit Triggers
-- ElementDefinition.OrganizationalParticipant
CREATE TRIGGER ElementDefinition_OrganizationalParticipant_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementDefinition_OrganizationalParticipant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementDefinition.ReferencedElement
CREATE TRIGGER ElementDefinition_ReferencedElement_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementDefinition_ReferencedElement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementDefinition_ReferencedElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ElementUsage - Reference Property Audit Triggers
-- ElementUsage.ExcludeOption
CREATE TRIGGER ElementUsage_ExcludeOption_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ElementUsage_ExcludeOption"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementUsage_ExcludeOption_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ElementUsage_ExcludeOption"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ExclusiveOrExpression - Reference Property Audit Triggers
-- ExclusiveOrExpression.Term
CREATE TRIGGER ExclusiveOrExpression_Term_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ExclusiveOrExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ExclusiveOrExpression_Term_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ExclusiveOrExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- ExternalIdentifierMap - Reference Property Audit Triggers

-- File - Reference Property Audit Triggers
-- File.Category
CREATE TRIGGER File_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."File_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER File_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."File_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileRevision - Reference Property Audit Triggers
-- FileRevision.FileType
CREATE TRIGGER FileRevision_FileType_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."FileRevision_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileRevision_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."FileRevision_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- FileStore - Reference Property Audit Triggers

-- Folder - Reference Property Audit Triggers

-- Goal - Reference Property Audit Triggers
-- Goal.Category
CREATE TRIGGER Goal_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Goal_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Goal_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Goal_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- HyperLink - Reference Property Audit Triggers

-- IdCorrespondence - Reference Property Audit Triggers

-- MultiRelationship - Reference Property Audit Triggers
-- MultiRelationship.RelatedThing
CREATE TRIGGER MultiRelationship_RelatedThing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."MultiRelationship_RelatedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationship_RelatedThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."MultiRelationship_RelatedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NestedElement - Reference Property Audit Triggers
-- NestedElement.ElementUsage
CREATE TRIGGER NestedElement_ElementUsage_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."NestedElement_ElementUsage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NestedElement_ElementUsage_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."NestedElement_ElementUsage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- NestedParameter - Reference Property Audit Triggers

-- NotExpression - Reference Property Audit Triggers

-- Option - Reference Property Audit Triggers
-- Option.Category
CREATE TRIGGER Option_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Option_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Option_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Option_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OrExpression - Reference Property Audit Triggers
-- OrExpression.Term
CREATE TRIGGER OrExpression_Term_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."OrExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrExpression_Term_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."OrExpression_Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- OwnedStyle - Reference Property Audit Triggers

-- Parameter - Reference Property Audit Triggers

-- ParameterBase - Reference Property Audit Triggers

-- ParameterGroup - Reference Property Audit Triggers

-- ParameterOrOverrideBase - Reference Property Audit Triggers

-- ParameterOverride - Reference Property Audit Triggers

-- ParameterOverrideValueSet - Reference Property Audit Triggers

-- ParameterSubscription - Reference Property Audit Triggers

-- ParameterSubscriptionValueSet - Reference Property Audit Triggers

-- ParameterValue - Reference Property Audit Triggers

-- ParameterValueSet - Reference Property Audit Triggers

-- ParameterValueSetBase - Reference Property Audit Triggers

-- ParametricConstraint - Reference Property Audit Triggers

-- Point - Reference Property Audit Triggers

-- PossibleFiniteState - Reference Property Audit Triggers

-- PossibleFiniteStateList - Reference Property Audit Triggers
-- PossibleFiniteStateList.Category
CREATE TRIGGER PossibleFiniteStateList_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."PossibleFiniteStateList_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PossibleFiniteStateList_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."PossibleFiniteStateList_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Publication - Reference Property Audit Triggers
-- Publication.Domain
CREATE TRIGGER Publication_Domain_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Publication_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Publication_Domain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Publication_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Publication.PublishedParameter
CREATE TRIGGER Publication_PublishedParameter_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Publication_PublishedParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Publication_PublishedParameter_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Publication_PublishedParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RelationalExpression - Reference Property Audit Triggers

-- Relationship - Reference Property Audit Triggers
-- Relationship.Category
CREATE TRIGGER Relationship_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Relationship_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Relationship_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Relationship_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RelationshipParameterValue - Reference Property Audit Triggers

-- Requirement - Reference Property Audit Triggers
-- Requirement.Category
CREATE TRIGGER Requirement_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Requirement_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Requirement_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Requirement_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsContainer - Reference Property Audit Triggers
-- RequirementsContainer.Category
CREATE TRIGGER RequirementsContainer_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RequirementsContainer_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RequirementsContainer_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RequirementsContainer_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- RequirementsContainerParameterValue - Reference Property Audit Triggers

-- RequirementsGroup - Reference Property Audit Triggers

-- RequirementsSpecification - Reference Property Audit Triggers

-- RuleVerification - Reference Property Audit Triggers

-- RuleVerificationList - Reference Property Audit Triggers

-- RuleViolation - Reference Property Audit Triggers
-- RuleViolation.ViolatingThing
CREATE TRIGGER RuleViolation_ViolatingThing_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."RuleViolation_ViolatingThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RuleViolation_ViolatingThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."RuleViolation_ViolatingThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- SharedStyle - Reference Property Audit Triggers

-- SimpleParameterizableThing - Reference Property Audit Triggers

-- SimpleParameterValue - Reference Property Audit Triggers

-- Stakeholder - Reference Property Audit Triggers
-- Stakeholder.Category
CREATE TRIGGER Stakeholder_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Stakeholder_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Stakeholder_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Stakeholder_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Stakeholder.StakeholderValue
CREATE TRIGGER Stakeholder_StakeholderValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Stakeholder_StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Stakeholder_StakeholderValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Stakeholder_StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeholderValue - Reference Property Audit Triggers
-- StakeholderValue.Category
CREATE TRIGGER StakeholderValue_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeholderValue_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeholderValue_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeholderValue_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap - Reference Property Audit Triggers
-- StakeHolderValueMap.Category
CREATE TRIGGER StakeHolderValueMap_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap.Goal
CREATE TRIGGER StakeHolderValueMap_Goal_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap_Goal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_Goal_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap_Goal"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap.Requirement
CREATE TRIGGER StakeHolderValueMap_Requirement_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_Requirement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap_Requirement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap.StakeholderValue
CREATE TRIGGER StakeHolderValueMap_StakeholderValue_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_StakeholderValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap_StakeholderValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMap.ValueGroup
CREATE TRIGGER StakeHolderValueMap_ValueGroup_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER StakeHolderValueMap_ValueGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."StakeHolderValueMap_ValueGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- StakeHolderValueMapSettings - Reference Property Audit Triggers

-- Thing - Reference Property Audit Triggers
-- Thing.ExcludedDomain
CREATE TRIGGER Thing_ExcludedDomain_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- Thing.ExcludedPerson
CREATE TRIGGER Thing_ExcludedPerson_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedPerson_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

-- UserRuleVerification - Reference Property Audit Triggers

-- ValueGroup - Reference Property Audit Triggers
-- ValueGroup.Category
CREATE TRIGGER ValueGroup_Category_audit_prepare
  BEFORE UPDATE ON "Iteration_REPLACE"."ValueGroup_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ValueGroup_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "Iteration_REPLACE"."ValueGroup_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
