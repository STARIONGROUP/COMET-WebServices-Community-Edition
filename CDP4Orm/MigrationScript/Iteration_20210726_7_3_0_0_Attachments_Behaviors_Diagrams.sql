-- Create table for class Attachment
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

-- Attachment is contained (composite) by Thing: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."Attachment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."Attachment" ADD CONSTRAINT "Attachment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Attachment_Container" ON "SchemaName_Replace"."Attachment" ("Container");
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_Replace', 'SchemaName_Replace');

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
-- DiagramPort is contained (composite) by ArchitectureElement: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."DiagramPort" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."DiagramPort" ADD CONSTRAINT "DiagramPort_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."ArchitectureElement" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_DiagramPort_Container" ON "SchemaName_Replace"."DiagramPort" ("Container");
CREATE TRIGGER diagramport_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."DiagramPort"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'EngineeringModel_Replace', 'SchemaName_Replace');
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."Attachment_View" AS
 SELECT "Thing"."Iid", "Attachment"."ValueTypeDictionary" AS "ValueTypeSet",
	"Attachment"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Attachment_FileType"."FileType",'{}'::text[]) AS "FileType"
  FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
  LEFT JOIN (SELECT "Attachment" AS "Iid", array_agg("FileType"::text) AS "FileType"
   FROM "SchemaName_Replace"."Attachment_FileType_Data"() AS "Attachment_FileType"
   JOIN "SchemaName_Replace"."Attachment_Data"() AS "Attachment" ON "Attachment" = "Iid"
   GROUP BY "Attachment") AS "Attachment_FileType" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Thing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DefinedThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Option_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Option"."ValueTypeDictionary" AS "ValueTypeSet",
	"Option"."Container",
	"Option"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "NestedElement"."Container" AS "Iid", array_agg("NestedElement"."Iid"::text) AS "NestedElement"
   FROM "SchemaName_Replace"."NestedElement_Data"() AS "NestedElement"
   JOIN "SchemaName_Replace"."Option_Data"() AS "Option" ON "NestedElement"."Container" = "Option"."Iid"
   GROUP BY "NestedElement"."Container") AS "Option_NestedElement" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Alias_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Alias"."ValueTypeDictionary" AS "ValueTypeSet",
	"Alias"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Alias_Data"() AS "Alias" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Definition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Definition"."ValueTypeDictionary" AS "ValueTypeSet",
	"Definition"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Definition_Citation"."Citation",'{}'::text[]) AS "Citation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Definition_Note"."Note",'{}'::text[]) AS "Note",
	COALESCE("Definition_Example"."Example",'{}'::text[]) AS "Example"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Definition_Data"() AS "Definition" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Definition" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("Note"::text)] AS "Note"
   FROM "SchemaName_Replace"."Definition_Note_Data"() AS "Definition_Note"
   JOIN "SchemaName_Replace"."Definition_Data"() AS "Definition" ON "Definition" = "Iid"
   GROUP BY "Definition") AS "Definition_Note" USING ("Iid")
 LEFT JOIN (SELECT "Definition" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("Example"::text)] AS "Example"
   FROM "SchemaName_Replace"."Definition_Example_Data"() AS "Definition_Example"
   JOIN "SchemaName_Replace"."Definition_Data"() AS "Definition" ON "Definition" = "Iid"
   GROUP BY "Definition") AS "Definition_Example" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Citation"."Container" AS "Iid", array_agg("Citation"."Iid"::text) AS "Citation"
   FROM "SchemaName_Replace"."Citation_Data"() AS "Citation"
   JOIN "SchemaName_Replace"."Definition_Data"() AS "Definition" ON "Citation"."Container" = "Definition"."Iid"
   GROUP BY "Citation"."Container") AS "Definition_Citation" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Citation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Citation"."ValueTypeDictionary" AS "ValueTypeSet",
	"Citation"."Container",
	NULL::bigint AS "Sequence",
	"Citation"."Source",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Citation_Data"() AS "Citation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."HyperLink_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "HyperLink"."ValueTypeDictionary" AS "ValueTypeSet",
	"HyperLink"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."HyperLink_Data"() AS "HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."NestedElement_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "NestedElement"."ValueTypeDictionary" AS "ValueTypeSet",
	"NestedElement"."Container",
	NULL::bigint AS "Sequence",
	"NestedElement"."RootElement",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("NestedElement_NestedParameter"."NestedParameter",'{}'::text[]) AS "NestedParameter",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("NestedElement_ElementUsage"."ElementUsage",'{}'::text[]) AS "ElementUsage"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."NestedElement_Data"() AS "NestedElement" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "NestedElement" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("ElementUsage"::text)] AS "ElementUsage"
   FROM "SchemaName_Replace"."NestedElement_ElementUsage_Data"() AS "NestedElement_ElementUsage"
   JOIN "SchemaName_Replace"."NestedElement_Data"() AS "NestedElement" ON "NestedElement" = "Iid"
   GROUP BY "NestedElement") AS "NestedElement_ElementUsage" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "NestedParameter"."Container" AS "Iid", array_agg("NestedParameter"."Iid"::text) AS "NestedParameter"
   FROM "SchemaName_Replace"."NestedParameter_Data"() AS "NestedParameter"
   JOIN "SchemaName_Replace"."NestedElement_Data"() AS "NestedElement" ON "NestedParameter"."Container" = "NestedElement"."Iid"
   GROUP BY "NestedParameter"."Container") AS "NestedElement_NestedParameter" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."NestedParameter_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "NestedParameter"."ValueTypeDictionary" AS "ValueTypeSet",
	"NestedParameter"."Container",
	NULL::bigint AS "Sequence",
	"NestedParameter"."AssociatedParameter",
	"NestedParameter"."ActualState",
	"NestedParameter"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."NestedParameter_Data"() AS "NestedParameter" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Publication_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Publication"."ValueTypeDictionary" AS "ValueTypeSet",
	"Publication"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Publication_Domain"."Domain",'{}'::text[]) AS "Domain",
	COALESCE("Publication_PublishedParameter"."PublishedParameter",'{}'::text[]) AS "PublishedParameter"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Publication_Data"() AS "Publication" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Publication" AS "Iid", array_agg("Domain"::text) AS "Domain"
   FROM "SchemaName_Replace"."Publication_Domain_Data"() AS "Publication_Domain"
   JOIN "SchemaName_Replace"."Publication_Data"() AS "Publication" ON "Publication" = "Iid"
   GROUP BY "Publication") AS "Publication_Domain" USING ("Iid")
 LEFT JOIN (SELECT "Publication" AS "Iid", array_agg("PublishedParameter"::text) AS "PublishedParameter"
   FROM "SchemaName_Replace"."Publication_PublishedParameter_Data"() AS "Publication_PublishedParameter"
   JOIN "SchemaName_Replace"."Publication_Data"() AS "Publication" ON "Publication" = "Iid"
   GROUP BY "Publication") AS "Publication_PublishedParameter" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."PossibleFiniteStateList_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PossibleFiniteStateList"."ValueTypeDictionary" AS "ValueTypeSet",
	"PossibleFiniteStateList"."Container",
	NULL::bigint AS "Sequence",
	"PossibleFiniteStateList"."DefaultState",
	"PossibleFiniteStateList"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "PossibleFiniteState"."Container" AS "Iid", ARRAY[array_agg("PossibleFiniteState"."Sequence"::text), array_agg("PossibleFiniteState"."Iid"::text)] AS "PossibleState"
   FROM "SchemaName_Replace"."PossibleFiniteState_Data"() AS "PossibleFiniteState"
   JOIN "SchemaName_Replace"."PossibleFiniteStateList_Data"() AS "PossibleFiniteStateList" ON "PossibleFiniteState"."Container" = "PossibleFiniteStateList"."Iid"
   GROUP BY "PossibleFiniteState"."Container") AS "PossibleFiniteStateList_PossibleState" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."PossibleFiniteState_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PossibleFiniteState"."ValueTypeDictionary" AS "ValueTypeSet",
	"PossibleFiniteState"."Container",
	"PossibleFiniteState"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementBase"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" || "ElementDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementDefinition"."Container",
	NULL::bigint AS "Sequence",
	"ElementBase"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementUsage_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" || "ElementUsage"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementUsage"."Container",
	NULL::bigint AS "Sequence",
	"ElementBase"."Owner",
	"ElementUsage"."ElementDefinition",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ParameterOverride"."Container" AS "Iid", array_agg("ParameterOverride"."Iid"::text) AS "ParameterOverride"
   FROM "SchemaName_Replace"."ParameterOverride_Data"() AS "ParameterOverride"
   JOIN "SchemaName_Replace"."ElementUsage_Data"() AS "ElementUsage" ON "ParameterOverride"."Container" = "ElementUsage"."Iid"
   GROUP BY "ParameterOverride"."Container") AS "ElementUsage_ParameterOverride" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterBase"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterBase"."ParameterType",
	"ParameterBase"."Scale",
	"ParameterBase"."StateDependence",
	"ParameterBase"."Group",
	"ParameterBase"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterBase_Data"() AS "ParameterBase" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterOrOverrideBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterBase"."ValueTypeDictionary" || "ParameterOrOverrideBase"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterBase"."ParameterType",
	"ParameterBase"."Scale",
	"ParameterBase"."StateDependence",
	"ParameterBase"."Group",
	"ParameterBase"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParameterOrOverrideBase_ParameterSubscription"."ParameterSubscription",'{}'::text[]) AS "ParameterSubscription",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterBase_Data"() AS "ParameterBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterSubscription"."Container" AS "Iid", array_agg("ParameterSubscription"."Iid"::text) AS "ParameterSubscription"
   FROM "SchemaName_Replace"."ParameterSubscription_Data"() AS "ParameterSubscription"
   JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" ON "ParameterSubscription"."Container" = "ParameterOrOverrideBase"."Iid"
   GROUP BY "ParameterSubscription"."Container") AS "ParameterOrOverrideBase_ParameterSubscription" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterOverride_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterBase"."ValueTypeDictionary" || "ParameterOrOverrideBase"."ValueTypeDictionary" || "ParameterOverride"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterOverride"."Container",
	NULL::bigint AS "Sequence",
	"ParameterBase"."Owner",
	"ParameterOverride"."Parameter",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParameterOrOverrideBase_ParameterSubscription"."ParameterSubscription",'{}'::text[]) AS "ParameterSubscription",
	COALESCE("ParameterOverride_ValueSet"."ValueSet",'{}'::text[]) AS "ValueSet",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterBase_Data"() AS "ParameterBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterOverride_Data"() AS "ParameterOverride" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterSubscription"."Container" AS "Iid", array_agg("ParameterSubscription"."Iid"::text) AS "ParameterSubscription"
   FROM "SchemaName_Replace"."ParameterSubscription_Data"() AS "ParameterSubscription"
   JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" ON "ParameterSubscription"."Container" = "ParameterOrOverrideBase"."Iid"
   GROUP BY "ParameterSubscription"."Container") AS "ParameterOrOverrideBase_ParameterSubscription" USING ("Iid")
  LEFT JOIN (SELECT "ParameterOverrideValueSet"."Container" AS "Iid", array_agg("ParameterOverrideValueSet"."Iid"::text) AS "ValueSet"
   FROM "SchemaName_Replace"."ParameterOverrideValueSet_Data"() AS "ParameterOverrideValueSet"
   JOIN "SchemaName_Replace"."ParameterOverride_Data"() AS "ParameterOverride" ON "ParameterOverrideValueSet"."Container" = "ParameterOverride"."Iid"
   GROUP BY "ParameterOverrideValueSet"."Container") AS "ParameterOverride_ValueSet" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterSubscription_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterBase"."ValueTypeDictionary" || "ParameterSubscription"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterSubscription"."Container",
	NULL::bigint AS "Sequence",
	"ParameterBase"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParameterSubscription_ValueSet"."ValueSet",'{}'::text[]) AS "ValueSet",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterBase_Data"() AS "ParameterBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterSubscription_Data"() AS "ParameterSubscription" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterSubscriptionValueSet"."Container" AS "Iid", array_agg("ParameterSubscriptionValueSet"."Iid"::text) AS "ValueSet"
   FROM "SchemaName_Replace"."ParameterSubscriptionValueSet_Data"() AS "ParameterSubscriptionValueSet"
   JOIN "SchemaName_Replace"."ParameterSubscription_Data"() AS "ParameterSubscription" ON "ParameterSubscriptionValueSet"."Container" = "ParameterSubscription"."Iid"
   GROUP BY "ParameterSubscriptionValueSet"."Container") AS "ParameterSubscription_ValueSet" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterSubscriptionValueSet_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterSubscriptionValueSet"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterSubscriptionValueSet"."Container",
	NULL::bigint AS "Sequence",
	"ParameterSubscriptionValueSet"."SubscribedValueSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterSubscriptionValueSet_Data"() AS "ParameterSubscriptionValueSet" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterValueSetBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValueSetBase"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterValueSetBase"."ActualState",
	"ParameterValueSetBase"."ActualOption",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValueSetBase_Data"() AS "ParameterValueSetBase" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterOverrideValueSet_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValueSetBase"."ValueTypeDictionary" || "ParameterOverrideValueSet"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterOverrideValueSet"."Container",
	NULL::bigint AS "Sequence",
	"ParameterOverrideValueSet"."ParameterValueSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValueSetBase_Data"() AS "ParameterValueSetBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterOverrideValueSet_Data"() AS "ParameterOverrideValueSet" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Parameter_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterBase"."ValueTypeDictionary" || "ParameterOrOverrideBase"."ValueTypeDictionary" || "Parameter"."ValueTypeDictionary" AS "ValueTypeSet",
	"Parameter"."Container",
	NULL::bigint AS "Sequence",
	"ParameterBase"."ParameterType",
	"ParameterBase"."Scale",
	"ParameterBase"."StateDependence",
	"ParameterBase"."Group",
	"ParameterBase"."Owner",
	"Parameter"."RequestedBy",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParameterOrOverrideBase_ParameterSubscription"."ParameterSubscription",'{}'::text[]) AS "ParameterSubscription",
	COALESCE("Parameter_ValueSet"."ValueSet",'{}'::text[]) AS "ValueSet",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterBase_Data"() AS "ParameterBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" USING ("Iid")
  JOIN "SchemaName_Replace"."Parameter_Data"() AS "Parameter" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterSubscription"."Container" AS "Iid", array_agg("ParameterSubscription"."Iid"::text) AS "ParameterSubscription"
   FROM "SchemaName_Replace"."ParameterSubscription_Data"() AS "ParameterSubscription"
   JOIN "SchemaName_Replace"."ParameterOrOverrideBase_Data"() AS "ParameterOrOverrideBase" ON "ParameterSubscription"."Container" = "ParameterOrOverrideBase"."Iid"
   GROUP BY "ParameterSubscription"."Container") AS "ParameterOrOverrideBase_ParameterSubscription" USING ("Iid")
  LEFT JOIN (SELECT "ParameterValueSet"."Container" AS "Iid", array_agg("ParameterValueSet"."Iid"::text) AS "ValueSet"
   FROM "SchemaName_Replace"."ParameterValueSet_Data"() AS "ParameterValueSet"
   JOIN "SchemaName_Replace"."Parameter_Data"() AS "Parameter" ON "ParameterValueSet"."Container" = "Parameter"."Iid"
   GROUP BY "ParameterValueSet"."Container") AS "Parameter_ValueSet" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterValueSet_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValueSetBase"."ValueTypeDictionary" || "ParameterValueSet"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterValueSet"."Container",
	NULL::bigint AS "Sequence",
	"ParameterValueSetBase"."ActualState",
	"ParameterValueSetBase"."ActualOption",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValueSetBase_Data"() AS "ParameterValueSetBase" USING ("Iid")
  JOIN "SchemaName_Replace"."ParameterValueSet_Data"() AS "ParameterValueSet" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterGroup"."Container",
	NULL::bigint AS "Sequence",
	"ParameterGroup"."ContainingGroup",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterGroup_Data"() AS "ParameterGroup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Behavior_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Behavior"."ValueTypeDictionary" AS "ValueTypeSet",
	"Behavior"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "BehavioralParameter"."Container" AS "Iid", array_agg("BehavioralParameter"."Iid"::text) AS "BehavioralParameter"
   FROM "SchemaName_Replace"."BehavioralParameter_Data"() AS "BehavioralParameter"
   JOIN "SchemaName_Replace"."Behavior_Data"() AS "Behavior" ON "BehavioralParameter"."Container" = "Behavior"."Iid"
   GROUP BY "BehavioralParameter"."Container") AS "Behavior_BehavioralParameter" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."BehavioralParameter_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BehavioralParameter"."ValueTypeDictionary" AS "ValueTypeSet",
	"BehavioralParameter"."Container",
	NULL::bigint AS "Sequence",
	"BehavioralParameter"."Parameter",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Relationship_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Relationship"."ValueTypeDictionary" AS "ValueTypeSet",
	"Relationship"."Container",
	NULL::bigint AS "Sequence",
	"Relationship"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Relationship_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Relationship_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Relationship" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Relationship_Category_Data"() AS "Relationship_Category"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "Relationship" = "Iid"
   GROUP BY "Relationship") AS "Relationship_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RelationshipParameterValue"."Container" AS "Iid", array_agg("RelationshipParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RelationshipParameterValue_Data"() AS "RelationshipParameterValue"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "RelationshipParameterValue"."Container" = "Relationship"."Iid"
   GROUP BY "RelationshipParameterValue"."Container") AS "Relationship_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."MultiRelationship_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Relationship"."ValueTypeDictionary" || "MultiRelationship"."ValueTypeDictionary" AS "ValueTypeSet",
	"Relationship"."Container",
	NULL::bigint AS "Sequence",
	"Relationship"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Relationship_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Relationship_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("MultiRelationship_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" USING ("Iid")
  JOIN "SchemaName_Replace"."MultiRelationship_Data"() AS "MultiRelationship" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Relationship" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Relationship_Category_Data"() AS "Relationship_Category"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "Relationship" = "Iid"
   GROUP BY "Relationship") AS "Relationship_Category" USING ("Iid")
 LEFT JOIN (SELECT "MultiRelationship" AS "Iid", array_agg("RelatedThing"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."MultiRelationship_RelatedThing_Data"() AS "MultiRelationship_RelatedThing"
   JOIN "SchemaName_Replace"."MultiRelationship_Data"() AS "MultiRelationship" ON "MultiRelationship" = "Iid"
   GROUP BY "MultiRelationship") AS "MultiRelationship_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RelationshipParameterValue"."Container" AS "Iid", array_agg("RelationshipParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RelationshipParameterValue_Data"() AS "RelationshipParameterValue"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "RelationshipParameterValue"."Container" = "Relationship"."Iid"
   GROUP BY "RelationshipParameterValue"."Container") AS "Relationship_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParameterValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterValue"."ParameterType",
	"ParameterValue"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValue_Data"() AS "ParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RelationshipParameterValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValue"."ValueTypeDictionary" || "RelationshipParameterValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"RelationshipParameterValue"."Container",
	NULL::bigint AS "Sequence",
	"ParameterValue"."ParameterType",
	"ParameterValue"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValue_Data"() AS "ParameterValue" USING ("Iid")
  JOIN "SchemaName_Replace"."RelationshipParameterValue_Data"() AS "RelationshipParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."BinaryRelationship_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Relationship"."ValueTypeDictionary" || "BinaryRelationship"."ValueTypeDictionary" AS "ValueTypeSet",
	"Relationship"."Container",
	NULL::bigint AS "Sequence",
	"Relationship"."Owner",
	"BinaryRelationship"."Source",
	"BinaryRelationship"."Target",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Relationship_ParameterValue"."ParameterValue",'{}'::text[]) AS "ParameterValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Relationship_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" USING ("Iid")
  JOIN "SchemaName_Replace"."BinaryRelationship_Data"() AS "BinaryRelationship" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Relationship" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Relationship_Category_Data"() AS "Relationship_Category"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "Relationship" = "Iid"
   GROUP BY "Relationship") AS "Relationship_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RelationshipParameterValue"."Container" AS "Iid", array_agg("RelationshipParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RelationshipParameterValue_Data"() AS "RelationshipParameterValue"
   JOIN "SchemaName_Replace"."Relationship_Data"() AS "Relationship" ON "RelationshipParameterValue"."Container" = "Relationship"."Iid"
   GROUP BY "RelationshipParameterValue"."Container") AS "Relationship_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ExternalIdentifierMap_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ExternalIdentifierMap"."ValueTypeDictionary" AS "ValueTypeSet",
	"ExternalIdentifierMap"."Container",
	NULL::bigint AS "Sequence",
	"ExternalIdentifierMap"."ExternalFormat",
	"ExternalIdentifierMap"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ExternalIdentifierMap_Correspondence"."Correspondence",'{}'::text[]) AS "Correspondence",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ExternalIdentifierMap_Data"() AS "ExternalIdentifierMap" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "IdCorrespondence"."Container" AS "Iid", array_agg("IdCorrespondence"."Iid"::text) AS "Correspondence"
   FROM "SchemaName_Replace"."IdCorrespondence_Data"() AS "IdCorrespondence"
   JOIN "SchemaName_Replace"."ExternalIdentifierMap_Data"() AS "ExternalIdentifierMap" ON "IdCorrespondence"."Container" = "ExternalIdentifierMap"."Iid"
   GROUP BY "IdCorrespondence"."Container") AS "ExternalIdentifierMap_Correspondence" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."IdCorrespondence_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "IdCorrespondence"."ValueTypeDictionary" AS "ValueTypeSet",
	"IdCorrespondence"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."IdCorrespondence_Data"() AS "IdCorrespondence" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsContainer"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "RequirementsGroup"."Container" AS "Iid", array_agg("RequirementsGroup"."Iid"::text) AS "Group"
   FROM "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsGroup"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsGroup"."Container") AS "RequirementsContainer_Group" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsContainerParameterValue"."Container" AS "Iid", array_agg("RequirementsContainerParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainerParameterValue"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsContainerParameterValue"."Container") AS "RequirementsContainer_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsSpecification_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" || "RequirementsSpecification"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsSpecification"."Container",
	NULL::bigint AS "Sequence",
	"RequirementsContainer"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RequirementsContainer"."ValueTypeDictionary" || "RequirementsGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsGroup"."Container",
	NULL::bigint AS "Sequence",
	"RequirementsContainer"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "RequirementsGroup"."Container" AS "Iid", array_agg("RequirementsGroup"."Iid"::text) AS "Group"
   FROM "SchemaName_Replace"."RequirementsGroup_Data"() AS "RequirementsGroup"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsGroup"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsGroup"."Container") AS "RequirementsContainer_Group" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsContainerParameterValue"."Container" AS "Iid", array_agg("RequirementsContainerParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue"
   JOIN "SchemaName_Replace"."RequirementsContainer_Data"() AS "RequirementsContainer" ON "RequirementsContainerParameterValue"."Container" = "RequirementsContainer"."Iid"
   GROUP BY "RequirementsContainerParameterValue"."Container") AS "RequirementsContainer_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequirementsContainerParameterValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterValue"."ValueTypeDictionary" || "RequirementsContainerParameterValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"RequirementsContainerParameterValue"."Container",
	NULL::bigint AS "Sequence",
	"ParameterValue"."ParameterType",
	"ParameterValue"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParameterValue_Data"() AS "ParameterValue" USING ("Iid")
  JOIN "SchemaName_Replace"."RequirementsContainerParameterValue_Data"() AS "RequirementsContainerParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."SimpleParameterizableThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "SimpleParameterizableThing"."ValueTypeDictionary" AS "ValueTypeSet",
	"SimpleParameterizableThing"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "SimpleParameterValue"."Container" AS "Iid", array_agg("SimpleParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."SimpleParameterValue_Data"() AS "SimpleParameterValue"
   JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" ON "SimpleParameterValue"."Container" = "SimpleParameterizableThing"."Iid"
   GROUP BY "SimpleParameterValue"."Container") AS "SimpleParameterizableThing_ParameterValue" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Requirement_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "SimpleParameterizableThing"."ValueTypeDictionary" || "Requirement"."ValueTypeDictionary" AS "ValueTypeSet",
	"Requirement"."Container",
	NULL::bigint AS "Sequence",
	"SimpleParameterizableThing"."Owner",
	"Requirement"."Group",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "SimpleParameterValue"."Container" AS "Iid", array_agg("SimpleParameterValue"."Iid"::text) AS "ParameterValue"
   FROM "SchemaName_Replace"."SimpleParameterValue_Data"() AS "SimpleParameterValue"
   JOIN "SchemaName_Replace"."SimpleParameterizableThing_Data"() AS "SimpleParameterizableThing" ON "SimpleParameterValue"."Container" = "SimpleParameterizableThing"."Iid"
   GROUP BY "SimpleParameterValue"."Container") AS "SimpleParameterizableThing_ParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "ParametricConstraint"."Container" AS "Iid", ARRAY[array_agg("ParametricConstraint"."Sequence"::text), array_agg("ParametricConstraint"."Iid"::text)] AS "ParametricConstraint"
   FROM "SchemaName_Replace"."ParametricConstraint_Data"() AS "ParametricConstraint"
   JOIN "SchemaName_Replace"."Requirement_Data"() AS "Requirement" ON "ParametricConstraint"."Container" = "Requirement"."Iid"
   GROUP BY "ParametricConstraint"."Container") AS "Requirement_ParametricConstraint" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."SimpleParameterValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "SimpleParameterValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"SimpleParameterValue"."Container",
	NULL::bigint AS "Sequence",
	"SimpleParameterValue"."ParameterType",
	"SimpleParameterValue"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."SimpleParameterValue_Data"() AS "SimpleParameterValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ParametricConstraint_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParametricConstraint"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParametricConstraint"."Container",
	"ParametricConstraint"."Sequence",
	"ParametricConstraint"."TopExpression",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParametricConstraint_Expression"."Expression",'{}'::text[]) AS "Expression",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ParametricConstraint_Data"() AS "ParametricConstraint" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "BooleanExpression"."Container" AS "Iid", array_agg("BooleanExpression"."Iid"::text) AS "Expression"
   FROM "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression"
   JOIN "SchemaName_Replace"."ParametricConstraint_Data"() AS "ParametricConstraint" ON "BooleanExpression"."Container" = "ParametricConstraint"."Iid"
   GROUP BY "BooleanExpression"."Container") AS "ParametricConstraint_Expression" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."BooleanExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."OrExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" || "OrExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("OrExpression_Term"."Term",'{}'::text[]) AS "Term"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  JOIN "SchemaName_Replace"."OrExpression_Data"() AS "OrExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "OrExpression" AS "Iid", array_agg("Term"::text) AS "Term"
   FROM "SchemaName_Replace"."OrExpression_Term_Data"() AS "OrExpression_Term"
   JOIN "SchemaName_Replace"."OrExpression_Data"() AS "OrExpression" ON "OrExpression" = "Iid"
   GROUP BY "OrExpression") AS "OrExpression_Term" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."NotExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" || "NotExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	"NotExpression"."Term",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  JOIN "SchemaName_Replace"."NotExpression_Data"() AS "NotExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."AndExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" || "AndExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("AndExpression_Term"."Term",'{}'::text[]) AS "Term"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  JOIN "SchemaName_Replace"."AndExpression_Data"() AS "AndExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "AndExpression" AS "Iid", array_agg("Term"::text) AS "Term"
   FROM "SchemaName_Replace"."AndExpression_Term_Data"() AS "AndExpression_Term"
   JOIN "SchemaName_Replace"."AndExpression_Data"() AS "AndExpression" ON "AndExpression" = "Iid"
   GROUP BY "AndExpression") AS "AndExpression_Term" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ExclusiveOrExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" || "ExclusiveOrExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ExclusiveOrExpression_Term"."Term",'{}'::text[]) AS "Term"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  JOIN "SchemaName_Replace"."ExclusiveOrExpression_Data"() AS "ExclusiveOrExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ExclusiveOrExpression" AS "Iid", array_agg("Term"::text) AS "Term"
   FROM "SchemaName_Replace"."ExclusiveOrExpression_Term_Data"() AS "ExclusiveOrExpression_Term"
   JOIN "SchemaName_Replace"."ExclusiveOrExpression_Data"() AS "ExclusiveOrExpression" ON "ExclusiveOrExpression" = "Iid"
   GROUP BY "ExclusiveOrExpression") AS "ExclusiveOrExpression_Term" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RelationalExpression_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "BooleanExpression"."ValueTypeDictionary" || "RelationalExpression"."ValueTypeDictionary" AS "ValueTypeSet",
	"BooleanExpression"."Container",
	NULL::bigint AS "Sequence",
	"RelationalExpression"."ParameterType",
	"RelationalExpression"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."BooleanExpression_Data"() AS "BooleanExpression" USING ("Iid")
  JOIN "SchemaName_Replace"."RelationalExpression_Data"() AS "RelationalExpression" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."FileStore_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "FileStore"."ValueTypeDictionary" AS "ValueTypeSet",
	"FileStore"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("FileStore_Folder"."Folder",'{}'::text[]) AS "Folder",
	COALESCE("FileStore_File"."File",'{}'::text[]) AS "File",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Folder"."Container" AS "Iid", array_agg("Folder"."Iid"::text) AS "Folder"
   FROM "SchemaName_Replace"."Folder_Data"() AS "Folder"
   JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" ON "Folder"."Container" = "FileStore"."Iid"
   GROUP BY "Folder"."Container") AS "FileStore_Folder" USING ("Iid")
  LEFT JOIN (SELECT "File"."Container" AS "Iid", array_agg("File"."Iid"::text) AS "File"
   FROM "SchemaName_Replace"."File_Data"() AS "File"
   JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" ON "File"."Container" = "FileStore"."Iid"
   GROUP BY "File"."Container") AS "FileStore_File" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DomainFileStore_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "FileStore"."ValueTypeDictionary" || "DomainFileStore"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainFileStore"."Container",
	NULL::bigint AS "Sequence",
	"FileStore"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("FileStore_Folder"."Folder",'{}'::text[]) AS "Folder",
	COALESCE("FileStore_File"."File",'{}'::text[]) AS "File",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" USING ("Iid")
  JOIN "SchemaName_Replace"."DomainFileStore_Data"() AS "DomainFileStore" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Folder"."Container" AS "Iid", array_agg("Folder"."Iid"::text) AS "Folder"
   FROM "SchemaName_Replace"."Folder_Data"() AS "Folder"
   JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" ON "Folder"."Container" = "FileStore"."Iid"
   GROUP BY "Folder"."Container") AS "FileStore_Folder" USING ("Iid")
  LEFT JOIN (SELECT "File"."Container" AS "Iid", array_agg("File"."Iid"::text) AS "File"
   FROM "SchemaName_Replace"."File_Data"() AS "File"
   JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" ON "File"."Container" = "FileStore"."Iid"
   GROUP BY "File"."Container") AS "FileStore_File" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Folder_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Folder"."ValueTypeDictionary" AS "ValueTypeSet",
	"Folder"."Container",
	NULL::bigint AS "Sequence",
	"Folder"."Creator",
	"Folder"."ContainingFolder",
	"Folder"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Folder_Data"() AS "Folder" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."File_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "File"."ValueTypeDictionary" AS "ValueTypeSet",
	"File"."Container",
	NULL::bigint AS "Sequence",
	"File"."LockedBy",
	"File"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("File_FileRevision"."FileRevision",'{}'::text[]) AS "FileRevision",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("File_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."File_Data"() AS "File" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "File" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."File_Category_Data"() AS "File_Category"
   JOIN "SchemaName_Replace"."File_Data"() AS "File" ON "File" = "Iid"
   GROUP BY "File") AS "File_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "FileRevision"."Container" AS "Iid", array_agg("FileRevision"."Iid"::text) AS "FileRevision"
   FROM "SchemaName_Replace"."FileRevision_Data"() AS "FileRevision"
   JOIN "SchemaName_Replace"."File_Data"() AS "File" ON "FileRevision"."Container" = "File"."Iid"
   GROUP BY "FileRevision"."Container") AS "File_FileRevision" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."FileRevision_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "FileRevision"."ValueTypeDictionary" AS "ValueTypeSet",
	"FileRevision"."Container",
	NULL::bigint AS "Sequence",
	"FileRevision"."Creator",
	"FileRevision"."ContainingFolder",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("FileRevision_FileType"."FileType",'{}'::text[]) AS "FileType"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."FileRevision_Data"() AS "FileRevision" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "FileRevision" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("FileType"::text)] AS "FileType"
   FROM "SchemaName_Replace"."FileRevision_FileType_Data"() AS "FileRevision_FileType"
   JOIN "SchemaName_Replace"."FileRevision_Data"() AS "FileRevision" ON "FileRevision" = "Iid"
   GROUP BY "FileRevision") AS "FileRevision_FileType" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ActualFiniteStateList_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ActualFiniteStateList"."ValueTypeDictionary" AS "ValueTypeSet",
	"ActualFiniteStateList"."Container",
	NULL::bigint AS "Sequence",
	"ActualFiniteStateList"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ActualFiniteStateList_ActualState"."ActualState",'{}'::text[]) AS "ActualState",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ActualFiniteStateList_PossibleFiniteStateList"."PossibleFiniteStateList",'{}'::text[]) AS "PossibleFiniteStateList",
	COALESCE("ActualFiniteStateList_ExcludeOption"."ExcludeOption",'{}'::text[]) AS "ExcludeOption"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ActualFiniteStateList_Data"() AS "ActualFiniteStateList" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ActualFiniteStateList" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("PossibleFiniteStateList"::text)] AS "PossibleFiniteStateList"
   FROM "SchemaName_Replace"."ActualFiniteStateList_PossibleFiniteStateList_Data"() AS "ActualFiniteStateList_PossibleFiniteStateList"
   JOIN "SchemaName_Replace"."ActualFiniteStateList_Data"() AS "ActualFiniteStateList" ON "ActualFiniteStateList" = "Iid"
   GROUP BY "ActualFiniteStateList") AS "ActualFiniteStateList_PossibleFiniteStateList" USING ("Iid")
 LEFT JOIN (SELECT "ActualFiniteStateList" AS "Iid", array_agg("ExcludeOption"::text) AS "ExcludeOption"
   FROM "SchemaName_Replace"."ActualFiniteStateList_ExcludeOption_Data"() AS "ActualFiniteStateList_ExcludeOption"
   JOIN "SchemaName_Replace"."ActualFiniteStateList_Data"() AS "ActualFiniteStateList" ON "ActualFiniteStateList" = "Iid"
   GROUP BY "ActualFiniteStateList") AS "ActualFiniteStateList_ExcludeOption" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ActualFiniteState"."Container" AS "Iid", array_agg("ActualFiniteState"."Iid"::text) AS "ActualState"
   FROM "SchemaName_Replace"."ActualFiniteState_Data"() AS "ActualFiniteState"
   JOIN "SchemaName_Replace"."ActualFiniteStateList_Data"() AS "ActualFiniteStateList" ON "ActualFiniteState"."Container" = "ActualFiniteStateList"."Iid"
   GROUP BY "ActualFiniteState"."Container") AS "ActualFiniteStateList_ActualState" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ActualFiniteState_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ActualFiniteState"."ValueTypeDictionary" AS "ValueTypeSet",
	"ActualFiniteState"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ActualFiniteState_PossibleState"."PossibleState",'{}'::text[]) AS "PossibleState"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ActualFiniteState_Data"() AS "ActualFiniteState" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ActualFiniteState" AS "Iid", array_agg("PossibleState"::text) AS "PossibleState"
   FROM "SchemaName_Replace"."ActualFiniteState_PossibleState_Data"() AS "ActualFiniteState_PossibleState"
   JOIN "SchemaName_Replace"."ActualFiniteState_Data"() AS "ActualFiniteState" ON "ActualFiniteState" = "Iid"
   GROUP BY "ActualFiniteState") AS "ActualFiniteState_PossibleState" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RuleVerificationList_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "RuleVerificationList"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleVerificationList"."Container",
	NULL::bigint AS "Sequence",
	"RuleVerificationList"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "RuleVerification"."Container" AS "Iid", ARRAY[array_agg("RuleVerification"."Sequence"::text), array_agg("RuleVerification"."Iid"::text)] AS "RuleVerification"
   FROM "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification"
   JOIN "SchemaName_Replace"."RuleVerificationList_Data"() AS "RuleVerificationList" ON "RuleVerification"."Container" = "RuleVerificationList"."Iid"
   GROUP BY "RuleVerification"."Container") AS "RuleVerificationList_RuleVerification" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RuleVerification_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "RuleVerification"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleVerification"."Container",
	"RuleVerification"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RuleVerification_Violation"."Violation",'{}'::text[]) AS "Violation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RuleViolation"."Container" AS "Iid", array_agg("RuleViolation"."Iid"::text) AS "Violation"
   FROM "SchemaName_Replace"."RuleViolation_Data"() AS "RuleViolation"
   JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" ON "RuleViolation"."Container" = "RuleVerification"."Iid"
   GROUP BY "RuleViolation"."Container") AS "RuleVerification_Violation" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."UserRuleVerification_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "RuleVerification"."ValueTypeDictionary" || "UserRuleVerification"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleVerification"."Container",
	"RuleVerification"."Sequence",
	"UserRuleVerification"."Rule",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RuleVerification_Violation"."Violation",'{}'::text[]) AS "Violation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" USING ("Iid")
  JOIN "SchemaName_Replace"."UserRuleVerification_Data"() AS "UserRuleVerification" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RuleViolation"."Container" AS "Iid", array_agg("RuleViolation"."Iid"::text) AS "Violation"
   FROM "SchemaName_Replace"."RuleViolation_Data"() AS "RuleViolation"
   JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" ON "RuleViolation"."Container" = "RuleVerification"."Iid"
   GROUP BY "RuleViolation"."Container") AS "RuleVerification_Violation" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RuleViolation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "RuleViolation"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleViolation"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("RuleViolation_ViolatingThing"."ViolatingThing",'{}'::text[]) AS "ViolatingThing"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."RuleViolation_Data"() AS "RuleViolation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "RuleViolation" AS "Iid", array_agg("ViolatingThing"::text) AS "ViolatingThing"
   FROM "SchemaName_Replace"."RuleViolation_ViolatingThing_Data"() AS "RuleViolation_ViolatingThing"
   JOIN "SchemaName_Replace"."RuleViolation_Data"() AS "RuleViolation" ON "RuleViolation" = "Iid"
   GROUP BY "RuleViolation") AS "RuleViolation_ViolatingThing" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."BuiltInRuleVerification_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "RuleVerification"."ValueTypeDictionary" || "BuiltInRuleVerification"."ValueTypeDictionary" AS "ValueTypeSet",
	"RuleVerification"."Container",
	"RuleVerification"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("RuleVerification_Violation"."Violation",'{}'::text[]) AS "Violation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" USING ("Iid")
  JOIN "SchemaName_Replace"."BuiltInRuleVerification_Data"() AS "BuiltInRuleVerification" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "RuleViolation"."Container" AS "Iid", array_agg("RuleViolation"."Iid"::text) AS "Violation"
   FROM "SchemaName_Replace"."RuleViolation_Data"() AS "RuleViolation"
   JOIN "SchemaName_Replace"."RuleVerification_Data"() AS "RuleVerification" ON "RuleViolation"."Container" = "RuleVerification"."Iid"
   GROUP BY "RuleViolation"."Container") AS "RuleVerification_Violation" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Stakeholder_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Stakeholder"."ValueTypeDictionary" AS "ValueTypeSet",
	"Stakeholder"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Goal_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Goal"."ValueTypeDictionary" AS "ValueTypeSet",
	"Goal"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ValueGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ValueGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"ValueGroup"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."StakeholderValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "StakeholderValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"StakeholderValue"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."StakeHolderValueMap_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "StakeHolderValueMap"."ValueTypeDictionary" AS "ValueTypeSet",
	"StakeHolderValueMap"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "StakeHolderValueMapSettings"."Container" AS "Iid", array_agg("StakeHolderValueMapSettings"."Iid"::text) AS "Settings"
   FROM "SchemaName_Replace"."StakeHolderValueMapSettings_Data"() AS "StakeHolderValueMapSettings"
   JOIN "SchemaName_Replace"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap" ON "StakeHolderValueMapSettings"."Container" = "StakeHolderValueMap"."Iid"
   GROUP BY "StakeHolderValueMapSettings"."Container") AS "StakeHolderValueMap_Settings" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."StakeHolderValueMapSettings_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "StakeHolderValueMapSettings"."ValueTypeDictionary" AS "ValueTypeSet",
	"StakeHolderValueMapSettings"."Container",
	NULL::bigint AS "Sequence",
	"StakeHolderValueMapSettings"."GoalToValueGroupRelationship",
	"StakeHolderValueMapSettings"."ValueGroupToStakeholderValueRelationship",
	"StakeHolderValueMapSettings"."StakeholderValueToRequirementRelationship",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."StakeHolderValueMapSettings_Data"() AS "StakeHolderValueMapSettings" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramThingBase_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagrammingStyle_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagrammingStyle"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagrammingStyle"."FillColor",
	"DiagrammingStyle"."StrokeColor",
	"DiagrammingStyle"."FontColor",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagrammingStyle_UsedColor"."UsedColor",'{}'::text[]) AS "UsedColor",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Color"."Container" AS "Iid", array_agg("Color"."Iid"::text) AS "UsedColor"
   FROM "SchemaName_Replace"."Color_Data"() AS "Color"
   JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" ON "Color"."Container" = "DiagrammingStyle"."Iid"
   GROUP BY "Color"."Container") AS "DiagrammingStyle_UsedColor" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."SharedStyle_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagrammingStyle"."ValueTypeDictionary" || "SharedStyle"."ValueTypeDictionary" AS "ValueTypeSet",
	"SharedStyle"."Container",
	NULL::bigint AS "Sequence",
	"DiagrammingStyle"."FillColor",
	"DiagrammingStyle"."StrokeColor",
	"DiagrammingStyle"."FontColor",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagrammingStyle_UsedColor"."UsedColor",'{}'::text[]) AS "UsedColor",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" USING ("Iid")
  JOIN "SchemaName_Replace"."SharedStyle_Data"() AS "SharedStyle" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Color"."Container" AS "Iid", array_agg("Color"."Iid"::text) AS "UsedColor"
   FROM "SchemaName_Replace"."Color_Data"() AS "Color"
   JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" ON "Color"."Container" = "DiagrammingStyle"."Iid"
   GROUP BY "Color"."Container") AS "DiagrammingStyle_UsedColor" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Color_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "Color"."ValueTypeDictionary" AS "ValueTypeSet",
	"Color"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."Color_Data"() AS "Color" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramElementContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramCanvas_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramCanvas"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramCanvas"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ArchitectureDiagram_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramCanvas"."ValueTypeDictionary" || "ArchitectureDiagram"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramCanvas"."Container",
	NULL::bigint AS "Sequence",
	"ArchitectureDiagram"."TopArchitectureElement",
	"ArchitectureDiagram"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "DiagramElementThing"."Container" AS "Iid", array_agg("DiagramElementThing"."Iid"::text) AS "DiagramElement"
   FROM "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "DiagramElementThing"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "DiagramElementThing"."Container") AS "DiagramElementContainer_DiagramElement" USING ("Iid")
  LEFT JOIN (SELECT "Bounds"."Container" AS "Iid", array_agg("Bounds"."Iid"::text) AS "Bounds"
   FROM "SchemaName_Replace"."Bounds_Data"() AS "Bounds"
   JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" ON "Bounds"."Container" = "DiagramElementContainer"."Iid"
   GROUP BY "Bounds"."Container") AS "DiagramElementContainer_Bounds" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramElementThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramEdge_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramEdge"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	"DiagramEdge"."Source",
	"DiagramEdge"."Target",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagramElementContainer_DiagramElement"."DiagramElement",'{}'::text[]) AS "DiagramElement",
	COALESCE("DiagramElementContainer_Bounds"."Bounds",'{}'::text[]) AS "Bounds",
	COALESCE("DiagramElementThing_LocalStyle"."LocalStyle",'{}'::text[]) AS "LocalStyle",
	COALESCE("DiagramEdge_Point"."Point",'{}'::text[]) AS "Point",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementContainer_Data"() AS "DiagramElementContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramElementThing_Data"() AS "DiagramElementThing" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagramEdge_Data"() AS "DiagramEdge" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "Point"."Container" AS "Iid", ARRAY[array_agg("Point"."Sequence"::text), array_agg("Point"."Iid"::text)] AS "Point"
   FROM "SchemaName_Replace"."Point_Data"() AS "Point"
   JOIN "SchemaName_Replace"."DiagramEdge_Data"() AS "DiagramEdge" ON "Point"."Container" = "DiagramEdge"."Iid"
   GROUP BY "Point"."Container") AS "DiagramEdge_Point" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Bounds_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "Bounds"."ValueTypeDictionary" AS "ValueTypeSet",
	"Bounds"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."Bounds_Data"() AS "Bounds" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."OwnedStyle_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagrammingStyle"."ValueTypeDictionary" || "OwnedStyle"."ValueTypeDictionary" AS "ValueTypeSet",
	"OwnedStyle"."Container",
	NULL::bigint AS "Sequence",
	"DiagrammingStyle"."FillColor",
	"DiagrammingStyle"."StrokeColor",
	"DiagrammingStyle"."FontColor",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DiagrammingStyle_UsedColor"."UsedColor",'{}'::text[]) AS "UsedColor",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" USING ("Iid")
  JOIN "SchemaName_Replace"."OwnedStyle_Data"() AS "OwnedStyle" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Color"."Container" AS "Iid", array_agg("Color"."Iid"::text) AS "UsedColor"
   FROM "SchemaName_Replace"."Color_Data"() AS "Color"
   JOIN "SchemaName_Replace"."DiagrammingStyle_Data"() AS "DiagrammingStyle" ON "Color"."Container" = "DiagrammingStyle"."Iid"
   GROUP BY "Color"."Container") AS "DiagrammingStyle_UsedColor" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Point_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "Point"."ValueTypeDictionary" AS "ValueTypeSet",
	"Point"."Container",
	"Point"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."DiagramThingBase_Data"() AS "DiagramThingBase" USING ("Iid")
  JOIN "SchemaName_Replace"."Point_Data"() AS "Point" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramShape_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramObject_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramObject"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ArchitectureElement_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramObject"."ValueTypeDictionary" || "ArchitectureElement"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramPort_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramPort"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramPort"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiagramFrame_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DiagramThingBase"."ValueTypeDictionary" || "DiagramElementContainer"."ValueTypeDictionary" || "DiagramElementThing"."ValueTypeDictionary" || "DiagramShape"."ValueTypeDictionary" || "DiagramFrame"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiagramElementThing"."Container",
	NULL::bigint AS "Sequence",
	"DiagramElementThing"."DepictedThing",
	"DiagramElementThing"."SharedStyle",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
