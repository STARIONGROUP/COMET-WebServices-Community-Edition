-- Create table for class Attachment (which derives from: Thing)
CREATE TABLE "SiteDirectory"."Attachment" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Attachment_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Attachment" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Attachment
CREATE TABLE "SiteDirectory"."Attachment_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Attachment_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Attachment_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Attachment
CREATE TABLE "SiteDirectory"."Attachment_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Attachment_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AttachmentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Attachment_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Attachment is contained (composite) by DefinedThing: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Attachment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Attachment" ADD CONSTRAINT "Attachment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Attachment_Container" ON "SiteDirectory"."Attachment" ("Container");
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

-- Class Attachment derives from Thing
ALTER TABLE "SiteDirectory"."Attachment" ADD CONSTRAINT "AttachmentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- FileType is a collection property (many to many) of class Attachment: [1..*]-[1..1]
CREATE TABLE "SiteDirectory"."Attachment_FileType" (
  "Attachment" uuid NOT NULL,
  "FileType" uuid NOT NULL,
  CONSTRAINT "Attachment_FileType_PK" PRIMARY KEY("Attachment", "FileType"),
  CONSTRAINT "Attachment_FileType_FK_Source" FOREIGN KEY ("Attachment") REFERENCES "SiteDirectory"."Attachment" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Attachment_FileType_FK_Target" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Attachment_FileType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_FileType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_FileType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_FileType" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Attachment_FileType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Attachment_FileType_ValidFrom" ON "SiteDirectory"."Attachment_FileType" ("ValidFrom");
CREATE INDEX "Idx_Attachment_FileType_ValidTo" ON "SiteDirectory"."Attachment_FileType" ("ValidTo");

CREATE TABLE "SiteDirectory"."Attachment_FileType_Audit" (LIKE "SiteDirectory"."Attachment_FileType");

ALTER TABLE "SiteDirectory"."Attachment_FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Attachment_FileTypeAudit_ValidFrom" ON "SiteDirectory"."Attachment_FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_Attachment_FileTypeAudit_ValidTo" ON "SiteDirectory"."Attachment_FileType_Audit" ("ValidTo");

CREATE TRIGGER Attachment_FileType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER attachment_filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Attachment_FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Attachment', 'SiteDirectory');

ALTER TABLE "SiteDirectory"."Attachment"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Attachment_ValidFrom" ON "SiteDirectory"."Attachment" ("ValidFrom");
CREATE INDEX "Idx_Attachment_ValidTo" ON "SiteDirectory"."Attachment" ("ValidTo");

CREATE TABLE "SiteDirectory"."Attachment_Audit" (LIKE "SiteDirectory"."Attachment");

ALTER TABLE "SiteDirectory"."Attachment_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Attachment_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Attachment_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AttachmentAudit_ValidFrom" ON "SiteDirectory"."Attachment_Audit" ("ValidFrom");
CREATE INDEX "Idx_AttachmentAudit_ValidTo" ON "SiteDirectory"."Attachment_Audit" ("ValidTo");

CREATE TRIGGER Attachment_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Attachment_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Attachment"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();




