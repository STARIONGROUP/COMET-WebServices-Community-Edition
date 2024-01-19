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

-- Class ArchitectureElement derives from DiagramObject
ALTER TABLE "SchemaName_Replace"."ArchitectureElement" ADD CONSTRAINT "ArchitectureElementDerivesFromDiagramObject" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."DiagramObject" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

-- Class DiagramPort derives from DiagramShape
ALTER TABLE "SchemaName_Replace"."DiagramPort" ADD CONSTRAINT "DiagramPortDerivesFromDiagramShape" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."DiagramShape" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DiagramFrame derives from DiagramShape
ALTER TABLE "SchemaName_Replace"."DiagramFrame" ADD CONSTRAINT "DiagramFrameDerivesFromDiagramShape" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."DiagramShape" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;

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
