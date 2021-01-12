-- Create table for class SampledFunctionParameterType (which derives from: ParameterType)
CREATE TABLE "SiteDirectory"."SampledFunctionParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SampledFunctionParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SampledFunctionParameterType
CREATE TABLE "SiteDirectory"."SampledFunctionParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SampledFunctionParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SampledFunctionParameterType
CREATE TABLE "SiteDirectory"."SampledFunctionParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SampledFunctionParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SampledFunctionParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class IndependentParameterTypeAssignment (which derives from: Thing and implements: ParameterTypeAssignment)
CREATE TABLE "SiteDirectory"."IndependentParameterTypeAssignment" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "IndependentParameterTypeAssignment_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for IndependentParameterTypeAssignment
CREATE TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IndependentParameterTypeAssignment_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for IndependentParameterTypeAssignment
CREATE TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IndependentParameterTypeAssignment_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IndependentParameterTypeAssignmentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DependentParameterTypeAssignment (which derives from: Thing and implements: ParameterTypeAssignment)
CREATE TABLE "SiteDirectory"."DependentParameterTypeAssignment" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DependentParameterTypeAssignment_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DependentParameterTypeAssignment
CREATE TABLE "SiteDirectory"."DependentParameterTypeAssignment_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DependentParameterTypeAssignment_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DependentParameterTypeAssignment
CREATE TABLE "SiteDirectory"."DependentParameterTypeAssignment_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DependentParameterTypeAssignment_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DependentParameterTypeAssignmentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class OrganizationalParticipant (which derives from: Thing)
CREATE TABLE "SiteDirectory"."OrganizationalParticipant" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "OrganizationalParticipant_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for OrganizationalParticipant
CREATE TABLE "SiteDirectory"."OrganizationalParticipant_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrganizationalParticipant_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for OrganizationalParticipant
CREATE TABLE "SiteDirectory"."OrganizationalParticipant_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrganizationalParticipant_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrganizationalParticipantCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class LogEntryChangelogItem (which derives from: Thing)
CREATE TABLE "SiteDirectory"."LogEntryChangelogItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "LogEntryChangelogItem_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" SET (autovacuum_analyze_threshold = 2500);

-- create revision-history table for LogEntryChangelogItem
CREATE TABLE "SiteDirectory"."LogEntryChangelogItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Revision" SET (autovacuum_analyze_threshold = 2500);

-- create cache table for LogEntryChangelogItem
CREATE TABLE "SiteDirectory"."LogEntryChangelogItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LogEntryChangelogItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Class SampledFunctionParameterType derives from ParameterType
ALTER TABLE "SiteDirectory"."SampledFunctionParameterType" ADD CONSTRAINT "SampledFunctionParameterTypeDerivesFromParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- IndependentParameterTypeAssignment is contained (composite) by SampledFunctionParameterType: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD CONSTRAINT "IndependentParameterTypeAssignment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SampledFunctionParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_IndependentParameterTypeAssignment_Container" ON "SiteDirectory"."IndependentParameterTypeAssignment" ("Container");
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER independentparametertypeassignment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- DependentParameterTypeAssignment is contained (composite) by SampledFunctionParameterType: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD CONSTRAINT "DependentParameterTypeAssignment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SampledFunctionParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_DependentParameterTypeAssignment_Container" ON "SiteDirectory"."DependentParameterTypeAssignment" ("Container");
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER dependentparametertypeassignment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class IndependentParameterTypeAssignment derives from Thing
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD CONSTRAINT "IndependentParameterTypeAssignmentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- IndependentParameterTypeAssignment.MeasurementScale is an optional association to MeasurementScale: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD COLUMN "MeasurementScale" uuid;
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD CONSTRAINT "IndependentParameterTypeAssignment_FK_MeasurementScale" FOREIGN KEY ("MeasurementScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- IndependentParameterTypeAssignment.ParameterType is an association to ParameterType: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment" ADD CONSTRAINT "IndependentParameterTypeAssignment_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DependentParameterTypeAssignment derives from Thing
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD CONSTRAINT "DependentParameterTypeAssignmentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- DependentParameterTypeAssignment.MeasurementScale is an optional association to MeasurementScale: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD COLUMN "MeasurementScale" uuid;
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD CONSTRAINT "DependentParameterTypeAssignment_FK_MeasurementScale" FOREIGN KEY ("MeasurementScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- DependentParameterTypeAssignment.ParameterType is an association to ParameterType: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment" ADD CONSTRAINT "DependentParameterTypeAssignment_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- OrganizationalParticipant is contained (composite) by EngineeringModelSetup: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."OrganizationalParticipant" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."OrganizationalParticipant" ADD CONSTRAINT "OrganizationalParticipant_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_OrganizationalParticipant_Container" ON "SiteDirectory"."OrganizationalParticipant" ("Container");
CREATE TRIGGER organizationalparticipant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- EngineeringModelSetup.DefaultOrganizationalParticipant is an optional association to OrganizationalParticipant: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD COLUMN "DefaultOrganizationalParticipant" uuid;
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD CONSTRAINT "EngineeringModelSetup_FK_DefaultOrganizationalParticipant" FOREIGN KEY ("DefaultOrganizationalParticipant") REFERENCES "SiteDirectory"."OrganizationalParticipant" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

-- update audit table
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" ADD COLUMN "DefaultOrganizationalParticipant" uuid;
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" 
  ADD COLUMN "Action_New" character(1),
  ADD COLUMN "Actor_New" uuid;

UPDATE "SiteDirectory"."EngineeringModelSetup_Audit" SET "Action_New"="Action", "Actor_New"="Actor";
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" DROP COLUMN "Action" CASCADE, DROP COLUMN "Actor" CASCADE;

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" RENAME COLUMN "Action_New" to "Action";
ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" RENAME COLUMN "Actor_New" to "Actor";

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" ALTER COLUMN "Action" SET NOT NULL;

-- Class OrganizationalParticipant derives from Thing
ALTER TABLE "SiteDirectory"."OrganizationalParticipant" ADD CONSTRAINT "OrganizationalParticipantDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- OrganizationalParticipant.Organization is an association to Organization: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."OrganizationalParticipant" ADD COLUMN "Organization" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."OrganizationalParticipant" ADD CONSTRAINT "OrganizationalParticipant_FK_Organization" FOREIGN KEY ("Organization") REFERENCES "SiteDirectory"."Organization" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- AffectedDomainIid is a collection property of class SiteLogEntry: [0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid" (
  "SiteLogEntry" uuid NOT NULL,
  "AffectedDomainIid" uuid NOT NULL,
  CONSTRAINT "SiteLogEntry_AffectedDomainIid_PK" PRIMARY KEY("SiteLogEntry","AffectedDomainIid"),
  CONSTRAINT "SiteLogEntry_AffectedDomainIid_FK_Source" FOREIGN KEY ("SiteLogEntry") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteLogEntry_AffectedDomainIid_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedDomainIid_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" (LIKE "SiteDirectory"."SiteLogEntry_AffectedDomainIid");

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntry_AffectedDomainIidAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedDomainIidAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit" ("ValidTo");

CREATE TRIGGER SiteLogEntry_AffectedDomainIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_AffectedDomainIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER sitelogentry_affecteddomainiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');
-- LogEntryChangelogItem is contained (composite) by SiteLogEntry: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" ADD CONSTRAINT "LogEntryChangelogItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_LogEntryChangelogItem_Container" ON "SiteDirectory"."LogEntryChangelogItem" ("Container");
CREATE TRIGGER logentrychangelogitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class LogEntryChangelogItem derives from Thing
ALTER TABLE "SiteDirectory"."LogEntryChangelogItem" ADD CONSTRAINT "LogEntryChangelogItemDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- AffectedReferenceIid is a collection property of class LogEntryChangelogItem: [0..*]
CREATE TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" (
  "LogEntryChangelogItem" uuid NOT NULL,
  "AffectedReferenceIid" uuid NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_AffectedReferenceIid_PK" PRIMARY KEY("LogEntryChangelogItem","AffectedReferenceIid"),
  CONSTRAINT "LogEntryChangelogItem_AffectedReferenceIid_FK_Source" FOREIGN KEY ("LogEntryChangelogItem") REFERENCES "SiteDirectory"."LogEntryChangelogItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIid_ValidFrom" ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIid_ValidTo" ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" ("ValidTo");

CREATE TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" (LIKE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid");

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIidAudit_ValidFrom" ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIidAudit_ValidTo" ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit" ("ValidTo");

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER logentrychangelogitem_affectedreferenceiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('LogEntryChangelogItem', 'SiteDirectory');

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SampledFunctionParameterType_ValidFrom" ON "SiteDirectory"."SampledFunctionParameterType" ("ValidFrom");
CREATE INDEX "Idx_SampledFunctionParameterType_ValidTo" ON "SiteDirectory"."SampledFunctionParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" (LIKE "SiteDirectory"."SampledFunctionParameterType");

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SampledFunctionParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SampledFunctionParameterTypeAudit_ValidFrom" ON "SiteDirectory"."SampledFunctionParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_SampledFunctionParameterTypeAudit_ValidTo" ON "SiteDirectory"."SampledFunctionParameterType_Audit" ("ValidTo");

CREATE TRIGGER SampledFunctionParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SampledFunctionParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SampledFunctionParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SampledFunctionParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_IndependentParameterTypeAssignment_ValidFrom" ON "SiteDirectory"."IndependentParameterTypeAssignment" ("ValidFrom");
CREATE INDEX "Idx_IndependentParameterTypeAssignment_ValidTo" ON "SiteDirectory"."IndependentParameterTypeAssignment" ("ValidTo");

CREATE TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" (LIKE "SiteDirectory"."IndependentParameterTypeAssignment");

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."IndependentParameterTypeAssignment_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IndependentParameterTypeAssignmentAudit_ValidFrom" ON "SiteDirectory"."IndependentParameterTypeAssignment_Audit" ("ValidFrom");
CREATE INDEX "Idx_IndependentParameterTypeAssignmentAudit_ValidTo" ON "SiteDirectory"."IndependentParameterTypeAssignment_Audit" ("ValidTo");

CREATE TRIGGER IndependentParameterTypeAssignment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IndependentParameterTypeAssignment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IndependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DependentParameterTypeAssignment_ValidFrom" ON "SiteDirectory"."DependentParameterTypeAssignment" ("ValidFrom");
CREATE INDEX "Idx_DependentParameterTypeAssignment_ValidTo" ON "SiteDirectory"."DependentParameterTypeAssignment" ("ValidTo");

CREATE TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" (LIKE "SiteDirectory"."DependentParameterTypeAssignment");

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DependentParameterTypeAssignment_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DependentParameterTypeAssignmentAudit_ValidFrom" ON "SiteDirectory"."DependentParameterTypeAssignment_Audit" ("ValidFrom");
CREATE INDEX "Idx_DependentParameterTypeAssignmentAudit_ValidTo" ON "SiteDirectory"."DependentParameterTypeAssignment_Audit" ("ValidTo");

CREATE TRIGGER DependentParameterTypeAssignment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DependentParameterTypeAssignment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DependentParameterTypeAssignment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

ALTER TABLE "SiteDirectory"."OrganizationalParticipant"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_OrganizationalParticipant_ValidFrom" ON "SiteDirectory"."OrganizationalParticipant" ("ValidFrom");
CREATE INDEX "Idx_OrganizationalParticipant_ValidTo" ON "SiteDirectory"."OrganizationalParticipant" ("ValidTo");

CREATE TABLE "SiteDirectory"."OrganizationalParticipant_Audit" (LIKE "SiteDirectory"."OrganizationalParticipant");

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrganizationalParticipant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrganizationalParticipantAudit_ValidFrom" ON "SiteDirectory"."OrganizationalParticipant_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrganizationalParticipantAudit_ValidTo" ON "SiteDirectory"."OrganizationalParticipant_Audit" ("ValidTo");

CREATE TRIGGER OrganizationalParticipant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrganizationalParticipant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LogEntryChangelogItem_ValidFrom" ON "SiteDirectory"."LogEntryChangelogItem" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_ValidTo" ON "SiteDirectory"."LogEntryChangelogItem" ("ValidTo");

CREATE TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" (LIKE "SiteDirectory"."LogEntryChangelogItem");

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogEntryChangelogItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogEntryChangelogItemAudit_ValidFrom" ON "SiteDirectory"."LogEntryChangelogItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItemAudit_ValidTo" ON "SiteDirectory"."LogEntryChangelogItem_Audit" ("ValidTo");

CREATE TRIGGER LogEntryChangelogItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

CREATE OR REPLACE FUNCTION "SiteDirectory"."SampledFunctionParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."SampledFunctionParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SampledFunctionParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SampledFunctionParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SampledFunctionParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."IndependentParameterTypeAssignment_Data" ()
    RETURNS SETOF "SiteDirectory"."IndependentParameterTypeAssignment" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."IndependentParameterTypeAssignment";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","MeasurementScale","ParameterType","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."IndependentParameterTypeAssignment"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","MeasurementScale","ParameterType","ValidFrom","ValidTo"
      FROM "SiteDirectory"."IndependentParameterTypeAssignment_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."DependentParameterTypeAssignment_Data" ()
    RETURNS SETOF "SiteDirectory"."DependentParameterTypeAssignment" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DependentParameterTypeAssignment";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","MeasurementScale","ParameterType","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DependentParameterTypeAssignment"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","MeasurementScale","ParameterType","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DependentParameterTypeAssignment_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."EngineeringModelSetup_Data" ()
    RETURNS SETOF "SiteDirectory"."EngineeringModelSetup" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EngineeringModelSetup";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","DefaultOrganizationalParticipant","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EngineeringModelSetup"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","DefaultOrganizationalParticipant","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EngineeringModelSetup_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."OrganizationalParticipant_Data" ()
    RETURNS SETOF "SiteDirectory"."OrganizationalParticipant" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."OrganizationalParticipant";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Organization","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."OrganizationalParticipant"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Organization","ValidFrom","ValidTo"
      FROM "SiteDirectory"."OrganizationalParticipant_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."LogEntryChangelogItem_Data" ()
    RETURNS SETOF "SiteDirectory"."LogEntryChangelogItem" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."LogEntryChangelogItem";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."LogEntryChangelogItem"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."LogEntryChangelogItem_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteLogEntry_AffectedDomainIid" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteLogEntry_AffectedDomainIid";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "SiteLogEntry","AffectedDomainIid","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteLogEntry_AffectedDomainIid"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "SiteLogEntry","AffectedDomainIid","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Data" ()
    RETURNS SETOF "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "LogEntryChangelogItem","AffectedReferenceIid","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "LogEntryChangelogItem","AffectedReferenceIid","ValidFrom","ValidTo"
      FROM "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE VIEW "SiteDirectory"."SampledFunctionParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "SampledFunctionParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("SampledFunctionParameterType_IndependentParameterType"."IndependentParameterType",'{}'::text[]) AS "IndependentParameterType",
	COALESCE("SampledFunctionParameterType_DependentParameterType"."DependentParameterType",'{}'::text[]) AS "DependentParameterType",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."SampledFunctionParameterType_Data"() AS "SampledFunctionParameterType" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ParameterType" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."ParameterType_Category_Data"() AS "ParameterType_Category"
   JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" ON "ParameterType" = "Iid"
   GROUP BY "ParameterType") AS "ParameterType_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SiteDirectory"."Alias_Data"() AS "Alias"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SiteDirectory"."Definition_Data"() AS "Definition"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SiteDirectory"."HyperLink_Data"() AS "HyperLink"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "IndependentParameterTypeAssignment"."Container" AS "Iid", ARRAY[array_agg("IndependentParameterTypeAssignment"."Sequence"::text), array_agg("IndependentParameterTypeAssignment"."Iid"::text)] AS "IndependentParameterType"
   FROM "SiteDirectory"."IndependentParameterTypeAssignment_Data"() AS "IndependentParameterTypeAssignment"
   JOIN "SiteDirectory"."SampledFunctionParameterType_Data"() AS "SampledFunctionParameterType" ON "IndependentParameterTypeAssignment"."Container" = "SampledFunctionParameterType"."Iid"
   GROUP BY "IndependentParameterTypeAssignment"."Container") AS "SampledFunctionParameterType_IndependentParameterType" USING ("Iid")
  LEFT JOIN (SELECT "DependentParameterTypeAssignment"."Container" AS "Iid", ARRAY[array_agg("DependentParameterTypeAssignment"."Sequence"::text), array_agg("DependentParameterTypeAssignment"."Iid"::text)] AS "DependentParameterType"
   FROM "SiteDirectory"."DependentParameterTypeAssignment_Data"() AS "DependentParameterTypeAssignment"
   JOIN "SiteDirectory"."SampledFunctionParameterType_Data"() AS "SampledFunctionParameterType" ON "DependentParameterTypeAssignment"."Container" = "SampledFunctionParameterType"."Iid"
   GROUP BY "DependentParameterTypeAssignment"."Container") AS "SampledFunctionParameterType_DependentParameterType" USING ("Iid");

CREATE VIEW "SiteDirectory"."IndependentParameterTypeAssignment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "IndependentParameterTypeAssignment"."ValueTypeDictionary" AS "ValueTypeSet",
	"IndependentParameterTypeAssignment"."Container",
	"IndependentParameterTypeAssignment"."Sequence",
	"IndependentParameterTypeAssignment"."MeasurementScale",
	"IndependentParameterTypeAssignment"."ParameterType",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."IndependentParameterTypeAssignment_Data"() AS "IndependentParameterTypeAssignment" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."DependentParameterTypeAssignment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DependentParameterTypeAssignment"."ValueTypeDictionary" AS "ValueTypeSet",
	"DependentParameterTypeAssignment"."Container",
	"DependentParameterTypeAssignment"."Sequence",
	"DependentParameterTypeAssignment"."MeasurementScale",
	"DependentParameterTypeAssignment"."ParameterType",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DependentParameterTypeAssignment_Data"() AS "DependentParameterTypeAssignment" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

DROP VIEW "SiteDirectory"."EngineeringModelSetup_View";

CREATE OR REPLACE VIEW "SiteDirectory"."EngineeringModelSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EngineeringModelSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelSetup"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelSetup"."DefaultOrganizationalParticipant",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("EngineeringModelSetup_Participant"."Participant",'{}'::text[]) AS "Participant",
	COALESCE("EngineeringModelSetup_RequiredRdl"."RequiredRdl",'{}'::text[]) AS "RequiredRdl",
	COALESCE("EngineeringModelSetup_IterationSetup"."IterationSetup",'{}'::text[]) AS "IterationSetup",
	COALESCE("EngineeringModelSetup_OrganizationalParticipant"."OrganizationalParticipant",'{}'::text[]) AS "OrganizationalParticipant",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("EngineeringModelSetup_ActiveDomain"."ActiveDomain",'{}'::text[]) AS "ActiveDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "EngineeringModelSetup" AS "Iid", array_agg("ActiveDomain"::text) AS "ActiveDomain"
   FROM "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Data"() AS "EngineeringModelSetup_ActiveDomain"
   JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" ON "EngineeringModelSetup" = "Iid"
   GROUP BY "EngineeringModelSetup") AS "EngineeringModelSetup_ActiveDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SiteDirectory"."Alias_Data"() AS "Alias"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SiteDirectory"."Definition_Data"() AS "Definition"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SiteDirectory"."HyperLink_Data"() AS "HyperLink"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Participant"."Container" AS "Iid", array_agg("Participant"."Iid"::text) AS "Participant"
   FROM "SiteDirectory"."Participant_Data"() AS "Participant"
   JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" ON "Participant"."Container" = "EngineeringModelSetup"."Iid"
   GROUP BY "Participant"."Container") AS "EngineeringModelSetup_Participant" USING ("Iid")
  LEFT JOIN (SELECT "ModelReferenceDataLibrary"."Container" AS "Iid", array_agg("ModelReferenceDataLibrary"."Iid"::text) AS "RequiredRdl"
   FROM "SiteDirectory"."ModelReferenceDataLibrary_Data"() AS "ModelReferenceDataLibrary"
   JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" ON "ModelReferenceDataLibrary"."Container" = "EngineeringModelSetup"."Iid"
   GROUP BY "ModelReferenceDataLibrary"."Container") AS "EngineeringModelSetup_RequiredRdl" USING ("Iid")
  LEFT JOIN (SELECT "IterationSetup"."Container" AS "Iid", array_agg("IterationSetup"."Iid"::text) AS "IterationSetup"
   FROM "SiteDirectory"."IterationSetup_Data"() AS "IterationSetup"
   JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" ON "IterationSetup"."Container" = "EngineeringModelSetup"."Iid"
   GROUP BY "IterationSetup"."Container") AS "EngineeringModelSetup_IterationSetup" USING ("Iid")
  LEFT JOIN (SELECT "OrganizationalParticipant"."Container" AS "Iid", array_agg("OrganizationalParticipant"."Iid"::text) AS "OrganizationalParticipant"
   FROM "SiteDirectory"."OrganizationalParticipant_Data"() AS "OrganizationalParticipant"
   JOIN "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup" ON "OrganizationalParticipant"."Container" = "EngineeringModelSetup"."Iid"
   GROUP BY "OrganizationalParticipant"."Container") AS "EngineeringModelSetup_OrganizationalParticipant" USING ("Iid");

CREATE VIEW "SiteDirectory"."OrganizationalParticipant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "OrganizationalParticipant"."ValueTypeDictionary" AS "ValueTypeSet",
	"OrganizationalParticipant"."Container",
	NULL::bigint AS "Sequence",
	"OrganizationalParticipant"."Organization",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."OrganizationalParticipant_Data"() AS "OrganizationalParticipant" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

DROP VIEW "SiteDirectory"."SiteLogEntry_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SiteLogEntry_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "SiteLogEntry"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteLogEntry"."Container",
	NULL::bigint AS "Sequence",
	"SiteLogEntry"."Author",
	COALESCE("SiteLogEntry_LogEntryChangelogItem"."LogEntryChangelogItem",'{}'::text[]) AS "LogEntryChangelogItem",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("SiteLogEntry_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("SiteLogEntry_AffectedItemIid"."AffectedItemIid",'{}'::text[]) AS "AffectedItemIid",
	COALESCE("SiteLogEntry_AffectedDomainIid"."AffectedDomainIid",'{}'::text[]) AS "AffectedDomainIid"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "SiteLogEntry" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."SiteLogEntry_Category_Data"() AS "SiteLogEntry_Category"
   JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" ON "SiteLogEntry" = "Iid"
   GROUP BY "SiteLogEntry") AS "SiteLogEntry_Category" USING ("Iid")
 LEFT JOIN (SELECT "SiteLogEntry" AS "Iid", array_agg("AffectedItemIid"::text) AS "AffectedItemIid"
   FROM "SiteDirectory"."SiteLogEntry_AffectedItemIid_Data"() AS "SiteLogEntry_AffectedItemIid"
   JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" ON "SiteLogEntry" = "Iid"
   GROUP BY "SiteLogEntry") AS "SiteLogEntry_AffectedItemIid" USING ("Iid")
 LEFT JOIN (SELECT "SiteLogEntry" AS "Iid", array_agg("AffectedDomainIid"::text) AS "AffectedDomainIid"
   FROM "SiteDirectory"."SiteLogEntry_AffectedDomainIid_Data"() AS "SiteLogEntry_AffectedDomainIid"
   JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" ON "SiteLogEntry" = "Iid"
   GROUP BY "SiteLogEntry") AS "SiteLogEntry_AffectedDomainIid" USING ("Iid")
  LEFT JOIN (SELECT "LogEntryChangelogItem"."Container" AS "Iid", array_agg("LogEntryChangelogItem"."Iid"::text) AS "LogEntryChangelogItem"
   FROM "SiteDirectory"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem"
   JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" ON "LogEntryChangelogItem"."Container" = "SiteLogEntry"."Iid"
   GROUP BY "LogEntryChangelogItem"."Container") AS "SiteLogEntry_LogEntryChangelogItem" USING ("Iid");

CREATE VIEW "SiteDirectory"."LogEntryChangelogItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "LogEntryChangelogItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"LogEntryChangelogItem"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("LogEntryChangelogItem_AffectedReferenceIid"."AffectedReferenceIid",'{}'::text[]) AS "AffectedReferenceIid"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "LogEntryChangelogItem" AS "Iid", array_agg("AffectedReferenceIid"::text) AS "AffectedReferenceIid"
   FROM "SiteDirectory"."LogEntryChangelogItem_AffectedReferenceIid_Data"() AS "LogEntryChangelogItem_AffectedReferenceIid"
   JOIN "SiteDirectory"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem" ON "LogEntryChangelogItem" = "Iid"
   GROUP BY "LogEntryChangelogItem") AS "LogEntryChangelogItem_AffectedReferenceIid" USING ("Iid");