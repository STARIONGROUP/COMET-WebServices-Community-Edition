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
