-- Create table for class Attachment (which derives from: Thing)
CREATE TABLE "SchemaName_Replace"."Attachment" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Attachment_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."Attachment" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Attachment
CREATE TABLE "SchemaName_Replace"."Attachment_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Attachment_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."Attachment_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Attachment
CREATE TABLE "SchemaName_Replace"."Attachment_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Attachment_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AttachmentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."Attachment_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class Behavior (which derives from: DefinedThing)
CREATE TABLE "SchemaName_Replace"."Behavior" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Behavior_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."Behavior" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Behavior" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Behavior
CREATE TABLE "SchemaName_Replace"."Behavior_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Behavior_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."Behavior_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Behavior_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Behavior
CREATE TABLE "SchemaName_Replace"."Behavior_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Behavior_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BehaviorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."Behavior_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Behavior_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class BehavioralParameter (which derives from: Thing)
CREATE TABLE "SchemaName_Replace"."BehavioralParameter" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "BehavioralParameter_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for BehavioralParameter
CREATE TABLE "SchemaName_Replace"."BehavioralParameter_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BehavioralParameter_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for BehavioralParameter
CREATE TABLE "SchemaName_Replace"."BehavioralParameter_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BehavioralParameter_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BehavioralParameterCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class ArchitectureDiagram (which derives from: DiagramCanvas and implements: OwnedThing)
CREATE TABLE "SchemaName_Replace"."ArchitectureDiagram" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ArchitectureDiagram_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ArchitectureDiagram
CREATE TABLE "SchemaName_Replace"."ArchitectureDiagram_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArchitectureDiagram_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ArchitectureDiagram
CREATE TABLE "SchemaName_Replace"."ArchitectureDiagram_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArchitectureDiagram_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ArchitectureDiagramCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class ArchitectureElement (which derives from: DiagramObject)
CREATE TABLE "SchemaName_Replace"."ArchitectureElement" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ArchitectureElement_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ArchitectureElement
CREATE TABLE "SchemaName_Replace"."ArchitectureElement_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArchitectureElement_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ArchitectureElement
CREATE TABLE "SchemaName_Replace"."ArchitectureElement_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArchitectureElement_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ArchitectureElementCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DiagramPort (which derives from: DiagramShape)
CREATE TABLE "SchemaName_Replace"."DiagramPort" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DiagramPort_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."DiagramPort" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramPort" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DiagramPort
CREATE TABLE "SchemaName_Replace"."DiagramPort_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramPort_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DiagramPort
CREATE TABLE "SchemaName_Replace"."DiagramPort_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramPort_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DiagramPortCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DiagramFrame (which derives from: DiagramShape)
CREATE TABLE "SchemaName_Replace"."DiagramFrame" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DiagramFrame_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."DiagramFrame" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramFrame" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DiagramFrame
CREATE TABLE "SchemaName_Replace"."DiagramFrame_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramFrame_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DiagramFrame
CREATE TABLE "SchemaName_Replace"."DiagramFrame_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DiagramFrame_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DiagramFrameCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Attachment is contained (composite) by DefinedThing: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."Attachment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."Attachment" ADD CONSTRAINT "Attachment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Attachment_Container" ON "SchemaName_Replace"."Attachment" ("Container");
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_Replace', 'SchemaName_Replace');

-- Class Attachment derives from Thing
ALTER TABLE "SchemaName_Replace"."Attachment" ADD CONSTRAINT "AttachmentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- FileType is a collection property (many to many) of class Attachment: [1..*]-[1..1]
CREATE TABLE "SchemaName_Replace"."Attachment_FileType" (
  "Attachment" uuid NOT NULL,
  "FileType" uuid NOT NULL,
  CONSTRAINT "Attachment_FileType_PK" PRIMARY KEY("Attachment", "FileType"),
  CONSTRAINT "Attachment_FileType_FK_Source" FOREIGN KEY ("Attachment") REFERENCES "SchemaName_Replace"."Attachment" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Attachment_FileType_FK_Target" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SchemaName_Replace"."Attachment_FileType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Attachment_FileType_ValidFrom" ON "SchemaName_Replace"."Attachment_FileType" ("ValidFrom");
CREATE INDEX "Idx_Attachment_FileType_ValidTo" ON "SchemaName_Replace"."Attachment_FileType" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."Attachment_FileType_Audit" (LIKE "SchemaName_Replace"."Attachment_FileType");

ALTER TABLE "SchemaName_Replace"."Attachment_FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Attachment_FileTypeAudit_ValidFrom" ON "SchemaName_Replace"."Attachment_FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_Attachment_FileTypeAudit_ValidTo" ON "SchemaName_Replace"."Attachment_FileType_Audit" ("ValidTo");

CREATE TRIGGER Attachment_FileType_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER attachment_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."Attachment_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Attachment', 'EngineeringModel_Replace');

-- Behavior is contained (composite) by ElementDefinition: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."Behavior" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."Behavior" ADD CONSTRAINT "Behavior_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Behavior_Container" ON "SchemaName_Replace"."Behavior" ("Container");
CREATE TRIGGER behavior_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."Behavior"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_Replace', 'SchemaName_Replace');

-- Class Behavior derives from DefinedThing
ALTER TABLE "SchemaName_Replace"."Behavior" ADD CONSTRAINT "BehaviorDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- BehavioralParameter is contained (composite) by Behavior: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."BehavioralParameter" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."BehavioralParameter" ADD CONSTRAINT "BehavioralParameter_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."Behavior" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_BehavioralParameter_Container" ON "SchemaName_Replace"."BehavioralParameter" ("Container");
CREATE TRIGGER behavioralparameter_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."BehavioralParameter"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_Replace', 'SchemaName_Replace');
-- Class BehavioralParameter derives from Thing
ALTER TABLE "SchemaName_Replace"."BehavioralParameter" ADD CONSTRAINT "BehavioralParameterDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- BehavioralParameter.Parameter is an association to Parameter: [1..1]-[1..1]
ALTER TABLE "SchemaName_Replace"."BehavioralParameter" ADD COLUMN "Parameter" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."BehavioralParameter" ADD CONSTRAINT "BehavioralParameter_FK_Parameter" FOREIGN KEY ("Parameter") REFERENCES "SchemaName_Replace"."Parameter" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class ArchitectureDiagram derives from DiagramCanvas
ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" ADD CONSTRAINT "ArchitectureDiagramDerivesFromDiagramCanvas" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."DiagramCanvas" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ArchitectureDiagram.TopArchitectureElement is an optional association to ArchitectureElement: [0..1]-[1..1]
ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" ADD COLUMN "TopArchitectureElement" uuid;
ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" ADD CONSTRAINT "ArchitectureDiagram_FK_TopArchitectureElement" FOREIGN KEY ("TopArchitectureElement") REFERENCES "SchemaName_Replace"."ArchitectureElement" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- ArchitectureDiagram.Owner is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" ADD COLUMN "Owner" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram" ADD CONSTRAINT "ArchitectureDiagram_FK_Owner" FOREIGN KEY ("Owner") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

ALTER TABLE "SchemaName_Replace"."Attachment"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Attachment_ValidFrom" ON "SchemaName_Replace"."Attachment" ("ValidFrom");
CREATE INDEX "Idx_Attachment_ValidTo" ON "SchemaName_Replace"."Attachment" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."Attachment_Audit" (LIKE "SchemaName_Replace"."Attachment");

ALTER TABLE "SchemaName_Replace"."Attachment_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Attachment_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Attachment_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AttachmentAudit_ValidFrom" ON "SchemaName_Replace"."Attachment_Audit" ("ValidFrom");
CREATE INDEX "Idx_AttachmentAudit_ValidTo" ON "SchemaName_Replace"."Attachment_Audit" ("ValidTo");

CREATE TRIGGER Attachment_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

ALTER TABLE "SchemaName_Replace"."Behavior"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Behavior_ValidFrom" ON "SchemaName_Replace"."Behavior" ("ValidFrom");
CREATE INDEX "Idx_Behavior_ValidTo" ON "SchemaName_Replace"."Behavior" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."Behavior_Audit" (LIKE "SchemaName_Replace"."Behavior");

ALTER TABLE "SchemaName_Replace"."Behavior_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Behavior_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."Behavior_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."Behavior_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BehaviorAudit_ValidFrom" ON "SchemaName_Replace"."Behavior_Audit" ("ValidFrom");
CREATE INDEX "Idx_BehaviorAudit_ValidTo" ON "SchemaName_Replace"."Behavior_Audit" ("ValidTo");

CREATE TRIGGER Behavior_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."Behavior"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Behavior_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."Behavior"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SchemaName_Replace"."BehavioralParameter"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_BehavioralParameter_ValidFrom" ON "SchemaName_Replace"."BehavioralParameter" ("ValidFrom");
CREATE INDEX "Idx_BehavioralParameter_ValidTo" ON "SchemaName_Replace"."BehavioralParameter" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."BehavioralParameter_Audit" (LIKE "SchemaName_Replace"."BehavioralParameter");

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."BehavioralParameter_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BehavioralParameterAudit_ValidFrom" ON "SchemaName_Replace"."BehavioralParameter_Audit" ("ValidFrom");
CREATE INDEX "Idx_BehavioralParameterAudit_ValidTo" ON "SchemaName_Replace"."BehavioralParameter_Audit" ("ValidTo");

CREATE TRIGGER BehavioralParameter_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."BehavioralParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BehavioralParameter_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."BehavioralParameter"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ArchitectureDiagram_ValidFrom" ON "SchemaName_Replace"."ArchitectureDiagram" ("ValidFrom");
CREATE INDEX "Idx_ArchitectureDiagram_ValidTo" ON "SchemaName_Replace"."ArchitectureDiagram" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" (LIKE "SchemaName_Replace"."ArchitectureDiagram");

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureDiagram_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArchitectureDiagramAudit_ValidFrom" ON "SchemaName_Replace"."ArchitectureDiagram_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArchitectureDiagramAudit_ValidTo" ON "SchemaName_Replace"."ArchitectureDiagram_Audit" ("ValidTo");

CREATE TRIGGER ArchitectureDiagram_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."ArchitectureDiagram"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArchitectureDiagram_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."ArchitectureDiagram"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

ALTER TABLE "SchemaName_Replace"."ArchitectureElement"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ArchitectureElement_ValidFrom" ON "SchemaName_Replace"."ArchitectureElement" ("ValidFrom");
CREATE INDEX "Idx_ArchitectureElement_ValidTo" ON "SchemaName_Replace"."ArchitectureElement" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."ArchitectureElement_Audit" (LIKE "SchemaName_Replace"."ArchitectureElement");

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ArchitectureElement_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArchitectureElementAudit_ValidFrom" ON "SchemaName_Replace"."ArchitectureElement_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArchitectureElementAudit_ValidTo" ON "SchemaName_Replace"."ArchitectureElement_Audit" ("ValidTo");

CREATE TRIGGER ArchitectureElement_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."ArchitectureElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArchitectureElement_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."ArchitectureElement"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SchemaName_Replace"."DiagramPort"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DiagramPort_ValidFrom" ON "SchemaName_Replace"."DiagramPort" ("ValidFrom");
CREATE INDEX "Idx_DiagramPort_ValidTo" ON "SchemaName_Replace"."DiagramPort" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."DiagramPort_Audit" (LIKE "SchemaName_Replace"."DiagramPort");

ALTER TABLE "SchemaName_Replace"."DiagramPort_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramPort_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramPortAudit_ValidFrom" ON "SchemaName_Replace"."DiagramPort_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramPortAudit_ValidTo" ON "SchemaName_Replace"."DiagramPort_Audit" ("ValidTo");

CREATE TRIGGER DiagramPort_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."DiagramPort"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramPort_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."DiagramPort"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SchemaName_Replace"."DiagramFrame"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DiagramFrame_ValidFrom" ON "SchemaName_Replace"."DiagramFrame" ("ValidFrom");
CREATE INDEX "Idx_DiagramFrame_ValidTo" ON "SchemaName_Replace"."DiagramFrame" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."DiagramFrame_Audit" (LIKE "SchemaName_Replace"."DiagramFrame");

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."DiagramFrame_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiagramFrameAudit_ValidFrom" ON "SchemaName_Replace"."DiagramFrame_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiagramFrameAudit_ValidTo" ON "SchemaName_Replace"."DiagramFrame_Audit" ("ValidTo");

CREATE TRIGGER DiagramFrame_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."DiagramFrame"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiagramFrame_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."DiagramFrame"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."Attachment_Data" ()
    RETURNS SETOF "SchemaName_Replace"."Attachment" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."Attachment";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."Attachment"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."Attachment_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."Behavior_Data" ()
    RETURNS SETOF "SchemaName_Replace"."Behavior" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."Behavior";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."Behavior"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."Behavior_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."BehavioralParameter_Data" ()
    RETURNS SETOF "SchemaName_Replace"."BehavioralParameter" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."BehavioralParameter";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Parameter","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."BehavioralParameter"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Parameter","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."BehavioralParameter_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."ArchitectureDiagram_Data" ()
    RETURNS SETOF "SchemaName_Replace"."ArchitectureDiagram" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."ArchitectureDiagram";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","TopArchitectureElement","Owner","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."ArchitectureDiagram"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","TopArchitectureElement","Owner","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."ArchitectureDiagram_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."ArchitectureElement_Data" ()
    RETURNS SETOF "SchemaName_Replace"."ArchitectureElement" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."ArchitectureElement";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."ArchitectureElement"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."ArchitectureElement_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."DiagramPort_Data" ()
    RETURNS SETOF "SchemaName_Replace"."DiagramPort" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."DiagramPort";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."DiagramPort"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."DiagramPort_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."DiagramFrame_Data" ()
    RETURNS SETOF "SchemaName_Replace"."DiagramFrame" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."DiagramFrame";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."DiagramFrame"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."DiagramFrame_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."Attachment_FileType_Data" ()
    RETURNS SETOF "SchemaName_Replace"."Attachment_FileType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."Attachment_FileType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Attachment","FileType","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."Attachment_FileType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Attachment","FileType","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."Attachment_FileType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

DROP VIEW IF EXISTS "SchemaName_Replace"."DefinedThing_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."DefinedThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Option_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Option_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Option"."ValueTypeDictionary" AS "ValueTypeSet",
	"Option"."Container",
	"Option"."Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Option_NestedElement"."NestedElement",'{}'::text[]) AS "NestedElement",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Option_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."Option_Data"() AS "Option" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Option" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Option_Category_Data"() AS "Option_Category"
   JOIN "SchemaName_Replace"."Option_Data"() AS "Option" ON "Option" = "Iid"
   GROUP BY "Option") AS "Option_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "NestedElement"."Container" AS "Iid", array_agg("NestedElement"."Iid"::text) AS "NestedElement"
   FROM "SchemaName_Replace"."NestedElement_Data"() AS "NestedElement"
   JOIN "SchemaName_Replace"."Option_Data"() AS "Option" ON "NestedElement"."Container" = "Option"."Iid"
   GROUP BY "NestedElement"."Container") AS "Option_NestedElement" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Attachment_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Attachment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Attachment"."ValueTypeDictionary" AS "ValueTypeSet",
	"Attachment"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Attachment_FileType"."FileType",'{}'::text[]) AS "FileType"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Attachment_Data"() AS "Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Attachment" AS "Iid", array_agg("FileType"::text) AS "FileType"
   FROM "SchemaName_Replace"."Attachment_FileType_Data"() AS "Attachment_FileType"
   JOIN "SchemaName_Replace"."Attachment_Data"() AS "Attachment" ON "Attachment" = "Iid"
   GROUP BY "Attachment") AS "Attachment_FileType" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."PossibleFiniteStateList_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."PossibleFiniteStateList_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PossibleFiniteStateList"."ValueTypeDictionary" AS "ValueTypeSet",
	"PossibleFiniteStateList"."Container",
	NULL::bigint AS "Sequence",
	"PossibleFiniteStateList"."DefaultState",
	"PossibleFiniteStateList"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("PossibleFiniteStateList_PossibleState"."PossibleState",'{}'::text[]) AS "PossibleState",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("PossibleFiniteStateList_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."PossibleFiniteStateList_Data"() AS "PossibleFiniteStateList" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "PossibleFiniteStateList" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."PossibleFiniteStateList_Category_Data"() AS "PossibleFiniteStateList_Category"
   JOIN "SchemaName_Replace"."PossibleFiniteStateList_Data"() AS "PossibleFiniteStateList" ON "PossibleFiniteStateList" = "Iid"
   GROUP BY "PossibleFiniteStateList") AS "PossibleFiniteStateList_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "PossibleFiniteState"."Container" AS "Iid", ARRAY[array_agg("PossibleFiniteState"."Sequence"::text), array_agg("PossibleFiniteState"."Iid"::text)] AS "PossibleState"
   FROM "SchemaName_Replace"."PossibleFiniteState_Data"() AS "PossibleFiniteState"
   JOIN "SchemaName_Replace"."PossibleFiniteStateList_Data"() AS "PossibleFiniteStateList" ON "PossibleFiniteState"."Container" = "PossibleFiniteStateList"."Iid"
   GROUP BY "PossibleFiniteState"."Container") AS "PossibleFiniteStateList_PossibleState" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."PossibleFiniteState_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."PossibleFiniteState_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PossibleFiniteState"."ValueTypeDictionary" AS "ValueTypeSet",
	"PossibleFiniteState"."Container",
	"PossibleFiniteState"."Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."PossibleFiniteState_Data"() AS "PossibleFiniteState" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ElementBase_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementBase"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ElementBase_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ElementBase" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ElementBase_Category_Data"() AS "ElementBase_Category"
   JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" ON "ElementBase" = "Iid"
   GROUP BY "ElementBase") AS "ElementBase_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ElementDefinition_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" || "ElementDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementDefinition"."Container",
	NULL::bigint AS "Sequence",
	"ElementBase"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ElementDefinition_ContainedElement"."ContainedElement",'{}'::text[]) AS "ContainedElement",
	COALESCE("ElementDefinition_Parameter"."Parameter",'{}'::text[]) AS "Parameter",
	COALESCE("ElementDefinition_ParameterGroup"."ParameterGroup",'{}'::text[]) AS "ParameterGroup",
	COALESCE("ElementDefinition_Behavior"."Behavior",'{}'::text[]) AS "Behavior",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ElementBase_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("ElementDefinition_ReferencedElement"."ReferencedElement",'{}'::text[]) AS "ReferencedElement",
	COALESCE("ElementDefinition_OrganizationalParticipant"."OrganizationalParticipant",'{}'::text[]) AS "OrganizationalParticipant"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ElementBase" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ElementBase_Category_Data"() AS "ElementBase_Category"
   JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" ON "ElementBase" = "Iid"
   GROUP BY "ElementBase") AS "ElementBase_Category" USING ("Iid")
 LEFT JOIN (SELECT "ElementDefinition" AS "Iid", array_agg("ReferencedElement"::text) AS "ReferencedElement"
   FROM "SchemaName_Replace"."ElementDefinition_ReferencedElement_Data"() AS "ElementDefinition_ReferencedElement"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "ElementDefinition" = "Iid"
   GROUP BY "ElementDefinition") AS "ElementDefinition_ReferencedElement" USING ("Iid")
 LEFT JOIN (SELECT "ElementDefinition" AS "Iid", array_agg("OrganizationalParticipant"::text) AS "OrganizationalParticipant"
   FROM "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Data"() AS "ElementDefinition_OrganizationalParticipant"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "ElementDefinition" = "Iid"
   GROUP BY "ElementDefinition") AS "ElementDefinition_OrganizationalParticipant" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ElementUsage"."Container" AS "Iid", array_agg("ElementUsage"."Iid"::text) AS "ContainedElement"
   FROM "SchemaName_Replace"."ElementUsage_Data"() AS "ElementUsage"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "ElementUsage"."Container" = "ElementDefinition"."Iid"
   GROUP BY "ElementUsage"."Container") AS "ElementDefinition_ContainedElement" USING ("Iid")
  LEFT JOIN (SELECT "Parameter"."Container" AS "Iid", array_agg("Parameter"."Iid"::text) AS "Parameter"
   FROM "SchemaName_Replace"."Parameter_Data"() AS "Parameter"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "Parameter"."Container" = "ElementDefinition"."Iid"
   GROUP BY "Parameter"."Container") AS "ElementDefinition_Parameter" USING ("Iid")
  LEFT JOIN (SELECT "ParameterGroup"."Container" AS "Iid", array_agg("ParameterGroup"."Iid"::text) AS "ParameterGroup"
   FROM "SchemaName_Replace"."ParameterGroup_Data"() AS "ParameterGroup"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "ParameterGroup"."Container" = "ElementDefinition"."Iid"
   GROUP BY "ParameterGroup"."Container") AS "ElementDefinition_ParameterGroup" USING ("Iid")
  LEFT JOIN (SELECT "Behavior"."Container" AS "Iid", array_agg("Behavior"."Iid"::text) AS "Behavior"
   FROM "SchemaName_Replace"."Behavior_Data"() AS "Behavior"
   JOIN "SchemaName_Replace"."ElementDefinition_Data"() AS "ElementDefinition" ON "Behavior"."Container" = "ElementDefinition"."Iid"
   GROUP BY "Behavior"."Container") AS "ElementDefinition_Behavior" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ElementUsage_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementUsage_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" || "ElementUsage"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementUsage"."Container",
	NULL::bigint AS "Sequence",
	"ElementBase"."Owner",
	"ElementUsage"."ElementDefinition",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ElementUsage_ParameterOverride"."ParameterOverride",'{}'::text[]) AS "ParameterOverride",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ElementBase_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("ElementUsage_ExcludeOption"."ExcludeOption",'{}'::text[]) AS "ExcludeOption"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ElementUsage_Data"() AS "ElementUsage" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ElementBase" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ElementBase_Category_Data"() AS "ElementBase_Category"
   JOIN "SchemaName_Replace"."ElementBase_Data"() AS "ElementBase" ON "ElementBase" = "Iid"
   GROUP BY "ElementBase") AS "ElementBase_Category" USING ("Iid")
 LEFT JOIN (SELECT "ElementUsage" AS "Iid", array_agg("ExcludeOption"::text) AS "ExcludeOption"
   FROM "SchemaName_Replace"."ElementUsage_ExcludeOption_Data"() AS "ElementUsage_ExcludeOption"
   JOIN "SchemaName_Replace"."ElementUsage_Data"() AS "ElementUsage" ON "ElementUsage" = "Iid"
   GROUP BY "ElementUsage") AS "ElementUsage_ExcludeOption" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterOverride"."Container" AS "Iid", array_agg("ParameterOverride"."Iid"::text) AS "ParameterOverride"
   FROM "SchemaName_Replace"."ParameterOverride_Data"() AS "ParameterOverride"
   JOIN "SchemaName_Replace"."ElementUsage_Data"() AS "ElementUsage" ON "ParameterOverride"."Container" = "ElementUsage"."Iid"
   GROUP BY "ParameterOverride"."Container") AS "ElementUsage_ParameterOverride" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Behavior_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Behavior_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Behavior"."ValueTypeDictionary" AS "ValueTypeSet",
	"Behavior"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Behavior_BehavioralParameter"."BehavioralParameter",'{}'::text[]) AS "BehavioralParameter",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."Behavior_Data"() AS "Behavior" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "BehavioralParameter"."Container" AS "Iid", array_agg("BehavioralParameter"."Iid"::text) AS "BehavioralParameter"
   FROM "SchemaName_Replace"."BehavioralParameter_Data"() AS "BehavioralParameter"
   JOIN "SchemaName_Replace"."Behavior_Data"() AS "Behavior" ON "BehavioralParameter"."Container" = "Behavior"."Iid"
   GROUP BY "BehavioralParameter"."Container") AS "Behavior_BehavioralParameter" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."BehavioralParameter_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."BehavioralParameter_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BehavioralParameter"."ValueTypeDictionary" AS "ValueTypeSet",
	"BehavioralParameter"."Container",
	NULL::bigint AS "Sequence",
	"BehavioralParameter"."Parameter",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BehavioralParameter_Data"() AS "BehavioralParameter" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."RequirementsContainer_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsContainer"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RequirementsContainer_Group"."Group",'{}'::text[]) AS "Group",
	COALESCE("RequirementsContainer_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("RequirementsContainer_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "RequirementsContainer" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."RequirementsContainer_Category_Data"() AS "RequirementsContainer_Category"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainer" = "Iid"
   GROUP BY "RequirementsContainer") AS "RequirementsContainer_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsGroup"."Container" AS "Iid", array_agg("RequirementsGroup"."Iid"::text) AS "Group"
   FROM "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsGroup"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsGroup"."Container") AS "RequirementsContainer_Group" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsContainerParameterValue"."Container" AS "Iid", array_agg("RequirementsContainerParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainerParameterValue"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsContainerParameterValue"."Container") AS "RequirementsContainer_ParameterValue" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."RequirementsSpecification_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsSpecification_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" || "RequirementsSpecification"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsSpecification"."Container",
	NULL::bigint AS "Sequence",
	"RequirementsContainer"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RequirementsContainer_Group"."Group",'{}'::text[]) AS "Group",
	COALESCE("RequirementsContainer_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("RequirementsSpecification_Requirement"."Requirement",'{}'::text[]) AS "Requirement",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("RequirementsContainer_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsSpecification_Data"() AS "RequirementsSpecification" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "RequirementsContainer" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."RequirementsContainer_Category_Data"() AS "RequirementsContainer_Category"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainer" = "Iid"
   GROUP BY "RequirementsContainer") AS "RequirementsContainer_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsGroup"."Container" AS "Iid", array_agg("RequirementsGroup"."Iid"::text) AS "Group"
   FROM "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsGroup"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsGroup"."Container") AS "RequirementsContainer_Group" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsContainerParameterValue"."Container" AS "Iid", array_agg("RequirementsContainerParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainerParameterValue"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsContainerParameterValue"."Container") AS "RequirementsContainer_ParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "Requirement"."Container" AS "Iid", array_agg("Requirement"."Iid"::text) AS "Requirement"
   FROM "SchemaName_Replace"."Requirement_Data"() AS "Requirement"
   JOIN "SchemaName_Replace"."RequirementsSpecification_Data"() AS "RequirementsSpecification" ON "Requirement"."Container" = "RequirementsSpecification"."Iid"
   GROUP BY "Requirement"."Container") AS "RequirementsSpecification_Requirement" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."RequirementsGroup_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" || "RequirementsGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsGroup"."Container",
	NULL::bigint AS "Sequence",
	"RequirementsContainer"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RequirementsContainer_Group"."Group",'{}'::text[]) AS "Group",
	COALESCE("RequirementsContainer_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("RequirementsContainer_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "RequirementsContainer" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."RequirementsContainer_Category_Data"() AS "RequirementsContainer_Category"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainer" = "Iid"
   GROUP BY "RequirementsContainer") AS "RequirementsContainer_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsGroup"."Container" AS "Iid", array_agg("RequirementsGroup"."Iid"::text) AS "Group"
   FROM "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsGroup"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsGroup"."Container") AS "RequirementsContainer_Group" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsContainerParameterValue"."Container" AS "Iid", array_agg("RequirementsContainerParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainerParameterValue"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsContainerParameterValue"."Container") AS "RequirementsContainer_ParameterValue" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."SimpleParameterizableThing_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."SimpleParameterizableThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "SimpleParameterizableThing"."ValueTypeDictionary" AS "ValueTypeSet",
	"SimpleParameterizableThing"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("SimpleParameterizableThing_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "SimpleParameterValue"."Container" AS "Iid", array_agg("SimpleParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."SimpleParameterValue_Data"() AS "SimpleParameterValue"
   JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" ON "SimpleParameterValue"."Container" = "SimpleParameterizableThing"."Iid"
   GROUP BY "SimpleParameterValue"."Container") AS "SimpleParameterizableThing_ParameterValue" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Requirement_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Requirement_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "SimpleParameterizableThing"."ValueTypeDictionary" || "Requirement"."ValueTypeDictionary" AS "ValueTypeSet",
	"Requirement"."Container",
	NULL::bigint AS "Sequence",
	"SimpleParameterizableThing"."Owner",
	"Requirement"."Group",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("SimpleParameterizableThing_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Requirement_ParametricConstraint"."ParametricConstraint",'{}'::text[]) AS "ParametricConstraint",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Requirement_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" USING ("Iid")
  JOIN "SchemaName_Replace"."Requirement_Data"() AS "Requirement" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Requirement" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Requirement_Category_Data"() AS "Requirement_Category"
   JOIN "SchemaName_Replace"."Requirement_Data"() AS "Requirement" ON "Requirement" = "Iid"
   GROUP BY "Requirement") AS "Requirement_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "SimpleParameterValue"."Container" AS "Iid", array_agg("SimpleParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."SimpleParameterValue_Data"() AS "SimpleParameterValue"
   JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" ON "SimpleParameterValue"."Container" = "SimpleParameterizableThing"."Iid"
   GROUP BY "SimpleParameterValue"."Container") AS "SimpleParameterizableThing_ParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "ParametricConstraint"."Container" AS "Iid", ARRAY[array_agg("ParametricConstraint"."Sequence"::text), array_agg("ParametricConstraint"."Iid"::text)] AS "ParametricConstraint"
   FROM "SchemaName_Replace"."ParametricConstraint_Data"() AS "ParametricConstraint"
   JOIN "SchemaName_Replace"."Requirement_Data"() AS "Requirement" ON "ParametricConstraint"."Container" = "Requirement"."Iid"
   GROUP BY "ParametricConstraint"."Container") AS "Requirement_ParametricConstraint" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."RuleVerificationList_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."RuleVerificationList_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RuleVerificationList"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleVerificationList"."Container",
	NULL::bigint AS "Sequence",
	"RuleVerificationList"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RuleVerificationList_RuleVerification"."RuleVerification",'{}'::text[]) AS "RuleVerification",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."RuleVerificationList_Data"() AS "RuleVerificationList" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RuleVerification"."Container" AS "Iid", ARRAY[array_agg("RuleVerification"."Sequence"::text), array_agg("RuleVerification"."Iid"::text)] AS "RuleVerification"
   FROM "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification"
   JOIN "SchemaName_Replace"."RuleVerificationList_Data"() AS "RuleVerificationList" ON "RuleVerification"."Container" = "RuleVerificationList"."Iid"
   GROUP BY "RuleVerification"."Container") AS "RuleVerificationList_RuleVerification" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Stakeholder_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Stakeholder_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Stakeholder"."ValueTypeDictionary" AS "ValueTypeSet",
	"Stakeholder"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Stakeholder_StakeholderValue"."StakeholderValue",'{}'::text[]) AS "StakeholderValue",
	COALESCE("Stakeholder_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."Stakeholder_Data"() AS "Stakeholder" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Stakeholder" AS "Iid", array_agg("StakeholderValue"::text) AS "StakeholderValue"
   FROM "SchemaName_Replace"."Stakeholder_StakeholderValue_Data"() AS "Stakeholder_StakeholderValue"
   JOIN "SchemaName_Replace"."Stakeholder_Data"() AS "Stakeholder" ON "Stakeholder" = "Iid"
   GROUP BY "Stakeholder") AS "Stakeholder_StakeholderValue" USING ("Iid")
 LEFT JOIN (SELECT "Stakeholder" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Stakeholder_Category_Data"() AS "Stakeholder_Category"
   JOIN "SchemaName_Replace"."Stakeholder_Data"() AS "Stakeholder" ON "Stakeholder" = "Iid"
   GROUP BY "Stakeholder") AS "Stakeholder_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."Goal_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."Goal_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Goal"."ValueTypeDictionary" AS "ValueTypeSet",
	"Goal"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Goal_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."Goal_Data"() AS "Goal" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Goal" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Goal_Category_Data"() AS "Goal_Category"
   JOIN "SchemaName_Replace"."Goal_Data"() AS "Goal" ON "Goal" = "Iid"
   GROUP BY "Goal") AS "Goal_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ValueGroup_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ValueGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ValueGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"ValueGroup"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ValueGroup_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."ValueGroup_Data"() AS "ValueGroup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ValueGroup" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ValueGroup_Category_Data"() AS "ValueGroup_Category"
   JOIN "SchemaName_Replace"."ValueGroup_Data"() AS "ValueGroup" ON "ValueGroup" = "Iid"
   GROUP BY "ValueGroup") AS "ValueGroup_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."StakeholderValue_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."StakeholderValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "StakeholderValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"StakeholderValue"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("StakeholderValue_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."StakeholderValue_Data"() AS "StakeholderValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "StakeholderValue" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."StakeholderValue_Category_Data"() AS "StakeholderValue_Category"
   JOIN "SchemaName_Replace"."StakeholderValue_Data"() AS "StakeholderValue" ON "StakeholderValue" = "Iid"
   GROUP BY "StakeholderValue") AS "StakeholderValue_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."StakeHolderValueMap_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."StakeHolderValueMap_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "StakeHolderValueMap"."ValueTypeDictionary" AS "ValueTypeSet",
	"StakeHolderValueMap"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("StakeHolderValueMap_Settings"."Settings",'{}'::text[]) AS "Settings",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("StakeHolderValueMap_Goal"."Goal",'{}'::text[]) AS "Goal",
	COALESCE("StakeHolderValueMap_ValueGroup"."ValueGroup",'{}'::text[]) AS "ValueGroup",
	COALESCE("StakeHolderValueMap_StakeholderValue"."StakeholderValue",'{}'::text[]) AS "StakeholderValue",
	COALESCE("StakeHolderValueMap_Requirement"."Requirement",'{}'::text[]) AS "Requirement",
	COALESCE("StakeHolderValueMap_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "StakeHolderValueMap" AS "Iid", array_agg("Goal"::text) AS "Goal"
   FROM "SchemaName_Replace"."StakeHolderValueMap_Goal_Data"() AS "StakeHolderValueMap_Goal"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMap" = "Iid"
   GROUP BY "StakeHolderValueMap") AS "StakeHolderValueMap_Goal" USING ("Iid")
 LEFT JOIN (SELECT "StakeHolderValueMap" AS "Iid", array_agg("ValueGroup"::text) AS "ValueGroup"
   FROM "SchemaName_Replace"."StakeHolderValueMap_ValueGroup_Data"() AS "StakeHolderValueMap_ValueGroup"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMap" = "Iid"
   GROUP BY "StakeHolderValueMap") AS "StakeHolderValueMap_ValueGroup" USING ("Iid")
 LEFT JOIN (SELECT "StakeHolderValueMap" AS "Iid", array_agg("StakeholderValue"::text) AS "StakeholderValue"
   FROM "SchemaName_Replace"."StakeHolderValueMap_StakeholderValue_Data"() AS "StakeHolderValueMap_StakeholderValue"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMap" = "Iid"
   GROUP BY "StakeHolderValueMap") AS "StakeHolderValueMap_StakeholderValue" USING ("Iid")
 LEFT JOIN (SELECT "StakeHolderValueMap" AS "Iid", array_agg("Requirement"::text) AS "Requirement"
   FROM "SchemaName_Replace"."StakeHolderValueMap_Requirement_Data"() AS "StakeHolderValueMap_Requirement"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMap" = "Iid"
   GROUP BY "StakeHolderValueMap") AS "StakeHolderValueMap_Requirement" USING ("Iid")
 LEFT JOIN (SELECT "StakeHolderValueMap" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."StakeHolderValueMap_Category_Data"() AS "StakeHolderValueMap_Category"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMap" = "Iid"
   GROUP BY "StakeHolderValueMap") AS "StakeHolderValueMap_Category" USING ("Iid")
  LEFT JOIN (SELECT "Alias"."Container" AS "Iid", array_agg("Alias"."Iid"::text) AS "Alias"
   FROM "SchemaName_Replace"."Alias_Data"() AS "Alias"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Alias"."Container" = "DefinedThing"."Iid"
   GROUP BY "Alias"."Container") AS "DefinedThing_Alias" USING ("Iid")
  LEFT JOIN (SELECT "Definition"."Container" AS "Iid", array_agg("Definition"."Iid"::text) AS "Definition"
   FROM "SchemaName_Replace"."Definition_Data"() AS "Definition"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Definition"."Container" = "DefinedThing"."Iid"
   GROUP BY "Definition"."Container") AS "DefinedThing_Definition" USING ("Iid")
  LEFT JOIN (SELECT "HyperLink"."Container" AS "Iid", array_agg("HyperLink"."Iid"::text) AS "HyperLink"
   FROM "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "HyperLink"."Container" = "DefinedThing"."Iid"
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "StakeHolderValueMapSettings"."Container" AS "Iid", array_agg("StakeHolderValueMapSettings"."Iid"::text) AS "Settings"
   FROM "SchemaName_Replace"."StakeHolderValueMapSettings_Data"() AS "StakeHolderValueMapSettings"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMapSettings"."Container" = "StakeHolderValueMap"."Iid"
   GROUP BY "StakeHolderValueMapSettings"."Container") AS "StakeHolderValueMap_Settings" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."DiagramCanvas_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramCanvas_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramCanvas"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramCanvas"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramCanvas_Data"() AS "DiagramCanvas" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ArchitectureDiagram_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ArchitectureDiagram_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramCanvas"."ValueTypeDictionary" || "ArchitectureDiagram"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramCanvas"."Container",
	NULL::bigint AS "Sequence",
	"ArchitectureDiagram"."TopArchitectureElement",
	"ArchitectureDiagram"."Owner",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramCanvas_Data"() AS "DiagramCanvas" USING ("Iid")
  JOIN "SchemaName_Replace"."ArchitectureDiagram_Data"() AS "ArchitectureDiagram" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."DiagramObject_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramObject_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramObject"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramShape_Data"() AS "DiagramShape" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramObject_Data"() AS "DiagramObject" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid")
  LEFT JOIN (SELECT "OwnedStyle"."Container" AS "Iid", array_agg("OwnedStyle"."Iid"::text) AS "LocalStyle"
   FROM "SchemaName_Replace"."OwnedStyle_Data"() AS "OwnedStyle"
   JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" ON "OwnedStyle"."Container" = "DiagramElementThing"."Iid"
   GROUP BY "OwnedStyle"."Container") AS "DiagramElementThing_LocalStyle" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."ArchitectureElement_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ArchitectureElement_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramObject"."ValueTypeDictionary" || "ArchitectureElement"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("ArchitectureElement_DiagramPort"."DiagramPort",'{}'::text[]) AS "DiagramPort",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramShape_Data"() AS "DiagramShape" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramObject_Data"() AS "DiagramObject" USING ("Iid")
  JOIN "SchemaName_Replace"."ArchitectureElement_Data"() AS "ArchitectureElement" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid")
  LEFT JOIN (SELECT "OwnedStyle"."Container" AS "Iid", array_agg("OwnedStyle"."Iid"::text) AS "LocalStyle"
   FROM "SchemaName_Replace"."OwnedStyle_Data"() AS "OwnedStyle"
   JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" ON "OwnedStyle"."Container" = "DiagramElementThing"."Iid"
   GROUP BY "OwnedStyle"."Container") AS "DiagramElementThing_LocalStyle" USING ("Iid")
  LEFT JOIN (SELECT "DiagramPort"."Container" AS "Iid", array_agg("DiagramPort"."Iid"::text) AS "DiagramPort"
   FROM "SchemaName_Replace"."DiagramPort_Data"() AS "DiagramPort"
   JOIN "SchemaName_Replace"."ArchitectureElement_Data"() AS "ArchitectureElement" ON "DiagramPort"."Container" = "ArchitectureElement"."Iid"
   GROUP BY "DiagramPort"."Container") AS "ArchitectureElement_DiagramPort" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."DiagramPort_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramPort_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramPort"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramPort"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramShape_Data"() AS "DiagramShape" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramPort_Data"() AS "DiagramPort" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid")
  LEFT JOIN (SELECT "OwnedStyle"."Container" AS "Iid", array_agg("OwnedStyle"."Iid"::text) AS "LocalStyle"
   FROM "SchemaName_Replace"."OwnedStyle_Data"() AS "OwnedStyle"
   JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" ON "OwnedStyle"."Container" = "DiagramElementThing"."Iid"
   GROUP BY "OwnedStyle"."Container") AS "DiagramElementThing_LocalStyle" USING ("Iid");

DROP VIEW IF EXISTS "SchemaName_Replace"."DiagramFrame_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramFrame_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramFrame"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramShape_Data"() AS "DiagramShape" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramFrame_Data"() AS "DiagramFrame" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid")
  LEFT JOIN (SELECT "OwnedStyle"."Container" AS "Iid", array_agg("OwnedStyle"."Iid"::text) AS "LocalStyle"
   FROM "SchemaName_Replace"."OwnedStyle_Data"() AS "OwnedStyle"
   JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" ON "OwnedStyle"."Container" = "DiagramElementThing"."Iid"
   GROUP BY "OwnedStyle"."Container") AS "DiagramElementThing_LocalStyle" USING ("Iid");
